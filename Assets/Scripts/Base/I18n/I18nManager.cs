using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using SimpleJSON;
using System.Collections;

public enum I18nLang
{
    /// <summary> 中文简体 </summary>
    cn = 1,
    /// <summary> 英文 </summary>
    en = 2,
    /// <summary> 台湾繁体 </summary>
    tw = 3,
    /// <summary> 香港 </summary>
    hk = 4,
    /// <summary> 日语 </summary>
    jp = 5,
    /// <summary> 韩语 </summary>
    kor = 6,
    /// <summary> 印尼 </summary>
    id = 7,
    /// <summary> 俄罗斯 </summary>
    ru = 8,
    /// <summary> 泰文 </summary>
    th = 9,
    /// <summary> 马来西亚 </summary>
    mys = 10,
    /// <summary> 越南 </summary>
    vie = 11,
}


public class I18nManager : MonoSingleton<I18nManager>
{


    public const string I18N = "I18N";

    Dictionary<string, Dictionary<string, string>> i18nIDs
        = new Dictionary<string, Dictionary<string, string>>();

    Dictionary<string, JSONNode> i18nDatas = new Dictionary<string, JSONNode>();


    I18nLang _language = I18nLang.en;

    public I18nLang language
    {
        get => _language;
    }


    /* 【旧的写法】
    public JSONNode LoadData(string file)
    {

        JSONNode target = null;
        if (ApplicationSettings.Instance.IsUseHotfixBundle())
        {

            //AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AstBundle/luban/generatedatas/bytes/" + file + ".unity3d");

            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(PathHelper.abDirLOCPTH, "luban/generatedatas/bytes/" + file + ".unity3d"));
            TextAsset textAsset = assetBundle.LoadAsset<TextAsset>(file + ".json");
            target = JSON.Parse(textAsset.text);
            assetBundle.Unload(false);
            return target;
        }
        else
        {
#if UNITY_EDITOR
            target = JSON.Parse(File.ReadAllText(Application.dataPath + "/AstBundle/LuBan/GenerateDatas/bytes/" + file + ".json", System.Text.Encoding.UTF8));
#endif
        }
        return target;
    }

  */



    /*
    Coroutine coOnAddData;
    IEnumerator DelayTask(Action task, int timeMS)
    {
        yield return new WaitForSeconds((float)timeMS / 1000f);
        task?.Invoke();
    }
    */



    public void LoadData(string assetPathOrBundleName, Action<JSONNode> onFinishCallback)
    {
        ResourceManager02.Instance.LoadAsset<TextAsset>(assetPathOrBundleName, (textAsset) =>
        {
            JSONNode targetNode = JSON.Parse(textAsset.text);
            onFinishCallback?.Invoke(targetNode);
        });
    }

    string GetFileName(string filePath)
    {
        string[] strs = filePath.Replace("\\", "/").Split('/');
        string fileName = strs[strs.Length-1].Split('.')[0];
        return fileName;
    }


    int loadCount = 0;
    Queue<Action> loadFinishCallbacks = new Queue<Action>();

    void AddI18nJson(string filePath, Action onFinishCallback = null)
    {
        string fileName = GetFileName(filePath);

        if (i18nDatas.ContainsKey(fileName))
        {
            onFinishCallback?.Invoke();
            return;
        }

        if (onFinishCallback !=null)
        {
            loadFinishCallbacks.Enqueue(onFinishCallback);
        }

        loadCount++;

        LoadData(filePath, (res) =>
        {
            Dictionary<string, string> idMap = new Dictionary<string, string>();
            foreach (JSONNode node in res)
            {
                try
                {
                    idMap.Add((string)node["en"], $"{fileName}.{(long)node["id"]}");
                }
                catch (Exception e)
                {
                    DebugUtils.LogWarning($"【多语言】：值重复： {(string)node["en"]}");
                    //DebugUtil.LogException(e);  
                }
                try
                {
                    idMap.Add((string)node["key"], $"{fileName}.{(long)node["id"]}");
                }
                catch (Exception e)
                {
                    DebugUtils.LogWarning($"【多语言】：键重复： {(string)node["key"]}");
                    //DebugUtil.LogException(e);
                }
            }

            i18nIDs.Add(fileName, idMap);
            i18nDatas.Add(fileName, res);


            // 单重复加载多个文件时，这里会触发多次
            //Debug.LogWarning($"############# 加载文件{fileName}，触发多语言事件");
            // EventCenter.Instance.EventTrigger<I18nLang>(I18N, this._language);  

            //onFinishCallback?.Invoke();

            if(--loadCount == 0)
            {
                while (loadFinishCallbacks.Count > 0)
                {
                    Action func = loadFinishCallbacks.Dequeue();
                    func?.Invoke();
                }
            }
        });
    }

    void DeleteI18nJson(string filePath)
    {
        string fileName = GetFileName(filePath);
        this.i18nIDs.Remove(fileName);
        this.i18nDatas.Remove(fileName);
    }

    public void ChangeLanguage(I18nLang language, bool isForce = false)
    {
        if (!isForce && this._language == language)
            return;
        this._language = language;

        EventCenter.Instance.EventTrigger<I18nLang>(I18N, this._language);
    }

    public string T(string valueOrKey)
    {
        var values = i18nIDs.Values;
        for (int i = 0; i < values.Count; i++)
        {
            Dictionary<string, string> map = values.ElementAt(i);
            if (map.ContainsKey(valueOrKey))
            {

                string[] info = map[valueOrKey].Split('.');
                string fileName = info[0];
                long id = long.Parse(info[1]);
                JSONNode rows = i18nDatas[fileName];
                for (int j = 0; j < rows.Count; j++)
                {
                    if ((long)rows[j]["id"] == id)
                        return (string)rows[j][Enum.GetName(typeof(I18nLang), language)];
                }
            }
        }
        DebugUtils.LogWarning($"i18n is not find : {valueOrKey}");
        return valueOrKey;
    }





#region 针对多游戏动态加载和卸载

    List<I18nJsonInfo> i18nJsnInfos = new List<I18nJsonInfo>();
    public void AddI18nJsonInfo(string pth, string mark, Action onFinishCallback)
    {
        foreach (I18nJsonInfo item in i18nJsnInfos)
        {
            if (item.path == pth)
            {
                onFinishCallback?.Invoke();
                return;
            }
        }

        i18nJsnInfos.Add(new I18nJsonInfo()
        {
            mark = mark,
            path = pth,
        });

        AddI18nJson(pth, onFinishCallback);
    }
    public void RemoveI18nJsonInfoByMark(string mark)
    {
        int index = i18nJsnInfos.Count;
        while (--index >= 0)
        {
            if (i18nJsnInfos[index].mark == mark)
            {
                DeleteI18nJson(i18nJsnInfos[index].path);

                i18nJsnInfos.RemoveAt(index);
            }
        }
    }

    public void RemoveI18nJsonInfoByPath(string path)
    {
        int index = i18nJsnInfos.Count;
        while (--index >= 0)
        {
            if (i18nJsnInfos[index].path == path)
            {
                DeleteI18nJson(path);

                i18nJsnInfos.RemoveAt(index);
            }
        }
    }
    #endregion
}


public static class I18nMgr
{
    public static string I18N => I18nManager.I18N;

    public static string T(string valueOrKey) => I18nManager.Instance.T(valueOrKey);

    public static void ChangeLanguage(I18nLang language, bool isForce = false) =>
        I18nManager.Instance.ChangeLanguage(language, isForce);

    // public static void Delete(string filePath) => I18nManager.Instance.RemoveI18nJsonInfoByPath(filePath);
    public static void Delete(string mark) => I18nManager.Instance.RemoveI18nJsonInfoByMark(mark);
    public static I18nLang language => I18nManager.Instance.language;

    public static void Add(string filePath,string mark, Action onFinishCallback) => I18nManager.Instance.AddI18nJsonInfo(filePath, mark, onFinishCallback);

}