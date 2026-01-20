using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_CScript : CharaScript
{
    [SerializeField, Header("ツタPrefab")]
    GameObject ivyPrefab;

    public class Ivy
    {
        GameObject ivyObj;
        Vector2 ivyPos;
        int turn; //出現後経過ターン数

        public Ivy(GameObject ivyObj, Vector2 ivyPos, int turn)
        {
            this.ivyObj = ivyObj;
            this.ivyPos = ivyPos;
            this.turn = turn;
        }

        public void ProceedTurn()
        {
            turn++;
        }

        public int GetTurn()
        {
            return turn;
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
        int i = 0;
        while (true)
        {
            if (i >= 64) break;
            i++;
            int x = Random.Range(0, 8);
            int z = Random.Range(0, 8);

            if (gridManager.CheckCellState(z, x) == CellScript.CellState.empty && !gridManager.CheckHaveDamage(z, x))
            {
                //攻撃予定位置
                movePos = new Vector3(x, transform.position.y, z);
                AttackState();
                break;
            }
        } 
    }

    /// <summary>
    /// 移動はしない
    /// ランダムな位置にダメージマス生成
    /// </summary>
    /// <returns></returns>
    public override IEnumerator Move()
    {
        CheckDestroyIvy();

        //ツタ上限3つ
        if (ivyList.Count < 3)
        {
            animator.SetTrigger("IsAttack");
            shadowAnim.SetTrigger("IsAttack");

            soundManager.Ivy();
            yield return new WaitForSecondsRealtime(0.2f);

            Vector3 instPos = new Vector3(movePos.x, ivyPrefab.transform.position.y, movePos.z);
            var ivy = Instantiate(ivyPrefab, instPos, ivyPrefab.transform.rotation);

            Ivy newIvy = new Ivy(ivy, new Vector2((int)movePos.x, (int)movePos.z), 0);
            ivyList.Add(newIvy);

            gridManager.SetDamageState((int)movePos.z, (int)movePos.x, ivy);

            yield return null;

            DeleteImage();
        }
    }

    public override void AttackState()
    {
        //ツタ上限3つ

        attackImage.transform.position = new Vector3(movePos.x, attackImage.transform.position.y, movePos.z);
        attackImage.SetActive(true);

    }

    /// <summary>
    /// ツタの時間経過での消滅ロジック
    /// </summary>
    void CheckDestroyIvy()
    {
        List<Ivy> remove = new List<Ivy>();

        //1ターン経過
        foreach (var ivy in ivyList)
        {
            ivy.ProceedTurn();

            //3ターン経過済みのツタを削除予定リストに追加
            if (ivy.GetTurn() >= 3)
            {
                remove.Add(ivy);    
            }
        }
        
        //ツタ削除
        foreach(var ivy in remove)
        {
            ivyList.Remove(ivy);

            ivy.DestroySelf();
            Vector2 ivyPos = ivy.GetPos();
            gridManager.DeleteDamageState((int)ivyPos.y, (int)ivyPos.x);
        }
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
