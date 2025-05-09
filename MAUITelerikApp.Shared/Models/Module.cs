﻿using System.ComponentModel.DataAnnotations;

namespace MAUIDevExpressApp.Shared.Models
{
    public class Module
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Navigation Property for Pages

        public virtual ICollection<Page> Pages { get; set; } = new List<Page>();
    }
}
