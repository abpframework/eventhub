# EventHub

[![.NET](https://github.com/volosoft/eventhub/actions/workflows/dotnet.yml/badge.svg)](https://github.com/volosoft/eventhub/actions/workflows/dotnet.yml)

This is a reference application built with the ABP Framework. It implements Domain Driven Design with multiple application layers.

## The book

This solution is originally prepared to be a real-world example for the **Mastering ABP Framework** book.

![abp-book](etc/images/abp-book.png)

**The book is the only source that explain the solution**. The solution is highly referred in the *Understanding the Reference Solution*, *Domain Driven Design* and other parts of the book.

**You can order the book on [Amazon](https://www.amazon.com/gp/product/B097Z2DM8Q) or on [Packt's website](https://www.packtpub.com/product/mastering-abp-framework/9781801079242).**

## How to run

* Run `EventHub.DbMigrator` to create the database.
* Run `etc/docker/up.ps1` before running the solution.
* Run `EventHub.IdentityServer`
* Run `EventHub.HttpApi.Host`
* Run `EventHub.Web`
* Run `EventHub.Admin.HttpApi.Host`
* Run `EventHub.Web.Admin`

`admin` user's password is `1q2w3E*`

## See live

See the solution live on https://openeventhub.com
