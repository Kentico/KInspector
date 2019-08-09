using Newtonsoft.Json;

namespace KenticoInspector.Reports.UserPasswordAnalysis.Models.Data.Results
{
    public class CmsUserResultWithPasswordFormat : CmsUserResult
    {
        [JsonProperty(Order = 1)]
        public new string UserPasswordFormat { get; set; }

        public CmsUserResultWithPasswordFormat(
            CmsUser user)
            : base(user)
        {
            UserPasswordFormat = user.UserPasswordFormat;
        }
    }
}