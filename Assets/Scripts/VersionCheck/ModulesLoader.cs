
//#define TEST_HOTFIX_0
#define NEW_VER_DLL
using HybridCLR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class ModulesLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        Debug.LogWarning($"isUseMoudle is {ApplicationSettings.Instance.isUseMoudle}");

        if (ApplicationSettings.Instance.isUseMoudle == false)
        {
            gameObject.SetActive(false);
            this.enabled = false;
            yield break;
        }

        PageLaunch.Instance.Open(typeof(LoadingProgressMod));

        OnLoadingBefore();



        yield return ModulesVersionCheck.Instance.DoHotfix(null);

        Debug.Log($"autoHotfixUrl = {GlobalModel.autoHotfixUrl}");
        Debug.Log($"hofixKey = {GlobalModel.hotfixKey}");
        Debug.Log($"hofixVersion = {GlobalModel.hotfixVersion}");

        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.COPY_SA_HOTFIX_DLL);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.COPY_SA_ASSET_BUNDLE);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.COPY_SA_ASSET_BACKUP);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.COPY_SA_ASSET_MOD_VER);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.CHECK_COPY_TEMP_HOTFIX_FILE);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.CHECK_WEB_VERSION);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DOWNLOAD_MOD_MAIN);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DOWNLOAD_MOD_GAME);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DOWNLOAD_MOD_GAME_ONCE);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DELETE_UNUSE_ASSET_BUNDLE);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DELETE_UNUSE_ASSET_DLL);
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.DELETE_UNUSE_ASSET_BACKUP);


        #region  非编辑器，才需要补充元数据（加载dll来补充元数据）
        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.LOAD_AOT_META_DATA, 1);
        if (!Application.isEditor)
        {
            yield return LoadMetadataForAOTAssemblies(); // s_assetDatas 的AOT Meta 加载到“程序集对象”中
        }
        #endregion
        //DllHelper.Instance.AddAotMeta();

        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.LOAD_AOT_META_DATA);


        #region 加载热更程序集到 "程序集对象"中

        List<string> hotfixDlls = DllHelper.Instance.GetDllNameNoSuffixList(GlobalModel.version);

        Assembly ass = null;

        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.LOAD_HOTFIX_DLL, hotfixDlls.Count);

        for (int i = 0; i < hotfixDlls.Count; i++)
        {
            if (Application.isEditor) //读取编译器热更新dll程序集。
            {
                ass = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == hotfixDlls[i]);
            }
            else // 读取本地热更新dll程序集。
            {
                // 加载本地（热更新dll）
                string path = PathHelper.GetDllLOCPTH(hotfixDlls[i]);
                ass = Assembly.Load(File.ReadAllBytes(path));
            }

            DllHelper.Instance.SethotUpdateAss(hotfixDlls[i], ass);  //并缓存程序集

            PageLaunch.Instance.Next(LoadingProgressMod.LOAD_HOTFIX_DLL, $"load hotfix dll: {hotfixDlls[i]} {i}/{hotfixDlls.Count}");
        }

        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.LOAD_HOTFIX_DLL);

        #endregion

        //进入游戏
        OpenMain();
    }

    /// <summary> 加载前 </summary>
    void OnLoadingBefore()
    {
        GameObject goReporter = GOFind.FindObjectIncludeInactive("Reporter");
        if (goReporter != null)
        {
            goReporter.SetActive(!ApplicationSettings.Instance.isRelease);
        }
    }

    private static IEnumerator LoadMetadataForAOTAssemblies()
    {

        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        HomologousImageMode mode = HomologousImageMode.SuperSet;

        PageLaunch.Instance.AddProgressCount(LoadingProgressMod.LOAD_AOT_META_DATA, AOTGenericReferences.PatchedAOTAssemblyList.Count);
        int i = 0;

        //从本地Streaming加载dll
        foreach (string item in AOTGenericReferences.PatchedAOTAssemblyList)
        {
            string aotDllName = $"{item}.bytes";

            UnityWebRequest req = UnityWebRequest.Get(Application.streamingAssetsPath + $"/{PathHelper.aotMetaFolderName}/" + aotDllName);

            PageLaunch.Instance.Next(LoadingProgressMod.LOAD_AOT_META_DATA,
                $"load streamingAssets/{PathHelper.aotMetaFolderName}/{aotDllName} {++i}/{AOTGenericReferences.PatchedAOTAssemblyList.Count}");

            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                byte[] aotMetaBytes = req.downloadHandler.data;

                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(aotMetaBytes, mode);
                //Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
            }
            else
            {
                throw new Exception($"加载AOT元数据报错 {aotDllName}");
            }
        }
        PageLaunch.Instance.RemoveProgress(LoadingProgressMod.LOAD_AOT_META_DATA);
    }

    private void OpenMain()
    {
        Assembly ass = DllHelper.Instance.GetAss("Main");//Main.dll.bytes
        Type t = ass.GetType("Main");
        MethodInfo m = t.GetMethod("MainStart");
        m.Invoke(null, null);
    }


}
