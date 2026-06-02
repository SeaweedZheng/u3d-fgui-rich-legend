using FairyGUI;
using GameMaker;
using SlotMaker;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby89000000
{
    public class PageLobbyMain : MachinePageBase
    {
        public const string pkgName = "Lobby89000000";
        public const string resName = "PageLobbyMain";

        public static void OnBeforeCreat(Action onFinishCallback)
        {
            // 添加模块
            ModuleDownloadManager.Instance.AddModeToRuning("Main");

            onFinishCallback?.Invoke();
        }
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

        public override void OnOpen(PageName name, InParamsBase data)
        {
            base.OnOpen(name, data);

            MainModel.Instance.gameID = -1;

            // 添加事件监听

            //EventCenter.Instance.AddEventListener<EventData>(MetaUIEvent.ON_CREDIT_EVENT.);



            creditCtr.Enable();

            InitParam();
        }


        public override void OnClose(OutParamsBase data = null)
        {

            // 删除事件监听

            creditCtr.Disable();

            base.OnClose(data);
        }


        public override void OnTop() {

            GameSoundHelper.Instance.PlayMusicSingle(SoundKey.LobbyBG1);
        }

        GButton btnClose;

        GList glstGames;

        GComponent comButtom, comSidebar;

        Controller ctrSound;

        UIMyCreditController creditCtr = new UIMyCreditController();
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

                string imgUrl = LobbyGamesManager.Instance.GetGameValueFromServer<string>(gameId,"lobby_icon_big");

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

                GButton btnLike = btnItem.GetChild("like").asButton;
                btnLike.selected = LobbyGamesManager.Instance.GetGameValueFromCache<bool>(gameId, "like");
                btnLike.onChanged.Clear();
                btnLike.onChanged.Add((EventContext context) =>
                {
                    LobbyGamesManager.Instance.SetValueForCache<bool>(gameId, "like", btnLike.selected);

                    // 2. 阻止事件向下穿透（核心代码）
                    //context.StopPropagation(); // 停止事件冒泡(不起作用)
                });
                btnLike.onClick.Set((EventContext context) =>
                {
                    //GameSoundHelper.Instance.PlaySoundEff(GameMaker.SoundKey.NormalClick);
                    // 2. 阻止事件向下穿透（核心代码）
                    context.StopPropagation(); // 停止事件冒泡(不起作用)
                });
                btnLike.sound = null;  // 关掉默认按钮声音


                btnItem.sound = null;  // 关掉默认按钮声音
                btnItem.onClick.Set(() =>
                {
                    PageUtils.EnterGame(gameId, () =>
                    {
                        GameSoundHelper.Instance.StopMusic(); //关掉背景音乐
                        GameSoundHelper.Instance.PlaySoundEff(SoundKey.EnterGame);
                    });
                    /*
                    GameUpdateChecker.Instance.CheckPlayabilityWhenEnterGame(gameId, (isPlayable) =>
                    {
                        if (isPlayable)
                        {
                            MaskPopupHandler.Instance.OpenPopup();
                            GameSoundHelper.Instance.StopMusic(); //关掉背景音乐
                            GameSoundHelper.Instance.PlaySoundEff(SoundKey.EnterGame);


                            ModuleDownloadManager.Instance.AddModeToRuning(gameId);

                            string enterPageName = LobbyGamesManager.Instance.GetGameValueFromSever<string>(gameId, "enter_page");
                            PageName pn = (PageName)Enum.Parse(typeof(PageName), enterPageName);
                            PageManager.Instance.OpenPage(pn,
                            onFinishCalllback: (page) =>
                            {
                                MainModel.Instance.gameID = gameId;
                                MaskPopupHandler.Instance.ClosePopup();
                            });
                        }
                    });
                    */

                });
            };
            glstGames.numItems = ids.Count;// 更新列表项数量（关键：触发重新渲染）


            comButtom = this.contentPane.GetChild("buttom").asCom;
            GComponent comMenu = comButtom.GetChild("btnMore").asCom.GetChild("menu").asCom;
            GButton btnSound = comMenu.GetChild("sound").asButton;
            ctrSound = btnSound.GetController("c1");
            ctrSound.selectedIndex = SBoxModel.Instance.soundLevel;
            btnSound.onClick.Set((EventContext context) =>
            {
                GameSoundHelper.Instance.PlaySoundEff(GameMaker.SoundKey.NormalClick);

                if (++SBoxModel.Instance.soundLevel > 3)
                    SBoxModel.Instance.soundLevel = 0;

                ctrSound.selectedIndex = SBoxModel.Instance.soundLevel;

                // 2. 阻止事件向下穿透（核心代码）
                context.StopPropagation(); // 停止事件冒泡
            });



            // 侧边栏折叠按钮
            comSidebar = this.contentPane.GetChild("sidebar").asCom;
            GButton btnFold = comSidebar.GetChild("btnFold").asButton;
            btnFold.onClick.Set((EventContext context) =>
            {
                GameSoundHelper.Instance.PlaySoundEff(GameMaker.SoundKey.NormalClick);
                // 2. 阻止事件向下穿透（核心代码）
                context.StopPropagation(); // 停止事件冒泡
            });
            btnFold.onChanged.Set(() =>
            {
                comSidebar.GetTransition(btnFold.selected ?  "unfold": "fold").Play();
            });
            btnFold.selected = false;




            creditCtr.InitParam(comButtom.GetChild("credit").asTextField);

        }



        void OnCreditEvent(EventData res)
        {
            if (res.name == MetaUIEvent.UpdateNaviCredit)
            {
                UpdateNaviCredit data = res.value as  UpdateNaviCredit;
            }
        }




        /*
        EventCenter.Instance.EventTrigger<EventData>(MetaUIEvent.ON_CREDIT_EVENT,
new EventData<UpdateNaviCredit>(MetaUIEvent.UpdateNaviCredit,
new UpdateNaviCredit()
        {
            isAnim = SyncCreditAnim(isAnim),
    fromCredit = fromCredit,
    toCredit = MainModel.Instance.myCredit
}));
        */
    }

}