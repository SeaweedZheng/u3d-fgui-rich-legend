using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LobbyGamesUtils
{
    
    public static List<int> GetVisiableGameIds()
    {

        List<int> gameIds = new List<int>();
        foreach (var game in LobbyGamesManager.Instance.lobbyGamesInfoLocal)
        {
           
            int gameId = game["game_id"].Value<int>();
            if (game["display_in_lobby"].Value<bool>() == true
                && LobbyGamesManager.Instance.GetSeverValue<bool>(gameId, "is_available") == true)
            {
                gameIds.Add(gameId);
            }
        }
        return gameIds;
    }

/*
    public static List<int> GetVisiableGameIds()
    {

        List<int> gameIds = new List<int>();
        foreach (var game in LobbyGamesManager.Instance.lobbyGamesInfoSever)
        {
            int gameId = game["game_id"].Value<int>();
            if (game["is_available"].Value<bool>() == true
                && LobbyGamesManager.Instance.GetLocalValue<bool>(gameId, "display_in_lobby") == true)
            {
                gameIds.Add(gameId);
            }
        }
        return gameIds;
    }*/
}
