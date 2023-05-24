using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required]
        [StringLength(40)]
        public string? CompanyName { get; set; } = null;

        [StringLength(40)]
        public string? ContactNumber { get; set; } = null;

        [StringLength(40)]
        public string? Address { get; set; } = null;

        [StringLength(60)]
        public string? EmailCompany { get; set; } = null;
        public virtual ICollection<Item> Items { get; set; }
        public Supplier()
        {
            Items = new HashSet<Item>(); //hashet itu harus unik
        }
    }
}