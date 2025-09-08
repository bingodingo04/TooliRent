using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Tool
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ToolStatus Status { get; set; } = ToolStatus.Available;
        public string? SerialNumber { get; set; }
        public string? Condition { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
    }
}
