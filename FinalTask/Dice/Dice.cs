namespace FinalTask.Dice;

public readonly struct Dice
{
    private const int MinAllowedValue = 1;

    private readonly int _min;
    private readonly int _max;
    private static readonly Random Random = new();

    public int Number => Random.Next(_min, _max + 1);

    public Dice(int min, int max)
    {
        if (min < MinAllowedValue || min > int.MaxValue)
            throw new WrongDiceNumberException($"Invalid value: {min}. Allowed range: {MinAllowedValue} to {int.MaxValue}.");

        if (max < MinAllowedValue || max > int.MaxValue)
            throw new WrongDiceNumberException($"Invalid value: {max}. Allowed range: {MinAllowedValue} to {int.MaxValue}.");

        if (min > max)
            throw new WrongDiceNumberException($"Min ({min}) cannot be greater than max ({max}).");

        _min = min;
        _max = max;
    }
}