using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models.ViewModels
{
    public class DeclarationListViewModel
    {
        public IEnumerable<Car> Cars { get; set; }
        public string CurrentCategory { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
