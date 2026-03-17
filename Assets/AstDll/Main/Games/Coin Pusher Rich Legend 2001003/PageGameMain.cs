using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using System;
using SlotMaker;
using SimpleJSON;

namespace CoinPusherRichLegend2001003
{
    public partial class PageGameMain : MachinePageBase //: PageBase
    {
        public const string pkgName = "RichLegend2001003";
        public const string resName = "PageGameMain";


        protected override void OnInit()
        {

            base.OnInit();

            int count = 3;

            Action callback = () =>
            {
                if (--count == 0)
                {
                    isInit = true;
                    InitParam();
                }
            };


            ResourceManager02.Instance.LoadAsset<TextAsset>(
            "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001003/ABs/Datas/slot_machine_config.json",
            (TextAsset data) =>
            {
                configStr = data.text;

                callback();
            });


            // 异步加载资源

            ResourceManager02.Instance.LoadAsset<GameObject>(
            "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001003/Prefabs/Game Controller/Game Main Controller.prefab",
            (GameObject clone) =>
            {
                cloneGameCtrl = clone;
                callback();
            
            });


            ResourceManager02.Instance.LoadAsset<TextAsset>(
            ConfigUtils.GetGameInfoURL(2001003),
            (TextAsset data) =>
            {
                string gameInfoStr = data.text;

                // 游戏信息
                nodeGameInfo = JSONObject.Parse(gameInfoStr);
                DeckCreater.Instance.SetNodLines(nodeGameInfo["pay_lines"].ToString());

                callback();
            });







            machineBtnClickHelper = new MachineButtonClickHelper()
            {
                //shortClickHandler = new Dictionary<MachineButtonKey, Action<MachineButtonInfo>>()
                //{
                //    [MachineButtonKey.BtnSpin] = (info) =>
                //    {
                //        if (PanelController02.isOpenIntroduce == true)
                //        {
                //            return;
                //        }

                //        //DebugUtils.LogError("游戏接受到机台短按的数据：Spin");
                //        EventData<bool> res = new EventData<bool>(PanelEvent.SpinButtonClick, false); // isLongClick
                //        OnClickSpinButton(res);

                //    },
                //    [MachineButtonKey.BtnSwitch] = (info) =>
                //    {
                //        //DebugUtils.LogError("游戏接受到机台短按的数据：Switch");
                //        NetClineBiz.LoginJpConsoleBiz();
                //    }
                //},
                downClickHandler = new Dictionary<MachineButtonKey, Action<MachineButtonInfo>>()
                {
                    [MachineButtonKey.BtnPayTable] = (info) => MainModel.Instance.panel?.OnDownClickHandler(MachineButtonKey.BtnPayTable),
                    [MachineButtonKey.BtnExit] = (info) =>  MainModel.Instance.panel?.OnDownClickHandler(MachineButtonKey.BtnExit),    
                    [MachineButtonKey.BtnSpin] = (info) => MainModel.Instance.panel?.OnDownClickHandler(MachineButtonKey.BtnSpin),
                    [MachineButtonKey.BtnPrev] = (info) =>  MainModel.Instance.panel?.OnDownClickHandler(MachineButtonKey.BtnPrev),
                    [MachineButtonKey.BtnNext] = (info) =>  MainModel.Instance.panel?.OnDownClickHandler(MachineButtonKey.BtnNext),
                    //[MachineButtonKey.BtnBetDown] = (info) =>  MainModel.Instance.panel?.OnDownClickHandler(MachineButtonKey.BtnBetDown),
                    //[MachineButtonKey.BtnBetUp] = (info) => MainModel.Instance.panel?.OnDownClickHandler(MachineButtonKey.BtnBetUp),
                },
                upClickHandler = new Dictionary<MachineButtonKey, Action<MachineButtonInfo>>()
                {
                    [MachineButtonKey.BtnPayTable] = (info) => MainModel.Instance.panel?.OnUpClickHandler(MachineButtonKey.BtnPayTable),
                    [MachineButtonKey.BtnSpin] = (info) => MainModel.Instance.panel?.OnUpClickHandler(MachineButtonKey.BtnSpin),
                    [MachineButtonKey.BtnExit] = (info) => MainModel.Instance.panel?.OnUpClickHandler(MachineButtonKey.BtnExit),
                    [MachineButtonKey.BtnPrev] = (info) => MainModel.Instance.panel?.OnUpClickHandler(MachineButtonKey.BtnPrev),
                    [MachineButtonKey.BtnNext] = (info) =>  MainModel.Instance.panel?.OnUpClickHandler(MachineButtonKey.BtnNext),
                    //[MachineButtonKey.BtnBetUp] = (info) => MainModel.Instance.panel?.OnUpClickHandler(MachineButtonKey.BtnBetUp),
                    //[MachineButtonKey.BtnBetDown] = (info) =>  MainModel.Instance.panel?.OnUpClickHandler(MachineButtonKey.BtnBetDown),
                },

                //longClickHandler = new Dictionary<MachineButtonKey, Action<MachineButtonInfo>>()
                //{
                //    [MachineButtonKey.BtnSpin] = (info) =>
                //    {
                //        DebugUtils.LogError("游戏接受到机台长按的数据：Spin");
                //        EventData<bool> res = new EventData<bool>(PanelEvent.SpinButtonClick, true); // isLongClick
                //        OnClickSpinButton(res);
                //    }
                //}
            };



        }

        GameObject goGameCtrl, cloneGameCtrl;




        public void InitGameCtrl()
        {
            if (goGameCtrl != null) return;


            goGameCtrl = GameObject.Instantiate(cloneGameCtrl);

            //Debug.LogError("创建 Push Game Main Controller");

            goGameCtrl.name = "Game Main Controller";
            goGameCtrl.transform.SetParent(null);

            slotMachineCtrl = goGameCtrl.transform.Find("Slot Machine").GetComponent<SlotMachinePresenter>();

            ContentModel model = goGameCtrl.transform.Find("Blackboard/Content Model").GetComponent<ContentModel>();
            if (model == null)
                DebugUtils.LogError("ContentModel is null");

            //ContentModel.Instance = model;

            mono = goGameCtrl.transform.GetComponent<MonoHelper>();
            //DebugUtils.Log(mono);
            //DebugUtils.LogWarning("i am Game Controller");


            // DebugUtils.LogError("A ContentModel = " + goGameCtrl.transform.Find("Blackboard/Content Model").GetComponent<ContentModel>().transform.name);

            //fguiPoolHelper = goGameCtrl.transform.Find("Pool").GetComponent<FguiPoolHelper>();

            //gObjectPoolHelper = goGameCtrl.transform.Find("GObject Pool").GetComponent<FguiGObjectPoolHelper>();

        }



        public override void OnOpen(PageName name, InParamsBase data)
        {
            base.OnOpen(name, data);


            MainModel.Instance.gameID = 2001003;

            // 添加事件监听
            EventCenter.Instance.AddEventListener<EventData>(PanelEvent.ON_PANEL_INPUT_EVENT, OnPanelInputEvent);
            EventCenter.Instance.AddEventListener<EventData>(SlotMachineEvent.ON_WIN_EVENT, OnWinEvent);
            EventCenter.Instance.AddEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);

            SlotMachineView.Enable();
            rewardPanelCtrl.Enable();
            creditCtr.Enable();


            InitParam();
        }


        public override void OnClose(OutParamsBase data = null)
        {

            EventCenter.Instance.RemoveEventListener<EventData>(PanelEvent.ON_PANEL_INPUT_EVENT, OnPanelInputEvent);
            EventCenter.Instance.RemoveEventListener<EventData>(SlotMachineEvent.ON_WIN_EVENT, OnWinEvent);
            EventCenter.Instance.RemoveEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);

            //         OnContentEvent(SlotMachineEvent.ON_CONTENT_EVENT, new EventData(SlotMachineEvent.BeginSpin));
            // 删除事件监听


            OnGameReset();

            SlotMachineView.Disable();
            rewardPanelCtrl.Disable();
            creditCtr.Disable();


            GameObject.Destroy(goGameCtrl);
            goGameCtrl = null;


            // 退出循环读玩家积分
            Timers.inst.Remove(TaskRepeatGetMayCredit);



            base.OnClose(data);
        }



        public override void OnTop()
        {
            base.OnTop();


            ContentModel.Instance.curGameNumber = MainModel.Instance.gameNumber;
        }


        //string gameInfoStr = null;
        string configStr = null;

        SlotMachineConfig smConfig;
        SlotMachineView2001003 SlotMachineView = new SlotMachineView2001003();
        SlotMachinePresenter slotMachineCtrl;





        MonoHelper mono;
        List<GComponent> lstPayTable;


        MiniReelGroup uiJP1Ctrl = new MiniReelGroup();
        MiniReelGroup uiJP2Ctrl = new MiniReelGroup();
        MiniReelGroup uiJP3Ctrl = new MiniReelGroup();
        MiniReelGroup uiJPMegaCtrl = new MiniReelGroup();


        JSONNode nodeGameInfo;

        WinTipController winTipCtrl = new WinTipController();

        RewardPanelController rewardPanelCtrl = new RewardPanelController();



        GComponent goEffectCoinDrop;

        GTextField txtRewardCoins, txtRemainCoins,
            txtMachineNumber, txtTurnNumber;  //rewardCoins


        UIMyCreditController creditCtr = new UIMyCreditController();
        public override void InitParam()
        {

            if (!isInit) return;


            // 其他对象实例化

            preLoadedCallback?.Invoke();
            preLoadedCallback?.RemoveAllListeners();

            if (!isOpen) return;


            InitGameCtrl();


            smConfig = new SlotMachineConfig(configStr);
            GComponent goSlotMachine = this.contentPane.GetChild("slotMachine").asCom;

            GComponent goAnchorSymbolEffect = this.contentPane.GetChild("anchorSymbolEffect").asCom;
            //GComponent goAnchorSymbolEffect = goSlotMachine.GetChild("slotCover").asCom;  //在画线下
            //GComponent goAnchorSymbolEffect = goSlotMachine.GetChild("payLines").asCom; //在画线上
            SlotMachineView.InitParam(goSlotMachine, goAnchorSymbolEffect, smConfig);
            slotMachineCtrl.InitParam(SlotMachineView, smConfig, true);




            winTipCtrl.InitParam(this.contentPane.GetChild("winTip").asCom, smConfig);


            goEffectCoinDrop = this.contentPane.GetChild("effectCoinDrop").asCom;
            goEffectCoinDrop.visible = false;
            txtRewardCoins = this.contentPane.GetChild("rewardCoins").asCom.GetChild("rewardCoins").asTextField;
            txtRemainCoins = this.contentPane.GetChild("rewardCoins").asCom.GetChild("remainCoins").asTextField;

            txtMachineNumber = this.contentPane.GetChild("turnInfo").asCom.GetChild("machineNumber").asTextField;
            txtMachineNumber.text = SBoxModel.Instance.seatId.ToString();
            txtTurnNumber = this.contentPane.GetChild("turnInfo").asCom.GetChild("turnNumber").asTextField;



            InitUIJackpotGame();

            InitPanel();

            rewardPanelCtrl.InitParam(this.contentPane.GetChild("panel").asCom);

            creditCtr.InitParam(this.contentPane.GetChild("myCredit").asCom.GetChild("credit").asTextField);


            // 循环读玩家积分
            DoTaskRepeatGetMayCredit();

        }
        protected override void OnBeforetLanguageChange(I18nLang lang) { 
        
            foreach(GComponent tab in ContentModel.Instance.goPayTableLst)
            {
                tab.displayObject.gameObject.GetOrAddComponent<GOResidualMark>().referenceCount--;
            }
            ContentModel.Instance.goPayTableLst = null;

            DebugUtils.LogError($"@333 ContentModel.Instance.guid = {ContentModel.Instance.guid}");
        }


        public void InitUIJackpotGame()
        {
            //游戏彩金ui
            uiJP1Ctrl.Init("JP1", this.contentPane.GetChild("jp1").asCom.GetChild("reels").asList, "N0", 16);
            uiJP2Ctrl.Init("JP2", this.contentPane.GetChild("jp2").asCom.GetChild("reels").asList, "N0", 16);
            uiJP3Ctrl.Init("JP3", this.contentPane.GetChild("jp3").asCom.GetChild("reels").asList, "N0", 16);
            uiJPMegaCtrl.Init("JPMega", this.contentPane.GetChild("jp0").asCom.GetChild("reels").asList, "N0", 16);

            uiJP1Ctrl.SetData(3000);
            uiJP2Ctrl.SetData(2000);
            uiJP3Ctrl.SetData(1000);
            uiJPMegaCtrl.SetData(500);
        }

        public void InitPanel()
        {


            //MainModel.Instance.contentMD.goPayTableLst[contentIndex]


            if (ContentModel.Instance.goPayTableLst == null)
            {
                List<GComponent> lstPayTable = new List<GComponent>();

                for(int i=1; i<=2; i++)
                {
                    GComponent paytable = UIPackage.CreateObjectFromURL($"ui://RichLegend2001003/PayTable{i}").asCom;
                    paytable.displayObject.gameObject.GetOrAddComponent<GOResidualMark>().InitParam(paytable);

                    lstPayTable.Add(paytable);
                    paytable.displayObject.gameObject.GetOrAddComponent<GOResidualMark>().referenceCount++;
                }
                ContentModel.Instance.goPayTableLst = lstPayTable;
                
            }

            MainModel.Instance.contentMD = ContentModel.Instance;

            // 面板
            GComponent gOwnerPanel = this.contentPane.GetChild("anchorPanel").asCom;
            ContentModel.Instance.goAnchorPanel = gOwnerPanel;
            EventCenter.Instance.EventTrigger<EventData>(PanelEvent.ON_PANEL_EVENT,
                new EventData<GComponent>(PanelEvent.AnchorPanelChange, gOwnerPanel));
        }



        void OnPanelInputEvent(EventData res)
        {
            switch (res.name)
            {
                case PanelEvent.SpinButtonClick:
                    {
                        OnClickSpinButton(res);
                    }
                    break;
                case PanelEvent.TotalSpinsButtonClick:
                    {
                        //#seaweed# OnClickTotalSpinsButtonClick(res);
                    }
                    break;
            }
        }

        void OnClickSpinButton(EventData res)
        {
            if (res.name != PanelEvent.SpinButtonClick) return;

            bool isLongClick = (bool)res.value;
            switch (ContentModel.Instance.btnSpinState)
            {
                case SpinButtonState.Stop:
                    {
                        if (ContentModel.Instance.isSpin) return; // 已经开始玩直接退出

                        ContentModel.Instance.isSpin = true;

                        Action successCallback = () =>
                        {
                            DebugUtils.Log("游戏结束");
                            ContentModel.Instance.isSpin = false;
                            ContentModel.Instance.btnSpinState = SpinButtonState.Stop;
                            ContentModel.Instance.gameState = GameState.Idle;
                        };

                        if (isLongClick)
                        {
                            TestManager.Instance.ShowTip("Spin按钮 - 长按");

                            ContentModel.Instance.isAuto = true;
                            ContentModel.Instance.btnSpinState = SpinButtonState.Auto;

                            StartGameAuto(successCallback, StopGameWhenError); //自动玩
                        }
                        else
                        {
                            TestManager.Instance.ShowTip("Spin按钮 - 短按");

                            //ContentModel.Instance.btnSpinState = SpinButtonState.Spin;
                            //StartGameOnce(successCallback, StopGameWhenError);//开始玩

                            ContentModel.Instance.btnSpinState = SpinButtonState.Spin;
                            StartGameTotalSpins(successCallback, StopGameWhenError); //开始玩
                        }


                    }
                    break;

                case SpinButtonState.Spin:
                    {
                        // 已经在游戏时，去停止游戏
                        if (!ContentModel.Instance.isSpin) return; // 已经停止直接退出

                        slotMachineCtrl.isStopImmediately = true; // 去停止游戏  


                        smConfig.SelectReelSetting("stop_immediately");
                        smConfig.SelectWinEffectSetting("stop_immediately");
            

                        //SlotGameEffectManager.Instance.SetEffect(SlotGameEffect.StopImmediately);
                    }
                    break;
                case SpinButtonState.Auto:
                    {
                        //停止自动玩
                        ContentModel.Instance.isSpin = true;
                        ContentModel.Instance.isAuto = false;
                        ContentModel.Instance.btnSpinState = SpinButtonState.Spin;
                    }
                    break;
            }
        }




        private void StopGameWhenError(object[] data)
        {
            int code = (int)data[0];
            string msg = data[1] as string;
            ContentModel.Instance.isSpin = false;
            ContentModel.Instance.isAuto = false;
            ContentModel.Instance.btnSpinState = SpinButtonState.Stop;
            ContentModel.Instance.gameState = GameState.Idle;


            // 有好酷优先用好酷
            if (code == ErrorCode.BALANCE_IS_INSUFFICIENT && SBoxModel.Instance.isUseIot)
            {

                if (!DeviceIOTPayment.Instance.isIOTConneted)
                {
                    TipPopupHandler.Instance.OpenPopupOnce(string.Format(I18nMgr.T("IOT connection failed [{0}]"), ErrorCode.DEVICE_IOT_MQTT_NOT_CONNECT));
                }
                else if (!DeviceIOTPayment.Instance.isIOTSignInGetQRCode)
                {
                    TipPopupHandler.Instance.OpenPopupOnce(string.Format(I18nMgr.T("IOT connection failed [{0}]"), ErrorCode.DEVICE_IOT_NOT_SIGN_IN));
                }
                else
                {
                    DeviceIOTPayment.Instance.DoQrCoinIn();
                }
                return;
                
            }
            else
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    string massage = I18nMgr.T(msg);
                    TipPopupHandler.Instance.OpenPopupOnce(massage);
                }
            }

        }




        float skipGetMyCreditTimeS = 0;
        void DoTaskRepeatGetMayCredit()
        {
            Timers.inst.Remove(TaskRepeatGetMayCredit);
            Timers.inst.Add(0.8f, 0, TaskRepeatGetMayCredit);
        }

        void TaskRepeatGetMayCredit(object param)
        {
            if (skipGetMyCreditTimeS - Time.unscaledTime > 0) return;
            
            // #读取玩家积分
            MachineDataManagerG2001003.Instance.RequestGetMyCredit(0, (object res) =>
            {
                int curMyCredit = (int)res;
                if (SBoxModel.Instance.myCredit != curMyCredit)
                {
                    SBoxModel.Instance.myCredit = curMyCredit;
                    MainBlackboardController.Instance.SyncMyTempCreditToReal(true);
                }
            });
        }


        void OnWinEvent(EventData res)
        {
            if (res.name == SlotMachineEvent.TotalWinCredit)
            {
                long totalWinCredit = (long)res.value;
                txtRemainCoins.text = totalWinCredit.ToString();
            }
        }


        protected virtual void OnPropertyChange(EventData res = null)
        {
            //ContentModel
            string name = res.name;
            switch (name)
            {
                case "SBoxModel/seatId":
                    {
                        if(txtMachineNumber != null)
                        {
                            txtMachineNumber.text = SBoxModel.Instance.seatId.ToString();
                        }
                    }
                    break;
                case "ContentModel/curGameNumber":
                    {
                        if (txtTurnNumber != null)
                        {
                            txtTurnNumber.text = ContentModel.Instance.curGameNumber.ToString();
                        }
                    }
                    break;
            }
        }

    }
}