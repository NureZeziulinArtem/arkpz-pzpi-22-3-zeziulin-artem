using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Account : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual List<Device> Devices { get; set; } = [];
        public virtual List<CartItem> CartItems { get; set; } = [];
        public virtual List<Plant> AddedPlants { get; set; } = [];
    }
}
