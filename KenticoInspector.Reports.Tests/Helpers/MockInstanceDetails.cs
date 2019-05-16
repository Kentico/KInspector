using KenticoInspector.Core.Models;
using System;

namespace KenticoInspector.Reports.Tests.Helpers
{
    static class MockInstanceDetails
    {
        public static InstanceDetails Kentico9 = new InstanceDetails
        {
            AdministrationVersion = new Version("9.0"),
            DatabaseVersion = new Version("9.0"),
        };

        public static InstanceDetails Kentico10 = new InstanceDetails
        {
            AdministrationVersion = new Version("10.0"),
            DatabaseVersion = new Version("10.0"),
        };

        public static InstanceDetails Kentico11 = new InstanceDetails
        {
            AdministrationVersion = new Version("11.0"),
            DatabaseVersion = new Version("11.0"),
        };

        public static InstanceDetails Kentico12 = new InstanceDetails
        {
            AdministrationVersion = new Version("12.0"),
            DatabaseVersion = new Version("12.0"),
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
            }

            if (instanceDetails != null)
            {
                instanceDetails.Guid = instance.Guid;
            }

            return instanceDetails;
        }
    }
}
