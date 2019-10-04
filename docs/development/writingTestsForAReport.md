---
title: Writing tests for a report
parent: Development
order: 10
---

# Writing tests for a report

Tests are an invaluable way to ensure that a report is producing accurate and predictable results and can help identify when a report needs to be broader or narrower in its inspection.

// TODO Describe structure of a test

<div class="ui horizontal divider">DO:</div>

- Name test methods `Should_{Behavior}_When_{Case}`.
- Name test cases in a friendly way.
- Assign a category to each test case.
- Represent database test data as a property named `{DataClass}[With|Without]{IssueDescription}`.
- Represent database test data using `IEnumerable<{DataClass}>`, even for "empty" data.
- Use `Is.EqualTo()`, `Has.One.Member()`, or similar constraints.

<div class="ui horizontal divider">DO NOT:</div>

- Use similar test methods when they could be refactored into a shared method with multiple test cases.
