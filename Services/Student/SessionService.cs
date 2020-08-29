using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Unity;

namespace InstantTutors.Services.Student
{
    public interface ISessionService
    {
        Task<List<Sessions>> GetSessionsAsync();
        Task<Sessions> GetSessionByIdAsync(int Id);
        Task<List<Sessions>> GetSessionByUserIdAsync(string UserId);
        Task<List<Sessions>> GetSessionByUserIdAsync(string UserId, int limit);

        Task<List<Sessions>> GetSessionByTutorIdAsync(string UserId);

        Task<List<Sessions>> GetSessionAdminAsync(string UserId);
    }
    public class SessionService : ISessionService
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<List<Sessions>> GetSessionsAsync()
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule).AsNoTracking()
                .ToListAsync();
        }
        public async Task<Sessions> GetSessionByIdAsync(int Id)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Where(x => x.Id == Id).AsNoTracking().FirstOrDefaultAsync();
        }
        public async Task<List<Sessions>> GetSessionByUserIdAsync(string UserId)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.CreatedDate).AsNoTracking().ToListAsync();
        }

        public async Task<List<Sessions>> GetSessionAdminAsync(string UserId)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Where(x => x.CreatedBy == UserId)
                .OrderByDescending(x => x.CreatedDate).AsNoTracking().ToListAsync();
        }
        public async Task<List<Sessions>> GetSessionByUserIdAsync(string UserId, int limit)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.CreatedDate)
                .Take(limit).AsNoTracking().ToListAsync();
        }
        public async Task<List<Sessions>> GetSessionByTutorIdAsync(string UserId)
        {
            return await _dbContext.Sessions
    .Include(x => x.SessionSchedule)
    .Where(x => x.TutorUserId == UserId)
    .OrderByDescending(x => x.CreatedDate).AsNoTracking().ToListAsync();
        }
    }
}