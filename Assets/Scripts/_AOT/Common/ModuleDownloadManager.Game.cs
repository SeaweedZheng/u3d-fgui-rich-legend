using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;


public partial class ModuleDownloadManager 
{
    /// <summary> 运行模块的哈希值 </summary>
    public Dictionary<string, string> runingModHash = new Dictionary<string, string>();




#if false
    public void HotDownloadGame(string moduleName, Action<object[]> onFinish)
            => StartCoroutine(CoHotDownloadGameAsset(moduleName, onFinish));


        IEnumerator CoHotDownloadGameAsset(string moduleName, Action<object[]> onFinish)
        {
            // 必须先检查是否允许热更新，猜呢调用这个接口
            bool isNest = false;
            bool isAllowDownload = false;
            bool isError = false;

            CheckGameHotDownload(moduleName, (res) =>
            {
                isAllowDownload = (int)res[0] == 0;

                isNest = true;

            });


            yield return new WaitUntil(() => isNest == true);
            isNest = false;

            if (!isAllowDownload)
            {
                onFinish.Invoke(new object[] { 2, "You need to restart the software to update the game." });
                yield break;
            }


            yield return DownLoadNeedModToTemp(moduleName,
                null,
                (startMsg) =>
                {
                    Debug.Log(startMsg);
                },
                (endMsg) =>
                {
                    Debug.Log(endMsg);
                },
                (ProgressMsg) =>
                {
                    Debug.Log(ProgressMsg);
                },
                (successMsg) =>
                {

                },
                (errorMsg) =>
                {
                    Debug.LogError(errorMsg);
                    isError = true;
                });

            if (isError)
            {
                onFinish?.Invoke(new object[] { 1, $"download mod {moduleName} fail" });
                yield break;
            }

            PlayerPrefs.SetString(HotfixState.HOTFIX_STATE, HotfixState.HotfixCopying);
            yield return CopyTempWebHotfixFileToTargetDir();


            onFinish?.Invoke(new object[] { 0 });
        }







        /// <summary>
        /// 【暂时不用】游戏是否允许
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="onFinish"></param>
        public void CheckGameHotDownload(string moduleName, Action<object[]> onFinish)
            => StartCoroutine(CoCheckGameHotDownload(moduleName, onFinish));

        IEnumerator CoCheckGameHotDownload(string moduleName, Action<object[]> onFinish)
        {
            string webPth = PathHelper.GetModuleVersionWEBURL(moduleName);

            JObject webModVerNode = null;

            // 非cdn加载
            UnityWebRequest reqModVerFile = UnityWebRequest.Get(webPth + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
            yield return reqModVerFile.SendWebRequest();
            if (reqModVerFile.result == UnityWebRequest.Result.Success)
            {
                string tvStr = reqModVerFile.downloadHandler.text;
                webModVerNode = JObject.Parse(tvStr);
            }
            else
            {
                onFinish?.Invoke(new object[] { 1, $"can not download remote mod ver file:{moduleName}" });
                yield break;
            }

            Dictionary<string, string> webModHashDis = new Dictionary<string, string>();

            JObject dependenciesNode = webModVerNode["dependencies"] as JObject;
            foreach (KeyValuePair<string, JToken> kv in dependenciesNode)
            {
                webModHashDis.Add(kv.Key, kv.Value["hash"].Value<string>());
            }
            webModHashDis.Add(webModVerNode["name"].Value<string>(), webModVerNode["hash"].Value<string>());


            foreach (KeyValuePair<string, string> kv in webModHashDis)
            {
                if (runingModHash.ContainsKey(kv.Key) && runingModHash[kv.Key] != kv.Value)
                {
                    onFinish?.Invoke(new object[] { 1, "The dependent module is already running" });
                    yield break;
                }
            }
            onFinish?.Invoke(new object[] { 0, "" });
        }


    /// <summary>
    /// 【暂时用不上】下载模块所需要大小
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void ChechDownloadTotalSize(string moduleName, Action<long> onSuccess, Action<string> onError)
            => StartCoroutine(CoChechDownloadTotalSize(moduleName, onSuccess, onError));

        IEnumerator CoChechDownloadTotalSize(string moduleName, Action<long> onSuccess, Action<string> onError)
        {
            string webPth = PathHelper.GetModuleVersionWEBURL(moduleName);

            JObject webModVerNode = null;

            // 非cdn加载
            UnityWebRequest reqModVerFile = UnityWebRequest.Get(webPth + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
            yield return reqModVerFile.SendWebRequest();
            if (reqModVerFile.result == UnityWebRequest.Result.Success)
            {
                string tvStr = reqModVerFile.downloadHandler.text;
                webModVerNode = JObject.Parse(tvStr);
            }
            else
            {
                Debug.LogError($"download web mod version file: {webPth}");
                onError?.Invoke($"download web mod version file");
                yield break;
            }

            JObject locModVerNode = null;
            string locPth = PathHelper.GetModuleVersionLOCPTH(moduleName);

            if (File.Exists(locPth))
            {
                string content = File.ReadAllText(locPth);
                locModVerNode = JObject.Parse(content);


                if (locModVerNode["hash"].Value<string>() == webModVerNode["hash"].Value<string>())
                {
                    onError?.Invoke("The game is already the latest version and does not require downloading");
                    yield break;
                }
            }

            List<string> locAssetHash = new List<string>();
            if (locModVerNode != null)
            {
                JObject manifestNode = locModVerNode["asset_bundle"]["manifest"] as JObject;
                locAssetHash.Add(manifestNode["hash"].Value<string>());

                JObject bundlesNode = locModVerNode["asset_bundle"]["bundles"] as JObject;
                foreach (KeyValuePair<string, JToken> kv in bundlesNode)
                {
                    locAssetHash.Add(kv.Value["hash"].Value<string>());
                }

                JObject backupNode = locModVerNode["asset_backup"] as JObject;
                foreach (KeyValuePair<string, JToken> kv in backupNode)
                {
                    locAssetHash.Add(kv.Value["hash"].Value<string>());
                }

                JObject dllNode = locModVerNode["asset_dll"] as JObject;
                foreach (KeyValuePair<string, JToken> kv in dllNode)
                {
                    locAssetHash.Add(kv.Value["hash"].Value<string>());
                }
            }

            Dictionary<string, int> webDifAssetSize = new Dictionary<string, int>();

            JObject manifestNode02 = webModVerNode["asset_bundle"]["manifest"] as JObject;
            if (!locAssetHash.Contains(manifestNode02["hash"].Value<string>()))
            {
                webDifAssetSize.Add(manifestNode02["hash"].Value<string>(), manifestNode02["size_bytes"].Value<int>());
            }

            JObject bundlesNode02 = webModVerNode["asset_bundle"]["bundles"] as JObject;
            foreach (KeyValuePair<string, JToken> kv in bundlesNode02)
            {
                string hash = kv.Value["hash"].Value<string>();
                if (!locAssetHash.Contains(hash))
                    webDifAssetSize.Add(hash, kv.Value["size_bytes"].Value<int>());
            }

            JObject backupNode02 = webModVerNode["asset_backup"] as JObject;
            foreach (KeyValuePair<string, JToken> kv in backupNode02)
            {
                string hash = kv.Value["hash"].Value<string>();
                if (!locAssetHash.Contains(hash))
                    webDifAssetSize.Add(hash, kv.Value["size_bytes"].Value<int>());
            }

            JObject dllNode02 = webModVerNode["asset_dll"] as JObject;
            foreach (KeyValuePair<string, JToken> kv in dllNode02)
            {
                string hash = kv.Value["hash"].Value<string>();
                if (!locAssetHash.Contains(hash))
                    webDifAssetSize.Add(hash, kv.Value["size_bytes"].Value<int>());
            }

            long totalSize = 0;
            foreach (KeyValuePair<string, int> item in webDifAssetSize)
            {
                totalSize += item.Value;
            }

            onSuccess?.Invoke(totalSize);
        }

#endif







    public object[] GetDownloadTotalSize(string moduleName, JObject webModVerNode)
        {
            JObject locModVerNode = null;
            string locPth = PathHelper.GetModuleVersionLOCPTH(moduleName);

            if (File.Exists(locPth))
            {
                string content = File.ReadAllText(locPth);
                locModVerNode = JObject.Parse(content);

                if (locModVerNode["hash"].Value<string>() == webModVerNode["hash"].Value<string>())
                {
                    return new object[] { 2, "The game is already the latest version and does not require downloading" };
                }
            }

            List<string> locAssetHash = new List<string>();
            if (locModVerNode != null)
            {
                JObject manifestNode = locModVerNode["asset_bundle"]["manifest"] as JObject;
                locAssetHash.Add(manifestNode["hash"].Value<string>());

                JObject bundlesNode = locModVerNode["asset_bundle"]["bundles"] as JObject;
                foreach (KeyValuePair<string, JToken> kv in bundlesNode)
                {
                    locAssetHash.Add(kv.Value["hash"].Value<string>());
                }

                JObject backupNode = locModVerNode["asset_backup"] as JObject;
                foreach (KeyValuePair<string, JToken> kv in backupNode)
                {
                    locAssetHash.Add(kv.Value["hash"].Value<string>());
                }

                JObject dllNode = locModVerNode["asset_dll"] as JObject;
                foreach (KeyValuePair<string, JToken> kv in dllNode)
                {
                    locAssetHash.Add(kv.Value["hash"].Value<string>());
                }
            }

            Dictionary<string, int> webDifAssetSize = new Dictionary<string, int>();

            JObject manifestNode02 = webModVerNode["asset_bundle"]["manifest"] as JObject;
            if (!locAssetHash.Contains(manifestNode02["hash"].Value<string>()))
            {
                webDifAssetSize.Add(manifestNode02["hash"].Value<string>(), manifestNode02["size_bytes"].Value<int>());
            }

            JObject bundlesNode02 = webModVerNode["asset_bundle"]["bundles"] as JObject;
            foreach (KeyValuePair<string, JToken> kv in bundlesNode02)
            {
                string hash = kv.Value["hash"].Value<string>();
                if (!locAssetHash.Contains(hash))
                    webDifAssetSize.Add(hash, kv.Value["size_bytes"].Value<int>());
            }

            JObject backupNode02 = webModVerNode["asset_backup"] as JObject;
            foreach (KeyValuePair<string, JToken> kv in backupNode02)
            {
                string hash = kv.Value["hash"].Value<string>();
                if (!locAssetHash.Contains(hash))
                    webDifAssetSize.Add(hash, kv.Value["size_bytes"].Value<int>());
            }

            JObject dllNode02 = webModVerNode["asset_dll"] as JObject;
            foreach (KeyValuePair<string, JToken> kv in dllNode02)
            {
                string hash = kv.Value["hash"].Value<string>();
                if (!locAssetHash.Contains(hash))
                    webDifAssetSize.Add(hash, kv.Value["size_bytes"].Value<int>());
            }

            long totalSize = 0;
            foreach (KeyValuePair<string, int> item in webDifAssetSize)
            {
                totalSize += item.Value;
            }

            return new object[] { 0, "need download", totalSize };
        }

    }




/// <summary>
/// 游戏是否允许游玩
/// </summary>
public partial class ModuleDownloadManager
{


    #region 游戏是否允许游玩

    /*
    /// <summary>
    /// 游戏是否在进入前可以热更
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="needHash"></param>
    /// <returns></returns>
    public bool CheckGameUpdateAtEnter(string moduleName, string needHash)
    {
        //该模块已经运行过并存在于缓存中
        if (runingModHash.ContainsKey(moduleName) && needHash != runingModHash[moduleName])
        { 
            return false;
        }

        string loclModVerPth = PathHelper.GetModuleVersionLOCPTH(moduleName);

        if (!File.Exists(loclModVerPth))
        {
            return true;
        }

        JObject locModVerNode = null;
        string content = File.ReadAllText(loclModVerPth);  //【存在bug】 这里total文件是个空！！
        locModVerNode = JObject.Parse(content);

        //检查依赖
        foreach (KeyValuePair<string, JToken> kv in (JObject)locModVerNode["dependencies"])
        {
            JObject node = kv.Value as JObject;

            if (!CheckGameUpdateAtEnter(kv.Key, node["hash"].Value<string>()))
                return false;
        }


        return true;
    }
    */

    /// <summary>
    /// 检查游戏是否能玩
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public bool CheckGamePlayable(string moduleName) => CheckLocalModCanRun(moduleName);

    const string NULL = nameof(NULL);
    const string UNDEFINE = nameof(UNDEFINE);

    /// <summary>
    /// 检查当前游戏是否可以允许（mod是否匹配） 
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="needHash"></param>
    /// <returns></returns>
    /// <remarks>
    /// * 游戏mod时可以热更新的覆盖的，所以不用将其信息缓存到内存中
    /// </remarks>
    /// 
    bool CheckLocalModCanRun(string moduleName, string needHash = null)
    {

        //该模块已经运行过并存在于缓存中
        if (runingModHash.ContainsKey(moduleName) && needHash != null)
        {
            return needHash == runingModHash[moduleName];
        }

        string loclModVerPth = PathHelper.GetModuleVersionLOCPTH(moduleName);

        if (!File.Exists(loclModVerPth))
        {
            return false;
        }

        JObject locModVerNode = null;

        string content = File.ReadAllText(loclModVerPth);  //【存在bug】 这里total文件是个空！！
        locModVerNode = JObject.Parse(content);

        string selfHash = locModVerNode["hash"].Value<string>();

        //检查自身
        if (needHash != null && selfHash != needHash)
            return false;

        //检查依赖
        foreach (KeyValuePair<string, JToken> kv in (JObject)locModVerNode["dependencies"])
        {
            JObject node = kv.Value as JObject;

            if (!CheckLocalModCanRun(kv.Key, node["hash"].Value<string>()))
                return false;
        }

        return true;
    }


    public void AddModeToRuning(int gameId)
    {
        if (!ApplicationSettings.Instance.isUseMoudle)
            return;

        string moduleName = LobbyGamesManager.Instance.GetGameValueFromServer<string>(gameId, "module_name");
        AddModeToRuning(moduleName);
    }

    /// <summary>
    /// 将已运行的主模块、游戏模块 加入到允许模块中
    /// </summary>
    /// <param name="moduleName"></param>
    public void AddModeToRuning(string moduleName)
    {
        if (!ApplicationSettings.Instance.isUseMoudle)
            return;

        if (string.IsNullOrEmpty(moduleName) || runingModHash.ContainsKey(moduleName))
            return;

        string loclModVerPth = PathHelper.GetModuleVersionLOCPTH(moduleName);

        if (!File.Exists(loclModVerPth))
        {
            return;
        }

        JObject locModVerNode = null;

        string content = File.ReadAllText(loclModVerPth);  //【存在bug】 这里total文件是个空！！
        locModVerNode = JObject.Parse(content);

        string selfHash = locModVerNode["hash"].Value<string>();
        runingModHash.Add(moduleName, selfHash);

        //检查依赖
        foreach (KeyValuePair<string, JToken> kv in (JObject)locModVerNode["dependencies"])
        {
            AddModeToRuning(kv.Key);
        }
    }

    #endregion



    /*
    /// <summary>
    /// 添加模块
    /// </summary>
    /// <param name="modName"></param>
    public void AddModInfo(string modName)
    {
        if (!runingModHash.ContainsKey(modName))
        {
            string locModPth = PathHelper.GetModuleVersionLOCPTH(modName);
            if (File.Exists(locModPth))
            {
                string content = File.ReadAllText(locModPth);
                JObject modNode = JObject.Parse(content);
                runingModHash.Add(modName, modNode["hash"].Value<string>());
            }
        }
    }
    */
}

