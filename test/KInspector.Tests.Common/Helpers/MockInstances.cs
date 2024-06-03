using KInspector.Core.Models;

namespace KInspector.Tests.Common.Helpers
{
    public static class MockInstances
    {
        public static Instance Kentico9 = new()
        {
            Name = "K9 Test Instance",
            Guid = Guid.NewGuid(),
            AdministrationPath = "C:\\inetpub\\wwwroot\\Kentico9",
            AdministrationUrl = "http://kentico9.com"
        };

        public static Instance Kentico10 = new()
        {
            Name = "K10 Test Instance",
            Guid = Guid.NewGuid(),
            AdministrationPath = "C:\\inetpub\\wwwroot\\Kentico10",
            AdministrationUrl = "http://kentico10.com"
        };

        public static Instance Kentico11 = new()
        {
            Name = "K11 Test Instance",
            Guid = Guid.NewGuid(),
            AdministrationPath = "C:\\inetpub\\wwwroot\\Kentico11",
            AdministrationUrl = "http://kentico11.com"
        };

        public static Instance Kentico12 = new()
        {
            Name = "K12 Test Instance",
            Guid = Guid.NewGuid(),
            AdministrationPath = "C:\\inetpub\\wwwroot\\Kentico12",
            AdministrationUrl = "http://kentico12.com"
        };

        public static Instance Kentico13 = new()
        {
            Name = "K13 Test Instance",
            Guid = Guid.NewGuid(),
            AdministrationPath = "C:\\inetpub\\wwwroot\\Kentico13",
            AdministrationUrl = "http://kentico13.com"
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
                default:
                    throw new NotImplementedException();
            }
        }
    }
}