using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelObject : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == Tags.PIXELOBJECT)
            collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
        else if (tag == Tags.FLOOR)
            Destroy(gameObject);
        else if (tag == Tags.OBSTACLE)
            StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }




}
