using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using FairyGUI;

namespace ConsoleSlot01
{
    public class PageConsoleGameHistory : MachinePageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleGameHistory";
        public override PageType pageType => PageType.Overlay;

        protected override void OnInit()
        {
            base.OnInit();

            isInit = true;
            InitParam();
        }

        public override void OnOpen(PageName name, EventData data)
        {
            base.OnOpen(name, data);

            InitParam();
        }

        GButton btnClose, btnPrev, btnNext;

        Controller myController;

        GComponent goNavBottom;

        JackpotRecordView jpRecordView = new JackpotRecordView();
        JackpotRecordPresenter jpRecordCtrl = new JackpotRecordPresenter();

        IVTable tabJpRecordView => jpRecordView;

        public override void InitParam()
        {

            if (!isInit) return;

            if (!isOpen) return;

            goNavBottom = this.contentPane.GetChild("navBottom").asCom;

            btnClose = goNavBottom.GetChild("btnExit").asButton;
            btnClose.onClick.Clear();
            btnClose.onClick.Add(() =>
            {
                CloseSelf(null);
            });

            GList pages = this.contentPane.GetChild("tabs").asList;




            jpRecordView.InitParam(pages.GetChild("jackpot").asCom);
            jpRecordCtrl.InitParam(jpRecordView);

            jpRecordView.onChangeNavBottomTitle -= OnChangeNavButtomTitle;
            jpRecordView.onChangeNavBottomTitle += OnChangeNavButtomTitle;


            btnPrev = goNavBottom.GetChild("btnPrev").asButton;
            btnNext = goNavBottom.GetChild("btnNext").asButton;
            btnPrev.onChanged.Clear();
            btnNext.onChanged.Clear();
            btnPrev.onClick.Add(OnClickPrev);
            btnNext.onClick.Add(OnClickNext);


            myController = this.contentPane.GetController("tab");
            myController.onChanged.Clear();
            myController.onChanged.Add(OnControllerChanged);
            myController.selectedIndex = 0;

            SetNavBottomTitle();

        }


        void OnClickPrev()
        {
            if(myController.selectedIndex == 0)
            {
                
            }else if (myController.selectedIndex == 1)
            {
                tabJpRecordView.OnClickPrev();
            }

            SetNavBottomTitle();
        }

        void OnClickNext()
        {
            if (myController.selectedIndex == 0)
            {

            }
            else if (myController.selectedIndex == 1)
            {
                tabJpRecordView.OnClickNext();
            }

            SetNavBottomTitle();
        }

        void OnControllerChanged()
        {
            SetNavBottomTitle();
        }

        void SetNavBottomTitle()
        {
            string title = "";
            if (myController.selectedIndex == 0)
            {
                title = string.Format(I18nMgr.T("Game History, Page {0} of {1}"), tabJpRecordView.curPageIndex, tabJpRecordView.pageCount);
  
            }
            else if (myController.selectedIndex == 1)
            {
                title = string.Format(I18nMgr.T("Jackpot History, Page {0} of {1}"), tabJpRecordView.curPageIndex, tabJpRecordView.pageCount);
       
                //goNavBottom.GetChild("title").asTextField.text = tabJpRecordView.GetNavBottomTitle();
            }

            goNavBottom.GetChild("title").asTextField.text = title;
        }


        void OnChangeNavButtomTitle(int curPageIndex, int pageCount)
        {
            SetNavBottomTitle();
        }
    }
}
