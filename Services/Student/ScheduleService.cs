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

namespace InstantTutors.Services.Student
{
    public interface IScheduleService
    {
        Task<(bool, SessionViewModel)> CreateScheduleAsync(SessionViewModel _session);
        Task<(bool, SessionViewModel)> UpdateScheduleAsync(SessionViewModel _session);

        Task<(bool, TutorSessionViewModel)> CreateScheduleAsync(TutorSessionViewModel _session);
        Task<(bool, TutorSessionViewModel)> UpdateScheduleAsync(TutorSessionViewModel _session);


        Task<(bool, AdminSessionViewModel)> CreateScheduleAsync(AdminSessionViewModel _session);
        Task<(bool, AdminSessionViewModel)> UpdateScheduleAsync(AdminSessionViewModel _session);
    }
    public class ScheduleService : IScheduleService
    {
        [Dependency]
        public ApplicationDbContext _dbContext { get; set; }

        public async Task<(bool, SessionViewModel)> CreateScheduleAsync(SessionViewModel _session)
        {
            var _SessionDB = _dbContext.Sessions.Add(new Sessions()
            {
                Title = _session.Title,
                Description = _session.Description,
                CommunicationMethod = _session.CommunicationMethod,
                Concerns = _session.Concerns,
                CreatedDate = DateTime.Now,
                UserId = _session.UserId,
                TutorUserId = _session.Tutor,
                Status = 0,
                CreatedBy = _session.UserId,
                StartDate = _session.StartDate,
                EndDate = _session.EndDate
            });
            await _dbContext.SaveChangesAsync();

            if (_session.SessionSchedules != null)
            {
                foreach (var schedule in _session.SessionSchedules)
                {
                    foreach (var time in schedule.Timing)
                    {
                        if (time.Selected)
                        {
                            var SessionScheduleDB = _dbContext.SessionSchedule.Add(new SessionSchedule()
                            {
                                Day = schedule.Day,
                                Time = time.AvailabilityTime,
                                SessionId = _SessionDB.Id,
                                CreatedDate = DateTime.Now,
                                SelectedDate = schedule.SelectedDate
                            });
                            await _dbContext.SaveChangesAsync();
                            time.Id = SessionScheduleDB.Id;
                        }
                    }
                }
            }
            _session.Id = _SessionDB.Id;
            return (true, _session);
        }
        public async Task<(bool, SessionViewModel)> UpdateScheduleAsync(SessionViewModel _session)
        {
            if (_session.Id == 0)
            {
                await this.CreateScheduleAsync(_session);
            }
            else
            {
                var _SessionDB = await _dbContext.Sessions.Where(x => x.Id == _session.Id).FirstOrDefaultAsync();
                if (_SessionDB == null)
                {
                    return (false, _session);
                }

                _SessionDB.Title = _session.Title;
                _SessionDB.Description = _session.Description;
                _SessionDB.CommunicationMethod = _session.CommunicationMethod;
                _SessionDB.Concerns = _session.Concerns;
                _SessionDB.UpdatedDate = DateTime.Now;
                _SessionDB.TutorUserId = _session.Tutor;
                _SessionDB.Status = 0;
                _SessionDB.CreatedBy = _session.UserId;

                _SessionDB.StartDate = _session.StartDate;
                _SessionDB.EndDate = _session.EndDate;

                await _dbContext.SaveChangesAsync();

                if (_session.SessionSchedules != null)
                {
                    var sessionSchedulesExist = await _dbContext.SessionSchedule.Where(x => x.SessionId == _SessionDB.Id).ToListAsync();
                    _dbContext.SessionSchedule.RemoveRange(sessionSchedulesExist);
                    await _dbContext.SaveChangesAsync();

                    foreach (var schedule in _session.SessionSchedules)
                    {
                        foreach (var time in schedule.Timing)
                        {
                            if (time.Selected)
                            {
                                var SessionScheduleDB = _dbContext.SessionSchedule.Add(new SessionSchedule()
                                {
                                    Day = schedule.Day,
                                    Time = time.AvailabilityTime,
                                    SessionId = _SessionDB.Id,
                                    CreatedDate = DateTime.Now,
                                    SelectedDate = schedule.SelectedDate
                                });
                                await _dbContext.SaveChangesAsync();
                                schedule.Id = SessionScheduleDB.Id;
                            }
                        }
                    }
                }
                _session.Id = _SessionDB.Id;
            }
            return (true, _session);
        }

        public async Task<(bool, TutorSessionViewModel)> CreateScheduleAsync(TutorSessionViewModel _session)
        {
            var _SessionDB = _dbContext.Sessions.Add(new Sessions()
            {
                Title = _session.Title,
                Description = _session.Description,
                CommunicationMethod = _session.CommunicationMethod,
                Concerns = _session.Concerns,
                CreatedDate = DateTime.Now,
                UserId = _session.Student,
                TutorUserId = _session.UserId,
                Status = 0,
                CreatedBy = _session.UserId,
                StartDate = _session.StartDate,
                EndDate = _session.EndDate
            });
            await _dbContext.SaveChangesAsync();

            if (_session.SessionSchedules != null)
            {
                foreach (var schedule in _session.SessionSchedules)
                {
                    foreach (var time in schedule.Timing)
                    {
                        if (time.Selected)
                        {
                            var SessionScheduleDB = _dbContext.SessionSchedule.Add(new SessionSchedule()
                            {
                                Day = schedule.Day,
                                Time = time.AvailabilityTime,
                                SessionId = _SessionDB.Id,
                                CreatedDate = DateTime.Now,
                                SelectedDate = schedule.SelectedDate
                            });
                            await _dbContext.SaveChangesAsync();
                            time.Id = SessionScheduleDB.Id;
                        }
                    }
                }
            }
            _session.Id = _SessionDB.Id;
            return (true, _session);
        }
        public async Task<(bool, TutorSessionViewModel)> UpdateScheduleAsync(TutorSessionViewModel _session)
        {
            if (_session.Id == 0)
            {
                await this.CreateScheduleAsync(_session);
            }
            else
            {
                var _SessionDB = await _dbContext.Sessions.Where(x => x.Id == _session.Id).FirstOrDefaultAsync();
                if (_SessionDB == null)
                {
                    return (false, _session);
                }

                _SessionDB.Title = _session.Title;
                _SessionDB.Description = _session.Description;
                _SessionDB.CommunicationMethod = _session.CommunicationMethod;
                _SessionDB.Concerns = _session.Concerns;
                _SessionDB.UpdatedDate = DateTime.Now;
                _SessionDB.TutorUserId = _session.UserId;
                _SessionDB.UserId = _session.Student;
                _SessionDB.Status = 0;
                _SessionDB.CreatedBy = _session.UserId;
                _SessionDB.StartDate = _session.StartDate;
                _SessionDB.EndDate = _session.EndDate;

                await _dbContext.SaveChangesAsync();

                if (_session.SessionSchedules != null)
                {
                    var sessionSchedulesExist = await _dbContext.SessionSchedule.Where(x => x.SessionId == _SessionDB.Id).ToListAsync();
                    _dbContext.SessionSchedule.RemoveRange(sessionSchedulesExist);
                    await _dbContext.SaveChangesAsync();

                    foreach (var schedule in _session.SessionSchedules)
                    {
                        foreach (var time in schedule.Timing)
                        {
                            if (time.Selected)
                            {
                                var SessionScheduleDB = _dbContext.SessionSchedule.Add(new SessionSchedule()
                                {
                                    Day = schedule.Day,
                                    Time = time.AvailabilityTime,
                                    SessionId = _SessionDB.Id,
                                    CreatedDate = DateTime.Now,
                                    SelectedDate = schedule.SelectedDate
                                });
                                await _dbContext.SaveChangesAsync();
                                schedule.Id = SessionScheduleDB.Id;
                            }
                        }
                    }
                }
                _session.Id = _SessionDB.Id;
            }
            return (true, _session);
        }

        public async Task<(bool, AdminSessionViewModel)> CreateScheduleAsync(AdminSessionViewModel _session)
        {
            var _SessionDB = _dbContext.Sessions.Add(new Sessions()
            {
                Title = _session.Title,
                Description = _session.Description,
                CommunicationMethod = _session.CommunicationMethod,
                Concerns = _session.Concerns,
                CreatedDate = DateTime.Now,
                UserId = _session.Student,
                TutorUserId = _session.Tutor,
                Status = 0,
                CreatedBy = _session.UserId,
                StartDate = _session.StartDate,
                EndDate = _session.EndDate
            });
            await _dbContext.SaveChangesAsync();

            if (_session.SessionSchedules != null)
            {
                foreach (var schedule in _session.SessionSchedules)
                {
                    foreach (var time in schedule.Timing)
                    {
                        if (time.Selected)
                        {
                            var SessionScheduleDB = _dbContext.SessionSchedule.Add(new SessionSchedule()
                            {
                                Day = schedule.Day,
                                Time = time.AvailabilityTime,
                                SessionId = _SessionDB.Id,
                                CreatedDate = DateTime.Now,
                                SelectedDate=schedule.SelectedDate
                            });
                            await _dbContext.SaveChangesAsync();
                            time.Id = SessionScheduleDB.Id;
                        }
                    }
                }
            }
            _session.Id = _SessionDB.Id;
            return (true, _session);
        }
        public async Task<(bool, AdminSessionViewModel)> UpdateScheduleAsync(AdminSessionViewModel _session)
        {
            if (_session.Id == 0)
            {
                await this.CreateScheduleAsync(_session);
            }
            else
            {
                var _SessionDB = await _dbContext.Sessions.Where(x => x.Id == _session.Id).FirstOrDefaultAsync();
                if (_SessionDB == null)
                {
                    return (false, _session);
                }

                _SessionDB.Title = _session.Title;
                _SessionDB.Description = _session.Description;
                _SessionDB.CommunicationMethod = _session.CommunicationMethod;
                _SessionDB.Concerns = _session.Concerns;
                _SessionDB.UpdatedDate = DateTime.Now;
                _SessionDB.TutorUserId = _session.UserId;
                _SessionDB.UserId = _session.Student;
                _SessionDB.Status = 0;
                _SessionDB.CreatedBy = _session.UserId;
                _SessionDB.TutorUserId = _session.Tutor;
                _SessionDB.StartDate = _session.StartDate;
                _SessionDB.EndDate = _session.EndDate;

                await _dbContext.SaveChangesAsync();

                if (_session.SessionSchedules != null)
                {
                    var sessionSchedulesExist = await _dbContext.SessionSchedule.Where(x => x.SessionId == _SessionDB.Id).ToListAsync();
                    _dbContext.SessionSchedule.RemoveRange(sessionSchedulesExist);
                    await _dbContext.SaveChangesAsync();

                    foreach (var schedule in _session.SessionSchedules)
                    {
                        foreach (var time in schedule.Timing)
                        {
                            if (time.Selected)
                            {
                                var SessionScheduleDB = _dbContext.SessionSchedule.Add(new SessionSchedule()
                                {
                                    Day = schedule.Day,
                                    Time = time.AvailabilityTime,
                                    SessionId = _SessionDB.Id,
                                    CreatedDate = DateTime.Now,
                                    SelectedDate = schedule.SelectedDate
                                });
                                await _dbContext.SaveChangesAsync();
                                schedule.Id = SessionScheduleDB.Id;
                            }
                        }
                    }
                }
                _session.Id = _SessionDB.Id;
            }
            return (true, _session);
        }
    }
}