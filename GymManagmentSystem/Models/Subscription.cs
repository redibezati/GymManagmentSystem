using System;

namespace GymManagmentSystem.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int NumberOfMonths { get; set; }
        public string WeekFrequency { get; set; } // e.g., "2 days/week"
        public int TotalNumberOfSessions { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsDeleted { get; set; }
    }
}
