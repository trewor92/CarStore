using System.Collections.Generic;

namespace CarStoreWeb.Models.ViewModels
{
    public class NavigationMenuViewComponentViewModel
    {
        public IDictionary<string,int> Categories { get; set; }
        public string CurrentCategory { get; set; }
    }
}
