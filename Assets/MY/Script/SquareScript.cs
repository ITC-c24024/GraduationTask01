using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareScript : MonoBehaviour
{
    [SerializeField] GridManager gridManager;

    [SerializeField, Header("選択可能マスイメージ")]
    GameObject[] squares;
    [SerializeField, Header("マス選択中イメージ")]
    GameObject selectObj;

    float squarePosY;

    void Start()
    {
        squarePosY = squares[0].transform.position.y;
    }

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
            if (posX < 0 || 7 < posX) continue;
            var isEnemy = gridManager.CheckCellState((int)playerPos.z, (int)posX) == CellScript.CellState.enemy;
            if (i != 0 && !isEnemy)
            {
                squares[n].transform.position = new Vector3(playerPos.x + i*2/3f, squarePosY, playerPos.z);
                squares[n].transform.localEulerAngles = new Vector3(
                    squares[n].transform.localEulerAngles.x,
                    GetAngle(i, 0),
                    squares[n].transform.localEulerAngles.z
                    );
                squares[n].SetActive(true);
                n++;
            }
        }
        for (int i = -1; i <= 1; i++)
        {            
            float posZ = playerPos.z + i;
            if (posZ < 0 || 7 < posZ) continue;
            var isEnemy = gridManager.CheckCellState((int)posZ, (int)playerPos.x) == CellScript.CellState.enemy;
            if (i != 0 && !isEnemy)
            {
                squares[n].transform.position = new Vector3(playerPos.x, squarePosY, playerPos.z + i * 2 / 3f);
                squares[n].transform.localEulerAngles = new Vector3(
                    squares[n].transform.localEulerAngles.x,
                    GetAngle(0, i),
                    squares[n].transform.localEulerAngles.z
                    );
                squares[n].SetActive(true);
                n++;
            }
        }
    }

    /// <summary>
    /// Imageの向きを計算
    /// </summary>
    /// <param name="direction">選んだマスの方向</param>
    /// <returns>Imageの向き</returns>
    int GetAngle(int x,int y)
    {
        int angle = 0;
        if (x > 0) angle = 90;
        else if (x < 0) angle = -90;
        else if (y > 0) angle = 0;
        else if (y < 0) angle = 180;

        return angle;
    }

    /// <summary>
    ///選択中のマスをハイライト 
    /// </summary>
    /// <param name="playerPos">プレイヤーの位置</param>
    /// <param name="direction">playerPosから見た選択するマスの方向</param>
    public void SelectImage(Vector3 playerPos, Vector2 direction)
    {
        Vector3 selectPos = new Vector3(
            playerPos.x + direction.x*2/3,
            selectObj.transform.position.y,
            playerPos.z + direction.y*2/3
            );

        selectObj.transform.position = selectPos;

        int angle = 0;
        if (direction.x > 0) angle = 90;
        else if (direction.x < 0) angle = -90;
        else if (direction.y > 0) angle = 0;
        else if (direction.y < 0) angle = 180;

        selectObj.transform.localEulerAngles = new Vector3(
            selectObj.transform.localEulerAngles.x, 
            angle, 
            selectObj.transform.localEulerAngles.z
            );

        selectObj.SetActive(true);
    }
}
