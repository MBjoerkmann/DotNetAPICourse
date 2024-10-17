using System.Runtime.CompilerServices;

namespace Models
{
    public class ChessPlayer : IPlayer
    {
        public string Name { get; set;} = "";
        public string Color { get; set; } = "";
        public double EloScore { get; set; } = 400;

        public ChessPlayer ()
        {
            
        }
        public void updateElo(double actualOutcome, double opponentEloScore)
        {
            const double Kfactor = 32;
            EloScore = EloScore + Kfactor * (actualOutcome - calculateExpectedOutcome(EloScore, opponentEloScore));
        }
        public double calculateExpectedOutcome(double playerA_elo, double playerB_elo)
        {
            double chancePlayerAWins = 1 / (1 + (Math.Pow(10, (playerB_elo - playerA_elo) / 400)));
            
            return chancePlayerAWins;
        }
        public string getEloString()
        {
            return($"The elo of {Name} is {EloScore}");
        }
    }

}