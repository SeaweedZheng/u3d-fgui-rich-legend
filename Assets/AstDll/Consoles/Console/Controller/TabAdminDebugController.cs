using FairyGUI;
using System.Collections.Generic;
using System.Reflection;
using GameMaker;

public class TabAdminDebugController 
{
    GComponent owner;

    GButton btnErrorCodeQuery;


    GButton tgIsDebugLog, tgUseDebugTool, tgUseReporterPage, tgIsShowUpdateInfo,
        tgPauseAtPopupFreeSpinTrigger, tgPauseAtPopupFreeSpinResult, tgPauseAtPopupJackpotGame, tgPauseAtPopupJackpotOnline;

    GRichTextField txtAppKey, txtServerAddress, txtReportAddress, txtInstallVer, txtSoftwareVer, txtHotfixKey, txtAutoHotfixAddress, txtIOTAddress;


    public void InitParam(GComponent comp)
    {
        owner = comp;


        btnErrorCodeQuery = owner.GetChild("errorCodeQuery").asCom.GetChild("btn").asButton;
        btnErrorCodeQuery.onClick.Set(OnClickErrorCodeQuery);

        tgIsDebugLog = owner.GetChild("isDebugLog").asCom.GetChild("switch").asButton;
        tgIsDebugLog.onClick.Set(OnClickIsDebugLog);

        tgUseDebugTool = owner.GetChild("useDebugTool").asCom.GetChild("switch").asButton;
        tgUseDebugTool.onClick.Set(OnClickUseDebugTool);

        tgUseReporterPage = owner.GetChild("useReporterPage").asCom.GetChild("switch").asButton;
        tgUseReporterPage.onClick.Set(OnClickUseReportPage);

        tgIsShowUpdateInfo = owner.GetChild("isShowUpdateInfo").asCom.GetChild("switch").asButton;
        tgIsShowUpdateInfo.onClick.Set(OnClickIsShowUpdateInfo);

        tgPauseAtPopupFreeSpinTrigger = owner.GetChild("pauseAtPopupFreeSpinTrigger").asCom.GetChild("switch").asButton;
        tgPauseAtPopupFreeSpinTrigger.onClick.Set(OnClickPauseAtPopupFreeSpinTrigger);

        tgPauseAtPopupFreeSpinResult = owner.GetChild("pauseAtPopupFreeSpinResult").asCom.GetChild("switch").asButton;
        tgPauseAtPopupFreeSpinResult.onClick.Set(OnClickpPauseAtPopupFreeSpinResult);

        tgPauseAtPopupJackpotGame = owner.GetChild("pauseAtPopupJackpotGame").asCom.GetChild("switch").asButton;
        tgPauseAtPopupJackpotGame.onClick.Set(OnClickPauseAtPopupJackpotGame);

        tgPauseAtPopupJackpotOnline = owner.GetChild("pauseAtPopupJackpotOnline").asCom.GetChild("switch").asButton;
        tgPauseAtPopupJackpotOnline.onClick.Set(OnClickPauseAtPopupJackpotOnline);

        txtAppKey = owner.GetChild("appKey").asCom.GetChild("value").asRichTextField;
        txtServerAddress = owner.GetChild("serverAddress").asCom.GetChild("value").asRichTextField;
        txtReportAddress = owner.GetChild("reportAddress").asCom.GetChild("value").asRichTextField;
        txtInstallVer = owner.GetChild("installVer").asCom.GetChild("value").asRichTextField;
        txtSoftwareVer = owner.GetChild("softwareVer").asCom.GetChild("value").asRichTextField;
        txtHotfixKey = owner.GetChild("hotfixKey").asCom.GetChild("value").asRichTextField;
        txtAutoHotfixAddress = owner.GetChild("autoHotfixAddress").asCom.GetChild("value").asRichTextField;
        txtIOTAddress = owner.GetChild("iotAddress").asCom.GetChild("value").asRichTextField;




        txtInstallVer.text = $"{ApplicationSettings.Instance.appVersion}/{GlobalData.installHofixVersion}";
        txtSoftwareVer.text = $"{ApplicationSettings.Instance.appVersion}/{GlobalData.hotfixVersion}";
        txtAppKey.text = ApplicationSettings.Instance.appKey;
        txtHotfixKey.text = GlobalData.hotfixKey;
        txtServerAddress.text = ApplicationSettings.Instance.resourceServer;
        txtAutoHotfixAddress.text = GlobalData.autoHotfixUrl;

        tgIsDebugLog.selected = SBoxModel.Instance.isDebugLog;
        tgUseDebugTool.selected = SBoxModel.Instance.isUseTestTool;
        tgUseReporterPage.selected = SBoxModel.Instance.isUseReporterPage ;
        tgPauseAtPopupFreeSpinTrigger.selected = PlayerPrefsUtils.isPauseAtPopupFreeSpinTrigger;
        tgPauseAtPopupJackpotGame.selected = PlayerPrefsUtils.isPauseAtPopupJackpotGame;
        tgPauseAtPopupJackpotOnline.selected = PlayerPrefsUtils.isPauseAtPopupJackpotOnline;
        tgPauseAtPopupFreeSpinResult.selected = PlayerPrefsUtils.isPauseAtPopupFreeSpinResult;

        tgIsShowUpdateInfo.selected = false;


        SetItemTouchable(tgIsDebugLog, !ApplicationSettings.Instance.isRelease);
        SetItemTouchable(tgUseDebugTool, !ApplicationSettings.Instance.isRelease);
        SetItemTouchable(tgUseReporterPage, !ApplicationSettings.Instance.isRelease);
        SetItemTouchable(tgPauseAtPopupFreeSpinTrigger, !ApplicationSettings.Instance.isRelease);
        SetItemTouchable(tgPauseAtPopupJackpotGame, !ApplicationSettings.Instance.isRelease);
        SetItemTouchable(tgPauseAtPopupJackpotOnline, !ApplicationSettings.Instance.isRelease);
        SetItemTouchable(tgPauseAtPopupFreeSpinResult, !ApplicationSettings.Instance.isRelease);
    }

    public void SetItemTouchable(GButton btn, bool isTouchable)
    {
        btn.GetChild("interactable").visible = !isTouchable;
        btn.touchable = isTouchable;
    }


    void OnClickIsDebugLog()
    {
        SBoxModel.Instance.isDebugLog = !SBoxModel.Instance.isDebugLog;
        tgIsDebugLog.selected = SBoxModel.Instance.isDebugLog;
    }

    void OnClickUseDebugTool()
    {
        SBoxModel.Instance.isUseTestTool = !SBoxModel.Instance.isUseTestTool;
        tgUseDebugTool.selected = SBoxModel.Instance.isUseTestTool;
        TestUtils.CheckTestManager();
    }

    void OnClickUseReportPage()
    {
        SBoxModel.Instance.isUseReporterPage = !SBoxModel.Instance.isUseReporterPage;
        tgUseReporterPage.selected = SBoxModel.Instance.isUseReporterPage;
        TestUtils.CheckReporter();
    }

    void OnClickIsShowUpdateInfo()
    {

    }


    void OnClickPauseAtPopupFreeSpinTrigger()
    {
        PlayerPrefsUtils.isPauseAtPopupFreeSpinTrigger = !PlayerPrefsUtils.isPauseAtPopupFreeSpinTrigger;
        tgPauseAtPopupFreeSpinTrigger.selected = PlayerPrefsUtils.isPauseAtPopupFreeSpinTrigger;
    }

    void OnClickpPauseAtPopupFreeSpinResult()
    {
        PlayerPrefsUtils.isPauseAtPopupFreeSpinResult = !PlayerPrefsUtils.isPauseAtPopupFreeSpinResult;
        tgPauseAtPopupFreeSpinResult.selected = PlayerPrefsUtils.isPauseAtPopupFreeSpinResult;
    }


    void OnClickPauseAtPopupJackpotGame()
    {
        PlayerPrefsUtils.isPauseAtPopupJackpotGame = !PlayerPrefsUtils.isPauseAtPopupJackpotGame;
        tgPauseAtPopupJackpotGame.selected = PlayerPrefsUtils.isPauseAtPopupJackpotGame;
    }

    void OnClickPauseAtPopupJackpotOnline()
    {
        PlayerPrefsUtils.isPauseAtPopupJackpotOnline = !PlayerPrefsUtils.isPauseAtPopupJackpotOnline;
        tgPauseAtPopupJackpotOnline.selected = PlayerPrefsUtils.isPauseAtPopupJackpotOnline;
    }

    void OnClickErrorCodeQuery()
    {
        //ErrorCode

        BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;
        FieldInfo[] staticFields = typeof(ErrorCode).GetFields(bindingFlags) ;

        var result = new List<string>();
        foreach (FieldInfo field in staticFields)
        {
            string fieldName = field.Name; // 字段名
            object fieldValue = field.GetValue(null); // 静态字段无实例，GetValue 参数传 null

            // 拼接目标格式，添加到列表
            result.Add($"{fieldName}: {fieldValue}");
        }

        Dictionary<string,object> req= new Dictionary<string,object>();

        PageManager.Instance.OpenPage(PageName.ConsolePopupConsoleNotice, new EventData<Dictionary<string, object>>("",     
            new Dictionary<string, object>()
            {
                ["title"] = I18nMgr.T("Error Code Query"),
                ["content"] = result,
            }
        ), null);
    }
}
