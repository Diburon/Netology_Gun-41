using Die = FinalTask.Dice.Dice;

namespace FinalTask.Games;

public class DiceGame : CasinoGameBase
{
    private const int MinimumDiceCount = 1;

    private List<Die> _dice = null!;

    public DiceGame(int diceCount, int min, int max) : base(diceCount, min, max)
    {
        if (diceCount < MinimumDiceCount)
            throw new ArgumentException($"Dice count must be at least {MinimumDiceCount}.", nameof(diceCount));
    }

    protected override void FactoryMethod()
    {
        int count = (int)InitData![0];
        int min = (int)InitData[1];
        int max = (int)InitData[2];

        _dice = new List<Die>();
        for (int i = 0; i < count; i++)
        {
            _dice.Add(new Die(min, max));
        }
    }

    public override void PlayGame()
    {
        int playerScore = 0;
        var playerRolls = new List<int>();

        foreach (var dice in _dice)
        {
            int value = dice.Number;
            playerScore += value;
            playerRolls.Add(value);
        }

        int dealerScore = 0;
        var dealerRolls = new List<int>();

        foreach (var dice in _dice)
        {
            int value = dice.Number;
            dealerScore += value;
            dealerRolls.Add(value);
        }

        WriteResult($"Your rolls: {string.Join(", ", playerRolls)} = {playerScore}");
        WriteResult($"Dealer rolls: {string.Join(", ", dealerRolls)} = {dealerScore}");

        if (playerScore > dealerScore)
        {
            WriteResult("You win!");
            OnWinInvoke();
        }
        else if (dealerScore > playerScore)
        {
            WriteResult("You lose!");
            OnLooseInvoke();
        }
        else
        {
            WriteResult("Draw!");
            OnDrawInvoke();
        }
    }
}