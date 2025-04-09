using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{ // ��Ͽ� ���� ����
    public static float COLLISION_SIZE = 1.0f; // ����� �浹 ũ��
    public static float VANISH_TIME = 3.0f; // �� �ٰ� ����� �������� �ð�
    public struct iPosition
    { // �׸��忡���� ��ǥ�� ��Ÿ���� ����ü
        public int x; // X ��ǥ
        public int y; // Y ��ǥ
    }
    public enum COLOR
    { // ��� ����
        NONE = -1, // �� ���� ����
        PINK = 0, BLUE, YELLOW, GREEN, // ��ȫ��, �Ķ���, �����, ���
        MAGENTA, ORANGE, GRAY, // ����Ÿ, ��Ȳ��, �׷���
        NUM, // �÷��� �� �������� ��Ÿ��(=7)
        FIRST = PINK, LAST = ORANGE,// �ʱ� �÷�(��ȫ��), ���� �÷�(��Ȳ��)
        NORMAL_COLOR_NUM = GRAY, // ���� �÷�(ȸ�� �̿��� ��)�� ��
    };
    public enum DIR4
    { // �����¿� �� ����
        NONE = -1, // �������� ����
        RIGHT, LEFT, UP, DOWN, // ��. ��, ��, ��
        NUM, // ������ �� ���� �ִ��� ��Ÿ��(=4)
    };
    public static int BLOCK_NUM_X = 9; // ����� ��ġ�� �� �ִ� X���� �ִ��
    public static int BLOCK_NUM_Y = 9; // ����� ��ġ�� �� �ִ� Y���� �ִ��

    public enum STEP
    {
        // ���� ���� ����, ��� ��, ���� ����, ������ ����, �����̵� ��, �Ҹ� ��, ����� ��, ���� ��, ũ�� �����̵� ��, ���� ����
        NONE = -1, IDLE = 0, GRABBED, RELEASED, SLIDE, VACANT, RESPAWN, FALL, LONG_SLIDE, NUM,
    };
}

public class BlockControl : MonoBehaviour
{
    public Block.COLOR color = (Block.COLOR)0; // ��� ��
    public BlockRoot block_root = null; // ����� ��
    public Block.iPosition i_pos; // ��� ��ǥ
    public Block.STEP step = Block.STEP.NONE; // ���� ����
    public Block.STEP next_step = Block.STEP.NONE; // ���� ����
    private Vector3 position_offset_initial = Vector3.zero; // ��ü �� ��ġ
    public Vector3 position_offset = Vector3.zero; // ��ü �� ��ġ

    void Start()
    {
        this.setColor(this.color);
        this.next_step = Block.STEP.IDLE;
    } // ��ĥ��

    public void setColor(Block.COLOR color)
    { // Ư�� color�� ����� ĥ��
        this.color = color; // �̹��� ������ ���� ��� ������ ����
        Color color_value; // Color Ŭ������ ���� ��Ÿ��
        switch (this.color)
        { // ĥ�� ���� ���� ������
            default:
            case Block.COLOR.PINK:
                color_value = new Color(1.0f, 0.5f, 0.5f); break;
            case Block.COLOR.BLUE:
                color_value = Color.blue; break;
            case Block.COLOR.YELLOW:
                color_value = Color.yellow; break;
            case Block.COLOR.GREEN:
                color_value = Color.green; break;
            case Block.COLOR.MAGENTA:
                color_value = Color.magenta; break;
            case Block.COLOR.ORANGE:
                color_value = new Color(1.0f, 0.46f, 0.0f); break;
        }
        this.GetComponent<Renderer>().material.color = color_value; // ���󺯰�
    }
    void Update()
    { // ����� ������ ���� ����� ũ�� �ϰ� �׷��� ���� ���� ������� ���ư�
        Vector3 mouse_position; // ���콺 ��ġ
        this.block_root.unprojectMousePosition(out mouse_position, Input.mousePosition); // ���콺 ��ġ ȹ��
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y); // ȹ���� ���콺 ��ġ�� X�� Y������ ��
        while (this.next_step != Block.STEP.NONE)
        { // '���� ���' ���°� '���� ����' �̿��� ����  '���� ���' ���°� ����� ���
            this.step = this.next_step;
            this.next_step = Block.STEP.NONE;
            switch (this.step)
            {
                case Block.STEP.IDLE: // '���' ����
                    this.position_offset = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f; break; // ��� ǥ�� ũ�⸦ ���� ũ��� ��
                case Block.STEP.GRABBED: // �������� ����
                    this.transform.localScale = Vector3.one * 1.2f; break; // ��� ǥ�� ũ�⸦ ũ�� �Ѵ�
                case Block.STEP.RELEASED: // '������ �ִ�' ����
                    this.position_offset = Vector3.zero;
                    this.transform.localScale = Vector3.one * 1.0f; break; // ��� ǥ�� ũ�⸦ ���� ������� ��
            }
        } // �׸��� ��ǥ�� ���� ��ǥ(���� ��ǥ)�� ��ȯ�ϰ�
        Vector3 position = BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset; // position_offset�� �߰�
        this.transform.position = position; // ���� ��ġ�� ���ο� ��ġ�� ����
    }
    public void beginGrab()
    { // ������ �� ȣ��
        this.next_step = Block.STEP.GRABBED;
    }
    public void endGrab()
    { // ������ �� ȣ��
        this.next_step = Block.STEP.IDLE;
    }
    public bool isGrabbable()
    { // ���� �� �ִ� ���� ���� �Ǵ�
        bool is_grabbable = false;
        switch (this.step)
        {
            case Block.STEP.IDLE: // '���' ������ ����.
                is_grabbable = true; // true(���� �� �ִ�)�� ��ȯ
                break;
        }
        return (is_grabbable);
    }
    public bool isContainedPosition(Vector2 position)
    { // ������ ���콺 ��ǥ�� �ڽŰ� ��ġ���� ��ȯ
        bool ret = false;
        Vector3 center = this.transform.position;
        float h = Block.COLLISION_SIZE / 2.0f;
        do
        {
            if (position.x < center.x - h || center.x + h < position.x) { break; } // X ��ǥ�� �ڽŰ� ��ġ�� ������ ������ ���� ����
            if (position.y < center.y - h || center.y + h < position.y) { break; } // Y ��ǥ�� �ڽŰ� ��ġ�� ������ ������ ���� ����
            ret = true; // X ��ǥ, Y ��ǥ ��� ���� ������ true(���� ����)�� ��ȯ
        } while (false);
        return (ret);
    }
}


