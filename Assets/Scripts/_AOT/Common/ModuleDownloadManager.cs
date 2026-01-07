using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public partial class ModuleDownloadManager : MonoBehaviour
{

    static ModuleDownloadManager instance;
    public static ModuleDownloadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(ModuleDownloadManager)) as ModuleDownloadManager;
            }
            return instance;
        }
    }


    /// <summary> 运行模块的哈希值 </summary>
    Dictionary<string,string> runingModHash = new Dictionary<string,string>();

    public IEnumerator DownLoadWebMod(string moduleName,
    Action<string> onDownloadStart, Action<string> onDownloadEnd, Action<string> onProgress,
    Action<string> onSuccess, Action<string> onError)
    {

        //下载远程modeVer 文件到内存
        string modVversionFileWebUrl = PathHelper.GetModuleVersionWEBURL(moduleName);

        JObject webModVerNode = null;

        // 非cdn加载
        UnityWebRequest reqModVerFile = UnityWebRequest.Get(modVversionFileWebUrl + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
        yield return reqModVerFile.SendWebRequest();
        if (reqModVerFile.result == UnityWebRequest.Result.Success)
        {
            string tvStr = reqModVerFile.downloadHandler.text;
            webModVerNode = JObject.Parse(tvStr);
        }
        else
        {
            onError?.Invoke($"download mod ver file fail. url:{modVversionFileWebUrl}");
            yield break;
        }

        if (webModVerNode != null)  // 有网络
        {

        }
        else {   // 无网络
        
        }


    }





    /// <summary>
    /// 
    /// </summary>
    /// <param name="moduleName"></param>
    /// <remarks>
    /// *本地已存在版本是否已是远端版本？
    /// *下载远程modeVer 文件到内存
    /// *对比本地和远程的版本文件hash
    /// *下载依赖
    /// *下载dll
    /// *下载ab
    /// *下载backup
    /// *保存版本文件到临时目录
    /// *[X] 拷贝版本文件
    /// ==================
    /// *不进行拷贝，等所有该下载的文件下载完成，才进行拷贝
    /// (因为拷贝失败，也是要求能进入机台继续玩。不同于手机网络游戏)
    /// </remarks>
    public IEnumerator DownLoadNeedModToTemp(string moduleName, string needHash,
        Action<string> onDownloadStart, Action<string> onDownloadEnd, Action<string> onProgress,
        Action<string> onSuccess, Action<string> onError)
    {

        bool isError = false;
        string msg = "";

        Action<string> errorFunc = (m) =>
        {
            isError = true;
            msg = m;
        };


        //本地版本满足要求的版本则无需下载
        string loclModVerPth = PathHelper.GetModuleVersionLOCPTH(moduleName);
        JObject locModVerNode = null;
        if (
            !string.IsNullOrEmpty(needHash) 
            && File.Exists(loclModVerPth)
            )
        {
            string content = File.ReadAllText(loclModVerPth);  //【存在bug】 这里total文件是个空！！
            locModVerNode = JObject.Parse(content);

            if (locModVerNode["hash"].Value<string>() == needHash)
            {
                //检查依赖
                foreach (KeyValuePair<string, JToken> kv in (JObject)locModVerNode["dependencies"])
                {
                    JObject node = kv.Value as JObject;

                    yield return DownLoadNeedModToTemp(kv.Key, node["hash"].Value<string>(),
                            onDownloadStart, onDownloadEnd, onProgress,
                            null,
                            errorFunc);
                }

                if (isError)
                {
                    onError?.Invoke(msg);
                    yield break;
                }

                onSuccess?.Invoke($"finish mod check: {moduleName}");
                yield break;
            }
        }


        //下载远程modeVer 文件到内存
        string modVversionFileWebUrl = PathHelper.GetModuleVersionWEBURL(moduleName);

        JObject webModVerNode = null;

        // 非cdn加载
        UnityWebRequest reqModVerFile = UnityWebRequest.Get(modVversionFileWebUrl + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
        yield return reqModVerFile.SendWebRequest();
        if (reqModVerFile.result == UnityWebRequest.Result.Success)
        {
            string tvStr = reqModVerFile.downloadHandler.text;
            webModVerNode = JObject.Parse(tvStr);
        }
        else
        {
            onError?.Invoke($"download mod ver file fail. url:{modVversionFileWebUrl}");
            yield break;
        }

        // 远端hash不匹配
        if (!string.IsNullOrEmpty(needHash) && webModVerNode["hash"].Value<string>() != needHash)
        {
            onError?.Invoke($"web mod version hash is not match: {moduleName}  need hash:{needHash}");
            yield break;
        }


        //下载依赖
        foreach (KeyValuePair<string,JToken> kv in  (JObject)webModVerNode["dependencies"])
        {
            JObject node = kv.Value as JObject;

            yield return DownLoadNeedModToTemp(kv.Key, node["hash"].Value<string>(),
                    onDownloadStart, onDownloadEnd, onProgress,
                    null, errorFunc);
        }

        if (isError)
        {
            onError?.Invoke(msg);
            yield break;
        }


        onDownloadStart?.Invoke($"start download mod :{moduleName}");


        // 下载dll
        JObject localAssetDllNode = locModVerNode != null? (locModVerNode["asset_dll"] as JObject) : null;
        JObject webAssetDllNode = webModVerNode["asset_dll"] as JObject;
        if (webAssetDllNode != null && webAssetDllNode.Count >0)
        {
            foreach (KeyValuePair<string, JToken> kv in webAssetDllNode)
            {
                string targetHash = (kv.Value["hash"]).Value<string>();
                if (localAssetDllNode != null
                    && localAssetDllNode.ContainsKey(kv.Key)
                    && localAssetDllNode[kv.Key].Value<string>() == targetHash)
                {
                    continue;
                }

                yield return DownloadAssetOnce( 
                    PathHelper.GetDllWEBURL(kv.Key),
                    targetHash,
                    PathHelper.GetTempDllLOCPTH(kv.Key),
                    (str) => { }, errorFunc);
            }
        }

        if (isError)
        {
            onError?.Invoke(msg);
            yield break;
        }

        // 下载ab
        JObject locAssetbundleNode = locModVerNode != null ? 
            (locModVerNode["asset_bundle"]["bundles"] as JObject) : null;
        JObject webAssetbundleNode = webModVerNode["asset_bundle"]["bundles"] as JObject;
        if (webAssetbundleNode != null && webAssetbundleNode.Count > 0)
        {
            foreach (KeyValuePair<string, JToken> kv in webAssetbundleNode)
            {

                string targetHash = (kv.Value["hash"]).Value<string>();
                if (locAssetbundleNode != null
                    && locAssetbundleNode.ContainsKey(kv.Key)
                    && locAssetbundleNode[kv.Key].Value<string>() == targetHash)
                {
                    continue;
                }


                yield return DownloadAssetOnce(
                    PathHelper.GetAssetBundleWEBPTH(kv.Key),
                    targetHash,
                    PathHelper.GetTempAssetBundleLOCPTH(kv.Key),
                    (str) => { }, errorFunc);
            }
        }

        if (isError)
        {
            onError?.Invoke(msg);
            yield break;
        }

        // 下载backup
        JObject locAssetbackupNode = locModVerNode != null ?
            (locModVerNode["asset_backup"] as JObject) : null;
        JObject webAssetbackupNode = webModVerNode["asset_backup"]as JObject;
        if (webAssetbackupNode != null && webAssetbackupNode.Count > 0)
        {
            foreach (KeyValuePair<string, JToken> kv in webAssetbackupNode)
            {

                string targetHash = (kv.Value["hash"]).Value<string>();
                if (locAssetbackupNode != null
                    && locAssetbackupNode.ContainsKey(kv.Key)
                    && locAssetbackupNode[kv.Key].Value<string>() == targetHash)
                {
                    continue;
                }

                yield return DownloadAssetOnce(
                    PathHelper.GetAssetBackupWEBURL(kv.Key),
                    targetHash,
                    PathHelper.GetTempAstBackupLOCPTH(kv.Key),
                    (str) => { }, errorFunc);
            }
        }

        if (isError)
        {
            onError?.Invoke(msg);
            yield break;
        }

        // “模块版本”文件写入临时缓存路劲中
        FileUtils.WriteAllText(PathHelper.GetTmpModuleVersionLOCPTH(moduleName), webModVerNode.ToString());

        onSuccess?.Invoke($"finish download mod: {moduleName}");
    }





    /// <summary>
    /// 查看是否有新热更完整的资源缓存
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// * 每次最后，都有删除临时缓存
    /// </remarks>
    public  IEnumerator CopyTempWebHotfixFileToTargetDir()
    {
        // 是否有热更新文件需要拷贝
        if (PlayerPrefs.HasKey(HotfixState.HOTFIX_STATE) && PlayerPrefs.GetString(HotfixState.HOTFIX_STATE) == HotfixState.HotfixCopying)
        {
            //开始拷贝
            yield return FileUtils.CopyDirectoryAsync(PathHelper.tmpHotfixDirLOCPTH, PathHelper.hotfixDirLOCPTH);

            PlayerPrefs.SetString(HotfixState.HOTFIX_STATE, HotfixState.HotfixCompleted);
        }

        // 删除缓存
        if (Directory.Exists(PathHelper.tmpHotfixDirLOCPTH))
        {
            yield return FileUtils.DeleteDirectoryAsync(PathHelper.tmpHotfixDirLOCPTH);
        }
    }



    string throwErrMsg = "";

    private IEnumerator DownloadAssetOnce(string url, string needMd5, string savePth, Action<string> onDownloadProgress, Action<string> onError)
    {

        onDownloadProgress?.Invoke($"download asset: {url}");

        /*
        if (File.Exists(savePth))
        {
            if (needMd5 == FileUtils.CalculateFileMD5(savePth))
            {
                yield break;
            }
        }
        */

        // cdn 加载
        UnityWebRequest req = UnityWebRequest.Get(url); //ApplicationSettings.Instance.hotfixUrl
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            string md5 = FileUtils.CalculateMD5(req.downloadHandler.data);

            if (md5 == needMd5)
            {
                FileUtils.WriteAllBytes(savePth, req.downloadHandler.data);
                yield break;
            }
        }

        // 非cdn加载
        UnityWebRequest req01 = UnityWebRequest.Get(url + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
        yield return req01.SendWebRequest();
        if (req01.result == UnityWebRequest.Result.Success)
        {
            string md5 = FileUtils.CalculateMD5(req01.downloadHandler.data);

            if (md5 == needMd5)
            {
                FileUtils.WriteAllBytes(savePth, req01.downloadHandler.data);

                //【这块先隐藏】 s_assetDatas[dallame] = req01.downloadHandler.data;
                yield break;
            }
        }

        //

        throwErrMsg = $"download asset '{url}' is fail";
        //throw new Exception(throwErrMsg);

        PlayerPrefs.SetString(HotfixState.HOTFIX_STATE, HotfixState.HotfixDownloadFail);
        onError?.Invoke(throwErrMsg);
    }










    // 热更AB资源
    public IEnumerator DownloadManifestToTemp(Action<string> onSuccess, Action<string> onError)
    {

        using (UnityWebRequest req = UnityWebRequest.Get(PathHelper.mainfestWEBURL + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"))
        {
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError)
            {
                Debug.LogError(req.error);
                onError?.Invoke(req.error);
                yield break;
            }
            else
            {
                FileUtils.WriteAllBytes(PathHelper.tmpMainfestLOCPTH, req.downloadHandler.data);

                onSuccess?.Invoke(null);
            }
        }
    }


}


/// <summary>
/// 游戏是否允许热更新
/// </summary>
public partial class ModuleDownloadManager
{

    public void HotDownloadGame(string moduleName, Action<object[]> onFinish)
        => StartCoroutine(CoHotDownloadGameAsset(moduleName, onFinish));
 

    IEnumerator CoHotDownloadGameAsset(string moduleName, Action<object[]> onFinish)
    {
        // 必须先检查是否允许热更新，猜呢调用这个接口
        bool isNest = false;
        bool isAllowDownload = false;
        bool isError = false;

        ChecGameHotDownload(moduleName, (res) =>
        {
            isAllowDownload = (int)res[0] == 0;

            isNest = true;

        });


        yield return new WaitUntil(() => isNest == true);
        isNest = false;

        if (!isAllowDownload)
        {
            onFinish.Invoke(new object[] {2, "You need to restart the software to update the game." });
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
            onFinish?.Invoke(new object[] {1,$"download mod {moduleName} fail" });
            yield break;
        }

        PlayerPrefs.SetString(HotfixState.HOTFIX_STATE, HotfixState.HotfixCopying);
        yield return CopyTempWebHotfixFileToTargetDir();


        onFinish?.Invoke(new object[] { 0 });
    }


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
                runingModHash.Add(modName,modNode["hash"].Value<string>());
            }
        }
    }


    /// <summary>
    /// 游戏是否允许
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="onFinish"></param>
    public void ChecGameHotDownload(string moduleName, Action<object[]> onFinish)
        => StartCoroutine(CoChecGameHotDownload(moduleName, onFinish));


    IEnumerator CoChecGameHotDownload(string moduleName, Action<object[]> onFinish)
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


        foreach (KeyValuePair<string,string>kv in webModHashDis)
        {
            if (runingModHash.ContainsKey(kv.Key) && runingModHash[kv.Key] != kv.Value)
            {
                onFinish?.Invoke(new object[] { 1, "The dependent module is already running" });
                yield break ;
            }
        }
        onFinish?.Invoke(new object[] {0 });
    }



    /// <summary>
    /// 下载模块所需要大小
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


            if (locModVerNode["hash"].Values<string>() == webModVerNode["hash"].Values<string>())
            {
                onError?.Invoke("The game is already the latest version and does not require downloading");
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



}




/// <summary>
/// 游戏是否允许游玩
/// </summary>
public partial class ModuleDownloadManager
{


    #region 游戏是否允许游玩

    /// <summary>
    /// 检查游戏是否能玩
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public bool CheckGamePlayable(string moduleName)
    {
        if (checkedGameMode.Contains(moduleName))
            return true;
        bool isOk = CheckLocalMod(moduleName);

        if (isOk)
            checkedGameMode.Add(moduleName);

        return isOk;
    }
    /// <summary>
    /// 已经检查过，能玩的游戏
    /// </summary>
    List<string> checkedGameMode = new List<string>();


    const string NULL = nameof(NULL);
    const string UNDEFINE = nameof(UNDEFINE);

    /// <summary>
    /// 本地模块的hash值
    /// </summary>
    Dictionary<string, string> localModHash = new Dictionary<string, string>();

    /// <summary>
    /// 检查mod是否允许执行
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="needHash"></param>
    /// <returns></returns>
    bool CheckLocalMod(string moduleName, string needHash = null)
    {
        string loclModVerPth = PathHelper.GetModuleVersionLOCPTH(moduleName);

        if (!localModHash.ContainsKey(moduleName))
            localModHash.Add(moduleName, UNDEFINE);

        if (!File.Exists(loclModVerPth))
        {
            localModHash[moduleName] = NULL;
            return false;
        }

        if (localModHash[moduleName] != UNDEFINE)
        {
            if (needHash != null && localModHash[moduleName] != needHash)
                return false;
        }

        JObject locModVerNode = null;

        string content = File.ReadAllText(loclModVerPth);  //【存在bug】 这里total文件是个空！！
        locModVerNode = JObject.Parse(content);

        string selfHash = locModVerNode["hash"].Value<string>();
        localModHash[moduleName] = selfHash;

        //检查自身
        if (needHash != null && selfHash != needHash)
            return false;

        //检查依赖
        foreach (KeyValuePair<string, JToken> kv in (JObject)locModVerNode["dependencies"])
        {
            JObject node = kv.Value as JObject;

            if (CheckLocalMod(kv.Key, node["hash"].Value<string>()))
                return false;
        }

        return true;
    }

    #endregion

}
