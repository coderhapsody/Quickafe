using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Resources;

namespace Quickafe.Providers 
{
	public class ProductCategoryProvider : BaseProvider
	{
		public ProductCategoryProvider(IQuickafeDbContext context) : base(context)
        {
        }

        public void AddProductCategory(ProductCategory productCategory)
        {
            if (DataContext.ProductCategories.Any(cat => cat.Name == productCategory.Name))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, productCategory.Name));

            DataContext.ProductCategories.Add(productCategory);
            SetAuditFields(productCategory);
            DataContext.SaveChanges();
        }

        public void UpdateProductCategory(ProductCategory productCategory)
        {
            if(DataContext.ProductCategories.Any(cat => cat.Name == productCategory.Name && cat.Id != productCategory.Id))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, productCategory.Name));

            DataContext.ProductCategories.Attach(productCategory);
            DataContext.Entry(productCategory).State = EntityState.Modified;
            SetAuditFields(productCategory);
            DataContext.SaveChanges();
        }

        public void DeleteProductCategory(long productCategoryId)
        {
            ProductCategory productCategory = GetProductCategory(productCategoryId);
            DataContext.ProductCategories.Remove(productCategory);
            DataContext.SaveChanges();
        }

        public void DeleteProductCategory(long[] arrayProductCategoryId)
        {
            IEnumerable<ProductCategory> productcategories = DataContext.ProductCategories.Where(it => arrayProductCategoryId.Contains(it.Id)).ToList();
            DataContext.ProductCategories.RemoveRange(productcategories);
            DataContext.SaveChanges();
        }

        public ProductCategory GetProductCategory(long productCategoryId)
        {
            return DataContext.ProductCategories.Single(entity => entity.Id == productCategoryId);
        }

        public IEnumerable<ProductCategory> GetProductCategories(bool onlyActive = false)
        {
            DataContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<ProductCategory> query = DataContext.ProductCategories;

            if (onlyActive)
                query = query.Where(it => it.IsActive);

            return query.ToList();
        }
	}
}
