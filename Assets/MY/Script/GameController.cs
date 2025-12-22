using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    WaveManager waveManager;
    SoundManager soundManager;

    [SerializeField, Header("フェードインImage")]
    Image fadeImage;
    [SerializeField, Header("プレイヤー待機UI")]
    Image waitImage;

    [SerializeField, Header("速度UIの矢印")]
    GameObject[] arrowImage;

    [SerializeField, Header("プレイヤーオブジェクト")]
    GameObject[] playerObj;

    public bool isOpen = false;
    public bool isStart = false;

    void Awake()
    {
        Application.targetFrameRate = 30;
        Cursor.visible = false;
    }

    void Start()
    {
        waveManager = gameObject.GetComponent<WaveManager>();
        soundManager = gameObject.GetComponent<SoundManager>();
        waitImage.gameObject.SetActive(true);

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        float time = 0;
        float reqired = 1.0f;
        while (time < reqired)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(1, 0, time / reqired);
            fadeImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }

        //isOpen = true;
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
