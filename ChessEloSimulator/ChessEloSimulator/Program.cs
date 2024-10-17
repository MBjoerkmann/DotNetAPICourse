using System.Security;
using Models;

Player playerA = new Player();
Player playerB = new Player();
playerA.Name = "mathias";
playerA.EloScore = 1200;
playerB.Name = "john";
playerB.EloScore = Console.ReadLine().;

double expectedscoreA = playerA.calculateExpectedOutcome(playerA.EloScore, playerB.EloScore);
playerA.updateElo(1, playerB.EloScore);
System.Console.WriteLine($"playerA updated elo is: {playerA.EloScore}");