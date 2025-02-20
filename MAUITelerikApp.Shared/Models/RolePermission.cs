using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIDevExpressApp.Shared.Models
{
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        [ForeignKey("Permission")]
        public int PermissionId { get; set; }
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
