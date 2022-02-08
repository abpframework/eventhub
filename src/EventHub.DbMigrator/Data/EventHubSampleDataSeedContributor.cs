#if DEBUG //ONLY FOR DEBUG MODE!
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHub.Events;
using EventHub.Events.Registrations;
using EventHub.Organizations;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace EventHub.DbMigrator.Data
{
    public class EventHubSampleDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<Event, Guid> _eventRepository;
        private readonly EventManager _eventManager;
        private readonly EventRegistrationManager _eventRegistrationManager;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        private Guid _userSandraId;
        private Guid _userSergeyId;
        private Guid _userWellyId;
        private Guid _userAlessandroId;
        private Guid _userMarkId;
        private Guid _userTonyId;
        private Guid _organizationVolosoftId;
        private Guid _organizationAbpId;
        private Guid _organizationAngularCoderId;
        private Guid _organizationDotnetWorldId;
        private Guid _organizationDeveloperDaysId;
        private Guid _organizationCSharpLoversId;

        public EventHubSampleDataSeedContributor(
            IGuidGenerator guidGenerator,
            IdentityUserManager identityUserManager,
            IRepository<Organization, Guid> organizationRepository,
            IRepository<Event, Guid> eventRepository,
            EventManager eventManager,
            EventRegistrationManager eventRegistrationManager,
            IRepository<IdentityUser, Guid> userRepository)
        {
            _guidGenerator = guidGenerator;
            _identityUserManager = identityUserManager;
            _organizationRepository = organizationRepository;
            _eventRepository = eventRepository;
            _eventManager = eventManager;
            _eventRegistrationManager = eventRegistrationManager;
            _userRepository = userRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _userRepository.GetCountAsync() <= 1)
            {
                await CreateSampleUsersAsync();
                await CreateSampleOrganizationsAsync();
                await CreateSampleEventsAsync();
            }
        }

        private async Task CreateSampleUsersAsync()
        {
            var users = new List<IdentityUser>
            {
                new IdentityUser(_userSandraId = _guidGenerator.Create(), "SandraWolf", "sandra_wolf@gmail.com"),
                new IdentityUser(_userSergeyId = _guidGenerator.Create(), "SergeyPolgul", "sergey_polgul@gmail.com"),
                new IdentityUser(_userWellyId = _guidGenerator.Create(), "WellyTambunan", "welly_tambunan@gmail.com"),
                new IdentityUser(_userAlessandroId = _guidGenerator.Create(), "AlessandroMuci", "alessandro_muci@gmail.com"),
                new IdentityUser(_userMarkId = _guidGenerator.Create(), "MarkGodfrey", "mark_godfrey@gmail.com"),
                new IdentityUser(_userTonyId = _guidGenerator.Create(), "TonyBurton", "tony_burton@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "DavidGraus", "david_graus@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "DavidRoss", "david_ross@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "TomLidy", "tom_lidy@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "ShreePatel", "shree_patel@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "JanneSpijkervet", "janne_spijkervet@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "AngieHaynes", "angie_haynes@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "DeanHeasman", "dean_heasman@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "SergeyPerepelkin", "sergey_perepelkin@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "RobertKelleman", "robert_kelleman@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "CarolConlon", "carol_conlon@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "RyanPonder", "ryan_ponder@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "RogerBailey", "roger_bailey@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "LylianPortes", "lylian_portres@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "SimonVendrov", "simon_vendrov@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "NicolaEdwards", "nicola_edwards@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "TonyLee", "tony_lee@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "KellyDixon", "kelly_dixon@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "Maria", "maria_richardson@gmail.com"),
                new IdentityUser(_guidGenerator.Create(), "ScottHanselman", "scott_hanselman@gmail.com"),
            };

            foreach (var user in users)
            {
                (await _identityUserManager.CreateAsync(user, "1q2w3E*")).CheckErrors();
            }
        }

        private async Task CreateSampleOrganizationsAsync()
        {
            if (await _organizationRepository.GetCountAsync() > 0)
            {
                return;
            }

            var organizations = new List<Organization>
            {
                new Organization(_organizationVolosoftId = Guid.NewGuid(), _userSandraId, "volo",
                    "Volosoft",
                    "Volosoft is a company that develops community-driven open source projects and commercial projects. In this organization, you'll get the latest information about Volosoft products such as ABP framework, AspNetZero etc."),
                new Organization(_organizationAbpId = Guid.NewGuid(), _userSergeyId, "abp", "ABP",
                    "This group was created to share common information about the ABP framework."),
                new Organization(_organizationAngularCoderId = Guid.NewGuid(), _userWellyId,
                    "ng-coders", "Angular Coders",
                    "Angular Coders was created with the goal of sharing Angular knowledge and best-practices as well as providing a space where people can meet up and chat with others working with Angular."),
                new Organization(_organizationDotnetWorldId = Guid.NewGuid(), _userAlessandroId,
                    "dotnet-world", "Dotnet World",
                    "Dotnet World is a place for local technology enthusiasts to connect and collaborate around the .NET ecosystem. All that's required is an interest in .NET and a willingness to learn and share."),
                new Organization(_organizationDeveloperDaysId = Guid.NewGuid(), _userMarkId,
                    "developer-days", "Developer Days",
                    "Developer Days is a platform of resources to help you further your application development! It is intended to further expand your application development and DevOps skills through workshops, tech talks, and social engagements."),
                new Organization(_organizationCSharpLoversId = Guid.NewGuid(), _userTonyId,
                    "csharp-lovers", "C# Lovers",
                    "This group was created with the goal of sharing the latest and best information about C# language. At our monthly meetings you can hear expert speakers who dig 'under the hood' and keep you informed on best practices and future directions.")
            };

            await _organizationRepository.InsertManyAsync(organizations, autoSave: true);
        }

        private async Task CreateSampleEventsAsync()
        {
            if (await _eventRepository.GetCountAsync() > 0)
            {
                return;
            }

            var organizationDotnetWorld = await _organizationRepository.GetAsync(_organizationDotnetWorldId);
            var organizationDeveloperDays = await _organizationRepository.GetAsync(_organizationDeveloperDaysId);
            var organizationCSharpLovers = await _organizationRepository.GetAsync(_organizationCSharpLoversId);
            var organizationAngularCoders = await _organizationRepository.GetAsync(_organizationAngularCoderId);
            var organizationVolosoft = await _organizationRepository.GetAsync(_organizationVolosoftId);
            var organizationAbp = await _organizationRepository.GetAsync(_organizationAbpId);

            var userSandra = await _userRepository.GetAsync(_userSandraId);
            var userTony = await _userRepository.GetAsync(_userTonyId);
            var userAlessandro = await _userRepository.GetAsync(_userAlessandroId);
            var userMark = await _userRepository.GetAsync(_userMarkId);

            //10 past events
            var pastEvent1 = await _eventManager.CreateAsync(organizationDotnetWorld,
                "Identity & Access Control for modern Applications and APIs using ASP.NET Core 5",
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(-1),
                "Modern application design has changed quite a bit in recent years. \"Mobile-first\" and \"cloud-ready\" are the types of applications you are expected to develop. Also, to keep pace with these demands, Microsoft has revamped their complete web stack with ASP.NET Core to meet these architectural demands."
            );
            await _eventManager.SetLocationAsync(pastEvent1, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent1);

            var pastEvent2 = await _eventManager.CreateAsync(organizationDotnetWorld,
                "Modern distributed systems with gRPC in ASP.NET Core 5",
                DateTime.Now.AddDays(-20),
                DateTime.Now.AddDays(-2),
                "gRPC is a high-performance, cross-platform framework for building distributed systems and APIs. It’s an ideal choice for communication between microservices, internal network applications, or mobile devices and services.");
            await _eventManager.SetLocationAsync(pastEvent2, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent2);

            var pastEvent3 = await _eventManager.CreateAsync(organizationDotnetWorld,
                "Practical Clean Architecture with .NET 5",
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(-7),
                "The explosive growth of web frameworks and the demands of users have changed the approach to building enterprise applications. Many challenges exist and just getting started can be a daunting prospect. Let's change that now.");
            await _eventManager.SetLocationAsync(pastEvent3, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent3);

            var pastEvent4 = await _eventManager.CreateAsync(organizationDeveloperDays,
                "Domain Modeling Made Functional",
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(-7),
                "Functional programming and domain-driven design might not seem to be a good match, but in fact functional programming can be an excellent approach to designing decoupled, reusable systems with a rich domain model. ");
            await _eventManager.SetLocationAsync(pastEvent4, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent4);

            var pastEvent5 = await _eventManager.CreateAsync(organizationCSharpLovers,
                "Getting started with GraphQL on .NET",
                DateTime.Now.AddDays(-32),
                DateTime.Now.AddDays(-10),
                "GraphQL is a query language for APIs and a runtime for fulfilling those queries with your existing data. That sounds nice, but what is GraphQL and how can we use it in .NET?");
            await _eventManager.SetLocationAsync(pastEvent5, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent5);

            var pastEvent6 = await _eventManager.CreateAsync(organizationDotnetWorld,
                "A Year Long Journey from .NET Framework to .NET Core",
                DateTime.Now.AddDays(-35),
                DateTime.Now.AddDays(-15),
                "In this session we will walk through a real world example of a year-long modernization journey. We will take a closer look at how you can get started, what obstacles you may face and how to overcome them.");
            await _eventManager.SetLocationAsync(pastEvent6, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent6);

            var pastEvent7 = await _eventManager.CreateAsync(organizationDeveloperDays,
                "March: Building Realtime Serverless APIs with GraphQL",
                DateTime.Now.AddDays(-25),
                DateTime.Now.AddDays(-14),
                "GraphQL is making a huge splash in modern serverless applications. GraphQL allows you to present custom apis that allow the consumer of the API to decide what information they want and how it should be shaped. It also allows for real time pushes of data using web sockets and subscriptions. In this talk, you will learn how to implement this powerful API in the cloud using serverless functions in Azure.");
            await _eventManager.SetLocationAsync(pastEvent7, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent7);

            var pastEvent8 = await _eventManager.CreateAsync(organizationDotnetWorld,
                ".NET Core Dependency Injection - The Booster Jab",
                DateTime.Now.AddDays(-25),
                DateTime.Now.AddDays(-10),
                "With the releases of .NET Core 2.x, 3.x and now .NET 5, more developers have now got to grips with the basics of using the default Microsoft Dependency Injection container that comes with .NET Core. However, the story does not end there...");
            await _eventManager.SetLocationAsync(pastEvent8, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent8);

            var pastEvent9 = await _eventManager.CreateAsync(organizationAngularCoders,
                "[Front End] Real life introduction to unit testing in Angular",
                DateTime.Now.AddDays(-15),
                DateTime.Now.AddDays(-2),
                "How many times have you tried to start writing unit tests and didn’t know where to start? Or the tutorials weren’t clear enough? In our webinar we’ll start with just a bit of theory and then we’ll move on to examples. We’ll show you how to test basic (and a bit more complex) components, services, how to deal with dependencies, asynchronous code, forms, child components etc");
            await _eventManager.SetLocationAsync(pastEvent9, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent9);

            var pastEvent10 = await _eventManager.CreateAsync(organizationAngularCoders,
                "Introduction to Angular Unit Tests",
                DateTime.Now.AddDays(-45),
                DateTime.Now.AddDays(-43),
                "Introduction to Angular unit tests includes introduction to Jasmin and Angular testing module. You will learn to create unit tests for your Angular project by understanding Jasmin and Angular testing module which provides infrastructure for testing Angular core functionality.");
            await _eventManager.SetLocationAsync(pastEvent10, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(pastEvent10);

            //15 upcoming events
            var upcomingEvent1 = await _eventManager.CreateAsync(organizationDotnetWorld,
                "ASP.NET Core Health Checks",
                DateTime.Now,
                DateTime.Now.AddDays(7),
                "In the microservice environment, it is important to know the health state of the different services and handle bad states. This is why Microsoft created the ASP.NET Core Health Checks.");
            await _eventManager.SetLocationAsync(upcomingEvent1, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent1);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent1, userSandra);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent1, userAlessandro);

            var upcomingEvent2 = await _eventManager.CreateAsync(organizationDotnetWorld,
                "GraphQL in a .NET world",
                DateTime.Now,
                DateTime.Now.AddDays(7),
                "Grab some snacks and a drink, and tech along to our online GraphQL meetup! GraphQL is all the rage nowadays, especially in the frontend community. Although Microsoft has been silent about GraphQL, the open-source community is slowly picking up the task of integrating GraphQL tech in .NET Core. This presentation will be an introduction to GraphQL for anyone who has never tried it.");
            await _eventManager.SetLocationAsync(upcomingEvent2, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent2);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent2, userTony);

            var upcomingEvent3 = await _eventManager.CreateAsync(organizationCSharpLovers,
                "C# 9 and .NET 5",
                DateTime.Now,
                DateTime.Now.AddDays(15),
                "First, we will present .NET 5, it's importance in the .NET evolution, and the reasons to switch your new development to .NET 5. We will pay close attention to the challenging topic of migration to .NET 5 and provide a few guidelines. Afterward, we will dig into C# 9.0 and see how it makes us more productive and enables us to write more expressive code.");
            await _eventManager.SetLocationAsync(upcomingEvent3, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent3);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent3, userTony);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent3, userAlessandro);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent3, userSandra);

            var upcomingEvent4 = await _eventManager.CreateAsync(organizationDotnetWorld,
                "Level Up Your Web Dev with Blazor and .NET 5",
                DateTime.Now,
                DateTime.Now.AddDays(10),
                "Join us for a discussion and live coding session where Jeff Fritz will take us on a tour of a more complex Blazor Static Web App. We'll learn how to take advantage of other Azure Services without breaking the bank while delivering cool features like search, caching, and using event-driven architecture.");
            await _eventManager.SetLocationAsync(upcomingEvent4, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent4);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent4, userMark);

            var upcomingEvent5 = await _eventManager.CreateAsync(organizationVolosoft,
                "Implementing Domain Driven Design",
                DateTime.Now,
                DateTime.Now.AddDays(30),
                "This talk starts by introducing the DDD and providing a layering model based on the DDD and the Clean Architecture. It then introduces the core building of an application built on the DDD principles. In the second part of the talk, it shows some strict coding rules for the core building blocks with real code examples and suggestions. These rules are essential to build a large scale application implements DDD patterns & practices.");
            await _eventManager.SetLocationAsync(upcomingEvent5, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent5);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent5, userMark);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent5, userAlessandro);

            var upcomingEvent6 = await _eventManager.CreateAsync(organizationAbp,
                "ABP: Open source web application framework for ASP.NET Core",
                DateTime.Now,
                DateTime.Now.AddDays(30),
                "ABP Framework is a complete infrastructure to create modern web applications by following the software development best practices and conventions. In this talk we'll examine ABP Framework top to down.");
            await _eventManager.SetLocationAsync(upcomingEvent6, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent6);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent6, userAlessandro);

            var upcomingEvent7 = await _eventManager.CreateAsync(organizationDotnetWorld,
                ".NET Dependency Injection Tips and Tricks",
                DateTime.Now,
                DateTime.Now.AddDays(30),
                "This is a Zoom Meeting and the link will be provided the day of the event via email and 1 hour before the meeting start. We'll start with quick intro and then we'll deep dive into .NET dependency injection system.");
            await _eventManager.SetLocationAsync(upcomingEvent7, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent7);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent7, userSandra);

            var upcomingEvent8 = await _eventManager.CreateAsync(organizationDeveloperDays,
                "Clean Architecture",
                DateTime.Now.AddDays(5),
                DateTime.Now.AddDays(15),
                "Recently Bob Martin has categorized a set of architectures, including hexagonal architecture, onion architecture and screaming architecture as 'the clean architecture' - a layered architecture of concentric circles with a strong emphasis on separation of concerns. This architecture has become popular because of its amenability to modification as an evolutionary architecture and its support for practices such as TDD.In this presentation we will discuss the clean architecture and its benefits");
            await _eventManager.SetLocationAsync(upcomingEvent8, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent8);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent8, userSandra);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent8, userAlessandro);

            var upcomingEvent9 = await _eventManager.CreateAsync(organizationAngularCoders,
                "Angular on the rocks - There are no bad practices!",
                DateTime.Now.AddDays(5),
                DateTime.Now.AddDays(10),
                "There are no bad practices. Just trade-offs.Let's dive into the real world! because sometimes, a 'Bad practice' is not optional...The trick is to learn and make sure you understand what's going -on, and how to it right!We will use jQuery in Angular, work with promises instead of RxJS, Pass callbacks as Inputs - and much more!");
            await _eventManager.SetLocationAsync(upcomingEvent9, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent9);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent9, userAlessandro);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent9, userMark);

            var upcomingEvent10 = await _eventManager.CreateAsync(organizationAngularCoders,
                "Google Developers Experts Tech Talks",
                DateTime.Now.AddDays(4),
                DateTime.Now.AddDays(5),
                "This meetup will explore an approach to iteratively and rigorously testing mainframe migrations to cloud, all while building “living documentation” of the old and new systems. It will discuss how visual data flows build accurate logical pictures of legacy systems, while one flow can generate automated tests for both legacy and new components. ");
            await _eventManager.SetLocationAsync(upcomingEvent10, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent10);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent10, userAlessandro);

            var upcomingEvent11 = await _eventManager.CreateAsync(organizationAngularCoders,
                "TypeScript Online Meetup",
                DateTime.Now.AddDays(7),
                DateTime.Now.AddDays(14),
                "In this talk, we'll go through Typescript top to down and understand what it solved for software applications");
            await _eventManager.SetLocationAsync(upcomingEvent11, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent11);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent11, userMark);

            var upcomingEvent12 = await _eventManager.CreateAsync(organizationAngularCoders,
                "2021 - New Dawn",
                DateTime.Now.AddDays(1),
                DateTime.Now.AddDays(2),
                "Cypress is a next-generation front end testing tool built for the modern web. They address the key pain points developers and QA engineers face when testing modern applications.In this session, we will learn how to add cypress to angular projects, setup, write, and run cypress tests.");
            await _eventManager.SetLocationAsync(upcomingEvent12, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent12);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent12, userMark);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent12, userAlessandro);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent12, userTony);

            var upcomingEvent13 = await _eventManager.CreateAsync(organizationAngularCoders,
                "AngularUP 2021",
                DateTime.Now.AddDays(10),
                DateTime.Now.AddDays(12),
                "Some of the topics covered in the conference: Angular core topicsThe future of Angular and Angular migration path, Bundling & Packaging, Rendering on server and mobile, Performance, Tools & The ecosystem");
            await _eventManager.SetLocationAsync(upcomingEvent13, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent13);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent13, userAlessandro);

            var upcomingEvent14 = await _eventManager.CreateAsync(organizationDeveloperDays,
                "Thinkful Webinar | Data Science vs. Data Analytics",
                DateTime.Now.AddDays(5),
                DateTime.Now.AddDays(12),
                "We’ll start by going over the differences between the two careers. Then, we’ll walk you through how to get the skills to be successful in each, and discuss the different jobs that will be available to you once you’ve acquired those skills.");
            await _eventManager.SetLocationAsync(upcomingEvent14, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent14);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent14, userTony);

            var upcomingEvent15 = await _eventManager.CreateAsync(organizationDeveloperDays,
                "DevFest Silicon Valley",
                DateTime.Now.AddDays(36),
                DateTime.Now.AddDays(50),
                "Mark your calendars! Join us for DevFest Silicon Valley, virtually from anywhere around the world! DevFest brings together thousands of developers from all different backgrounds and skill levels who have a shared passion for Google technologies to teach, learn, and connect. Let's keep the momentum going for DevFest 2021!");
            await _eventManager.SetLocationAsync(upcomingEvent15, true, "https://www.youtube.com/" ,null, null);
            await _eventRepository.InsertAsync(upcomingEvent15);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent15, userTony);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent15, userAlessandro);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent15, userMark);
            await _eventRegistrationManager.RegisterAsync(upcomingEvent15, userSandra);
        }
    }
}
#endif
