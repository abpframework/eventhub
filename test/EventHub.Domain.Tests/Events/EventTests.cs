using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace EventHub.Events
{
    public class EventTests : EventHubDomainTestBase
    {
        private readonly EventHubTestData _testData;

        public EventTests()
        {
            _testData = GetRequiredService<EventHubTestData>();
        }


        [Fact]
        public void Should_Not_Allow_End_Time_To_Be_Earlier_Than_Start_Time()
        {
            var exception = Assert.Throws<BusinessException>(() =>
            {
                new Event(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "1a8j3v0d",
                    "Introduction to the ABP Framework",
                    DateTime.Now,
                    DateTime.Now.AddDays(-2),
                    "In this event, we will introduce the ABP Framework and explore the fundamental features."
                );
            });

            exception.Code.ShouldBe(EventHubErrorCodes.EndTimeCantBeEarlierThanStartTime);
        }
        
        [Fact]
        public void Should_Add_Track_A_Valid_Track()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );

            @event.AddTrack(Guid.NewGuid(), "Track-1");

            @event.Tracks.ShouldContain(x => x.Name == "Track-1");
        }
        
        [Fact]
        public void Should_Not_Add_Track_For_Exist_Same_Track_Name()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );
            
            @event.AddTrack(Guid.NewGuid(), "Track-1");

            var exception = Assert.Throws<BusinessException>(() => { @event.AddTrack(Guid.NewGuid(), "Track-1"); });
            
            exception.Code.ShouldBe(EventHubErrorCodes.TrackNameAlreadyExist);
        }
        
        [Fact]
        public void Should_Update_Track_A_Valid_Track()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );

            @event.AddTrack(Guid.NewGuid(), "Track-1");
            @event.AddTrack(Guid.NewGuid(), "Track-2");
            
            @event.Tracks.ShouldContain(x => x.Name == "Track-2");
        }
        
        [Fact]
        public void Should_Not_Update_Track_For_Exist_Same_Track_Name()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );

            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");
            @event.AddTrack(Guid.NewGuid(), "Track-2");

            var exception = Assert.Throws<BusinessException>(() => { @event.UpdateTrack(track1Id, "Track-2"); });
            
            exception.Code.ShouldBe(EventHubErrorCodes.TrackNameAlreadyExist);
        }
        
        [Fact]
        public void Should_Not_Update_Track_For_Not_Exist_Track_Id()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );

            var exception = Assert.Throws<BusinessException>(() => { @event.UpdateTrack(Guid.NewGuid(), "Track-1"); });
            
            exception.Code.ShouldBe(EventHubErrorCodes.TrackNotFound);
        }

        [Fact]
        public void Should_Remove_Track_A_Valid_Track()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );

            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");
            @event.AddTrack(Guid.NewGuid(), "Track-2");

            @event.RemoveTrack(track1Id);
            
            @event.Tracks.ShouldNotContain(x => x.Name == "Track-1");
        }
        
        [Fact]
        public void Should_Not_Remove_Track_For_Not_Exist_Track_Id()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );
            
            var exception = Assert.Throws<BusinessException>(() => { @event.RemoveTrack(Guid.NewGuid()); });
            
            exception.Code.ShouldBe(EventHubErrorCodes.TrackNotFound);
        }

        [Fact]
        public void Should_Add_Session_A_Valid_Session()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );

            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");
            var session1Id = Guid.NewGuid();
            @event.AddSession(track1Id, session1Id, "Session-1 Title", "Session-1 desc".PadLeft(50, 't'), DateTime.Now, DateTime.Now, "en", new List<Guid>());

            @event.Tracks.ShouldContain(x => x.Name == "Track-1");
            var track = @event.Tracks.Single(x => x.Name == "Track-1");
            track.Sessions.ShouldContain(x => x.Id == session1Id);
        }
        
        [Fact]
        public void Should_Not_Add_Session_For_Exist_Same_Session_Name()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );
            
            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");
            @event.AddSession(track1Id, Guid.NewGuid(), "Session-1 Title", "Session-1 desc".PadLeft(50, 't'), DateTime.Now, DateTime.Now.AddHours(1), "en", new List<Guid>());

            var exception = Assert.Throws<BusinessException>(() =>
            {
                @event.AddSession(track1Id, Guid.NewGuid(), "Session-1 Title", "Session-1 desc".PadLeft(50, 't'), DateTime.Now.AddHours(2), DateTime.Now.AddHours(3), "en", new List<Guid>());
            });
            
            exception.Code.ShouldBe(EventHubErrorCodes.SessionTitleAlreadyExist);
        }
        
        [Fact]
        public void Should_Not_Add_Session_For_End_Time_To_Be_Earlier_Than_Start_Time()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );
            
            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");
            var exception = Assert.Throws<BusinessException>(() =>
            {
                @event.AddSession(track1Id, Guid.NewGuid(), "Session-1 Title", "Session-1 desc".PadLeft(50, 't'), DateTime.Now.AddHours(2), DateTime.Now.AddHours(1), "en", new List<Guid>());
            });
            
            exception.Code.ShouldBe(EventHubErrorCodes.SessionEndTimeCantBeEarlierThanStartTime);
        }
        
        [Fact]
        public void Should_Not_Add_Session_For_Session_Time_Is_Not_In_Event_Time()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );
            
            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");
            var exception = Assert.Throws<BusinessException>(() =>
            {
                @event.AddSession(track1Id, Guid.NewGuid(), "Session-1 Title", "Session-1 desc".PadLeft(50, 't'), DateTime.Now.AddHours(2), DateTime.Now.AddDays(3), "en", new List<Guid>());
            });
            
            exception.Code.ShouldBe(EventHubErrorCodes.SessionTimeShouldBeInTheEventTime);
        }
        
        [Fact]
        public void Should_Not_Add_Session_For_Session_Time_Conflicts_With_An_Existing_Session()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );
            
            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");
            
            @event.AddSession(track1Id, Guid.NewGuid(), "Session-1 Title", "Session-1 desc".PadLeft(50, 't'), DateTime.Now, DateTime.Now.AddDays(1), "en", new List<Guid>());

            var exception = Assert.Throws<BusinessException>(() =>
            {
                @event.AddSession(track1Id, Guid.NewGuid(), "Session-2 Title", "Session-2 desc".PadLeft(50, 't'), DateTime.Now, DateTime.Now.AddDays(1), "en", new List<Guid>());
            });
            
            exception.Code.ShouldBe(EventHubErrorCodes.SessionTimeConflictsWithAnExistingSession);
        }
        
        [Fact]
        public void Should_Add_Session_With_Speakers()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );
            
            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");

            var speakerUserIds = new List<Guid>();
            speakerUserIds.Add(_testData.UserAdminId);
            speakerUserIds.Add(_testData.UserJohnId);

            var sessionId = Guid.NewGuid();
            @event.AddSession(track1Id, sessionId, "Session-1 Title", "Session-1 desc".PadLeft(50, 't'), DateTime.Now, DateTime.Now.AddDays(1), "en", speakerUserIds);
            
            var track = @event.Tracks.Single(x => x.Id == track1Id);
            var session = track.Sessions.Single(x => x.Id == sessionId);
            
            session.Speakers.ShouldContain(x => x.UserId == _testData.UserAdminId);
            session.Speakers.ShouldContain(x => x.UserId == _testData.UserJohnId);
        }
        
        [Fact]
        public void Should_Update_Session_With_Valid_Session()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );
            
            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");

            var speakerUserIds = new List<Guid>();
            speakerUserIds.Add(_testData.UserAdminId);
            speakerUserIds.Add(_testData.UserJohnId);

            var sessionId = Guid.NewGuid();
            @event.AddSession(track1Id, sessionId, "Session-1 Title", "Session-1 desc".PadLeft(50, 't'), @event.StartTime.AddMinutes(30), @event.EndTime.AddHours(-1), "en", speakerUserIds);
            
            var track = @event.Tracks.Single(x => x.Id == track1Id);
            var session = track.Sessions.Single(x => x.Id == sessionId);
            session.Title.ShouldContain("Session-1 Title");
            
            
            speakerUserIds = new List<Guid>();
            speakerUserIds.Add(_testData.UserAdminId);
            @event.UpdateSession(track1Id, sessionId, "Session-1 Title Updated", "Session-1 desc".PadLeft(50, 't'), @event.StartTime.AddMinutes(30), @event.EndTime.AddMinutes(-70), "en", speakerUserIds);

            track = @event.Tracks.Single(x => x.Id == track1Id);
            session = track.Sessions.Single(x => x.Id == sessionId);
            session.Title.ShouldContain("Session-1 Title Updated");
            session.Speakers.ShouldContain(x => x.UserId == _testData.UserAdminId);
            session.Speakers.ShouldNotContain(x => x.UserId == _testData.UserJohnId);
        }
        
        [Fact]
        public void Should_Remove_Session_A_Valid_Session()
        {
            var @event = new Event(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "1a8j3v0d",
                "Introduction to the ABP Framework",
                DateTime.Now,
                DateTime.Now.AddDays(2),
                "In this event, we will introduce the ABP Framework and explore the fundamental features."
            );
            
            var track1Id = Guid.NewGuid();
            @event.AddTrack(track1Id, "Track-1");

            var speakerUserIds = new List<Guid>();
            speakerUserIds.Add(_testData.UserAdminId);
            speakerUserIds.Add(_testData.UserJohnId);

            var sessionId = Guid.NewGuid();
            @event.AddSession(track1Id, sessionId, "Session-1 Title", "Session-1 desc".PadLeft(50, 't'), DateTime.Now, DateTime.Now.AddDays(1), "en", speakerUserIds);

            speakerUserIds = new List<Guid>();
            speakerUserIds.Add(_testData.UserAdminId);
            @event.RemoveSession(track1Id, sessionId);

            var track = @event.Tracks.Single(x => x.Id == track1Id);
            track.Sessions.ShouldNotContain(x => x.Id == sessionId);
        }
    }
}
