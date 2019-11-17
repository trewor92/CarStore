using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreRest.Models
{
    [Owned]
    public class CarDescription
    {
        public string Color { get; set; }
        public int YearOfManufacture { get; set; }
        public string FuelType { get; set; }
        public double EngineСapacity { get; set; }
    }   
}
