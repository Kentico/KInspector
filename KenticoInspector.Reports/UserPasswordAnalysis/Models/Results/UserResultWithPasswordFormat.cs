using Newtonsoft.Json;

namespace KenticoInspector.Reports.UserPasswordAnalysis.Models.Data.Results
{
    public class UserResultWithPasswordFormat : UserResult
    {
        [JsonProperty(Order = 1)]
        public string PasswordFormat { get; set; }

        public UserResultWithPasswordFormat(UserDto userDto) : base(userDto)
        {
            PasswordFormat = userDto.UserPasswordFormat;
        }
    }
}