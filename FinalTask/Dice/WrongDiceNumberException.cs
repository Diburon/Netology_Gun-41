namespace FinalTask.Dice;

public class WrongDiceNumberException : Exception
{
    public WrongDiceNumberException(string message) : base(message) { }
}