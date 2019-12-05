using System.Collections.Generic;

namespace CarStoreWeb.Models.ViewModels
{
    public class NoticeListViewModel
    {
        public IEnumerable<CarViewModel> CarViewModels { get; set; }
        public string CurrentCategory { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
