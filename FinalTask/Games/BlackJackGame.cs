using System.Collections.Generic;
using FinalTask.Cards;

namespace FinalTask.Games;

public class BlackjackGame : CasinoGameBase
{
    private const int MinimumCardCount = 4;
    private const int BlackjackTarget = 21;
    private const int AceHighValue = 11;
    private const int AceValueDifference = 10;
    private const int FaceCardValue = 10;

    private Queue<Card> _deck = null!;

    public BlackjackGame(int cardCount) : base(cardCount)
    {
        if (cardCount < MinimumCardCount)
            throw new ArgumentException($"Card count must be at least {MinimumCardCount}.", nameof(cardCount));
    }

    protected override void FactoryMethod()
    {
        int count = (int)InitData![0];
        var cards = new List<Card>();

        var suits = Enum.GetValues<Suit>();
        var ranks = Enum.GetValues<Rank>();

        for (int i = 0; i < count; i++)
        {
            var suit = suits[i % suits.Length];
            var rank = ranks[i % ranks.Length];
            cards.Add(new Card(suit, rank));
        }

        _deck = new Queue<Card>();
        Shuffle(cards);
    }

    private void Shuffle(List<Card> cards)
    {
        var random = new Random();
        while (cards.Count > 0)
        {
            int index = random.Next(cards.Count);
            _deck.Enqueue(cards[index]);
            cards.RemoveAt(index);
        }
    }

    private static int GetCardValue(Card card)
    {
        return card.Rank switch
        {
            Rank.Six => 6,
            Rank.Seven => 7,
            Rank.Eight => 8,
            Rank.Nine => 9,
            Rank.Ten or Rank.Jack or Rank.Queen or Rank.King => FaceCardValue,
            Rank.Ace => AceHighValue,
            _ => 0
        };
    }

    private static int CalculateScore(IReadOnlyList<Card> hand)
    {
        int score = 0;
        int aces = 0;

        foreach (var card in hand)
        {
            int value = GetCardValue(card);
            score += value;
            if (card.Rank == Rank.Ace) aces++;
        }

        while (score > BlackjackTarget && aces > 0)
        {
            score -= AceValueDifference;
            aces--;
        }

        return score;
    }

    private Card DrawCard()
    {
        if (_deck.Count == 0)
            throw new InvalidOperationException("Deck is empty.");
        return _deck.Dequeue();
    }

    public override void PlayGame()
    {
        var playerHand = new List<Card> { DrawCard(), DrawCard() };
        var dealerHand = new List<Card> { DrawCard(), DrawCard() };

        WriteResult($"Your cards: {string.Join(", ", playerHand)}");
        WriteResult($"Dealer cards: {string.Join(", ", dealerHand)}");

        int playerScore = CalculateScore(playerHand);
        int dealerScore = CalculateScore(dealerHand);

        WriteResult($"Your score: {playerScore}");
        WriteResult($"Dealer score: {dealerScore}");

        while (playerScore == dealerScore)
        {
            if (playerScore >= BlackjackTarget && dealerScore >= BlackjackTarget)
            {
                WriteResult($"Draw! Both have {BlackjackTarget} or more.");
                OnDrawInvoke();
                return;
            }

            if (playerScore < BlackjackTarget && dealerScore < BlackjackTarget)
            {
                playerHand.Add(DrawCard());
                dealerHand.Add(DrawCard());
                playerScore = CalculateScore(playerHand);
                dealerScore = CalculateScore(dealerHand);
                WriteResult($"Your cards: {string.Join(", ", playerHand)} (score: {playerScore})");
                WriteResult($"Dealer cards: {string.Join(", ", dealerHand)} (score: {dealerScore})");
            }
            else
            {
                break;
            }
        }

        bool playerBust = playerScore > BlackjackTarget;
        bool dealerBust = dealerScore > BlackjackTarget;

        if (playerBust && dealerBust)
        {
            WriteResult("Draw! Both busted.");
            OnDrawInvoke();
        }
        else if (playerBust)
        {
            WriteResult("You lose! You busted.");
            OnLooseInvoke();
        }
        else if (dealerBust)
        {
            WriteResult("You win! Dealer busted.");
            OnWinInvoke();
        }
        else if (playerScore > dealerScore)
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