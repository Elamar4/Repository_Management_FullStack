namespace Repository_management_backend.Models.DTOs.Inventory
{
    /// <summary>Anbar malı: ümumi qalıq, icarədə olan və boş qalıq.</summary>
    public class InventoryStockDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalCount { get; set; }    // anbar qalığı (ümumi)
        public decimal RentedOut { get; set; }     // icarədə olan (qaimələrdən hesablanır)
        public decimal FreeCount { get; set; }     // boş qalıq = Total - RentedOut
    }
}
