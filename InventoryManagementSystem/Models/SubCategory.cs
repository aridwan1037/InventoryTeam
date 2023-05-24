using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;

public class SubCategory
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdSubCategory { get; set; }

    [Required] //tidak boleh null
    [StringLength(10)]
    public string? SubCategoryCode { get; set; }

    [Required]
    public string? SubCategoryName { get; set; }
    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    public virtual ICollection<Item> Items { get; set; }
    public SubCategory()
    {
        Items = new HashSet<Item>(); //hashet itu harus unik
    }


    [Column(TypeName = "ntext")]
    public string? Description { get; set; }


}
