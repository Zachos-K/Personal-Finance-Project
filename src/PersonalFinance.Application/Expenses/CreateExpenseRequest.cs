using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Application.Expenses;

public class CreateExpenseRequest
{
    public Guid CategoryId { get; set; }

    public decimal Amount { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}