using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers.Inventory.ViewModels.InventoryIn;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Quickafe.Providers.Inventory
{
    public class InventoryInProvider :BaseProvider 
    {
        private readonly IMapper mapper;
        private readonly AutoNumberProvider autoNumberProvider;

        public InventoryInProvider(IQuickafeDbContext context, IMapper mapper, AutoNumberProvider autoNumberProvider) : base(context)
        {
            this.mapper = mapper;
            this.autoNumberProvider = autoNumberProvider;
        }

        public IQueryable<ListInventoryViewModel> GetInventory()
        {
            IQueryable<ListInventoryViewModel> query =
                from inventory in DataContext.Inventories
                     .Where(m => m.Direction == "I")
                     .OrderByDescending(m => m.InventoryDate)
                     .ThenByDescending(m => m.InventoryNo)
                    //TODO
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

        internal void AssignInventoryInNumbering(DataAccess.Inventory inventory)
        {
            long lastNumber = autoNumberProvider.GetLastNumber("INV-IN", inventory.InventoryDate.Year, inventory.InventoryDate.Month);
            string gen = lastNumber.ToString("00000");
            string inventoryNo = String.Format("{0}/{1}/{2}/{3}",
                "INV-IN",
                inventory.InventoryDate.Year.ToString(),
                inventory.InventoryDate.Month.ToString("00"),
                gen);
            inventory.InventoryNo = inventoryNo;

            autoNumberProvider.IncrementLastNumber("INV-IN", inventory.InventoryDate.Year, inventory.InventoryDate.Month);
        }

        public void AddInventory(Quickafe.DataAccess.Inventory inventory)
        {
            AssignInventoryInNumbering(inventory);

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

        public string Left(string str, int length)
        {
            str = (str ?? string.Empty);
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public string Right(string str, int length)
        {
            str = (str ?? string.Empty);
            return (str.Length >= length)
                ? str.Substring(str.Length - length, length)
                : str;
        }

        public long GetLocationId()
        {
            var loc = DataContext.Locations.SingleOrDefault();
            return loc.Id;
        }

        public long GetUOMId()
        {
            var uom = DataContext.Uoms.SingleOrDefault();
            return uom.Id;
        }

        public void PostInInventory(long[] arrayInventoryId)
        {
            IEnumerable<DataAccess.Inventory> inventories = DataContext.Inventories
                .Where(inv => inv.Direction == "I" && arrayInventoryId.Contains(inv.Id));
            foreach (DataAccess.Inventory inventory in inventories)
            {
                inventory.PostedWhen = DateTime.Now;
                inventory.PostedBy = CurrentUserName;
            }
            DataContext.SaveChanges();
        }
    }
}
