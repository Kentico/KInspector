using Newtonsoft.Json;

namespace KInspector.Reports.UserPasswordAnalysis.Models.Data
{
    public class CmsUser
    {
        public int UserID { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        [JsonIgnore]
        public string? UserPassword { get; set; }

        [JsonIgnore]
        public string? UserPasswordFormat { get; set; }

        public string? UserPrivilegeLevel { get; set; }

        [JsonIgnore]
        public string? FirstName { get; set; }

        [JsonIgnore]
        public string? MiddleName { get; set; }

        [JsonIgnore]
        public string? LastName { get; set; }

        public string? FullName { get; set; }
    }
}