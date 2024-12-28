using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DeviceLog
    {
        public int Id { get; set; }
        public DateTime Recorded { get; set; }
        public float Ph { get; set; }
        public int Moisture { get; set; }

        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }
    }
}
