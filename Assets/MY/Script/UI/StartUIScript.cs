using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class StartUIScript : MonoBehaviour
{
    [SerializeField, Header("スタートImage")]
    Image startImage;

    [Tooltip("0:ウェーブ開始\n1:プレイヤーのターン\n2:敵のターン\n3:ウェーブクリア\n4:ゲームオーバー")]
    [SerializeField, Header("スタートText")]   
    Text[] startText = new Text[5];

    [SerializeField, Header("敵の数Text")]
    Text enemyCountText;
    [SerializeField, Header("ウェーブText")]
    Text waveCountText;

    void Start()
    {
        
    }

    /// <summary>
    /// スタートUIフェードイン、アウト
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetUI(int i)
    {        
        startImage.gameObject.SetActive(true);
        startText[i].gameObject.SetActive(true);

        //フェードイン
        float time = 0;
        float reqired = 0.5f;       
        while (time < reqired)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, time / reqired);
            startImage.color = new Color(1, 1, 1, alpha);
            startText[i].color = new Color(1, 1, 1, alpha);

            yield return null;
        }
        
        startImage.color = Color.white;
        startText[i].color = Color.white;

        float wait = 0;
        if (i == 0 || i == 3 || i == 4) wait = 2.0f;
        else wait = 0.75f;
        
        yield return new WaitForSeconds(wait);

        //フェードアウト
        time = 0;
        reqired /= 2;
        while (time < reqired)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, time / (reqired));
            startImage.color = new Color(1, 1, 1, alpha);
            startText[i].color = new Color(1, 1, 1, alpha);

            yield return null;
        }
        startImage.color = Color.clear;
        startText[i].color = Color.clear;

        startImage.gameObject.SetActive(false);
        startText[i].gameObject.SetActive(false);
    }

    /// <summary>
    /// 現在のウェーブ数を表示
    /// </summary>
    /// <param name="i">現在のウェーブ数</param>
    public void SetWaveCount(int i)
    {
        waveCountText.text = $"ウェーブ{i}";
    }

    /// <summary>
    /// 敵の数を表示
    /// </summary>
    /// <param name="n">残り敵数</param>
    public void SetEnemyCount(int n)
    {
        enemyCountText.text = $"残り {n}体";
    }
}
