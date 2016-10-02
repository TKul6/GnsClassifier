namespace GnsClassifier.Common
{
    public class UserData
    {
        public string Username { get; private set; }

        public int Score { get; private set; }

        public UserData(string username, int score)
        {
            Username = username;
            Score = score;
        }

        public void IncrementScore()
        {
            Score++;
        }
    }
}