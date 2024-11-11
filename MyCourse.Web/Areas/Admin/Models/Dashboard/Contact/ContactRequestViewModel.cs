namespace MyCourse.Web.Areas.Admin.Models.Dashboard.Contact
{
    public class ContactRequestViewModel
    {
        public List<ContactRequestListItemViewModel> ActiveContactRequests { get; set; } = new List<ContactRequestListItemViewModel>();
        public List<ContactRequestListItemViewModel> InactiveContactRequests { get; set; } = new List<ContactRequestListItemViewModel>();

        public ContactRequestDetailViewModel? SelectedRequest { get; set; }
    }
}
