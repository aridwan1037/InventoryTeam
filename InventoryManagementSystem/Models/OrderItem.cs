using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Models
{
    public enum OrderItemStatus
    {
        WaitingPickUp,
        DonePickUp,
        CancelledBySystem,
    }

    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId {get;set;}
        public int? RequestId {get;set;}
        public virtual RequestItem? RequestItem {get;set;}
        public int? BorrowedId {get;set;}
        public virtual BorrowedItem? BorrowedItem {get;set;}
        public int ItemId {get;set;}
        public virtual Item? Item {get;set;} 

        [ForeignKey(nameof(User))]
        public string UserId {get;set;}=null!;
        public virtual User? User {get;set;}

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreateAt {get;set;}

        [Required]
        [DataType(DataType.Date)]
        public DateTime BorrowDateApproved {get;set;}

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDateApproved {get;set;}

        public string NoteDonePickUp {get;set;}="";

        public string NoteWaitingPickUp {get;set;}="";

        [Required]
        public OrderItemStatus Status {get;set;}=OrderItemStatus.WaitingPickUp;

    }
}