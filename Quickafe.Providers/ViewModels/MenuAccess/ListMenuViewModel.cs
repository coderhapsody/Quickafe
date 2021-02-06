namespace Quickafe.Providers.ViewModels.RoleAccess
{
    public class ListMenuViewModel
    {
        public long id { get; set; }
        public string FAIcon { get; set; }
        public string Name { get; set; }
        public bool hasChildren { get; set; }
        public string Url { get; set; }
    }
}
