public class Game
{
    public int NumberOfRounds { get; set; }
    public ICollection<IStrategy> Strategies { get; set; }
    public Dictionary<string, uint> Results { get; set; }

    /// <summary>
    /// Determines whether a strategy will play itself during a game.
    /// </summary>
    public bool PlaySameStrategy { get; set; } = true;
    /// <summary>
    /// Determines if there is a percatage chance of noise disrupting every move strategy makes. If 0, every move remains unchaged. If between 0 and 100, then there is % chance of a move being flipped.
    /// </summary>
    public int RandomNoise { get; set; } = 0;
    /// <summary>
    /// If value is > 1, starts an evolution game, which after each round of game discards a strategy with the lowest points.
    /// </summary>
    public int EvolutionGameRounds { get; set; } = 1;

    public Game(int? numberOfRoundsExact = null, int? numberOfRoundsLowerBound = null, int? numberOfRoundsUpperBound = null)
    {
        if (numberOfRoundsExact == null)
        {
            var rand = new Random();

            if (numberOfRoundsLowerBound != null && numberOfRoundsUpperBound != null)
                NumberOfRounds = rand.Next((int)numberOfRoundsLowerBound, (int)numberOfRoundsUpperBound);
            else if (numberOfRoundsUpperBound != null)
                NumberOfRounds = rand.Next((int)numberOfRoundsUpperBound);
            else
                NumberOfRounds = rand.Next(int.MaxValue);
        }
        else
            NumberOfRounds = (int)numberOfRoundsExact;

        Strategies = new List<IStrategy>();
        Results = new Dictionary<string, uint>();
    }

    public bool AddStrategy(IStrategy strategy)
    {
        if (Strategies.Contains(strategy))
            return false;
        else
        {
            Strategies.Add(strategy); 
            return true;
        }
    }

    public void Start()
    {
        foreach (var strat in Strategies)
        {
            Results.Add(strat.Name, 0);
            strat.Initialize(NumberOfRounds);
        }
            
        for (int generation = 0; generation < EvolutionGameRounds; generation++)
        {
            for (int first = 0; first < Strategies.Count; first++)
            {
                for (int second = first; second < Strategies.Count; second++)
                {
                    uint firstPoints = 0, secondPoints = 0;
                    Move firstPrevMove = Move.None, secondPrevMove = Move.None;

                    IStrategy firstStrat = Strategies.ElementAt(first);
                    IStrategy secondStrat = Strategies.ElementAt(second);

                    Random noise;

                    if (firstStrat.Name == secondStrat.Name && !PlaySameStrategy)
                        continue;

                    for (int k = 0; k < NumberOfRounds; k++)
                    {
                        firstPrevMove = firstStrat.MakeMove(firstPrevMove);
                        secondPrevMove = secondStrat.MakeMove(secondPrevMove);

                        // Adding random noise if enabled
                        if (RandomNoise > 0)
                        {
                            noise = new Random();
                            if (RandomNoise >= noise.Next(0, 100))
                            {
                                Move chosenPrevMove = noise.Next(0, 100) > 50 ? firstPrevMove : secondPrevMove;

                                if (chosenPrevMove == Move.Cooperate) chosenPrevMove = Move.Deflect;
                                if (chosenPrevMove != Move.Deflect) chosenPrevMove = Move.Cooperate;
                            }
                        }

                        // Resolving points after round
                        ResolveRound(firstPrevMove, secondPrevMove, ref firstPoints, ref secondPoints);
                    }

                    Results[firstStrat.Name] += firstPoints;
                    if (firstStrat.Name != secondStrat.Name)
                        Results[secondStrat.Name] += secondPoints;
                }
            }
            if (EvolutionGameRounds > 1)
            {
                var lowestPointsKey = Results.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
                
            }
        }
    }

    public void ResolveRound(Move firstMove, Move secondMove, ref uint firstPoints, ref uint secondPoints)
    {
        if (firstMove == Move.Cooperate && secondMove == Move.Cooperate)
        {
            firstPoints += 3;
            secondPoints += 3;
        }
        else if (firstMove == Move.Deflect && secondMove == Move.Cooperate)
        {
            firstPoints += 5;
            secondPoints += 0;
        }
        else if (firstMove == Move.Cooperate && secondMove == Move.Deflect)
        {
            firstPoints += 0;
            secondPoints += 5;            
        }
        else if (firstMove == Move.Deflect && secondMove == Move.Deflect)
        {
            firstPoints += 1;
            secondPoints += 1;
        }
    }

    public void PrintResults()
    {
        int maxKeyLength = Results.Keys.Max(key => key.Length);
        foreach (var res in Results.OrderByDescending(res => res.Value))
            Console.WriteLine($"{res.Key + ":".PadRight(maxKeyLength)}\t{res.Value}");
    }
}