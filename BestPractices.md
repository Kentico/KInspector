## SQL scripts
- Indent columns once, one column per line
- Indent `WHERE` conditions once, one condition per line
- Write operations `UPPERCASE`, columns `PascalCase`
- Write a blank line between operations
## Metadata
- Name reports _Sentence case_
- Use bold format for Kentico application names using `**{ApplicationName}**`
- Use code format for code snippets using `{Code}`
- Write `longDescription` as a continuation of `shortDescription` (due to how it appears in the UI)
- Sort all terms alphabetically
- Name summary terms following `[information|good|error|warning]Summary`
- Define table titles under a property named `tableNames`
- Name Table titles class `TableNamesTerms`
## Code formatting
- Sort usings with `System*` first
- Sort usings with spaces between top-level namespaces
- Sort usings alphabetically
- Use `static`, `readonly`, and remove `set`ters where possible
- Use LEGO naming for methods
- Use declarative instead of interrogative naming of variables
- Start list initializer on new line, and omit empty `()`
- Indent each parameter on new line, in signature, method call, or constructor 
- Indent each level of LINQ one tab from previous line
- Use `Any()` instead of `Count() > 0`
- Limit methods to 10-20 lines
## Reports
- Organize `GetResults` following "get SQL data, filter data, call `CompileResults` with results class parameters"
- Use static, functional data flow between `GetResults` and `CompileResults`
- Use implicit naming in object initializers in database service methods or terms 
- Order properties in `ReportResults` object initializers following `Status`, `Summary`, `Type`, `Data`
- Return separate `ReportResults` objects for each status, avoiding ternary operators in the initializers
## Data
- Place data classes under `Models\Data`
- Match data class property types to column data (`string`, `int`, `DateTime`, `XDocument`)
- Name data class `{PascalCase table name without underscores}` except for these special cases:
    - View_CMS_Tree_Joined: `CmsTreeNode`
- Name SQL script `Get{DataClassName}{SummaryOfQuery}`
- Name data class property names to match table columns exactly
- Drop _Cms_ in object names and properties (for readability)
## Results
- Place results classes under `Models\Results`
- Name results class `{DataObjectName}Results`
- Inherit related data class in results class
- Include ID column in all table results, where possible
## Tests
- Name test cases/methods following `Should_{Behavior}_When_{Case}`
- Name test data properties following `{ObjectName}[With|Without]Issue[s]`
- Match test data properties to database results as `IEnumerable<{DataClass}>`, except for special cases, even for "empty" data
- Use `Is.EqualTo()`, `Has.One.Member()`, or similar constraints
- Place first a test case without issues 