using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models.ViewModels
{
    public enum SortState
    {
        YearOfManufactureAsc,    //  YearOfManufacture по возрастанию
        YearOfManufactureDesc,   //  YearOfManufacture по убыванию
        FuelTypeAsc, 
        FuelTypeDesc,
        EngineCapacityAsc,
        EngineCapacityDesc,
        PriceAsc,
        PriceDesc
    }
}
