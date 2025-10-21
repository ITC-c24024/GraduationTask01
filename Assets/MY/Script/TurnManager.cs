using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] PlayerController playerCon;
    //��
    [SerializeField] CharaScript enemySC;
    //��
    GridManager gridManager;
    //��
    
    [SerializeField] CellScript cellScript;

    //��
    [SerializeField, Header("�GPrefab")]
    GameObject enemyPrefab;

    //���s���R���[�`��
    Coroutine runnning = null;

    void Start()
    {
        //��
        gridManager = gameObject.GetComponent<GridManager>();
        EnemySpown();
        StartCoroutine(TurnStart());
    }

    //��
    void EnemySpown()
    {
        GameObject enemy = Instantiate(
           enemyPrefab,
           new Vector3(7, 0.5f, 7),
           enemyPrefab.transform.rotation
           );

        enemySC = enemy.GetComponent<CharaScript>();
        enemySC.turnManager = this;
        enemySC.gridManager = gridManager;
        enemySC.cellScript = cellScript;

    }

    /// <summary>
    /// �R���[�`���I��
    /// </summary>
    public void FinCoroutine()
    {
        runnning = null;
    }

    /// <summary>
    /// �^�[���i�s�Ǘ�
    /// </summary>
    /// <returns></returns>
    IEnumerator TurnStart()
    {
        //�s���I��
        runnning = StartCoroutine(playerCon.SelectAction());

        while (runnning != null) yield return null;

        //���s�^�[��
        for (int i = 0; i < playerCon.actionLimit; i++)
        {
            //��s���G

            //�v���C���[���s
            runnning = StartCoroutine(playerCon.ExecutionAct());
            //�����s���G
            if (i < enemySC.actionLimit) StartCoroutine(enemySC.Move());

            while (runnning != null) yield return null;

            //��s���G
        }
    }

    public Vector3 GetPlayerPos()
    {
        return playerCon.playerPos;
    }
}
