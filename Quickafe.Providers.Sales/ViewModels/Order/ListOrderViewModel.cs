namespace Quickafe.Providers.Sales.ViewModels.Order
{
    public class ListOrderViewModel : DataAccess.Order
    {
        public int OrderDetailCount { get; set; }
        public bool IsVoid { get; set; }
        public string TableCode { get; set; }
    }
}
