using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAUIDevExpressApp.Shared.Models
{
    public class Page
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("Module")]
        public int ModuleId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual Module Module { get; set; }

        // ✅ Navigation property for Permissions
        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
