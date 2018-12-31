using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using uStora.Common;
using uStora.Data.Infrastructure;
using uStora.Model.Models;
using uStora.Service;
using uStora.Web.Infrastructure.Core;
using uStora.Web.Infrastructure.Extensions;
using uStora.Web.Models;

namespace uStora.Web.Api
{
    [RoutePrefix("api/trackorder")]
    [Authorize]
    public class TrackOrderController : ApiControllerBase
    {
        #region init
        ITrackOrderService _trackOrderService;
        IOrderService _orderService;

        IUnitOfWork _unitOfWork;
        IApplicationUserService _appUserService;
        public TrackOrderController(IErrorService errorService,
            ITrackOrderService trackOrderService,
            IUnitOfWork unitOfWork,
            IOrderService orderService,
    
            IApplicationUserService appUserService) : base(errorService)
        {
            _trackOrderService = trackOrderService;
            _unitOfWork = unitOfWork;
            _orderService = orderService;

            _appUserService = appUserService;
        }
        #endregion
        [Route("getbyid/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _trackOrderService.FindById(id);

                var responseData = Mapper.Map<TrackOrder, TrackOrderViewModel>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);

                return response;
            });
        }

        [Route("getlonglatbyorderid/{id:int}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetLongLatByOrderId(HttpRequestMessage request,int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _trackOrderService.GetLongLatByOrderId(id);
                var responseData = Mapper.Map<IEnumerable<TrackOrder>, IEnumerable<TrackOrderViewModel>>(model);
                //foreach (var id in listDrivers)
                //{
                //    var user = _appUserService.GetUserById(id);
                //    userVm.Add(Mapper.Map<ApplicationUser, ApplicationUserViewModel>(user));
                //}
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [Route("getorders")]
        [HttpGet]
        public HttpResponseMessage GetOrders(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var listOrders = _orderService.GetUnCompletedOrder();
                var responseData = Mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(listOrders);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }


        [Route("getdriver")]
        [HttpGet]
        public HttpResponseMessage GetDrivers(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var listDrivers = _appUserService.GetUserIdByGroupId(CommonConstants.DriverId);
                List<ApplicationUserViewModel> userVm = new List<ApplicationUserViewModel>();
                foreach (var id in listDrivers)
                {
                    var user = _appUserService.GetUserById(id);
                    userVm.Add(Mapper.Map<ApplicationUser, ApplicationUserViewModel>(user));
                }
                var response = request.CreateResponse(HttpStatusCode.OK, userVm);
                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string keyword, int page, int pageSize = 20)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                HttpResponseMessage response = null;
                var listTrackOrders = _trackOrderService.GetAll(keyword);
                totalRow = listTrackOrders.Count();
                var query = listTrackOrders.OrderByDescending(x => x.OrderId).Skip(page * pageSize).Take(pageSize);
                var responseData = Mapper.Map<IEnumerable<TrackOrder>, IEnumerable<TrackOrderViewModel>>(listTrackOrders);
                var paginationSet = new PaginationSet<TrackOrderViewModel>()
                {
                    Items = responseData,
                    Page = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
                };
                response = request.CreateResponse(HttpStatusCode.OK, paginationSet);
                return response;
            });
        }
        [Route("create")]
        [HttpPost]
        [Authorize(Roles = "AddUser")]
        public HttpResponseMessage Create(HttpRequestMessage request, TrackOrderViewModel trackOrderVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var newTrackOrder = new TrackOrder();
                    newTrackOrder.UpdateTrackOrder(trackOrderVm);
                    newTrackOrder.Status = true;
                    _trackOrderService.Add(newTrackOrder);
                    var order = _orderService.FindById(newTrackOrder.OrderId);
                    order.PaymentStatus = 1;
                    _orderService.Update(order);
                    _trackOrderService.SaveChanges();

                    var responseData = Mapper.Map<TrackOrder, TrackOrderViewModel>(newTrackOrder);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        [Authorize(Roles = "UpdateTrackOrder")]
        public HttpResponseMessage Update(HttpRequestMessage request, TrackOrderViewModel TrackOrderVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var dbTrackOrder = _trackOrderService.FindById(TrackOrderVm.ID);
                    if (TrackOrderVm.isDone)
                    {
                        var order = _orderService.FindById(dbTrackOrder.OrderId);
                        order.PaymentStatus = 2;
                        TrackOrderVm.Status = false;
                        _orderService.Update(order);
                    }
                    dbTrackOrder.UpdateTrackOrder(TrackOrderVm);
                    _trackOrderService.Update(dbTrackOrder);
                    _trackOrderService.SaveChanges();

                    var responseData = Mapper.Map<TrackOrder, TrackOrderViewModel>(dbTrackOrder);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("updatepickup")]
        [HttpPut]
        //[AllowAnonymous]
        [Authorize(Roles = "UpdateTrackOrder")]
        public HttpResponseMessage UpdatePickup(HttpRequestMessage request, TrackOrderViewModel TrackOrderVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var dbTrackOrder = _trackOrderService.FindById(TrackOrderVm.ID);
                    dbTrackOrder.UpdateTrackOrder(TrackOrderVm);
                    dbTrackOrder.CreatedDate  = DateTime.Now;
                    dbTrackOrder.StatusPickup = true;
                    _trackOrderService.Update(dbTrackOrder);
                    _trackOrderService.SaveChanges();

                    var responseData = Mapper.Map<TrackOrder, TrackOrderViewModel>(dbTrackOrder);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }


        [Route("delete")]
        [HttpDelete]
        [Authorize(Roles = "DeleteUser")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var oldTrackOrder = _trackOrderService.FindById(id);
                    _trackOrderService.Delete(oldTrackOrder.ID);
                    _trackOrderService.SaveChanges();

                    var responseData = Mapper.Map<TrackOrder, TrackOrderViewModel>(oldTrackOrder);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string selectedTrackOrders)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var listTrackOrders = new JavaScriptSerializer().Deserialize<List<int>>(selectedTrackOrders);
                    foreach (var item in listTrackOrders)
                    {
                        _trackOrderService.Delete(item);
                    }

                    _trackOrderService.SaveChanges();

                    response = request.CreateResponse(HttpStatusCode.OK, listTrackOrders.Count);
                }

                return response;
            });
        }
    }
}
