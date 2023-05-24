using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{
    public enum GoodReceiptStatus
    {
        Returned,
        Lost,
        Broken,
    }
    public class GoodReceipt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReceiptId { get; set; }
        public int ItemId { get; set; }
        public virtual Item? Item { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual User? User { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreateAt { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReceivedDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime MissedDueDate { get; set; }
        public string? NoteItemReturned { get; set; }
        public string? NoteItemLost { get; set; }
        public string? NoteItemBroken { get; set; }
        public string? PicturePath { get; set; }
        public int? BorrowedId { get; set; }
        public virtual BorrowedItem? BorrowedItem { get; set; }

        public GoodReceiptStatus Status { get; set; }

    }
    public class GoodReceiptViewModel : GoodReceipt
    {
        public IFormFile? Picture { get; set; }
    }

}