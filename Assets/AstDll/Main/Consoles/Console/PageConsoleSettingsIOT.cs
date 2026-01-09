using FairyGUI;
using GameMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ConsoleSlot01
{
    public class PageConsoleSettingsIOT : PageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleSettingsIOT";
        public override PageType pageType => PageType.Overlay;


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

            callback();

        }

        public override void OnOpen(PageName name, EventData data)
        {
            base.OnOpen(name, data);

            // 添加事件监听
            settingIOTPresenter.Enable();

            InitParam();
        }


        public override void OnClose(EventData data = null)
        {

            // 删除事件监听
            settingIOTPresenter.Disable();

            base.OnClose(data);
        }


        // public override void OnTop() { DebugUtils.Log($"i am top {this.name}"); }

        GButton btnClose;


        SettingIOTView settingIOTView = new SettingIOTView();
        SettingIOTPresenter settingIOTPresenter = new SettingIOTPresenter();

        public override void InitParam()
        {

            if (!isInit) return;

            if (!isOpen) return;

            // btnClose =  this.contentPane.GetChild("btnExit").asButton;
            btnClose = this.contentPane.GetChild("navBottom").asCom.GetChild("btnExit").asButton;
            btnClose.onClick.Clear();
            btnClose.onClick.Add(() =>
            {
                //DebugUtils.Log("i am here 123");
                CloseSelf(null);
                //  CloseSelf(new EventData("Exit"));
            });

            GComponent tab = this.contentPane.GetChild("tabs").asList.GetChildAt(0).asCom;

            settingIOTView.InitParam(tab);
            settingIOTPresenter.InitParam(settingIOTView);
        }
    }

}