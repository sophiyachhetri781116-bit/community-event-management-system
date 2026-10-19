using CommunityEventManagement.Data;
using CommunityEventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Services
{
    public class ParticipantService(IDbContextFactory<ApplicationDbContext> contextFactory)
        : BaseService<Participant>(contextFactory)
    {
        public override Participant? GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Participants
                .Include(p => p.Registrations)
                .FirstOrDefault(p => p.Id == id);
        }

        // Finds the Participant record linked to a logged-in user account,
        // automatically creating one the first time that user registers for an event.
        public Participant GetOrCreateForUser(string applicationUserId, string email, string? name)
        {
            using var context = _contextFactory.CreateDbContext();

            var existing = context.Participants
                .FirstOrDefault(p => p.ApplicationUserId == applicationUserId);
            if (existing != null)
                return existing;

            // If an Admin already manually added this person by email, link that record instead
            // of creating a duplicate.
            var byEmail = context.Participants.FirstOrDefault(p => p.Email == email);
            if (byEmail != null)
            {
                byEmail.ApplicationUserId = applicationUserId;
                context.SaveChanges();
                return byEmail;
            }

            var participant = new Participant
            {
                Name = string.IsNullOrWhiteSpace(name) ? email : name,
                Email = email,
                ApplicationUserId = applicationUserId
            };
            context.Participants.Add(participant);
            context.SaveChanges();
            return participant;
        }
    }
}