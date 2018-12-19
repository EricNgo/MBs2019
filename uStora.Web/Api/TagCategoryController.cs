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
    [RoutePrefix("api/tagcategory")]
    public class TagCategoryController : ApiControllerBase
    {
        #region Initialize

        private ITagCategoryService _tagCategoryService;
        private IBrandService _brandService;
        private IManufactorService _manufactorService;
        private IExportManagerService _exportManager;
        private IImportManagerService _importManager;
        private IUnitOfWork _unitOfWork;

        public TagCategoryController(IErrorService errorService,
            IManufactorService manufactorService,
            ITagCategoryService tagCategoryService, IBrandService brandService,
            IExportManagerService exportManager,
            IUnitOfWork unitOfWork,
            IImportManagerService importManager)
            : base(errorService)
        {
            _manufactorService = manufactorService;
            _tagCategoryService = tagCategoryService;
            _brandService = brandService;
            _exportManager = exportManager;
            _importManager = importManager;
            _unitOfWork = unitOfWork;
        }

        #endregion Initialize

        #region Methods

        [Route("getallparents")]
        [HttpGet]
        [AllowAnonymous]
        //[Authorize(Roles = "ViewUser")]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _tagCategoryService.GetAll();

                var responseData = Mapper.Map<IEnumerable<TagCategory>, IEnumerable<TagCategoryViewModel>>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

     

        [Route("getbyid/{id:int}")]
        [HttpGet]
        //[Authorize(Roles = "UpdateUser")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _tagCategoryService.FindById(id);

                var responseData = Mapper.Map<TagCategory, TagCategoryViewModel>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);

                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        //[Authorize(Roles = "ViewUser")]
        [AllowAnonymous]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int page, int pageSize = 20, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                var model = _tagCategoryService.GetAll(filter);

                totalRow = model.Count();
                var query = model.OrderByDescending(x => x.Name).Skip(page * pageSize).Take(pageSize);

                var responseData = Mapper.Map<IEnumerable<TagCategory>, IEnumerable<TagCategoryViewModel>>(query);

                var paginationSet = new PaginationSet<TagCategoryViewModel>()
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
        public HttpResponseMessage Create(HttpRequestMessage request, TagCategoryViewModel tagCategoryVm)
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
                    var newTagCategory = new TagCategory();
                    newTagCategory.UpdateTagCategory(tagCategoryVm);
                 
                    _tagCategoryService.Add(newTagCategory);
                    _tagCategoryService.SaveChanges();

                    var responseData = Mapper.Map<TagCategory, TagCategoryViewModel>(newTagCategory);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        [AllowAnonymous]
        //[Authorize(Roles = "UpdateUser")]
        public HttpResponseMessage Update(HttpRequestMessage request, TagCategoryViewModel tagCategoryVm)
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
                    var dbTagCategory = _tagCategoryService.FindById(tagCategoryVm.ID);

                    dbTagCategory.UpdateTagCategory(tagCategoryVm);
                  
                    _tagCategoryService.Update(dbTagCategory);
                    _tagCategoryService.SaveChanges();

                    var responseData = Mapper.Map<TagCategory, TagCategoryViewModel>(dbTagCategory);
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
                    _tagCategoryService.IsDeleted(id);
                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [Authorize(Roles = "DeleteUser")]
        [AllowAnonymous]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string selectedTagCategories)
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
                    var listTagCategory = new JavaScriptSerializer().Deserialize<List<int>>(selectedTagCategories);
                    foreach (var item in listTagCategory)
                    {
                        _tagCategoryService.Delete(item);
                    }

                    _tagCategoryService.SaveChanges();

                    response = request.CreateResponse(HttpStatusCode.OK, listTagCategory.Count);
                }

                return response;
            });
        }

        #endregion Methods


    }


}