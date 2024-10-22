using MyCourse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Entities
{
    public class Media : BaseEntity
    {
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long FileSize { get; set; }

        // Navigation Properties
        public ICollection<CourseMedia> CourseMedias { get; set; } = [];
        public ICollection<ApplicationMedia> ApplicationMedias { get; set; } = [];
    }
}


// TODO: Controller for Path:

//// Erstelle den Upload-Pfad
//string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", mediaType == MediaType.Image ? "images" : "videos");
//string subFolder = "tests"; // Optional: kannst du dynamisch gestalten
//string finalFolder = Path.Combine(uploadsFolder, subFolder);

//// Sicherstellen, dass der Zielordner existiert
//if (!Directory.Exists(finalFolder))
//{
//    Directory.CreateDirectory(finalFolder);
//}

//// Generiere einen eindeutigen Dateinamen
//string uniqueFileName = Guid.NewGuid().ToString() + extension;
//string filePath = Path.Combine(finalFolder, uniqueFileName);

//// Speichere die Datei im Dateisystem
//using (var fileStream = new FileStream(filePath, FileMode.Create))
//{
//    await file.CopyToAsync(fileStream);
//}

//// Erstelle den relativen Pfad zur Datei
//string relativePath = Path.Combine("/uploads", mediaType == MediaType.Image ? "images" : "videos", subFolder, uniqueFileName).Replace("\\", "/");

//// Speichere die Media-Entität in der Datenbank
//var media = new Media
//{
//    Url = relativePath,
//    FileName = uniqueFileName,
//    MediaType = mediaType,
//    ContentType = file.ContentType,
//    FileSize = file.Length,
//    Description = description
//};
