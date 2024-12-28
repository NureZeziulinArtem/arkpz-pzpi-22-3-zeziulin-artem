using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Fertilizer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageExtension { get; set; }
        public float Ph { get; set; }
        public float Size { get; set; }
        public int Price { get; set; }
    }
}
