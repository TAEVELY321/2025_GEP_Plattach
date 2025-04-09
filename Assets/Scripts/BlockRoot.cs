using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{ // ����� ���μ��� �ٵ���(grid) ������� ����
    public GameObject BlockPrefab = null; // ���� ����� ������
    public BlockControl[,] blocks; // �׸���
                                   // ����� ����� ���� ���� 9ĭ, ���� 9ĭ�� ��ġ
    private GameObject main_camera = null; // ���� ī�޶�
    private BlockControl grabbed_block = null; // ���� ���
    void Start()
    {
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
    } // ī�޶�κ��� ���콺 Ŀ���� ����ϴ� ������ ��� ���ؼ� �ʿ�

    public void initialSetUp()
    {
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y]; // �׸����� ũ�⸦ 9��9��
        int color_index = 0; // ����� �� ��ȣ
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
                block.setColor((Block.COLOR)color_index); // ����� ���� ����
                                                          // ����� �̸��� ����(�ļ�)�Ѵ�. ���߿� ��� ���� Ȯ�ζ� �ʿ�
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
          // if (!this.is_has_falling_block()) { // ���߿� �ּ� ����
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
            } // }
        }
        else
        { // ����� ����� ��
            if (!Input.GetMouseButton(0))
            { // ���콺 ��ư�� ������ ���� ������
                this.grabbed_block.endGrab(); // ����� ���� ���� ó���� ����
                this.grabbed_block = null;
            } // grabbed_block�� ���� ��.
        }
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
}

