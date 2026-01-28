using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR

[InitializeOnLoad]
public static class NativeLeakSetup
{
    static NativeLeakSetup()
    {
        NativeLeakDetection.Mode =
            NativeLeakDetectionMode.EnabledWithStackTrace;
    }
}
#endif

public class GameController : MonoBehaviour
{
    WaveManager waveManager;
    SoundManager soundManager;

    [SerializeField, Header("フェードオブジェクト")]
    GameObject fadeObj;
    [SerializeField, Header("プレイヤー待機UI")]
    Image waitImage;

    [SerializeField, Header("速度UIの矢印")]
    GameObject[] arrowImage;

    [SerializeField, Header("プレイヤーオブジェクト")]
    GameObject[] playerObj;

    public bool isOpen = false;
    public bool isStart = false;

    [SerializeField, Header("倍速段階")]
    int[] gameSpeed;
    int num = 0;//要素数

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
        fadeObj.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        Vector2 startPos = new Vector2(0, fadeObj.transform.localPosition.y); ;
        Vector2 targetPos = new Vector2(-2560, fadeObj.transform.localPosition.y);

        float time = 0;
        float reqired = 1.0f;
        while (time < reqired)
        {
            time += Time.deltaTime;

            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, time / reqired);
            fadeObj.transform.localPosition = currentPos;

            yield return null;
        }
    }
    IEnumerator FadeOut()
    {
        Vector2 startPos = new Vector2(2560, fadeObj.transform.localPosition.y);
        Vector2 targetPos = new Vector2(0, fadeObj.transform.localPosition.y);

        float time = 0;
        float reqired = 1.0f;
        while (time < reqired)
        {
            time += Time.deltaTime;

            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, time / reqired);
            fadeObj.transform.localPosition = currentPos;

            yield return null;
        }
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

    public IEnumerator FinishGame()
    {
        yield return FadeOut();
        SceneManager.LoadScene("ResultScene");
    }

    /// <summary>
    /// ゲーム速度アップ
    /// </summary>
    public void SpeedUp()
    {
        num++;
        if (num > gameSpeed.Length - 1) num = 0;

        Time.timeScale = gameSpeed[num];

        SetSpeedUI(Time.timeScale);
    }
    /// <summary>
    /// ゲーム速度に応じて速度UIを表示
    /// </summary>
    /// <param name="nowSpeed">現在の速度</param>
    public void SetSpeedUI(float nowSpeed)
    {
        //表示するImageの数
        int count = num;

        for (int i = 0; i < arrowImage.Length; i++)
        {
            arrowImage[i].SetActive(false);
        }

        arrowImage[count].SetActive(true);
    }
}
