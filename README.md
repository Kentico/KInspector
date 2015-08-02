# KInspector
[![Build status](https://ci.appveyor.com/api/projects/status/udykjx510v83w9y6?svg=true)](https://ci.appveyor.com/project/kentico/kinspector)

KInspector is an application for analyzing health, performance and security of your **Kentico** solution. 

It was developed by the consulting department to improve the web sites of Kentico customers. Initially, it was an internal application, but we think that every Kentico user can benefit from this app and that's why we made it open source. 

The application contains three types of modules:
- **Setup** modules can help you prepare your instance for testing. You can disable SMTP servers, web farm servers, add localhost license and so on.
- **Analysis** modules checks for the health and performance. It tests database for consistency issues, display common event log errors and recommend you some best practices.
- **Security** modules scans for potential security issues like XSS or SQL injection throughout the system.

The best thing about KInspector is that it can analyze **any version** of Kentico. 


## Get the application

It's super easy. Download the [latest release](https://github.com/Kentico/KInspector/releases/latest) zip package, unpack it and run the ```Start.cmd```. It automatically opens a new browser window with the [target setup](http://i.imgur.com/4n5s56z.png) page. 
> Make sure that you provide all the fields with the correct value. Some of the modules work with the database and some of them with a code. If you don't fill the values correctly, it will probably throw an error.

Once you pass the setup page, you're ready to start analyze. On a [main menu](http://i.imgur.com/H7zBQOZ.png) page, select ```Analysis``` category and run the modules. You will see a page similar to this one

[![Module results](http://i.imgur.com/UUdTlNL.png)](http://i.imgur.com/Vti1Fo7.png)

Now you can start implement suggested improvements.

## Contributing
Want improve the KInspector? Great! Read the [contributing guidelines](https://github.com/Kentico/KInspector/blob/master/CONTRIBUTING.md) and then [write your first module](https://github.com/Kentico/KInspector/wiki/Writing-a-custom-module) or improve the existing one.

If anything feels wrong or incomplete, please let us know. Create a new [issue](https://github.com/Kentico/KInspector/issues/new) or submit a [pull request](https://help.github.com/articles/using-pull-requests/).
