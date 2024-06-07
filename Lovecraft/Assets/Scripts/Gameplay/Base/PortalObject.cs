using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PortalObject : MonoBehaviour
{
    [SerializeField] private PortalObject connectedPortal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.parent.tag != "Player" || connectedPortal == null) { return; }

        other.transform.parent.GetComponent<NavMeshAgent>().enabled = false;

        Transform _spawnPoint = connectedPortal.transform.Find("SpawnPoint");
        other.transform.parent.SetPositionAndRotation(_spawnPoint.position, other.transform.parent.rotation);
        other.transform.SetPositionAndRotation(other.transform.position, _spawnPoint.rotation);


        other.transform.parent.GetComponent<NavMeshAgent>().enabled = true;
    }
}
