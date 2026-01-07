using FairyGUI;
using GameMaker;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lobby01
{
    public class PageLobbyMain : MachinePageBase
    {
        public const string pkgName = "Lobby01";
        public const string resName = "PageLobbyMain";


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

            // 异步加载资源

            /*
            ResourceManager02.Instance.LoadAsset<GameObject>(
            "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/Prefabs/Game Controller/Push Game Main Controller.prefab",
            (GameObject clone) =>
            {
                callback();
            });
            */
            callback();
        }

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


        // public override void OnTop() { DebugUtils.Log($"i am top {this.name}"); }

        GButton btnClose;

        GList glstGames;

        public override void InitParam()
        {

            if (!isInit) return;

            if (!isOpen) return;

            glstGames = this.contentPane.GetChild("games").asList;


            List<int> ids = LobbyGamesUtils.GetVisiableGameIds();

            // 初始化渲染逻辑（绑定数据到item）
            glstGames.itemRenderer = (int index, GObject obj) => {
      

                GButton btnItem = obj.asButton;

                int gameId = ids[index];

                string imgUrl = LobbyGamesManager.Instance.GetSeverValue<string>(gameId,"lobby_icon_big");

                string pth = Application.isEditor ?
                PathHelper.GetAssetBackupSAPTH(imgUrl) :
                PathHelper.GetAssetBackupLOCPTH(imgUrl);

                DebugUtils.Log($"imgUrl:{imgUrl}  -- pth:{pth}");
                FileLoaderManager.Instance.LoadImageAsTexture(pth, (Texture2D texture) =>
                {
                    NTexture nTexture = new NTexture(texture);
                    GLoader icon = btnItem.GetChild("icon").asLoader;
                    icon.texture = nTexture;
                    icon.fill = FillType.ScaleFree;      // 等比缩放，可能留白

                });

                GButton gButton = btnItem.GetChild("like").asButton;
                gButton.selected = LobbyGamesManager.Instance.GetLocalValue<bool>(gameId, "like");
                gButton.onChanged.Clear();
                gButton.onChanged.Add(() =>
                {
                    LobbyGamesManager.Instance.SetLocalValue<bool>(gameId,"", gButton.selected);
                });

                btnItem.onClick.Set(() =>
                {
                    MaskPopupHandler.Instance.OpenPopup();

                    string enterPageName = LobbyGamesManager.Instance.GetSeverValue<string>(gameId, "enter_page");
                    PageName pn = (PageName)Enum.Parse(typeof(PageName), enterPageName);
                    PageManager.Instance.OpenPage(pn, 
                    onFinishCalllback :(page) =>
                    {
                        MaskPopupHandler.Instance.ClosePopup();
                    });
                });
            };
            glstGames.numItems = ids.Count;// 更新列表项数量（关键：触发重新渲染）



            /*
            // btnClose =  this.contentPane.GetChild("btnExit").asButton;
            btnClose = this.contentPane.GetChild("navBottom").asCom.GetChild("btnExit").asButton;
            btnClose.onClick.Clear();
            btnClose.onClick.Add(() =>
            {
                //DebugUtils.Log("i am here 123");
                CloseSelf(null);
                //  CloseSelf(new EventData("Exit"));
            });
            */

            /* 
            if (inParams != null)
            {   
                Dictionary<string, object> argDic = null;
                argDic = (Dictionary<string, object>)inParams.value;
                title = (string)argDic["title"];
                isPlaintext = (bool)argDic["isPlaintext"];
                if (argDic.ContainsKey("content"))
                {
                    input = (string)argDic["content"];
                }
            }
           */
        }
    }

}