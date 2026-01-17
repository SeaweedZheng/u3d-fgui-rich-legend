using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using System;


namespace CoinPusherRichLegend2001001
{
    public class PageGameMain : MachinePageBase //: PageBase
    {
        public const string pkgName = "RichLegend2001001";
        public const string resName = "PageGameMain";


        protected override void OnInit()
        {

            base.OnInit();

            int count = 2;

            Action callback = () =>
            {
                if (--count == 0)
                {
                    isInit = true;
                    InitParam();
                }
            };


            ResourceManager02.Instance.LoadAsset<TextAsset>(
            "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Datas/slot_machine_config.json",
            (TextAsset data) =>
            {
                configStr = data.text;

                callback();
            });


            // 异步加载资源

            ResourceManager02.Instance.LoadAsset<GameObject>(
            "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/Prefabs/Game Controller/Game Main Controller.prefab",
            (GameObject clone) =>
            {

                goGameCtrl = GameObject.Instantiate(clone);

                //Debug.LogError("创建 Push Game Main Controller");

                goGameCtrl.name = "Game Main Controller";
                goGameCtrl.transform.SetParent(null);

                slotMachineCtrl = goGameCtrl.transform.Find("Slot Machine").GetComponent<SlotMachinePresenter>();

                ContentModel model = goGameCtrl.transform.Find("Blackboard/Content Model").GetComponent<ContentModel>();
                if(model == null)
                    DebugUtils.LogError("ContentModel is null");
                //mono = goGameCtrl.transform.GetComponent<MonoHelper>();
                //DebugUtils.Log(mono);
                //DebugUtils.LogWarning("i am Game Controller");


                // DebugUtils.LogError("A ContentModel = " + goGameCtrl.transform.Find("Blackboard/Content Model").GetComponent<ContentModel>().transform.name);

                //fguiPoolHelper = goGameCtrl.transform.Find("Pool").GetComponent<FguiPoolHelper>();

                //gObjectPoolHelper = goGameCtrl.transform.Find("GObject Pool").GetComponent<FguiGObjectPoolHelper>();

                callback();
            
            });

        }

        GameObject goGameCtrl;



        public override void OnOpen(PageName name, EventData data)
        {
            base.OnOpen(name, data);

            // 添加事件监听

            InitParam();
        }


        public override void OnClose(EventData data = null)
        {

            // 删除事件监听

            base.OnClose(data);
        }



        string configStr = null;

        SlotMachineConfig config;
        SlotMachineViewBase SlotMachineView = new SlotMachineViewBase();
        SlotMachinePresenter slotMachineCtrl;


        List<GComponent> lstPayTable;

        public override void InitParam()
        {

            if (!isInit) return;

            if (!isOpen) return;

            config = new SlotMachineConfig(configStr);

            GComponent goSlotMachine = this.contentPane.GetChild("slotMachine").asCom;
            SlotMachineView.InitParam(goSlotMachine,null,config);
            slotMachineCtrl.InitParam(SlotMachineView, config);



            MainModel.Instance.contentMD = ContentModel.Instance;

            lstPayTable = new List<GComponent>();
            ContentModel.Instance.goPayTableLst = lstPayTable.ToArray();


            GComponent gOwnerPanel = this.contentPane.GetChild("anchorPanel").asCom;
            ContentModel.Instance.goAnchorPanel = gOwnerPanel;
            EventCenter.Instance.EventTrigger<EventData>(PanelEvent.ON_PANEL_EVENT,
                new EventData<GComponent>(PanelEvent.AnchorPanelChange, gOwnerPanel));
        }






    }
}