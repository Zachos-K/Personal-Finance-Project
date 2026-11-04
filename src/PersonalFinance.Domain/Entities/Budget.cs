namespace PersonalFinance.Domain.Entities;

public class Budget
{
    public Guid Id {get; set; }

    public Guid UserId {get; set; }

    public int Year {get; set; }

    public int Month {get; set; }

    public decimal Amount {get; set; }

    public User User {get; set; } = null!;
}