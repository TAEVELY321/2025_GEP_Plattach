using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneControl : MonoBehaviour
{
    private ScoreCounter score_counter = null;
    private ScoreEntry closestEntry = null;
    public enum STEP
    {
        NONE = -1, PLAY = 0, CLEAR, NUM,
    }; // 상태 정보 없음, 플레이 중, 클리어, 상태의 종류(= 2)
    public STEP step = STEP.NONE; // 현재 상태
    public STEP next_step = STEP.NONE; // 다음 상태
    private float remainingTime;
    public float score = 0.0f; // 점수
    public float step_timer = 0.0f; // 경과 시간
    private float clear_time = 0.0f; // 클리어 시간
    public GUIStyle guistyle; // 폰트 스타일
    private BlockRoot block_root = null;
    void Start()
    {
        // BlockRoot 스크립트를 가져옴
        this.block_root = this.gameObject.GetComponent<BlockRoot>();
        this.block_root.create(); // create() 메서드에서 초기 설정
        // BlockRoot 스크립트의 initialSetUp()을 호출
        this.block_root.initialSetUp();
        this.score_counter = this.gameObject.GetComponent<ScoreCounter>(); // ScoreCounter 가져오기
        this.next_step = STEP.PLAY; // 다음 상태를 '플레이 중'으로
        this.guistyle.fontSize = 24; // 폰트 크기를 24로
        this.remainingTime = score_counter.gameDuration; // 남은 시간 초기화
    }
    void Update()
    {
        this.step_timer += Time.deltaTime;
        if (this.step == STEP.CLEAR && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("TitleScene");
        }
        if (this.next_step == STEP.NONE)
        {// 상태 변화 대기 -----.
            switch (this.step)
            {
                case STEP.PLAY:
                    this.remainingTime -= Time.deltaTime; // 남은 시간 감소
                    if(this.remainingTime <= 0.0f)
                    {
                        this.remainingTime = 0.0f;
                        this.next_step = STEP.CLEAR; // 남은 시간이 0이 되면 클리어 상태로 전환
                    }
                    UpdateClosestScore(); // 가장 근접한 기록 업데이트
                    //if (this.score_counter.isGameClear()) { this.next_step = STEP.CLEAR; } // 클리어 조건을 만족하면, 클리어 상태로 이행
                    break;
            }
            
        }
        while (this.next_step != STEP.NONE)
        { // 상태가 변화했다면 ------
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.CLEAR:
                    
                    this.block_root.enabled = false; // block_root를 정지
                    this.clear_time = this.step_timer; // 경과 시간을 클리어 시간으로 설정
                    this.score = this.score_counter.last.total_socre; // 점수를 클리어 점수로 설정

                    string nickname = PlayerPrefs.GetString("nickname", "플레이어");
                    int score = this.score_counter.last.total_socre;
                    //int time = Mathf.CeilToInt(this.clear_time); // 클리어 시간

                    Leaderboard lb = new Leaderboard();
                    lb.Load();
                    lb.AddScore(nickname, score);
                    lb.Save();
                    foreach (var entry in lb.scores)
                    {
                        Debug.Log($"{entry.nickname} - {entry.score}점");
                    }
                    
                    break;
            }
            this.step_timer = 0.0f;
        }
    }
    void OnGUI()
    {
        switch (this.step)
        {
            case STEP.PLAY:
                
                if (this.step == STEP.PLAY)
                {
                    GUI.color = Color.black;
                    GUI.Label(new Rect(40.0f, 10.0f, 200.0f, 20.0f), "시간" + Mathf.CeilToInt(this.remainingTime).ToString() + "초", guistyle);
                    GUI.color = Color.white;

                    if (isNewRecord)
                    {
                        GUI.Label(new Rect(Screen.width - 280, 10, 250, 60), "🎉 신기록 달성!", guistyle);
                    }
                    else if (closestEntry != null)
                    {
                        string msg = $"가장 근접한 기록\n{closestEntry.nickname} / {closestEntry.score}점";
                        GUI.Label(new Rect(Screen.width - 280, 10, 250, 60), msg, guistyle);
                    }
                }

                break;
            case STEP.CLEAR:
                GUI.color = Color.black;
                // 「☆클리어-！☆」라는 문자열을 표시
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f), "☆클리어-!☆", guistyle);
                // 클리어 시간을 표시
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 40.0f, 200.0f, 20.0f), "점수" + Mathf.CeilToInt(this.score).ToString() + "점", guistyle);
                GUI.color = Color.white;
                break;
        }
    }
    private bool isNewRecord = false;

    void UpdateClosestScore()
    {
        int myScore = score_counter.last.total_socre;

        Leaderboard lb = new Leaderboard();
        lb.Load();

        isNewRecord = false;
        closestEntry = null;

        ScoreEntry lowestAboveMine = null;

        foreach (var entry in lb.scores)
        {
            if (entry.score > myScore)
            {
                if (lowestAboveMine == null || entry.score < lowestAboveMine.score)
                {
                    lowestAboveMine = entry;
                }
            }
        }

        if (lowestAboveMine == null)
        {
            // 내 점수보다 높은 점수가 없다 = 신기록
            isNewRecord = true;
        }
        else
        {
            closestEntry = lowestAboveMine;
        }
    }

}
