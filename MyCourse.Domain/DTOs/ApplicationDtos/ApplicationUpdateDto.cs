using MyCourse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.DTOs.ApplicationDtos
{
    public class ApplicationUpdateDto
    {
        public int Id { get; set; }
        public ApplicationStatusType Status { get; set; }
    }

}
