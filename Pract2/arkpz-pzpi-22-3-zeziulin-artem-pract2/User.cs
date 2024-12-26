using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pl_2
{
    public class User
    {
        public string Role { get; private set; }

        public User(string role)
        {
            Role = role;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var admin = new User("admin");
            var guest = new User("guest");
        }
    }
}

namespace Pl_2_fixed
{
    public class User
    {
        public string Role { get; private set; }

        public User(string role)
        {
            Role = role;
        }

        public static User CreateAdmin()
        {
            return new User("admin");
        }

        public static User CreateGuest()
        {
            return new User("guest");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var admin = User.CreateAdmin();
            var guest = User.CreateGuest();
        }
    }
}
