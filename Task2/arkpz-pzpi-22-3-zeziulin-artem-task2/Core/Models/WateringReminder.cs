using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class WateringReminder
    {
        public int Id { get; set; }
        public bool Regular { get; set; }
        public DateTime ReminderDate { get; set; }
        public int? DayGap { get; set; }

        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }
    }
}
