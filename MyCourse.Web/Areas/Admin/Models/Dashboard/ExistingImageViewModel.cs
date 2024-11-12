namespace MyCourse.Web.Areas.Admin.Models.Dashboard
{
    public class ExistingImageViewModel
    {
        public int MediaId { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool ToDelete { get; set; }
        
        // TODO
        // public string Caption { get; set; }

    }
}
