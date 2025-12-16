using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CScript : CharaScript
{
    [SerializeField, Header("ツタオブジェクト")]
    GameObject ivyObj;

    List<GameObject> ivyList = new List<GameObject>();

    void Start()
    {
        charaState = CharaState.enemy;
        curPos = transform.position;

        attackImage = Instantiate(attackImagePrefab);
    }


    /// <summary>
    /// ランダムなマスを攻撃予定位置にする
    /// </summary>
    public override void SetAction()
    {
        while (true)
        {
            int x = Random.Range(0, 8);
            int z = Random.Range(0, 8);

            if (gridManager.CheckCellState(z, x) == CellScript.CellState.empty && gridManager.CheckHaveDamage(z, x))
            {
                //攻撃予定位置
                movePos = new Vector3(x, transform.position.y, z);
                
                break;
            }
        }        

        AttackState();
    }

    /// <summary>
    /// 移動はしない
    /// ランダムな位置にダメージマス生成
    /// </summary>
    /// <returns></returns>
    public override IEnumerator Move()
    {
        animator.SetTrigger("IsAttack");
        shadowAnim.SetTrigger("IsAttack");

        Vector3 instPos = new Vector3(movePos.x, ivyObj.transform.position.y, movePos.z);
        var ivy= Instantiate(ivyObj, instPos, ivyObj.transform.rotation);
        ivyList.Add(ivy);
        
        gridManager.SetDamageState((int)movePos.z, (int)movePos.x);

        yield return null;

        DeleteImage();

        //turnManager.FinCoroutine();
    }

    public override void AttackState()
    {
        attackImage.transform.position= new Vector3(movePos.x, attackImage.transform.position.y, movePos.z);
        attackImage.SetActive(true);      
    }

    public override void ReciveDamage(int amount, Vector2 kbDir)
    {
        hp -= amount;
        if (hp < 0) hp = 0;

        for (int i = hp; i < hpBar.Length; i++)
        {
            hpBar[i].gameObject.SetActive(false);
        }       

        //HPが0になったら死亡
        if (hp <= 0)
        {
            StartCoroutine(Dead());
            turnManager.enemyList.Remove(this);

            for(int i = 0; i < ivyList.Count; i++)
            {
                Destroy(ivyList[i]);
            }
        }
    }
}
