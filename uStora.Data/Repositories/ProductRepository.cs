﻿using uStora.Data.Infrastructure;
using uStora.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace uStora.Data.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IEnumerable<Product> GetProductsByTag(string tagId, int brandId);

        IEnumerable<Product> GetPromotionProduct();

    }

    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IDbFactory dbFactory)
            : base(dbFactory)
        {
        }

        public IEnumerable<Product> GetProductsByTag(string tagId, int brandId)
        {
            var query = from p in DbContext.Products
                        join pt in DbContext.ProductTags
                        on p.ID equals pt.ProductID
                        where pt.TagID == tagId && p.IsDeleted == false
                        select p;
            if(brandId != 0)
            {
                query = from p in DbContext.Products
                        join brand in DbContext.Brands
                        on p.BrandID equals brand.ID
                        join pt in DbContext.ProductTags
                        on p.ID equals pt.ProductID
                        where pt.TagID == tagId && p.BrandID == brandId && p.IsDeleted == false
                        select p;
            }

            return query;
        }

        public IEnumerable<Product> GetPromotionProduct()
        {
            return DbContext.Database.SqlQuery<Product>("GetPromotionProduct");
        }
    }
}