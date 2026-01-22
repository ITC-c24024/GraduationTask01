using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RuleScript : MonoBehaviour
{
    [SerializeField, Header("入力用")]
    InputActionAsset inputAsset;

    //バインドされたアクション
    InputAction selectAct;

    [SerializeField, Header("ルールのページUI")]
    GameObject[] page = new GameObject[7];

    //選択番号
    int select;

    //入力クールタイム
    bool isCooltime = false;
    void Awake()
    {
        //選択用アクションを適用
        selectAct = inputAsset.FindAction("Move");
    }

    void OnEnable()
    {
        selectAct.Enable();

        //全ページを非表示
        foreach (var p in page)
        {
            p.SetActive(false);
        }

        //1ページ目を表示
        select = 0;
        page[select].SetActive(true);
    }

    void OnDisable()
    {
        selectAct.Disable();
    }

    void Update()
    {
        //スティック入力の値を取得
        var value = selectAct.ReadValue<Vector2>();

        //入力可能だったら
        if (!isCooltime)
        {
            if (value.x >= 0.5f)
            {
                page[select].SetActive(false);

                if (select == page.Length - 1) select = 0;
                else select++;

                page[select].SetActive(true);

                StartCoroutine(StartCooltime());
            }
            else if (value.x <= -0.5f)
            {
                page[select].SetActive(false);

                if (select == 0) select = page.Length - 1;
                else select--;

                page[select].SetActive(true);

                StartCoroutine(StartCooltime());
            }
        }
    }

    IEnumerator StartCooltime()
    {
        if (isCooltime) yield break;

        isCooltime = true;

        yield return new WaitForSeconds(0.2f);

        isCooltime = false;
    }
}
