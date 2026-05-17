using UnityEngine;

public static class GameSessionStats
{
    private const string LastScoreKey = "LastScore";
    private const string BestScoreKey = "BestScore";
    private const string LastBestStreakKey = "LastBestStreak";
    private const string LastWasBestKey = "LastWasBest";
    private const string GamesPlayedKey = "GamesPlayed";

    public static int LastScore => PlayerPrefs.GetInt(LastScoreKey, 0);
    public static int BestScore => PlayerPrefs.GetInt(BestScoreKey, 0);
    public static int LastBestStreak => PlayerPrefs.GetInt(LastBestStreakKey, 0);
    public static bool LastWasBest => PlayerPrefs.GetInt(LastWasBestKey, 0) == 1;
    public static int GamesPlayed => PlayerPrefs.GetInt(GamesPlayedKey, 0);

    public static bool SaveGameResult(int score, int bestStreak)
    {
        PlayerPrefs.SetInt(LastScoreKey, score);
        PlayerPrefs.SetInt(LastBestStreakKey, bestStreak);
        PlayerPrefs.SetInt(GamesPlayedKey, GamesPlayed + 1);

        bool isNewBest = score > BestScore;
        PlayerPrefs.SetInt(LastWasBestKey, isNewBest ? 1 : 0);
        if (isNewBest)
        {
            PlayerPrefs.SetInt(BestScoreKey, score);
        }

        PlayerPrefs.Save();
        return isNewBest;
    }
}
