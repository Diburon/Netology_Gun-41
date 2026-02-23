namespace FinalTask.Games;

public abstract class CasinoGameBase
{
    protected object[]? InitData { get; }

    protected CasinoGameBase(params object[] initData)
    {
        InitData = initData;
        FactoryMethod();
    }

    public abstract void PlayGame();

    public event EventHandler? OnWin;
    public event EventHandler? OnLoose;
    public event EventHandler? OnDraw;

    protected virtual void OnWinInvoke() => OnWin?.Invoke(this, EventArgs.Empty);
    protected virtual void OnLooseInvoke() => OnLoose?.Invoke(this, EventArgs.Empty);
    protected virtual void OnDrawInvoke() => OnDraw?.Invoke(this, EventArgs.Empty);

    protected abstract void FactoryMethod();

    protected static void WriteResult(string message)
    {
        Console.WriteLine(message);
    }
}