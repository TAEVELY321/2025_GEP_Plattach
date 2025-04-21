using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public string nickname;
    public int score;
    public int time;
}

public class Leaderboard
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();

    [System.Serializable]
    private class Wrapper
    {
        public List<ScoreEntry> scores;
    }

    public void Load()
    {
        string json = PlayerPrefs.GetString("Leaderboard", "");
        if (!string.IsNullOrEmpty(json))
        {
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
            if (wrapper != null && wrapper.scores != null)
                scores = wrapper.scores;
        }
    }

    public void Save()
    {
        Wrapper wrapper = new Wrapper { scores = scores };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("Leaderboard", json);
    }

    public void AddScore(string name, int score)
    {
        scores.Add(new ScoreEntry { nickname = name, score = score});
        scores = scores.OrderByDescending(s => s.score).Take(10).ToList(); // ← 다시 점수 기준
    }


}
