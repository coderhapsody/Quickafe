using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Resources;

namespace Quickafe.Providers 
{
	public class PaymentTypeProvider : BaseProvider
	{
		public PaymentTypeProvider(IQuickafeDbContext context) : base(context)
        {
        }

        public void AddPaymentType(PaymentType paymentType)
        {
            if(DataContext.PaymentTypes.Any(payType => payType.Name == paymentType.Name))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, paymentType.Name));

            DataContext.PaymentTypes.Add(paymentType);
            SetAuditFields(paymentType);
            DataContext.SaveChanges();
        }

        public void UpdatePaymentType(PaymentType paymentType)
        {
            if (DataContext.PaymentTypes.Any(payType => payType.Name == paymentType.Name && payType.Id != paymentType.Id))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, paymentType.Name));

            DataContext.PaymentTypes.Attach(paymentType);
            DataContext.Entry(paymentType).State = EntityState.Modified;
            SetAuditFields(paymentType);
            DataContext.SaveChanges();
        }

        private bool IsSystemDefault(long paymentTypeId)
        {
            var paymentType = GetPaymentType(paymentTypeId);
            return paymentType == null ? false : paymentType.IsSystemDefault;
        }

        public void DeletePaymentType(long paymentTypeId)
        {
            if (!IsSystemDefault(paymentTypeId))
            {
                PaymentType paymentType = GetPaymentType(paymentTypeId);
                DataContext.PaymentTypes.Remove(paymentType);
                DataContext.SaveChanges();
            }
        }

        public void DeletePaymentType(long[] arrayPaymentTypeId)
        {
            foreach(long paymentTypeId in arrayPaymentTypeId)
            {
                if (!IsSystemDefault(paymentTypeId))
                {
                    var paymentType = GetPaymentType(paymentTypeId);
                    DataContext.PaymentTypes.Remove(paymentType);
                }
            }
            DataContext.SaveChanges();
        }

        public PaymentType GetPaymentType(long paymentTypeId)
        {
            return DataContext.PaymentTypes.Single(entity => entity.Id == paymentTypeId);
        }

        public IQueryable<PaymentType> ListPaymentTypes(bool onlyActive = false)
        {
            DataContext.Configuration.ProxyCreationEnabled = false;
            IQueryable<PaymentType> query = DataContext.PaymentTypes;

            if (onlyActive)
                query = query.Where(it => it.IsActive);

            return query;
        }

        public IEnumerable<PaymentType> GetPaymentTypes(bool onlyActive = false)
        {
            DataContext.Configuration.ProxyCreationEnabled = false;

            IQueryable<PaymentType> query = DataContext.PaymentTypes;

            if (onlyActive)
                query = query.Where(it => it.IsActive);

            return query.ToList();
        }
	}
}
