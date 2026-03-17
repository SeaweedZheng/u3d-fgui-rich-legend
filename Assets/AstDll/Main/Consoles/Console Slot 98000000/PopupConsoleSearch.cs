using FairyGUI;
using GameMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;



namespace ConsoleSlot98000000
{

    public class InParamsPopupConsoleSearch: InParamsBase
    {
        /// <summary> 弹窗抬头 </summary>
        public string title = "";

        /// <summary> 选项集合 </summary>
        public List<InParamItemSelectOption> options = new List<InParamItemSelectOption>();
    }

    public class OutParamsPopupConsoleSearch: OutParamsBase
    {
        /// <summary>
        /// 选择结果
        /// </summary>
        /// <remarks>
        /// * selectType - selectContentKey
        /// </remarks>
        public Dictionary<string,string> selectResult = new Dictionary<string,string>();
    }



    public class PopupConsoleSearch : PageBase
    {
        public const string pkgName = "ConsoleSlot98000000";
        public const string resName = "PopupConsoleSearch";
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

        public override void OnOpen(PageName name, InParamsBase data)
        {
            base.OnOpen(name, data);

            // 添加事件监听

            InitParam();
        }


        public override void OnClose(OutParamsBase data = null)
        {

            // 删除事件监听

            base.OnClose(data);
        }


        // public override void OnTop() { DebugUtils.Log($"i am top {this.name}"); }

        GButton btnClose, btnConfirm;



        GRichTextField txtTitle;
        GList glstOptions;


        Dictionary<string, string> selectResult = new Dictionary<string, string>();
        public override void InitParam()
        {

            if (!isInit) return;

            OnPreLoaded(); // 其他卡顿的资源实例化

            preLoadedCallback?.Invoke();
            preLoadedCallback?.RemoveAllListeners();

            if (!isOpen) return;



            btnClose =  this.contentPane.GetChild("btnExit").asButton;
            // btnClose = this.contentPane.GetChild("navBottom").asCom.GetChild("btnExit").asButton;
            btnClose.onClick.Clear();
            btnClose.onClick.Add(() =>
            {
                CloseSelf(null);
            });


            btnConfirm = this.contentPane.GetChild("btnConfirm").asButton;

            btnConfirm.onClick.Clear();
            btnConfirm.onClick.Add(() =>
            {
                CloseSelf(new OutParamsPopupConsoleSearch()
                {
                    selectResult = selectResult
                });
            });

            txtTitle = this.contentPane.GetChild("title").asRichTextField;

            glstOptions = this.contentPane.GetChild("options").asList;




            if (inParams != null)
            {

                var inp = inParams as InParamsPopupConsoleSearch;


                txtTitle.text = inp.title;

                glstOptions.itemRenderer = (int index, GObject obj) => {

                    InParamItemSelectOption data = inp.options[index];

                    GComponent goItem = obj as GComponent;
                    goItem.GetChild("key").asRichTextField.text = data.selectName;

                    GComboBox gcb = goItem.GetChild("value").asComboBox;

                    gcb.items = data.selectContent.Values.ToArray();
                    gcb.values = data.selectContent.Keys.ToArray();
                    gcb.onChanged.Clear();
                    gcb.onChanged.Add((EventContext context) =>
                    {
                        GComboBox sender = context.sender as GComboBox;
                        OnSelect(data.selectType , sender.value);
                    });

                    gcb.selectedIndex = data.selectContent.Keys.ToList().IndexOf(data.selectKey);
                };
                glstOptions.numItems = inp.options.Count;

            }
         
        }


         void OnSelect(string selectType , string selectKey)
        {
            if (!selectResult.ContainsKey(selectType))
            {
                selectResult.Add(selectType, "");
            }
            selectResult[selectType] = selectKey;
        }

        public void OnPreLoaded()
        {

        }
    }

}