using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers.Inventory.ViewModels.InventoryOut;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Quickafe.Providers.Inventory
{
    public class InventoryOutProvider : BaseProvider
    {
        private readonly IMapper mapper;
        private readonly AutoNumberProvider autoNumberProvider;
        private readonly InventoryInProvider inventoryInProvider;

        public InventoryOutProvider(IQuickafeDbContext context, IMapper mapper, AutoNumberProvider autoNumberProvider, InventoryInProvider inventoryInProvider) 
            : base(context)
        {
            this.mapper = mapper;
            this.autoNumberProvider = autoNumberProvider;
            this.inventoryInProvider = inventoryInProvider;
        }

        public IQueryable<ListInventoryViewModel> GetInventory()
        {
            IQueryable<ListInventoryViewModel> query =
                from inventory in DataContext.Inventories
                .Where(m => m.Direction == "O")
                .OrderByDescending(m => m.InventoryDate)
                .ThenByDescending(m => m.InventoryNo)
                select new ListInventoryViewModel
                {
                    Id = inventory.Id,
                    InventoryNo = inventory.InventoryNo,
                    InventoryDate = inventory.InventoryDate,
                    Direction = inventory.Direction,
                    InventoryType = inventory.InventoryType == "S" ? "Semi Finished Goods" : "Finished Goods",
                    MutationType = inventory.MutationType,
                    Notes = inventory.Notes,
                    PostedWhen = inventory.PostedWhen,
                    PostedBy = inventory.PostedBy
                };

            return query;
        }

        public Quickafe.DataAccess.Inventory GetInventory(long inventoryId)
        {
            return DataContext.Inventories.Single(entity => entity.Id == inventoryId);
        }

        public IEnumerable<InventoryDetail> GetInventoryDetail(long inventoryId)
        {
            return DataContext.InventoryDetails.Where(o => o.InventoryId == inventoryId);
        }

        internal void AssignInventoryOutNumbering(DataAccess.Inventory inventory)
        {
            long cnt = autoNumberProvider.GetLastNumber("INV-OUT", inventory.InventoryDate.Year, inventory.InventoryDate.Month);
            string gen = cnt.ToString("00000");
            string inventoryNo = String.Format("{0}/{1}/{2}/{3}",
                "INV-OUT",
                inventory.InventoryDate.Year,
                inventory.InventoryDate.Month.ToString("00"),
                gen);
            inventory.InventoryNo = inventoryNo;
            autoNumberProvider.IncrementLastNumber("INV-OUT", inventory.InventoryDate.Year, inventory.InventoryDate.Month);
        }

        public void AddInventory(Quickafe.DataAccess.Inventory inventory)
        {
            AssignInventoryOutNumbering(inventory);

            SetAuditFields(inventory);
            DataContext.Inventories.Add(inventory);
            DataContext.SaveChanges();
        }

        public void UpdateInventory(Quickafe.DataAccess.Inventory inventory)
        {
            SetAuditFields(inventory);
            var oldInventoryDetail = DataContext.InventoryDetails.Where(o => o.InventoryId == inventory.Id).ToList();
            DataContext.InventoryDetails.RemoveRange(oldInventoryDetail);
            foreach (InventoryDetail inventoryDetail in inventory.InventoryDetails)
                DataContext.InventoryDetails.Add(inventoryDetail);
            DataContext.SaveChanges();
        }

        public void DeleteInventory(long id)
        {
            var entityDtl = GetInventoryDetail(id);
            DataContext.InventoryDetails.RemoveRange(entityDtl);
            Quickafe.DataAccess.Inventory entity = GetInventory(id);
            DataContext.Inventories.Remove(entity);
            DataContext.SaveChanges();
        }

        public long GetLocationId() => 1;

        public long GetUOMId() => 1;

        public void PostFromSales(Payment payment)
        {
            DataAccess.Inventory orderInventory = new DataAccess.Inventory();
            orderInventory.LocationId = 1;
            orderInventory.Direction = "O";
            orderInventory.RefNo = payment.PaymentNo;
            orderInventory.InventoryDate = DateTime.Today;
            orderInventory.InventoryType = "F";
            orderInventory.MutationType = "SALES";
            orderInventory.PostedWhen = DateTime.Now;
            orderInventory.PostedBy = CurrentUserName;
            Order order = payment.Order;
            foreach (OrderDetail detail in order.OrderDetails)
            {
                InventoryDetail orderInventoryDetail = new InventoryDetail();
                orderInventoryDetail.ProductId = detail.ProductId;
                orderInventoryDetail.Qty = detail.Qty;
                orderInventoryDetail.UomId = 1;
                orderInventoryDetail.NotesDtl = $"Payment {payment.PaymentNo}, Posting for Order {order.OrderNo}, OrderDetailId={detail.Uid}";

                orderInventory.InventoryDetails.Add(orderInventoryDetail);
            }
            AssignInventoryOutNumbering(orderInventory);
            SetAuditFields(orderInventory, "SYSTEM");
            DataContext.Inventories.Add(orderInventory);
            //DataContext.SaveChanges();
        }

        public void UnPostFromSales(Payment payment)
        {
            DataAccess.Inventory unpostSalesInventory = new DataAccess.Inventory();
            unpostSalesInventory.LocationId = 1;
            unpostSalesInventory.Direction = "I";
            unpostSalesInventory.RefNo = payment.Order.OrderNo;
            unpostSalesInventory.InventoryDate = DateTime.Today;
            unpostSalesInventory.InventoryType = "F";
            unpostSalesInventory.MutationType = "ADJUSTMENT";
            unpostSalesInventory.PostedWhen = DateTime.Now;
            unpostSalesInventory.PostedBy = CurrentUserName;
            Order order = payment.Order;
            foreach (OrderDetail detail in order.OrderDetails)
            {
                InventoryDetail unpostSalesInventoryDetail = new InventoryDetail();
                unpostSalesInventoryDetail.ProductId = detail.ProductId;
                unpostSalesInventoryDetail.Qty = detail.Qty;
                unpostSalesInventoryDetail.UomId = 1;
                unpostSalesInventoryDetail.NotesDtl = $"Payment {payment.PaymentNo}, Void Payment for Order {order.OrderNo}, OrderDetailId={detail.Uid}";

                unpostSalesInventory.InventoryDetails.Add(unpostSalesInventoryDetail);
            }
            inventoryInProvider.AssignInventoryInNumbering(unpostSalesInventory);
            SetAuditFields(unpostSalesInventory, "SYSTEM");
            DataContext.Inventories.Add(unpostSalesInventory);
            //DataContext.SaveChanges();
        }

        public void PostOutInventory(long[] arrayInventoryId)
        {
            IEnumerable<DataAccess.Inventory> inventories = DataContext.Inventories.Where(inv => inv.Direction == "O" && arrayInventoryId.Contains(inv.Id));
            foreach (DataAccess.Inventory inventory in inventories)
            {
                inventory.PostedWhen = DateTime.Now;
                inventory.PostedBy = CurrentUserName;

                if (inventory.InventoryType == "S")
                {
                    if (inventory.MutationType == "PROCESS")
                    {
                        IList<InventoryDetail> details = inventory.InventoryDetails.ToList();

                        DataAccess.Inventory yieldInventory = new DataAccess.Inventory();
                        yieldInventory.LocationId = 1;
                        yieldInventory.Direction = "I";
                        yieldInventory.RefNo = inventory.InventoryNo;
                        yieldInventory.InventoryDate = DateTime.Today;
                        yieldInventory.InventoryType = "F";
                        yieldInventory.MutationType = "PROCESS";
                        yieldInventory.PostedWhen = DateTime.Now;
                        yieldInventory.PostedBy = CurrentUserName;
                        foreach (InventoryDetail detail in details)
                        {
                            Product yieldProduct = DataContext.Products.SingleOrDefault(p => p.Id == detail.ProductId);
                            if (yieldProduct != null)
                            {
                                InventoryDetail yieldInventoryDetail = new InventoryDetail();
                                yieldInventoryDetail.ProductId = yieldProduct.YieldProductId.GetValueOrDefault(999);
                                yieldInventoryDetail.Qty = detail.Qty;
                                yieldInventoryDetail.UomId = 1;
                                yieldInventoryDetail.NotesDtl = $"Yield From {inventory.InventoryNo}, InventoryDetailId={detail.Id}";

                                yieldInventory.InventoryDetails.Add(yieldInventoryDetail);
                            }
                        }
                        inventoryInProvider.AssignInventoryInNumbering(yieldInventory);

                        SetAuditFields(yieldInventory, "SYSTEM");
                        DataContext.Inventories.Add(yieldInventory);
                    }
                }
            }
            DataContext.SaveChanges();
        }        
    }
}
