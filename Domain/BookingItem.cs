using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BookingItem
    {
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public Guid ToolId { get; set; }
        public Tool Tool { get; set; } = null!;
    }
}
