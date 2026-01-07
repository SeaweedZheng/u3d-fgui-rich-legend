using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using SlotMaker;
using FairyGUI;

namespace ConsoleCoinPusher01
{
    public class InfoBaseController
    {

        public InfoBaseController()
        {
            Init();
        }

        public void Init()
        {
            EventCenter.Instance.AddEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
        }

        public  void Dispose()
        {
            EventCenter.Instance.RemoveEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
        }




        public GComponent goOwnerBase;
        GRichTextField  rtxtTitle, rtxtSeatID, rtxtGroupId, rtxtHardwareVer, rtxtAlgorithmVer, rtxtSoftwareVer;



        public string title
        {
            get
            {
                if (rtxtTitle == null)
                    rtxtTitle = goOwnerBase.GetChild("title").asRichTextField;
                return rtxtTitle.text;
            }
            set
            {
                rtxtTitle.text = value; 
            }
        }
       


        public void InitParam(GComponent gBase , string title)
        {
            if (gBase == null) return;

            goOwnerBase = gBase;


            rtxtTitle = goOwnerBase.asCom.GetChild("title").asRichTextField;


            rtxtSeatID = goOwnerBase.asCom.GetChild("seatId").asCom.GetChild("value").asRichTextField;
            rtxtGroupId = goOwnerBase.asCom.GetChild("groupId").asCom.GetChild("value").asRichTextField;
            rtxtHardwareVer = goOwnerBase.asCom.GetChild("hardwareVer").asCom.GetChild("value").asRichTextField;
            rtxtAlgorithmVer = goOwnerBase.asCom.GetChild("algorithmVer").asCom.GetChild("value").asRichTextField;
            rtxtSoftwareVer = goOwnerBase.asCom.GetChild("softwareVer").asCom.GetChild("value").asRichTextField;


            rtxtTitle.text = title;


            rtxtSeatID.text = SBoxModel.Instance.seatId.ToString();
            rtxtGroupId.text = SBoxModel.Instance.groupId.ToString();
            rtxtHardwareVer.text = SBoxModel.Instance.HardwareVer;

            rtxtAlgorithmVer.text = SBoxModel.Instance.AlgorithmVer;
            rtxtSoftwareVer.text = GlobalData.hotfixVersion;
        }

        void OnPropertyChange(EventData res) {

            string name = res.name;
            switch (name)
            {
                case "SBoxModel/groupId":
                    rtxtGroupId.text =  SBoxModel.Instance.groupId.ToString();
                    break;
                case "SBoxModel/seatId":
                    rtxtSeatID.text = SBoxModel.Instance.seatId.ToString();
                    break;
            }
        }



    }
}
