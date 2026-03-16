using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using FairyGUI;

namespace ConsoleSlot98000000
{
    public class PageConsoleGameHistory : MachinePageBase
    {
        public const string pkgName = "ConsoleSlot98000000";
        public const string resName = "PageConsoleGameHistory";
        public override PageType pageType => PageType.Overlay;

        protected override void OnInit()
        {
            base.OnInit();

            isInit = true;
            InitParam();
        }

        public override void OnOpen(PageName name, InParamsBase data)
        {
            base.OnOpen(name, data);

            InitParam();
        }

        GButton btnClose, btnPrev, btnNext;

        Controller myController;

        GComponent goNavBottom;

        JackpotRecordView jpRecordView = new JackpotRecordView();
        JackpotRecordPresenter jpRecordPresenter = new JackpotRecordPresenter();

        MultipleGameRecordView multipleGameRecordView = new MultipleGameRecordView();
        MultipleGameRecordPresenter multipleGameRecordPresenter = new MultipleGameRecordPresenter();


        IVTable tabGameRecordView => multipleGameRecordView;
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

            GList tabs = this.contentPane.GetChild("tabs").asList;


            multipleGameRecordView.InitParam(tabs.GetChild("game").asCom);
            multipleGameRecordPresenter.InitParam(multipleGameRecordView);

            multipleGameRecordView.onChangeNavBottomTitle -= OnChangeNavButtomTitle;
            multipleGameRecordView.onChangeNavBottomTitle += OnChangeNavButtomTitle;

            jpRecordView.InitParam(tabs.GetChild("jackpot").asCom);
            jpRecordPresenter.InitParam(jpRecordView);

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
                tabGameRecordView.OnClickPrev();
            }
            else if (myController.selectedIndex == 1)
            {
                tabJpRecordView.OnClickPrev();
            }

            SetNavBottomTitle();
        }

        void OnClickNext()
        {
            if (myController.selectedIndex == 0)
            {
                tabGameRecordView.OnClickNext();
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
                title = string.Format(I18nMgr.T("Game History, Page {0} of {1}"), tabGameRecordView.curPageIndex + 1, tabGameRecordView.pageCount);
  
            }
            else if (myController.selectedIndex == 1)
            {
                title = string.Format(I18nMgr.T("Jackpot History, Page {0} of {1}"), tabJpRecordView.curPageIndex + 1, tabJpRecordView.pageCount);
       
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
