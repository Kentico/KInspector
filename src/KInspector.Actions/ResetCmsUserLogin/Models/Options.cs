using System.ComponentModel;

namespace KInspector.Actions.ResetCmsUserLogin.Models
{
    public class Options
    {
        [DisplayName("User ID")]
        [Description("The user ID to reset and enable.")]
        public int? UserId { get; set; }
    }
}
