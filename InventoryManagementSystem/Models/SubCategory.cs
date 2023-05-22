using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;

public class SubCategory
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdSubCategory { get; set; }

    [Required] //tidak boleh null
    [StringLength(15)]
    public string? SubCategoryCode { get; set; }

    [Required] //tidak boleh null
    [StringLength(15)]
    public string? SubCategoryName { get; set; }
    public int IdCategory { get; set; }
    public virtual Category? Category { get; set; }

    [Column(TypeName = "ntext")]
    public string? Description { get; set; }


}
