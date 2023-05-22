using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_M_System.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required]
        [StringLength(40)]
        public string? CompanyName { get; set; } = null;

        [StringLength(40)]
        public string? ContactName { get; set; } = null;

        [StringLength(40)]
        public string? Address { get; set; } = null;

        [StringLength(60)]
        public string? City { get; set; } = null;
        public virtual ICollection<Item> Items { get; set; }
        public Supplier()
        {
            Items = new HashSet<Item>(); //hashet itu harus unik
        }
    }
}