# Usage Guide

When you run the application, you will see 3 available tabs:

- [Instances](#managing-instances)
- [Reports](#reports)
- [Actions](#actions)

To begin, you must first register 1 or more instances which are stored in a local JSON file.

## Managing instances

The __Instances__ tab allows you to create and delete instances, and choose which instance is currently connected. An instance _must_ be connected in order to run reports and actions.

Click the __New instance__ button to add a new Kentico instance. You must supply the following parameters:

- __Name__: An arbitrary name for the instance
- __Administration path__: The full path to the __CMS__ folder of the Kentico administration, e.g. "C:\inetpub\wwwroot\MySite\CMS"
- __Administration URL__: The URL of the Kentico administration website

The application should automatically detect the connection string from the `web.config` at the root of the administration path. If not, you will need to provide details to connect to the Kentico database.

## Reports

Reports provide detailed information about the Kentico instance and potential issues, _without_ modifying any data. After you click the "Run" button to run the report, the status bar will change to a color indicating the status:

- :green_book: Success: There are no issues in the results
- :blue_book: Information: The results contain information for your review
- :orange_book: Warning: The results contain issues that can be reviewed, but don't necessarily need resolving
- :closed_book: Error: The results contain issues that should be resolved, either manually or by running an [action](#actions)

You can click the __Results__ tab to view the detailed information provided by the report.

## Actions

> :warning: Actions __modify the database__ and should be used with caution! Always make a backup of your database before running an action.

Clicking the "Run" button without providing options will display a list of the data that can be modified by the action. After reviewing the data on the __Results__ tab, switch to the __Options__ tab and add the action parameters. When you click "Run" again, the data will be modified and the status bar will change to a color indicating the status:

- :green_book: Success: There data was modified successfully
- :blue_book: Information: Options were not set and data can be reviewed on the Results tab
- :closed_book: Error: One or more of the options are invalid, or an exception occurred
