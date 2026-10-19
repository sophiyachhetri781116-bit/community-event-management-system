using CommunityEventManagement.Data;
using CommunityEventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Services
{
    public class EventService(IDbContextFactory<ApplicationDbContext> contextFactory)
        : BaseService<Event>(contextFactory), IEventService
    {
        public override List<Event> GetList()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Events
                .Include(e => e.Venues)
                .Include(e => e.Activities)
                .Include(e => e.Registrations)
                .ToList();
        }

        public override Event? GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Events
                .Include(e => e.Venues)
                .Include(e => e.Activities)
                .Include(e => e.Registrations)
                .FirstOrDefault(e => e.Id == id);
        }

        public List<Event> GetUpcomingEvents()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            using var context = _contextFactory.CreateDbContext();
            return context.Events
                .Include(e => e.Venues)
                .Include(e => e.Activities)
                .Where(e => e.Date >= today)
                .OrderBy(e => e.Date)
                .ToList();
        }

        // METHOD OVERLOADING: same name, different parameters — next N upcoming events
        public List<Event> GetUpcomingEvents(int maxCount)
        {
            return GetUpcomingEvents().Take(maxCount).ToList();
        }

        // ALGORITHM: filter by optional date, venue, specific activity, and activity type/category
        public List<Event> FilterEvents(DateOnly? date, int? venueId, int? activityId, string? activityType)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.Events
                .Include(e => e.Venues)
                .Include(e => e.Activities)
                .AsQueryable();

            if (date.HasValue)
                query = query.Where(e => e.Date == date.Value);

            if (venueId.HasValue)
                query = query.Where(e => e.Venues.Any(v => v.Id == venueId.Value));

            if (activityId.HasValue)
                query = query.Where(e => e.Activities.Any(a => a.Id == activityId.Value));

            if (!string.IsNullOrEmpty(activityType))
                query = query.Where(e => e.Activities.Any(a => a.Type == activityType));

            return query.OrderBy(e => e.Date).ToList();
        }

        // METHOD OVERLOADING: filter by a single criterion — date only
        public List<Event> FilterEvents(DateOnly date) => FilterEvents(date, null, null, null);

        // METHOD OVERLOADING: filter by a single criterion — venue only
        public List<Event> FilterEvents(int venueId) => FilterEvents(null, venueId, null, null);

        public void AddEvent(Event eventItem, List<int> venueIds, List<int> activityIds)
        {
            using var context = _contextFactory.CreateDbContext();
            eventItem.Venues = context.Venues.Where(v => venueIds.Contains(v.Id)).ToList();
            eventItem.Activities = context.Activities.Where(a => activityIds.Contains(a.Id)).ToList();
            context.Events.Add(eventItem);
            context.SaveChanges();
        }

        public void UpdateEvent(Event eventItem, List<int> venueIds, List<int> activityIds)
        {
            using var context = _contextFactory.CreateDbContext();
            var existing = context.Events
                .Include(e => e.Venues)
                .Include(e => e.Activities)
                .First(e => e.Id == eventItem.Id);

            existing.Name = eventItem.Name;
            existing.Date = eventItem.Date;
            existing.Time = eventItem.Time;
            existing.Description = eventItem.Description;

            existing.Venues.Clear();
            foreach (var v in context.Venues.Where(v => venueIds.Contains(v.Id)))
                existing.Venues.Add(v);

            existing.Activities.Clear();
            foreach (var a in context.Activities.Where(a => activityIds.Contains(a.Id)))
                existing.Activities.Add(a);

            context.SaveChanges();
        }
    }
}