using KInspector.Core.Models;

namespace KInspector.Tests.Common.Helpers
{
    public static class MockInstanceDetails
    {
        public static InstanceDetails Kentico9 = new()
        {
            AdministrationVersion = new Version("9.0"),
            AdministrationDatabaseVersion = new Version("9.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico9.com" }
            }
        };

        public static InstanceDetails Kentico10 = new()
        {
            AdministrationVersion = new Version("10.0"),
            AdministrationDatabaseVersion = new Version("10.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico10.com" }
            }
        };

        public static InstanceDetails Kentico11 = new()
        {
            AdministrationVersion = new Version("11.0"),
            AdministrationDatabaseVersion = new Version("11.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico11.com" }
            }
        };

        public static InstanceDetails Kentico12 = new()
        {
            AdministrationVersion = new Version("12.0"),
            AdministrationDatabaseVersion = new Version("12.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico12.com" }
            }
        };

        public static InstanceDetails Kentico13 = new()
        {
            AdministrationVersion = new Version("13.0"),
            AdministrationDatabaseVersion = new Version("13.0"),
            Sites = new List<Site>
            {
                new() { DomainName = "kentico13.com" }
            }
        };

        public static InstanceDetails Get(int majorVersion)
        {
            InstanceDetails? instanceDetails = majorVersion switch
            {
                9 => Kentico9,
                10 => Kentico10,
                11 => Kentico11,
                12 => Kentico12,
                13 => Kentico13,
                _ => throw new NotImplementedException(),
            };

            return instanceDetails;
        }
    }
}