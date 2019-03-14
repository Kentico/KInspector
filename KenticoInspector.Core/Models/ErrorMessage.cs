using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Core.Models
{
    public class ErrorMessage
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}
