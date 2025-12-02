using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    WaveManager waveManager;

    [SerializeField, Header("プレイヤー待機UI")]
    Image waitImage;

    [SerializeField, Header("プレイヤーオブジェクト")]
    GameObject[] playerObj;

    public bool isStart = false;

    void Start()
    {
        waveManager = gameObject.GetComponent<WaveManager>();

        waitImage.gameObject.SetActive(true);
    }

    void Update()
    {
        
    }

    public IEnumerator GameStart()
    {
        waitImage.gameObject.SetActive(false); ;

        //プレイヤーを初期位置に移動
        for (int i = 0; i < playerObj.Length; i++)
        {
            var playerCon = playerObj[i].GetComponent<PlayerController>();
            playerCon.SetPlayer();
        }

        yield return new WaitForSeconds(2.0f);

        isStart = true;

        //ウェーブスタート
        waveManager.StartWave();
    }
}
