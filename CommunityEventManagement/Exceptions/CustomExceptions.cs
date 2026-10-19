namespace CommunityEventManagement.Exceptions
{
    // Thrown when a participant is registered for the same event more than once.
    public class DuplicateRegistrationException : Exception
    {
        public DuplicateRegistrationException(string message) : base(message) { }
    }

    // Thrown when an event's registrations exceed its venue capacity.
    public class VenueCapacityExceededException : Exception
    {
        public VenueCapacityExceededException(string message) : base(message) { }
    }

    // Thrown when a requested entity cannot be found.
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message) { }
    }
}