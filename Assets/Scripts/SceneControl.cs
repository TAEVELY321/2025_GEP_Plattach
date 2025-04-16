using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneControl : MonoBehaviour
{
    private ScoreCounter score_counter = null;
    public enum STEP
    {
        NONE = -1, PLAY = 0, CLEAR, NUM,
    }; // ���� ���� ����, �÷��� ��, Ŭ����, ������ ����(= 2)
    public STEP step = STEP.NONE; // ���� ����
    public STEP next_step = STEP.NONE; // ���� ����
    public float step_timer = 0.0f; // ��� �ð�
    private float clear_time = 0.0f; // Ŭ���� �ð�
    public GUIStyle guistyle; // ��Ʈ ��Ÿ��
    private BlockRoot block_root = null;
    void Start()
    {
        // BlockRoot ��ũ��Ʈ�� ������
        this.block_root = this.gameObject.GetComponent<BlockRoot>();
        this.block_root.create(); // create() �޼��忡�� �ʱ� ����
        // BlockRoot ��ũ��Ʈ�� initialSetUp()�� ȣ��
        this.block_root.initialSetUp();
        this.score_counter = this.gameObject.GetComponent<ScoreCounter>(); // ScoreCounter ��������
        this.next_step = STEP.PLAY; // ���� ���¸� '�÷��� ��'����
        this.guistyle.fontSize = 24; // ��Ʈ ũ�⸦ 24��
    }
    void Update()
    {
        this.step_timer += Time.deltaTime;
        if (this.next_step == STEP.NONE)
        {// ���� ��ȭ ��� -----.
            switch (this.step)
            {
                case STEP.PLAY:
                    if (this.score_counter.isGameClear()) { this.next_step = STEP.CLEAR; } // Ŭ���� ������ �����ϸ�, Ŭ���� ���·� ����
                    break;
            }
            switch (this.step)
            {
                case STEP.CLEAR:
                    if (Input.GetMouseButtonDown(0))
                    {
                        SceneManager.LoadScene("TitleScene");
                    }
                    break;
            }
        }
        while (this.next_step != STEP.NONE)
        { // ���°� ��ȭ�ߴٸ� ------
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.CLEAR:
                    
                    this.block_root.enabled = false; // block_root�� ����
                    this.clear_time = this.step_timer; // ��� �ð��� Ŭ���� �ð����� ����
                    
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
                GUI.color = Color.black;
                // ��� �ð��� ǥ��
                GUI.Label(new Rect(40.0f, 10.0f, 200.0f, 20.0f), "�ð�" + Mathf.CeilToInt(this.step_timer).ToString() + "��", guistyle);
                GUI.color = Color.white;
                break;
            case STEP.CLEAR:
                GUI.color = Color.black;
                // ����Ŭ����-���١���� ���ڿ��� ǥ��
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 20.0f, 200.0f, 20.0f), "��Ŭ����-!��", guistyle);
                // Ŭ���� �ð��� ǥ��
                GUI.Label(new Rect(Screen.width / 2.0f - 80.0f, 40.0f, 200.0f, 20.0f), "Ŭ���� �ð�" + Mathf.CeilToInt(this.clear_time).ToString() + "��", guistyle);
                GUI.color = Color.white;
                break;
        }
    }
}
