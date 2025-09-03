using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public AppUser Member { get; set; } = null!;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime? PickedUpAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public ICollection<BookingItem> Items { get; set; } = new List<BookingItem>();
    }
}
