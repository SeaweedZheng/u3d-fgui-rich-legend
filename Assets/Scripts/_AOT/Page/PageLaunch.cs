#define DISABLE_DELAY
using System.Collections.Generic;
using UnityEngine;
using System;
using FairyGUI;
using UnityEditor;



public class PageLaunch 
{
    private static PageLaunch _instance;
    public static PageLaunch Instance
    {
        get
        {
            if (_instance == null)
            {
                if(UIPackage.GetByName("Native") == null)
                    UIPackage.AddPackage("Native/FGUIs/Native"); // Native/FGUIs/ == Resources/Native/FGUIs
                _instance = new PageLaunch();
                _instance.goOwnerPage = UIPackage.CreateObject("Native", "PageLaunch").asCom;
                GRoot.inst.AddChild(_instance.goOwnerPage);
                _instance.goOwnerPage.sortingOrder = 99;
            }
            return _instance;
        }
    }

    GComponent goOwnerPage;

    GLoader lodBG, lodLogo;

    GProgressBar pbLoading;

    GTextField txtMsg;

    GButton btnQuit;

    Dictionary<string, int> allProgress = new Dictionary<string, int>();

    Dictionary<string, int> curProgress = new Dictionary<string, int>();


    public class ShowMsgInfo
    {
        public string msg;
        public float progress = 0;
    }
    List<ShowMsgInfo> msgLst = new List<ShowMsgInfo>();


    private void InitParam(Type tpLoadingProgerss)
    {

        btnQuit = goOwnerPage.GetChild("btnQuit").asButton;
        btnQuit.onClick.Clear();
        btnQuit.onClick.Add(DoAplicationQuit);
        btnQuit.visible = !ApplicationSettings.Instance.isRelease;


        pbLoading = goOwnerPage.GetChild("progress").asProgress;
        txtMsg = goOwnerPage.GetChild("title").asTextField;
        lodLogo = goOwnerPage.GetChild("logo").asLoader;
        lodLogo.url = ApplicationSettings.Instance.logoUrl;

        msgLst.Clear();
        isError = false;
        allProgress.Clear();
        curProgress.Clear();
        pbLoading.value = 0;
        txtMsg.text = "";

        //tpLoadingProgerss = typeof(LoadingProgress);
        var fields = tpLoadingProgerss.GetFields();
        foreach (var fieldInfo in fields)
        {
            allProgress.Add((string)fieldInfo.GetRawConstantValue(), 0);
            curProgress.Add((string)fieldInfo.GetRawConstantValue(), 0);
        }


        Timers.inst.Remove(Update);
        Timers.inst.Add(0.1f, 0, Update);
    }


    void DoAplicationQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 编辑器中退出播放模式
#else
                    Application.Quit(); // 构建后退出应用
#endif
    }

    /// <summary>
    /// <p>获取进度条的值</p>
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// * 将进度条的加载分成多段。<br/>
    /// * 每段进度，再分成多个小段任务。<br/>
    /// </remarks>
    float GetProgressValue()
    {
        float partOne = 1f / (float)(allProgress.Count);

        float A = (float)(allProgress.Count - curProgress.Count) * partOne;

        float B = 0f;

        foreach (KeyValuePair<string, int> kv in curProgress)
        {
            int hasDoNum = allProgress[kv.Key] - kv.Value;
            if (hasDoNum > 0)
            {
                B += (float)hasDoNum * (partOne / (float)allProgress[kv.Key]);
            }
        }
        //Debug.Log($"@ A = {A} , B = {B} , C = {A + B} , partOne = {partOne}");
        return A + B;
    }

    /// <summary>
    /// <p>添加某个进度任务</p>
    /// </summary>
    /// <param name="mark">进度"mark"</param>
    /// <param name="count">该进度的任务个数</param>
    public void AddProgressCount(string mark, int count)
    {
        //新写法(支持重复校验-热更版本文件 - 避免网络连接延时)
        if (!curProgress.ContainsKey(mark))
        {
            curProgress.Add(mark, 0);
        }
        curProgress[mark] += count;
        allProgress[mark] += count;
    }


    /// <summary>
    /// <p>删除某个进度任务</p>
    /// </summary>
    /// <param name="mark">进度"mark"</param>
    public void RemoveProgress(string mark)
    {
        if (curProgress.ContainsKey(mark))
            curProgress.Remove(mark);
    }



    /// <summary>
    /// <p>显示加载进度和信息</p>
    /// </summary>
    /// <param name="mark">进度"mark"</param>
    /// <param name="str">显示的信息</param>
    /// <remarks>
    /// * 仅仅是界面的显示。<br/>
    /// * 不做任何的数据修改。<br/>
    /// </remarks>
    public void Next(string mark, string str)
    {
        if (isError) return;

        if (curProgress.ContainsKey(mark))
        {
            if (--curProgress[mark] < 0)
                curProgress[mark] = 0;
        }
        float val = GetProgressValue();

        msgLst.Add(new ShowMsgInfo()
        {
            msg = CreatStr(str, val),
            progress = val,
        });

#if DISABLE_DELAY
        ShowProgressUIMsg();
#endif
    }


    bool isError = false;
    public void Error(string str)
    {
        isError = true;
        txtMsg.text = str;
    }

    public void Finish(string str)
    {
        msgLst.Add(new ShowMsgInfo()
        {
            msg = CreatStr(str, 1),
            progress = 1,
        });
#if DISABLE_DELAY
        ShowProgressUIMsg();
#endif

    }


    string CreatStr(string str, float pg)
    {
        string _pg = (pg * 100f).ToString("N1");
        return ApplicationSettings.Instance.isRelease ? $"{_pg}%" : $"{str}  ({_pg})%";
    }

    public void Open(Type tpLoadingProgerss)
    {
        goOwnerPage.visible = true;
        InitParam(tpLoadingProgerss);
    }


    Coroutine corClose = null;
    public void Close(float delayS = -1)
    {
        if (delayS > 0)
        {
            DelayToClose(delayS);
            return;
        }
        CloseSelf(null);
    }

  
    void DelayToClose(float delayS)
    {
        Timers.inst.Remove(CloseSelf);
        Timers.inst.Add(delayS, 1, CloseSelf);
    }

    void CloseSelf(object data)
    {
        Timers.inst.Remove(Update);
        goOwnerPage.visible = false;
    }






    float lastRunTimeS = 0;
    public void Update(object data)
    {
#if DISABLE_DELAY
        return;
#endif
        if (isError)
            return;

        float nowRunTimeS = Time.unscaledTime;
        if (nowRunTimeS - lastRunTimeS > 0.2f)
        {
            lastRunTimeS = nowRunTimeS;
            ShowProgressUIMsg();
        }
    }


    float curShowProgress = 0f;
    string curShowMsg = "";
    void ShowProgressUIMsg()
    {
        if (isError)
            return;

        if (msgLst.Count > 0)
        {
            ShowMsgInfo msgInfo = msgLst[0];
            msgLst.RemoveAt(0);

            curShowProgress = msgInfo.progress;
            curShowMsg = msgInfo.msg;

            pbLoading.value = curShowProgress;
            txtMsg.text = curShowMsg;
        }
    }

    /// <summary>
    /// 只显示内容
    /// </summary>
    /// <param name="msg"></param>
    public void RefreshProgressUIMsg(string msg)
    {
        if (isError)
            return;

        curShowMsg = CreatStr(msg, curShowProgress);

        pbLoading.value = curShowProgress;
        txtMsg.text = curShowMsg;
    }
}
