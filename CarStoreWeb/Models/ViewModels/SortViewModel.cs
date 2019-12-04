namespace CarStoreWeb.Models.ViewModels
{
    public class SortViewModel
    {
        public SortState YearOfManufactureSort { get; private set; } //value for sort
        public SortState FuelTypeSort { get; private set; }    //value for sort
        public SortState EngineСapacitySort { get; private set; }   //value for sort 
        public SortState PriceSort { get; private set; }   //value for sort 
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
