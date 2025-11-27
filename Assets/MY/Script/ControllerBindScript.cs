using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerBindScript : MonoBehaviour
{
    GameController gameController;
    PoseScript poseScript;

    [SerializeField, Header("プレイヤーオブジェクト")]
    GameObject[] playerObj;

    int slotNum = 0;

    public void OnPlayerJoined(PlayerInput joined)
    {
        // 仮オブジェクトかどうか判定
        if (joined.gameObject.tag.StartsWith("P")) return;

        // 対応する本物プレイヤーのPlayerInput
        var player = playerObj[slotNum].GetComponent<PlayerInput>();

        playerObj[slotNum].SetActive(true);

        // 仮オブジェクトのデバイスを本物に移す
        player.SwitchCurrentControlScheme(joined.devices.ToArray());

        // 仮オブジェクトを削除
        Destroy(joined.gameObject);

        //全員現れたら
        if (slotNum == playerObj.Length - 1)
        {
            gameController = GetComponent<GameController>(); 
            StartCoroutine(gameController.GameStart());

            poseScript = GetComponent<PoseScript>();
            poseScript.enabled = true;
        }

        slotNum++;
    }
}
