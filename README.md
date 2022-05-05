# EventHub

[![.NET](https://github.com/volosoft/eventhub/actions/workflows/dotnet.yml/badge.svg)](https://github.com/volosoft/eventhub/actions/workflows/dotnet.yml)

This is a reference application built with the ABP Framework. It implements the Domain Driven Design with multiple application layers.

## The book

This solution is originally prepared to be a real-world example for the **Mastering ABP Framework** book.

![abp-book](etc/images/abp-book.png)

**The book is the only source that explains this solution**. This solution is highly referred in *Understanding the Reference Solution*, *Domain Driven Design* and other parts of the book.

**You can order the book on [Amazon](https://www.amazon.com/gp/product/B097Z2DM8Q) or on [Packt's website](https://www.packtpub.com/product/mastering-abp-framework/9781801079242).**

## Requirements

* .NET 6.0+
* Docker

## How to run

* Execute `dotnet build /graphBuild` command in the root folder of the solution.
* Execute `etc/docker/up.ps1` to run the depending services.
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

#### Event Creation Page

The event creation process consists of three steps: "Create a New Event", "Add Tracks to the Event (optional)" and "Add Sessions to the Tracks (optional)".

* After these steps, an "Event Preview" page is shown to the user to check the event details and publish the event.

##### Create a New Event

![Event Creation Page](etc/images/event-creation-page.png)

##### Add Tracks to the Event (optional)

![Event Creation Page - Tracks](etc/images/event-creation-page-tracks.png)

##### Add Sessions to the Tracks (optional)

![Event Creation Page - Sessions](etc/images/event-creation-page-sessions.png)

#### New Event Preview Page

![Event Creation Page - Preview](etc/images/event-creation-page-preview.png)

#### Events Page

![Events Page](etc/images/events-page.png)

#### Event Details Page

![Event Detail](etc/images/event-detail.png)

#### Organizations Page

![Organizations Page](etc/images/organizations-page.png)

#### Organization Details Page

![Organization Detail Page](etc/images/organization-detail-page.png)

#### Profile Page

![Profile Page](etc/images/profile-page.png)

#### Payment Module Pages

The payment module provides an API to make payments via **PayPal** easily. This application uses this module to perform payment transactions.

> To learn more about the **Payment Module** and see the integration, please check out the [payment module documentation](modules/payment/README.md).

##### Pricing Page

![Pricing Page](etc/images/pricing-page.png)

#### Pre-Checkout Page

![Pre Checkout Page](etc/images/pre-checkout-page.png)
