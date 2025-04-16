using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{ // ����� ���μ��� �ٵ���(grid) ������� ����
    public GameObject BlockPrefab = null; // ���� ����� ������
    public BlockControl[,] blocks; // �׸���
                                   // ����� ����� ���� ���� 9ĭ, ���� 9ĭ�� ��ġ
    private ScoreCounter score_counter = null; // ���� ī���� ScoreCounter
    private GameObject main_camera = null; // ���� ī�޶�
    private BlockControl grabbed_block = null; // ���� ���
    protected bool is_vanishing_prev = false; // �տ��� ��ȭ�ߴ°�
    public TextAsset levelData = null; // ���� �������� �ؽ�Ʈ�� ����
    public LevelControl level_control; // LevelControl�� ����
    void Start()
    {
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
        this.score_counter = this.gameObject.GetComponent<ScoreCounter>();

    } // ī�޶�κ��� ���콺 Ŀ���� ����ϴ� ������ ��� ���ؼ� �ʿ�

    public void initialSetUp()
    {
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y]; // �׸����� ũ�⸦ 9��9��
        int color_index = 0; // ����� �� ��ȣ
        Block.COLOR color = Block.COLOR.FIRST;
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        { // ó��~��������
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            { // ����~������
              // BlockPrefab�� �ν��Ͻ��� ���� �����.
                GameObject game_object = Instantiate(this.BlockPrefab) as GameObject;
                BlockControl block = game_object.GetComponent<BlockControl>(); // ����� BlockControl Ŭ������ ������
                this.blocks[x, y] = block; // ����� �׸��忡 ����
                block.i_pos.x = x; // ����� ��ġ ����(�׸��� ��ǥ)�� ����
                block.i_pos.y = y;
                block.block_root = this; // �� BlockControl�� ������ GameRoot�� �ڽ��̶�� ����
                Vector3 position = BlockRoot.calcBlockPosition(block.i_pos); // �׸��� ��ǥ�� ���� ��ġ(scene�� ��ǥ)�� ��ȯ
                block.transform.position = position; // ���� ��� ��ġ�� �̵�
                //block.setColor((Block.COLOR)color_index); // ����� ���� ����
                                                          // ����� �̸��� ����(�ļ�)�Ѵ�. ���߿� ��� ���� Ȯ�ζ� �ʿ�
                color = this.selectBlockColor();
                block.setColor(color);
                block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";
                // ��ü �� �߿��� ���Ƿ� �ϳ��� ���� ����
                color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
            }
        }
    }
    // ������ �׸��� ��ǥ�� �������� ��ǥ�� ����
    public static Vector3 calcBlockPosition(Block.iPosition i_pos)
    {
        // ��ġ�� ���� �� ���� ��ġ�� �ʱⰪ���� ����
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);
        // �ʱ갪 + �׸��� ��ǥ �� ��� ũ��
        position.x += (float)i_pos.x * Block.COLLISION_SIZE;
        position.y += (float)i_pos.y * Block.COLLISION_SIZE;
        return (position); // �������� ��ǥ�� ��ȯ
    }
    void Update()
    { // ���콺 ��ǥ�� ��ġ���� üũ, ���� �� �ִ� ������ ����� ����
        Vector3 mouse_position; // ���콺 ��ġ
        this.unprojectMousePosition(out mouse_position, Input.mousePosition); // ���콺 ��ġ�� ������
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y); // ������ ���콺 ��ġ�� �ϳ��� Vector2�� ����
        if (this.grabbed_block == null)
        { // ���� ����� �������
            if (!this.is_has_falling_block()) { // ���߿� �ּ� ����
                if (Input.GetMouseButtonDown(0))
                { // ���콺 ��ư�� ��������
                    foreach (BlockControl block in this.blocks)
                    { // blocks �迭�� ��� ��Ҹ� ���ʷ� ó��
                        if (!block.isGrabbable())
                        { // ����� ���� �� ���ٸ�
                            continue;
                        } // ������ ó������ ����
                        if (!block.isContainedPosition(mouse_position_xy))
                        { // ���콺 ��ġ�� ��� ���� ���� �ƴϸ�
                            continue;
                        } // ������ ó������ ����
                        this.grabbed_block = block; // ó�� ���� ����� grabbed_block�� ���
                        this.grabbed_block.beginGrab(); break;
                    } // ����� ���� ó���� ����
                } 
            }
        }
        else
        { // ����� ����� ��
            do
            {
                BlockControl swap_target = this.getNextBlock(grabbed_block, grabbed_block.slide_dir); // �����̵��� ���� ����� ������
                if (swap_target == null)
                { // �����̵��� �� ����� ��� ������
                    break;
                } // ���� Ż��
                if (!swap_target.isGrabbable())
                { // �����̵��� ���� ����� ���� �� �ִ� ���°� �ƴ϶��
                    break;
                } // ���� Ż��
                  // ���� ��ġ���� �����̵� ��ġ������ �Ÿ��� ����
                float offset = this.grabbed_block.calcDirOffset(mouse_position_xy, this.grabbed_block.slide_dir);
                if (offset < Block.COLLISION_SIZE / 2.0f)
                { // ���� �Ÿ��� ��� ũ���� ���ݺ��� �۴ٸ�
                    break;
                } // ���� Ż��
                this.swapBlock(grabbed_block, grabbed_block.slide_dir, swap_target); // ����� ��ü
                this.grabbed_block = null; // ������ ����� ��� ���� ����
            } while (false);
            if (!Input.GetMouseButton(0))
            { // ���콺 ��ư�� ������ ���� ������
                this.grabbed_block.endGrab(); // ����� ���� ���� ó���� ����
                this.grabbed_block = null;
            } // grabbed_block�� ���� ��.
        }
        // ���� �� �Ǵ� �����̵� ���̸�
        if (this.is_has_falling_block() || this.is_has_sliding_block())
        {
            // �ƹ��͵� ���� �ʴ´�
            // ���� �ߵ� �����̵� �ߵ� �ƴϸ�
        }
        else
        {
            int ignite_count = 0; // �Һ��� ����
                                  // �׸��� ���� ��� ��Ͽ� ���ؼ� ó��
            foreach (BlockControl block in this.blocks)
            {
                if (!block.isIdle())
                { // ��� ���̸� ������ ó������ �����ϰ�
                    continue; // ���� ����� ó��
                }
                // ���� �Ǵ� ���ο� ���� �� ����� �� �� �̻� �����ߴٸ�
                if (this.checkConnection(block))
                {
                    ignite_count++; // �Һ��� ������ ����
                }
            }
            if (ignite_count > 0)
            { // �Һ��� ������ 0���� ũ�� �� �� ������ ������ ���� ����
                if (!this.is_vanishing_prev)
                {
                    this.score_counter.clearIgniteCount(); // ���� ��ȭ�� �ƴ϶��, ��ȭ Ƚ���� ����
                }
                this.score_counter.addIgniteCount(ignite_count);// ��ȭ Ƚ���� ����
                this.score_counter.updateTotalScore(); // �հ� ���� ����
                int block_count = 0; // �Һٴ� ���� ��� ��(���� �忡�� ���)
                                     // �׸��� ���� ��� ��Ͽ� ���ؼ� ó��
                foreach (BlockControl block in this.blocks)
                {
                    if (block.isVanishing())
                    { // Ÿ�� ���̸�
                        block.rewindVanishTimer(); // �ٽ� ��ȭ!
                        block_count++; // ��ȭ ���� ��� ������ ����
                    }
                }
            }
        }
        bool is_vanishing = this.is_has_vanishing_block(); // �ϳ��� ���� ���� ����� �ִ°�?.
        do
        {
            if (is_vanishing) { break; } // ���� ���� ����� �ִٸ�, ���� ó���� �������� �ʴ´�.
            if (this.is_has_sliding_block()) { break; } // ��ü ���� ����� �ִٸ�, ���� ó���� �������� �ʴ´�.
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            {
                if (this.is_has_sliding_block_in_column(x))
                { // ���� ��ü ���� ����� �ִٸ� �� ���� ó������ �ʰ� ���� ���� ����.
                    continue;
                }
                for (int y = 0; y < Block.BLOCK_NUM_Y - 1; y++)
                {// �� ���� �ִ� ����� ���������� �˻��Ѵ�.
                    if (!this.blocks[x, y].isVacant())
                    { // ���� ����� ��ǥ�ö�� ���� �������.
                        continue;
                    }
                    for (int y1 = y + 1; y1 < Block.BLOCK_NUM_Y; y1++)
                    { // ���� ��� �Ʒ��� �ִ� ����� �˻�.
                        if (this.blocks[x, y1].isVacant()) { continue; } // �Ʒ��� �ִ� ����� ��ǥ�ö�� ���� �������.
                        this.fallBlock(this.blocks[x, y], Block.DIR4.UP, this.blocks[x, y1]); // ����� ��ü�Ѵ�.
                        break;
                    }
                }
            }
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            { // ����ó��.
                int fall_start_y = Block.BLOCK_NUM_Y;
                for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
                {
                    if (!this.blocks[x, y].isVacant()) { continue; } // ��ǥ�� ����� �ƴ϶�� ���� �������.
                    this.blocks[x, y].beginRespawn(fall_start_y); // ��� ��Ȱ.
                    fall_start_y++;
                }
            }
        } while (false);
        this.is_vanishing_prev = is_vanishing;

    }
    public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)
    {
        bool ret;
        // ��� �տ� ī�޶� ���ϴ� ��(plane)�� ����
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
        // ī�޶�� ���콺�� ����ϴ� ���� ����
        Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(mouse_position);
        float depth;
        // ����(ray)�� ��(plane)�� ��Ҵٸ�
        if (plane.Raycast(ray, out depth))
        { // depth ������ ����ϰ�
            world_position = ray.origin + ray.direction * depth; // ���콺 ��ġ(3D)�� ���
            ret = true;
        }
        else
        { // ���� �ʾҴٸ�
            world_position = Vector3.zero; // ���콺 ��ġ�� 0���� ���
            ret = false;
        }
        return (ret); // ī�޶� ����ϴ� ������ ��Ͽ� ��Ҵ����� ��ȯ
    }
    public BlockControl getNextBlock(BlockControl block, Block.DIR4 dir)
    { // �μ��� ������ ��ϰ� �������� ����� �����̵��� ���� ��� ����� �ִ��� ��ȯ
        BlockControl next_block = null; // �����̵��� ���� ����� ���⿡ ����
        switch (dir)
        {
            case Block.DIR4.RIGHT:
                if (block.i_pos.x < Block.BLOCK_NUM_X - 1)
                { // �׸��� ���̶��
                    next_block = this.blocks[block.i_pos.x + 1, block.i_pos.y];
                }
                break;
            case Block.DIR4.LEFT:
                if (block.i_pos.x > 0)
                { // �׸��� ���̶��
                    next_block = this.blocks[block.i_pos.x - 1, block.i_pos.y];
                }
                break;
            case Block.DIR4.UP:
                if (block.i_pos.y < Block.BLOCK_NUM_Y - 1)
                { // �׸��� ���̶��
                    next_block = this.blocks[block.i_pos.x, block.i_pos.y + 1];
                }
                break;
            case Block.DIR4.DOWN:
                if (block.i_pos.y > 0)
                { // �׸��� ���̶��
                    next_block = this.blocks[block.i_pos.x, block.i_pos.y - 1];
                }
                break;
        }
        return (next_block);
    }
    public static Vector3 getDirVector(Block.DIR4 dir)
    { // �μ��� ������ �������� ���� ��Ͽ��� ���� �������� �̵��ϴ� �� ��ȯ
        Vector3 v = Vector3.zero;
        switch (dir)
        {
            case Block.DIR4.RIGHT: v = Vector3.right; break; // ���������� 1���� �̵�
            case Block.DIR4.LEFT: v = Vector3.left; break; // �������� 1���� �̵�
            case Block.DIR4.UP: v = Vector3.up; break; // ���� 1���� �̵�
            case Block.DIR4.DOWN: v = Vector3.down; break; // �Ʒ��� 1���� �̵�
        }
        v *= Block.COLLISION_SIZE; // ����� ũ�⸦ ����
        return (v);
    }
    public static Block.DIR4 getOppositDir(Block.DIR4 dir)
    { // ����� ���� ��ü�ϱ� ���� �μ��� ������ ������ �ݴ� ������ ��ȯ
        Block.DIR4 opposit = dir;
        switch (dir)
        {
            case Block.DIR4.RIGHT: opposit = Block.DIR4.LEFT; break;
            case Block.DIR4.LEFT: opposit = Block.DIR4.RIGHT; break;
            case Block.DIR4.UP: opposit = Block.DIR4.DOWN; break;
            case Block.DIR4.DOWN: opposit = Block.DIR4.UP; break;
        }
        return (opposit);
    }
    public void swapBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    { // ������ ����� ��ü
      // ������ ��� ���� ���
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;
        // ������ ����� Ȯ������ ���
        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;
        // ������ ����� '������� �ð�'�� ���
        float vanish_timer0 = block0.vanish_timer;
        float vanish_timer1 = block1.vanish_timer;
        // ������ ����� �̵��� ���� ����
        Vector3 offset0 = BlockRoot.getDirVector(dir);
        Vector3 offset1 = BlockRoot.getDirVector(BlockRoot.getOppositDir(dir));
        // ���� ��ü
        block0.setColor(color1);
        block1.setColor(color0);
        // Ȯ������ ��ü
        block0.transform.localScale = scale1;
        block1.transform.localScale = scale0;
        // '������� �ð�'�� ��ü
        block0.vanish_timer = vanish_timer1;
        block1.vanish_timer = vanish_timer0;
        block0.beginSlide(offset0); // ���� ��� �̵��� ����
        block1.beginSlide(offset1); // �̵��� ��ġ�� ��� �̵��� ����
    }
    // �μ��� ���� ����� �� ���� ��� �ȿ� ���� �� �ľ��ϴ� �޼���
    public bool checkConnection(BlockControl start)
    {
        bool ret = false;
        int normal_block_num = 0;
        if (!start.isVanishing())
        { // �μ��� ����� �Һ��� ������ �ƴϸ�
            normal_block_num = 1;
        }
        int rx = start.i_pos.x; // �׸��� ��ǥ�� ����� �д�
        int lx = start.i_pos.x;
        for (int x = lx - 1; x > 0; x--)
        { // ����� ������ �˻�
            BlockControl next_block = this.blocks[x, start.i_pos.y];
            if (next_block.color != start.color) { break; } // ���� �ٸ���, ������ ����������
            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) { break; } // ���� ���̸�, ������ ����������
            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) { break; } // �����̵� ���̸�, ������ ����������
            if (!next_block.isVanishing())
            { // �Һ��� ���°� �ƴϸ�
                normal_block_num++;
            } // �˻�� ī���͸� ����
            lx = x;
        }
        for (int x = rx + 1; x < Block.BLOCK_NUM_X; x++)
        { // ����� �������� �˻�
            BlockControl next_block = this.blocks[x, start.i_pos.y];
            if (next_block.color != start.color) { break; }
            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) { break; }
            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) { break; }
            if (!next_block.isVanishing()) { normal_block_num++; }
            rx = x;
        }
        do
        {
            if (rx - lx + 1 < 3) { break; } // ������ ����� �׸��� ��ȣ - ���� ����� �׸��� ��ȣ + �߾� ���(1)�� ���� ���� 3 �̸��̸�, ���� Ż��
            if (normal_block_num == 0) { break; } // �Һ��� ���� ����� �ϳ��� ������, ���� Ż��
            for (int x = lx; x < rx + 1; x++)
            { // ������ ���� �� ����� �Һ��� ���·�
                this.blocks[x, start.i_pos.y].toVanishing();
                ret = true;
            }
        } while (false);

        normal_block_num = 0;
        if (!start.isVanishing())
        {
            normal_block_num = 1;
        }
        int uy = start.i_pos.y;
        int dy = start.i_pos.y;
        for (int y = dy - 1; y > 0; y--)
        { // ����� ������ �˻�.
            BlockControl next_block = this.blocks[start.i_pos.x, y];
            if (next_block.color != start.color) { break; }
            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) { break; }
            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) { break; }
            if (!next_block.isVanishing()) { normal_block_num++; }
            dy = y;
        }
        for (int y = uy + 1; y < Block.BLOCK_NUM_Y; y++)
        { // ����� �Ʒ����� �˻�.
            BlockControl next_block = this.blocks[start.i_pos.x, y];
            if (next_block.color != start.color) { break; }
            if (next_block.step == Block.STEP.FALL || next_block.next_step == Block.STEP.FALL) { break; }
            if (next_block.step == Block.STEP.SLIDE || next_block.next_step == Block.STEP.SLIDE) { break; }
            if (!next_block.isVanishing()) { normal_block_num++; }
            uy = y;
        }
        do
        {
            if (uy - dy + 1 < 3) { break; }
            if (normal_block_num == 0) { break; }
            for (int y = dy; y < uy + 1; y++)
            {
                this.blocks[start.i_pos.x, y].toVanishing();
                ret = true;
            }
        } while (false);
        return (ret);
    }
    // �Һٴ� ���� ����� �ϳ��� ������ true�� ��ȯ�Ѵ�.
    private bool is_has_vanishing_block()
    {
        bool ret = false;
        foreach(BlockControl block in this.blocks) {
            if (block.vanish_timer > 0.0f)
            {
                ret = true;
                break;
            }
        }
        return(ret);
    }
    // �����̵� ���� ����� �ϳ��� ������ true�� ��ȯ�Ѵ�.
    private bool is_has_sliding_block()
    {
        bool ret = false;
        foreach (BlockControl block in this.blocks)
        {
            if(block.step == Block.STEP.SLIDE) {
                ret = true;
                break;
            }
        }
        return (ret);
    }
    // ���� ���� ����� �ϳ��� ������ true�� ��ȯ�Ѵ�.
    private bool is_has_falling_block()
    {
        bool ret = false;
        foreach (BlockControl block in this.blocks)
        {
            if(block.step == Block.STEP.FALL) {
                ret = true;
                break;
            }
        }
        return (ret);
    }
    public void fallBlock(BlockControl block0, Block.DIR4 dir, BlockControl block1)
    { // �������� �� ���Ʒ� ����� ��ü�Ѵ�.
      // block0�� block1�� ��, ũ��, ����� ������ �ɸ��� �ð�, ǥ��, ��ǥ��, ���¸� ���.
        Block.COLOR color0 = block0.color;
        Block.COLOR color1 = block1.color;
        Vector3 scale0 = block0.transform.localScale;
        Vector3 scale1 = block1.transform.localScale;
        float vanish_timer0 = block0.vanish_timer;
        float vanish_timer1 = block1.vanish_timer;
        bool visible0 = block0.isVisible();
        bool visible1 = block1.isVisible();
        Block.STEP step0 = block0.step;
        Block.STEP step1 = block1.step;
        // block0�� block1�� ���� �Ӽ��� ��ü�Ѵ�.
        block0.setColor(color1);
        block1.setColor(color0);
        block0.transform.localScale = scale1;
        block1.transform.localScale = scale0;
        block0.vanish_timer = vanish_timer1;
        block1.vanish_timer = vanish_timer0;
        block0.setVisible(visible1);
        block1.setVisible(visible0);
        block0.step = step1;
        block1.step = step0;
        block0.beginFall(block1);
    }
    private bool is_has_sliding_block_in_column(int x)
    { // ������ �׸��� ��ǥ�� ��(���� ��)�� �����̵� ���� ����� �ϳ��� ������, true�� ��ȯ�Ѵ�.
        bool ret = false;
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        {
            if (this.blocks[x, y].isSliding())
            { // �����̵� ���� ����� ������,
                ret = true; // true�� ��ȯ�Ѵ�.
                break;
            }
        }
        return (ret);
    }
    public void create()
    { // ���� �������� �ʱ�ȭ, �ε�, ���� �������� ����
        this.level_control = new LevelControl();
        this.level_control.initialize(); // ���� ������ �ʱ�ȭ
        this.level_control.loadLevelData(this.levelData); // ������ �б�
        this.level_control.selectLevel(); // ���� ����
    }
    public Block.COLOR selectBlockColor()
    { // ���� ������ ���� Ȯ���� �������� ���� �����ؼ� ��ȯ
        Block.COLOR color = Block.COLOR.FIRST;
        // �̹� ������ ���� �����͸� ������
        LevelData level_data = this.level_control.getCurrentLevelData();
        float rand = Random.Range(0.0f, 1.0f); // 0.0~1.0 ������ ����
        float sum = 0.0f; // ���� Ȯ���� �հ�
        int i = 0;
        // ����� ���� ��ü�� ó���ϴ� ����
        for (i = 0; i < level_data.probability.Length - 1; i++)
        {
            if (level_data.probability[i] == 0.0f)
            {
                continue; // ���� Ȯ���� 0�̸� ������ ó������ ����
            }
            sum += level_data.probability[i]; // ���� Ȯ���� ����
            if (rand < sum)
            { // �հ谡 �������� ������
                break; // ������ ��������
            }
        }
        color = (Block.COLOR)i; // i��° ���� ��ȯ
        return (color);
    }
}




