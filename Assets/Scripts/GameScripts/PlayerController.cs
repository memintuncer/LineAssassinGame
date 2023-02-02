using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerStates
    {
        Idle,
        FollowPath,
        DrawingPath,
        Attack,
        IsSpotted,
        Death,
        Success,
        Fail,
        Finish,
        NoEffect,

    }

  
    public PlayerStates PlayerState = PlayerStates.Idle;

    [SerializeField] float MovementSpeed, TimeForNextRay = 0.05f, AttackTimer=0.25f;
    [SerializeField] Animator PlayerAnimator;
    InputManager InputManager;
    [SerializeField] LineRenderer LineRenderer;
    bool IsTouchedToPlayer,IsLineCompleted,IsAttackToEnemy,IsPlayerShooted,IsDead,CanFinishTheLevel;
    [SerializeField] Camera GameCamera;
    public List<Vector3> WayPoints;
    public int WayPointsCount = 0, CurrentWayPointIndex = 0;
    Transform TargetEnemy = null;
    float DrawTimer = 0.75f;
    Transform PlayerModel,FinishDestination;

    private void OnEnable()
    {
        EventManager.StartListening(GameConstants.GameEvents.ATTACK_TO_ENEMY, SetIsAttackToEnemy);
        EventManager.StartListening(GameConstants.GameEvents.PLAYER_IS_SHOOTED, PlayerIsShooted);
        EventManager.StartListening(GameConstants.GameEvents.OPEN_THE_DOOR, DoorIsOpened);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameConstants.GameEvents.ATTACK_TO_ENEMY, SetIsAttackToEnemy);
        EventManager.StopListening(GameConstants.GameEvents.PLAYER_IS_SHOOTED, PlayerIsShooted);
        EventManager.StopListening(GameConstants.GameEvents.OPEN_THE_DOOR, DoorIsOpened);
    }

    void SetIsAttackToEnemy(EventParam param)
    {
        IsAttackToEnemy = true;
    }
  

       void PlayerIsShooted(EventParam param)
    {
        
        IsPlayerShooted = true;
        IsDead = true;
        PlayerState = PlayerStates.Death;
    }
       void DoorIsOpened(EventParam param)
    {
        
        CanFinishTheLevel = true;
    }



    void Start()
    {
        InputManager = InputManager.Instance;
        LineRenderer.enabled = false;
        
    }

    void Update()
    {
        if (!IsDead)
        {
            PlayerStatesController();
        }
        if (IsDead)
        {
            
            DeathStatesController();
        }
        
        
        
        transform.GetChild(0).transform.GetChild(0).localPosition = Vector3.zero;
    }


    void DeathStatesController()
    {
        switch (PlayerState)
        {
            case PlayerStates.IsSpotted:
                break;
            case PlayerStates.Death:
                StartCoroutine(PlayerDeathCRT());
                PlayerState = PlayerStates.NoEffect;
                break;

        }
    }

    void PlayerStatesController()
    {
        switch (PlayerState)
        {
            
            case PlayerStates.Idle:
                
                if (InputManager.isPointerClick || InputManager.isDrag)
                {
                    CheckTouchedToPlayer();
                  
                }
                if (IsTouchedToPlayer)
                {
                    LineRenderer.enabled = true;
                    PlayerState = PlayerStates.DrawingPath;

                }
                if (IsAttackToEnemy)
                {
                    PlayerState = PlayerStates.Attack;
                }
                break;
            case PlayerStates.DrawingPath:
               
                if (InputManager.isDrag)
                {
                    DrawPath();
                }
                else
                {
                    PlayerAnimator.SetTrigger("Run");
                    PlayerState = PlayerStates.FollowPath;
                }

                break;
            case PlayerStates.FollowPath:
               
                FollowCreatedPath();
                if (IsLineCompleted)
                {
                    ReturnToIdleState();
                    PlayerState = PlayerStates.Idle;
                    EventManager.TriggerEvent(GameConstants.GameEvents.COMPLETED_FOLLOWING_PATH, new EventParam());
                }

                if (IsAttackToEnemy)
                {
                    PlayerState = PlayerStates.Attack;
                }
                if (IsPlayerShooted)
                {
                    PlayerState = PlayerStates.Death;
                }
                break;
            case PlayerStates.Attack:
                
                AttackToEnemy();
                
                AttackTimer -= Time.deltaTime;
                if (AttackTimer <= 0)
                {
                    IsAttackToEnemy = false;
                    AttackTimer = 0.25f;
                    TargetEnemy = null;
                    if (WayPointsCount > 0)
                    {
                        PlayerState = PlayerStates.FollowPath;
                    }
                    else
                    {
                        PlayerState = PlayerStates.Idle;
                    }
                    
                    
                }
                
                break;
           
            case PlayerStates.Success:
                StartCoroutine(LevelSuccessedCRT());
                
                break;


        }
    }

    IEnumerator PlayerDeathCRT()
    {
        PlayerAnimator.SetTrigger("Death");
        yield return new WaitForSeconds(1.5f);
        EventManager.TriggerEvent(GameConstants.LEVEL_EVENTS.LEVEL_FAILED, new EventParam());
    }

    IEnumerator LevelSuccessedCRT()
    {
       
        transform.LookAt(FinishDestination);
        transform.position = Vector3.MoveTowards(transform.position, FinishDestination.position, MovementSpeed * Time.deltaTime);
        yield return new WaitForSeconds(0.5f);
        MovementSpeed = 0;
        PlayerAnimator.SetTrigger("Idle");

    }
    void AttackToEnemy()
    {
        transform.LookAt(TargetEnemy);
        PlayerAnimator.SetTrigger("MeeleAttack");
     
    }


    void FollowCreatedPath()
    {
        if (WayPoints.Count > 0)
        {
            transform.LookAt(WayPoints[CurrentWayPointIndex]);
            transform.position = Vector3.MoveTowards(transform.position, WayPoints[CurrentWayPointIndex], MovementSpeed * Time.deltaTime);

            if (transform.position == WayPoints[CurrentWayPointIndex])
            {
                if (CurrentWayPointIndex < WayPoints.Count - 1)
                    CurrentWayPointIndex++;
                if (CurrentWayPointIndex == WayPoints.Count - 1)
                {
                    IsLineCompleted = true;


                }
            }
        }
        if (WayPoints.Count == 0)
        {
            IsLineCompleted = true;
        }
            

    }

    void ReturnToIdleState()
    {
        PlayerAnimator.SetTrigger("Idle");
        WayPoints.Clear();
        CurrentWayPointIndex = 0;
        WayPointsCount = 0;
        IsTouchedToPlayer = false;
        LineRenderer.positionCount = 0;
        LineRenderer.enabled = false;
        IsLineCompleted = false;
        DrawTimer = 0.75f;
    }

    void LineCompleted()
    {
        IsLineCompleted = true;
        PlayerAnimator.SetTrigger("Idle");
    }

    void CheckTouchedToPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 250f))
        {
            
            if (hit.collider.tag == "Player")
            {
                IsTouchedToPlayer = true;
            }
        }
    }


    void DrawPath()
    {
        DrawTimer -= Time.deltaTime;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100));
        Vector3 direction = worldMousePos - Camera.main.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, direction, out hit, Mathf.Infinity))
        {
           
            if ((hit.collider.tag == "Ground" || hit.collider.tag == "FinishPoint")&& DrawTimer >0)
            {
                
                Vector3 new_way_point = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                if (!WayPoints.Contains(new_way_point))
                {
                    WayPoints.Add(new_way_point);
                    LineRenderer.positionCount = WayPointsCount + 1;
                    LineRenderer.SetPosition(WayPointsCount, new_way_point);
                    WayPointsCount++;
                    EventManager.TriggerEvent(GameConstants.GameEvents.NEW_WAYPOINT_CREATED, new EventParam());
                }
              
               
            }
           
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            transform.LookAt(TargetEnemy);
            PlayerAnimator.SetTrigger("MeeleAttack");
            TargetEnemy = other.transform;
            EventParam new_event_param = new EventParam();
            new_event_param.TargetEnemy = other.transform.gameObject;
            EventManager.TriggerEvent(GameConstants.GameEvents.ATTACK_TO_ENEMY, new_event_param);
        }

        if(other.tag == "FinishPoint" && CanFinishTheLevel)
        {
            
            FinishDestination = other.transform.parent.GetChild(0);
            Destroy(other.gameObject);
            PlayerState = PlayerStates.Success;
            EventManager.TriggerEvent(GameConstants.LEVEL_EVENTS.LEVEL_SUCCESSED, new EventParam());
        }
    }
}
