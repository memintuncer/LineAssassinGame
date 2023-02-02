using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] Transform AllEnemies, AllDiamonds;
    public int TotalEnemiesCount, TotalDiamondsCount, CurrentKilledEnemisCount=0, CurrentCollectedDiamondsCount=0;
    [SerializeField] DoorController DoorController;

    private void OnEnable()
    {
        EventManager.StartListening(GameConstants.GameEvents.ATTACK_TO_ENEMY, IncreaseKilledEnemyCount);
        EventManager.StartListening(GameConstants.GameEvents.COLLECT_DIAMOND, IncreaseCollecteddiamondCount);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameConstants.GameEvents.ATTACK_TO_ENEMY, IncreaseKilledEnemyCount);
        EventManager.StopListening(GameConstants.GameEvents.COLLECT_DIAMOND, IncreaseCollecteddiamondCount);
    }


    void IncreaseKilledEnemyCount(EventParam param)
    {
        CurrentKilledEnemisCount++;
        CheckDoorCanBeOpened();
    }
    void IncreaseCollecteddiamondCount(EventParam param)
    {
        CurrentCollectedDiamondsCount++;
        CheckDoorCanBeOpened();
    }


    void CheckDoorCanBeOpened()
    {
        if(CurrentKilledEnemisCount == TotalEnemiesCount && CurrentCollectedDiamondsCount == TotalDiamondsCount)
        {
            DoorController.OpenTheDoor();
        }
    }
    void Start()
    {
        TotalEnemiesCount = AllEnemies.childCount;
        TotalDiamondsCount = AllDiamonds.childCount;
        CheckDoorCanBeOpened();
    }

   

    public int GetTotalEnemiesCount()
    {
        return TotalEnemiesCount;
    }
     public int GetTotalDiamondsCount()
    {
        return AllDiamonds.childCount;
    }


   
}
