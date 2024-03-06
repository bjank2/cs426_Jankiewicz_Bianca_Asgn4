using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// adding namespaces
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class Target : NetworkBehaviour
{

    private string sequence = "";
    //this method is called whenever a collision is detected
    [SerializeField] private GameObject Menu;
    public GameObject BBQuestion;
    [SerializeField] private static int RandomNumber = 13;
    [SerializeField] private TMP_Text BinCode;
    public string requiredSequence = System.Convert.ToString(RandomNumber, 2).PadLeft(4, '0');
   
    void Start()
    {
    BinCode.text = RandomNumber.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (collision.gameObject.tag == "HostBullet")
        {
            sequence += "0";
        }
        else if (collision.gameObject.tag == "ClientBullet")
        {
            sequence += "1";
        }

        if (sequence.Length >= requiredSequence.Length && sequence.Substring(sequence.Length - requiredSequence.Length) == requiredSequence)
        {
            DestroyTargetServerRpc();
            // Calculate a new position 10 units above the current GameObject's position
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
            // Create a new rotation of 90 degrees around the Y axis
            Quaternion spawnRotation = Quaternion.Euler(0, 0, 0);
            GameObject newBillboard = Instantiate(BBQuestion, spawnPosition, spawnRotation);
            sequence = "";
        }

        // // printing if collision is detected on the console

        // // if the collision is detected destroy the object
        // DestroyTargetServerRpc();
    }

    // client can not spawn or destroy objects
    // so we need to use ServerRpc
    // we also need to add RequireOwnership = false
    // because we want to destroy the object even if the client is not the owner
    [ServerRpc(RequireOwnership = false)]
    public void DestroyTargetServerRpc()
    {
        //despawn
        GetComponent<NetworkObject>().Despawn(true);
        //after collision is detected destroy the gameobject
        Destroy(gameObject);
    }

    [ClientRpc]
    private void ShowMenuClientRpc()
    {
        Menu.SetActive(true);
    }
}