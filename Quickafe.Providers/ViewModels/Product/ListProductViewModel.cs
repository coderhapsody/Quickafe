namespace Quickafe.Providers.ViewModels.Product
{
    public class ListProductViewModel : DataAccess.Product
    {
        public string ProductCategoryName { get; set; }
        public bool CanOrder { get; set; }
    }
}
