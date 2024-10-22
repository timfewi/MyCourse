using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Entities
{
    public class ApplicationMedia : BaseEntity
    {
        public int ApplicationId { get; set; }
        public int MediaId { get; set; }
        [ForeignKey(nameof(ApplicationId))]
        public Application Application { get; set; } = new();
        [ForeignKey(nameof(MediaId))]
        public Media Media { get; set; } = new();
    }
}
