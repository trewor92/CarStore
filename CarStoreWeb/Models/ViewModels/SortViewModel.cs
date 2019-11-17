using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models.ViewModels
{
    public class SortViewModel
    {
        public SortState YearOfManufactureSort { get; private set; } // значение для сортировки
        public SortState FuelTypeSort { get; private set; }    // значение для сортировки
        public SortState EngineСapacitySort { get; private set; }   // значение для сортировки 
        public SortState PriceSort { get; private set; }   // значение для сортировки 
        public SortState? CurrentSortOrder { get; private set; }

        public SortViewModel(SortState? sortOrder)
        {
            CurrentSortOrder = sortOrder;
            YearOfManufactureSort = sortOrder == SortState.YearOfManufactureAsc ? SortState.YearOfManufactureDesc: SortState.YearOfManufactureAsc;
            FuelTypeSort = sortOrder == SortState.FuelTypeAsc ? SortState.FuelTypeDesc : SortState.FuelTypeAsc;
            EngineСapacitySort = sortOrder == SortState.EngineCapacityAsc ? SortState.EngineCapacityDesc : SortState.EngineCapacityAsc;
            PriceSort = sortOrder == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;

        }
    }
}
