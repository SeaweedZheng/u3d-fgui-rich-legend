using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using FairyGUI;
using System;

namespace ConsoleSlot01
{
    public class PopupConsoleNotice : MachinePageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PopupConsoleNotice";
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

        GButton btnConfitm, btnNext;

        GRichTextField txtTitle;

        List<GRichTextField> lstContent = new List<GRichTextField>();

        GList dots;


        string title = "标题";

        List<string> content = new List<string> { };

        int pageIndex = 0;
        int pageCount = 1;
        int contentCount => lstContent.Count;

        public override void InitParam() {

            if (!isInit) return;

            if (!isOpen) return;

          
            content.Clear();
            //测试数据
            for (int i=0; i<32; i++)
            {
                content.Add($"测试数据:{i}");
            }



            btnConfitm = this.contentPane.GetChild("btnConfirm").asButton;
            btnConfitm.onClick.Clear();
            btnConfitm.onClick.Add(CloseSelf);

            btnNext = this.contentPane.GetChild("btnNext").asButton;
            btnNext.onClick.Clear();
            btnNext.onClick.Add(OnClickNext);

            txtTitle = this.contentPane.GetChild("title").asRichTextField;
            txtTitle.text = "";


            lstContent.Clear();

            for (int i=0; i<15;i++)
            {
                lstContent.Add(this.contentPane.GetChild("row"+i).asRichTextField);
            }

            foreach (GRichTextField item in lstContent)
            {
                item.text = "";
            }


            dots = this.contentPane.GetChild("dots").asList;
            dots.numItems = 0;

            if (inParams != null)
            {
                Dictionary<string, object> argDic = null;
                argDic = (Dictionary<string, object>)inParams.value;

                title = (string)argDic["title"];

                if (argDic.ContainsKey("content"))
                {
                    content = (List<string>)argDic["content"];
                }
            }

            txtTitle.text = title;

            pageCount = (int)Math.Ceiling(content.Count / 15f);
            pageIndex = 0;
            dots.numItems = pageCount;

            SetDotsHighlight();
            SetContent();

        }

        void SetContent()
        {

            foreach (GRichTextField item in lstContent)
            {
                item.visible = false;
            }

            int count = content.Count - contentCount * pageIndex;
            count = Math.Min(count, lstContent.Count);

            for (int i = 0; i < count; i++)
            {
                //DebugUtils.Log($"id = {i + pageIndex * contentCount}");
                lstContent[i].visible = true;
                lstContent[i].text = content[i + pageIndex * contentCount];
            }

        }

        void SetDotsHighlight()
        {
            GObject[] items = dots.GetChildren();
            foreach (GObject item in items)
            {
                item.asImage.color = Color.white;
            }

            Color grayColor;
            if (ColorUtility.TryParseHtmlString("#00F3EF", out grayColor))
            {
                dots.GetChildAt(pageIndex).asImage.color = grayColor;
            }
        }


        void CloseSelf()=> CloseSelf(null);
        void OnClickNext()
        {
            if (++pageIndex >= pageCount)
                pageIndex = 0;

            SetDotsHighlight();
            SetContent();

        }
    }



}