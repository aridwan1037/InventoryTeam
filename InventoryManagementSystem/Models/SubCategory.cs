using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;

public class SubCategory
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdSubCategory { get; set; }

    [Required] //tidak boleh null
    [StringLength(5)]
    public string? SubCategoryCode { get; set; }

    [Required] 
    public string? SubCategoryName { get; set; }
    public int IdCategory { get; set; }
    public virtual Category? Category { get; set; }

    public virtual ICollection<Item> Items { get; set; } // untuk one to many
                                                         //pakai virtual karena nanti ada eager loading dan easy loading
                                                         //eager loading => semua di load
                                                         // lazy loading => hanya load sesuatu yang akan diakses (seeprti streaming => parsial)
                                                         //manual loading => menentukan sendiri apa yang mau diambil
    public SubCategory()
    {
        Items = new HashSet<Item>(); //hashet itu harus unik
    }


    [Column(TypeName = "ntext")]
    public string? Description { get; set; }


}
