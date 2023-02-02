using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonComponent<LevelManager>
{
    [SerializeField] GameObject[] LevelPrefabs;
    int CurrentLevelIndex=0;
    GameObject CurrentLevel;

    private void OnEnable()
    {
        EventManager.StartListening(GameConstants.LEVEL_EVENTS.LEVEL_FAILED, RestartLevel);
        EventManager.StartListening(GameConstants.LEVEL_EVENTS.LEVEL_SUCCESSED, LoadNextLevel);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameConstants.LEVEL_EVENTS.LEVEL_FAILED, RestartLevel);
        EventManager.StopListening(GameConstants.LEVEL_EVENTS.LEVEL_SUCCESSED, LoadNextLevel);
    }

    void LoadNextLevel(EventParam param)
    {
        StartCoroutine(LoadNextLevelCRT());
    }

    IEnumerator LoadNextLevelCRT()
    {
        GameObject next_level = CreateCurrentLevel((CurrentLevelIndex+1)%LevelPrefabs.Length);
        yield return new WaitForSeconds(3f);
        Destroy(CurrentLevel);
        next_level.SetActive(true);
        CurrentLevel = next_level;
        CurrentLevelIndex++;
    }

    void RestartLevel(EventParam param)
    {
        StartCoroutine(RestartLevelCRT());
    }

    IEnumerator RestartLevelCRT()
    {
        GameObject restart_level = CreateCurrentLevel((CurrentLevelIndex) % LevelPrefabs.Length);
        yield return new WaitForSeconds(5f);
        Destroy(CurrentLevel);
        restart_level.SetActive(true);
        CurrentLevel = restart_level;
        
    }


    void Start()
    {
        CurrentLevel = CreateCurrentLevel(CurrentLevelIndex);
        CurrentLevel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrenLevelIndex()
    {
        return CurrentLevelIndex;
    }

    public void NextLevel()
    {
        CurrentLevelIndex++;
    }

    GameObject CreateCurrentLevel(int level_index)
    {
        GameObject current_level = Instantiate(LevelPrefabs[level_index], Vector3.zero, Quaternion.identity);
        current_level.SetActive(false);
        return current_level;
    }
}
