using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using FairyGUI;
using System.Linq;

namespace ConsoleSlot01
{


    public class PageConsoleAdmin : MachinePageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleAdmin";
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

        public override void OnClose(EventData data = null)
        {
            tabPermissionsCtrl.Disable();

            base.OnClose(data);
        }

        GButton btnClose, btnClose2;
        GRichTextField navBottomTitle2;

        TabAdminDebugController tabDebugCtrl = new TabAdminDebugController();
        TabAdminDisplayPermissions tabPermissionsCtrl = new TabAdminDisplayPermissions();
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


            btnClose2 = this.contentPane.GetChild("navBottom2").asCom.GetChild("btnExit").asButton;
            btnClose2.onClick.Clear();
            btnClose2.onClick.Add(() =>
            {
                CloseSelf(null);
            });

            navBottomTitle2 = this.contentPane.GetChild("navBottom2").asCom.GetChild("title").asRichTextField;

            GList tabs = this.contentPane.GetChild("tabs").asList;



            tabDebugCtrl.InitParam(tabs.GetChild("debug").asCom);



            tabPermissionsCtrl.onPageChange -= OnPageChange;
            tabPermissionsCtrl.onPageChange += OnPageChange;
            tabPermissionsCtrl.InitParam(tabs.GetChild("permissions").asCom);

        }

        void OnPageChange(int curPageIndex, int pageCount)
        {
            string str = string.Format(I18nMgr.T("Permissions, Page: {0}/{1}"), curPageIndex + 1, pageCount);
            navBottomTitle2.text = str;
        }
    }
}
