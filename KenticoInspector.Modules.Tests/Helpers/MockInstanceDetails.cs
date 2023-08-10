using KenticoInspector.Core.Models;

using System;
using System.Collections.Generic;

namespace KenticoInspector.Modules.Tests.Helpers
{
    public static class MockInstanceDetails
    {
        public static InstanceDetails Kentico9 = new InstanceDetails
        {
            AdministrationVersion = new Version("9.0"),
            DatabaseVersion = new Version("9.0"),
            Sites = new List<Site>
            {
                new Site { DomainName = "kentico9.com" }
            }
        };

        public static InstanceDetails Kentico10 = new InstanceDetails
        {
            AdministrationVersion = new Version("10.0"),
            DatabaseVersion = new Version("10.0"),
            Sites = new List<Site>
            {
                new Site { DomainName = "kentico10.com" }
            }
        };

        public static InstanceDetails Kentico11 = new InstanceDetails
        {
            AdministrationVersion = new Version("11.0"),
            DatabaseVersion = new Version("11.0"),
            Sites = new List<Site>
            {
                new Site { DomainName = "kentico11.com" }
            }
        };

        public static InstanceDetails Kentico12 = new InstanceDetails
        {
            AdministrationVersion = new Version("12.0"),
            DatabaseVersion = new Version("12.0"),
            Sites = new List<Site>
            {
                new Site { DomainName = "kentico12.com" }
            }
        };

        public static InstanceDetails Kentico13 = new InstanceDetails
        {
            AdministrationVersion = new Version("13.0"),
            DatabaseVersion = new Version("13.0"),
            Sites = new List<Site>
            {
                new Site { DomainName = "kentico13.com" }
            }
        };

        public static InstanceDetails Get(int majorVersion, Instance instance)
        {
            InstanceDetails instanceDetails = null;

            switch (majorVersion)
            {
                case 9:
                    instanceDetails = Kentico9;
                    break;
                case 10:
                    instanceDetails = Kentico10;
                    break;
                case 11:
                    instanceDetails = Kentico11;
                    break;
                case 12:
                    instanceDetails = Kentico12;
                    break;
                case 13:
                    instanceDetails = Kentico13;
                    break;
            }

            if (instanceDetails != null)
            {
                instanceDetails.Guid = instance.Guid;
            }

            return instanceDetails;
        }
    }
}