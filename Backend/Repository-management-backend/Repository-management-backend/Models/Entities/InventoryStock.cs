namespace Repository_management_backend.Models.Entities
{
    /// <summary>Anbar qalığı. İcarədə olan say qaimələrdən hesablanır;
    /// burada yalnız ümumi say saxlanılır.</summary>
    public class InventoryStock
    {
        public int Id { get; set; }

        public int BranchId { get; set; }
        public Branch? Branch { get; set; }

        public string Name { get; set; } = string.Empty;   // məs. "Boy dikt", "Taxta 5/15", "Dəmir dirək 3.85"
        public decimal TotalCount { get; set; }
    }
}
