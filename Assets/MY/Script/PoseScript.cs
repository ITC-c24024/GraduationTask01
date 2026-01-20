using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PoseScript : MonoBehaviour
{
    SoundManager soundManager;

    [SerializeField, Header("矢印Image")]
    Image[] arrowImage;
    [SerializeField, Header("ポーズImage")]
    Image poseImage;

    [SerializeField] PlayerInput[] playerInput;
    InputAction[] poseAction = new InputAction[2];
    InputAction[] selectAction = new InputAction[2];
    InputAction[] decisionAction = new InputAction[2];

    //選択している番号
    int selectNum = 0;

    //入力可能判定
    bool canInput = true;

    void Start()
    {
        //プレイヤーのInputActionを取得
        for (int i = 0; i < playerInput.Length; i++)
        {
            var input = playerInput[i];
            var map = input.currentActionMap;
            poseAction[i] = map["Pose"];
            selectAction[i] = map["Move"];
            decisionAction[i] = map["SelectAttack"];
        }

        soundManager = GetComponent<SoundManager>();
    }

    void Update()
    {
        //ポーズ切り替え入力
        bool pose = poseAction[0].triggered || poseAction[1].triggered;
        
        if (pose && Time.timeScale == 1)//ポーズ画面を開いていないとき
        {
            poseImage.gameObject.SetActive(true);
            Time.timeScale = 0;
        }    
        else if (Time.timeScale == 0)//ポーズ画面が開いていたら
        {       
            //ポーズ画面を閉じる
            if (pose)
            {
                Time.timeScale = 1;
                poseImage.gameObject.SetActive(false);
            }

            //スティック入力
            Vector2 stick1 = selectAction[0].ReadValue<Vector2>();
            Vector2 stick2 = selectAction[1].ReadValue<Vector2>();
            if (canInput && (stick1.y < -0.5 || stick2.y < -0.5))
            {
                StartCoroutine(InputDelay());
                SelectUI(1);
            }
            else if (canInput && (stick1.y > 0.5 || stick2.y > 0.5))
            {
                StartCoroutine(InputDelay());
                SelectUI(-1);
            }

            //決定入力
            bool decision = decisionAction[0].triggered || decisionAction[1].triggered;
            if (decision)
            {
                StartCoroutine(SwitchScene());
            }
        }     
    }

    /// <summary>
    /// スティック入力無効タイマー
    /// </summary>
    /// <returns></returns>
    IEnumerator InputDelay()
    {
        canInput = false;
        yield return new WaitForSecondsRealtime(0.2f);
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

    IEnumerator SwitchScene()
    {
        soundManager.Decision();

        yield return new WaitForSecondsRealtime(0.1f);

        Time.timeScale = 1;

        switch (selectNum)
        {
            case 0:               
                poseImage.gameObject.SetActive(false);
                break;
            case 1:
                SceneManager.LoadScene("MainScene");
                break;
            case 2:
                SceneManager.LoadScene("TitleScene");
                break;
        }
    }
}
