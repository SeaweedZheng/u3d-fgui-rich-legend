using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


/*
public class LobbyGamesManager :  MonoBehaviour
{
    private static object _mutex = new object();
    static LobbyGamesManager _instance;

    public static LobbyGamesManager Instance
    {
        get
        {

            lock (_mutex)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LobbyGamesManager>();
                    // FindObjectOfType(typeof(DevicePrinterOut)) as DevicePrinterOut;
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        _instance = obj.AddComponent<LobbyGamesManager>();

                        obj.name = _instance.GetType().Name;
                        if (obj.transform.parent == null)
                        {
                            DontDestroyOnLoad(obj);
                        }
                    }
                }
                return _instance;
            }
        }
    }

    */



public  class LobbyGamesManager
{
    private static object _mutex = new object();
    static LobbyGamesManager _instance;

    public static LobbyGamesManager Instance
    {
        get
        {
            lock (_mutex)
            {
                if (_instance == null)
                {
                    _instance = new LobbyGamesManager();
                }
                return _instance;
            }
        }
    }



    /// <summary>
    /// 远端游戏信息文件
    /// </summary>
    /// <remarks>
    /// * 本地没有的话，获取SA里的数据
    /// * 不支持用户进行修改
    /// 
    /// </remarks>
    public JArray lobbyGamesInfoSever => _lobbyGamesInfoSever;
    JArray _lobbyGamesInfoSever = null;


    /// <summary>
    /// 本地游戏信息文件，支持用户进行修改。
    /// </summary>
    /// <remarks>
    /// * 以服务器游戏信息文件为基础
    /// </remarks>
    public JArray lobbyGamesInfoLocal
    {
        get
        {
            if (_lobbyGamesInfoLocal == null)
            {
                string jsn = PlayerPrefs.GetString(PARAM_LOBBY_GAMES, JsonConvert.SerializeObject(lobbyGamesInfoSever));
                _lobbyGamesInfoLocal = JArray.Parse(jsn);
            }
            return _lobbyGamesInfoLocal;
        }
        set
        {
            _lobbyGamesInfoLocal = value;
        }
    }
    JArray _lobbyGamesInfoLocal = null;
    const string PARAM_LOBBY_GAMES = nameof(PARAM_LOBBY_GAMES);



    const string assetLobbyGameFile = "Assets/AstBackup/Lobbys/Game Info/lobby_games.json";




    public  void LoadLobbyGamesInfoWhenHotfix(Action<object[]> onFinishCallback = null)
    {


        string pthTemp = PathHelper.GetTempAssetBackupLOCPTH(assetLobbyGameFile);
        string pthLocal = PathHelper.GetAssetBackupLOCPTH(assetLobbyGameFile);

        if (File.Exists(pthTemp))
        {
            string strs = File.ReadAllText(pthTemp, Encoding.UTF8);
            _lobbyGamesInfoSever = JArray.Parse(strs);
        }
        else if (File.Exists(pthLocal))
        {
            string strs = File.ReadAllText(pthLocal, Encoding.UTF8);
            _lobbyGamesInfoSever = JArray.Parse(strs);
        }
        else
        {
            onFinishCallback.Invoke(new object[] {1, $"can not find file: {assetLobbyGameFile}"});
        }

        ChangeLocalInfo(null); //  可能热更不成功
        onFinishCallback?.Invoke(new object[] { 0});
    }

    /*
    public async void LoadLobbyGamesInfo(Action onFinishCallback = null)
    {

        string pthLocal = PathHelper.GetAssetBackupLOCPTH(assetLobbyGameFile);

        if(File.Exists(pthLocal))
        {
            string strs = File.ReadAllText(pthLocal, Encoding.UTF8);
            _lobbyGamesInfoSever = JArray.Parse(strs);
        }
        else{
            string pthSA = PathHelper.GetAssetBackupSAPTH(assetLobbyGameFile);

            byte[] bytes = await StreamingAssetsUtils.LoadAssetAsync(pthSA);

            string jsnStr = Encoding.UTF8.GetString(bytes);
            _lobbyGamesInfoSever = JArray.Parse(jsnStr);        
        }

        //SyncLocalInfo();
        onFinishCallback?.Invoke();
    }*/


    public async void LoadLobbyGamesInfoSA(Action onFinishCallback = null)
    {
        string pthSA = PathHelper.GetAssetBackupSAPTH(assetLobbyGameFile);

        byte[] bytes = await StreamingAssetsUtils.LoadAssetAsync(pthSA);

        string jsnStr = Encoding.UTF8.GetString(bytes);
        _lobbyGamesInfoSever = JArray.Parse(jsnStr);

        //SyncLocalInfo();
        onFinishCallback?.Invoke();
    }





    public void SaveLobbyGameInfoAndHash()
    {
        SaveLobbyGameInfo();
        PlayerPrefs.SetString(PARAM_LOBBY_GAMES_KEYS_HASH, curHash);
    }

    public void SaveLobbyGameInfo()
    {
        PlayerPrefs.SetString(PARAM_LOBBY_GAMES, JsonConvert.SerializeObject(_lobbyGamesInfoLocal));
        //PlayerPrefs.Save();
    }


    string GetSeverHash()
    {
        int hash = JsonConvert.SerializeObject(lobbyGamesInfoSever).GetHashCode();
        return $"{hash}";
        /*
        string content = $"{lobbyGamesInfoSever.Count}";
        if (lobbyGamesInfoSever.Count > 0)
        {
            JObject target = lobbyGamesInfoSever[0] as JObject;
            foreach (KeyValuePair<string, JToken> kv in target)
            {
                content += kv.Key;
            }
        }

        string md5 = FileUtils.ComputeMD5ForStr(content);
        return md5;
        */
    }

    string GetLocHash()
    {
        int hash = JsonConvert.SerializeObject(lobbyGamesInfoLocal).GetHashCode();
        return $"{hash}";
    }

    string curHash = "";
    public void ChangeLocalInfo(Action onChangeCallback)
    {
        curHash = GetSeverHash();

        string lastHash = PlayerPrefs.GetString(PARAM_LOBBY_GAMES_KEYS_HASH, GetLocHash());
        if (curHash != lastHash)
        {
            JArray newInfo = JArray.Parse(JsonConvert.SerializeObject(lobbyGamesInfoSever));  //拷贝

            Dictionary<int, JObject> dic = new Dictionary<int, JObject>();
            foreach (JToken node in lobbyGamesInfoLocal)
            {
                dic.Add(node["game_id"].Value<int>(), node as JObject);
            }


            foreach (JToken node in newInfo)
            {
                JObject obj = node as JObject;
                int gameId = obj["game_id"].Value<int>();
                if (dic.ContainsKey(gameId))
                {
                    foreach (KeyValuePair<string, JToken> kv in obj)
                    {
                        if (dic[gameId].ContainsKey(kv.Key))
                        {
                            obj[kv.Key] = dic[gameId][kv.Key];
                        }
                    }
                }
            }
            lobbyGamesInfoLocal = newInfo;

            onChangeCallback?.Invoke();
        }
    }

#if false
    public void SyncLocalInfo()
    {
        /*
        string curMd5 = GetKeysMD5();
        string lastMd5 = PlayerPrefs.GetString(PARAM_LOBBY_GAMES_KEYS_HASH, curMd5);
        if (curMd5 != lastMd5)
        {
            JArray newInfo = JArray.Parse(JsonConvert.SerializeObject(lobbyGamesInfoSever));  //拷贝

            Dictionary<int, JObject> dic = new Dictionary<int, JObject>();
            foreach (JToken node in lobbyGamesInfoLocal)
            {
                dic.Add(node["game_id"].Value<int>(), node as JObject);
            }


            foreach (JToken node in newInfo)
            {
                JObject obj = node as JObject;
                int gameId = obj["game_id"].Value<int>();
                if (dic.ContainsKey(gameId))
                {
                    foreach (KeyValuePair<string,JToken>kv in  obj)
                    {
                        if (dic[gameId].ContainsKey(kv.Key))
                        {
                            obj[kv.Key] = dic[gameId][kv.Key];
                        }
                    }
                }
            }
            lobbyGamesInfoLocal = newInfo;

            PlayerPrefs.SetString(PARAM_LOBBY_GAMES_KEYS_HASH, curMd5);
        }
        */

        ChangeLocalInfo(() =>
        {
            SaveLobbyGameInfo();
            PlayerPrefs.SetString(PARAM_LOBBY_GAMES_KEYS_HASH, curHash);
        });
    }
#endif

    const string PARAM_LOBBY_GAMES_KEYS_HASH = nameof(PARAM_LOBBY_GAMES_KEYS_HASH);



    public bool isAllowChangeFlag(int gameId, string key)
    {
        JObject nodeSever = null;
        for (int i = 0; i < lobbyGamesInfoSever.Count; i++)
        {
            JObject temp = lobbyGamesInfoSever[i] as JObject;
            if (temp["game_id"].Value<int>() == gameId)
            {
                nodeSever = temp;
                break;
            }
        }
        if (nodeSever == null || !nodeSever.ContainsKey(key))
            return false;

        try
        {
            if (nodeSever[key].Value<bool>() == false)
            {
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            return false;  // “字符串，数字，浮点值” 要以服务器为主
        }

    }


    public bool? GetFlagValue(int gameId, string key)
    {
        JObject nodeSever = null;
        for(int i=0; i< lobbyGamesInfoSever.Count;i++)
        {
            JObject temp = lobbyGamesInfoSever[i] as JObject;
            if (temp["game_id"].Value<int>() == gameId)
            {
                nodeSever = temp;
                break;
            }
        }
        if(nodeSever == null || !nodeSever.ContainsKey(key))
             return null;

        try
        {
            if (nodeSever[key].Type != JTokenType.Null && nodeSever[key].Value<bool>() == false) // 非null 非true
            {
                return false;
            }

            JObject nodeLocal = null;
            for (int i = 0; i < lobbyGamesInfoLocal.Count; i++)
            {
                JObject temp = lobbyGamesInfoLocal[i] as JObject;
                if (temp["game_id"].Value<int>() == gameId)
                {
                    nodeLocal = temp;
                    break;
                }
            }

            return nodeLocal[key].Value<bool>();
        }
        catch (Exception e)
        {
            return null;  
        }

    }

    /*
    public bool HasLocalKey(int gameId, string key)
    {

    }*/

    public T GetLocalValue<T>(int gameId, string key) //where T : class
    {
        JObject nodeLocal = null;
        for (int i = 0; i < lobbyGamesInfoLocal.Count; i++)
        {
            JObject temp = lobbyGamesInfoLocal[i] as JObject;
            if (temp["game_id"].Value<int>() == gameId)
            {
                nodeLocal = temp;
                break;
            }
        }
        if (nodeLocal == null || !nodeLocal.ContainsKey(key))
            return default(T); // bool默认false，int默认0，引用类型默认null

        return nodeLocal[key].Value<T>();

    }

    public void SetLocalValue<T>(int gameId, string key, T value) //where T : class
    {
        JObject nodeLocal = null;
        for (int i = 0; i < lobbyGamesInfoLocal.Count; i++)
        {
            JObject temp = lobbyGamesInfoLocal[i] as JObject;
            if (temp["game_id"].Value<int>() == gameId)
            {
                nodeLocal = temp;
                break;
            }
        }
        if (nodeLocal != null && nodeLocal.ContainsKey(key))
        {
            JToken jTokenValue = JToken.FromObject(value);// 核心解决：将T类型转换为JToken（使用JToken.FromObject，支持任意T类型）
            nodeLocal[key] = jTokenValue;

            Debug.Log($"data: {JsonConvert.SerializeObject(lobbyGamesInfoLocal)}");
            SaveLobbyGameInfo();
        }
    }

    public JObject GetLocalNode(int gameId)
    {
        JObject nodeSever = null;
        for (int i = 0; i < lobbyGamesInfoLocal.Count; i++)
        {
            JObject temp = lobbyGamesInfoLocal[i] as JObject;
            if (temp["game_id"].Value<int>() == gameId)
            {
                nodeSever = temp;
                break;
            }
        }
        return nodeSever;
    }

    public JObject GetSeverNode(int gameId)
    {
        JObject nodeSever = null;
        for (int i = 0; i < lobbyGamesInfoSever.Count; i++)
        {
            JObject temp = lobbyGamesInfoSever[i] as JObject;
            if (temp["game_id"].Value<int>() == gameId)
            {
                nodeSever = temp;
                break;
            }
        }
        return nodeSever;
    }
    public T GetSeverValue<T>(int gameId, string key) //where T : class
    {

        JObject nodeSever = null;
        for (int i = 0; i < lobbyGamesInfoSever.Count; i++)
        {
            JObject temp = lobbyGamesInfoSever[i] as JObject;
            if (temp["game_id"].Value<int>() == gameId)
            {
                nodeSever = temp;
                break;
            }
        }
        if (nodeSever == null || !nodeSever.ContainsKey(key))
            return default(T); // bool默认false，int默认0，引用类型默认null

        return nodeSever[key].Value<T>();

    }




}
