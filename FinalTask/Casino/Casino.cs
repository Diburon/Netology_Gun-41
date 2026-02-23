using FinalTask.Games;
using FinalTask.Models;
using FinalTask.Services;

namespace FinalTask.Casino;

public class Casino : IGame
{
    private const string ProfileId = "player_profile";
    private const int DefaultDeckSize = 36;
    private const int DefaultDiceCount = 2;
    private const int DefaultDiceMin = 1;
    private const int DefaultDiceMax = 6;
    private const int ZeroBank = 0;
    private const int BankHalfDivisor = 2;

    private readonly ISaveLoadService<string> _saveLoadService;
    private readonly BlackjackGame _blackjackGame;
    private readonly DiceGame _diceGame;

    public Casino(ISaveLoadService<string> saveLoadService)
    {
        _saveLoadService = saveLoadService ?? throw new ArgumentNullException(nameof(saveLoadService));
        _blackjackGame = new BlackjackGame(DefaultDeckSize);
        _diceGame = new DiceGame(DefaultDiceCount, DefaultDiceMin, DefaultDiceMax);
    }

    public void StartGame()
    {
        Console.WriteLine("Welcome to Final Task Casino!");

        PlayerProfile profile = LoadOrCreateProfile();

        while (true)
        {
            Console.WriteLine("\nChoose a game:");
            Console.WriteLine("1 - Blackjack");
            Console.WriteLine("2 - Dice");
            Console.Write("Your choice: ");

            string? input = Console.ReadLine();
            if (input != "1" && input != "2")
            {
                Console.WriteLine("Invalid input. Enter 1 or 2.");
                continue;
            }

            if (profile.Bank <= ZeroBank)
            {
                Console.WriteLine("No money? Kicked!");
                break;
            }

            Console.Write($"Enter your bet (max {profile.Bank}): ");
            if (!int.TryParse(Console.ReadLine(), out int bet) || bet <= ZeroBank || bet > profile.Bank)
            {
                Console.WriteLine("Invalid bet.");
                continue;
            }

            bool? won = null;
            CasinoGameBase game = input == "1" ? _blackjackGame : _diceGame;

            void OnWin(object? s, EventArgs e) => won = true;
            void OnLoose(object? s, EventArgs e) => won = false;
            void OnDraw(object? s, EventArgs e) => won = null;

            game.OnWin += OnWin;
            game.OnLoose += OnLoose;
            game.OnDraw += OnDraw;

            game.PlayGame();

            game.OnWin -= OnWin;
            game.OnLoose -= OnLoose;
            game.OnDraw -= OnDraw;

            if (won == true)
            {
                profile.Bank += bet;
                if (profile.Bank > PlayerProfile.MaxBank)
                {
                    int remainder = profile.Bank - PlayerProfile.MaxBank;
                    profile.Bank = PlayerProfile.MaxBank;
                    Console.WriteLine($"You bankrupted the casino! New casino will be built. Your remainder: {remainder}");
                }
                else if (profile.Bank > PlayerProfile.MaxBank / BankHalfDivisor)
                {
                    profile.Bank /= BankHalfDivisor;
                    Console.WriteLine("You wasted half of your bank money in casino's bar");
                }
            }
            else if (won == false)
            {
                profile.Bank -= bet;
            }

            Console.WriteLine($"\nYour bank: {profile.Bank}");
        }

        Console.WriteLine("\nGoodbye!");
        SaveProfile(profile);
    }

    private PlayerProfile LoadOrCreateProfile()
    {
        try
        {
            string json = _saveLoadService.LoadData(ProfileId);
            var profile = PlayerProfileSerializer.Deserialize(json);
            if (profile != null)
            {
                Console.WriteLine($"Welcome back, {profile.Name}!");

                if (profile.Bank <= ZeroBank)
                {
                    Console.WriteLine("You were out of money. Here's a fresh start!");
                    profile.Bank = PlayerProfile.InitialBank;
                }

                return profile;
            }
        }
        catch (FileNotFoundException)
        {
            // Profile doesn't exist
        }

        Console.Write("Enter your name: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
            name = "Player";

        return new PlayerProfile(name.Trim());
    }

    private void SaveProfile(PlayerProfile profile)
    {
        string json = PlayerProfileSerializer.Serialize(profile);
        _saveLoadService.SaveData(json, ProfileId);
    }
}