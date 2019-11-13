using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models.ViewModels
{
    public class NavigationMenuViewComponentViewModel
    {
        public IDictionary<string,int> Categories { get; set; }
        public string CurrentCategory { get; set; }
    }
}
