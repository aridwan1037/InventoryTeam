using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{
    public enum RequestItemStatus
    {
        WaitingApproval,
        Rejected,
        Approved,
        Cancel,
    }

    public class RequestItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        public int? OrderItemId { get; set; }
        public virtual OrderItem? OrderItem { get; set; }
        public int ItemId { get; set; }
        public virtual Item? Item { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual User? User { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        public DateTime RequestBorrowDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime RequestDueDate { get; set; }
        [Required]
        public string NoteRequest { get; set; } = ""; //di isi ketika user ngajukan request
        public string? NoteActionRequest { get; set; }  //di isi ketika admin melakukan aksi reject atau approved
        public RequestItemStatus Status { get; set; } = RequestItemStatus.WaitingApproval;

    }
}