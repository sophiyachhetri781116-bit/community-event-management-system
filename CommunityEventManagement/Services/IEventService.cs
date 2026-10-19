using CommunityEventManagement.Data.Models;

namespace CommunityEventManagement.Services
{
    public interface IEventService : IService<Event>
    {
        List<Event> GetUpcomingEvents();
        List<Event> GetUpcomingEvents(int maxCount);   // method overloading
        List<Event> FilterEvents(DateOnly? date, int? venueId, int? activityId, string? activityType);
        List<Event> FilterEvents(DateOnly date);        // overload: filter by date only
        List<Event> FilterEvents(int venueId);           // overload: filter by venue only
        void AddEvent(Event eventItem, List<int> venueIds, List<int> activityIds);
        void UpdateEvent(Event eventItem, List<int> venueIds, List<int> activityIds);
    }
}