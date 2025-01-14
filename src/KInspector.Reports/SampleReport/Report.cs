using KInspector.Core;
using KInspector.Core.Constants;
using KInspector.Core.Models;
using KInspector.Core.Services.Interfaces;
using KInspector.Reports.SampleReport.Models;

using System.Text;

namespace KInspector.Reports.SampleReport
{
    public class Report : AbstractReport<Terms>
    {
        public Report(IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
        }

        // Hide sample report in UI
        public override IList<Version> CompatibleVersions => Array.Empty<Version>();

        public override IList<string> Tags => new List<string> {
            ModuleTags.Consistency
        };

        public override Task<ModuleResults> GetResults()
        {
            var random = new Random();
            var issueCount = random.Next(0, 3);
            var results = new ModuleResults()
            {
                Type = ResultsType.StringList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.Summary?.With(new { issueCount })
            };
            for (int i = 0; i < issueCount; i++)
            {
                var name = $"test-{i}";
                var problem = GetRandomString(10);
                results.StringResults.Add(Metadata.Terms.DetailedResult?.With(new { name, problem }));
            }

            return Task.FromResult(results);
        }

        private static string GetRandomString(int size)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}