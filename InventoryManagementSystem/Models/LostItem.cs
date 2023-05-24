using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{
    public enum LostItemStatus
    {
        Active,
        Resolve
    }

    public class LostItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LostId { get; set; }

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
        public DateTime LostDate { get; set; }

        public string? NoteItemLost { get; set; }
        public string? NoteItemFound { get; set; }


        public int? BorrowedId { get; set; }
        public virtual BorrowedItem? BorrowedItem { get; set; }

        public LostItemStatus Status { get; set; }

    }
}