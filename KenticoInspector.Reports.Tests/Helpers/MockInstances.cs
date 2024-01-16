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
            AdminPath = "C:\\inetpub\\wwwroot\\Kentico9",
            AdminUrl = "http://kentico9.com",
            DatabaseSettings = null
        };

        public static Instance Kentico10 = new Instance
        {
            Name = "K10 Test Instance",
            Guid = Guid.NewGuid(),
            AdminPath = "C:\\inetpub\\wwwroot\\Kentico10",
            AdminUrl = "http://kentico10.com",
            DatabaseSettings = null
        };

        public static Instance Kentico11 = new Instance
        {
            Name = "K11 Test Instance",
            Guid = Guid.NewGuid(),
            AdminPath = "C:\\inetpub\\wwwroot\\Kentico11",
            AdminUrl = "http://kentico11.com",
            DatabaseSettings = null
        };

        public static Instance Kentico12 = new Instance
        {
            Name = "K12 Test Instance",
            Guid = Guid.NewGuid(),
            AdminPath = "C:\\inetpub\\wwwroot\\Kentico12",
            AdminUrl = "http://kentico12.com",
            DatabaseSettings = null
        };

        public static Instance Kentico13 = new Instance
        {
            Name = "K13 Test Instance",
            Guid = Guid.NewGuid(),
            AdminPath = "C:\\inetpub\\wwwroot\\Kentico13",
            AdminUrl = "http://kentico13.com",
            DatabaseSettings = null
        };

        public static Instance Get(int majorVersion)
        {
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
                case 13:
                    return Kentico13;
            }

            return null;
        }
    }
}