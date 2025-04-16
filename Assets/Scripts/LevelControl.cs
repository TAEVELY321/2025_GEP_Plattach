using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{ // �� ������ ���� �����͸� �����ϴ� List ��
    public float[] probability; // ����� �����󵵸� �����ϴ� �迭
    public float heat_time; // ���ҽð�
    public LevelData()
    { // ������
        this.probability = new float[(int)Block.COLOR.NORMAL_COLOR_NUM]; // ����� ���� ���� ���� ũ��� ���̾� ������ Ȯ��
        for (int i = 0; i < (int)Block.COLOR.NORMAL_COLOR_NUM; i++)
        { // ��� ������ ����Ȯ���� �켱 �յ��ϰ�
            this.probability[i] = 1.0f / (float)Block.COLOR.NORMAL_COLOR_NUM;
        }
    }
    public void clear()
    { // ��� ������ ����Ȯ���� 0���� �����ϴ� �޼ҵ�
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] = 0.0f;
        }
    }
    public void normalize()
    {// ��� ������ ����Ȯ���� �հ踦 100%(=1.0)�� �ϴ� �޼ҵ�
        float sum = 0.0f;
        for (int i = 0; i < this.probability.Length; i++)
        { // ����Ȯ���� '�ӽ� �հ谪'�� ���
            sum += this.probability[i];
        }
        for (int i = 0; i < this.probability.Length; i++)
        {
            this.probability[i] /= sum; // ������ ����Ȯ���� '�ӽ� �հ谪'���� ������, �հ谡 100%(=1.0) �� ������
            if (float.IsInfinity(this.probability[i]))
            { // ���� �� ���� ���Ѵ���
                this.clear(); // ��� Ȯ���� 0���� �����ϰ�
                this.probability[0] = 1.0f; // ������ ��Ҹ� 1.0���� ����
                break; // �׸��� ������ ��������
            }
        }
    }
}
public class LevelControl
{
    private List<LevelData> level_datas = null; // �� ������ ���� ������
    private int select_level = 0; // ���õ� ����
    public void initialize() { this.level_datas = new List<LevelData>(); } // List�� �ʱ�ȭ

    public void loadLevelData(TextAsset level_data_text)
    { // �ؽ�Ʈ �����͸� �о�ͼ� �� ������ �ؼ��ϰ� �����͸� ����
        string level_texts = level_data_text.text; // �ؽ�Ʈ �����͸� ���ڿ��μ� �޾Ƶ��δ�
        string[] lines = level_texts.Split('\n'); // ���� �ڵ�'\'���� ������, ���ڿ� �迭�� ����ִ´�
        foreach (var line in lines)
        { // lines ���� �� �࿡ ���Ͽ� ���ʷ� ó���ذ��� ����
            if (line == "")
            { // ���� �������
                continue;
            } // �Ʒ� ó���� ���� �ʰ� ������ ó������ ����
            string[] words = line.Split(); // �� ���� ���带 �迭�� ����
            int n = 0;
            LevelData level_data = new LevelData(); // LevelData�� ������ �ۼ�, ���⿡ ���� ó���ϴ� ���� �����͸� �ִ´�
            foreach (var word in words)
            { // words���� �� ���忡 ���ؼ�, ������� ó���� ���� ����
                if (word.StartsWith("#")) { break; } // ������ ���� ���ڰ� #�̸�, ���� Ż��
                if (word == "") { continue; } // ���尡 �������, ���� �������� ����
                switch (n)
                { // 'n'�� ���� 0,1,2,...6���� ��ȭ���Ѱ����ν� �ϰ� �� �׸��� ó��. �� ���带 float������ ��ȯ�ϰ� level_data�� ����
                    case 0: level_data.probability[(int)Block.COLOR.PINK] = float.Parse(word); break;
                    case 1: level_data.probability[(int)Block.COLOR.BLUE] = float.Parse(word); break;
                    case 2: level_data.probability[(int)Block.COLOR.GREEN] = float.Parse(word); break;
                    case 3: level_data.probability[(int)Block.COLOR.ORANGE] = float.Parse(word); break;
                    case 4: level_data.probability[(int)Block.COLOR.YELLOW] = float.Parse(word); break;
                    case 5: level_data.probability[(int)Block.COLOR.MAGENTA] = float.Parse(word); break;
                    case 6: level_data.heat_time = float.Parse(word); break;
                }
                n++;
            }
            if (n >= 7)
            { // 8�׸�(�̻�)�� ����� ó���Ǿ��ٸ�.
                level_data.normalize(); // ���� Ȯ���� �հ谡 ��Ȯ�� 100%�� �ǵ��� �ϰ� ����
                this.level_datas.Add(level_data); // List ������ level_datas�� level_data�� �߰��Ѵ�
            }
            else
            { // �׷��� ������(���� ���ɼ��� �ִ�).
                if (n == 0)
                { // 1���嵵 ó������ ���� ���� �ּ��̹Ƿ�, ���� ����. �ƹ��͵� ���� �ʴ´�.
                }
                else
                { // �� �̿ܶ�� ����.
                    Debug.LogError("[LevelData] Out of parameter.\n"); // �������� ������ ���� �ʴ´ٴ� ���� �޽����� ǥ��
                }
            }
        }
        // level_datas�� �����Ͱ� �ϳ��� ������
        if (this.level_datas.Count == 0)
        {
            // ���� �޽����� ǥ��
            Debug.LogError("[LevelData] Has no data.\n");
            // level_datas�� LevelData�� �ϳ� �߰�
            this.level_datas.Add(new LevelData());
        }
    }
    public void selectLevel()
    { // �� ���� ���� ���Ͽ��� ���� ����� ������ ����
      // 0~���� ������ ���� ���Ƿ� ����
        this.select_level = Random.Range(0, this.level_datas.Count);
        Debug.Log("select level = " + this.select_level.ToString());
    }
    public LevelData getCurrentLevelData()
    { // ���õǾ� �ִ� ���� ������ ���� �����͸� ��ȯ
      // ���õ� ������ ���� �����͸� ��ȯ
        return (this.level_datas[this.select_level]);
    }
    public float getVanishTime()
    { // ���õǾ� �ִ� ���� ������ ���� �ð��� ��ȯ
      // ���õ� ������ ���ҽð��� ��ȯ
        return (this.level_datas[this.select_level].heat_time);
    }
}