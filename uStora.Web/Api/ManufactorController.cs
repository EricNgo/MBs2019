using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using uStora.Model.Models;
using uStora.Service;
using uStora.Web.Infrastructure.Core;

namespace uStora.Web.Api
{
    [RoutePrefix("api/manufactor")]
    [Authorize]
    public class ManufactorController : ApiControllerBase
    {
        private IManufactorService _manuFactorService;

        public ManufactorController(IErrorService errorService, IManufactorService manuFactorService) : base(errorService)
        {
            _manuFactorService = manuFactorService;
        }

        [Route("getbyid/{id:int}")]
        [HttpGet]
        [Authorize(Roles = "UpdateUser, Admin")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _manuFactorService.FindById(id);

                var response = request.CreateResponse(HttpStatusCode.OK, model);

                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        [Authorize(Roles = "ViewUser, Admin")]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string keyword, int page, int pageSize = 20)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                var model = _manuFactorService.GetAll(keyword);

                totalRow = model.Count();

                var query = model.OrderByDescending(x => x.Id).Skip(page * pageSize).Take(pageSize);


                var paginationSet = new PaginationSet<Manufactor>()
                {
                    Items = query,
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
        [Authorize(Roles = "AddUser, Admin")]
        public HttpResponseMessage Create(HttpRequestMessage request, Manufactor viewmodel)
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
                    var entity = new Manufactor
                    {
                        Name = viewmodel.Name,
                        LogoUrl = viewmodel.LogoUrl
                    };
                    entity = _manuFactorService.Add(entity);
                    _manuFactorService.SaveChanges();

                    response = request.CreateResponse(HttpStatusCode.Created, entity);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        [AllowAnonymous]
        [Authorize(Roles = "UpdateUser, Admin")]
        public HttpResponseMessage Update(HttpRequestMessage request, Manufactor vm)
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
                    var manufactor = _manuFactorService.FindById(vm.Id);

                    manufactor.Name = vm.Name;
                    manufactor.LogoUrl = vm.LogoUrl;

                    _manuFactorService.Update(manufactor);
                    _manuFactorService.SaveChanges();

                    response = request.CreateResponse(HttpStatusCode.Created,
                        new
                        {
                            manufactor.Id,
                            manufactor.Name,
                            manufactor.LogoUrl
                        });
                }

                return response;
            });
        }

        [Route("delete")]
        [HttpDelete]
        [AllowAnonymous]
        [Authorize(Roles = "DeleteUser, Admin")]
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
                    var manufactor = _manuFactorService.FindById(id);
                    _manuFactorService.Delete(manufactor.Id);
                    _manuFactorService.SaveChanges();
                                       
                    response = request.CreateResponse(HttpStatusCode.Created, manufactor);
                }

                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [AllowAnonymous]
        [Authorize(Roles = "DeleteUser, Admin")]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string selectedManufactors)
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
                    var listManus = new JavaScriptSerializer().Deserialize<List<int>>(selectedManufactors);
                    foreach (var item in listManus)
                    {
                        _manuFactorService.Delete(item);
                    }

                    _manuFactorService.SaveChanges();

                    response = request.CreateResponse(HttpStatusCode.OK, listManus.Count);
                }

                return response;
            });
        }
    }
}