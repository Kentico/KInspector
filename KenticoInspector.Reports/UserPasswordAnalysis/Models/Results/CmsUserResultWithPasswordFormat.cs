using Newtonsoft.Json;

namespace KenticoInspector.Reports.UserPasswordAnalysis.Models.Data.Results
{
    public class CmsUserResultWithPasswordFormat : CmsUserResult
    {
        [JsonProperty(Order = 1)]
        public string PasswordFormat { get; set; }

        public CmsUserResultWithPasswordFormat(CmsUser cmsUser) : base(cmsUser)
        {
            PasswordFormat = cmsUser.UserPasswordFormat;
        }
    }
}