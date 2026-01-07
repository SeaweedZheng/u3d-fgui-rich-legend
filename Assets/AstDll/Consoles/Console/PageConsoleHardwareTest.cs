using System;
using GameMaker;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace ConsoleSlot01
{


    public class PageConsoleHardwareTest : MachinePageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleHardwareTest";
        public override PageType pageType => PageType.Overlay;

        protected override void OnInit()
        {
            base.OnInit();

            // 优先
            machineCustomButton.isPriority = true;

            /**/
            machineBtnClickHelper = new MachineButtonClickHelper()
            {
                downClickHandler = new Dictionary<MachineButtonKey, Action<MachineButtonInfo>>()
                {
                    [MachineButtonKey.BtnPayTable] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnPayTable);
                    },
                    [MachineButtonKey.BtnExit] = (info) =>
                    {
                        //OnMachineButtonClickDown(MachineButtonKey.BtnExit);
                    },
                    [MachineButtonKey.BtnSpin] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnSpin);
                    },
                    [MachineButtonKey.BtnPrev] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnPrev);
                    },
                    [MachineButtonKey.BtnNext] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnNext);
                    },
                    [MachineButtonKey.BtnBetDown] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnBetDown);
                    },
                    [MachineButtonKey.BtnBetUp] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnBetUp);
                    },
                    [MachineButtonKey.BtnConsole] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnConsole);
                    },
                    [MachineButtonKey.BtnDoor] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnDoor);
                    },
                    [MachineButtonKey.BtnCreditUp] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnCreditUp);
                    },
                    [MachineButtonKey.BtnCreditDown] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnCreditDown);
                    },
                    [MachineButtonKey.BtnTicketOut] = (info) =>
                    {
                        OnMachineButtonClickDown(MachineButtonKey.BtnTicketOut);
                    },
                },
                upClickHandler = new Dictionary<MachineButtonKey, Action<MachineButtonInfo>>()
                {
                    [MachineButtonKey.BtnPayTable] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnPayTable);
                    },
                    [MachineButtonKey.BtnSpin] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnSpin);
                    },
                    [MachineButtonKey.BtnExit] = (info) =>
                    {
                        //OnMachineButtonClickUp(MachineButtonKey.BtnExit);
                    },
                    [MachineButtonKey.BtnPrev] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnPrev);
                    },
                    [MachineButtonKey.BtnNext] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnNext);
                    },
                    [MachineButtonKey.BtnBetUp] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnBetUp);
                    },
                    [MachineButtonKey.BtnBetDown] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnBetDown);
                    },
                    [MachineButtonKey.BtnConsole] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnConsole);
                    },
                    [MachineButtonKey.BtnDoor] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnDoor);
                    },
                    [MachineButtonKey.BtnCreditUp] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnCreditUp);
                    },
                    [MachineButtonKey.BtnCreditDown] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnCreditDown);
                    },
                    [MachineButtonKey.BtnTicketOut] = (info) =>
                    {
                        OnMachineButtonClickUp(MachineButtonKey.BtnTicketOut);
                    },
                },

            };
            


            isInit = true;
            InitParam();
        }

        public override void OnOpen(PageName name, EventData data)
        {
            base.OnOpen(name, data);

            tabButtonCtrl.Enable();

            InitParam();
        }

        public override void OnClose(EventData data = null)
        {
            tabButtonCtrl.Disable();

            base.OnClose(data);
        }

        GButton btnClose;

        TabHardwareTestButtonController tabButtonCtrl = new TabHardwareTestButtonController();
        TabHardwareTestScreenController tabScreenCtrl = new TabHardwareTestScreenController();
        public override void InitParam()
        {

            if (!isInit) return;

            if (!isOpen) return;


            btnClose = this.contentPane.GetChild("navBottom").asCom.GetChild("btnExit").asButton;
            btnClose.onClick.Clear();
            btnClose.onClick.Add(() =>
            {
                CloseSelf(null);
            });

            GList pages = this.contentPane.GetChild("tabs").asList;

            tabButtonCtrl.InitParam(pages.GetChild("buttons").asCom);

            tabScreenCtrl.InitParam(pages.GetChild("screen").asCom);
        }


        public void OnMachineButtonClickUp(MachineButtonKey btn)
        {
            tabButtonCtrl.OnMachineButtonClickUp(btn);
        }

        public void OnMachineButtonClickDown(MachineButtonKey btn)
        {
            tabButtonCtrl.OnMachineButtonClickDown(btn);
        }

    }

}