namespace KInspector.Actions.ResetCmsUserLogin.Models
{
    public class CmsUser
    {
        public int UserID { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public bool Enabled { get; set; }
    }
}
