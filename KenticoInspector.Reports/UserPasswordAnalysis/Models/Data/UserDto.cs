namespace KenticoInspector.Reports.UserPasswordAnalysis.Models.Data
{
    public class UserDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string UserPassword { get; set; }

        public string UserPasswordFormat { get; set; }

        public PrivilegeLevel UserPrivilegeLevel { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }
    }
}