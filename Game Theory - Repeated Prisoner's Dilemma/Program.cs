Game game = new Game(numberOfRoundsExact: 200);

game.PlaySameStrategy = false;

//game.AddStrategy(new AlwaysCooperate());
//game.AddStrategy(new AlwaysDeflect());
//game.AddStrategy(new TotallyRandom());

game.AddStrategy(new TitForTat());
game.AddStrategy(new ProbeOpponent());
game.AddStrategy(new AverageResponses());

game.Start();

game.PrintResults();