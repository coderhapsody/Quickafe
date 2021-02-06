namespace Quickafe.Providers.Sales.ViewModels.Order
{
    public class OrderDetailSummary
    {
        public OrderDetailSummary(decimal totalOrder, decimal taxAmount, decimal serviceCharge, decimal grandTotal)
        {
        }

        public decimal TotalOrder { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal ServiceCharge { get; private set; }
        public decimal GrandTotal { get; private set; }
    }
}
