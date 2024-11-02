﻿using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MyCourse.Web.Models.CourseModels
{
    public class CourseDetailsViewModel
    {
        public int Id { get; set; }

        public string? ImageUrl { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Kursbeschreibung")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Datum")]
        public DateTime CourseDate { get; set; }
        
        [Display(Name = "Kursdauer")]
        public TimeSpan CourseDuration { get; set; }

        [Display(Name = "Derzeitige Anmeldungen")]
        public int ApplicationCount { get; set; }
        
        [Display(Name = "Maximale Anzahl an Teilnehmern")]
        public int MaxParticipants { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;
        
        [Display(Name = "Preis")]
        public decimal Price { get; set; }
    }
}
