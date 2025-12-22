using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CScript : CharaScript
{
    [SerializeField, Header("ツタPrefab")]
    GameObject ivyPrefab;

    public struct Ivy
    {
        GameObject ivyObj;
        Vector2 ivyPos;

        public Ivy(GameObject ivyObj,Vector2 ivyPos)
        {
            this.ivyObj = ivyObj;
            this.ivyPos = ivyPos;
        }

        public void DestroySelf()
        {
            Destroy(this.ivyObj); 
        }

        public Vector2 GetPos()
        {
            return this.ivyPos;
        }
    }
    
    List<Ivy> ivyList = new List<Ivy>();

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

            if (gridManager.CheckCellState(z, x) == CellScript.CellState.empty && !gridManager.CheckHaveDamage(z, x))
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

        Vector3 instPos = new Vector3(movePos.x, ivyPrefab.transform.position.y, movePos.z);
        var ivy= Instantiate(ivyPrefab, instPos, ivyPrefab.transform.rotation);

        Ivy newIvy = new Ivy(ivy, new Vector2((int)movePos.x, (int)movePos.z));
        ivyList.Add(newIvy);

        gridManager.SetDamageState((int)movePos.z, (int)movePos.x, ivy);

        yield return null;

        DeleteImage();
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

        for (int i = hp; i < hpBar.Count; i++)
        {
            hpBar[i].gameObject.SetActive(false);
        }       

        //HPが0になったら死亡
        if (hp <= 0)
        {
            StartCoroutine(Dead());
            turnManager.enemyList.Remove(this);

            foreach(var ivy in ivyList)
            {
                ivy.DestroySelf();
                Vector2 ivyPos = ivy.GetPos();
                gridManager.DeleteDamageState((int)ivyPos.y, (int)ivyPos.x);
            }
        }
    }
}
