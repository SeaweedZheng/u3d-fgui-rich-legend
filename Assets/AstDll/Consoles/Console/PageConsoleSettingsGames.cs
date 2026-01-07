using FairyGUI;
using GameMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;

namespace ConsoleSlot01
{
    public class PageConsoleSettingsGames : PageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleSettingsGames";
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

            // 异步加载资源
            /*
            if (!string.IsNullOrEmpty(lobbyGamesInfo))
            {
                callback();
            }
            else
            {
                BackupManager.Instance.LoadAsset<TextAsset>(
                "Assets/GameBackup/Lobbys/Game Info/lobby_games.json",
                (TextAsset txa) =>
                {
                    lobbyGamesInfo = txa.text;

                    callback();
                });
            }
            */


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


       // string lobbyGamesInfo = null;


        // public override void OnTop() { DebugUtils.Log($"i am top {this.name}"); }

        GButton btnClose, btnCloseP,  btnPrev, btnNext;


        GComponent comNavBottomPages;

        SettingLobbyGamesView settingLobbyGamesView = new SettingLobbyGamesView();
        SettingLobbyGamesPresenter settingLobbyGamesPresenter = new SettingLobbyGamesPresenter();
        public override void InitParam()
        {

            if (!isInit) return;

            if (!isOpen) return;

            // btnClose =  this.contentPane.GetChild("btnExit").asButton;
            btnClose = this.contentPane.GetChild("navBottom").asCom.GetChild("btnExit").asButton;
            btnClose.onClick.Clear();
            btnClose.onClick.Add(() =>
            {
                CloseSelf(null);
            });

            comNavBottomPages = this.contentPane.GetChild("navBottomPages").asCom;

            btnCloseP = comNavBottomPages.asCom.GetChild("btnExit").asButton;
            btnCloseP.onClick.Clear();
            btnCloseP.onClick.Add(() =>
            {
                CloseSelf(null);
            });

            GList tabs = this.contentPane.GetChild("tabs").asList;
            GComponent tabGames = tabs.GetChildAt(0).asCom;


            settingLobbyGamesView.InitParam(tabGames);
            settingLobbyGamesPresenter.InitParam(settingLobbyGamesView);



            btnPrev = comNavBottomPages.GetChild("btnPrev").asButton;
            btnNext = comNavBottomPages.GetChild("btnNext").asButton;
            btnPrev.onChanged.Clear();
            btnNext.onChanged.Clear();
            btnPrev.onClick.Add(OnClickPrev);
            btnNext.onClick.Add(OnClickNext);

            SetNavBottomTitle();
        }



        void OnClickPrev()
        {
            settingLobbyGamesView.OnClickPrev();

            SetNavBottomTitle();
        }

        void OnClickNext()
        {

            settingLobbyGamesView.OnClickNext();

            SetNavBottomTitle();
        }
        void SetNavBottomTitle()
        {
            string title = string.Format(I18nMgr.T("Games, Page {0} of {1}"), settingLobbyGamesView.curPageIndex, settingLobbyGamesView.pageCount);

            comNavBottomPages.GetChild("title").asTextField.text = title;
        }
    }
}