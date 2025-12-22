using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    WaveManager waveManager;
    SoundManager soundManager;

    [SerializeField, Header("プレイヤー待機UI")]
    Image waitImage;

    [SerializeField, Header("速度UIの矢印")]
    GameObject[] arrowImage;

    [SerializeField, Header("プレイヤーオブジェクト")]
    GameObject[] playerObj;

    public bool isStart = false;

    void Awake()
    {
        Application.targetFrameRate = 30;
    }

    void Start()
    {
        waveManager = gameObject.GetComponent<WaveManager>();
        soundManager = gameObject.GetComponent<SoundManager>();
        waitImage.gameObject.SetActive(true);
    }

    public IEnumerator GameStart()
    {
        waitImage.gameObject.SetActive(false);

        //プレイヤーを初期位置に移動
        for (int i = 0; i < playerObj.Length; i++)
        {
            var playerCon = playerObj[i].GetComponent<PlayerController>();
            playerCon.SetPlayer();
        }

        yield return new WaitForSeconds(2.0f);

        isStart = true;

        //ウェーブスタート
        StartCoroutine(waveManager.StartWave());
    }

    /// <summary>
    /// ゲーム速度アップ
    /// </summary>
    public void SpeedUp()
    {
        Time.timeScale += 1;
        //上限3倍
        if (Time.timeScale > 3)
        {
            Time.timeScale = 1;
        }

        SetSpeedUI(Time.timeScale);
    }
    /// <summary>
    /// ゲーム速度に応じて速度UIを表示
    /// </summary>
    /// <param name="nowSpeed">現在の速度</param>
    public void SetSpeedUI(float nowSpeed)
    {
        //表示するImageの数
        int count = (int)nowSpeed - 1;

        for (int i = 0; i < arrowImage.Length; i++)
        {
            arrowImage[i].SetActive(false);
        }

        arrowImage[count].SetActive(true);
    }
}
