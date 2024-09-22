using System;

namespace GymManagmentSystem.Models
{
    public class MemberSubscription
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int SubscriptionId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal PaidPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RemainingSessions { get; set; }
        public bool IsDeleted { get; set; }
    }
}
