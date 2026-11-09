namespace PersonalFinance.Application.Expenses;

public class ExpenseResponse
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; }
}