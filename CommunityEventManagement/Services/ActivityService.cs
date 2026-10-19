using CommunityEventManagement.Data;
using CommunityEventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityEventManagement.Services
{
    public class ActivityService(IDbContextFactory<ApplicationDbContext> contextFactory)
        : BaseService<Activity>(contextFactory)
    {
    }
}