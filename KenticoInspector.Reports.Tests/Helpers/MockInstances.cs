using KenticoInspector.Core.Models;
using System;

namespace KenticoInspector.Reports.Tests.Helpers
{
    public static class MockInstances
    {
        public static Instance Kentico9 = new Instance
        {
            Name = "K9 Test Instance",
            Guid = Guid.NewGuid(),
            Path = "C:\\inetpub\\wwwroot\\Kentico9",
            Url = "http://localhost/kentico9",
            DatabaseSettings = null
        };

        public static Instance Kentico10 = new Instance
        {
            Name = "K11 Test Instance",
            Guid = Guid.NewGuid(),
            Path = "C:\\inetpub\\wwwroot\\Kentico10",
            Url = "http://localhost/kentico10",
            DatabaseSettings = null
        };

        public static Instance Kentico11 = new Instance
        {
            Name = "K11 Test Instance",
            Guid = Guid.NewGuid(),
            Path = "C:\\inetpub\\wwwroot\\Kentico11",
            Url = "http://localhost/kentico11",
            DatabaseSettings = null
        };

        public static Instance Kentico12 = new Instance
        {
            Name = "K11 Test Instance",
            Guid = Guid.NewGuid(),
            Path = "C:\\inetpub\\wwwroot\\Kentico12",
            Url = "http://localhost/kentico12",
            DatabaseSettings = null
        };

        public static Instance Get(int majorVersion) {
            switch (majorVersion)
            {
                case 9:
                    return Kentico9;
                case 10:
                    return Kentico10;
                case 11:
                    return Kentico11;
                case 12:
                    return Kentico12;
            }

            return null;
        }
    }
}
