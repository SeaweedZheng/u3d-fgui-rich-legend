using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIRouter
{
    private static UIRouter _instance;
    public static UIRouter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIRouter();
            }
            return _instance;
        }
    }


    public Dictionary<PageName, object[]> pathDict;

    public UIRouter()
    {

        pathDict = new Dictionary<PageName, object[]>()
        {
            // 通用
            [PageName.CommonPopupSystemTip] = new object[] { "Assets/AstBundle/_Commons/Common/FGUIs", "Common.PopupSystemTip" },

            // [Console Slot]拉霸机后台
            [PageName.ConsolePageConsoleMain] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleMain" },
            [PageName.ConsolePageConsoleMachineSettings] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleMachineSettings" },
            [PageName.ConsolePopupI18nTest] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupI18nTest" },
            [PageName.ConsolePageConsoleBusinessRecord] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleBusinessRecord" },
            [PageName.ConsolePageConsoleGameInformation] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleGameInformation" },
            [PageName.ConsolePopupConsoleKeyboard001] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleKeyboard001" },
            [PageName.ConsolePopupConsoleKeyboard002] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleKeyboard002" },
            [PageName.ConsolePopupConsoleTip] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleTip" },
            [PageName.ConsolePopupConsoleCommon] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleCommon" },
            [PageName.ConsolePopupConsoleSetParameter002] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleSetParameter002" },
            [PageName.ConsolePopupConsoleSetParameter001] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleSetParameter001" },
            [PageName.ConsolePopupConsoleCoder] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleCoder" },
            [PageName.ConsolePopupConsoleMask] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleMask" },
            [PageName.ConsolePopupConsoleSlideSetting] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleSlideSetting" },
            [PageName.ConsolePageDrawLine] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleDrawLine" },
            [PageName.ConsolePopupConsoleCalendar] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleCalendar" },
            [PageName.ConsolePopupConsoleSound] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleSound" },
            [PageName.ConsolePopupConsoleChoose001] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleChoose001" },
            [PageName.ConsolePageConsoleLogRecord] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleLogRecord" },
            [PageName.ConsolePageConsoleHardwareTest] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleHardwareTest" },
            [PageName.ConsolePageConsoleCheckScreenColor] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleCheckScreenColor" },
            [PageName.ConsolePageConsoleAdmin] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleAdmin" },
            [PageName.ConsolePopupConsoleNotice] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleNotice" },
            [PageName.ConsolePageConsoleGameHistory] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleGameHistory" },
            [PageName.ConsolePageConsoleSettingsMenu] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsMenu" },
            [PageName.ConsolePageConsoleSettingsMachine] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsMachine" },
            [PageName.ConsolePageConsoleSettingsInOut] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsInOut" },
            [PageName.ConsolePageConsoleSettingsRecord] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsRecord" },
            [PageName.ConsolePageConsoleSettingsPrinter] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsPrinter" },
            [PageName.ConsolePageConsoleSettingsBillValidator] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsBillValidator" },
            [PageName.ConsolePageConsoleSettingsRemoteControl] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsRemoteControl" },
            [PageName.ConsolePageConsoleSettingsScreen] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsScreen" },
            [PageName.ConsolePageConsoleSettingsIOT] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsIOT" },
            [PageName.ConsolePageConsoleSettingsGames] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PageConsoleSettingsGames" },
            [PageName.ConsolePopupConsoleCommon002] = new object[] { "Assets/AstBundle/Consoles/Console/FGUIs", "ConsoleSlot01.PopupConsoleCommon002" },


            // [Console Coin Pusher]推币机新后台
            [PageName.ConsolePusher01PageConsoleAdmin] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleAdmin" },
            [PageName.ConsolePusher01PageConsoleMain] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleMain" },
            [PageName.ConsolePusher01PageConsoleCheckHardware] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleCheckHardware" },
            [PageName.ConsolePusher01PageConsoleCoder] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleCoder" },
            [PageName.ConsolePusher01PageConsoleSettings] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleSettings" },
            [PageName.ConsolePusher01PageConsoleTestCoinPush]=new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleTestCoinPush" },
            [PageName.ConsolePusher01PageConsoleSetParameter002] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleSetParameter002" },
            [PageName.ConsolePusher01PageConsoleSetParameter001] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleSetParameter001" },
            [PageName.ConsolePusher01PageConsoleBusinessRecord] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleBusinessRecord" },
            [PageName.ConsolePusher01PageConsoleCheckHardware02] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleCheckHardware02" },
            [PageName.ConsolePusher01PageConsoleRecordChoose] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleRecordChoose" },
            [PageName.ConsolePusher01PageConsoleEventRecord] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleEventRecord" },
            [PageName.ConsolePusher01PageConsoleErrorRecord] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleErrorRecord" },
            [PageName.ConsolePusher01PopupConsoleRecord] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PopupConsoleRecord" },
            [PageName.ConsolePusher01PageConsoleJpRecord] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleJpRecord" },
            [PageName.ConsolePusher01PageConsoleCoinRecord] = new object[] { "Assets/AstBundle/Consoles/Console Coin Pusher 01/FGUIs", "ConsoleCoinPusher01.PageConsoleCoinRecord" },






            // 大厅
            [PageName.Lobby01PageLobbyMain] = new object[] { "Assets/AstBundle/Lobbys/Lobby01/FGUIs", "Lobby01.PageLobbyMain" },




            // [Coin Pusher]帝国之辉
            [PageName.PusherEmperorsReinPopupGameLoading] = new object[] { "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/FGUIs", "PusherEmperorsRein.PopupGameLoading" },
            [PageName.PusherEmperorsReinPageGameMain] = new object[] { "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/FGUIs", "PusherEmperorsRein.PageGameMain" },
            [PageName.PusherEmperorsReinPopupBigWin] = new object[] { "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/FGUIs", "PusherEmperorsRein.PopupBigWin" },
            [PageName.PusherEmperorsReinPopupFreeSpinTrigger] = new object[] { "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/FGUIs", "PusherEmperorsRein.PopupFreeSpinTrigger" },
            [PageName.PusherEmperorsReinPopupJackpotGame] = new object[] { "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/FGUIs", "PusherEmperorsRein.PopupJackpotGame" },

            [PageName.PusherEmperorsReinPopupJackpotOnline] = new object[] { "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/FGUIs", "PusherEmperorsRein.PopupJackpotOnline" },
            [PageName.PusherEmperorsReinPopupFreeSpinResult] = new object[] { "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/FGUIs", "PusherEmperorsRein.PopupFreeSpinResult" },
            [PageName.PusherEmperorsReinPageFreeBonusGame2] = new object[] { "Assets/AstBundle/Games/BonusGame2/FGUIs", "PusherEmperorsRein.PageFreeBonusGame2" },

            // [Slot]帝国之辉
            [PageName.SlotEmperorsReinPageERGameMain] = new object[] { "Assets/AstBundle/Games/Coin Pusher Emperors Rein 200/FGUIs", "SlotEmperorsRein.PageGameMainSlot" },


            // 富翁传奇
            [PageName.RichLegend2001001PageGameMain] = new object[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/FGUIs", "CoinPusherRichLegend2001001.PageGameMain" },
            [PageName.RichLegend2001001PopupGameLoading] = new object[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/FGUIs", "CoinPusherRichLegend2001001.PopupGameLoading" },


        };
    }
}


public enum PageName
{

    // 通用
    CommonPopupSystemTip,


    // 拉霸机-管理后天
    ConsolePageConsoleMain,
    ConsolePageConsoleMachineSettings,
    ConsolePopupI18nTest,
    ConsolePageConsoleBusinessRecord,
    ConsolePageConsoleGameInformation,
    ConsolePopupConsoleKeyboard001,
    ConsolePopupConsoleKeyboard002,
    ConsolePopupConsoleTip,
    ConsolePopupConsoleCommon,
    ConsolePopupConsoleSetParameter002,
    ConsolePopupConsoleSetParameter001,
    ConsolePopupConsoleCoder,
    ConsolePopupConsoleMask,
    ConsolePopupConsoleSlideSetting,
    ConsolePageDrawLine,
    ConsolePopupConsoleCalendar,
    ConsolePopupConsoleSound,
    ConsolePopupConsoleChoose001,
    ConsolePageConsoleLogRecord,
    ConsolePageConsoleHardwareTest,
    ConsolePageConsoleCheckScreenColor,
    ConsolePageConsoleAdmin,
    ConsolePopupConsoleNotice,
    ConsolePageConsoleGameHistory,
    ConsolePageConsoleSettingsMenu,
    ConsolePageConsoleSettingsMachine,
    ConsolePageConsoleSettingsInOut,
    ConsolePageConsoleSettingsRecord,
    ConsolePageConsoleSettingsPrinter,
    ConsolePageConsoleSettingsBillValidator,
    ConsolePageConsoleSettingsRemoteControl,
    ConsolePageConsoleSettingsScreen,
    ConsolePageConsoleSettingsIOT,
    ConsolePageConsoleSettingsGames,
    ConsolePopupConsoleCommon002,

    // 推币机新后台
    ConsolePusher01PageConsoleMain,
    ConsolePusher01PageConsoleCheckHardware,
    ConsolePusher01PageConsoleCoder,
    ConsolePusher01PageConsoleSettings,
    ConsolePusher01PageConsoleTestCoinPush,
    ConsolePusher01PageConsoleSetParameter001,
    ConsolePusher01PageConsoleSetParameter002,
    ConsolePusher01PageConsoleBusinessRecord,
    ConsolePusher01PageConsoleAdmin,
    ConsolePusher01PageConsoleCheckHardware02,
    ConsolePusher01PageConsoleRecordChoose,
    ConsolePusher01PageConsoleEventRecord,
    ConsolePusher01PageConsoleErrorRecord,
    ConsolePusher01PopupConsoleRecord,
    ConsolePusher01PageConsoleJpRecord,
    ConsolePusher01PageConsoleCoinRecord,


    // 推币机-帝国之辉
    PusherEmperorsReinPageGameMain,
    PusherEmperorsReinPopupGameLoading,
    PusherEmperorsReinPopupBigWin,
    PusherEmperorsReinPopupFreeSpinTrigger,
    PusherEmperorsReinPopupFreeSpinResult,
    PusherEmperorsReinPopupJackpotGame,

    PusherEmperorsReinPopupJackpotOnline,
    PusherEmperorsReinPageFreeBonusGame2,
    
    // 拉霸机-帝国之辉
    SlotEmperorsReinPageERGameMain,


    //【拉霸机】
    SlotEmperorsReinPageFreeBonusGame1,




    Lobby01PageLobbyMain,




    // 富翁传奇2001001
    RichLegend2001001PageGameMain,
    RichLegend2001001PopupGameLoading,
}


