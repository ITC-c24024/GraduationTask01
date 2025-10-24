using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField, Header("�G��HP�o�[")]
    Slider hpPrefab;
    [SerializeField] Canvas canvas;

    //���s���R���[�`���̐�
    int runnning = 0;

    void Start()
    {
        //��
        Application.targetFrameRate = 30;
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

        Slider slider = Instantiate(hpPrefab);
        slider.transform.SetParent(canvas.transform);

        enemySC = enemy.GetComponent<CharaScript>();
        enemySC.turnManager = this;
        enemySC.gridManager = gridManager;
        enemySC.cellScript = cellScript;
        enemySC.hpSlider = slider;
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

        yield return new WaitForSeconds(0.5f);

        playerCon[0].PosReset();
        playerCon[1].PosReset();

        yield return new WaitForSeconds(1.0f);

        //���s�^�[��
        for (int i = 0; i < playerCon[0].actionLimit || i < playerCon[1].actionLimit; i++)
        {
            //��s���G

            while (runnning != 0) yield return null;

            yield return new WaitForSeconds(0.5f);

            //�v���C���[���s
            if(i < playerCon[0].actionLimit)
            {
                StartCoroutine(playerCon[0].ExecutionAct(i));
                runnning++;
            }
            if(i < playerCon[1].actionLimit)
            {
                StartCoroutine(playerCon[1].ExecutionAct(i));
                runnning++;
            }          

            while (runnning != 0) yield return null;

            yield return new WaitForSeconds(0.5f);

            //��s���G
            if (i < enemySC.actionLimit)
            {
                StartCoroutine(enemySC.Move());
                runnning++;
            }

            while (runnning != 0) yield return null;

            //�\������ׂč폜
            gridManager.ResetReserveListAll();
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
