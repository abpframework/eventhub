# EventHub

[![.NET](https://github.com/volosoft/eventhub/actions/workflows/dotnet.yml/badge.svg)](https://github.com/volosoft/eventhub/actions/workflows/dotnet.yml)

This is a reference application built with the ABP Framework. It implements Domain Driven Design with multiple application layers.

## The book

This solution is originally prepared to be a real-world example for the **Mastering ABP Framework** book.

![abp-book](etc/images/abp-book.png)

**The book is the only source that explains the solution**. The solution is highly referred in *Understanding the Reference Solution*, *Domain Driven Design* and other parts of the book.

**You can order the book on [Amazon](https://www.amazon.com/gp/product/B097Z2DM8Q) or on [Packt's website](https://www.packtpub.com/product/mastering-abp-framework/9781801079242).**

## Requirements

* .NET 6.0+
* Docker

## How to run

* Run `etc/docker/up.ps1` before running the solution.
* Run `EventHub.DbMigrator` to create the database and seed initial data.
* Run `EventHub.IdentityServer`
* Run `EventHub.HttpApi.Host`
* Run `EventHub.Web`
* Run `EventHub.Admin.HttpApi.Host`
* Run `EventHub.Web.Admin`

`admin` user's password is `1q2w3E*`

## See live

See the solution live on https://openeventhub.com

## Screenshots

### Public Web Side - (MVC/Razor Page UI)

#### Home Page

![Home Page](etc/images/homepage.png)

#### Events Page

![Events Page](etc/images/events-page.png)

#### Event Detail Page

![Event Detail](etc/images/event-detail.png)

#### Organizations Page

![Organizations Page](etc/images/organizations-page.png)

#### Profile Page

![Profile Page](etc/images/profile-page.png)

#### Payment Module Pages

The payment module provides an API to make payments via using **PayPal** easily. This application uses this module to perform payment transactions.

> To learn more about the **Payment Module** and see the integration, please see the [payment module documentation](modules/payment/README.md).

##### Pricing Page

![Pricing Page](etc/images/pricing-page.png)

#### Pre Checkout Page

![Pre Checkout Page](etc/images/pre-checkout-page.png)