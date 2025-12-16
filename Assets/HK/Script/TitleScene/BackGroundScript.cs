using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackGroundScript : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public bool isScroll = false;
    public float scrollSpeed;

    float length;

    void Start()
    {
        length = Mathf.Abs(endPos.x - startPos.x);
        StartCoroutine(ScrollObj());
    }

    IEnumerator ScrollObj()
    {
        while (isScroll)
        {
            // ˆê’è—Ê‚¾‚¯ˆÚ“®
            transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

            // end ‚ð’´‚¦‚½‚çƒsƒbƒ^ƒŠ–ß‚·
            if (transform.position.x <= endPos.x)
            {
                transform.position = new Vector3(
                    transform.position.x + length,
                    transform.position.y,
                    transform.position.z
                );
            }

            yield return null;
        }
    }
}

