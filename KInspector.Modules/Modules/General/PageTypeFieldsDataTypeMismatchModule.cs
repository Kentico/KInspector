using System;
using Kentico.KInspector.Core;

namespace Kentico.KInspector.Modules
{
    public class PageTypeFieldsDataTypeMismatchModule : IModule
    {
        public ModuleMetadata GetModuleMetadata()
        {
            return new ModuleMetadata
            { 
                Name = "Page type fields data type mismatch",
                SupportedVersions = new[] {
                    new Version("6.0"),
                    new Version("7.0"),
                    new Version("8.0"), 
                    new Version("8.1"), 
                    new Version("8.2"),
                    new Version("9.0"),
                },
                Comment = @"You may face this error when exporting/importing a site or when working with web parts / widgets that list more than one-page type. 
This error is caused by at least two different page types (for example A and B) having a field named in the same way (for example FieldName) but in each page type the field data is stored as a different data type (for example for A, it is 'Text' and for B it is 'GUID').
The best practice, in this case, is to use the page type code name as the Field name prefix. For example: CustomPageTypeA_FieldName

For more information, see https://devnet.kentico.com/articles/conversion-failed-when-converting-from-a-character-string-to-uniqueidentifier",
            };
        }

        public ModuleResults GetResults(IInstanceInfo instanceInfo)
        {
            var dbService = instanceInfo.DBService;
            var results = dbService.ExecuteAndGetTableFromFile("PageTypeFieldsDataTypeMismatchModule.sql");

            return new ModuleResults
            {
                Result = results,
                Status = results.Rows.Count > 0 ? Status.Warning : Status.Good,
            };
        }
    }
}
