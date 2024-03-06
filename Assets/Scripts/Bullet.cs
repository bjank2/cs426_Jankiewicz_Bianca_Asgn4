using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int BinaryValue { get; set; }
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.tag == "Target" | collision.gameObject.tag == "Plane")
        Destroy(gameObject);
        Debug.Log("Collision Detected");

    }

}
