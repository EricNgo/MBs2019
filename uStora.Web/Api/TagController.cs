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

        //[Route("getallparents")]
        //[HttpGet]
        //[Authorize(Roles = "ViewUser")]
        //public HttpResponseMessage GetAll(HttpRequestMessage request)
        //{
        //    Func<HttpResponseMessage> func = () =>
        //    {
        //        var model = _productService.GetAll();

        //        var responseData = Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(model);

        //        var response = request.CreateResponse(HttpStatusCode.OK, responseData);
        //        return response;
        //    };
        //    return CreateHttpResponse(request, func);
        //}

        //[Route("manufactors")]
        //[HttpGet]
        //public HttpResponseMessage GetManufactors(HttpRequestMessage request)
        //{
        //    Func<HttpResponseMessage> func = () =>
        //    {
        //        var model = _manufactorService.GetAll();

        //       var response = request.CreateResponse(HttpStatusCode.OK, model);
        //        return response;
        //    };
        //    return CreateHttpResponse(request, func);
        //}

        //[Route("listbrands")]
        //[HttpGet]
        //[Authorize(Roles = "ViewUser")]
        //public HttpResponseMessage ListBrands(HttpRequestMessage request)
        //{
        //    Func<HttpResponseMessage> func = () =>
        //    {
        //        var model = _brandService.GetAll("");

        //        var responseData = Mapper.Map<IEnumerable<Brand>, IEnumerable<BrandViewModel>>(model);

        //        var response = request.CreateResponse(HttpStatusCode.OK, responseData);
        //        return response;
        //    };
        //    return CreateHttpResponse(request, func);
        //}

        //[Route("getbyid/{id:int}")]
        //[HttpGet]
        //[Authorize(Roles = "UpdateUser")]
        //public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        //{
        //    return CreateHttpResponse(request, () =>
        //    {
        //        var model = _productService.FindById(id);

        //        var responseData = Mapper.Map<Product, ProductViewModel>(model);

        //        var response = request.CreateResponse(HttpStatusCode.OK, responseData);

        //        return response;
        //    });
        //}

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

        //[Route("getstock")]
        //[HttpGet]
        //[Authorize(Roles = "ViewUser")]
        //public HttpResponseMessage GetStock(HttpRequestMessage request, int page, int pageSize = 20, string filter = null)
        //{
        //    return CreateHttpResponse(request, () =>
        //    {
        //        int totalRow = 0;
        //        var model = _productService.GetAll(filter);

        //        totalRow = model.Count();
        //        var query = model.OrderByDescending(x => x.CreatedDate).Skip(page * pageSize).Take(pageSize);

        //        var responseData = Mapper.Map<IEnumerable<Product>, IEnumerable<StockViewModel>>(query);

        //        var paginationSet = new PaginationSet<StockViewModel>()
        //        {
        //            Items = responseData,
        //            Page = page,
        //            TotalCount = totalRow,
        //            TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
        //        };

        //         return request.CreateResponse(HttpStatusCode.OK, paginationSet);

        //    });
        //}

        //[Route("updatestock")]
        //[HttpPut]
        //[Authorize(Roles = "ViewUser")]
        //public HttpResponseMessage UpdateStock(HttpRequestMessage request, IList<StockViewModel> models)
        //{
        //    return CreateHttpResponse(request, () =>
        //    {
        //        try
        //        {
        //            foreach (var m in models)
        //            {
        //                var product = _productService.FindById(m.ProductId);
        //                product.Quantity += m.AdjustedQuantity;
        //                if (product.Quantity <= 0)
        //                {
        //                    product.StockStatus = (int)StockStatus.HetHang;
        //                } 
        //                else
        //                {                           
        //                    product.StockStatus = m.StockStatus;
        //                }


        //                _productService.Update(product);
        //            }

        //            _unitOfWork.Commit();

        //            return request.CreateResponse(HttpStatusCode.OK);
        //        }
        //        catch (Exception ex)
        //        {
        //            return request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
        //        }

        //    });
        //}


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

        //[Route("update")]
        //[HttpPut]
        //[AllowAnonymous]
        //[Authorize(Roles = "UpdateUser")]
        //public HttpResponseMessage Update(HttpRequestMessage request, ProductViewModel productVm)
        //{
        //    return CreateHttpResponse(request, () =>
        //    {
        //        HttpResponseMessage response = null;
        //        if (!ModelState.IsValid)
        //        {
        //            response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
        //        }
        //        else
        //        {
        //            var dbProduct = _productService.FindById(productVm.ID);

        //            dbProduct.UpdateProduct(productVm);
        //            dbProduct.UpdatedDate = DateTime.Now;
        //            dbProduct.UpdatedBy = User.Identity.Name;
        //            dbProduct.StockStatus = productVm.Quantity > 0 ? productVm.Quantity.Value : (int)ProductStatus.HetHang;
        //            _productService.Update(dbProduct);
        //            _productService.SaveChanges();

        //            var responseData = Mapper.Map<Product, ProductViewModel>(dbProduct);
        //            response = request.CreateResponse(HttpStatusCode.Created, responseData);
        //        }

        //        return response;
        //    });
        //}

        //[Route("delete")]
        //[HttpDelete]
        //[AllowAnonymous]
        //[Authorize(Roles = "DeleteUser")]
        //public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        //{
        //    return CreateHttpResponse(request, () =>
        //    {
        //        HttpResponseMessage response = null;
        //        if (!ModelState.IsValid)
        //        {
        //            response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
        //        }
        //        else
        //        {
        //            _productService.IsDeleted(id);
        //            response = request.CreateResponse(HttpStatusCode.OK);
        //        }

        //        return response;
        //    });
        //}

        //[Route("deletemulti")]
        //[HttpDelete]
        //[Authorize(Roles = "DeleteUser")]
        //[AllowAnonymous]
        //public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string selectedProducts)
        //{
        //    return CreateHttpResponse(request, () =>
        //    {
        //        HttpResponseMessage response = null;
        //        if (!ModelState.IsValid)
        //        {
        //            response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
        //        }
        //        else
        //        {
        //            var listProductCategory = new JavaScriptSerializer().Deserialize<List<int>>(selectedProducts);
        //            foreach (var item in listProductCategory)
        //            {
        //                _productService.Delete(item);
        //            }

        //            _productService.SaveChanges();

        //            response = request.CreateResponse(HttpStatusCode.OK, listProductCategory.Count);
        //        }

        //        return response;
        //    });
        //}

        #endregion Methods


    }


}