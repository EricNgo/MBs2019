using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using uStora.Model.Models;
using uStora.Service;
using uStora.Web.Infrastructure.Core;
using uStora.Web.Infrastructure.Extensions;
using System.Web.Script.Serialization;
using uStora.Web.Models;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using uStora.Common;
using uStora.Service.ExportImport;

namespace uStora.Web.Api
{
    [RoutePrefix("api/order")]
    [Authorize]
    public class OderController : ApiControllerBase
    {
        private readonly IOrderService _orderService;
        public OderController(IOrderService orderService, IErrorService errorService) : base(errorService)
        {
            _orderService = orderService;
        }

        [Route("getbyid/{id:int}/{rm?}")]
        [HttpGet]
        [Authorize(Roles = "UpdateUser")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id, bool rm = false)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _orderService.FindById(id);

                if (rm) model = _orderService.FindWithRelationData(id, related: true);

                var responseData = Mapper.Map<Order, OrderViewModel>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);

                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        [Authorize(Roles = "ViewUser")]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string keyword, int page, int pageSize = 20)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                var model = _orderService.GetAll(keyword);

                totalRow = model.Count();
                var query = model.OrderByDescending(x => x.CreatedDate).Skip(page * pageSize).Take(pageSize);

                var responseData = Mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(query.AsEnumerable());

                var paginationSet = new PaginationSet<OrderViewModel>()
                {
                    Items = responseData,
                    Page = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
                };
                var response = request.CreateResponse(HttpStatusCode.OK, paginationSet);
                return response;
            });
        }

        [Route("create")]
        [HttpPost]
        [AllowAnonymous]
        [Authorize(Roles = "AddUser")]
        public HttpResponseMessage Create(HttpRequestMessage request, OrderViewModel OrderVm)
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
                    var newOrder = new Order();
                    newOrder.UpdateOrder(OrderVm);
                    newOrder.CreatedDate = DateTime.Now;
                    newOrder.CreatedBy = User.Identity.Name;
                    _orderService.Add(newOrder);
                    _orderService.SaveChanges();

                    var responseData = Mapper.Map<Order, OrderViewModel>(newOrder);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        [AllowAnonymous]
        [Authorize(Roles = "UpdateUser")]
        public HttpResponseMessage Update(HttpRequestMessage request, OrderViewModel OrderVm)
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
                    var dbOrder = _orderService.FindById(OrderVm.ID);

                    dbOrder.UpdateOrder(OrderVm);
                    _orderService.Update(dbOrder);
                    _orderService.SaveChanges();

                    var responseData = Mapper.Map<Order, OrderViewModel>(dbOrder);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("delete")]
        [HttpDelete]
        [AllowAnonymous]
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
                    var oldOrder = _orderService.FindById(id);
                    _orderService.Delete(oldOrder.ID);
                    _orderService.SaveChanges();

                    var responseData = Mapper.Map<Order, OrderViewModel>(oldOrder);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [AllowAnonymous]
        [Authorize(Roles = "DeleteUser")]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string selectedOrders)
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
                    var listOrders = new JavaScriptSerializer().Deserialize<List<int>>(selectedOrders);
                    foreach (var item in listOrders)
                    {
                        _orderService.Delete(item);
                    }

                    _orderService.SaveChanges();

                    response = request.CreateResponse(HttpStatusCode.OK, listOrders.Count);
                }

                return response;
            });
        }

        [Route("exporttoexcel/{id:int}")]
        [Authorize(Roles = "AddUser")]
        [HttpGet]
        public async Task<HttpResponseMessage> ExportProductsToXlsx(HttpRequestMessage request, int id)
        {
            string fileName = string.Concat("Order_" + DateTime.Now.ToString("yyyyMMddhhmmsss") + ".xlsx");
            var folderReport = @"/Reports";
            string filePath = HttpContext.Current.Server.MapPath(folderReport);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fullPath = Path.Combine(filePath, fileName);
            try
            {
                var order = _orderService.GetAll().Where(x => x.ID == id).ToList();
                var source = Mapper.Map<List<Order>, List<OrderReportViewModel>>(order);
                await _orderService.GenerateXls(source, fullPath);
                return request.CreateErrorResponse(HttpStatusCode.OK, Path.Combine(folderReport, fileName));
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
