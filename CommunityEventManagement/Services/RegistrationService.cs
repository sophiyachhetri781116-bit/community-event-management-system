using CommunityEventManagement.Data;
using CommunityEventManagement.Data.Models;
using CommunityEventManagement.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Services
{
    public class RegistrationService(IDbContextFactory<ApplicationDbContext> contextFactory)
        : BaseService<Registration>(contextFactory)
    {
        public override List<Registration> GetList()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Registrations
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .OrderByDescending(r => r.RegistrationDate)
                .ToList();
        }

        public void Register(int eventId, int participantId, string status = "Confirmed")
        {
            using var context = _contextFactory.CreateDbContext();

            var ev = context.Events
                .Include(e => e.Venues)
                .Include(e => e.Registrations)
                .FirstOrDefault(e => e.Id == eventId)
                ?? throw new EntityNotFoundException($"Event with ID {eventId} was not found.");

            bool alreadyRegistered = context.Registrations
                .Any(r => r.EventId == eventId && r.ParticipantId == participantId);
            if (alreadyRegistered)
                throw new DuplicateRegistrationException(
                    "This participant is already registered for this event.");

            int totalCapacity = ev.Venues.Sum(v => v.Capacity);
            if (totalCapacity > 0 && ev.Registrations.Count >= totalCapacity)
                throw new VenueCapacityExceededException(
                    "This event has reached its venue capacity.");

            var registration = new Registration
            {
                EventId = eventId,
                ParticipantId = participantId,
                RegistrationDate = DateTime.Now,
                Status = status
            };
            context.Registrations.Add(registration);
            context.SaveChanges();
        }
    }
}