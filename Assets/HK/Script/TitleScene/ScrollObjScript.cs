using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObjScript : BackGroundScript
{
    [SerializeField, Header("オブジェクトリスト")]
    List<GameObject> objList;

    int objNum = 0;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        foreach (var obj in objList)
        {
            obj.SetActive(false);
        }
        
        objNum = Random.Range(0, objList.Count);
        objList[objNum].SetActive(true);

        var posZ = transform.position.z;
        endPos += new Vector3(0, 0, posZ);

        isScroll = true;

        StartCoroutine(ScrollObj());
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
                isScroll = false;
                gameObject.SetActive(false);
                endPos = new Vector3(-35f,0,0);
            }
            yield return null;
        }
    }
}
