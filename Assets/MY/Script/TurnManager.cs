using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] PlayerController[] playerCon;
    //��
    [SerializeField] CharaScript enemySC;
    //��
    GridManager gridManager;
    
    [SerializeField] CellScript cellScript;

    //��
    [SerializeField, Header("�GPrefab")]
    GameObject enemyPrefab;

    //���s���R���[�`���̐�
    int runnning = 0;

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
        runnning--;
    }

    /// <summary>
    /// �^�[���i�s�Ǘ�
    /// </summary>
    /// <returns></returns>
    IEnumerator TurnStart()
    {
        //�s���I��
        StartCoroutine(playerCon[0].SelectAction());
        runnning++;
        StartCoroutine(playerCon[1].SelectAction());
        runnning++;

        while (runnning != 0) yield return null;

        playerCon[0].PosReset();
        playerCon[1].PosReset();

        //���s�^�[��
        for (int i = 0; i < playerCon[0].actionLimit; i++)
        {
            //��s���G

            //�v���C���[���s
            StartCoroutine(playerCon[0].ExecutionAct());
            runnning++;
            StartCoroutine(playerCon[1].ExecutionAct());
            runnning++;

            //�����s���G
            if (i < enemySC.actionLimit) StartCoroutine(enemySC.Move());

            while (runnning != 0) yield return null;

            //��s���G
        }
    }

    public Vector3[] GetPlayerPos()
    {
        Vector3[] playerPos = new Vector3[] { 
            playerCon[0].playerPos, 
            playerCon[1].playerPos 
        };
        return playerPos;
    }
}
