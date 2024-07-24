public interface IStrategy
{
    public string Name { get; set; }
    public void Initialize(int? numberOfRounds);
    public Move MakeMove(Move opponentsPreviousMove);
}