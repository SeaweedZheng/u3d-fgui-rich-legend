#define NEW_VER_DLL
using FairyGUI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;


/// <summary>
/// 
/// </summary>
/// <remarks>
/// * 问题：total_version文件文件内容为空（下载并写入total_version文件，写到一半突然断电。）
/// </remarks>

public class ModulesVersionCheck : MonoBehaviour
{

    static ModulesVersionCheck instance;
    public static ModulesVersionCheck Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(ModulesVersionCheck)) as ModulesVersionCheck;
            }
            return instance;
        }
    }


    private void OnDestroy()
    {
        CloseCoCheckHotFixThrowErr();
    }


    const string PREFIX_TIP = "【Hotfix Step】";


    /// <summary> 总版本管理-子节点数据 </summary>


    JObject webMainModVersionFileNode;
    JObject webVersionFileNode;






#if false
    /// <summary> 获取远端资源备份的hash /// </summary>
    string GetWebAstBackupHash(string nodeName) => mainModVersionFileRemoteNode["asset_backup"][nodeName]["hash"].ToObject<string>();

    /// <summary> 获取本地dll文件的hash  </summary>
    string GetLocalAstBackupHash(string nodeName) => GlobalData.version["asset_backup"][nodeName]["hash"].ToObject<string>();

    /// <summary>
    /// 获取远端dll文件的hash
    /// </summary>
    /// <param name="dllName"></param>
    /// <returns></returns>
    string GetWebHotfixDllHash(string dllName) => mainModVersionFileRemoteNode["asset_dll"][dllName]["hash"].ToObject<string>();



    /// <summary>
    /// 获取本地dll文件的hash
    /// </summary>
    /// <param name="dllName"></param>
    /// <returns></returns>
    string GetLocalHotfixDllHash(string dllName) => GlobalData.version["asset_dll"][dllName]["hash"].ToObject<string>();

#endif


    void GetLocalVersionInfo()
    {
        string showMsg = $"read local version file   path: {PathHelper.mainModVersionLOCPTH}";
        PageLaunch.Instance.RefreshProgressUIMsg(showMsg);


        Debug.LogWarning($"{PREFIX_TIP} {showMsg}");

        string versionText = "";
        string mainModVersionText = "";
        string totalVersionText = "";


        // 获取主版本信息
        try
        {
            versionText = File.ReadAllText(PathHelper.versionLOCPTH);
            GlobalModel.version = JObject.Parse(versionText);
        }
        catch (Exception ex)
        {
            if (File.Exists(PathHelper.versionLOCPTH))
            {
                Debug.LogError($"can not find version file: {PathHelper.versionLOCPTH}");
            }

            HotfixState.hotfixThrowErrMsg = $"read local version file  err: {ex.Message} ; path: {PathHelper.versionLOCPTH} ; version text: {versionText}";
            PageLaunch.Instance.Error(HotfixState.hotfixThrowErrMsg);
            Debug.LogError(HotfixState.hotfixThrowErrMsg);
            throw ex;
        }



        // 获取主模块版本信息
        try
        {
            mainModVersionText = File.ReadAllText(PathHelper.mainModVersionLOCPTH);
            GlobalModel.mainModVersion = JObject.Parse(mainModVersionText);
        }
        catch (Exception ex)
        {
            if (File.Exists(PathHelper.mainModVersionLOCPTH))
            {
                Debug.LogError($"can not find main mod version file: {PathHelper.mainModVersionLOCPTH}");
            }

            HotfixState.hotfixThrowErrMsg = $"read local main mod version file  err: {ex.Message} ; path: {PathHelper.mainModVersionLOCPTH} ; version text: {mainModVersionText}";
            PageLaunch.Instance.Error(HotfixState.hotfixThrowErrMsg);
            Debug.LogError(HotfixState.hotfixThrowErrMsg);
            throw ex;
        }


        // 获取总版本文件信息
        GlobalModel.totalVersion = null;
        GlobalModel.autoHotfixUrl = "";

        if (File.Exists(PathHelper.totalVersionLOCPTH))
        {
            try
            {
                totalVersionText = File.ReadAllText(PathHelper.totalVersionLOCPTH);  //【存在bug】 这里total文件是个空！！
                GlobalModel.totalVersion = JObject.Parse(totalVersionText);

                JObject targetTotalVersionItem = null;
                JArray lst = GlobalModel.totalVersion["data"] as JArray;
                for (int i = 0; i < lst.Count; i++)
                {
                    string appKey = lst[i]["app_key"].ToObject<string>();
                    if (appKey == ApplicationSettings.Instance.appKey)
                    {
                        targetTotalVersionItem = lst[i] as JObject;
                        break;
                    }
                }

                if (targetTotalVersionItem != null)
                {
                    GlobalModel.autoHotfixUrl = FileUtils.GetDirWebUrl(PathHelper.totalVersionWEBURL, targetTotalVersionItem["hotfix_url"].ToObject<string>());
                }
                else
                {
                    Debug.LogWarning($"cant not find app key:{ApplicationSettings.Instance.appKey} in local total version");
                }
            }
            catch (Exception ex)
            {
                HotfixState.hotfixThrowErrMsg = $"read local total version file  err: {ex.Message} ; path: {PathHelper.totalVersionLOCPTH} ; version text: {totalVersionText}";
                PageLaunch.Instance.Error(HotfixState.hotfixThrowErrMsg);
                Debug.LogError(HotfixState.hotfixThrowErrMsg);
                throw ex;
            }
        }

    }

    #region 检查热更异常失败卡死
    //const string HOTFIX_THROW_ERR = "HOTFIX_THROW_ERR";
    //string throwErrMsg = "";
    Coroutine coCheckHotfixThrowErr;
    IEnumerator CheckHotfixThrowErr()
    {
        yield return new WaitForSecondsRealtime(100f);  // 5 * 60 （20分钟）
        PlayerPrefs.SetString(HotfixState.HOTFIX_THROW_ERR, HotfixState.hotfixThrowErrMsg);
        PlayerPrefs.Save();
        Debug.LogError($"save hotfix throw err : {HotfixState.hotfixThrowErrMsg}");

        if (!ApplicationSettings.Instance.isRelease)
            PageLaunch.Instance.Error(HotfixState.hotfixThrowErrMsg);
    }
    void StartCheckHotfixThrowErr()
    {
        CloseCoCheckHotFixThrowErr();
        coCheckHotfixThrowErr = StartCoroutine(CheckHotfixThrowErr());
    }
    void CloseCheckHotfixThrowErr()
    {
        CloseCoCheckHotFixThrowErr();
        ClearHotfixThrowErrFlg();
    }
    /// <summary> 清除热更标志位 </summary>
    void ClearHotfixThrowErrFlg()
    {
        PlayerPrefs.DeleteKey(HotfixState.HOTFIX_THROW_ERR);
        PlayerPrefs.Save();
    }
    void CloseCoCheckHotFixThrowErr()
    {
        if (coCheckHotfixThrowErr != null)
            StopCoroutine(coCheckHotfixThrowErr);
        coCheckHotfixThrowErr = null;
    }
    #endregion




    /// <summary>
    /// 检查并热更新资源
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator DoHotfix(UnityAction callback)
    {

        bool isLastHotfixThrowErr = PlayerPrefs.HasKey(HotfixState.HOTFIX_THROW_ERR);
        if (isLastHotfixThrowErr)
        {
            Debug.LogError($"last hotfix throw err: {PlayerPrefs.GetString(HotfixState.hotfixThrowErrMsg)}");
        }

        // 【解決热更异常】
        StartCheckHotfixThrowErr();

        UnityAction proxyCallback = () =>
        {
            CloseCheckHotfixThrowErr();
            callback?.Invoke();
        };


        if (PlayerPrefs.HasKey(HotfixState.HOTFIX_STATE))
        {
            Debug.LogWarning($"last hotfix state: {PlayerPrefs.GetString(HotfixState.HOTFIX_STATE)}");
        }

        ClearRemoteInfo();

        // 获取包内版本
        yield return GetStreamingAssetsVersion();

        // 编辑器不使用热更
        if (!ApplicationSettings.Instance.IsUseHotfixBundle())
        {

            yield return FileUtils.ReadStreamingAsset<string>(
                PathHelper.versionSAPTH,
                (obj) =>
                {
                    GlobalModel.version = JObject.Parse((string)obj);
                }, null);


            yield return FileUtils.ReadStreamingAsset<string>(
                PathHelper.mainModVersionSAPTH,
                (obj) =>
                {
                    GlobalModel.mainModVersion = JObject.Parse((string)obj);
                }, null);

            Debug.Log($"is use streaming assets version");
            // 回调


            // StreamingAssets里文件改变，也要同步本地大厅游戏信息
            LobbyGamesManager.Instance.ChangeLocalInfo(()=>{
                LobbyGamesManager.Instance.SaveLobbyGameInfoAndHash();
            });

            proxyCallback?.Invoke();
            yield break;
        }


        bool isFirstInstall = GlobalModel.isFirstInstall;

        // 首次装包拷贝文件
        if (isFirstInstall || !File.Exists(PathHelper.mainModVersionLOCPTH) || isLastHotfixThrowErr)
        {

            ClearHotfixThrowErrFlg();

            yield return CopyStreamingAssetsToPersistentDataPath();
        }

        // 检查要拷贝的文件
        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.CHECK_COPY_TEMP_HOTFIX_FILE, 1);
        PageLaunch.Instance.Next(LoadingProgressMod.CHECK_COPY_TEMP_HOTFIX_FILE, $"check cache : temp hotfix file");
        yield return ModuleDownloadManager.Instance.CopyTempWebHotfixFileToTargetDir();
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.CHECK_COPY_TEMP_HOTFIX_FILE);


        // 获取本地配置文件
        GetLocalVersionInfo();



        /// 【获取版本信息】多次请求，避免机台上电后，wifi要延时才链接。
        Debug.LogWarning($"{PREFIX_TIP} getting web version files fails");
        bool isGetWebVersionSuccess = false;
        int count = GlobalModel.hotfixRequestCount;
        int i = 0;
        while (true)
        {
            yield return GetWebTotalVersionAndVersion(() => isGetWebVersionSuccess = true, () => isGetWebVersionSuccess = false);

            if (isGetWebVersionSuccess || --count < 1)
                break;
            else
            {
                Debug.LogWarning($"getting web version files fails, count: {++i}  time: {Time.unscaledTime}");
                yield return new WaitForSecondsRealtime(2.5f);
            }
        }

        Debug.Log($"is get web version file = {isGetWebVersionSuccess}");


        if (!isGetWebVersionSuccess)
        {
            // 回调
            proxyCallback?.Invoke();
            yield break;
        }
        else
        {

            string localVersionVER = GlobalModel.mainModVersion["hotfix_version"].Value<string>();
            string webVersionVER = webMainModVersionFileNode["hotfix_version"].Value<string>();
            Debug.Log($"local main mod version file ver:{localVersionVER}  --  web main mod version file ver:{webVersionVER}");
            int[] localVersions = GetVersions(localVersionVER);
            int[] serverVersions = GetVersions(webVersionVER);

            if (localVersions[0] == serverVersions[0])
            {

                if (localVersions[1] != serverVersions[1] || localVersions[2] != serverVersions[2])
                {

                    bool isError = false;



                    // 开始下载本模块
                    PlayerPrefs.SetString(HotfixState.HOTFIX_STATE, HotfixState.HotfixDownloading);

                    // 下载主mainfest
                    yield return  ModuleDownloadManager.Instance.DownloadManifestToTemp(
                        (successMsg) =>
                        {
                        },
                        (errorMsg) =>
                        {
                            isError = true;
                        });



                    if (!isError)
                    {
                        // 下载主模块
                        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.DOWNLOAD_MOD_MAIN, 1);

                        PageLaunch.Instance.Next(LoadingProgressMod.DOWNLOAD_MOD_MAIN,
                            $"download main mod :{PathHelper.mianModuleName}");
                        yield return ModuleDownloadManager.Instance.DownLoadNeedModToTemp(PathHelper.mianModuleName,
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
                    }
                    PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DOWNLOAD_MOD_MAIN);


                    if (!isError)
                    {
                        LobbyGamesManager.Instance.LoadLobbyGamesInfoWhenHotfix((objs) =>
                        {
                            if ((int)objs[0] != 0)
                                isError = true;
                        });
                    }



                    // 下载“启动热更”游戏模块
                    if (!isError)
                    {

                        List<JObject> updateGames = new List<JObject>();
                        foreach (JObject Item in LobbyGamesManager.Instance.lobbyGamesInfoLocal)
                        {
                            bool updateAtLaunch = Item["update_at_launch"].Value<bool>();
                            if (updateAtLaunch)
                                updateGames.Add(Item);
                        }

                        int j = 0;
                        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.DOWNLOAD_MOD_GAME, updateGames.Count);
                        foreach (JObject Item in updateGames)
                        {
                            if (isError) continue;

                            string moduleName = Item["module_name"].Value<string>();

                            j++;
                            PageLaunch.Instance.Next(LoadingProgressMod.DOWNLOAD_MOD_GAME,
                                $"download game mod :{moduleName} {j}/{updateGames.Count}");
                            yield return ModuleDownloadManager.Instance.DownLoadNeedModToTemp(moduleName,
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
                        }
                    }
                    PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DOWNLOAD_MOD_GAME);




                    // 下载“启动热更-一次”游戏模块
                    if (!isError)
                    {
                        Debug.Log($"单次热更前：{JsonConvert.SerializeObject(LobbyGamesManager.Instance.lobbyGamesInfoLocal)}");
                        List<JObject> updateGames = new List<JObject>();
                        foreach (JObject Item in LobbyGamesManager.Instance.lobbyGamesInfoLocal)
                        {
                            bool updateAtLaunch = Item["update_at_launch_once"].Value<bool>();
                            if (updateAtLaunch)
                                updateGames.Add(Item);
                        }

                        int j = 0;
                        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.DOWNLOAD_MOD_GAME_ONCE, updateGames.Count);
                        foreach (JObject Item in updateGames)
                        {
                            if (isError) continue;

                            string moduleName = Item["module_name"].Value<string>();

                            j++;
                            PageLaunch.Instance.Next(LoadingProgressMod.DOWNLOAD_MOD_GAME_ONCE,
                                $"download game mod once :{moduleName} {j}/{updateGames.Count}");
                            yield return ModuleDownloadManager.Instance.DownLoadNeedModToTemp(moduleName,
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
                                    Item["update_at_launch_once"] = false;  // 单次热更完毕，置位
                                },
                                (errorMsg) =>
                                {
                                    Debug.LogError(errorMsg);
                                    isError = true;
                                });
                        }

                        Debug.Log($"单次热更后：{JsonConvert.SerializeObject(LobbyGamesManager.Instance.lobbyGamesInfoLocal)}");
                    }
                    PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DOWNLOAD_MOD_GAME_ONCE);



                    if (!isError)
                    {
                        // 写入主版本文件
                        FileUtils.WriteAllText(PathHelper.tmpVersionLOCPTH, webVersionFileNode.ToString());
                        
                        // 检查要拷贝的文件(全部需求模块下载完成，才能将临时文件拷贝到目标路劲中)
                        PageLaunch.Instance.AddProgressCount(LoadingProgress.COPY_TEMP_HOTFIX_FILE, 1);
                        PageLaunch.Instance.Next(LoadingProgress.COPY_TEMP_HOTFIX_FILE, $"copy cache : temp hotfix file");

                        PlayerPrefs.SetString(HotfixState.HOTFIX_STATE, HotfixState.HotfixCopying);
                        yield return ModuleDownloadManager.Instance.CopyTempWebHotfixFileToTargetDir();
                        PageLaunch.Instance.RemoveProgress(LoadingProgress.COPY_TEMP_HOTFIX_FILE);


                        // 重新获取本地配置文件
                        GlobalModel.mainModVersion = JObject.Parse(File.ReadAllText(PathHelper.mainModVersionLOCPTH));
                        GlobalModel.version = JObject.Parse(File.ReadAllText(PathHelper.versionLOCPTH));

                        // 保存
                        LobbyGamesManager.Instance.SaveLobbyGameInfoAndHash();

                        // 删除多余文件
                        yield return DeleteUnuseABAndManifest();


                        // 删除无用dll
                        yield return DeleteUnuseAssetDll();

                        // 删除无用backup
                        yield return DeleteUnuseAssetBackup();
                    }

                }
                else
                {
                    Debug.Log("no need for hotfix");
                }

                // 回调
                proxyCallback?.Invoke();
                yield break;
            }
            else // 网路主版本号大于当前主版本号
            {
                // 回调 - 后卡主 - 提醒需要下载更新的app安装包
                Debug.LogError("The local master version number and the remote master version number are not equal");
                proxyCallback?.Invoke();
                yield break;
            }
        }

    }






    #region 删除用不到的ab数据 dll数据
    /// <summary>
    /// 遍历所有预制体，设置预制体名
    /// </summary>
    /// <param name="rootFolderPath"></param>
    List<string> GetTargetFilePath(string rootFolderPath, string extension = ".png")
    {
        List<string> paths = new List<string>();
        foreach (string path in Directory.GetFiles(rootFolderPath))
        {
            //获取所有文件夹中包含后缀为 .prefab 的路径
            if (extension == ".*") // path System.IO.Path.GetExtension(path) != ".meta"
            {
                if (!path.EndsWith(".meta"))
                    paths.Add(path);
            }
            else if (path.EndsWith(extension))
            {
                paths.Add(path);
            }
        }
        if (Directory.GetDirectories(rootFolderPath).Length > 0)  //遍历所有文件夹
        {
            foreach (string path in Directory.GetDirectories(rootFolderPath))
            {
                paths.AddRange(GetTargetFilePath(path, extension));
            }
        }
        return paths;
    }

    List<string> GetUnuseAB()
    {

        string manifestAssetName = "AssetBundleManifest"; // 假设 manifest 文件的资源名称
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(PathHelper.mainfestLOCPTH);
        AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>(manifestAssetName);
        manifestBundle.Unload(false);

        string[] allAssetBundleNames = manifest.GetAllAssetBundles();

        List<string> bundlePathLst = new List<string>();
        foreach (string assetBundleName in allAssetBundleNames)
        {
            string pth1 = Path.Combine(PathHelper.abDirLOCPTH, assetBundleName);
            bundlePathLst.Add(pth1.Replace("\\", "/"));
        }

        // 主包ab包，是不带".unity3d"结尾的。（"AstBundle" 和 “AstBundle.manifest”）
        List<string> targetPathLst = new List<string>();  //获取普通包路劲 xxx.unity3d  和  xxx.unity3d.manifest
        targetPathLst.AddRange(GetTargetFilePath(PathHelper.abDirLOCPTH, ".unity3d"));
        //targetPathLst.AddRange(GetTargetFilePath(abDirLOCPTH, ".unity3d.manifest"));

        for (int i = 0; i < targetPathLst.Count; i++)
        {
            targetPathLst[i] = targetPathLst[i].Replace("\\", "/");
        }

        List<string> unusePths = new List<string>();
        foreach (string pth002 in targetPathLst)
        {
            /* string tempPth = pth002.EndsWith(".unity3d.manifest") ? pth002.Replace(".unity3d.manifest", ".unity3d") : pth002;
             if (!bundlePathLst.Contains(tempPth))
             {
                 unusePths.Add(pth002);
             }*/
            if (!bundlePathLst.Contains(pth002))
            {
                unusePths.Add(pth002);
            }
        }

        return unusePths;
    }

    /// <summary>
    /// 删除不用的ab包和manifest文件
    /// </summary>
    IEnumerator DeleteUnuseABAndManifest()
    {
        List<string> unusePths = GetUnuseAB();

        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.DELETE_UNUSE_ASSET_BUNDLE, unusePths.Count);

        int i = 0;
        foreach (string pth in unusePths)
        {
            i++;
            if (File.Exists(pth))
            {
                PageLaunch.Instance.Next(LoadingProgressMod.DELETE_UNUSE_ASSET_BUNDLE, $"delete unuse ab {i}/{unusePths.Count}: {pth}  ");

                Debug.Log($"delete unuse ab {i}/{unusePths.Count}: {pth}");
                File.Delete(pth);
                yield return null;
            }
        }

        PageLaunch.Instance.Next(LoadingProgressMod.DELETE_UNUSE_ASSET_BUNDLE, $"delete unuse ab folder");

        yield return DeleteUnuseABFolderAndMeta();

        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DELETE_UNUSE_ASSET_BUNDLE);
    }


    /// <summary>
    /// 删除不用的文件夹
    /// </summary>
    /// <returns></returns>
    IEnumerator DeleteUnuseABFolderAndMeta()
    {

        List<string> allSubDirectories = GetAllSubFolders(PathHelper.abDirLOCPTH);

        // 对文件夹路径按字符长度从长到短进行排序（越长的路劲排在越前面）
        allSubDirectories.Sort((a, b) => b.Length - a.Length);

        /*
        // 输出所有子文件夹的名称
        foreach (string directory in allSubDirectories)
            Debug.Log(directory);
        */

        // 遍历排序后的目录路径
        foreach (string directory in allSubDirectories)
        {
            if (ShouldDeleteDirectory(directory))
            {
                DeleteDirectoryAndMeta(directory);
                yield return null;
            }
        }
    }

    static List<string> GetAllSubFolders(string directoryPath)
    {
        List<string> allFolders = new List<string>();
        try
        {
            // 获取当前目录下的所有子文件夹
            string[] subDirectories = Directory.GetDirectories(directoryPath);
            foreach (string subDirectory in subDirectories)
            {
                // 将当前子文件夹添加到结果列表中
                allFolders.Add(subDirectory);
                // 递归调用该方法，获取当前子文件夹下的所有子文件夹
                allFolders.AddRange(GetAllSubFolders(subDirectory));
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"访问目录 {directoryPath} 时出错: {e.Message}");
        }
        return allFolders;
    }


    /// <summary>
    /// 如果该路劲存在，且没有子级文件夹，或子级文件都是 .meta文件， 则把这个目录及其里面的内都删除
    /// </summary>
    /// <param name="directoryPath"></param>
    static bool ShouldDeleteDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            return false;
        }

        string[] subDirectories = Directory.GetDirectories(directoryPath);
        if (subDirectories.Length > 0)
        {
            return false;
        }

        string[] files = Directory.GetFiles(directoryPath);
        foreach (string file in files)
        {
            if (Path.GetExtension(file).ToLower() != ".meta")
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 删除目录，和目录的.meta文件，及目录里的所有内容
    /// </summary>
    /// <param name="directoryPath"></param>
    static void DeleteDirectoryAndMeta(string directoryPath)
    {
        try
        {
            // 删除目录及其内容
            Directory.Delete(directoryPath, true);

            // 删除对应的 .meta 文件
            string metaFilePath = directoryPath + ".meta";
            if (File.Exists(metaFilePath))
            {
                File.Delete(metaFilePath);
            }

            Debug.Log($"delete unuse dir: {directoryPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"delete unuse dir: {directoryPath} error: {e.Message}");
        }
    }


    private IEnumerator DeleteUnuseAssetDll()
    {

        JObject hotfixDlls = GlobalModel.version["asset_dll"] as JObject;

        List<string> targetPathLst = new List<string>();  //获取普通包路劲 xxx.unity3d  和  xxx.unity3d.manifest
        targetPathLst.AddRange(GetTargetFilePath(PathHelper.hotfixDirLOCPTH, ".dll.bytes"));

        int idx = targetPathLst.Count - 1;
        while (idx >= 0)
        {
            string[] pths = targetPathLst[idx].Replace("\\", "/").Split('/');
            string name = pths[pths.Length - 1].Replace(".dll.bytes", "");

            if (hotfixDlls.ContainsKey(name))
            {
                targetPathLst.RemoveAt(idx);
            }
            idx--;
        }

        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.DELETE_UNUSE_ASSET_DLL, targetPathLst.Count);
        int i = 0;
        foreach (string s in targetPathLst)
        {
            i++;
            if (File.Exists(s))
            {
                PageLaunch.Instance.Next(LoadingProgressMod.DELETE_UNUSE_ASSET_DLL, $"delete unuse dll {i}/{targetPathLst.Count}: {s}  ");

                Debug.Log($"delete unuse dll {i}/{targetPathLst.Count}: {s}");

                File.Delete(s);
                yield return null;
            }
        }

        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DELETE_UNUSE_ASSET_DLL);

    }


    private IEnumerator DeleteUnuseAssetBackup()
    {

        JObject assetBackup = GlobalModel.version["asset_backup"] as JObject;

        List<string> targetPathLst = new List<string>();  //获取普通包路劲 xxx.unity3d  和  xxx.unity3d.manifest
        targetPathLst.AddRange(GetTargetFilePath(PathHelper.hotfixDirLOCPTH, ".*"));

        string targetSegment = $"{PathHelper.backupFolderName}/";

        int idx = targetPathLst.Count - 1;
        while (idx >= 0)
        {
            string pth = targetPathLst[idx].Replace("\\", "/");
            int index = pth.IndexOf(targetSegment, StringComparison.Ordinal);
            pth = index == -1 ? pth : pth.Substring(index + targetSegment.Length);

            if (assetBackup.ContainsKey(pth))
            {
                targetPathLst.RemoveAt(idx);
            }
            idx--;
        }

        PageLaunch.Instance.AddProgressCount(LoadingProgress.DELETE_UNUSE_ASSET_BACKUP, targetPathLst.Count);
        int i = 0;
        foreach (string s in targetPathLst)
        {
            i++;
            if (File.Exists(s))
            {
                PageLaunch.Instance.Next(LoadingProgress.DELETE_UNUSE_ASSET_BACKUP, $"delete unuse backup {i}/{targetPathLst.Count}: {s}  ");

                Debug.Log($"delete unuse dll {i}/{targetPathLst.Count}: {s}");

                File.Delete(s);
                yield return null;
            }
        }

        PageLaunch.Instance.RemoveProgress(LoadingProgress.DELETE_UNUSE_ASSET_BACKUP);

    }
    #endregion





    #region 拷贝本包文件

    private IEnumerator GetStreamingAssetsVersion()
    {

        bool isNext = false;

        // 所有资源版本
        GlobalModel.streamingAssetsVersion = null;
        yield return FileUtils.ReadStreamingAsset<string>(PathHelper.versionSAPTH, (obj) =>
        {
            GlobalModel.streamingAssetsVersion = JObject.Parse((string)obj);
        }, (err) =>
        {
            throw new System.Exception(err);
        });


        // 主模块版本
        GlobalModel.streamingAssetsMainModVersion = null;
        yield return FileUtils.ReadStreamingAsset<string>(PathHelper.mainModVersionSAPTH, (obj) =>
        {
            GlobalModel.streamingAssetsMainModVersion = JObject.Parse((string)obj);
        }, (err) =>
        {
            throw new System.Exception(err);
        });


        LobbyGamesManager.Instance.LoadLobbyGamesInfoSA(() =>
        {
            isNext = true;
        });

        yield return new WaitUntil(() => isNext == true );
        isNext = false;


    }


    private IEnumerator CopyStreamingAssetsToPersistentDataPath()
    {

        Debug.LogWarning($"{PREFIX_TIP} copy streaming assets");

        // 删除临时文件目录
        if (Directory.Exists(PathHelper.tmpHotfixDirLOCPTH))
        {
            Debug.LogWarning("delete temp hotfix dir");
            yield return FileUtils.DeleteDirectoryAsync(PathHelper.tmpHotfixDirLOCPTH);
        }
        // 删除目录
        if (Directory.Exists(PathHelper.hotfixDirLOCPTH))
        {
            Debug.LogWarning("delete hotfix dir");
            yield return FileUtils.DeleteDirectoryAsync(PathHelper.hotfixDirLOCPTH);
        }

        // 删除total version 文件
        if (File.Exists(PathHelper.totalVersionLOCPTH))
        {
            Debug.LogWarning("delete total version file");
            File.Delete(PathHelper.totalVersionLOCPTH);
        }

        // 拷贝所有ab资源
        // 先加载manifest文件，读取所有ab资源
        using (var manifestWWW = new UnityWebRequest(PathHelper.mainfestSAPTH))
        {
            manifestWWW.downloadHandler = new DownloadHandlerAssetBundle(PathHelper.mainfestSAPTH, 0);
            yield return manifestWWW.SendWebRequest();

            if (manifestWWW.isNetworkError || manifestWWW.isHttpError)
            {
                throw new Exception("StreamingAssets中没有manifest文件，仅拷贝version文件");
            }
            else
            {
                // 拷贝所有ab资源到持久目录
                var manifestAB = DownloadHandlerAssetBundle.GetContent(manifestWWW);
                var abManifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                manifestAB.Unload(false);
                string[] abList = abManifest.GetAllAssetBundles();
                int totalCount = abList.Length;// 一个是版本配置文件，一个是manifest文件
                int completedCount = 0;

                PageLaunch.Instance.AddProgressCount(LoadingProgressMod.COPY_SA_ASSET_BUNDLE, totalCount);

                foreach (var abName in abList)
                {
                    string srcPath = PathHelper.GetAssetBundleSAPTH(abName);
                    string tarPath = PathHelper.GetAssetBundleLOCPTH(abName);
                    Debug.Log($"{srcPath} - {tarPath}");
                    completedCount++;

                    PageLaunch.Instance.Next(LoadingProgressMod.COPY_SA_ASSET_BUNDLE,
                        $"copy assetbundle to cache: {abName} {completedCount}/{totalCount}");

                    Debug.Log(string.Format("copy asset bundle {0}/{1}, bundle:{2}", completedCount, totalCount, abName));


                    // 如果软件包只包含部分游戏，ab包补全！所以这里用CopyStreamingAssetToLocalWhenFileExists接口
                    yield return FileUtils.CopyStreamingAssetToLocalWhenFileExists(srcPath, tarPath);

                }
                PageLaunch.Instance.Next(LoadingProgressMod.COPY_SA_ASSET_BUNDLE,
                    $"copy manifest to cache: {PathHelper.mainfestBundleName}");

                Debug.Log("copy manifest");
                yield return FileUtils.CopyStreamingAssetToLocal(PathHelper.mainfestSAPTH, PathHelper.mainfestLOCPTH);

            }
        }
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.COPY_SA_ASSET_BUNDLE);


        // 所有文件复制到本地
        JObject versionSAObj = GlobalModel.streamingAssetsVersion;

        if (versionSAObj != null)
        {

            JObject hotfixDll = versionSAObj["asset_dll"] as JObject;

            PageLaunch.Instance.AddProgressCount(LoadingProgressMod.COPY_SA_HOTFIX_DLL, hotfixDll.Count);

            int i = 0;
            foreach (KeyValuePair<string, JToken> kv in hotfixDll)
            {

                string saPth = PathHelper.GetDllSAPTH(kv.Key);
                string tarPath = PathHelper.GetDllLOCPTH(kv.Key);

                i++;
                PageLaunch.Instance.Next(LoadingProgressMod.COPY_SA_HOTFIX_DLL,
                    $"copy dll to cache: {kv.Key} {i}/{hotfixDll.Count}");

                Debug.Log(string.Format("copy dll {0}/{1}, dll:{2}", i, hotfixDll.Count, kv.Key));

                yield return FileUtils.CopyStreamingAssetToLocalWhenFileExists(saPth, tarPath);
            }

        }

        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.COPY_SA_HOTFIX_DLL);


        // 拷贝所有Backup文件

        if (versionSAObj != null)
        {
            JObject astBackup = versionSAObj["asset_backup"] as JObject;

            PageLaunch.Instance.AddProgressCount(LoadingProgressMod.COPY_SA_ASSET_BACKUP, astBackup.Count);

            int i = 0;
            foreach (KeyValuePair<string, JToken> kv in astBackup)
            {
                string saPth = PathHelper.GetAssetBackupSAPTH(kv.Key); //PathHelper.GetDllSAPTH(kv.Key);
                string tarPath = PathHelper.GetAssetBackupLOCPTH(kv.Key); // PathHelper.GetDllLOCPTH(kv.Key);

                i++;
                PageLaunch.Instance.Next(LoadingProgressMod.COPY_SA_ASSET_BACKUP,
                    $"copy asset backup to cache: {kv.Key} {i}/{astBackup.Count}");

                Debug.Log(string.Format("copy asset backup {0}/{1}, backup:{2}", i, astBackup.Count, kv.Key));

                yield return FileUtils.CopyStreamingAssetToLocalWhenFileExists(saPth, tarPath);
            }
        }
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.COPY_SA_ASSET_BACKUP);



       
        if (Directory.Exists(PathHelper.modulesDirLOCPTH) == false)  
        {
            Directory.CreateDirectory(PathHelper.modulesDirLOCPTH); 
        }



        // 拷贝“游戏的模块版本文件”

        if (LobbyGamesManager.Instance.lobbyGamesInfoSever.Count >0)
        {
            int i = 0;
            int count = LobbyGamesManager.Instance.lobbyGamesInfoSever.Count;
            PageLaunch.Instance.AddProgressCount(LoadingProgressMod.COPY_SA_ASSET_MOD_VER, count);

            foreach (JObject item in LobbyGamesManager.Instance.lobbyGamesInfoSever)
            {
                string moduleName = item["module_name"].Value<string>();

                if (moduleName == PathHelper.mianModuleName)
                {
                    continue;
                }

                string fromPht = PathHelper.GetModuleVersionSAPTH(moduleName);
                string toPth = PathHelper.GetModuleVersionLOCPTH(moduleName);

                i++;
                PageLaunch.Instance.Next(LoadingProgressMod.COPY_SA_ASSET_MOD_VER,
                    $"copy mod version to cache: {toPth} {i}/{count}");

                Debug.Log(string.Format("copy mod version to cache {0}/{1}, pth:{2}", i, count, toPth));

                yield return FileUtils.CopyStreamingAssetToLocalWhenFileExists(fromPht, toPth);
            }
        }
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.COPY_SA_ASSET_MOD_VER);


        // 最后才拷贝version文件"主版本文件"
        if (versionSAObj != null)
        {
            PageLaunch.Instance.RefreshProgressUIMsg($"copy version file to cache: {PathHelper.versionLOCPTH}");
            Debug.Log("copy version");
            yield return FileUtils.CopyStreamingAssetToLocal(PathHelper.versionSAPTH, PathHelper.versionLOCPTH);
        }



        // 最后才拷贝“主模块的mod版文件”
        if (GlobalModel.streamingAssetsMainModVersion  != null)
        {
            PageLaunch.Instance.RefreshProgressUIMsg($"copy main mod version file to cache: {PathHelper.mainModVersionLOCPTH}");
            Debug.Log("copy version");
            yield return FileUtils.CopyStreamingAssetToLocalWhenFileExists(PathHelper.mainModVersionSAPTH, PathHelper.mainModVersionLOCPTH);
        }
        else
        {
            Debug.LogError("the version in StreamingAssets can not find!! ");
        }
    }

    #endregion


    /*
    #region 下载资源拷贝

    /// <summary>
    /// 查看是否有新热更完整的资源缓存
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// * 每次最后，都有删除临时缓存
    /// </remarks>
    private IEnumerator CopyTempWebHotfixFileToTargetDir()
    {

        // 是否有热更新文件需要拷贝
        if (PlayerPrefs.HasKey(HotfixState.HOTFIX_STATE) && PlayerPrefs.GetString(HotfixState.HOTFIX_STATE) == HotfixState.HotfixCopying)
        {
            Debug.LogWarning($"{PREFIX_TIP}  copy temp file to target dir");

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

    #endregion

    */

    #region 检查远端版本

    /// <summary>
    /// 删除远程的参数
    /// </summary>
    void ClearRemoteInfo()
    {
        webMainModVersionFileNode = null;
        webVersionFileNode = null;
    }


    private IEnumerator GetWebTotalVersionAndVersion(UnityAction onSuccessCallback, UnityAction onErrorCallback)
    {

        string tvUrl = PathHelper.totalVersionWEBURL + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        Debug.Log($"download total version： {tvUrl}");

        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.CHECK_WEB_VERSION, 3);
        PageLaunch.Instance.Next(LoadingProgressMod.CHECK_WEB_VERSION, $"get web total version");

        using (UnityWebRequest reqTotalVersion = UnityWebRequest.Get(tvUrl))
        {
            yield return reqTotalVersion.SendWebRequest();

            if (reqTotalVersion.result == UnityWebRequest.Result.Success)
            {
                string tvStr = reqTotalVersion.downloadHandler.text;

                GlobalModel.totalVersion = JObject.Parse(tvStr);

                // 拷贝版本文件(写入文件时，可能断电重启，导致写入数据有问题)
                FileUtils.WriteAllBytes(PathHelper.totalVersionLOCPTH, reqTotalVersion.downloadHandler.data);

                JObject targetTotalVersionItem = null;
                if (GlobalModel.totalVersion != null)
                {
                    JArray lst = GlobalModel.totalVersion["data"] as JArray;
                    for (int i = 0; i < lst.Count; i++)
                    {
                        string appKey = lst[i]["app_key"].ToObject<string>();
                        if (appKey == ApplicationSettings.Instance.appKey)
                        {
                            targetTotalVersionItem = lst[i] as JObject;
                            break;
                        }
                    }
                }

                if (targetTotalVersionItem != null)
                {
                    GlobalModel.autoHotfixUrl = FileUtils.GetDirWebUrl(PathHelper.totalVersionWEBURL, targetTotalVersionItem["hotfix_url"].ToObject<string>());


                    PageLaunch.Instance.Next(LoadingProgressMod.CHECK_WEB_VERSION, $"get web version");

                    yield return GetWebVersion(onSuccessCallback, onErrorCallback);


                    PageLaunch.Instance.Next(LoadingProgressMod.CHECK_WEB_VERSION, $"get web main mod version");

                    yield return GetMainModWebVersion(onSuccessCallback, onErrorCallback);

                }
                else
                {
                    Debug.LogError($"web total version file cant not find node at app_key: {ApplicationSettings.Instance.appKey}");
                    onErrorCallback?.Invoke();
                }
            }
            else//服务器版本文件加载失败
            {
                Debug.LogError(reqTotalVersion.error);
                Debug.Log("没有网络直接复制文件");
                onErrorCallback?.Invoke();
            }
        }
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.CHECK_WEB_VERSION);
    }


    private IEnumerator GetMainModWebVersion(UnityAction onSuccessCallback, UnityAction onErrorCallback)
    {

        string vUrl = PathHelper.mainModVersionWEBURL + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        Debug.Log($"remote version url： {vUrl}");
        using (UnityWebRequest reqVersion = UnityWebRequest.Get(vUrl))
        {
            yield return reqVersion.SendWebRequest();

            if (reqVersion.result == UnityWebRequest.Result.Success)
            {
                string jsonStr = reqVersion.downloadHandler.text;

                //versionDataRemoteByte = reqVersion.downloadHandler.data;

                webMainModVersionFileNode = JObject.Parse(jsonStr);

                Debug.Log($"web main mod version file ; ver: {webMainModVersionFileNode["hotfix_version"].ToObject<string>()}");

                //Debug.Log($"version data :  {jsonStr}");
                onSuccessCallback?.Invoke();
            }
            else//服务器版本文件加载失败
            {
                Debug.LogError(reqVersion.error);
                Debug.Log("没有网络直接复制文件");
                onErrorCallback?.Invoke();
            }

        }
    }


    private IEnumerator GetWebVersion(UnityAction onSuccessCallback, UnityAction onErrorCallback)
    {

        string vUrl = PathHelper.versionFileWEBURL + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        Debug.Log($"remote version url： {vUrl}");
        using (UnityWebRequest reqVersion = UnityWebRequest.Get(vUrl))
        {
            yield return reqVersion.SendWebRequest();

            if (reqVersion.result == UnityWebRequest.Result.Success)
            {
                string jsonStr = reqVersion.downloadHandler.text;

                //versionDataRemoteByte = reqVersion.downloadHandler.data;

                webVersionFileNode = JObject.Parse(jsonStr);

                Debug.Log($"web version file ; ver: {webVersionFileNode["hotfix_version"].ToObject<string>()}");

                //Debug.Log($"version data :  {jsonStr}");
                onSuccessCallback?.Invoke();
            }
            else//服务器版本文件加载失败
            {
                Debug.LogError(reqVersion.error);
                Debug.Log("没有网络直接复制文件");
                onErrorCallback?.Invoke();
            }

        }
    }

    #endregion

















    private int[] GetVersions(string version)
    {
        string[] str = version.Split('.');

        List<int> vers = new List<int>();
        for (int i = 0; i < str.Length; i++)
        {
            vers.Add(int.Parse(str[i]));
        }
        // 解析主版本号和次版本号
        return vers.ToArray();
    }





}
