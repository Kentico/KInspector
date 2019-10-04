---
title: Creating a new report
parent: Development
order: 0
---

# Creating a new report

To create a new report, only the following projects should have changes:

- `KenticoInspector.Reports`
- `KenticoInspector.Reports.Tests`

Any changes to the other projects should logically fall into one of the following:

- A helper or service that makes code [DRY](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself) in more than 2 modules.
- A significant reduction or optimization of some internal API.
- A useful addition that brings great value to the new report and can improve other reports.

## Overview

The following is the general process for creating a report:

1. Identify an issue to inspect. Does an existing report already cover the issue?
1. Derive a name from the issue to use as the name of the report. Follow [the guidelines below](#report-naming) to derive a minimal descriptive name.
1. What results will the report produce: will it be a list of strings, tables, or other results? If there are tables, what columns will they have?
1. Scaffold the minimal structure [as shown below](#report-file-structure).
1. Loosely write the report class using placeholder variables for each SQL script and model class.
1. Create the missing SQL scripts, data classes, or result classes that contribute to each result.
1. Create any missing terms.
1. [Write tests](./writingTestsForAReport) to check that some specific data produces some specific result.

## Report naming

The report name is used for the report folder name and the C# namespace containing the report's classes.

{% include divider.md content="DO:"%}

- Use a declarative tense like `ObjectBehaviorType` or `BehaviorObjectType`.
  - {% include example.md %} `TaskProcessingAnalysis` where `Object` is `Task`, `Behavior` is `Processing`, and `Type` is `Analysis`.
- Use `Analysis` when the results represent work done on the data.
- Use `Summary` when the results represent just the data.
- Use `Validation` when the results represent whether the data is valid.

{% include divider.md content="DO NOT:"%}

- Use an `Object` that does not clearly relate to a Kentico object or website concept.
- Use a `Behavior` that is opinionated, or otherwise does not relate to Kentico health, performance, or security.
- Use a new `Type` unless it represents a significantly different type of report.

## Report file structure

A typical report has the following file structure:

```
KenticoInspector.Reports
└─{ReportFolder}
  └─Metadata
    | en-US.yaml
    | ...yaml
  └─Models
    | Terms.cs
    └─Data
      | ...cs
    └─Results
      | ...cs
  └─Scripts
    | ...sql
  | Report.cs
  | Scripts.cs
```

Where each item has the following meanings, further elaborated in later sections:

| **{ReportFolder}** | The name of this folder is unique for each report and must match the C# namespace containing the report's classes. |
| **Metadata** | This contains metadata YAML files for the English US culture and optionally any other cultures. The name must be a .NET culture `xx-XX` code name. |
| **Models** | This contains minimal POCO classes representing data used by the report. It must include `Terms.cs` to match the report's metadata terms. |
| **└─Data** | (Optional) This contains minimal POCO classes representing each SQL script. Optional if the report only inspects the filesystem. |
| **└─Results** | (Optional) This contains minimal POCO classes representing each result. Optional if the data is already readable as a result. |
| **Scripts** | (Optional) This contains SQL scripts used by the report. Optional if the report only inspects the filesystem. |
| **Report.cs** | This contains the main report class. |
| **Scripts.cs** | (Optional) This contains a static class with static strings representing the paths to each SQL script. Optional if the report only inspects the filesystem. |

## Report class structure

A typical report class has the following structure:

```csharp
namespace KenticoInspector.Reports.ReportName
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IReportMetadataService reportMetadataService, IDatabaseService databaseService, ...)
            : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", ...);

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Configuration,
            ...
        };

        public override ReportResults GetResults()
        {
            var data = databaseService.ExecuteSqlFromFile<DataClass>(Scripts.GetDataWithAProblem);

            var dataResults = GetDataResults(data);

            return CompileResults(dataResults);
        }

        private static IEnumerable<DataClassResult> GetDataResults(IEnumerable<DataClass> data)
        {
            ...
        }

        private ReportResults CompileResults(IEnumerable<DataClassResult> dataResults, ...)
        {
            if (!dataResults.Any())
            {
                return new ReportResults
                {
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            return new ReportResults
            {
                Data = dataResults,
                Status = ReportResultsStatus.Error,
                Summary = Metadata.Terms.ErrorSummary
            };
        }
    }
}
```

Where the class and its members have the following meanings:

| **Report : AbstractReport\<Terms\>** | The report must inherit `AbstractReport` and uses the report's [`Terms` class](#models-structure) as the only generic parameter. |
| **readonly IDatabaseService databaseService** | Zero or more fields for each injected service except for `IReportMetadataService`. |
| **ctor : base(reportMetadataService)** | The only required service is `IReportMetadataService`, which is passed into the base class' constructor. |
| **override IList\<Version\> CompatibleVersions** | Use `VersionHelper.GetVersionList` to specify which versions the report is compatible with. |
| **override IList\<Version\> IncompatibleVersions** | Optionally specify which versions the report is incompatible with. |
| **override IList\<string\> Tags** | Specify which `ReportTags` the report should be tagged with. Tags are used in the frontend to filter reports. |
| **override ReportResults GetResults()** | The entry point of the report. |
| **static IEnumerable\<DataClassResult\> GetDataResults(...)** | One or more methods that transform a collection of data classes into their equivalent results. |
| **ReportResults CompileResults(...)** | The exit point of the report. |

<br/>
And within each method, calls have the following meanings:

| **Scripts.GetDataWithAProblem** | A reference to a script defined in the [Scripts class](#scripts-structure). |
| **Metadata.Terms** | Contains the report's metadata terms. |

<br/>
Read about the available services in detail [here](./services).

Organize `GetResults` following "get SQL data, filter data, call `CompileResults` with results class parameters"

{% include divider.md content="DO:"%}

- Try to make private methods `static`. This can help keep the code simple.
- Try to organize `GetResults` like this:
  1. Get SQL data collections.
  1. Transform SQL data collections into results collections.
  1. Call `CompileResults`, passing in all result collections.
- Try to organize `CompileResults` like this:
  1. If all results collections are empty, return a `Good` or `Information` result.
  1. Take measurements of the results.
     - {% include example.md %} Count the results or compose a time span.
  1. For any result, always return a separate `ReportResults` object rather than modifying a shared object.
- When returning `IEnumerable<DataClassResult>`, return an empty collection if there are no results to show.
- When passing anonymous objects pass them implicitly.
  - {% include example.md %} Given an object like `var errorThreshold = 5`, pass it into an anonymous object like `var anonymousObject = new { errorThreshold }`.

{% include divider.md content="DO NOT:"%}

- Return `ReportResults` directly from `GetResults`.

## Scripts structure

The `Scripts` class has the following structure:

```csharp
namespace KenticoInspector.Reports.ReportName
{
    public static class Scripts
    {
        public static string BaseDirectory = $"{nameof(ReportName)}/Scripts";

        public static string GetDataWithAProblem = $"{BaseDirectory}/{nameof(GetDataWithAProblem)}.sql";

        ...
    }
}
```

Read about the SQL scripts API in detail [here](./workingWithSQL).

{% include divider.md content="DO:"%}

- Use method-style naming for SQL scripts like `Get{DataClassName}{SummaryOfQuery}.sql` or `Get{SummaryOfQuery}.sql`.
  - {% include example.md %} A script querying page types not assigned to a site is called `GetPageTypesNotAssignedToSite.sql`.
  - {% include example.md %} A script querying unused page types is called `GetUnusedPageTypes.sql`.
- Use a SQL formatting tool to consistently format SQL scripts.

## Models structure

Only the metadata `Terms` class has a required structure: it must match the `terms` structure in the metadata YAML.

{% include example.md %} The following YAML:

```yaml
terms:
  aProblem: A problem has been found.
  summaries:
    good: No results.
```

Is represented by the following class:

```csharp
namespace KenticoInspector.Reports.ReportName.Models
{
    public class Terms
    {
        public Term AProblem { get; set; }

        public Summaries Summaries { get; set; }
    }

    public class Summaries
    {
        public Term Good { get; set; }
    }
}
```

Read about the `Term` API in detail [here](./terms).

{% include divider.md content="DO:"%}

- Name data class `{PascalCase table name}` or `{PascalCase logical object name}` without underscores.
  - {% include example.md %} Data from `CMS_User` is represented by a POCO class called `CmsUser` placed in `.\Models\Data\CmsUser.cs`.
  - {% include example.md %} Data from `View_CMS_Tree_Joined` is represented by a POCO class called `CmsDocument` placed in `.\Models\Data\CmsDocument.cs`.
- Name results class `{DataClassName}Result`.
- Use `Cms` prefix when referring to Kentico objects.
- Match property types to SQL column data type:
  - `string` for `nvarchar` or similar text types.
  - `int` for `int` or similar number types.
  - `Guid` for `uniqueidentifier`.
  - `DateTime` for `datetime2` or similar date/time types.
  - `XDocument` for data stored as XML.
- Name properties `{PascalCase column name}`.
- Include a property matching an ID column in all table results, where possible.

## Metadata structure

The metadata YAML has the following structure:

```yaml
details:
  name: ...
  shortDescription: ...
  longDescription: |
    ...
terms:
  summaries:
    information: ...
    good: ...
    ...
  tableTitles:
    ...
```

Where each item has the following meanings:

| **details** | This object is required and contains text used for displaying the report, but not the report's results. |
| **name** | This property is required and is the name of the report. Use _Sentence case_. |
| **shortDescription** | This property is required and is a one-sentence description of the report's purpose. |
| **longDescription** | This property is required and is a detailed description of what the report does. It should not duplicate `shortDescription`. The `|` allows for multiline unescaped content. |
| **terms** | This object is required and contains text used for displaying the results. |
| **summaries** | This property is recommended and contains terms used for displaying the report summaries. |
| **tableTitles** | This property is recommended and contains terms used for displaying the report's results' table titles. |

<br/>
Every value supports Markdown.

{% include divider.md content="DO:"%}

- Use bold format for Kentico application names using double stars.
- Use code format for code snippets using backticks.
- Sort all terms alphabetically.
- Name summary terms `information`, `good`, `error`, or `warning`.
- Try to structure terms where it makes sense.
