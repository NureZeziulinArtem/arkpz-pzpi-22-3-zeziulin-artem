namespace GardeningAdviceSystem.Models.Account
{
    public class RegisterAdminModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public AdminType AdminType { get; set; }
    }

    public enum AdminType
    {
        Admin,
        DatabaseAdmin
    }
}
