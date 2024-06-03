using KInspector.Core.Constants;
using KInspector.Core.Helpers;
using KInspector.Tests.Common.Helpers;
using KInspector.Reports.ColumnFieldValidation;
using KInspector.Reports.ColumnFieldValidation.Models;
using KInspector.Reports.ColumnFieldValidation.Models.Data;

using NUnit.Framework;

namespace KInspector.Tests.Common.Reports
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    [TestFixture(13)]
    public class ColumnFieldValidationTests : AbstractModuleTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsClass> ValidCmsClasses => new List<CmsClass>
        {
            new()
            {
                ClassName = "Class.1",
                ClassTableName = "Class1",
                ClassXmlSchema = FileHelper.GetXDocumentFromFile(@"Reports\TestData\CMS_Class\Class1\ClassXmlSchema.xml")
            }
        };

        private IEnumerable<CmsClass> InvalidCmsClasses => new List<CmsClass>
        {
            new()
            {
                ClassName = "Class.1",
                ClassTableName = "Class1",
                ClassXmlSchema = FileHelper.GetXDocumentFromFile(@"Reports\TestData\CMS_Class\Class1\ClassXmlSchemaWithAddedField.xml")
            }
        };

        private IEnumerable<TableColumn> ValidTableColumns => new List<TableColumn>
        {
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column1",
                Data_Type = "int"
            },
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column2",
                Data_Type = "bigint"
            },
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column3",
                Data_Type = "float"
            },
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column4",
                Data_Type = "decimal"
            },
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column5",
                Data_Type = "nvarchar"
            },
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column6",
                Data_Type = "datetime2"
            },
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column7",
                Data_Type = "bit"
            },
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column8",
                Data_Type = "varbinary"
            },
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column9",
                Data_Type = "uniqueidentifier"
            }
        };

        private IEnumerable<TableColumn> InvalidTableColumns => new List<TableColumn>(ValidTableColumns)
        {
            new()
            {
                Table_Name = "Class1",
                Column_Name = "Class1Column10",
                Data_Type = "bit"
            }
        };

        public ColumnFieldValidationTests(int majorVersion) : base(majorVersion)
        {
            mockReport = new Report(
                _mockDatabaseService.Object,
                _mockModuleMetadataService.Object
                );
        }

        [TestCase(Category = "Class and table match", TestName = "Matching class and table produce a good result")]
        public async Task Should_ReturnGoodResult_When_ClassAndTableMatch()
        {
            // Arrange
            ArrangeDatabaseService(ValidCmsClasses, ValidTableColumns);

            // Act
            var results = await mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.Summaries?.Good?.ToString()));
        }

        [TestCase(Category = "Class has added field", TestName = "Class with added field produces an error result")]
        public async Task Should_ReturnErrorResult_When_ClassHasAddedField()
        {
            // Arrange
            ArrangeDatabaseService(InvalidCmsClasses, ValidTableColumns);

            // Act
            var results = await mockReport.GetResults();

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count(), Is.EqualTo(1));
        }

        [TestCase(Category = "Table has added column", TestName = "Table with added column produces an error result")]
        public async Task Should_ReturnErrorResult_When_TableHasAddedColumn()
        {
            // Arrange
            ArrangeDatabaseService(ValidCmsClasses, InvalidTableColumns);

            // Act
            var results = await mockReport.GetResults();

            // Assert
            Assert.That(results.TableResults.Any());
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
            Assert.That(results.TableResults.FirstOrDefault()?.Rows.Count(), Is.EqualTo(1));
        }

        private void ArrangeDatabaseService(IEnumerable<CmsClass> cmsClasses, IEnumerable<TableColumn> tableColumns)
        {
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetCmsClasses, cmsClasses);

            var classTableNames = cmsClasses
                .Select(cmsClass => cmsClass.ClassTableName);

            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(
                Scripts.GetTableColumns,
                nameof(classTableNames),
                classTableNames,
                tableColumns
                );
        }
    }
}