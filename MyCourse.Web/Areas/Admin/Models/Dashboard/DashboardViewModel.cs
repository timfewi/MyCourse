﻿using MyCourse.Domain.DTOs.CourseDtos;

namespace MyCourse.Web.Areas.Admin.Models.Dashboard
{
    public class DashboardViewModel
    {
        public int ActiveCoursesCount { get; set; }
        public int TotalRegistrations {  get; set; }
        public string Statistics {  get; set; } = string.Empty;
        public List<CourseWithApplicationsDto> CoursesWithApplications { get; set; } = new();

    }
}
