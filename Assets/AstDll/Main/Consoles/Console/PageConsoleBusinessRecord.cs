using FairyGUI;
using GameMaker;
using SBoxApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleSlot01
{
    public class PageConsoleBusinessRecord : PageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleBusinessRecord";
        public override PageType pageType => PageType.Overlay;
        protected override void OnInit()
        {
            
            base.OnInit();
        }


        public override void OnOpen(PageName name, EventData data)
        {
            base.OnOpen(name, data);

            EventCenter.Instance.AddEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);


            InitParam();
        }

        public override void OnClose(EventData data = null)
        {

            EventCenter.Instance.RemoveEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);



            base.OnClose(data);
        }

        public override void OnTop()
        {
            DebugUtils.Log($"i am top  {this.name}");
        }

        GButton btnClose, btnClose2, btnPrev, btnNext;

        GList lstPages;

        GComponent cmpTabBusiness, cmpTabCoin;

        Controller myController;

        GRichTextField rtxtTotalBet, rtxtTotalWin, rtxtTotalProfitBet, rtxtRemainPoints,

            rtxtTotalCoinIn, rtxtTotalCoinOut, rtxtTotalProfitCoinInOut,

            rtxtTotalScoreUp, rtxtTotalScoreDown, rtxtTotalProfitlScoreUpDown,

            rtxtbillingInformation,

            rtxtDayInOutTotalCoinIn, rtxtDayInOutTotalCoinOut, rtxtDayInOutTotalProfitlCoinInOut,
            rtxtDayInOutTotalScoreUp, rtxtDayInOutTotalScoreDown, rtxtDayInOutTotalProfitlScoreUpDown,

            rtxtPageButtom1, rtxtPageButtom2;



        InOutRecordView inOutRecordView = new InOutRecordView();
        InOutRecordPresenter inOutRecordPter = new InOutRecordPresenter();

        DayBusinessRecordView dayBusinessRecordView = new DayBusinessRecordView();
        DayBusinessRecordPresenter dayBusinessRecordPter = new DayBusinessRecordPresenter();


        int tabIndex = 0;

        public override void InitParam()
        {
            rtxtPageButtom1 = this.contentPane.GetChild("navBottom").asCom.GetChild("title").asRichTextField;
            rtxtPageButtom2 = this.contentPane.GetChild("navBottom2").asCom.GetChild("title").asRichTextField;


            btnClose = this.contentPane.GetChild("navBottom").asCom.GetChild("btnExit").asButton;
            btnClose.onClick.Clear();
            btnClose.onClick.Add(() =>
            {
                CloseSelf(null);
            });

            btnClose2 = this.contentPane.GetChild("navBottom2").asCom.GetChild("btnExit").asButton;
            btnClose2.onClick.Clear();
            btnClose2.onClick.Add(() =>
            {
                CloseSelf(null);
            });


            lstPages = this.contentPane.GetChild("tabs").asList;

            cmpTabBusiness = lstPages.GetChildAt(0).asCom;
            cmpTabCoin = lstPages.GetChildAt(1).asCom;


            myController = this.contentPane.GetController("tab");
            myController.onChanged.Clear();
            myController.onChanged.Add(OnControllerChanged);
            myController.selectedIndex = 0;
            tabIndex = 0;
            SetNavBottomTitle();



            InitTotalBusiness();

            InitDayBusiness();

            InitInOutRecord();


            btnPrev = this.contentPane.GetChild("navBottom2").asCom.GetChild("btnPrev").asButton;
            btnNext = this.contentPane.GetChild("navBottom2").asCom.GetChild("btnNext").asButton;
            btnPrev.onChanged.Clear();
            btnNext.onChanged.Clear();
            btnPrev.onClick.Add(() =>
            {
                //inOutRecordCtrl.OnPrevCoinInOutRecord();
                inOutRecordPter.OnClickPrevPage();
                SetNavBottomTitle();
            });
            btnNext.onClick.Add(() =>
            {
                //inOutRecordCtrl.OnNextCoinInOutRecord();
                inOutRecordPter.OnClickNextPage();
                SetNavBottomTitle();
            });



            MachineDataManager02.Instance.RequestGetPlayerInfo((res) =>
            {

                SBoxAccount data = (SBoxAccount)res;
                int pid = SBoxModel.Instance.pid;
                List<SBoxPlayerAccount> playerAccountList = data.PlayerAccountList;
                for (int i = 0; i < playerAccountList.Count; i++)
                {
                    if (playerAccountList[i].PlayerId == pid)
                    {
                        SBoxModel.Instance.SboxPlayerAccount = playerAccountList[i];
                        break;
                    }
                }

            }, (BagelCodeError err) =>
            {

                DebugUtils.Log(err.msg);
            });

        }


        void InitTotalBusiness()
        {
            rtxtTotalBet = cmpTabBusiness.GetChild("totalBet").asCom.GetChild("value").asRichTextField;
            rtxtTotalWin = cmpTabBusiness.GetChild("totalWin").asCom.GetChild("value").asRichTextField;
            rtxtTotalProfitBet = cmpTabBusiness.GetChild("totalProfitBet").asCom.GetChild("value").asRichTextField;
            rtxtRemainPoints = cmpTabBusiness.GetChild("remainingPoints").asCom.GetChild("value").asRichTextField;

            rtxtTotalCoinIn = cmpTabBusiness.GetChild("totalCoinIn").asCom.GetChild("value").asRichTextField;
            rtxtTotalCoinOut = cmpTabBusiness.GetChild("totalCoinOut").asCom.GetChild("value").asRichTextField;
            rtxtTotalProfitCoinInOut = cmpTabBusiness.GetChild("totalProfitCoinInOut").asCom.GetChild("value").asRichTextField;

            rtxtTotalScoreUp = cmpTabBusiness.GetChild("totalScoreUp").asCom.GetChild("value").asRichTextField;
            rtxtTotalScoreDown = cmpTabBusiness.GetChild("totalScoreDown").asCom.GetChild("value").asRichTextField;
            rtxtTotalProfitlScoreUpDown = cmpTabBusiness.GetChild("totalProfitScoreUpDown").asCom.GetChild("value").asRichTextField;

            rtxtbillingInformation = cmpTabBusiness.GetChild("billingInformation").asCom.GetChild("value").asRichTextField;

            OnPropertyChangeSBoxPlayerAccount();
        }
        void InitDayBusiness()
        {
            dayBusinessRecordView.InitParam(cmpTabBusiness);
            dayBusinessRecordPter.InitParam(dayBusinessRecordView);
        }

        void InitInOutRecord()
        {
            inOutRecordView.InitParam(cmpTabCoin);
            inOutRecordPter.InitParam(inOutRecordView);

            inOutRecordView.onChangeNavBottomTitle -= OnInOutPageIndexChange;
            inOutRecordView.onChangeNavBottomTitle += OnInOutPageIndexChange;
        }
        /**/
        void OnInOutPageIndexChange(int curPageIndex, int totalPageCount) => SetNavBottomTitle();


        void OnPropertyChange(EventData res = null)
        {
            string name = res.name;
            switch (name)
            {
                case "SBoxModel/SboxPlayerAccount":
                    OnPropertyChangeSBoxPlayerAccount(res);
                    break;
            }
        }

        void OnPropertyChangeSBoxPlayerAccount(EventData res = null)
        {
            SetDataTotalWinInfo();
            SetDataTotalCoinInOutScoreUpDown();
            SetDataBillInfo();
        }

        void SetDataTotalWinInfo()
        {
            rtxtTotalBet.text = SBoxModel.Instance.HistoryTotalBet.ToString();
            rtxtTotalWin.text = SBoxModel.Instance.HistoryTotalWin.ToString();
            rtxtTotalProfitBet.text = SBoxModel.Instance.HistoryTotalProfitBet.ToString();
            rtxtRemainPoints.text = $"{SBoxModel.Instance.myCredit}";
        }

        /// <summary>
        /// 设置账单信息
        /// </summary>
        void SetDataBillInfo()
        {
            string res = $"{SBoxModel.Instance.BillInfoTime}\n{SBoxModel.Instance.BillInfoLineMachineNumber}\n{SBoxModel.Instance.BillInfoHardwareAlgorithmVer}";
            rtxtbillingInformation.text = res;
        }


        /// <summary>
        /// 历史总上下分、总投退币
        /// </summary>
        void SetDataTotalCoinInOutScoreUpDown()
        {
            rtxtTotalCoinIn.text = $"{SBoxModel.Instance.HistoryTotalCoinInCredit}";
            rtxtTotalCoinOut.text = $"{SBoxModel.Instance.HistoryTotalCoinOutCredit}";
            rtxtTotalProfitCoinInOut.text = $"{SBoxModel.Instance.HistoryTotalProfitCoinIn}";

            rtxtTotalScoreUp.text = $"{SBoxModel.Instance.HistoryTotalScoreUpCredit}";
            rtxtTotalScoreDown.text = $"{SBoxModel.Instance.HistoryTotalScoreDownCredit}";
            rtxtTotalProfitlScoreUpDown.text = $"{SBoxModel.Instance.HistoryTotalProfitScoreUp}";
        }

        // 控制器变化回调
        private void OnControllerChanged(EventContext context)
        {
            Controller controller = (Controller)context.sender;
            DebugUtils.Log($"控制器已切换，当前页签索引: {controller.selectedIndex}, 名称: {controller.selectedPage}");
            tabIndex = controller.selectedIndex;
            SetNavBottomTitle();
        }


        private void SetNavBottomTitle()
        {
            if (tabIndex == 0)
            {
                rtxtPageButtom1.text = I18nMgr.T("Business Record"); 
            }
            else if (tabIndex == 1)
            {
                rtxtPageButtom2.text = string.Format(I18nMgr.T("Coin In-Out History, Page {0} of {1}"),
                    inOutRecordView.curPageIndex + 1, inOutRecordView.pageCount);
            }
        }
    }

}