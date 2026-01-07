using FairyGUI;
using GameMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ConsoleSlot01
{
    public class PageConsoleSettingsScreen : PageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleSettingsScreen";
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

            callback();

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
            btnClose.onClick.Add(() =>
            {
                //DebugUtils.Log("i am here 123");
                CloseSelf(null);
                //  CloseSelf(new EventData("Exit"));
            });

        }
    }
}