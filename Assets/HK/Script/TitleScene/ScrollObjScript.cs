using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObjScript : MonoBehaviour
{
    [SerializeField, Header("ÉJÉÅÉâÇÃzç¿ïW")]
    float cameraPosZ;

    SpriteRenderer[] spRenderer = new SpriteRenderer[2];
    void Start()
    {
        spRenderer[0] = gameObject.GetComponent<SpriteRenderer>();
        spRenderer[1] = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        var layerNum = cameraPosZ - transform.position.z;

        foreach (var sp in spRenderer)
        {
            sp.sortingOrder = (int)layerNum;
        }

    }

    void Update()
    {

    }
}
