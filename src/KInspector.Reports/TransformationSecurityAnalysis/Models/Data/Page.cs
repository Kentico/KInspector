using System.Globalization;

using KInspector.Core.Models;

namespace KInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    public class Page
    {
        public Site? Site { get; }

        public string Name { get; }

        public int NodeId { get; }

        public string AliasPath { get; }

        public CultureInfo Culture { get; }

        public int TemplateId { get; }

        public Page(PageDto pageDto, IEnumerable<Site> sites)
        {
            Site = sites.FirstOrDefault(site => site.Id == pageDto.NodeSiteID);
            Name = pageDto.DocumentName;
            NodeId = pageDto.NodeID;
            AliasPath = pageDto.NodeAliasPath;
            Culture = new CultureInfo(pageDto.DocumentCulture);
            TemplateId = pageDto.DocumentPageTemplateID;
        }
    }
}