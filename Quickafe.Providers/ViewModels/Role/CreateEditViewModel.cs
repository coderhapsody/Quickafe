using System.ComponentModel.DataAnnotations;

namespace Quickafe.Providers.ViewModels.Role
{
    public class CreateEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
