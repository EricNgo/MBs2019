using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using uStora.Common;
using uStora.Data.Infrastructure;
using uStora.Model.Models;
using uStora.Service;
using uStora.Service.ExportImport;
using uStora.Web.Infrastructure.Core;
using uStora.Web.Infrastructure.Extensions;
using uStora.Web.Models;
using static uStora.Web.Infrastructure.Extensions.StockStatusHelper;

namespace uStora.Web.API
{
    [RoutePrefix("api/tag")]
    public class TagController : ApiControllerBase
    {
        #region Initialize

        private ITagService _tagService;
        private IBrandService _brandService;
        private IManufactorService _manufactorService;
        private IExportManagerService _exportManager;
        private IImportManagerService _importManager;
        private IUnitOfWork _unitOfWork;

        public TagController(IErrorService errorService,
            IManufactorService manufactorService,
            ITagService tagService, IBrandService brandService,
            IExportManagerService exportManager,
            IUnitOfWork unitOfWork,
            IImportManagerService importManager)
            : base(errorService)
        {
            _manufactorService = manufactorService;
            _tagService = tagService;
            _brandService = brandService;
            _exportManager = exportManager;
            _importManager = importManager;
            _unitOfWork = unitOfWork;
        }

        #endregion Initialize

        #region Methods

        [Route("getallparents")]
        [HttpGet]
        //[Authorize(Roles = "ViewUser")]
        [AllowAnonymous]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            Func<HttpResponseMessage> func = () =>
            {
                var model = _tagService.GetAll();

                var responseData = Mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            };
            return CreateHttpResponse(request, func);
        }


        [Route("getbyid/{id}")]
        [HttpGet]
        //[Authorize(Roles = "UpdateUser")]
        [AllowAnonymous]
        public HttpResponseMessage GetById(HttpRequestMessage request, string id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _tagService.FindByString(id);

                var responseData = Mapper.Map<Tag, TagViewModel>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);

                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        [AllowAnonymous]
        //[Authorize(Roles = "ViewUser")]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int page, int pageSize = 20, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                var model = _tagService.GetAll(filter);

                totalRow = model.Count();
                var query = model.OrderByDescending(x => x.TagCategoryID).Skip(page * pageSize).Take(pageSize);

                var responseData = Mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(query);

                var paginationSet = new PaginationSet<TagViewModel>()
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
        //[Authorize(Roles = "AddUser")]
        public HttpResponseMessage Create(HttpRequestMessage request, TagViewModel tagVm)
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
                    var newTag = new Tag();
                    newTag.UpdateTag(tagVm);
        
                    _tagService.Add(newTag);
                    _tagService.SaveChanges();

                    var responseData = Mapper.Map<Tag, TagViewModel>(newTag);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        [AllowAnonymous]
        //[Authorize(Roles = "UpdateUser")]
        public HttpResponseMessage Update(HttpRequestMessage request, TagViewModel tagVm)
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
                    var dbTag = _tagService.FindByString(tagVm.ID);

                    dbTag.UpdateTag(tagVm);
                  
                    _tagService.Update(dbTag);
                    _tagService.SaveChanges();

                    var responseData = Mapper.Map<Tag, TagViewModel>(dbTag);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("delete")]
        [HttpDelete]
        [AllowAnonymous]
        //[Authorize(Roles = "DeleteUser")]
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
                    _tagService.IsDeleted(id);
                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        //[Authorize(Roles = "DeleteUser")]
        [AllowAnonymous]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string selectedTags)
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
                    var listTag= new JavaScriptSerializer().Deserialize<List<int>>(selectedTags);
                    foreach (var item in listTag)
                    {
                        _tagService.Delete(item);
                    }

                    _tagService.SaveChanges();

                    response = request.CreateResponse(HttpStatusCode.OK, listTag.Count);
                }

                return response;
            });
        }

        #endregion Methods


    }


}