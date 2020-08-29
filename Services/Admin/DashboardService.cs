using InstantTutors.Areas.Admin.ViewModels;
using InstantTutors.Areas.Student.ViewModels;
using InstantTutors.Areas.Tutor.ViewModels;
using InstantTutors.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Unity;

namespace InstantTutors.Services.Admin
{
    public interface IDashboardService
    {
        Task<AdminViewModel> GetAdminDashboardAsync();
        Task<List<Sessions>> GetSessionByUserIdAsync(string UserId);
        Task<Tutors> GetTutorByUserIdAsync(string UserId);
        Task<List<StudentViewModel>> GetStudentsAsync();
        Task<List<TutorViewModel>> GetTutorsAsync();
    }
    public class DashboardService : IDashboardService
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<AdminViewModel> GetAdminDashboardAsync()
        {
            AdminViewModel model = new AdminViewModel();
            
            model.StudentsList = await GetStudentsAsync();
            model.TutorsList = await GetTutorsAsync();

            return model;
        }
        public async Task<List<StudentViewModel>> GetStudentsAsync()
        {
            List<StudentViewModel> _studentsList = new List<StudentViewModel>();
            var _sRole = await _dbContext.Roles.Where(x => x.Name == "Student").FirstOrDefaultAsync();

            var _students = await _dbContext.Users
                     .Where(x => x.EmailConfirmed == true && x.Roles.Select(y => y.RoleId).Contains(_sRole.Id)).AsNoTracking()
                     .ToListAsync();

            foreach (var student in _students)
            {
                _studentsList.Add(new StudentViewModel()
                {
                    UserId = student.Id,
                    User = student,
                    SessionsList = await GetSessionByUserIdAsync(student.Id)
                });
            }
            return _studentsList;
        }
        public async Task<List<TutorViewModel>> GetTutorsAsync()
        {
            List<TutorViewModel> _tutorsList = new List<TutorViewModel>();
            var _tRole = await _dbContext.Roles.Where(x => x.Name == "Tutor").FirstOrDefaultAsync();

            var _tutors = await _dbContext.Users
                     .Where(x => x.EmailConfirmed == true && x.Roles.Select(y => y.RoleId).Contains(_tRole.Id)).AsNoTracking()
                     .ToListAsync();

            foreach (var tutor in _tutors)
            {
                var _tutor = await this.GetTutorByUserIdAsync(tutor.Id);
                _tutor = _tutor == null ? new Tutors() : _tutor;

                _tutorsList.Add(new TutorViewModel()
                {
                    NameOfSchool = _tutor.NameOfSchool,
                    Experience = _tutor.Experience,
                    PreviousSubjects = _tutor.PreviousSubjects,
                    UserId = _tutor.UserId,
                    Concerns = _tutor.Concerns,
                    GradeLevel = _tutor.GradeLevel,
                    CreatedDate = _tutor.CreatedDate,
                    Id = _tutor.Id,
                    User = tutor
                });
            }
            return _tutorsList;
        }
        public async Task<List<Sessions>> GetSessionByUserIdAsync(string UserId)
        {
            return await _dbContext.Sessions
                .Include(x => x.SessionSchedule)
                .Where(x => x.UserId == UserId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }
        public async Task<Tutors> GetTutorByUserIdAsync(string UserId)
        {
            return await _dbContext.Tutors
                .Include(x => x.User)
                //.Include(x => x.TuitionSubjects)
                //.Include(x => x.TutorAvailability)
                .Where(x => x.UserId == UserId).FirstOrDefaultAsync();
        }
    }
}