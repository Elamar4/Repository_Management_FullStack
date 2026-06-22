namespace Repository_management_backend.Models.Entities
{
    /// <summary>Filial. Bütün biznes datası filiala görə ayrılır.</summary>
    public class Branch
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;   // "merdekan", "pirsagi", "baku"
        public string Name { get; set; } = string.Empty;   // "Mərdəkan filialı"
        public bool IsActive { get; set; } = true;

        // Naviqasiyalar
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
    }
}
