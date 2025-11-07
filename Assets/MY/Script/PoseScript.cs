using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PoseScript : MonoBehaviour
{
    [SerializeField, Header("ポーズImage")]
    Image poseImage;

    [SerializeField] PlayerInput[] playerInput;
    InputAction[] poseAction = new InputAction[2];
    InputAction[] selectAction = new InputAction[2];
    InputAction[] decisionAction = new InputAction[2];

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

            //決定入力
            bool decision = decisionAction[0].triggered || decisionAction[1].triggered; ;

            if (decision)
            {
                Debug.Log("決定");
            }
        }     
    }

    /// <summary>
    /// スティック入力無効タイマー
    /// </summary>
    /// <returns></returns>
    IEnumerator InputDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
    }
}
