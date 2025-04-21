using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    private string nickname = "";
    private bool canStart = false;
    private GUIStyle guiStyle = new GUIStyle();
    public GUISkin skin;

    void OnGUI()
    {
        guiStyle.fontSize = 32;
        GUI.Label(new Rect(40, 80, 300, 40), "�г����� �Է��ϼ���", guiStyle);

        nickname = GUI.TextField(new Rect(40, 130, 200, 40), nickname, 16);

        if (nickname.Length > 0)
        {
            canStart = true;
            if (GUI.Button(new Rect(40, 190, 200, 40), "���� ����"))
            {
                PlayerPrefs.SetString("nickname", nickname);
                SceneManager.LoadScene("GameScene");
            }
        }

        if (GUI.Button(new Rect(40, 250, 200, 40), "�������� ����"))
        {
            SceneManager.LoadScene("LeaderBoard"); // �������� ������ �̵�
        }

        guiStyle.fontSize = 64;
        guiStyle.normal.textColor = Color.magenta;
        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 128, 32), "Plattach", guiStyle);
    }
}
