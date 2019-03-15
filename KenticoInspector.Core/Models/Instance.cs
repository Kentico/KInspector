using System;
using System.Collections.Generic;

namespace KenticoInspector.Core.Models
{
    public class Instance
    {
        public DatabaseSettings DatabaseSettings { get; set; }
        public List<ErrorMessage> ErrorMessages { get; set; }
        public Guid Guid { get; set; }
        public Version KenticoAdministrationVersion { get; set; }
        public Version KenticoDatabaseVersion { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<Site> Sites { get; set; }
        public string Url { get; set; }
        public void AddErrorMessage(string message, Exception exception)
        {
            if (ErrorMessages == null)
            {
                ErrorMessages = new List<ErrorMessage>();
            }

            ErrorMessages.Add(new ErrorMessage { Message = message, Exception = exception });
        }
    }
}
