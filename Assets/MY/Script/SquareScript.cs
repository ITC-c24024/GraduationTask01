using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScript : MonoBehaviour
{
    [SerializeField, Header("�I���\�}�X�C���[�W")]
    GameObject[] squares;
    [SerializeField, Header("�}�X�I�𒆃C���[�W")]
    GameObject selectObj;

    float squarePosY;

    void Start()
    {
        squarePosY = squares[0].transform.position.y;
    }

    /// <summary>
    /// �}�X�C���[�W�����ׂĔ�\��
    /// </summary>
    public void DeleteSquare()
    {
        for (int i = -0; i < squares.Length; i++)
        {
            squares[i].SetActive(false);
        }
        DeleteSelect();
    }

    public void DeleteSelect()
    {
        selectObj.SetActive(false);
    }

    /// <summary>
    /// �I���\�}�X���n�C���C�g
    /// </summary>
    /// <param name="playerPos">�v���C���[�̈ʒu</param>
    public void SetImage(Vector3 playerPos)
    {
        DeleteSquare();

        int n = 0;

        //�l���̃}�X�����邩�m�F���A�C���[�W��z�u
        for (int i = -1; i <= 1; i++)
        {
            float posX = playerPos.x + i;
            if (0 <= posX && posX <= 7 && i != 0)
            {
                squares[n].transform.position = new Vector3(posX, squarePosY, playerPos.z);
                squares[n].SetActive(true);
                n++;
            }
        }
        for (int i = -1; i <= 1; i++)
        {            
            float posZ = playerPos.z + i;
            if (0 <= posZ && posZ <= 7 && i != 0)
            {
                squares[n].transform.position = new Vector3(playerPos.x, squarePosY, posZ);
                squares[n].SetActive(true);
                n++;
            }
        }
    }

    /// <summary>
    ///�I�𒆂̃}�X���n�C���C�g 
    /// </summary>
    /// <param name="playerPos">�v���C���[�̈ʒu</param>
    /// <param name="direction">playerPos���猩���I������}�X�̕���</param>
    public void SelectImage(Vector3 playerPos, Vector2 direction)
    {
        Vector3 selectPos = new Vector3(
            playerPos.x + direction.x,
            squarePosY,
            playerPos.z + direction.y
            );

        selectObj.transform.position = selectPos;

        selectObj.SetActive(true);
    }
}
