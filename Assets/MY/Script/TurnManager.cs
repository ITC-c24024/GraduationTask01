using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [SerializeField] PlayerController[] playerCon;
    GridManager gridManager;   
    [SerializeField] CellScript cellScript;

    [SerializeField, Header("���[���h�J����")]
    Camera worldCamera;

    //�X�|�[�������G��List
    List<CharaScript> enemyList = new List<CharaScript>();
    [SerializeField, Header("�GPrefab")]
    GameObject enemyPrefab;
    [SerializeField, Header("�G��HP�o�[")]
    Slider hpPrefab;
    [SerializeField] Canvas canvas;

    //���s���R���[�`���̐�
    int runnning = 0;

    void Awake()
    {
        Application.targetFrameRate = 30;
    }

    void Start()
    {        
        gridManager = gameObject.GetComponent<GridManager>();

        //��
        Invoke("Call",3) ;      
    }
    //��
    void Call()
    {
        for (int i = 0; i < 3; i++)
        {
            EnemySpown();
        }
        StartCoroutine(TurnStart());
    }

    /// <summary>
    /// �G���X�|�[��
    /// </summary>
    /// <param name="enemyNum">�X�|�[�����������G�̐�</param>
    public bool EnemySpown()
    {
        //�X�|�[���������ʒu�𒲂ׂ�(��)
        int x = Random.Range(0, 8);
        int z = Random.Range(0, 8);

        //gridManager������W���炤
        Vector2Int spownPos = gridManager.EnemySpawnCheck(GetPlayerPos());
        if (spownPos == -Vector2.one) return false;

        //�G�X�|�[��
        GameObject enemy = Instantiate(
            enemyPrefab,
            new Vector3(spownPos.y, 0.6f, spownPos.x),
            enemyPrefab.transform.rotation
            );

        //HP�X���C�_�[�A�^�b�`
        Slider slider = Instantiate(hpPrefab);
        slider.transform.SetParent(canvas.transform);

        //�K�v�ȃR���|�[�l���g���擾
        CharaScript enemySC = enemy.GetComponent<CharaScript>();
        enemySC.turnManager = this;
        enemySC.gridManager = gridManager;
        enemySC.cellScript = cellScript;
        enemySC.hpSlider = slider;
        enemySC.worldCamera = worldCamera;
        enemySC.canvas = canvas;

        //List�ɒǉ�
        enemyList.Add(enemySC);

        //�}�X��ԍX�V
        gridManager.ChangeCellState(z, x, CellScript.CellState.enemy, enemySC, Vector2Int.zero);

        return true;
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
    public IEnumerator TurnStart()
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
            for (int n = 0; n < enemyList.Count; n++)
            {
                //�s���񐔂������Ȃ瓮��
                if (n < enemyList[n].actionLimit)
                {
                    StartCoroutine(enemyList[n].Move());
                    runnning++;
                }
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
