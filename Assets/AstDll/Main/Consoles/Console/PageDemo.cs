using FairyGUI;
using GameMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PageDemo : PageBase
{
    public const string pkgName = "Console";
    public const string resName = "PageDemo";

    /// <summary>
    /// 实例创建前
    /// </summary>
    /// <param name="onFinishCallback"></param>
    /// <remarks>
    /// * 用来加载多语言文件（xml、json）
    /// </remarks>
    public static void OnBeforeCreat(Action onFinishCallback)
    {
        int count = 3;
        Action xmlFinishCB = () => {
            if (--count == 0)
            {
                DebugUtils.LogWarning($"{resName} : before creat");
                onFinishCallback?.Invoke();
            }
        };

        Action jsonFinishCB = () =>
        {
            FguiI18nManager.Instance.Add(I18nLang.cn,
                "Assets/AstBundle/Consoles/Console Coin Pusher 01/ABs/I18ns/language_cn_console_coin_pusher_01.xml",
                pkgName,
                xmlFinishCB);
            FguiI18nManager.Instance.Add(I18nLang.tw,
                "Assets/AstBundle/Consoles/Console Coin Pusher 01/ABs/I18ns/language_tw_console_coin_pusher_01.xml",
                pkgName,
                xmlFinishCB);
            FguiI18nManager.Instance.Add(I18nLang.en,
                "Assets/AstBundle/Consoles/Console Coin Pusher 01/ABs/I18ns/language_en_console_coin_pusher_01.xml",
                pkgName,
                xmlFinishCB);
        };


        I18nMgr.Add("Assets/AstBundle/Consoles/Console Coin Pusher 01/ABs/I18ns/i18nConsoleCoinPusher01.json", pkgName, jsonFinishCB);

        // DebugUtils.LogWarning($"{resName} : before creat");
        // onFinishCallback?.Invoke();
    }

    protected override void OnInit()
    {
        
        base.OnInit();

        int count = 1;

        Action callback = () =>
        {
            if (--count == 0)
            {
                isInit = true;
                InitParam();
            }
        };

        // 异步加载资源

        ResourceManager02.Instance.LoadAsset<GameObject>(
        "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/Prefabs/Game Controller/Push Game Main Controller.prefab",
        (GameObject clone) =>
        {
            callback();
        });

    }

    public override void OnOpen(PageName name, EventData data)
    {
        base.OnOpen(name, data);

        // 添加事件监听

        InitParam();
    }


    public override void OnClose(EventData data = null)
    {

        // 删除事件监听

        base.OnClose(data);
    }


    // public override void OnTop() { DebugUtils.Log($"i am top {this.name}"); }

    GButton btnClose;

    

    public override void InitParam()
    {

        if (!isInit) return;

        if (!isOpen) return;

        // btnClose =  this.contentPane.GetChild("btnExit").asButton;
        btnClose = this.contentPane.GetChild("navBottom").asCom.GetChild("btnExit").asButton;
        btnClose.onClick.Clear();
        btnClose.onClick.Add(() => {
            //DebugUtils.Log("i am here 123");
            CloseSelf(null);
            //  CloseSelf(new EventData("Exit"));
        });


        /* 
        if (inParams != null)
        {   
            Dictionary<string, object> argDic = null;
            argDic = (Dictionary<string, object>)inParams.value;
            title = (string)argDic["title"];
            isPlaintext = (bool)argDic["isPlaintext"];
            if (argDic.ContainsKey("content"))
            {
                input = (string)argDic["content"];
            }
        }
       */
    }
}
