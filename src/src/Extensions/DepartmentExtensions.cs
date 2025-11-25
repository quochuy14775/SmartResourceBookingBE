using src.DTOs.Requests;
using src.Model;

namespace src.Extensions;

 public static class CourseExtensions
    {
        // public static async Task<(bool Success, string? Value)> CheckCourseRuleAsync(
        //     this CourseRequest model, 
        //     IRepository<Course> repository, 
        //     long? excludeId = null)
        // {
        //     // Check if course name already exists
        //     var existingCourse = await repository.Query()
        //         .Where(x => x.Name == model.Name && (!excludeId.HasValue || x.Id != excludeId) && !x.IsDeleted)
        //         .FirstOrDefaultAsync();
        //
        //     if (existingCourse != null)
        //     {
        //         return (false, "A course with this name already exists.");
        //     }
        //
        //     // Validate start and end dates
        //     if (model.StartDate >= model.EndDate)
        //     {
        //         return (false, "Start date must be before end date.");
        //     }
        //
        //     // Optional: Check if start date is not in the past (can be removed if past dates are allowed)
        //     if (model.StartDate < DateTime.UtcNow.Date)
        //     {
        //         return (false, "Start date cannot be in the past.");
        //     }
        //
        //     return (true, null);
        // }

        public static Department GetDepartment(this DepartmentRequest model)
        {
            var department = new Department
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
          
            return department;
        }

        public static void ToEntity(this DepartmentRequest model, Department entity)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        //
        // public static void AssignSubject(this Course course, Subject subject)
        // {
        //     course.SubjectId = subject.Id;
        //     course.Subject = subject;
        //     course.UpdatedAt = DateTime.UtcNow;
        // }
        //
        // public static async Task<(bool Success, string? Value)> CheckBusinessRuleAsync(
        //     long courseId,
        //     IRepository<Course> courseRepository,
        //     IRepository<Schedule> scheduleRepository)
        // {
        //     // First check the course status
        //     var course = await courseRepository.Query()
        //         .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);
        //         
        //     if (course == null)
        //     {
        //         return (false, "Course not found.");
        //     }
        //
        //     // Check if course is currently started using real-time calculation
        //     var realTimeCourseStatus = StatusHelper.CalculateCourseStatus(course);
        //     if (realTimeCourseStatus == CourseStatus.Started)
        //     {
        //         return (false, "This course is currently ongoing and cannot be updated or deleted.");
        //     }
        //
        //     // Additional check for schedules
        //     var activeSchedules = await scheduleRepository.Query()
        //         .Where(s => s.CourseId == courseId && !s.IsDeleted && s.IsActive)
        //         .ToListAsync();
        //
        //     // Check if any schedule is currently in progress using real-time calculation
        //     var ongoingSchedule = activeSchedules.FirstOrDefault(s => 
        //         StatusHelper.CalculateScheduleStatus(s) == ScheduleStatus.Started);
        //     if (ongoingSchedule != null)
        //     {
        //         return (false, "This course has an ongoing schedule and cannot be updated or deleted.");
        //     }
        //
        //     return (true, null);
        // }
    }