using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public struct Count
    { // ���� ������ ����ü
        public int ignite; // ���� ��
        public int score; // ����
        public int total_socre; // �հ� ����
    };
    public Count last; // ������(�̹�) ����
    public Count best; // �ְ� ����
    public static int QUOTA_SCORE = 100; // Ŭ���� �ϴ� �� �ʿ��� ����
    public GUIStyle guistyle; // ��Ʈ ��Ÿ��
    void Start()
    {
        this.last.ignite = 0;
        this.last.score = 0;
        this.last.total_socre = 0;
        this.guistyle.fontSize = 16;
    }
    void OnGUI()
    { // ȭ�鿡 �ؽ�Ʈ�� �̹��� ǥ��
        int x = 20;
        int y = 50;
        GUI.color = Color.black;
        this.print_value(x + 20, y, "���� ī��Ʈ", this.last.ignite);
        y += 30;
        this.print_value(x + 20, y, "���� ���ھ�", this.last.score);
        y += 30;
        this.print_value(x + 20, y, "�հ� ���ھ�", this.last.total_socre);
        y += 30;
    }
    // ������ �� ���� �����͸� �� ���� �࿡ ���� ǥ��.
    public void print_value(int x, int y, string label, int value)
    {
        GUI.Label(new Rect(x, y, 100, 20), label, guistyle); // label�� ǥ��
        y += 15;
        GUI.Label(new Rect(x + 20, y, 100, 20), value.ToString(), guistyle); // ���� �࿡ value�� ǥ��
        y += 15;
    }
    // ���� Ƚ���� ����
    public void addIgniteCount(int count)
    {
        this.last.ignite += count; // ���� ���� count�� �ջ�
        this.update_score(); // ���� ���
    }
    // ���� Ƚ���� ����
    public void clearIgniteCount()
    {
        this.last.ignite = 0; // ���� Ƚ�� ����
    }
    // ���ؾ� �� ������ ���
    private void update_score()
    {
        this.last.score = this.last.ignite * 10; // ���� ����
    }
    // �հ� ������ ����
    public void updateTotalScore()
    {
        this.last.total_socre += this.last.score;
    }
    // ������ Ŭ�����ߴ��� ���� (SceneControl���� ���)
    public bool isGameClear()
    {
        bool is_clear = false;
        // ���� �հ� ������ Ŭ���� ���غ��� ũ��
        if (this.last.total_socre > QUOTA_SCORE)
        {
            is_clear = true;
        }
        return (is_clear);
    }
}