namespace PersonalFinance.Domain.Entities;

public class Category
{

    public Guid Id {get; set; }

    public Guid UserId {get; set; }

    public string Name {get; set; } = string.Empty;

    public User User {get; set; } = null!;

    public ICollection<Expense> Expenses {get; set; } = new List<Expense>();
}