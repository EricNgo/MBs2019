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


        //[Route("create")]
        //[HttpPost]
        //[AllowAnonymous]
        //[Authorize(Roles = "AddUser")]
        //public HttpResponseMessage Create(HttpRequestMessage request, ProductViewModel productVm)
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
        //            var newProduct = new Product();
        //            newProduct.UpdateProduct(productVm);
        //            newProduct.StockStatus = productVm.Quantity > 0 ? productVm.Quantity.Value : (int)ProductStatus.HetHang;
        //            newProduct.CreatedDate = DateTime.Now;
        //            newProduct.CreatedBy = User.Identity.Name;
        //            _productService.Add(newProduct);
        //            _productService.SaveChanges();

        //            var responseData = Mapper.Map<Product, ProductViewModel>(newProduct);
        //            response = request.CreateResponse(HttpStatusCode.Created, responseData);
        //        }

        //        return response;
        //    });
        //}

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