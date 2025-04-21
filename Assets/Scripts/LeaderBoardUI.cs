using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardUI : MonoBehaviour
{
    private Leaderboard leaderboard = new Leaderboard();
    private GUIStyle guiStyle = new GUIStyle();

    void Start()
    {
        leaderboard.Load();
        guiStyle.fontSize = 24;
        guiStyle.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(40, 20, 300, 30), "리더보드 (ദ്ദി ˃ ᴗ ˂ )", guiStyle);

        for (int i = 0; i < leaderboard.scores.Count; i++)
        {
            var entry = leaderboard.scores[i];
            string line = $"{i + 1}. {entry.nickname} - {entry.score}점";
            GUI.Label(new Rect(40, 60 + i * 30, 500, 30), line, guiStyle);
        }

        if (GUI.Button(new Rect(40, 400, 200, 40), "타이틀로"))
        {
            SceneManager.LoadScene("TitleScene");
        }

        if (GUI.Button(new Rect(40, 500, 200, 40), "리더보드 삭제"))
        {
            PlayerPrefs.DeleteKey("Leaderboard");
            PlayerPrefs.Save();
            leaderboard.scores.Clear(); // 메모리에서도 비움
        }

    }

}
