using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float MinPh { get; set; }
        public float MaxPh { get; set; }
        public int MinMoisture { get; set; }
        public int MaxMoisture { get; set; }
        public string? ImageExtension { get; set; }

        public string? AccountId { get; set; }
        public virtual Account Account { get; set; }
    }
}
