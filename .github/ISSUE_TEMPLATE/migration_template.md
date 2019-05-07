---
name: V3->V4 Report Migration
about: Request a V3 module be migrated to V4

---

### Overview
We should migrate the [MODULE_NAME](LINK_TO_MODULE_CODE_FILE) module to the new V4 report format.

### Definition of done
- [ ] Code and Scripts are ported to `KenticoInspector.Reports` project in a dedicated folder for the report.
- [ ] Scripts are referenced via constants in a static class.
- [ ] Scripts are refactored to return simple results that are mappable to simple classes.
- [ ] Report logic is covered by unit test for a clean result
- [ ] Report logic is covered by unit tests for all known dirty results
- [ ] Useful, non-specific logic is abstracted to services or helpers.

_Note: The [Class/Table Validation report](https://github.com/Kentico/KInspector/tree/v4-dev/KenticoInspector.Reports/ClassTableValidation) (and it's [tests](https://github.com/Kentico/KInspector/blob/v4-dev/KenticoInspector.Reports.Tests/ClassTableValidationTests.cs)) is a good, simple example of the main concepts._

### Additional Details
- New name: __NAME__
- Tags: __tag 1, tag 2__
