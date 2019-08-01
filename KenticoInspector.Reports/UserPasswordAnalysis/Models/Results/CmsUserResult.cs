namespace KenticoInspector.Reports.UserPasswordAnalysis.Models.Data.Results
{
    public class CmsUserResult
    {
        public string UserName { get; }

        public string FullName { get; }

        public string Email { get; }

        public string PrivilegeLevel { get; }

        public CmsUserResult(CmsUser cmsUser)
        {
            UserName = cmsUser.UserName;

            if (string.IsNullOrEmpty(cmsUser.FullName))
            {
                FullName = string.Join(' ', cmsUser.FirstName, cmsUser.MiddleName, cmsUser.LastName);
            }
            else
            {
                FullName = cmsUser.FullName;
            }

            Email = cmsUser.Email;
            PrivilegeLevel = cmsUser.UserPrivilegeLevel.ToString();
        }
    }
}