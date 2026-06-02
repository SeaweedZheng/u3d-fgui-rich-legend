using System.Threading.Tasks;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;
using GameMaker;
using System.Security.Policy;
using System.Diagnostics.Eventing.Reader;
using Common99000000;

public class GameUpdateChecker : MonoSingleton<GameUpdateChecker>
{

    /// <summary>
    /// 大厅进入游戏前热更
    /// </summary>
    /// <param name="gameId"></param>
    /// <param name="onFinsih"></param>
    public void CheckPlayabilityWhenEnterGame(int gameId, Action<bool> onFinsih)
        => StartCoroutine(CoCheckPlayabilityWhenEnterGame(gameId, onFinsih));


    /// <summary>
    /// 大厅进入游戏前热更
    /// </summary>
    /// <param name="gameId"></param>
    /// <param name="isPopTip"></param>
    /// <returns></returns>
    /// <remarks>
    /// * 服务器维护中的游戏，则不出现在大厅展示列表中
    /// * 如果游戏能直接允许，则优先允许。无法运行才检查“进入游戏前热更”
    /// </remarks>
    IEnumerator CoCheckPlayabilityWhenEnterGame(int gameId, Action<bool> onFinsih)
    {
        // 编辑器下，且非调试热更新模式下
        if (Application.isEditor && !ApplicationSettings.Instance.IsUseHotfixBundle())
        {
            onFinsih?.Invoke(true);// 允许进入游戏
            yield break;
        }

        bool isNext = false;

        /*
        // 服务器在维护该游戏
        if (!LobbyGamesManager.Instance.GetSeverValue<bool>(gameId, "is_available")) //是否开放
        {
            //游戏维护中，敬请期待！
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Game under maintenance, please stay tuned!"));

            onFinsih?.Invoke(false);
            yield break;
        }
        */

        // 获取游戏的游戏模块名称
        string moduleName = LobbyGamesManager.Instance.GetGameValueFromServer<string>(gameId, "module_name");

        bool isOk = ModuleDownloadManager.Instance.CheckGamePlayable(moduleName);

        if (isOk)
        {
            onFinsih?.Invoke(true);// 允许进入游戏
            yield break;
        }

        // 允许进入游戏前热更
        if (!SBoxModel.Instance.isUpdateAtEnterGame){
            // 请联系工作人员，更新游戏
            //if (isPopTip)
                TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Please contact staff to update the game."));

            onFinsih?.Invoke(false);
            yield break;
        }
        else{

            // 标记（重启热更一次）
            // 【问题】为什么要在这里标记为重启热更一次？？(难道是允许进入前热更？？)
            LobbyGamesManager.Instance.SetValueForCache<bool>(gameId, "update_at_launch_once", true);

            string webPth = PathHelper.GetModuleVersionWEBURL(moduleName);

            JObject webModVerNode = null;

            // 非cdn加载
            UnityWebRequest reqModVerFile = UnityWebRequest.Get(webPth + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
            yield return reqModVerFile.SendWebRequest();
            if (reqModVerFile.result == UnityWebRequest.Result.Success){

                string tvStr = reqModVerFile.downloadHandler.text;
                webModVerNode = JObject.Parse(tvStr);


                // 获取网络该游戏的的所有依赖hash值
                Dictionary<string, string> webModHashDis = new Dictionary<string, string>();
                JObject dependenciesNode = webModVerNode["dependencies"] as JObject;
                foreach (KeyValuePair<string, JToken> kv in dependenciesNode)
                {
                    webModHashDis.Add(kv.Key, kv.Value["hash"].Value<string>());
                }
                webModHashDis.Add(webModVerNode["name"].Value<string>(), webModVerNode["hash"].Value<string>());

                // 检查已运行的依赖包，是否和热更发生冲突
                foreach (KeyValuePair<string, string> kv in webModHashDis)
                {
                    if (ModuleDownloadManager.Instance.runingModHash.ContainsKey(kv.Key) && ModuleDownloadManager.Instance.runingModHash[kv.Key] != kv.Value)
                    {

                        // 或者 提示“重启并进行热更新，是否选择重启？”

                        // 模块已经运行，需要重启才能热更新！
                        // 请联系工作人员，更新游戏 [1]
                        TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Please contact staff to update the game.")); //需要重启才能热更新！

                        onFinsih?.Invoke(false);
                        yield break;
                    }
                }

                object[] result = ModuleDownloadManager.Instance.GetDownloadTotalSize(moduleName, webModVerNode);

                if ((int)result[0] != 0) //无需下载！！
                {
                    // 请联系工作人员，更新游戏
                    TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Please contact staff to update the game."));

                    onFinsih?.Invoke(false);
                    yield break;
                }

                long totalSize = (long)result[2];

                // 检测到游戏更新，需下载 500M 资源，是否继续？ 
                bool isConfirmDownload = false;
                CommonPopupHandler.Instance.OpenPopup(new CommonPopupInfo()
                {
                    // 只能修改一次线号机台号，确定要修改？
                    type = CommonPopupType.YesNo,
                    text =  string.Format(I18nMgr.T("Game update detected. Download size: {0}. Continue?"),FileUtils.FormatFileSize(totalSize)),
                    buttonText1 = I18nMgr.T("Cancel"),
                    buttonText2 = I18nMgr.T("OK"),
                    callback1 = () =>
                    {
                        isNext = true;
                    },
                    callback2 = () =>
                    {
                        isConfirmDownload = true;
                        isNext = true;
                    },
                });

                yield return new WaitUntil(()=> isNext == true);

                if (!isConfirmDownload) // 不下载
                {
                    onFinsih?.Invoke(false);
                    yield break;
                }


                // 【优化】在这里标记为重启热更一次？？

                string assetPth = LobbyGamesManager.Instance.GetGameValueFromServer<string>(gameId, "poster_url");

                PageManager.Instance.OpenPage(PageName.CommonPopupSystemLoading, 
                    /*new EventData<Dictionary<string, object>>("",new Dictionary<string, object>()
                    {
                        ["title"] = "",
                        ["url"] = PathHelper.GetAssetBackupWEBURL(assetPth),
                    })*/
                    new InParamsPopupSystemLoading()
                    {
                        title = "",
                        url = PathHelper.GetAssetBackupWEBURL(assetPth),
                    }
                );



                bool isError = false;

                LoadingProgressPercentInfo pgInfo = new LoadingProgressPercentInfo()
                {
                    msg = $"start download mod: {moduleName}",
                    totalStepCount = 2,
                    curStepIndex = 0,
                    curStepSubCount = 0,
                };

                // 更新资源， 大厅显示进度条。（通过事件广播出去， 或者通过委托）
                yield return ModuleDownloadManager.Instance.DownLoadNeedModToTemp(moduleName,
                null,
                (startMsg) =>
                {
                    DebugUtils.Log(startMsg);
                },
                (endMsg) =>
                {
                    DebugUtils.Log(endMsg);
                },
                (ProgressMsg) =>
                {
                    DebugUtils.Log(ProgressMsg);

                    pgInfo.msg = ProgressMsg;
                    pgInfo.curStepSubCount++;
                    SendEventLoadingProgress(pgInfo);
                },
                (successMsg) =>
                {
                    // 清除标记 
                    LobbyGamesManager.Instance.SetValueForCache<bool>(gameId, "update_at_launch_once", false);
                },
                (errorMsg) =>
                {
                    Debug.LogError(errorMsg);
                    isError = true;
                });


                if (isError) // 热更失败(如断网、断电)
                {
                    // 下载失败，请联系工作人员。
                    TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Download failed. Please contact the staff."));

                    PageManager.Instance.ClosePage(PageName.CommonPopupSystemLoading);

                    onFinsih?.Invoke(false);
                    yield break;
                }


                pgInfo.msg = "start copy temp hotfix file to target dir";
                pgInfo.curStepIndex++;
                pgInfo.curStepSubCount = 0;
                SendEventLoadingProgress(pgInfo);

                // 开始拷贝
                PlayerPrefs.SetString(HotfixState.HOTFIX_STATE, HotfixState.HotfixCopying);
                yield return ModuleDownloadManager.Instance.CopyTempWebHotfixFileToTargetDir((msg) =>
                {
                    //Debug.LogError($"11111 = {msg}");
                    pgInfo.msg = msg;
                    pgInfo.curStepSubCount++;
                    SendEventLoadingProgress(pgInfo);
                });


                PageManager.Instance.ClosePage(PageName.CommonPopupSystemLoading);


                // 如果热更成功
                onFinsih?.Invoke(true);
                yield break;
            }
            else
            {
                // 不存在该游戏，可能该游戏远端下架。 或 本机网络问题  或 服务器停止运行。
      
                // 请联系工作人员，更新游戏
                TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Please contact staff to update the game."));

                onFinsih?.Invoke(false);
                yield break;
            }
        }
    }

    void SendEventLoadingProgress(LoadingProgressPercentInfo data)
    {
        EventCenter.Instance.EventTrigger<EventData>(GlobalEvent.ON_SYSTEM_UI_EVENT,
        new EventData<LoadingProgressPercentInfo>(GlobalEvent.LoadingProgressPercent, data));
    }




    /// <summary>
    /// 在后台进行游戏更新
    /// </summary>
    /// <param name="gameId"></param>
    /// <param name="onFinsih"></param>
    public void UpdateGameAtConsole(int gameId, Action<bool> onFinsih)
        => StartCoroutine(CoCheckUpdateGameAtConsole(gameId, onFinsih));




    IEnumerator CoCheckUpdateGameAtConsole(int gameId, Action<bool> onFinsih)
    {
        // 编辑器下，且非调试热更新模式下
        if (Application.isEditor && !ApplicationSettings.Instance.IsUseHotfixBundle())
        {
            //非热更模式下不需要热更
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("非热更模式下不需要热更"));
            onFinsih?.Invoke(false);// 不需要热更
            yield break;
        }

        bool isNext = false;


        // 机台长期运行，远端大厅信息文件可以已经更新内容，需重新获取！
        string url = PathHelper.GetAssetBackupWEBURL("Assets/AstBackup/Lobbys/Game Info/lobby_games.json");

        // 非cdn加载
        UnityWebRequest req01 = UnityWebRequest.Get(url + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
        yield return req01.SendWebRequest();
        if (req01.result != UnityWebRequest.Result.Success)
        {
            // 无法下载远程游戏信息文件
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("无法下载远端游戏信息文件"));
            onFinsih?.Invoke(false);
            yield break;
        }

        JArray newLobbyGameInfo =  JArray.Parse(req01.downloadHandler.text);


        string moduleName = LobbyGamesManager.Instance.GetGameValueFrom<string>(newLobbyGameInfo,gameId, "module_name");

        // 获取远端最新的游戏模块（有可能游戏已经下架）
        if (string.IsNullOrEmpty(moduleName))
        {
            // 游戏不存在
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("远端游戏不存在"));
            onFinsih?.Invoke(false);
            yield break;
        }

        bool isAvailable = LobbyGamesManager.Instance.GetGameValueFrom<bool>(newLobbyGameInfo, gameId, "is_available");

        //  如果游戏维护中
        if (!isAvailable)
        {
            // 游戏维护中
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("远端游戏维护中"));
            onFinsih?.Invoke(false);
            yield break;
        }


        // ====如果存在游戏则，下载游戏模块版本文件




        string webPth = PathHelper.GetModuleVersionWEBURL(moduleName);

        JObject webModVerNode = null;

        // 非cdn加载
        UnityWebRequest reqModVerFile = UnityWebRequest.Get(webPth + $"?t={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
        yield return reqModVerFile.SendWebRequest();

        if (reqModVerFile.result != UnityWebRequest.Result.Success)
        {
            // 游戏模块版本文件加载失败 

            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("游戏模块版本文件加载失败."));

            onFinsih?.Invoke(false);
            yield break;
        }
        else 
        {

            string tvStr = reqModVerFile.downloadHandler.text;
            webModVerNode = JObject.Parse(tvStr);


            // 获取网络该游戏的的所有依赖hash值
            Dictionary<string, string> webModHashDis = new Dictionary<string, string>();
            JObject dependenciesNode = webModVerNode["dependencies"] as JObject;
            foreach (KeyValuePair<string, JToken> kv in dependenciesNode)
            {
                webModHashDis.Add(kv.Key, kv.Value["hash"].Value<string>());
            }
            webModHashDis.Add(webModVerNode["name"].Value<string>(), webModVerNode["hash"].Value<string>());

            // 检查已运行的依赖包，是否和热更发生冲突
            foreach (KeyValuePair<string, string> kv in webModHashDis)
            {
                if (ModuleDownloadManager.Instance.runingModHash.ContainsKey(kv.Key) && ModuleDownloadManager.Instance.runingModHash[kv.Key] != kv.Value)
                {
                    // 或者 提示“重启并进行热更新，是否选择重启？”

                    // 模块已经运行，需要重启才能热更新！
                    // 请联系工作人员，更新游戏 [1]
                    TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Please contact staff to update the game.")); //需要重启才能热更新！

                    onFinsih?.Invoke(false);
                    yield break;
                }
            }

            object[] result = ModuleDownloadManager.Instance.GetDownloadTotalSize(moduleName, webModVerNode);

            if ((int)result[0] != 0) //无需下载！！
            {
                // 请联系工作人员，更新游戏
                TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("已是最新版本，无需下载."));

                onFinsih?.Invoke(false);
                yield break;
            }

            long totalSize = (long)result[2];

            // 检测到游戏更新，需下载 500M 资源，是否继续？ 
            bool isConfirmDownload = false;
            CommonPopupHandler.Instance.OpenPopup(new CommonPopupInfo()
            {
                // 只能修改一次线号机台号，确定要修改？
                type = CommonPopupType.YesNo,
                text = string.Format(I18nMgr.T("Game update detected. Download size: {0}. Continue?"), FileUtils.FormatFileSize(totalSize)),
                buttonText1 = I18nMgr.T("Cancel"),
                buttonText2 = I18nMgr.T("OK"),
                callback1 = () =>
                {
                    isNext = true;
                },
                callback2 = () =>
                {
                    isConfirmDownload = true;
                    isNext = true;
                },
            });

            yield return new WaitUntil(() => isNext == true);

            if (!isConfirmDownload) // 不下载
            {
                onFinsih?.Invoke(false);
                yield break;
            }

            // 标记（重启热更一次）
            LobbyGamesManager.Instance.SetValueForCache<bool>(gameId, "update_at_launch_once", true);

            string assetPth = LobbyGamesManager.Instance.GetGameValueFrom<string>(newLobbyGameInfo , gameId, "poster_url");

            PageManager.Instance.OpenPage(PageName.CommonPopupSystemLoading,
                    /*new EventData<Dictionary<string, object>>("", new Dictionary<string, object>()
                {
                    ["title"] = "",
                    ["url"] = string.IsNullOrEmpty(assetPth) ? null: PathHelper.GetAssetBackupWEBURL(assetPth),
                })*/
                    new InParamsPopupSystemLoading()
                    {
                        title = "",
                        url = string.IsNullOrEmpty(assetPth) ? null : PathHelper.GetAssetBackupWEBURL(assetPth),
                    }

                );



            bool isError = false;

            LoadingProgressPercentInfo pgInfo = new LoadingProgressPercentInfo()
            {
                msg = $"start download mod: {moduleName}",
                totalStepCount = 2,
                curStepIndex = 0,
                curStepSubCount = 0,
            };

            // 更新资源， 大厅显示进度条。（通过事件广播出去， 或者通过委托）
            yield return ModuleDownloadManager.Instance.DownLoadNeedModToTemp(moduleName,
            null,
            (startMsg) =>
            {
                DebugUtils.Log(startMsg);
            },
            (endMsg) =>
            {
                DebugUtils.Log(endMsg);
            },
            (ProgressMsg) =>
            {
                DebugUtils.Log(ProgressMsg);

                pgInfo.msg = ProgressMsg;
                pgInfo.curStepSubCount++;
                SendEventLoadingProgress(pgInfo);
            },
            (successMsg) =>
            {
                // 清除标记 
                LobbyGamesManager.Instance.SetValueForCache<bool>(gameId, "update_at_launch_once", false);
            },
            (errorMsg) =>
            {
                Debug.LogError(errorMsg);
                isError = true;
            });


            if (isError) // 热更失败(如断网、断电)
            {
                // 下载失败，请联系工作人员。
                TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Download failed."));

                PageManager.Instance.ClosePage(PageName.CommonPopupSystemLoading);

                onFinsih?.Invoke(false);
                yield break;
            }


            pgInfo.msg = "start copy temp hotfix file to target dir";
            pgInfo.curStepIndex++;
            pgInfo.curStepSubCount = 0;
            SendEventLoadingProgress(pgInfo);

            // 开始拷贝
            PlayerPrefs.SetString(HotfixState.HOTFIX_STATE, HotfixState.HotfixCopying);
            yield return ModuleDownloadManager.Instance.CopyTempWebHotfixFileToTargetDir((msg) =>
            {
                //Debug.LogError($"11111 = {msg}");
                pgInfo.msg = msg;
                pgInfo.curStepSubCount++;
                SendEventLoadingProgress(pgInfo);
            });


            PageManager.Instance.ClosePage(PageName.CommonPopupSystemLoading);



            // 下载成功
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Download successful."));
            // 如果热更成功
            onFinsih?.Invoke(true);
            yield break;
        }


    }

}
