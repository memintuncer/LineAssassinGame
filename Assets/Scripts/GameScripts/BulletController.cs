using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Transform Destination;
    [SerializeField] float BulletSpeed;
    GameObject PlayerTargetTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Destination != null)
        {
            transform.LookAt(Destination);
            transform.position = Vector3.MoveTowards(transform.position, Destination.position, BulletSpeed * Time.deltaTime);
          

        }
    }

    public void SetDestination(Transform destination)
    {
        Destination = destination;
    }

    private void OnTriggerEnter(Collider other)
    {
      
        if(other.tag == "Player" )
        {
            Destroy(gameObject);
            EventManager.TriggerEvent(GameConstants.GameEvents.PLAYER_IS_SHOOTED, new EventParam());
        }
    }

    public void SetPlayerTargetTransform(GameObject player_target_transform)
    {
        PlayerTargetTransform = player_target_transform;
    }
}
