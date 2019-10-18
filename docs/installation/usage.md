---
title: Usage
parent: Installation
order: 0
---

# Usage

This is the welcome screen of Kentico Inspector:

![The welcome screen of Kentico Inspector](welcomeScreen.png#image)

## Connecting to a Kentico instance

Click **CONNECT TO GET STARTED** to show the connections screen. It will display with either no connections or any previously saved connections:

![An empty connections screen of Kentico Inspector](connectionsScreenEmpty.png#image)

{% include divider.md content="OR:"%}

![A connections screen of Kentico Inspector with connections](connectionsScreenWithConnections.png#image)

The following actions are available:

- To configure a new connection:
  1. Click **NEW CONNECTION**.
  1. Fill out the fields with details matching an Kentico instance and the SQL server that it uses.
  1. Click **CONNECT TO KENTICO INSTANCE**.
- To connect to an existing connection, click **CONNECT** at the bottom of its card.
- To delete an existing connection, click **DELETE** at the bottom of its card.

## Running reports

Once connected to a Kentico instance, the list of tested and compatible reports is shown:

![Tested and compatible reports screen](reports.png#image)

Each report shows its name, short description, and tags.

The following actions are available:

- To run any report, click <i class="play icon"></i> on the right side of any report card.
- To read the long description of a report, click <i class="chevron down icon"></i> on the right side of any report card.
- To filter reports by tag(s), click on **Show reports by tag(s)** and select one or more tags.
- To show untested reports, click on **Show untested reports**. Untested reports are reports that are not compatible and not incompatible.
- To show incompatible reports, click on **Show incompatible reports**.
- To disconnect and connect to another Kentico instance, click <i class="plug icon"></i> and then **DISCONNECT**.
- To go to the welcome screen, click <i class="home icon"></i>.

After running a report, it will show a color-coded summary:

![A sample run report](sampleReport.png#image)

The summaries are colored to match the report status:

<div class="ui blue large label">The report is informational</div>
<div class="ui green large label">The report is good</div>
<div class="ui yellow large label">The report is a warning</div>
<div class="ui red large label">The report is an error</div>
<br/>

The following actions are available:

- To see the report's results in detail, click <i class="chevron down icon"></i> on the right side of the summary.
- To rerun the report, click <i class="redo icon"></i>.

## Working with results

The following are the available result types:

- Line of text
- Table

There can be zero or more of each result type in a report's results.

### Line of text

![Line of text result](lineOfTextResult.png#image)

There is no special interaction with a line of text.

### Table

![table result](tableResult.png#image)

The following actions are available:

- To sort by a column, click on the column's header.
- To change the number of displayed rows per page, click on the selector next to **Rows per page:** in the lower right.
- To change pages, click on <i class="chevron left icon"></i> or <i class="chevron left icon"></i> in the lower right.
