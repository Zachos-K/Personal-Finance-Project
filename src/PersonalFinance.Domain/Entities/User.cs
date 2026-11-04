namespace PersonalFinance.Domain.Entities;

public class User
{

    public Guid Id {get; set; }

    public string Email {get; set; } = string.Empty;

    public string PasswordHash {get; set; } = string.Empty;

    public DateTime CreatedAt {get; set; } = DateTime.UtcNow; 

    public ICollection<Expense> Expenses {get; set; } = new List<Expense>();

    public ICollection<Category> Categories {get; set; } = new List<Category>();

    public ICollection<Budget> Budgets {get; set; } = new List<Budget>(); 
}