using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CartItem
    {
        public string AccountId { get; set; }
        public virtual Account Account { get; set; }

        public int FertilizerId { get; set; }
        public virtual Fertilizer Fertilizer { get; set; }

        public bool Remind { get; set; }
        public DateTime? RemindAt { get; set; }
    }
}
