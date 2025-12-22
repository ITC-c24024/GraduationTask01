using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]

public class TitleManagerScript : MonoBehaviour
{
    SoundManager soundManager;

    [SerializeField, Header("矢印Image")]
    Image[] arrowImage;
    [SerializeField, Header("フェードImage")]
    Image fadeImage;

    //選択している番号
    int selectNum = 0;
    //入力制御タイマー
    float timer = 0;
    //入力可能判定
    bool canInput = true;

    InputAction stickAction;
    InputAction descisionAction;

    void Start()
    {
        Application.targetFrameRate = 30;
        Cursor.visible = false;

        var actionMap = GetComponent<PlayerInput>().currentActionMap;
        stickAction = actionMap["Move"];
        descisionAction = actionMap["SelectAttack"];

        soundManager = GetComponent<SoundManager>();
    }

    void Update()
    {
        Vector2 stick = stickAction.ReadValue<Vector2>();
        if (stick.y > 0.5f && canInput)
        {
            SelectUI(-1);
            StartCoroutine(Timer());
        }
        else if (stick.y < -0.5f && canInput)
        {
            SelectUI(1);
            StartCoroutine(Timer());
        }

        bool decision = descisionAction.triggered;
        if (decision)
        {
            StartCoroutine(SwitchScene(selectNum));
        }
    }

    IEnumerator Timer()
    {
        canInput = false;
        yield return new WaitForSeconds(0.2f);
        canInput = true;
    }

    /// <summary>
    /// UIを選択
    /// </summary>
    /// <param name="i">選択したい方向</param>
    void SelectUI(int i)
    {
        if (selectNum + i >= 0 && selectNum + i < arrowImage.Length)
        {
            soundManager.Select();
            
            arrowImage[selectNum].gameObject.SetActive(false);
            selectNum += i;          
            arrowImage[selectNum].gameObject.SetActive(true);
        }
    }

    IEnumerator SwitchScene(int i)
    {
        soundManager.Decision();

        yield return StartCoroutine(FadeOut());

        switch (selectNum)
        {
            case 0:
                SceneManager.LoadScene("MainScene");
                break;
            case 1:
                Application.Quit();
                break;
        }
    }

    IEnumerator FadeOut()
    {
        float time = 0;
        float reqired = 1.0f;
        while (time < reqired)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(0, 1, time / reqired);
            fadeImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }
    }
}
