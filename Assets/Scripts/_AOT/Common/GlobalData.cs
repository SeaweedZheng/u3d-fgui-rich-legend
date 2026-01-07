using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEditor;
using UnityEngine;

public static class GlobalData 
{
    /// <summary> 是否多次检查热更新 </summary>
    public const string HOTFIX_REQUEST_COUNT_01 = "HOTFIX_REQUEST_COUNT_01";


    public static int hotfixRequestCount
    {
        get
        {
            // 受到保护时默认开启联网
            if (isFirstInstall && ApplicationSettings.Instance.isUseProtectApplication)
            {
                // 旧包已经有了字段  HOTFIX_REQUEST_COUNT
                // 第一次装包时，要强制保存，开启联网
                PlayerPrefs.SetInt(HOTFIX_REQUEST_COUNT_01, 10);
                PlayerPrefs.Save();
                return 10;
            }

            //#seaweed# int defaultCount = ApplicationSettings.Instance.isUseProtectApplication ? 10 : 1;
            int defaultCount = 10;
            if (!PlayerPrefs.HasKey(HOTFIX_REQUEST_COUNT_01))
                PlayerPrefs.SetInt(HOTFIX_REQUEST_COUNT_01, defaultCount);
            int count = PlayerPrefs.GetInt(HOTFIX_REQUEST_COUNT_01, defaultCount);
            return count < 1 ? 1 : count;

        }
    }


    /// <summary> 首次非重复安装本包 </summary>
    public static bool isFirstInstall
    {
        get
        {
            if (_isFirstInstall == null)
            {
                if (streamingAssetsVersion == null)
                {
                    Debug.LogError("streamingAssetsVersion is null, please get streamingAssetsVersion first !");
                    return false;
                }
                else
                {

                    // 如果PlayerPrefs会丢失数据，改成在本地保存一份文本文件（写入json??）
                    // 检查大版本是否发生变化。（删除数据库，恢复为默认配置）
                    string INSTAL_VER = "INSTAL_VER";
                    string lastInstallVerNumber = PlayerPrefs.GetString(INSTAL_VER, "");
                    string curInstallVerNumber = $"{ApplicationSettings.Instance.appKey}-{ApplicationSettings.Instance.appVersion}-{streamingAssetsVersion["hotfix_version"].ToObject<string>()}";
                    bool isFirst = lastInstallVerNumber != curInstallVerNumber;
                    if (isFirst)
                    {
                        PlayerPrefs.SetString(INSTAL_VER, curInstallVerNumber);
                        PlayerPrefs.Save();
                        Debug.LogWarning($"@ is first install: {curInstallVerNumber}");
                    }
                    else
                    {
                        Debug.LogWarning($"@ is not first install: {curInstallVerNumber}");
                    }
                    _isFirstInstall = isFirst;
                }
            }
            return (bool)_isFirstInstall;
        }
    }
    static bool? _isFirstInstall = null;


    /// <summary> 包内版本信息(streamingAssetsVersion) </summary>
    public static JObject streamingAssetsVersion;


    /// <summary> 包内主模块版本信息(streamingAssetsVersion) </summary>
    public static JObject streamingAssetsMainModVersion;



    /*
    /// <summary> 所有模块的版本信息 </summary>
    public static JArray gameStateInfos
    {
        get
        {
            if (_modulesVersionInfos == null)
            {
                TextAsset txa = Resources.Load<TextAsset>("Datas/game_state_default");
                string content = PlayerPrefs.GetString(PARAM_MOD_VER_INFO, txa.text);
                _modulesVersionInfos = JArray.Parse(txa.text);
            }
            return _modulesVersionInfos;
        }
        set
        {
            _modulesVersionInfos = value;
            PlayerPrefs.GetString(PARAM_MOD_VER_INFO,  JsonConvert.SerializeObject(_modulesVersionInfos));
        }
    }
    static JArray _modulesVersionInfos = null;
    const string PARAM_MOD_VER_INFO = nameof(PARAM_MOD_VER_INFO);
    */




    /// <summary> 总版本信息 </summary>
    public static JObject totalVersion;

    /// <summary> 版本信息（主版本）（新版：dll+ab版本） </summary>
    static JObject _version;

    /// <summary> 主模块版本信息（新版：dll+ab版本） </summary>
    static JObject _mainModVersion; 

    /* 主版本文件 或 主模块版本文件*/
    public static JObject version
    {
        get => _version;
        set
        {
            //Debug.LogWarning($"@#@# 333 = {value["hotfix_version"].Value<string>()}");
                _version = value;
        }
    }


    public static JObject mainModVersion
    {
        get => _mainModVersion;
        set
        {
            //Debug.LogWarning($"@#@# 333 = {value["hotfix_version"].Value<string>()}");
            _mainModVersion = value;
        }
    }

    /// <summary> 热更版本 </summary>
    public static string hotfixVersion => ApplicationSettings.Instance.isUseMoudle ?
       mainModVersion["hotfix_version"].Value<string>() : version["hotfix_version"].Value<string>();

    /// <summary> 热更id </summary>
    public static string hotfixKey => ApplicationSettings.Instance.isUseMoudle ?
        mainModVersion["hotfix_key"].Value<string>() : version["hotfix_key"].Value<string>();


    /// <summary> 安卓包内的热更版本 </summary>
    public static string installHofixVersion => streamingAssetsVersion["hotfix_version"].ToObject<string>();


    /// <summary> 动态获取热更地址 </summary>
    public static string autoHotfixUrl ="";


    /// <summary> 获取建议升级的版本号 </summary>
    public static string versionSuggest
    {

        get
        {
            if(totalVersion == null)
                return null;

            JArray lst = totalVersion["data"] as JArray;
            JObject curTotalVersionItem = null;
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i]["app_key"].Value<string>() == ApplicationSettings.Instance.appKey)
                {
                    curTotalVersionItem = lst[i] as JObject;
                    break;
                }
            }

            if (curTotalVersionItem == null)
                return null;

            return curTotalVersionItem["version_suggest"].Value<string>();

        }
    }




    /// <summary>
    /// 保护apk
    /// </summary>
    /// <remarks>
    /// * 热更到更高版本，解除apk保护
    /// </remarks>
    public static bool isProtectApplication
    {
        get
        {
            if (ApplicationSettings.Instance.isUseProtectApplication)
            {
                try
                {
                    Debug.Log($" hotfixVersion: {GlobalData.hotfixVersion}   installHofixVersion: {GlobalData.installHofixVersion}");
                    Version hotfixVersion = new Version(GlobalData.hotfixVersion);
                    Version installHofixVersion = new Version(GlobalData.installHofixVersion);

                    if (installHofixVersion >= hotfixVersion)
                        Debug.LogError("应用受保护!!");

                    return installHofixVersion >= hotfixVersion;// Debug.LogError("应用受保护!!");
                }
                catch (Exception e)
                {
                    return true;
                }

            }
            return false;
        }
    }


}
