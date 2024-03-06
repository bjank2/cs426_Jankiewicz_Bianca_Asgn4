using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// adding namespaces
using Unity.Netcode;
using Unity.VisualScripting;
// because we are using the NetworkBehaviour class
// NewtorkBehaviour class is a part of the Unity.Netcode namespace
// extension of MonoBehaviour that has functions related to multiplayer
public class PlayerMovement : NetworkBehaviour
{
    public float speed = 2f;
    // create a list of colors
    public List<Material> materials = new List<Material>();

    // getting the reference to the prefab
    [SerializeField]
    private GameObject spawnedPrefab;
    // save the instantiated prefab
    private GameObject instantiatedPrefab;

    public GameObject cannon;
    public GameObject bullet;

    // reference to the camera audio listener
    [SerializeField] private AudioListener audioListener;
    // reference to the camera
    [SerializeField] private Camera playerCamera;

    // Players assigned value
    public NetworkVariable<int> PlayerNumber = new NetworkVariable<int>();

    // Screen text to tell player what number they are
    [SerializeField] public TMP_Text playerNumberText;

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        // check if the player is the owner of the object
        // makes sure the script is only executed on the owners 
        // not on the other prefabs 
        if (!IsOwner) return;

        Vector3 moveDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection.z = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection.z = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x = +1f;
        }
        transform.position += moveDirection * speed * Time.deltaTime;


        // if I is pressed spawn the object 
        // if J is pressed destroy the object
        if (Input.GetKeyDown(KeyCode.I))
        {
            //instantiate the object
            instantiatedPrefab = Instantiate(spawnedPrefab);
            // spawn it on the scene
            instantiatedPrefab.GetComponent<NetworkObject>().Spawn(true);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            //despawn the object
            instantiatedPrefab.GetComponent<NetworkObject>().Despawn(true);
            // destroy the object
            Destroy(instantiatedPrefab);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            // call the BulletSpawningServerRpc method
            // as client can not spawn objects
            bool isHost = IsServer;
            BulletSpawningServerRpc(cannon.transform.position, cannon.transform.rotation, isHost);
        }
    }

    // this method is called when the object is spawned
    // we will change the color of the objects
    public override void OnNetworkSpawn()
    {

        Transform cat = transform.Find("Crazy_Cat");
        // GetComponent<MeshRenderer>().material = materials[PlayerNumber.Value];

        SkinnedMeshRenderer[] catMeshRenderers = cat.GetComponentsInChildren<SkinnedMeshRenderer>();
        catMeshRenderers[0].material = materials[(int)OwnerClientId];

        // check if the player is the owner of the object
        if (!IsOwner) {
            playerCamera.enabled = false;
            audioListener.enabled = false;
        } else {
            playerCamera.enabled = true;
            audioListener.enabled = true;
            PlayerNumber.Value = IsServer ? 0 : 1;
        }
    }

    // need to add the [ServerRPC] attribute
    [ServerRpc]
    // method name must end with ServerRPC
    private void BulletSpawningServerRpc(Vector3 position, Quaternion rotation, bool isHost)
    {
        // call the BulletSpawningClientRpc method to locally create the bullet on all clients
        BulletSpawningClientRpc(position, rotation, isHost);
    }

    [ClientRpc]
    private void BulletSpawningClientRpc(Vector3 position, Quaternion rotation, bool isHost)
    {
        GameObject newBullet = Instantiate(bullet, position, rotation);
        newBullet.tag = isHost ? "HostBullet" : "ClientBullet";
        if (isHost)
        {
            newBullet.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        else
        {
            newBullet.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        newBullet.GetComponent<Rigidbody>().velocity += Vector3.up * 2;
        newBullet.GetComponent<Rigidbody>().AddForce(newBullet.transform.forward * 1500);
    }


}
