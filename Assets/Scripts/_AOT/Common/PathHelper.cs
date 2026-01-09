using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public static class PathHelper
{

    // 【远程】
    // 资源服务器/游戏平台/total_version.json

    // 游戏平台/debug/android/1/version.json
    // 游戏平台/debug/android/1/AstBundle/
    // 游戏平台/debug/android/1/AstDll/
    // 游戏平台/release/android/1_1_1/version.json
    // 游戏平台/release/android/1_1_1/AstBundle/
    // 游戏平台/release/android/1_1_1/AstDll/


    // 【本地】
    // Application.persistentDataPath/HotfixTmp/version.json
    // Application.persistentDataPath/HotfixTmp/AstBundle/
    // Application.persistentDataPath/HotfixTmp/AstDll/
    // Application.persistentDataPath/Hotfix/version.json
    // Application.persistentDataPath/Hotfix/AstBundle/
    // Application.persistentDataPath/Hotfix/AstDll/
    // Application.persistentDataPath/total_version.json

    // 【包体】
    // Application.streamingAssetsPath/total_version.json
    // Application.streamingAssetsPath/Hotfix/version.json
    // Application.streamingAssetsPath/Hotfix/AstBundle/
    // Application.streamingAssetsPath/Hotfix/AstDll/





    public static string astBundleDirPROJPTH => Application.dataPath + "/AstBundle";
    public static string gameDllDirPROJPTH => Application.dataPath + "/HotFix";



    /* 旧版本打包路劲
    public const string versionName = "version_0.json";
    public const string totalVersionName = "total_version_0.json";
    public string hotfixDirSAPTH => Application.streamingAssetsPath;
    */

    /* 新版本打包路劲 */

    public const string versionName = "version.json";
    public const string totalVersionName = "total_version.json";

    public const string abFolderName = "AstBundle";
    public const string backupFolderName = "AstBackup";
    public const string dllFolderName = "AstDll";


    public const string aotMetaFolderName = "AOTMeta";


    /// <summary> 【多合一】只存打入包中，一起安装的资源（选游戏） </summary>
    public static string hotfixDirSAPTH => Application.streamingAssetsPath + "/Hotfix";


    /// <summary> 【多合一】用来存储所有包 （所有游戏）</summary>
    public static string tmpHotfixDirSAPTH => Application.dataPath + "/StreamingAssetsTmp/Hotfix";



    public static string dllDirSAPTH => Path.Combine(hotfixDirSAPTH, dllFolderName);

    public static string abDirSAPTH => Path.Combine(hotfixDirSAPTH, abFolderName);

    public static string abDirWebPTH => Path.Combine(hotfixDirWEBURL, abFolderName);

    public static string totalVersionSAPTH => Path.Combine(hotfixDirSAPTH, totalVersionName);

    public static string versionSAPTH => Path.Combine(hotfixDirSAPTH, versionName);

    public static string mainfestSAPTH => Path.Combine(abDirSAPTH, mainfestBundleName);

    public static string mainfestBundleName = "AstBundle"; // 这个会和文件夹同名（固定的!）




    /// <summary> 这个网路下载路劲 （是动态获取的！）</summary>
    public static string hotfixDirWEBURL => GlobalModel.autoHotfixUrl;

    public static string hotfixDirLOCPTH => Application.persistentDataPath + "/Hotfix";
    /// <summary> hotfix下载资源临时缓存目录 </summary>
    public static string tmpHotfixDirLOCPTH => Application.persistentDataPath + "/HotfixTmp";


    /// <summary>总版本管理文件路劲 </summary>
    public static string totalVersionWEBURL => ApplicationSettings.Instance.platformResourceServerUrl + "/" + totalVersionName;

    public static string totalVersionLOCPTH => Path.Combine(Application.persistentDataPath, totalVersionName);



    /// <summary>热更新根路径 </summary>
    public static string versionFileWEBURL
    {
        get
        {
            if (string.IsNullOrEmpty(hotfixDirWEBURL))
                return null;
            return $"{hotfixDirWEBURL}{versionName}";
        }
    }



    //const string nameAstBundle = abFolderName;
    //const string nameAstDll = dllFolderName;
    public static string versionLOCPTH => Path.Combine(hotfixDirLOCPTH, versionName);
    public static string tmpVersionLOCPTH => Path.Combine(tmpHotfixDirLOCPTH, versionName);


    public static string tmpMainfestLOCPTH => Path.Combine(tmpABDirLOCPTH, mainfestBundleName);


    public static string mainfestLOCPTH => Path.Combine(abDirLOCPTH, mainfestBundleName);
    public static string mainfestWEBURL => Path.Combine(abDirWEBURL, mainfestBundleName);


    public static string tmpABDirLOCPTH => Path.Combine(tmpHotfixDirLOCPTH, abFolderName);
    //public static string abDirSAPTH => Path.Combine(hotfixDirSAPTH, abFolderName);

    public static string abDirLOCPTH => Path.Combine(hotfixDirLOCPTH, abFolderName);
    public static string abDirWEBURL => Path.Combine(hotfixDirWEBURL, abFolderName);

    public static string tmpDllDirLOCPTH => Path.Combine(tmpHotfixDirLOCPTH, dllFolderName);
    public static string dllDirLOCPTH => Path.Combine(hotfixDirLOCPTH, dllFolderName);


    public static string GetDllWEBURL(string dllName = "Main")
    {
        if (string.IsNullOrEmpty(hotfixDirWEBURL))
            return null;

        if (!dllName.EndsWith(".dll.bytes"))
            dllName = $"{dllName}.dll.bytes";

        return $"{hotfixDirWEBURL}/{dllFolderName}/{dllName}";
    }

    public static string GetTempDllLOCPTH(string dllName)
    {
        if (!dllName.EndsWith(".dll.bytes"))
            dllName = $"{dllName}.dll.bytes";

        return Path.Combine(tmpDllDirLOCPTH, dllName);
    }

    public static string GetDllLOCPTH(string dllName)
    {
        if (!dllName.EndsWith(".dll.bytes"))
            dllName = $"{dllName}.dll.bytes";

        return Path.Combine(dllDirLOCPTH, dllName);
    }

    public static string GetDllSAPTH(string dllName)
    {
        if (!dllName.EndsWith(".dll.bytes"))
            dllName = $"{dllName}.dll.bytes";

        return Path.Combine(dllDirSAPTH, dllName);
    }


    public static string GetAssetBundleWEBPTH(string abName) => Path.Combine(abDirWEBURL, abName);

    public static string GetAssetBundleSAPTH(string abName) => Path.Combine(abDirSAPTH, abName);

    public static string GetAssetBundleLOCPTH(string abName) => Path.Combine(abDirLOCPTH, abName);

    public static string GetTempAssetBundleLOCPTH(string abName) => Path.Combine(tmpABDirLOCPTH, abName);




    #region 资源备份

    public static string backupDirSAPTH => Path.Combine(hotfixDirSAPTH, backupFolderName);
    public static string backupDirLOCPTH => Path.Combine(hotfixDirLOCPTH, backupFolderName);
    public static string backupDirWEBURL => Path.Combine(hotfixDirWEBURL, backupFolderName);

    public static string tmpBackupDirLOCPTH => Path.Combine(tmpHotfixDirLOCPTH, backupFolderName);
    public static string astBackupDirPROJPTH => Application.dataPath + $"/{backupFolderName}";


    public static string GetTempAssetBackupLOCPTH(string nodeName) => Path.Combine(tmpBackupDirLOCPTH, nodeName);

    /*
    public static string GetAstBackupWEBURL(string nodeName = "Cpp Dll/mscatch.dll.bytes") //
    {
        if (string.IsNullOrEmpty(backupDirWEBURL))
            return null;
        return $"{backupDirWEBURL}/{nodeName}";
    }
    */

    public static string GetAssetBackupWEBURL(string pthOrNodeName = "Cpp Dll/mscatch.dll.bytes") // "Assets/AstBackup/Cpp Dll/mscatch.dll.bytes"
    {
        string nodeName = GetAssetBackupNodeName(pthOrNodeName);

        if (string.IsNullOrEmpty(backupDirWEBURL))
            return null;
        return $"{backupDirWEBURL}/{nodeName}";
    }

    public static string GetAssetBackupLOCPTH(string pthOrNodeName = "Assets/AstBackup/Cpp Dll/mscatch.dll.bytes")
    {
        string nodeName = GetAssetBackupNodeName(pthOrNodeName);  // Cpp Dll/mscatch.dll.bytes

        return Path.Combine(backupDirLOCPTH, nodeName);
    }

    public static string GetAssetBackupSAPTH(string pthOrNodeName = "Assets/AstBackup/Cpp Dll/mscatch.dll.bytes")
    {
        string nodeName = GetAssetBackupNodeName(pthOrNodeName);  // Cpp Dll/mscatch.dll.bytes

        return Path.Combine(backupDirSAPTH, nodeName);
    }

    public static string GetAssetBackupPROJPTH(string pthOrNodeName = "Assets/GameBackup/Cpp Dll/mscatch.dll.bytes")
    {
        string nodeName = GetAssetBackupNodeName(pthOrNodeName);  // Cpp Dll/mscatch.dll.bytes

        return Path.Combine(astBackupDirPROJPTH, nodeName);
    }

    public static string GetAssetBackupNodeName(string pthOrNodeName = "Assets/AstBackup/Cpp Dll/mscatch.dll.bytes") // "Cpp Dll/mscatch.dll.bytes"
    {
        pthOrNodeName = pthOrNodeName.Replace("\\", "/");

        if (pthOrNodeName.Contains(backupFolderName)) //   if (pthOrNodeName.StartsWith($"Assets/{FOLDERAstBackup}" ))
        {
            return pthOrNodeName.Substring(pthOrNodeName.IndexOf(backupFolderName) + backupFolderName.Length + 1);  // "Assets/AstBackup/Cpp Dll/mscatch.dll.bytes"
        }
        else
        {
            return pthOrNodeName; // "Cpp Dll/mscatch.dll.bytes"
        }

    }


    #endregion



    #region Modules

    const string moduleFolderName = "Modules";
    public const string mianModuleName = "Main";
    public static string modulesDirSAPTH => Path.Combine(hotfixDirSAPTH, moduleFolderName);
    public static string modulesDirWEBURL => Path.Combine(hotfixDirWEBURL, moduleFolderName);
    // 【注意】public static string modulesDirLOCPTH => Path.Combine(modulesDirLOCPTH, moduleFolderName);  //modulesDirLOCPTH自己调用自己导致闪退  

    public static string modulesDirLOCPTH => Path.Combine(hotfixDirLOCPTH, moduleFolderName);  //modulesDirLOCPTH自己调用自己导致闪退  
    public static string tmpModulesDirLOCPTH => Path.Combine(tmpHotfixDirLOCPTH, moduleFolderName); 



    public static string GetModuleVersionSAPTH(string name) =>  Path.Combine(modulesDirSAPTH, name ,versionName);
    public static string GetModuleVersionWEBURL(string name) => Path.Combine(hotfixDirWEBURL, name, versionName);
    public static string GetModuleVersionLOCPTH(string name) => Path.Combine(modulesDirLOCPTH, name, versionName);
    public static string GetTmpModuleVersionLOCPTH(string name) => Path.Combine(tmpModulesDirLOCPTH, name, versionName);
    public static string mainModVersionSAPTH => Path.Combine(modulesDirSAPTH, mianModuleName, versionName);

    public static string mainModVersionWEBURL => Path.Combine(modulesDirWEBURL, mianModuleName, versionName);

    public static string mainModVersionLOCPTH => Path.Combine(modulesDirLOCPTH, mianModuleName, versionName);

    public static string tmpMainModVersionLOCPTH => Path.Combine(tmpModulesDirLOCPTH, mianModuleName, versionName);
    #endregion

    #region 获取项目资源
    public static string GetAssetPth(string AssetsPth = "Assets/AstBackup/Cpp Dll/mscatch.dll.bytes")
    {
        // E:/work/u3d-fgui-rich-legend/Assets  +  "Assets/AstBackup/Cpp Dll/mscatch.dll.bytes";

        string prefix = "Assets/";
        if (AssetsPth.StartsWith(prefix))
        {
            AssetsPth = AssetsPth.Substring(prefix.Length);
        }

        string pth = Application.dataPath + AssetsPth;  // D:/xxxx/xxxx/Assets/AstBackup/Cpp Dll/mscatch.dll.bytes

        return pth;
    }

    #endregion
}
