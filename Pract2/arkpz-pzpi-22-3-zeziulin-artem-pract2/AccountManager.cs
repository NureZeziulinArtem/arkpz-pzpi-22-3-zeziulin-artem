namespace Pl_2
{
    public class AccountManager
    {
        private readonly string _name;
        private readonly string _surname;

        public AccountManager(string name, string surname)
        {
            _name = name;
            _surname = surname;
        }

        public void Greetings(string name, string surname)
        {
            Console.WriteLine($"Hello, {_name} {_surname}!");
        }
    }
}

namespace Pl_2_fixed
{
    public class AccountManager
    {
        private readonly string _name;
        private readonly string _surname;

        public AccountManager(string name, string surname)
        {
            _name = name;
            _surname = surname;
        }

        public void Greetings()
        {
            Console.WriteLine($"Hello, {_name} {_surname}!");
        }
    }
}
