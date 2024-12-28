using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public string AccountId { get; set; }
        public virtual Account Account { get; set; }

        public int? PlantId { get; set; }
        public virtual Plant? Plant { get; set; }

        public virtual List<DeviceLog> Logs { get; set; } = [];
        public virtual List<WateringReminder> Reminders { get; set; } = [];
    }
}
