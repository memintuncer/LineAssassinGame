using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    

    List<Transform> PatrolDestinations = new List<Transform>();
    
    [SerializeField] Transform PatrolsParentTransform;
    [SerializeField] Transform BulletSpawnPosition;
    [SerializeField] GameObject FieldOfView;
    [SerializeField] Animator EnemyAnimator;
    Collider SelfCollider;
    public bool IsPlayerSpotted = false,IsDead = false, IsPatrolling=false;
    Transform PlayerControllerTransform;
    public GameObject BulletPrefab;
    [SerializeField] float PatrolTimer, MovementSpeed;
    int DestinationIndex = 1;
    private void OnEnable()
    {
        EventManager.StartListening(GameConstants.GameEvents.ATTACK_TO_ENEMY, CheckAttackedSelf);
        EventManager.StartListening(GameConstants.GameEvents.PLAYER_IS_SPOTTED, ShootThePlayer);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameConstants.GameEvents.ATTACK_TO_ENEMY, CheckAttackedSelf);
        EventManager.StopListening(GameConstants.GameEvents.PLAYER_IS_SPOTTED, ShootThePlayer);
    }

    void ShootThePlayer(EventParam param)
    {
        if (!IsDead)
        {
            if (param.TargetEnemy == gameObject)
            {
                PlayerControllerTransform = param.PlayerTransform;
                EnemyAnimator.SetTrigger("ShootPlayer");
                transform.LookAt(PlayerControllerTransform);
                StartCoroutine(CreateBulletCRT(PlayerControllerTransform));
                IsPlayerSpotted = true;

            }
        }
        
       
    }

    IEnumerator CreateBulletCRT(Transform player_controller_transform)
    {
        yield return new WaitForSeconds(0.75f);
        GameObject bullet_object = Instantiate(BulletPrefab, BulletSpawnPosition.position, Quaternion.identity);
        BulletController bullet_controller = bullet_object.GetComponent<BulletController>();
        bullet_controller.SetDestination(player_controller_transform);
        bullet_controller.SetDestination(player_controller_transform);
        
    }



    public void SetIsPlayerSpotted(bool state)
    {
        IsPlayerSpotted = state;
    }

    void CheckAttackedSelf(EventParam param)
    {
        
        if(param.TargetEnemy == gameObject)
        {
            IsDead = true;
            Destroy(SelfCollider);
            StartCoroutine(EnemyDeathCRT());
        }
    }

    IEnumerator EnemyDeathCRT()
    {
        yield return new WaitForSeconds(0.5f);
        EnemyAnimator.SetTrigger("Death");
        FieldOfView.SetActive(false);
    }

    void Start()
    {
        GetPatrolDestinations();
        SelfCollider = GetComponent<Collider>();
    }

    void GetPatrolDestinations()
    {
        int patrol_count = PatrolsParentTransform.childCount;
        for (int i = 0; i < patrol_count; i++)
        {
            Transform destination = PatrolsParentTransform.GetChild(0);
            
            PatrolDestinations.Add(destination);
            destination.parent = transform.parent;
        }
    }
    void CheckPatrolState()
    {
        PatrolTimer -= Time.deltaTime;
        if(PatrolTimer <= 0)
        {
            IsPatrolling = true;
            EnemyAnimator.SetTrigger("Walk");
        }
    }


    void GoToPatrolDestination()
    {
        
      
        transform.LookAt(PatrolDestinations[DestinationIndex]);
        transform.position = Vector3.MoveTowards(transform.position, PatrolDestinations[DestinationIndex].position, MovementSpeed * Time.deltaTime);
        

        
    }

   
    void Update()
    {
        
        if (!IsDead)
        {
            if (IsPlayerSpotted)
            {
                transform.LookAt(PlayerControllerTransform);
            }
            if (!IsPlayerSpotted)
            {
                if (!IsPatrolling)
                {
                    CheckPatrolState();
                }
                
                if (IsPatrolling)
                {
                    GoToPatrolDestination();
                    if(transform.position == PatrolDestinations[DestinationIndex].position)
                    {
                        ReachedPatrolPoint();
                    }
                }
            }
        }
    }

    void ReachedPatrolPoint()
    {
        PatrolTimer = 8f;
        
        transform.LookAt(PatrolDestinations[(DestinationIndex + 1) % PatrolDestinations.Count]);
        DestinationIndex = (DestinationIndex + 1) % PatrolDestinations.Count;
        IsPatrolling = false;
        EnemyAnimator.SetTrigger("Idle");
    }
}
