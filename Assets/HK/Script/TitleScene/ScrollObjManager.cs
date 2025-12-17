using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObjManager : BackGroundScript
{
    [SerializeField, Header("オブジェクトリスト")]
    List<GameObject> objList;

    int objNum = 0;
    void Start()
    {
        StartCoroutine(ScrollObj());
        objNum = Random.Range(0, objList.Count);
        objList[objNum].SetActive(true);
    }

    void Update()
    {
        
    }

    IEnumerator ScrollObj()
    {
        while (isScroll)
        {
            float step = scrollSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, endPos, step);

            // 距離で判定する
            if (transform.position == endPos)
            {
                transform.position = startPos;
                transform.position = startPos;
                objList[objNum].SetActive(false);
                objNum = Random.Range(0, objList.Count);
                objList[objNum].SetActive(true);
            }
            yield return null;
        }
    }
}
