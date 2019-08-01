namespace KenticoInspector.Reports.UserPasswordAnalysis.Models.Data.Results
{
    public class UserResult
    {
        public string UserName { get; }

        public string FullName { get; }

        public string Email { get; }

        public string PrivilegeLevel { get; }

        public UserResult(UserDto userDto)
        {
            UserName = userDto.UserName;

            if (string.IsNullOrEmpty(userDto.FullName))
            {
                FullName = string.Join(' ', userDto.FirstName, userDto.MiddleName, userDto.LastName);
            }
            else
            {
                FullName = userDto.FullName;
            }

            Email = userDto.Email;
            PrivilegeLevel = userDto.UserPrivilegeLevel.ToString();
        }
    }
}