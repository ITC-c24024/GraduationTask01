using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObjManager : MonoBehaviour
{
    [SerializeField, Header("プレハブオブジェクト")]
    GameObject prefabObj;

    [SerializeField, Header("オブジェクトリスト")]
    List<GameObject> objList;

    int objNum = 0;

    bool isMove = true;
    void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            objList.Add(Instantiate(prefabObj, gameObject.transform.position, gameObject.transform.rotation));
        }

        foreach(var obj in objList)
        {
            obj.SetActive(false);
        }

        SpawnFirstObj();
        StartCoroutine(SelectObj());
    }

    void SpawnFirstObj()
    {
        var posX = -15f;
        
        while(posX < 34)
        {
            objList.Add(Instantiate(prefabObj, new Vector3(posX, 0, Random.Range(-1f, 18f)), Quaternion.identity));
            posX += Random.Range(3.5f, 7f);
        }
    }

    IEnumerator SelectObj()
    {
        while (isMove)
        {
            var obj = objList[objNum];
            obj.transform.position = new Vector3(35, 0, Random.Range(-1f, 18f));
            obj.SetActive(true);

            if (objNum < objList.Count - 1)
            {
                objNum++;
            }
            else
            {
                objNum = 0;
            }

            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }
    }
}
