using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControl : MonoBehaviour
{
    private BlockRoot block_root = null;
    void Start()
    {
        // BlockRoot ��ũ��Ʈ�� ������
        this.block_root = this.gameObject.GetComponent<BlockRoot>();
        // BlockRoot ��ũ��Ʈ�� initialSetUp()�� ȣ��
        this.block_root.initialSetUp();
    }
}
