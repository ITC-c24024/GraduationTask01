using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CharaScript : MonoBehaviour
{
    public TurnManager turnManager;
    public GridManager gridManager;
    public CellScript cellScript;

    public GameObject charaImage;
    public GameObject attackImage;
    public GameObject arrowImage;
    public Slider hpSlider;
    public Camera worldCamera;
    public Canvas canvas;

    //HP
    public int hp = 100;
    //攻撃力
    public int damage = 1;
    //被ダメージ
    public int recieveDamage = 0;

    //行動可能回数
    public int actionLimit = 1;

    //プレイヤーとの最短距離
    public float shortDir = 0;
    //移動法則
    public Direction[] moveRule;

    //敵が攻撃だけ行う判定
    public bool attackOnly = false;
    //移動しようとして戻る判定
    public bool goBack = false;

    //移動方向
    public Vector2 targetDir;

    //現在位置
    public Vector3 curPos;
    //移動予定位置
    public Vector3 movePos;

    public Animator animator;

    void Start()
    {
        hpSlider.transform.SetAsFirstSibling();
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
        curPos = transform.position;
    }

    void Update()
    {
        //HPバー追従
        Vector2 viewPos = worldCamera.WorldToViewportPoint(transform.position);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 charaScreenPos = new Vector2(
            canvasRect.sizeDelta.x * viewPos.x - (canvasRect.sizeDelta.x * 0.5f),
            canvasRect.sizeDelta.y * viewPos.y - (canvasRect.sizeDelta.y * 0.5f)
            );

        if (hpSlider != null)
        {
            hpSlider.transform.localPosition = charaScreenPos;
            hpSlider.transform.localScale = Vector2.one;
        }       
    }

    /// <summary>
    /// 進めるかの判定
    /// </summary>
    /// <param name="x">進みたいマスのx座標</param>
    /// <param name="z">進みたいマスのz座標</param>
    /// <returns></returns>
    public bool CanMove(Vector3 targetPos)
    {
        //ステージ範囲内か調べる
        float posX = targetPos.x;
        float posZ = targetPos.z;
        if (posX < 0 || 7 < posX) return false;
        else if (posZ < 0 || 7 < posZ) return false;
        //ほかの敵がいないか調べる
        else if (gridManager.CheckCellState((int)posZ, (int)posX) == CellScript.CellState.enemy) return false;
        else return true;
    }

    /// <summary>
    /// 2点間距離を計算
    /// </summary>
    /// <param name="targetPos">目標位置</param>
    /// <param name="startPos">スタート位置</param>
    /// <returns>結果の距離を返す</returns>
    float GetDistance(Vector3 targetPos, Vector3 startPos)
    {
        float distance = Mathf.Sqrt(
            Mathf.Pow(targetPos.x - startPos.x, 2) + Mathf.Pow(targetPos.z - startPos.z, 2)
            );
        return distance;
    }

    /// <summary>
    /// プレイヤーとの最短距離を設定
    /// </summary>
    public void SetShortDir()
    {
        //プレイヤーの位置を取得
        Vector3[] playerPos = turnManager.GetPlayerPos();

        //プレイヤーとの距離を計算
        float diff_A = GetDistance(playerPos[0], curPos);
        float diff_B = GetDistance(playerPos[1], curPos);

        if (diff_A <= diff_B)
        {
            shortDir = diff_A;
        }
        else
        {
            shortDir = diff_B;
        }
    }

    /// <summary>
    /// どのプレイヤーのを追うか決める
    /// </summary>
    /// <returns>追うプレイヤーの位置</returns>
    public Vector3 SelectPlayer()
    {
        //プレイヤーの位置を取得
        Vector3[] playerPos = turnManager.GetPlayerPos();

        //プレイヤーとの距離を計算
        float diff_A = GetDistance(playerPos[0], curPos);
        float diff_B = GetDistance(playerPos[1], curPos);

        //近い方を追う位置とする
        Vector3 targetPos;
        if (diff_A <= diff_B)
        {
            targetPos = playerPos[0];
        }
        else
        {
            targetPos = playerPos[1];
        }

        return targetPos;
    }

    /// <summary>
    /// プレイヤーの位置に行くための方向を決める
    /// </summary>
    /// <param name="playerPos">プレイヤーの位置</param>
    /// <param name="startPos">移動開始位置</param>
    /// <returns>進む方向</returns>
    public Vector2 GetDirection(Vector3 playerPos, Vector3 startPos, int ruleNum)
    {
        //進行方向
        Vector2 moveDir;

        //x座標、z座標の差を計算
        float diffX = playerPos.x - startPos.x;
        float diffZ = playerPos.z - startPos.z;

        //より離れてる軸に進む(Z軸優先)
        if (Mathf.Abs(diffX) <= Mathf.Abs(diffZ))
            moveDir = new Vector2(
                moveRule[ruleNum].x * Mathf.Sign(diffX),
                moveRule[ruleNum].z * Mathf.Sign(diffZ)
                );
        else
            moveDir = new Vector2(
                moveRule[ruleNum].z * Mathf.Sign(diffX),
                moveRule[ruleNum].x * Mathf.Sign(diffZ)
                );

        return moveDir;
    }

    /// <summary>
    /// 進む予定のマスを決める
    /// </summary>
    public void SetTargetPos()
    {
        //現在位置を移動開始位置とする
        Vector3 startPos = curPos;

        //プレイヤーの位置を見て、どのプレイヤーについていくか決める
        Vector3 playerPos = SelectPlayer();

        //移動開始位置からプレイヤーの位置に行くためにどの方向に行けばいいか決める
        targetDir = GetDirection(playerPos, startPos, 0);

        Vector3 originPos = curPos;

        //進む位置
        movePos = new Vector3(
            originPos.x + targetDir.x,
            originPos.y,
            originPos.z + targetDir.y
            );
        /*
        attackState.transform.localPosition = new Vector3(
            movePos.x, 
            attackState.transform.localPosition.y,
            movePos.z
            );
        */
    }

    /// <summary>
    /// 敵の攻撃予定地を表示
    /// </summary>
    public void AttackState()
    {
        //攻撃予定位置を表示
        attackImage.transform.localPosition = new Vector3(movePos.x, 0.101f, movePos.z);
        attackImage.SetActive(true);

        //矢印イメージを表示
        arrowImage.transform.localPosition= new Vector3(movePos.x - (targetDir.x / 2), 0.101f, movePos.z - (targetDir.y / 2));
        
        int angle = 0;
        if (targetDir.x > 0) angle = 90;
        else if (targetDir.x < 0) angle = -90;
        else if (targetDir.y > 0) angle = 0;
        else if (targetDir.y < 0) angle = 180;
        arrowImage.transform.localEulerAngles = new Vector3(arrowImage.transform.localEulerAngles.x, angle, arrowImage.transform.localEulerAngles.z);
        arrowImage.SetActive(true);     
    }

    /// <summary>
    /// 敵キャラを移動
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Move()
    {
        yield return null;
    }

    /// <summary>
    /// 進めないとき、移動しようとして元の位置に戻る
    /// </summary>
    /// <returns></returns>
    public IEnumerator Back(Vector3 originPos)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = originPos;

        float time = 0;
        float required = 0.1f;
        while (time < required)
        {
            time += Time.deltaTime;

            //現在地を計算
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, time / required);

            //キャラを移動
            transform.position = currentPos;

            yield return null;
        }
        transform.position = targetPos;
        curPos = targetPos;

        //マスの状態を敵自身にする
        gridManager.ChangeCellState(
                (int)curPos.z,
                (int)curPos.x,
                CellScript.CellState.enemy,
                this,
                new Vector2Int(0, 0)
                );
    }

    /// <summary>
    /// 被弾処理
    /// </summary>
    /// <param name="amount">被ダメージ</param>
    public virtual void ReciveDamage(int amount, Vector2 kbDir)
    {
        
    }

    /// <summary>
    /// 死んだときの演出
    /// </summary>
    /// <returns></returns>
    public IEnumerator Dead()
    {
        Debug.Log("倒した");
        //プレイヤーなら非表示
        if (gameObject.CompareTag("Player")) hpSlider.gameObject.SetActive(false);
        //敵ならDestroy
        else Destroy(hpSlider.gameObject);

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float time = 0;
        float required = 0.5f;
        while (time < required)
        {
            time += Time.deltaTime;

            //現在のスケールを計算
            Vector3 currentScale = Vector3.Lerp(startScale, targetScale, time / required);

            //キャラを縮小
            transform.localScale = currentScale;

            yield return null;
        }

        gridManager.LeaveCell((int)curPos.z, (int)curPos.x, this);

        //プレイヤーなら非表示
        if (gameObject.CompareTag("Player")) gameObject.SetActive(false);
        //敵ならDestroy
        else Destroy(gameObject);
    }
}

[Serializable]
public class Direction
{
    public int x;
    public int z;
}
