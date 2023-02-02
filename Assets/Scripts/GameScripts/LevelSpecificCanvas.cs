using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelSpecificCanvas : MonoBehaviour
{
    [SerializeField] LevelController LevelController;
    [SerializeField] GameObject EnemyCountInfoUI, DiamondCountInfoUI;
    [SerializeField] TextMeshProUGUI EnemyCountInfoText, DiamondCountInfoText, LevelInfoText;
    public int TotalEnemyCount, TotalDiamondCount,CurrentKilledEnemiesCount=0, CurrentCollectedDiamondsCount=0;
    LevelManager LevelManager;
    [SerializeField] Animator LevelEndAnimator;

   


    private void OnEnable()
    {
        EventManager.StartListening(GameConstants.GameEvents.ATTACK_TO_ENEMY, IncreaseKilledEnemyCount);
        EventManager.StartListening(GameConstants.GameEvents.COLLECT_DIAMOND, IncreaseCollecteddiamondCount);
        EventManager.StartListening(GameConstants.LEVEL_EVENTS.LEVEL_FAILED, LevelFailedFeedBack);
        EventManager.StartListening(GameConstants.LEVEL_EVENTS.LEVEL_SUCCESSED, LevelSuccessedFeedBack);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameConstants.GameEvents.ATTACK_TO_ENEMY, IncreaseKilledEnemyCount);
        EventManager.StopListening(GameConstants.GameEvents.COLLECT_DIAMOND, IncreaseCollecteddiamondCount); 
        EventManager.StopListening(GameConstants.LEVEL_EVENTS.LEVEL_FAILED, LevelFailedFeedBack);
        EventManager.StopListening(GameConstants.LEVEL_EVENTS.LEVEL_SUCCESSED, LevelSuccessedFeedBack);
    }

    void LevelFailedFeedBack(EventParam param)
    {
        LevelEndAnimator.SetTrigger("Fail");
    }
    void LevelSuccessedFeedBack(EventParam param)
    {
        LevelEndAnimator.SetTrigger("Success");
    }

    void IncreaseKilledEnemyCount(EventParam param)
    {
        CurrentKilledEnemiesCount++;
    }
     void IncreaseCollecteddiamondCount(EventParam param)
    {
        CurrentCollectedDiamondsCount++;
    }

    void Start()
    {
        LevelManager = LevelManager.Instance;
        TotalDiamondCount = LevelController.GetTotalDiamondsCount();
        TotalEnemyCount = LevelController.GetTotalEnemiesCount();
        
        if (TotalDiamondCount == 0)
        {
            DiamondCountInfoUI.SetActive(false);
        }
        if (TotalEnemyCount == 0)
        {
            EnemyCountInfoUI.SetActive(false);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        LevelInfoText.text = "LEVEL " + (LevelManager.GetCurrenLevelIndex() + 1).ToString();
        EnemyCountInfoText.text = CurrentKilledEnemiesCount.ToString() + "/" + LevelController.GetTotalEnemiesCount().ToString();
        DiamondCountInfoText.text = CurrentCollectedDiamondsCount.ToString() + "/"+ TotalDiamondCount.ToString();
    }
}
