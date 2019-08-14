namespace KenticoInspector.Reports.UserPasswordAnalysis.Models.Data.Results
{
    public class CmsUserResult : CmsUser
    {
        public CmsUserResult(CmsUser user)
        {
            UserID = user.UserID;
            UserName = user.UserName;

            if (string.IsNullOrEmpty(user.FullName))
            {
                FullName = $"{user.FirstName} {user.MiddleName} {user.LastName}";
            }
            else
            {
                FullName = user.FullName;
            }

            Email = user.Email;
            UserPrivilegeLevel = user.UserPrivilegeLevel;
        }
    }
}