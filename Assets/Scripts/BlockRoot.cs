using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRoot : MonoBehaviour
{ // 블록을 가로세로 바둑판(grid) 모양으로 관리
    public GameObject BlockPrefab = null; // 만들어낼 블록의 프리팹
    public BlockControl[,] blocks; // 그리드
                                   // 블록을 만들어 내고 가로 9칸, 세로 9칸에 배치
    private GameObject main_camera = null; // 메인 카메라
    private BlockControl grabbed_block = null; // 잡은 블록
    void Start()
    {
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");
    } // 카메라로부터 마우스 커서를 통과하는 광선을 쏘기 위해서 필요

    public void initialSetUp()
    {
        this.blocks = new BlockControl[Block.BLOCK_NUM_X, Block.BLOCK_NUM_Y]; // 그리드의 크기를 9×9로
        int color_index = 0; // 블록의 색 번호
        for (int y = 0; y < Block.BLOCK_NUM_Y; y++)
        { // 처음~마지막행
            for (int x = 0; x < Block.BLOCK_NUM_X; x++)
            { // 왼쪽~오른쪽
              // BlockPrefab의 인스턴스를 씬에 만든다.
                GameObject game_object = Instantiate(this.BlockPrefab) as GameObject;
                BlockControl block = game_object.GetComponent<BlockControl>(); // 블록의 BlockControl 클래스를 가져옴
                this.blocks[x, y] = block; // 블록을 그리드에 저장
                block.i_pos.x = x; // 블록의 위치 정보(그리드 좌표)를 설정
                block.i_pos.y = y;
                block.block_root = this; // 각 BlockControl이 연계할 GameRoot는 자신이라고 설정
                Vector3 position = BlockRoot.calcBlockPosition(block.i_pos); // 그리드 좌표를 실제 위치(scene의 좌표)로 변환
                block.transform.position = position; // 씬의 블록 위치를 이동
                block.setColor((Block.COLOR)color_index); // 블록의 색을 변경
                                                          // 블록의 이름을 설정(후술)한다. 나중에 블록 정보 확인때 필요
                block.name = "block(" + block.i_pos.x.ToString() + "," + block.i_pos.y.ToString() + ")";
                // 전체 색 중에서 임의로 하나의 색을 선택
                color_index = Random.Range(0, (int)Block.COLOR.NORMAL_COLOR_NUM);
            }
        }
    }
    // 지정된 그리드 좌표로 씬에서의 좌표를 구함
    public static Vector3 calcBlockPosition(Block.iPosition i_pos)
    {
        // 배치할 왼쪽 위 구석 위치를 초기값으로 설정
        Vector3 position = new Vector3(-(Block.BLOCK_NUM_X / 2.0f - 0.5f), -(Block.BLOCK_NUM_Y / 2.0f - 0.5f), 0.0f);
        // 초깃값 + 그리드 좌표 × 블록 크기
        position.x += (float)i_pos.x * Block.COLLISION_SIZE;
        position.y += (float)i_pos.y * Block.COLLISION_SIZE;
        return (position); // 씬에서의 좌표를 반환
    }
    void Update()
    { // 마우스 좌표와 겹치는지 체크, 잡을 수 있는 상태의 블록을 잡음
        Vector3 mouse_position; // 마우스 위치
        this.unprojectMousePosition(out mouse_position, Input.mousePosition); // 마우스 위치를 가져옴
        Vector2 mouse_position_xy = new Vector2(mouse_position.x, mouse_position.y); // 가져온 마우스 위치를 하나의 Vector2로 모음
        if (this.grabbed_block == null)
        { // 잡은 블록이 비었으면
          // if (!this.is_has_falling_block()) { // 나중에 주석 해제
            if (Input.GetMouseButtonDown(0))
            { // 마우스 버튼이 눌렸으면
                foreach (BlockControl block in this.blocks)
                { // blocks 배열의 모든 요소를 차례로 처리
                    if (!block.isGrabbable())
                    { // 블록을 잡을 수 없다면
                        continue;
                    } // 루프의 처음으로 점프
                    if (!block.isContainedPosition(mouse_position_xy))
                    { // 마우스 위치가 블록 영역 안이 아니면
                        continue;
                    } // 루프의 처음으로 점프
                    this.grabbed_block = block; // 처리 중인 블록을 grabbed_block에 등록
                    this.grabbed_block.beginGrab(); break;
                } // 잡았을 때의 처리를 실행
            } // }
        }
        else
        { // 블록을 잡았을 때
            if (!Input.GetMouseButton(0))
            { // 마우스 버튼이 눌려져 있지 않으면
                this.grabbed_block.endGrab(); // 블록을 놨을 때의 처리를 실행
                this.grabbed_block = null;
            } // grabbed_block을 비우게 설.
        }
    }
    public bool unprojectMousePosition(out Vector3 world_position, Vector3 mouse_position)
    {
        bool ret;
        // 블록 앞에 카메라를 향하는 판(plane)을 생성
        Plane plane = new Plane(Vector3.back, new Vector3(0.0f, 0.0f, -Block.COLLISION_SIZE / 2.0f));
        // 카메라와 마우스를 통과하는 빛을 생성
        Ray ray = this.main_camera.GetComponent<Camera>().ScreenPointToRay(mouse_position);
        float depth;
        // 광선(ray)이 판(plane)에 닿았다면
        if (plane.Raycast(ray, out depth))
        { // depth 정보를 기록하고
            world_position = ray.origin + ray.direction * depth; // 마우스 위치(3D)를 기록
            ret = true;
        }
        else
        { // 닿지 않았다면
            world_position = Vector3.zero; // 마우스 위치를 0으로 기록
            ret = false;
        }
        return (ret); // 카메라를 통과하는 광선이 블록에 닿았는지를 반환
    }
}

