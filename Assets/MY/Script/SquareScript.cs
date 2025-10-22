using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScript : MonoBehaviour
{
    [SerializeField, Header("選択可能マスイメージ")]
    GameObject[] squares;
    [SerializeField, Header("マス選択中イメージ")]
    GameObject selectObj;

    /// <summary>
    /// マスイメージをすべて非表示
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
    /// 選択可能マスをハイライト
    /// </summary>
    /// <param name="playerPos">プレイヤーの位置</param>
    public void SetImage(Vector3 playerPos)
    {
        DeleteSquare();

        int n = 0;

        //四方のマスがあるか確認し、イメージを配置
        for (int i = -1; i <= 1; i++)
        {
            float posX = playerPos.x + i;
            if (0 <= posX && posX <= 7 && i != 0)
            {
                squares[n].transform.position = new Vector3(posX, 0.01f, playerPos.z);
                squares[n].SetActive(true);
                n++;
            }
        }
        for (int i = -1; i <= 1; i++)
        {            
            float posZ = playerPos.z + i;
            if (0 <= posZ && posZ <= 7 && i != 0)
            {
                squares[n].transform.position = new Vector3(playerPos.x, 0.01f, posZ);
                squares[n].SetActive(true);
                n++;
            }
        }
    }

    /// <summary>
    ///選択中のマスをハイライト 
    /// </summary>
    /// <param name="playerPos">プレイヤーの位置</param>
    /// <param name="direction">playerPosから見た選択するマスの方向</param>
    public void SelectImage(Vector3 playerPos, Vector2 direction)
    {
        Vector3 selectPos = new Vector3(
            playerPos.x + direction.x,
            0.01f,
            playerPos.z + direction.y
            );

        selectObj.transform.position = selectPos;

        selectObj.SetActive(true);
    }
}
