using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers.ViewModels.Product;
using Quickafe.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickafe.Providers
{
    public class ProductProvider : BaseProvider
    {
        public ProductProvider(IQuickafeDbContext context) : base(context)
        {
        }

        public void AddProduct(Product product)
        {
            if (DataContext.Products.Any(p => p.Code == product.Code))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, product.Code));

            DataContext.Products.Add(product);
            SetAuditFields(product);
            DataContext.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            if (DataContext.Tables.Any(t => t.Code == product.Code && t.Id != product.Id))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, product.Code));

            DataContext.Products.Attach(product);
            DataContext.Entry(product).State = EntityState.Modified;
            SetAuditFields(product);
            DataContext.SaveChanges();
        }

        public void DeleteProduct(long productId)
        {
            Product entity = GetProduct(productId);
            DataContext.Products.Remove(entity);
            DataContext.SaveChanges();
        }

        public void DeleteProduct(long[] arrayProductId)
        {
            IEnumerable<Product> entities = DataContext.Products.Where(p => arrayProductId.Contains(p.Id)).ToList();
            DataContext.Products.RemoveRange(entities);
            DataContext.SaveChanges();
        }

        public Product GetProduct(string productCode)
        {
            DataContext.Configuration.ProxyCreationEnabled = false;
            return DataContext.Products.FirstOrDefault(p => p.Code == productCode);
        }

        public Product GetProduct(long productId)
        {
            return DataContext.Products.Single(p => p.Id == productId);
        }


        public IQueryable<ListProductViewModel> ListProducts(bool? finishedGoods = true, bool onlyActive = true, int unitPriceMode = -1)
        {
            IQueryable<ListProductViewModel> query =
                from p in DataContext.Products join 
                     prodCategory in DataContext.ProductCategories on p.ProductCategoryId equals prodCategory.Id
                select new ListProductViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    UnitPrice = unitPriceMode == 1 ? p.UnitPrice : (unitPriceMode == 2 ? p.UnitPrice2 : (unitPriceMode == 3 ? p.UnitPrice3 : p.UnitPrice)),
                    UnitPrice2 = p.UnitPrice2,
                    UnitPrice3 = p.UnitPrice3,
                    IsActive = p.IsActive,
                    ProductCategoryName = prodCategory.Name,
                    CanOrder = prodCategory.CanOrder
                };

            if (finishedGoods.HasValue)
            {
                query = query.Where(p => p.CanOrder == finishedGoods.Value);

                if (onlyActive)
                    query = query.Where(p => p.IsActive);
            }
            return query;
        }

        public IEnumerable<Product> GetProducts(bool onlyActive = true)
        {
            IQueryable<Product> query = DataContext.Products;

            if (onlyActive)
                query = query.Where(p => p.IsActive);

            return query.ToList();
        }

        private bool IsOrderableProductCategory(int productCategoryId)
        {
            ProductCategory productCategory = DataContext.ProductCategories.SingleOrDefault(p => p.Id == productCategoryId);
            return productCategory != null ? productCategory.CanOrder : true;
        }

        public IEnumerable<ListProductViewModel> GetYieldProducts(int productCategoryId)
        {
            IQueryable<ListProductViewModel> query = null;

            DataContext.Configuration.ProxyCreationEnabled = false;
            
            if (!IsOrderableProductCategory(productCategoryId))
            {
                query = from p in DataContext.Products
                        join pCategory in DataContext.ProductCategories on p.ProductCategoryId equals pCategory.Id
                        where pCategory.CanOrder
                        select new ListProductViewModel
                        {
                            Id = p.Id,
                            Name = p.Name
                        };

                return query.ToList();
            }

            return null;
        }        
    }
}
