using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreRest.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
  
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
