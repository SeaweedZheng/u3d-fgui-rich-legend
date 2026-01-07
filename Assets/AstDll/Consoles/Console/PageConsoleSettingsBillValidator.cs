using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameMaker;


namespace ConsoleSlot01
{
    public class PageConsoleSettingsBillValidator : PageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleSettingsBillValidator";
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
            srttingBilllValidatorPresenter.Enable();

            InitParam();
        }


        public override void OnClose(EventData data = null)
        {

            // 删除事件监听
            srttingBilllValidatorPresenter.Disable();

            base.OnClose(data);
        }


        GButton btnClose;

        SettingBillValidatorView settingBillValidatorView = new SettingBillValidatorView();
        SettingBillValidatorPresenter srttingBilllValidatorPresenter = new SettingBillValidatorPresenter();

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


            settingBillValidatorView.InitParam(tab);
            srttingBilllValidatorPresenter.InitParam(settingBillValidatorView);
        }
    }


}