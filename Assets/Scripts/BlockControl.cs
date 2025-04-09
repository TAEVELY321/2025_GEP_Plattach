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
    public float vanish_timer = -1.0f; // ����� ����� �������� �ð�
    public Block.DIR4 slide_dir = Block.DIR4.NONE; // �����̵�� ����
    public float step_timer = 0.0f; // ����� ��ü�� ���� �̵��ð� ��
    public Material opague_material; // ������ ��Ƽ����
    public Material transparent_material; // ������ ��Ƽ����

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
        if (this.vanish_timer >= 0.0f)
        { // Ÿ�̸Ӱ� 0 �̻��̸�
            this.vanish_timer -= Time.deltaTime; // Ÿ�̸��� ���� ����
            if (this.vanish_timer < 0.0f)
            { // Ÿ�̸Ӱ� 0 �̸��̸�
                if (this.step != Block.STEP.SLIDE)
                { // �����̵� ���� �ƴ϶��
                    this.vanish_timer = -1.0f;
                    this.next_step = Block.STEP.VACANT; // ���¸� ���Ҹ� �ߡ�����
                }
                else
                {
                    this.vanish_timer = 0.0f;
                }
            }
        }

        this.step_timer += Time.deltaTime;
        float slide_time = 0.2f;
        if (this.next_step == Block.STEP.NONE)
        { // '���� ���� ����'�� ���
            switch (this.step)
            {
                case Block.STEP.SLIDE:
                    if (this.step_timer >= slide_time)
                    { // �����̵� ���� ����� �Ҹ�Ǹ� VACANT(�����) ���·� ����
                        if (this.vanish_timer == 0.0f)
                        {
                            this.next_step = Block.STEP.VACANT;
                        }
                        else
                        {
                            this.next_step = Block.STEP.IDLE; // vanish_timer�� 0�� �ƴϸ� IDLE(���) ���·� ����
                        }
                    }
                    break;
            }
        }
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
                case Block.STEP.VACANT:
                    this.position_offset = Vector3.zero;
                    this.setVisible(false); // ����� ǥ������ �ʰ�
                    break;

            }
            this.step_timer = 0.0f;
        } // �׸��� ��ǥ�� ���� ��ǥ(���� ��ǥ)�� ��ȯ�ϰ�
        switch (this.step)
        {
            case Block.STEP.GRABBED: // ���� ����
                this.slide_dir = this.calcSlideDir(mouse_position_xy); break; // ���� ������ ���� �׻� �����̵� ������ üũ
            case Block.STEP.SLIDE: // �����̵�(��ü) ��
                float rate = this.step_timer / slide_time; // ����� ������ �̵��ϴ� ó��
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.position_offset = Vector3.Lerp(this.position_offset_initial, Vector3.zero, rate); break;
        }
        Vector3 position = BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset;
        this.transform.position = position;

        this.setColor(this.color);
        if (this.vanish_timer >= 0.0f)
        {
            Color color0 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.white, 0.5f); // ���� ���� ����� �߰� ��
            Color color1 = Color.Lerp(this.GetComponent<Renderer>().material.color, Color.black, 0.5f); // ���� ���� �������� �߰� ��
            if (this.vanish_timer < Block.VANISH_TIME / 2.0f)
            { // �Һٴ� ���� �ð��� ������ �����ٸ�
                color0.a = this.vanish_timer / (Block.VANISH_TIME / 2.0f); // ����(a)�� ����
                color1.a = color0.a;
                this.GetComponent<Renderer>().material = this.transparent_material;
            } // ������ ��Ƽ������ ����
            float rate = 1.0f - this.vanish_timer / Block.VANISH_TIME; // vanish_timer�� �پ����� 1�� �������
            this.GetComponent<Renderer>().material.color = Color.Lerp(color0, color1, rate); // ������ ���� �ٲ�
        }


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
    public Block.DIR4 calcSlideDir(Vector2 mouse_position) { // ���콺 ��ġ�� �������� �����̵�� ������ ����
        Block.DIR4 dir = Block.DIR4.NONE;
        Vector2 v = mouse_position - new Vector2(this.transform.position.x, this.transform.position.y); // ������ mouse_positio�� ���� ��ġ�� ����
        if (v.magnitude > 0.1f)
        { // ������ ũ�Ⱑ 0.1���� ������ �����̵� ���� ���� �ɷ� ����
            if (v.y > v.x)
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.UP;
                }
                else { dir = Block.DIR4.LEFT; }
            }
            else
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.RIGHT;
                }
                else { dir = Block.DIR4.DOWN; }
            }
        }
        return (dir);
    }
    public float calcDirOffset(Vector2 position, Block.DIR4 dir)
    { // ���� ��ġ�� �����̵��� ���� �Ÿ��� ��� �����ΰ� ��ȯ
        float offset = 0.0f;
        Vector2 v = position - new Vector2(this.transform.position.x, this.transform.position.y); // ������ ��ġ�� ����� ���� ��ġ�� ���� ��Ÿ���� ����
        switch (dir)
        { // ������ ���⿡ ���� ������
            case Block.DIR4.RIGHT: offset = v.x; break;
            case Block.DIR4.LEFT: offset = -v.x; break;
            case Block.DIR4.UP: offset = v.y; break;
            case Block.DIR4.DOWN: offset = -v.y; break;
        }
        return (offset);
    }
    public void beginSlide(Vector3 offset)
    { // �̵� ������ �˸��� �޼���
        this.position_offset_initial = offset;
        this.position_offset = this.position_offset_initial;
        this.next_step = Block.STEP.SLIDE;
    } // ���¸� SLIDE�� ����
    public void toVanishing()
    {
        // ������� ������ �ɸ��� �ð����� ���������� ����
        this.vanish_timer = Block.VANISH_TIME;
    }
    public bool isVanishing()
    {
        // vanish_timer�� 0���� ũ�� true
        bool is_vanishing = (this.vanish_timer > 0.0f);
        return (is_vanishing);
    }
    public void rewindVanishTimer()
    {
        // '����� ������ �ɸ��� �ð�'�� ���������� ����
        this.vanish_timer = Block.VANISH_TIME;
    }
    public bool isVisible()
    {
        // �׸��� ����(renderer.enabled�� true) ���¶�� ǥ��
        bool is_visible = this.GetComponent<Renderer>().enabled;
        return (is_visible);
    }
    public void setVisible(bool is_visible)
    {
        // �׸��� ���� ������ �μ��� ����
        this.GetComponent<Renderer>().enabled = is_visible;
    }
    public bool isIdle()
    {
        bool is_idle = false;
        // ���� ��� ���°� '��� ��'�̰�, ���� ��� ���°� '����'�̸�
        if (this.step == Block.STEP.IDLE && this.next_step == Block.STEP.NONE)
        {
            is_idle = true;
        }
        return (is_idle);
    }

}


