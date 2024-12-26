using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pl_2
{
    public abstract class AppUser
    {
        public virtual string Role { get; }
    }

    public class Admin : AppUser
    {
        public override string Role { get => "Admin"; }

        public void Greet(string name)
        {
            Console.WriteLine($"Hello, Admin {name}");
        }
    }

    public class GenericUser : AppUser
    {
        public override string Role { get => "User"; }

        public void Greet(string name)
        {
            Console.WriteLine($"Hello, User {name}");
        }
    }
}

namespace Pl_2_fixed
{
    public abstract class AppUser
    {
        public virtual string Role { get; }

        public void Greet(string name)
        {
            Console.WriteLine($"Hello, {Role} {name}");
        }
    }

    public class Admin : AppUser
    {
        public override string Role { get => "Admin"; }
    }

    public class GenericUser : AppUser
    {
        public override string Role { get => "User"; }
    }
}
