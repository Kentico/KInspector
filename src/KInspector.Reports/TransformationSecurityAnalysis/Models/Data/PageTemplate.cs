using System.Diagnostics;
using System.Xml.Linq;

using KInspector.Core.Models;

namespace KInspector.Reports.TransformationSecurityAnalysis.Models.Data
{
    [DebuggerDisplay("{CodeName} {DisplayName}")]
    public class PageTemplate
    {
        public int Id { get; }

        public string CodeName { get; }

        public string DisplayName { get; }

        public IOrderedEnumerable<Page> Pages { get; }

        public IEnumerable<WebPart> WebParts { get; private set; }

        public PageTemplate(IEnumerable<Site> sites, IEnumerable<PageDto> pageDtos, PageTemplateDto pageTemplateDto)
        {
            Id = pageTemplateDto.PageTemplateID;
            CodeName = pageTemplateDto.PageTemplateCodeName;
            DisplayName = pageTemplateDto.PageTemplateDisplayName;

            Pages = pageDtos
                    .Where(pageDto => pageDto.DocumentPageTemplateID == pageTemplateDto.PageTemplateID)
                    .Select(pageDto => new Page(pageDto, sites))
                    .OrderBy(page => page.AliasPath);

            WebParts = XDocument.Parse(pageTemplateDto.PageTemplateWebParts)
                    .Descendants("webpart")
                    .Select(webPartXml => new WebPart(webPartXml))
                    .ToList();
        }

        public void RemoveWebPartsWithNoProperties()
        {
            WebParts = WebParts
                    .Where(webPart => webPart
                        .Properties
                        .Any()
                    );
        }
    }
}