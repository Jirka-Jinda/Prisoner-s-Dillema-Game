public class AlwaysCooperate : IStrategy
{
    public string Name { get; set; } = "AlwaysCooperate";

    public void Initialize(int? numberOfRounds)
    {
        return;
    }

    public Move MakeMove(Move opponentsPreviousMove)
    {
        return Move.Cooperate;
    }
}

public class AlwaysDeflect : IStrategy
{
    public string Name { get; set; } = "AlwaysDeflect";

    public void Initialize(int? numberOfRounds)
    {
        return;
    }

    public Move MakeMove(Move opponentsPreviousMove)
    {
        return Move.Deflect;
    }
}

public class TotallyRandom : IStrategy
{
    public string Name { get; set; } = "TotallyRandom";
    public Random generator { get; set; } = new();

    public void Initialize(int? numberOfRounds)
    {
        return;
    }

    public Move MakeMove(Move opponentsPreviousMove)
    {
        if (generator.Next(0, 100) > 50)
            return Move.Cooperate;
        else
            return Move.Deflect;
    }
}

public class TitForTat : IStrategy
{
    public string Name { get; set; } = "TitForTat";

    public void Initialize(int? numberOfRounds)
    {
        return;
    }

    public Move MakeMove(Move opponentsPreviousMove)
    {
        if (opponentsPreviousMove == Move.Deflect)
            return Move.Deflect;
        else
            return Move.Cooperate;
    }
}

public class ProbeOpponent : IStrategy
{
    public string Name { get; set; } = "ProbeOpponent";
    private IStrategy defaultBehavior = new TitForTat(); 
    private int state = 0;

    public void Initialize(int? numberOfRounds)
    {
        return;
    }

    public Move MakeMove(Move opponentsPreviousMove)
    {
        switch (state)
        {
            case 0:
                state = 1;
                return defaultBehavior.MakeMove(opponentsPreviousMove);
            case 1:
                state = 2;
                return Move.Deflect;
            case 2:
                state = 3;
                return Move.Cooperate;
            case 3:
                if (opponentsPreviousMove == Move.Cooperate)
                {
                    state = 4;
                    return Move.Deflect;
                }
                else
                {
                    state = 5;
                    return Move.Cooperate;
                }
            case 4:
                return Move.Deflect;
            default:
                return defaultBehavior.MakeMove(opponentsPreviousMove);
        }
    }
}

public class AverageResponses : IStrategy
{
    public string Name { get; set; } = "AvrgResponses";
    private List<Move> responses = new List<Move>();

    public void Initialize(int? numberOfRounds)
    {
        return;
    }

    public Move MakeMove(Move opponentsPreviousMove)
    {
        if (opponentsPreviousMove != Move.None)
            responses.Add(opponentsPreviousMove);
        else
            return Move.Cooperate;

        var average = responses.Sum(move => (int)move) / responses.Count;

        if (average < 0.5)
            return Move.Deflect;
        else return Move.Cooperate;
    }
}