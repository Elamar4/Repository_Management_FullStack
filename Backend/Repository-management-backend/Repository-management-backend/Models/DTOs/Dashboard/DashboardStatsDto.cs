namespace Repository_management_backend.Models.DTOs.Dashboard
{
    /// <summary>Filial üzrə ümumi statistika.</summary>
    public class DashboardStatsDto
    {
        public int CustomerCount { get; set; }
        public int InvoiceCount { get; set; }
        public int OpenInvoiceCount { get; set; }
        public int ClosedInvoiceCount { get; set; }

        public int OverdueCount { get; set; }
        public int DueTodayCount { get; set; }
        public int DueSoonCount { get; set; }

        public decimal TotalDebt { get; set; }      // Σ ledger DebtChange
        public decimal TotalDeposit { get; set; }   // Σ ledger DepositChange
        public decimal TotalPaid { get; set; }      // Σ qaimə PaidAmount

        public int InventoryItemCount { get; set; }
    }
}
