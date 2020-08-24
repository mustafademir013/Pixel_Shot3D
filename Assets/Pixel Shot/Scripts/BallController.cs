using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    /// ----------- Variables------------- 

    //ready ball 
    public static GameObject currentBall;


    /// ----------- Static Functions------------- 
    /// 


    public static void SpawnBall(Vector3 position, GameObject ball)
    {
        currentBall = Instantiate(ball, position, Quaternion.identity);
    }

    //Currentball collider active 
    public static void SetActiveCollider(bool value)
    {
        if (currentBall != null)
            currentBall.GetComponent<Collider>().enabled = value;
    }
    //Currentball position set 
    public static void SetPosition(Vector3 targetPosition)
    {
        if (currentBall != null)
            currentBall.transform.position = targetPosition;
    }
    //Currentball addforce 
    public static void AddForce(Vector3 force)
    {
        if (currentBall != null)
        {
            currentBall.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            currentBall.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    /// ----------- Events-------------

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == Tags.PIXELOBJECT)
            collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
        else if (tag == Tags.FLOOR)
            Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        Debug.Log("IsTrigger");
    }
}
