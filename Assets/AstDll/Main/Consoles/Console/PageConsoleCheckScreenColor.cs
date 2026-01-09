using FairyGUI;
using GameMaker;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleSlot01
{

    public class PageConsoleCheckScreenColor : MachinePageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleCheckScreenColor";
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

            Timers.inst.Add(1.5f, 0, ChangeColor);

            InitParam();
        }


        public override void OnClose(EventData data = null)
        {
            Timers.inst.Remove(ChangeColor);

            base.OnClose(data);
        }

        GButton btnClose;

        GImage imgBG;

        GGraph graphBG;

        Color[] colors = new Color[]
        {
        Color.black,
        Color.red,
        Color.green,
        Color.blue,
        Color.white,

        };

        public override void InitParam()
        {
            if (!isInit) return;

            if (!isOpen) return;

            btnClose = this.contentPane.GetChild("btnExit").asButton;
            btnClose.onClick.Clear();
            btnClose.onClick.Add(() => { CloseSelf(null); });


            /*
            GObject[] chds = this.contentPane.GetChildren();
            foreach (GObject ch in chds)
            {
                DebugUtils.Log($"--  name: -{ch.name}-");
            }*/


            //imgBG = this.contentPane.GetChild("bg").asImage;
            //imgBG = goBG.asImage; //这里会报错，  (bg是图形，而不是图片)

            graphBG = this.contentPane.GetChild("bg").asGraph;
            graphBG.color = Color.black;

            index = 0;
        }

        int index = 0;
        void ChangeColor(object param)
        {
            if (graphBG == null) return;

            if (++index >= colors.Length)
                index = 0;

            graphBG.color = colors[index];
        }
    }

}