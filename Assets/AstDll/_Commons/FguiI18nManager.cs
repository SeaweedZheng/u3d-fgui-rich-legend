using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FairyGUI;
using System;
using SBoxApi;
using GameMaker;
using System.Xml.Linq;
using FairyGUI.Utils;

public partial class FguiI18nManager : MonoSingleton<FguiI18nManager>
{

#if false
    Dictionary<I18nLang, string> langFile = new Dictionary<I18nLang, string>()
    {
        [I18nLang.en] = "Assets/GameRes/Games/Console/Abs/lang_en.xml",
        [I18nLang.cn] = "Assets/GameRes/Games/Console/Abs/lang_cn.xml",
        [I18nLang.tw] = "Assets/GameRes/Games/Console/Abs/lang_tw.xml",
    };

    private Dictionary<I18nLang, string> langData = new Dictionary<I18nLang, string>();

        // [Button]
    void SetI18n(I18nLang lang, Action onFinishCallback)
    {
        //if (I18nMgr.language == lang) return;

        //PlayerPrefs.SetString(PARAM_I18N_LANG, Enum.GetName(typeof(I18nLang), lang));
        //SBoxModel.Instance.language = Enum.GetName(typeof(I18nLang), lang);

        if (langData.ContainsKey(lang))
        {
            SetLanguage(lang, langData[lang], onFinishCallback);
            return;
        }

        if (langFile.ContainsKey(lang))
        {
            ResourceManager02.Instance.LoadAsset<TextAsset>(langFile[lang], (res) =>
            {
                langData.Add(lang, res.text);
                SetLanguage(lang, res.text, onFinishCallback);
            });
        }
    }

#endif

    private const string PARAM_I18N_LANG = "PARAM_I18N_LANG";
    private void Awake()
    {
        //string defaultLangStr = Enum.GetName(typeof(I18nLang), I18nLang.cn);
        //string langStr = PlayerPrefs.GetString(PARAM_I18N_LANG, defaultLangStr);

        //string langStr = SBoxModel.Instance.language;
        //DebugUtils.LogError($"默认的语言： {langStr}");
        //SetI18n((I18nLang)Enum.Parse(typeof(I18nLang), langStr));
    }


    public I18nLang language => I18nManager.Instance.language;
    public void ChangeLanguage(I18nLang lang, Action onFinishCallback = null) => SetI18n_02(lang, onFinishCallback);


    void SetFguiXML(string xmlData)
    {
        if (!string.IsNullOrEmpty(xmlData))
        {
            //DebugUtils.Log(xmlAsset.text);
            FairyGUI.Utils.XML xml = new FairyGUI.Utils.XML(xmlData);
            UIPackage.SetStringsSource(xml);
            FguiI18nTextAssistant.Instance.LoadFromXML(xml);
        }
    }

    void SetLanguage(I18nLang lang, string xmlData, Action onFinishCallback)
    {

        UIPackage.branch = Enum.GetName(typeof(I18nLang), lang);  //分支 

        SetFguiXML(xmlData);

        I18nMgr.ChangeLanguage(lang);

        onFinishCallback?.Invoke();
    }


    void SetI18n_02(I18nLang lang, Action onFinishCallback)
    {
        if (i18nXmlInfos.ContainsKey(lang) && i18nXmlInfos[lang].Count > 0)
        {
            List<I18nXMLInfo> lst = i18nXmlInfos[lang];
            MultipleFguiXml2SingleXml(lang, lst, (totalXmlStr) =>
            {
                SetLanguage(lang, totalXmlStr, onFinishCallback);
            });
        }
        else
        {
            SetLanguage(lang, null, onFinishCallback);
        }
    }


    public void Add(I18nLang lang, string pth, string mark, Action OnFinishCallback) => AddI18nXmlInfo(lang, pth, mark, OnFinishCallback);

    public void Delete(string mark) => RemoveI18nXmlInfo(mark);

}








public partial class FguiI18nManager
{


    Coroutine coCreatTotalXml;
    IEnumerator DelayTask(Action task, int timeMS)
    {
        yield return new WaitForSeconds((float)timeMS / 1000f);
        task?.Invoke();
    }




    public Dictionary<I18nLang, TotalXMLInfo> totalLangData = new Dictionary<I18nLang, TotalXMLInfo>();
    public Dictionary<I18nLang, List<I18nXMLInfo>> i18nXmlInfos = new Dictionary<I18nLang, List<I18nXMLInfo>>();


    int loadCount = 0;
    Queue<Action> loadFinishCallbacks = new Queue<Action>();

    void AddI18nXmlInfo(I18nLang lang, string pth, string mark, Action onFinishCallback)
    {
        if (!i18nXmlInfos.ContainsKey(lang))
            i18nXmlInfos.Add(lang, new List<I18nXMLInfo>());

        foreach (I18nXMLInfo item in i18nXmlInfos[lang])
        {
            if (item.path == pth)
            {
                onFinishCallback?.Invoke();
                return;
            }
        }

        i18nXmlInfos[lang].Add(new I18nXMLInfo()
        {
            mark = mark,
            path = pth,
        });


        if (lang != I18nMgr.language)
        {
            onFinishCallback?.Invoke();
            return; // 非当前语言，不加载totoalXml
        }

        if (onFinishCallback != null)
            loadFinishCallbacks.Enqueue(onFinishCallback);



        if (coCreatTotalXml != null)
            StopCoroutine(coCreatTotalXml);
        coCreatTotalXml = StartCoroutine(DealyAddTotalXml(lang));
#if fasle //【Bug】xml加载需要时间
        coCreatTotalXml = StartCoroutine(DelayTask( () => {
            coCreatTotalXml = null;

            // 延时合并多个xml
            List<I18nXMLInfo> lst = i18nXmlInfos[lang];
            MultipleFguiXml2SingleXml(lang, lst, (totalXmlStr) =>
            {
                SetFguiXML(totalXmlStr);

                while (loadFinishCallbacks.Count > 0)
                {
                    Action func = loadFinishCallbacks.Dequeue();
                    func?.Invoke();
                }
            }); ;
        }, 100));
#endif
    }

    IEnumerator DealyAddTotalXml(I18nLang lang)
    {
        bool isNext = false;
        yield return new WaitForSecondsRealtime(0.1f); // 等待100ms


        // 延时合并多个xml
        List<I18nXMLInfo> lst = i18nXmlInfos[lang];
        MultipleFguiXml2SingleXml(lang, lst, (totalXmlStr) =>
        {
            SetFguiXML(totalXmlStr);

            isNext = true;
        });

        yield return new WaitUntil(() => isNext == true);
        isNext = false;

        yield return new WaitForSecondsRealtime(0.2f); // 等待xml加载完成

        while (loadFinishCallbacks.Count > 0)
        {
            Action func = loadFinishCallbacks.Dequeue();
            func?.Invoke();
        }

        coCreatTotalXml = null;
    }


    void RemoveI18nXmlInfo(string mark)
    {
        foreach (List<I18nXMLInfo> lst in i18nXmlInfos.Values)
        {
            int index = lst.Count;
            while (--index >= 0)
            {
                if (lst[index].mark == mark)
                {
                    lst.RemoveAt(index);
                }
            }
        }
    }

    public long GetXmlTotalHash(I18nLang lang)
    {
        long hash = 0;
        foreach (I18nXMLInfo item in i18nXmlInfos[lang])
        {
            hash += item.path.GetHashCode();
        }
        return hash;
    }


    /// <summary>
    /// 多个fgui项目的"语言A"xml文件，合并成同一个
    /// </summary>
    public void MultipleFguiXml2SingleXml(I18nLang lang, List<I18nXMLInfo> infos, Action<string> onFinishCallback)
    {

        long totalHash = GetXmlTotalHash(lang); //获取哈希值

        if (!totalLangData.ContainsKey(lang))
            totalLangData.Add(lang, new TotalXMLInfo());

        if (totalLangData[lang].hash == totalHash)
        {
            onFinishCallback?.Invoke(totalLangData[lang].content);
            return;
        }


        List<string> xmlStr = new List<string>();

        int count = infos.Count;

        string startTag = "<resources>";
        string endTag = "</resources>";

        string totalContent = "";

        Action onCallback = () =>
        {
            if (--count == 0)
            {

                foreach (string str in xmlStr)
                {
                    int startTagIndex = str.IndexOf(startTag);
                    int startPos = startTagIndex + startTag.Length;
                    int endTagIndex = str.IndexOf(endTag, startPos);
                    string result = str.Substring(startPos, endTagIndex - startPos);
                    totalContent += result;
                }
                totalContent = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<resources> {totalContent} \r\n</resources>";

                Debug.Log("totalContent = " + totalContent);

                totalLangData[lang] = new TotalXMLInfo()
                {
                    hash = totalHash,
                    content = totalContent,
                };

                onFinishCallback?.Invoke(totalContent);
            }
        };

        foreach (I18nXMLInfo item in infos)
        {
            ResourceManager02.Instance.LoadAsset<TextAsset>(item.path, (res) =>
            {
                string xmlData = res.text;
                xmlStr.Add(xmlData);

                onCallback();
            });
        }

    }


}


