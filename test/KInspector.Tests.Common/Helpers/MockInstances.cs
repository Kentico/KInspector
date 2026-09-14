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

        public static Instance XbK30 = new()
        {
            Name = "XbK 30 Test Instance",
            Guid = Guid.NewGuid(),
            AdministrationPath = "C:\\inetpub\\wwwroot\\XbK30",
            AdministrationUrl = "http://xbk30.com"
        };

        public static Instance XbK31 = new()
        {
            Name = "XbK 31 Test Instance",
            Guid = Guid.NewGuid(),
            AdministrationPath = "C:\\inetpub\\wwwroot\\XbK31",
            AdministrationUrl = "http://xbk31.com"
        };

        public static Instance Get(int majorVersion)
        {
            return majorVersion switch
            {
                9 => Kentico9,
                10 => Kentico10,
                11 => Kentico11,
                12 => Kentico12,
                13 => Kentico13,
                30 => XbK30,
                31 => XbK31,
                _ => throw new NotImplementedException(),
            };
        }
    }
}