using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.ViewModels
{
    public class RoleModifyModel
    {
        [Required]
        public string RoleName { get; set; }

        public string RoleId { get; set; }

        public string[] AddIds { get; set; }

        public string[] DeleteIds { get; set; }
    }
}
