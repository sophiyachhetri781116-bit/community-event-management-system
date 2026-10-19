using CommunityEventManagement.Data;
using CommunityEventManagement.Data.Models;
using CommunityEventManagement.Exceptions;
using CommunityEventManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace CommunityEventManagement.Tests
{
    // Helper: builds an isolated in-memory database for each test
    public class TestDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public TestDbContextFactory()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        public ApplicationDbContext CreateDbContext() => new ApplicationDbContext(_options);
    }

    [TestClass]
    public class EventServiceTests
    {
        [TestMethod]
        public void Add_ThenGetList_ReturnsTheEvent()
        {
            var factory = new TestDbContextFactory();
            var service = new EventService(factory);
            var newEvent = new Event
            {
                Name = "Test Event",
                Date = DateOnly.FromDateTime(DateTime.Today),
                Time = new TimeOnly(9, 0),
                Description = "A test event"
            };

            service.Add(newEvent);
            var result = service.GetList();

            Assert.HasCount(1, result);
            Assert.AreEqual("Test Event", result[0].Name);
        }

        [TestMethod]
        public void GetUpcomingEvents_ExcludesPastEvents()
        {
            var factory = new TestDbContextFactory();
            var service = new EventService(factory);
            service.Add(new Event
            {
                Name = "Past Event",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
                Time = new TimeOnly(9, 0),
                Description = "Already happened"
            });
            service.Add(new Event
            {
                Name = "Future Event",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                Time = new TimeOnly(9, 0),
                Description = "Coming up"
            });

            var upcoming = service.GetUpcomingEvents();

            Assert.HasCount(1, upcoming);
            Assert.AreEqual("Future Event", upcoming[0].Name);
        }

        [TestMethod]
        public void GetUpcomingEvents_WithMaxCount_LimitsResults()
        {
            var factory = new TestDbContextFactory();
            var service = new EventService(factory);
            for (int i = 1; i <= 3; i++)
            {
                service.Add(new Event
                {
                    Name = $"Event {i}",
                    Date = DateOnly.FromDateTime(DateTime.Today.AddDays(i)),
                    Time = new TimeOnly(9, 0),
                    Description = "Upcoming"
                });
            }

            var limited = service.GetUpcomingEvents(2);

            Assert.HasCount(2, limited);
        }

        [TestMethod]
        public void GetUpcomingEvents_IncludesEventsToday()
        {
            var factory = new TestDbContextFactory();
            var service = new EventService(factory);
            service.Add(new Event
            {
                Name = "Today Event",
                Date = DateOnly.FromDateTime(DateTime.Today),
                Time = new TimeOnly(9, 0),
                Description = "Happening today"
            });

            var upcoming = service.GetUpcomingEvents();

            Assert.HasCount(1, upcoming);
            Assert.AreEqual("Today Event", upcoming[0].Name);
        }

        [TestMethod]
        public void Update_ChangesEventName()
        {
            var factory = new TestDbContextFactory();
            var service = new EventService(factory);
            var ev = new Event
            {
                Name = "Original Name",
                Date = DateOnly.FromDateTime(DateTime.Today),
                Time = new TimeOnly(9, 0),
                Description = "Test"
            };
            service.Add(ev);

            ev.Name = "Updated Name";
            service.Update(ev);

            var updated = service.GetById(ev.Id);
            Assert.AreEqual("Updated Name", updated!.Name);
        }

        [TestMethod]
        public void Delete_RemovesEvent()
        {
            var factory = new TestDbContextFactory();
            var service = new EventService(factory);
            var ev = new Event
            {
                Name = "To Be Deleted",
                Date = DateOnly.FromDateTime(DateTime.Today),
                Time = new TimeOnly(9, 0),
                Description = "Temporary"
            };
            service.Add(ev);

            service.Delete(ev.Id);
            var result = service.GetList();

            Assert.HasCount(0, result);
        }

        [TestMethod]
        public void FilterEvents_ByActivityType_ReturnsMatchingEvents()
        {
            var factory = new TestDbContextFactory();
            using var context = factory.CreateDbContext();

            var workshopActivity = new Activity { Name = "Recycling Workshop", Type = "Workshop" };
            var talkActivity = new Activity { Name = "Community Talk", Type = "Talk" };
            var ev1 = new Event
            {
                Name = "Workshop Event",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                Time = new TimeOnly(9, 0),
                Description = "Has a workshop",
                Activities = new List<Activity> { workshopActivity }
            };
            var ev2 = new Event
            {
                Name = "Talk Event",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                Time = new TimeOnly(9, 0),
                Description = "Has a talk",
                Activities = new List<Activity> { talkActivity }
            };

            context.Activities.AddRange(workshopActivity, talkActivity);
            context.Events.AddRange(ev1, ev2);
            context.SaveChanges();

            var service = new EventService(factory);
            var results = service.FilterEvents(null, null, null, "Workshop");

            Assert.HasCount(1, results);
            Assert.AreEqual("Workshop Event", results[0].Name);
        }

        [TestMethod]
        public void FilterEvents_ByDateOnly_Overload_ReturnsMatchingEvents()
        {
            var factory = new TestDbContextFactory();
            var service = new EventService(factory);
            var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
            service.Add(new Event { Name = "Match", Date = targetDate, Time = new TimeOnly(9, 0), Description = "x" });
            service.Add(new Event { Name = "NoMatch", Date = targetDate.AddDays(1), Time = new TimeOnly(9, 0), Description = "x" });

            var results = service.FilterEvents(targetDate);

            Assert.HasCount(1, results);
            Assert.AreEqual("Match", results[0].Name);
        }
    }

    [TestClass]
    public class RegistrationServiceTests
    {
        private static (IDbContextFactory<ApplicationDbContext> factory, int eventId, int participantId) SeedEventAndParticipant()
        {
            var factory = new TestDbContextFactory();
            using var context = factory.CreateDbContext();

            var ev = new Event
            {
                Name = "Seed Event",
                Date = DateOnly.FromDateTime(DateTime.Today),
                Time = new TimeOnly(9, 0),
                Description = "Seed"
            };
            var participant = new Participant
            {
                Name = "Jane Doe",
                Email = "jane@example.com",
                Phone = "0400000000"
            };
            context.Events.Add(ev);
            context.Participants.Add(participant);
            context.SaveChanges();

            return (factory, ev.Id, participant.Id);
        }

        [TestMethod]
        public void Register_AddsARegistration()
        {
            var (factory, eventId, participantId) = SeedEventAndParticipant();
            var service = new RegistrationService(factory);

            service.Register(eventId, participantId);
            var registrations = service.GetList();

            Assert.HasCount(1, registrations);
            Assert.AreEqual("Confirmed", registrations[0].Status);
        }

        [TestMethod]
        public void Register_SameParticipantTwice_ThrowsDuplicateException()
        {
            var (factory, eventId, participantId) = SeedEventAndParticipant();
            var service = new RegistrationService(factory);
            service.Register(eventId, participantId); // first one succeeds

            Assert.Throws<DuplicateRegistrationException>(
                () => service.Register(eventId, participantId));
        }

        [TestMethod]
        public void Register_NonExistentEvent_ThrowsEntityNotFoundException()
        {
            var factory = new TestDbContextFactory();
            var service = new RegistrationService(factory);

            Assert.Throws<EntityNotFoundException>(
                () => service.Register(999, 1));
        }

        [TestMethod]
        public void Register_ExceedsVenueCapacity_ThrowsVenueCapacityExceededException()
        {
            var factory = new TestDbContextFactory();
            using var context = factory.CreateDbContext();

            var venue = new Venue { Name = "Small Hall", Address = "1 Test St", Capacity = 1 };
            var ev = new Event
            {
                Name = "Capacity Test Event",
                Date = DateOnly.FromDateTime(DateTime.Today),
                Time = new TimeOnly(9, 0),
                Description = "Test event with limited capacity",
                Venues = new List<Venue> { venue }
            };
            var participant1 = new Participant { Name = "Alice", Email = "alice@example.com" };
            var participant2 = new Participant { Name = "Bob", Email = "bob@example.com" };

            context.Venues.Add(venue);
            context.Events.Add(ev);
            context.Participants.Add(participant1);
            context.Participants.Add(participant2);
            context.SaveChanges();

            var service = new RegistrationService(factory);
            service.Register(ev.Id, participant1.Id); // fills the venue's capacity of 1

            Assert.Throws<VenueCapacityExceededException>(
                () => service.Register(ev.Id, participant2.Id));
        }
    }

    [TestClass]
    public class ModelValidationTests
    {
        [TestMethod]
        public void Event_WithPastDate_FailsValidation()
        {
            var pastEvent = new Event
            {
                Name = "Past Dated Event",
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                Time = new TimeOnly(9, 0),
                Description = "Should not validate"
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(pastEvent);
            bool isValid = Validator.TryValidateObject(pastEvent, context, results, true);

            Assert.IsFalse(isValid);
            Assert.IsTrue(results.Any(r => r.ErrorMessage!.Contains("past")));
        }

        [TestMethod]
        public void Participant_WithInvalidEmail_FailsValidation()
        {
            var participant = new Participant
            {
                Name = "Test Person",
                Email = "not-a-valid-email"
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(participant);
            bool isValid = Validator.TryValidateObject(participant, context, results, true);

            Assert.IsFalse(isValid);
        }
    }
}