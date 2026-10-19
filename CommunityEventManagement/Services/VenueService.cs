using CommunityEventManagement.Data;
using CommunityEventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Services
{
    public class VenueService(IDbContextFactory<ApplicationDbContext> contextFactory)
        : BaseService<Venue>(contextFactory)
    {
    }
}