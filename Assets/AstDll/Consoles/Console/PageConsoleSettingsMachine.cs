using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameMaker;

namespace ConsoleSlot01
{
    public class PageConsoleSettingsMachine : PageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleSettingsMachine";
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

        GButton btnClose,
            btnCoding;

        GList tabs;
        GComponent tabMachine;

        SettingLineIdMachineIdView lineMachineIdView = new SettingLineIdMachineIdView();
        SettingLineIdMachineIdPresenter lineMachineIdPter = new SettingLineIdMachineIdPresenter();


        SettingPasswordView passwordSettingView = new SettingPasswordView();
        SettingPasswordPresenter passwordSettingPter = new SettingPasswordPresenter();



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


            tabs = this.contentPane.GetChild("tabs").asList;

            GComponent tabMachine = tabs.GetChild("machine").asCom;
            GComponent tabUser = tabs.GetChild("user").asCom;

            lineMachineIdView.InitParam(tabMachine);
            lineMachineIdPter.InitParam(lineMachineIdView);

            passwordSettingView.InitParam(tabUser);
            passwordSettingPter.InitParam(passwordSettingView);


            btnCoding = tabMachine.GetChild("active").asCom.GetChild("value").asButton;
            btnCoding.onClick.Clear();
            btnCoding.onClick.Add(OnClickCoder);
        }


        void OnClickCoder()
        {
            EventCenter.Instance.EventTrigger<EventData>(MachineUIEvent.ON_MACHINE_UI_EVENT,
                new EventData<PageName>(MachineUIEvent.ShowCoding, PageName.ConsolePopupConsoleCoder));
        }
    }


}