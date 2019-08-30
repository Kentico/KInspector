## SQL scripts
- Use SQL Formatting tool: <link to tool>

## Metadata
- Name reports _Sentence case_
- Use bold format for Kentico application names using `**{ApplicationName}**`
- Use code format for code snippets using `{Code}`
- Write `shortDescription` as a one sentence description of report purpose
- Write `longDescription` as a detailed description of what the report does
- `longDescription` should not duplicate `shortDescription` (due to how it appears in the UI)
- Sort all terms alphabetically
- Define summaries under a property named `summaries`
- Name Summary label model class `Summaries`
- Recommended: Name summary terms following `[information|good|error|warning]`
- Define table labels under a property named `tableLabels`
- Name Table label model class `TableLabels`

## Code formatting
- Use <tool> to format code
- Use `static`, `readonly`, and remove `set`ters where possible
- Use descriptive names. 
  - `coupledDataItem` is better than `item`
  - <method example>
- Use declarative instead of interrogative naming of variables (ex. documentIsValid is better than isDocumentValid)
- When line exceeds ~120 characters put all parameters and the closing parenthesis on their own lines 
- Indent each level of LINQ one tab from previous line
- Use `Any()` instead of `Count() > 0`
- Properties should be ordered alphabetically in an object initializer
- Recommended: Limit methods to 10-20 lines

## Reports
- Organize `GetResults` following "get SQL data, filter data, call `CompileResults` with results class parameters"
- Use static, functional data flow between `GetResults` and `CompileResults`
- When passing anonymous objects pass them implicitly
- Return separate `ReportResults` objects for each status rather than modifying a shared object

## Data
- Place data classes under `Models\Data`
- Match data class property types to column data (`string`, `int`, `DateTime`, `XDocument`)
- Name data class `{PascalCase table name or logical object name without underscores}`
  - Use `Cms` prefix when referring to CMS objects
  - Table name example: CMS_User: `CmsUser`
  - Logical name example: View_CMS_Tree_Joined: `CmsDocument`
- Use method-style naming for SQL scripts
  - Data retrieval patterns: `Get{DataClassName}{SummaryOfQuery}` or `Get{SummaryOfQuery}`
- Name data class property names to match table columns exactly

## Results
- Place results classes under `Models\Results`
- <define results class>
- Name results class `{DataObjectName}Result`
- Include ID column in all table results, where possible

## Tests
- Name test cases/methods following `Should_{Behavior}_When_{Case}`
- Name test data properties following `{ObjectName}[With|Without]{IssueDescription}`
- Match test data properties to database results as `IEnumerable<{DataClass}>`, except for special cases, even for "empty" data
- Use `Is.EqualTo()`, `Has.One.Member()`, or similar constraints
- Test cases should have a friendly name and category
