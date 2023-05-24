using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{
    public enum BorrowedItemStatus
    {
        StillBorrowed,
        DoneBorrowing,
        DoneAndLost,

    }

    public class BorrowedItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BorrowedId { get; set; }

        public int? OrderId { get; set; }
        public virtual OrderItem? OrderItem { get; set; }

        public int? ReceiptId { get; set; }
        public virtual GoodReceipt? GoodReceipt { get; set; }

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
        public DateTime BorrowedDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        public string NoteBorrowed { get; set; } = "";
        public string? PicturePath { get; set; }
        public int? LostId { get; set; }
        public virtual LostItem? LostItems { get; set; }

        [Required]
        public BorrowedItemStatus Status { get; set; } = BorrowedItemStatus.StillBorrowed;
    }
    public class BorrowedItemViewModel : BorrowedItem
    {
        public IFormFile? Picture { get; set; }
    }
}
