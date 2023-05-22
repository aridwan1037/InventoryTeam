using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_M_System.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCategory { get; set; }

        [Required] //tidak boleh null
        [StringLength(15)]
        public string? CategoryName { get; set; }

        [Column(TypeName = "ntext")]
        public string? Description { get; set; }

        public virtual ICollection<Item> Items { get; set; } // untuk one to many
                                                             //pakai virtual karena nanti ada eager loading dan easy loading
                                                             //eager loading => semua di load
                                                             // lazy loading => hanya load sesuatu yang akan diakses (seeprti streaming => parsial)
                                                             //manual loading => menentukan sendiri apa yang mau diambil

        public Category()
        {
            Items = new HashSet<Item>(); //hashet itu harus unik
        }
    }
}