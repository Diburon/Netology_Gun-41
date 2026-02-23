namespace FinalTask.Models;

public class PlayerProfile
{
    public const int MaxBank = 1_000_000;
    public const int InitialBank = 1000;

    public string Name { get; set; } = string.Empty;
    public int Bank { get; set; }

    public PlayerProfile() { }

    public PlayerProfile(string name, int bank = InitialBank)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        if (bank < 0)
            throw new ArgumentException("Bank cannot be negative.", nameof(bank));

        Name = name.Trim();
        Bank = bank;
    }
}