using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class StartUIScript : MonoBehaviour
{
    [SerializeField, Header("スタートImage")]
    Image startImage;
    [SerializeField, Header("スタートText")]
    Text startText;

    [SerializeField]
    string[] massage = new string[3]; 

    void Start()
    {
        
    }

    /// <summary>
    /// スタートUIフェードイン、アウト
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetUI(int turnNum)
    {
        startText.text = massage[turnNum];
        
        startImage.gameObject.SetActive(true);
        startText.gameObject.SetActive(true);

        //フェードイン
        float time = 0;
        float reqired = 0.5f;       
        while (time < reqired)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, time / reqired);
            startImage.color = new Color(1, 1, 1, alpha);
            startText.color = new Color(1, 1, 1, alpha);

            yield return null;
        }
        
        startImage.color = Color.white;
        startText.color = Color.white;

        float wait = 0;
        if (turnNum == 0 || turnNum == 2) wait = 2.0f;
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
            startText.color = new Color(1, 1, 1, alpha);

            yield return null;
        }
        startImage.color = Color.clear;
        startText.color = Color.clear;

        startImage.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
    }
}
