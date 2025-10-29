using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        Invoke("Call",2) ;      
    }
    //��
    void Call()
    {
        for (int i = 0; i < 5; i++)
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
        //gridManager������W���炤
        Vector2Int spownPos = gridManager.EnemySpawnCheck(GetPlayerPos());
        if (spownPos == -Vector2.one) return false;

        //�G�X�|�[��
        GameObject enemy = Instantiate(
            enemyPrefab,
            new Vector3(spownPos.y, enemyPrefab.transform.position.y, spownPos.x),
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
        gridManager.ChangeCellState(spownPos.x, spownPos.y, CellScript.CellState.enemy, enemySC, Vector2Int.zero);

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
    /// �G���v���C���[�Ƃ̋������߂����ɕ��בւ�
    /// </summary>
    public void SortEnemy()
    {
        //�G���g��shortDir���X�V
        foreach(var enemy in enemyList)
        {
            enemy.SetShortDir();
        }

        //���X�g����בւ�
        enemyList.OrderByDescending(x => x.shortDir);
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

        //���������בւ�
        SortEnemy();

        //�G�s��
        for (int n = 0; n < enemyList.Count; n++)
        {
            //�s���񐔂������Ȃ瓮��
            if (n < enemyList[n].actionLimit)
            {
                StartCoroutine(enemyList[n].Move());
                runnning++;

                while (runnning != 0) yield return null;
            }
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
