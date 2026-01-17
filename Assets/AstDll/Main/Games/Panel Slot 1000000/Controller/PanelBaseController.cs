using FairyGUI;
using GameMaker;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PanelSlot1000000
{

    public class PanelBaseController : MonoBehaviour, IPanel
    {
        enum PopState
        {
            None,
            Change,
            Help,
            Bet,
            payTable,
        }

        PopState popState = PopState.None;
        protected GComponent gOwnerPanel, gPayTableWin,
            //setPanel, 
             payTableContent;

        GButton btnSound, btnHelp;
        protected SpinButtonBaseController spinBtnCtrl = new SpinButtonBaseController();
        protected GButton btnPayTable, btnPrev, btnNext;
        protected GTextField txtBet, txtWin;
        //protected bool isSet;

        GameObject goSpinClone;
        bool isInit;
        /// <summary> 浠嬬粛椤电储寮?</summary>
        public int contentIndex;


        protected virtual int IntroduceIndexMax => 6;

        protected virtual void OnEnable()
        {
            EventCenter.Instance.AddEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
            EventCenter.Instance.AddEventListener<EventData>(SlotMachineEvent.ON_WIN_EVENT, OnTotalWinCredit);
            EventCenter.Instance.AddEventListener<EventData>(MetaUIEvent.ON_CREDIT_EVENT, OnUpdateNaviCredit);
            EventCenter.Instance.AddEventListener<EventData>(PanelEvent.ON_PANEL_EVENT, OnPanelEventAnchorPanelChange);

            MainModel.Instance.panel = this;

            Init();
        }


        protected virtual void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
            EventCenter.Instance.RemoveEventListener<EventData>(SlotMachineEvent.ON_WIN_EVENT, OnTotalWinCredit);
            EventCenter.Instance.RemoveEventListener<EventData>(MetaUIEvent.ON_CREDIT_EVENT, OnUpdateNaviCredit);
            EventCenter.Instance.RemoveEventListener<EventData>(PanelEvent.ON_PANEL_EVENT, OnPanelEventAnchorPanelChange);
            gOwnerPanel.visible = false;
        }




        public virtual void Init(EventData res = null)
        {

            GComponent _goAnchorPanel = null;
            if (res != null)
                _goAnchorPanel = res.value as GComponent;
            else if (MainModel.Instance.contentMD != null)
                _goAnchorPanel = MainModel.Instance.contentMD.goAnchorPanel;

            if (_goAnchorPanel == null)
            {
                return;
            }


            int count = 2;
            Action loadComplete = () =>
            {
                if (--count == 0)
                {
                    isInit = true;
                    InitParam();
                }
            };

            string urlStr = $"ui://{nameof(PanelSlot1000000)}/Panel";
            //Debug.Log("urlStr" + urlStr);
            if (gOwnerPanel != _goAnchorPanel && _goAnchorPanel != null)
            {
                if (UIPackage.GetByName(nameof(PanelSlot1000000)) == null)
                {
                    ResourceManager02.Instance.LoadAssetBundleAsync("Assets/AstBundle/Games/Panel Slot 1000000/FGUIs", (ab) =>
                    {
                        UIPackage.AddPackage(ab);
                        if (_goAnchorPanel.GetChild("icon") == null)
                        {
                            Debug.LogError("i null holder");
                        }
                        GLoader anchorPanel = _goAnchorPanel.GetChild("icon").asLoader;
                        GObject[] chlds = _goAnchorPanel.GetChildren();
                        foreach (GObject ch in chlds)
                        {
                            Debug.Log(ch.name);
                        }
                        anchorPanel.url = urlStr;
                        anchorPanel.visible = true;

                        // 获取url实例化后的对象
                        gOwnerPanel = anchorPanel.component;
                        gOwnerPanel.touchable = true;

                        loadComplete();
                    });

                }
                else
                {
                    GLoader anchorPanel = _goAnchorPanel.GetChild("icon").asLoader;
                    anchorPanel.url = urlStr;
                    anchorPanel.visible = true;

                    // 获取url实例化后的对象
                    gOwnerPanel = anchorPanel.component;
                    gOwnerPanel.touchable = true;

                    loadComplete();
                }
            }
            else
            {
                loadComplete();
            }


            if(goSpinClone == null)
                ResourceManager02.Instance.LoadAsset<GameObject>("Assets/AstBundle/Games/Panel Slot 1000000/Prefabs/Slot Btn Spin.prefab",
                    (GameObject clone) =>
                    {
                        goSpinClone = clone;
                        loadComplete();
                    });
            else
            {
                loadComplete();
            }

        }



        protected virtual void InitParam()
        {
            gOwnerPanel = MainModel.Instance.contentMD.goAnchorPanel.asCom.GetChild("icon").asLoader.component;
            //setPanel = gOwnerPanel.GetChild("setPanel").asCom;
            gOwnerPanel.GetChild("credit").asTextField.text = MainModel.Instance.myCredit.ToString(); //SBoxModel.Instance.myCredit.ToString();




            txtWin = gOwnerPanel.GetChild("win").asTextField;
            txtWin.text = 9.ToString();


            btnBetUp = gOwnerPanel.GetChild("btnBetUp").asButton;
            btnBetUp.onClick.Clear();
            btnBetUp.onClick.Add(OnClickButtonBetUp);


            btnBetDown = gOwnerPanel.GetChild("btnBetDown").asButton;
            btnBetDown.touchable = false;
            btnBetDown.GetChild("untouch").visible = true;
            btnBetDown.onClick.Clear();
            btnBetDown.onClick.Add(OnClickButtonBetDown);


            txtBet = gOwnerPanel.GetChild("bet").asTextField;
            txtBet.text = SBoxModel.Instance.betList[MainModel.Instance.contentMD.betIndex].ToString();
            spinBtnCtrl.InitParam(gOwnerPanel.GetChild("btnSpin").asCom, "Stop", OnClickSpinButton, goSpinClone);


            gPayTableWin = gOwnerPanel.GetChild("payTable").asCom;

            payTableContent = gPayTableWin.GetChild("content").asCom;
            if(MainModel.Instance.contentMD.goPayTableLst.Length > 0)
                payTableContent.AddChild(MainModel.Instance.contentMD.goPayTableLst[contentIndex]);

            btnPrev = gPayTableWin.GetChild("btnController").asCom.GetChild("btnPrev").asButton;
            btnPrev.onClick.Clear();
            btnPrev.onClick.Add(OnClickIntroduceL);


            btnNext = gPayTableWin.GetChild("btnController").asCom.GetChild("btnNext").asButton;
            btnNext.onClick.Clear();
            btnNext.onClick.Add(OnClickIntroduceR);


            btnHelp = gOwnerPanel.GetChild("btnHelp").asButton;
            btnHelp.GetChild("untouch").visible = false;
            btnHelp.touchable = true;
            btnHelp.onClick.Set(() => {
                DebugUtils.Log("i am btnHelp");
            });
            btnHelp.onChanged.Clear();
            btnHelp.onChanged.Add(OnClickHelp);


            GComponent gMenu = btnHelp.GetChild("menu").asCom;
            btnPayTable = gMenu.GetChild("btnPayTable").asButton;
            btnPayTable.onClick.Clear();
            btnPayTable.onClick.Add(() =>
            {
                IntroduceInit();
                GlobalSoundHelper.Instance.PlaySoundEff(GameMaker.SoundKey.PopupOpen);
                gPayTableWin.visible = true;
                //setPanel.visible = false;
                btnHelp.selected = false;
            });



            btnSound = gMenu.GetChild("btnSound").asButton;
            btnSound.onClick.Set((EventContext context) =>
            {
                if (++SBoxModel.Instance.soundLevel > 3)
                    SBoxModel.Instance.soundLevel = 0;

                btnSound.GetController("button").selectedIndex = SBoxModel.Instance.soundLevel;
                //GlobalSoundHelper.Instance.PlaySoundEff(GameMaker.SoundKey.NormalClick);
                context.StopPropagation(); // 停止事件冒泡(不起作用)
            });
            //btnSound.sound = null;
            btnSound.GetController("button").selectedIndex = SBoxModel.Instance.soundLevel;



            OnPropertyChangeBetList();
            OnPropertyChangeTotalBet();
            OnPropertyChangeBtnSpinState();
            OnPropertyIsConnectMoneyBox();
        }


        protected virtual void OnClickHelp()
        {
            if (btnHelp.selected)
            {
                gOwnerPanel.GetChild("mask").asGraph.visible = true;
                spinBtnCtrl.goOwnerSpin.GetController("button").selectedPage = "untouchable";
                spinBtnCtrl.goOwnerSpin.touchable = false;
            }
            else
            {

                gPayTableWin.visible = false;
                gOwnerPanel.GetChild("mask").asGraph.visible = false;

                spinBtnCtrl.goOwnerSpin.GetController("button").selectedPage = "stop";
                spinBtnCtrl.goOwnerSpin.touchable = true;
            }
        }

        protected virtual void IntroduceInit()
        {
            contentIndex = 0;
            payTableContent.RemoveChildren();
            payTableContent.AddChild(MainModel.Instance.contentMD.goPayTableLst[contentIndex]);
            btnPrev.touchable = false;
            btnPrev.GetChild("untouch").visible = true;
            btnNext.touchable = true;
            btnNext.GetChild("untouch").visible = false;
            gPayTableWin.GetChild("btnController").asCom.GetController("c1").selectedIndex = contentIndex;
        }

        protected virtual void OnClickIntroduceL()
        {
            IntroduceChange(false);
            if (contentIndex == 0)
            {
                btnPrev.touchable = false;
                btnPrev.GetChild("untouch").visible = true;
            }
            else
            {
                btnNext.GetChild("untouch").visible = false;
                btnNext.touchable = true;
            }
        }

        protected virtual void OnClickIntroduceR()
        {
            IntroduceChange(true);
            if (contentIndex == 6)
            {
                btnNext.touchable = false;
                btnNext.GetChild("untouch").visible = true;

            }
            else
            {

                btnPrev.touchable = true;
                btnPrev.GetChild("untouch").visible = false;
            }
        }

        protected virtual void IntroduceChange(bool jia)
        {
            if (jia)
            {
                contentIndex += 1;
            }
            else
            {
                contentIndex -= 1;
            }

            payTableContent.RemoveChildren();
            payTableContent.AddChild(MainModel.Instance.contentMD.goPayTableLst[contentIndex]);
            gPayTableWin.GetChild("btnController").asCom.GetController("c1").selectedIndex = contentIndex;
        }
        protected virtual void OnPropertyChange(EventData res = null)
        {
            //ContentModel
            string name = res.name;
            switch (name)
            {
                case "ContentModel/totalBet":
                    OnPropertyChangeTotalBet(res);
                    break;
                case "SBoxModel/betList":
                    OnPropertyChangeBetList(res);
                    break;
                case "ContentModel/btnSpinState":
                    OnPropertyChangeBtnSpinState(res);
                    break;
                case "ContentModel/gameState":
                    OnPropertyGameState(res);
                    break;
                case "SBoxModel/isConnectMoneyBox":
                    OnPropertyIsConnectMoneyBox(res);
                    break;
            }
        }


        //  panel ctl  --> game ctl  --> model -->  panel ctl
        protected virtual void OnPropertyChangeTotalBet(EventData res = null)
        {
            //long totalBet = (long)res?.value;

            //if (totalBet == null)
            //    totalBet = MainModel.Instance.contentMD.totalBet;
        }

        protected virtual void OnPropertyChangeBetList(EventData res = null)
        {
            List<long> betList = (List<long>)res?.value;

            if (betList == null)
                betList = SBoxModel.Instance.betList;
        }
        protected virtual void OnPropertyChangeBtnSpinState(EventData res = null)
        {
            string changeSpinState = (string)res?.value;

            if (changeSpinState == null)
                changeSpinState = "Stop";

            if (gOwnerPanel == null) return;


            switch (changeSpinState)
            {
                case SpinButtonState.Stop:
                    {
                        spinBtnCtrl.State = "Stop";
                        ChangButtonNo(false);
                    }
                    break;
                case SpinButtonState.Spin:
                    {
                        spinBtnCtrl.State = "Spin";
                        //if (gOwnerPanel != null)
                        //{
                        //    gOwnerPanel.GetChild("goodLuck").asLoader.visible = false;
                        //    gOwnerPanel.GetChild("win").asCom.visible = true;
                        //}
                        ChangButtonNo(true);
                    }
                    break;
                case SpinButtonState.Auto:
                    {
                        spinBtnCtrl.State = "Auto";
                        //if (gOwnerPanel != null)
                        //{
                        //    gOwnerPanel.GetChild("goodLuck").asLoader.visible = false;
                        //    gOwnerPanel.GetChild("win").asCom.visible = true;
                        //}
                        ChangButtonNo(true);
                    }
                    break;
            }


        }
        protected virtual void OnPropertyGameState(EventData res = null)
        {
            string gameState = (string)res?.value;

            if (gameState == GameState.Spin || gameState == GameState.FreeSpin)
            {
                txtWin.text = 0.ToString();
            }
        }
        protected virtual void OnPropertyIsConnectMoneyBox(EventData res = null)
        {

        }


        protected virtual void OnPanelEventAnchorPanelChange(EventData res = null)
        {
            if (res.name == PanelEvent.AnchorPanelChange)
            {
                Init();
            }
        }


        protected virtual void OnTotalWinCredit(EventData receivedEvent)
        {
            if (receivedEvent.name == SlotMachineEvent.TotalWinCredit)
            {
                long totalWinCredit = (long)receivedEvent.value;
                //gOwnerPanel.GetChild("win").asCom.GetChild("winSound").asTextField.text = totalWinCredit.ToString();
                txtWin.text = totalWinCredit.ToString();
                // uiWin.SetToCredit(totalWinCredit);
            }
            else if (receivedEvent.name == SlotMachineEvent.SingleWinBonus)
            {
                long totalWinCredit = (long)receivedEvent.value;
                NumberAnimation.Instance.AnimateNumber(txtWin,
                                                       long.Parse(txtWin.text),
                                                       totalWinCredit + long.Parse(txtWin.text),
                                                       0.4f);

            }

        }



        protected virtual void OnUpdateNaviCredit(EventData receivedEvent = null)
        {
            if (gOwnerPanel == null) return;

            bool isAmin = false;
            long fromCredit = 0;
            long toCredit = 0;
            if (receivedEvent == null || receivedEvent.value == null)
            {
                isAmin = false;
                toCredit = MainBlackboardController.Instance.myTempCredit;
            }
            else
            {
                UpdateNaviCredit data = (UpdateNaviCredit)receivedEvent.value;

                isAmin = data.isAnim;
                fromCredit = data.fromCredit;
                toCredit = data.toCredit;
            }


            if (isAmin)
            {
                NumberAnimation.Instance.AnimateNumber(gOwnerPanel.GetChild("credit").asTextField, fromCredit, toCredit);
            }
            else
            {
                NumberAnimation.Instance.PauseTextFieldAnimation(gOwnerPanel.GetChild("credit").asTextField);
                if (gOwnerPanel.GetChild("credit").asTextField != null)
                {
                    gOwnerPanel.GetChild("credit").asTextField.text = toCredit.ToString();
                }
            }
        }



        // Spin鎸夐挳
        //public void OnLongClickSpinButton(string customDataOrState) => OnClickSpinButton(true);
        //public void OnShortClickSpinButton(string customDataOrState) => OnClickSpinButton(false);


        int i = 0;
        public void OnClickSpinButton(bool isLong)
        {


            EventCenter.Instance.EventTrigger<EventData>(PanelEvent.ON_PANEL_INPUT_EVENT,
               new EventData<bool>(PanelEvent.SpinButtonClick, isLong));

        }

        #region 置灰
        public virtual void ChangButtonNo(bool can)
        {
            if (can)
            {
                //gOwnerPanel.GetChild("ButtonPRIZE").asButton.GetChild("n1").visible = true;
                //gOwnerPanel.GetChild("ButtonPRIZE").asButton.touchable = false;
                btnHelp.GetChild("untouch").visible = true;
                btnHelp.touchable = false;
                btnBetUp.GetChild("untouch").visible = true;
                btnBetUp.touchable = false;
                btnBetDown.GetChild("untouch").visible = true;
                btnBetDown.touchable = false;

                // ChangeBetButtonInteractable(MainModel.Instance.contentMD.betIndex, SBoxModel.Instance.betList.Count);
            }
            else
            {
                //gOwnerPanel.GetChild("ButtonPRIZE").asButton.GetChild("n1").visible = false;
                //gOwnerPanel.GetChild("ButtonPRIZE").asButton.touchable = true;
                btnHelp.GetChild("untouch").visible = false;
                btnHelp.touchable = true;
                btnBetUp.GetChild("untouch").visible = false;
                btnBetUp.touchable = true;
                btnBetDown.GetChild("untouch").visible = false;
                btnBetDown.touchable = true;

                if (MainModel.Instance.contentMD.betIndex == 0)
                {
                    btnBetDown.GetChild("untouch").visible = true;
                    btnBetDown.touchable = false;
                }

                if (MainModel.Instance.contentMD.betIndex == 7)
                {
                    btnBetUp.GetChild("untouch").visible = true;
                    btnBetUp.touchable = false;
                }
            }

        }
        #endregion


        protected virtual void OnClickButtonBetUp()
        {
            GlobalSoundHelper.Instance.PlaySoundEff(SoundKey.BetUp);

            //soundHelper.PlaySoundEff(GameMaker.SoundKey.BetUp);
            List<long> betList = SBoxModel.Instance.betList;
            int betIndex = MainModel.Instance.contentMD.betIndex;
            if (++betIndex >= betList.Count)
            {

                betIndex = betList.Count - 1;
            }
            MainModel.Instance.contentMD.totalBet = betList[betIndex];
            ChangeBetButtonInteractable(betIndex, betList.Count);
        }

        protected virtual void OnClickButtonBetDown()
        {
            GlobalSoundHelper.Instance.PlaySoundEff(SoundKey.BetDown);

            //soundHelper.PlaySoundEff(GameMaker.SoundKey.BetDown);
            List<long> betList = SBoxModel.Instance.betList;

            int betIndex = MainModel.Instance.contentMD.betIndex;
            if (--betIndex < 0)
            {
                betIndex = 0;
            }
            MainModel.Instance.contentMD.totalBet = betList[betIndex];

            ChangeBetButtonInteractable(betIndex, betList.Count);
        }
        protected GButton btnBetDown, btnBetUp;

        protected int curBetIndex = 0;
        protected int curBetListCount = 1;
        protected virtual void ChangeBetButtonInteractable(int? betIndex01 = null, int? betListCount01 = null)
        {

            if (betIndex01 != null && betListCount01 != null)
            {
                curBetIndex = (int)betIndex01;
                curBetListCount = (int)betListCount01;
            }
            MainModel.Instance.contentMD.betIndex = curBetIndex;

            txtBet.text = MainModel.Instance.contentMD.totalBet.ToString();
            btnBetDown.touchable = curBetIndex > 0;
            btnBetDown.GetChild("untouch").visible = btnBetDown.touchable ? false : true;
            btnBetUp.touchable = curBetIndex < curBetListCount - 1;
            btnBetUp.GetChild("untouch").visible = btnBetUp.touchable ? false : true;
        }

        public virtual void OnLongClickHandler(MachineButtonKey machineButtonKey) { }

        public virtual void OnShortClickHandler(MachineButtonKey machineButtonKey) { }

        public virtual void OnDownClickHandler(MachineButtonKey machineButtonKey)
        {
            switch (machineButtonKey)
            {
                case MachineButtonKey.BtnSpin:
                    {

                    }
                    break;
            }
        }

        public virtual void OnUpClickHandler(MachineButtonKey machineButtonKey)
        {
            switch (machineButtonKey)
            {
                case MachineButtonKey.BtnSpin:
                    {

                    }
                    break;
            }
        }
    }
}
