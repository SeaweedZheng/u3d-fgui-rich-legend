using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameMaker;
using UnityEditor;
using Newtonsoft.Json.Linq;
namespace ConsoleSlot01
{
    public class PageConsoleSettingsMenu : PageBase
    {
        public const string pkgName = "Console";
        public const string resName = "PageConsoleSettingsMenu";
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

        GButton btnCloseP;

        GRichTextField navBottomPagesTitle;

        GList menu;

        Dictionary<string, GComponent> menuItems = new Dictionary<string, GComponent>();

        public override void InitParam()
        {

            if (!isInit) return;

            if (!isOpen) return;


            btnCloseP = this.contentPane.GetChild("navBottomPages").asCom.GetChild("btnExit").asButton;
            btnCloseP.onClick.Clear();
            btnCloseP.onClick.Add(() =>
            {
                //DebugUtils.Log("i am here 123");
                CloseSelf(null);
                //  CloseSelf(new EventData("Exit"));
            });

            navBottomPagesTitle = this.contentPane.GetChild("navBottomPages").asCom.GetChild("title").asRichTextField;


            menu = this.contentPane.GetChild("menu1").asList;

            GObject[] childs = menu.GetChildren();

            foreach (GObject item in childs)
            {
                if (menuItems.ContainsKey(item.name))
                    menuItems.Remove(item.name);

                menuItems.Add(item.name, item.asCom);
            }


            while (menu.numChildren > 0)
            {
                menu.RemoveChildAt(menu.numChildren - 1);
            }

            string userDisplayName = SBoxModel.Instance.isCurPermissionsAdmin ? "admin_display" :
                SBoxModel.Instance.isCurPermissionsManager ? "manager_display" :
                SBoxModel.Instance.isCurPermissionsShift ? "staff_display" : null;

            if (userDisplayName != null)
            {
                foreach (JToken nod in SBoxModel.Instance.consoleDisplayPermissions)
                {
                    bool isEnable = nod[ApplicationSettings.Instance.isRelease ? "release_has" : "debug_has"]
                        .Value<bool>();
                    if (isEnable)
                    {
                        if (nod[userDisplayName].Value<bool>())
                        {
                            GComponent chd = menuItems[nod["key"].Value<string>()];
                            menu.AddChild(chd);
                            chd.onClick.Clear();
                            chd.onClick.Add(() =>
                            {
                                OnClickMenuItem(chd.name);
                            });
                        }
                    }
                }
            }

            OnPageChang(0, 1);
        }
        void OnClickMenuItem(string name)
        {

            DebugUtils.Log($"name: {name}");
            switch (name)
            {
                case SettingModuleName.settingMachine:
                    {
                        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleSettingsMachine);
                    }
                    break;
                case SettingModuleName.settingGames:
                    {
                        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleSettingsGames);
                    }
                    break;
                case SettingModuleName.settingInOut:
                    {
                        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleSettingsInOut);
                    }
                    break;
                case SettingModuleName.settingRecord:
                    {
                        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleSettingsRecord);
                    }
                    break;
                case SettingModuleName.settingScreen:
                    {
                        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleSettingsScreen);
                    }
                    break;
                case SettingModuleName.settingBillValidator:
                    {
                        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleSettingsBillValidator);
                    }
                    break;
                case SettingModuleName.settingPrinter:
                    {
                        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleSettingsPrinter);
                    }
                    break;
                case SettingModuleName.settingRemoteControl:
                    {
                        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleSettingsRemoteControl);                     
                    }
                    break;

                case SettingModuleName.settingIOT:
                    {
                        PageManager.Instance.OpenPage(PageName.ConsolePageConsoleSettingsIOT);
                    }
                    break;
                case SettingModuleName.settingSas:
                    {

                    }
                    break;
                case SettingModuleName.settingJackpotOnline:
                    {

                    }
                    break;

                case SettingModuleName.settingATM:
                    {

                    }
                    break;
                case SettingModuleName.settingDigitCounter:
                    {

                    }
                    break;
            }
        }
        void OnPageChang(int curPageIndex, int pageCount)
        {
            string str = string.Format(I18nMgr.T("Settings, Page: {0}/{1}"), curPageIndex + 1, pageCount);
            navBottomPagesTitle.text = str;
        }
    }

}



public class SettingModuleName
{
    public const string settingMachine = nameof(settingMachine);

    public const string settingGames = nameof(settingGames);

    public const string settingInOut = nameof(settingInOut);

    public const string settingRecord = nameof(settingRecord);

    public const string settingScreen = nameof(settingScreen);

    public const string settingBillValidator = nameof(settingBillValidator);

    public const string settingPrinter = nameof(settingPrinter);

    public const string settingRemoteControl = nameof(settingRemoteControl);

    public const string settingIOT = nameof(settingIOT);

    public const string settingSas = nameof(settingSas);

    public const string settingJackpotOnline = nameof(settingJackpotOnline);

    public const string settingATM = nameof(settingATM);

    public const string settingDigitCounter = nameof(settingDigitCounter);
}