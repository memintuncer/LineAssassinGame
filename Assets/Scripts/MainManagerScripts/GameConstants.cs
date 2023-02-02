using UnityEngine;

public class GameConstants
{
    
    public struct LEVEL_EVENTS
    {
       
        public static string OBJECTIVE_FAILED = "OBJECTIVE_FAILED";
        public static string LEVEL_FAILED = "LEVEL_FAILED";
        public static string LEVEL_SUCCESSED = "LEVEL_SUCCESSED";
        public static string LEVEL_STARTED = "LEVEL_STARTED";
        
        public static string LevelFinished = "LevelFinished";

        public static string REACHED_FINISH = "REACHED_FINISH";
    }

   
    public struct GameEvents
    {
       
        public static string GAME_STARTED = "GAME_STARTED";
        public static string PLAYER_ATTACK_STARTED = "PLAYER_ATTACK_STARTED";
        public static string PLAYER_ATTACK_ENDED = "PLAYER_ATTACK_ENDED";
        public static string PLAYER_IS_DEAD = "PLAYER_IS_DEAD";
        public static string PLAYER_IS_SHOOTED = "PLAYER_IS_SHOOTED";
        public static string ATTACK_TO_ENEMY = "ATTACK_TO_ENEMY";
        public static string COLLECT_DIAMOND = "COLLECT_DIAMOND";
        public static string NEW_WAYPOINT_CREATED = "NEW_WAYPOINT_CREATED";
        public static string PLAYER_IS_SPOTTED = "PLAYER_IS_SPOTTED";
        public static string OPEN_THE_DOOR = "OPEN_THE_DOOR";
        public static string COMPLETED_FOLLOWING_PATH = "COMPLETED_FOLLOWING_PATH";
        

    }


}