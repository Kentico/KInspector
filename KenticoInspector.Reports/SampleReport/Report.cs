﻿using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.SampleReport.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.SampleReport
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IModuleMetadataService moduleMetadataService) : base(moduleMetadataService)
        {
            this.databaseService = databaseService;
        }

        // Hide sample report in UI
        public override IList<Version> CompatibleVersions => new Version[0];

        public override IList<string> Tags => new List<string> {
            ModuleTags.Consistency
        };

        public override ReportResults GetResults()
        {
            var random = new Random();
            var issueCount = random.Next(0, 3);
            var data = new List<string>();
            for (int i = 0; i < issueCount; i++)
            {
                var name = $"test-{i}";
                var problem = GetRandomString(10);
                data.Add(Metadata.Terms.DetailedResult.With(new { name, problem }));
            }

            return new ReportResults()
            {
                Data = data,
                Type = ResultsType.StringList,
                Status = ResultsStatus.Information,
                Summary = Metadata.Terms.Summary.With(new { issueCount })
            };
        }

        private string GetRandomString(int size)
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