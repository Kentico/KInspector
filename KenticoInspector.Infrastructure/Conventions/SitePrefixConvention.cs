using Dapper.FluentMap.Conventions;
using Dapper.FluentMap.Mapping;
using KenticoInspector.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Infrastructure.Conventions
{
    class SitePrefixConvention : Convention
    {
        public SitePrefixConvention()
        {
            Properties().Configure(c => c.HasPrefix("site").IsCaseInsensitive());
        }
    }
}
