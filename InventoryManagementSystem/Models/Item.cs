using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;

public class Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdItem { get; set; }
    [Required]
    public string? KodeItem { get; set; }
    [Required]
    public string? Name { get; set; }
    public string? PicturePath { get; set; }
    public string? Description { get; set; }
    public bool Availability { get; set; } = true;
    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public virtual Category? Category { get; set; }
    public virtual SubCategory? SubCategory { get; set; }
    public int SupplierId { get; set; }
    public virtual Supplier? Supplier { get; set; }
}

public class ItemViewModel : Item
{
    public IFormFile? Picture { get; set; }
}



