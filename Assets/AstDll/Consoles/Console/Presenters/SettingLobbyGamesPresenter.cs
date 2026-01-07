using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public interface IVSettingLobbyGames
{
    event Action onClickNext;
    event Action onClickPrev;
    event Action<FuzzyQueryInfo> onFuzzyQuery;


    event Action<int, bool> onChangeUpdateAtLaunch;
    event Action<int> onClickActive;
    event Action<int> onClickStartUpdate;


    int countPerPage
    {
        get;
    }

    /// <summary>  清除所有内容（包括：页尾、内容、日期） </summary>
    void ClearAll();
    void SetSelectionList(List<string> data);
    void SetSelectionIndex(int index);
    void SetContent(List<SelectedGameInfo> content, int curPageIndex, int pageCount);
}

public class SettingLobbyGamesPresenter
{
    IVSettingLobbyGames view;


    int fromIdxJackpotRecord = 0;
    int curPageIndex = 0;
    int pageCount = 1;


    public void InitParam(IVSettingLobbyGames v)
    {

        if (this.view != null)
        {
            this.view.onFuzzyQuery -= OnFuzzyQuery;
            this.view.onClickPrev -= OnClickPrevPage;
            this.view.onClickNext -= OnClickNextPage;

            this.view.onChangeUpdateAtLaunch -= OnChangeUpdateAtLaunch;
            this.view.onClickActive -= OnClickActive;
            this.view.onClickStartUpdate -= OnClickStartUpdate;
        }

        this.view = v;
        this.view.onFuzzyQuery += OnFuzzyQuery;
        this.view.onClickPrev += OnClickPrevPage;
        this.view.onClickNext += OnClickNextPage;

        this.view.onChangeUpdateAtLaunch += OnChangeUpdateAtLaunch;
        this.view.onClickActive += OnClickActive;
        this.view.onClickStartUpdate += OnClickStartUpdate;

        InitInfo();
    }

    JArray selectedGamesServer = null;
    const string ALL = "All";

    void InitInfo()
    {
        List<string> selectLst = new List<string>() { ALL };

        foreach (JToken item in LobbyGamesManager.Instance.lobbyGamesInfoSever)
        {
            string tp = item["game_type"].Value<string>();
            if (!selectLst.Contains(tp))
            {
                selectLst.Add(tp);
            }
        }
        view.SetSelectionList(selectLst);
        view.SetSelectionIndex(curPageIndex);

        OnFuzzyQuery(null);
    }



    void OnFuzzyQuery(FuzzyQueryInfo info = null) //gameId, gameType gameName
    {
        selectedGamesServer = JArray.Parse(JsonConvert.SerializeObject(LobbyGamesManager.Instance.lobbyGamesInfoSever));

        if (info != null)
        {

            int i = selectedGamesServer.Count;
            while (--i > 0)
            {
                if (info.gameType != null && info.gameType != ALL && selectedGamesServer[i]["game_type"].Value<string>() != info.gameType)
                {
                    selectedGamesServer.RemoveAt(i);
                }
                if (info.gameId != null && selectedGamesServer[i]["game_id"].Value<int>() != info.gameId)
                {
                    selectedGamesServer.RemoveAt(i);
                }
                if (info.gameName != null && selectedGamesServer[i]["game_name"].Value<string>() != info.gameName)
                {
                    selectedGamesServer.RemoveAt(i);
                }
            }

        }

        pageCount = (selectedGamesServer.Count + (view.countPerPage - 1)) / view.countPerPage; //向上取整
        fromIdxJackpotRecord = 0;
        curPageIndex = 0;
        SetUIContent();

    }




    void SetUIContent()
    {
        int lastIdx = fromIdxJackpotRecord + view.countPerPage - 1;
        if (lastIdx > selectedGamesServer.Count - 1)
        {
            lastIdx = selectedGamesServer.Count - 1;
        }

        List<SelectedGameInfo> result = new List<SelectedGameInfo>();

        for (int i = 0; i <= lastIdx - fromIdxJackpotRecord; i++)
        {

            JObject resSer = selectedGamesServer[i + fromIdxJackpotRecord] as JObject;
            SelectedGameInfo item = new SelectedGameInfo();
            int gameId = resSer["game_id"].Value<int>();

            item.gameId = gameId;
            item.gameName = I18nMgr.T(resSer["game_name"].Value<string>());
            item.gameType = resSer["game_type"].Value<string>();
            item.gameSoftwareVer = "1.1.1"; //待完善
            item.gameAlgorithmVer = "1.1.1"; //待完善
            item.updateAtlaunch = LobbyGamesManager.Instance.GetLocalValue<bool>(gameId, "update_at_launch");
            item.active = true; //待完善
            item.gameIconUrl = resSer["lobby_icon_small"].Value<string>();
            item.displayInLobby = LobbyGamesManager.Instance.GetLocalValue<bool>(gameId, "display_in_lobby");
            item.isAvailable = resSer["is_available"].Value<bool>();
            result.Add(item);
        }

        view.SetContent(result, curPageIndex, 00);
    }

    private void OnClickNextPage()
    {
        if (fromIdxJackpotRecord + view.countPerPage >= selectedGamesServer.Count)
            return;

        fromIdxJackpotRecord += view.countPerPage;
        curPageIndex++;
        SetUIContent();
    }
    private void OnClickPrevPage()
    {
        if (fromIdxJackpotRecord <= 0)
            return;

        fromIdxJackpotRecord -= view.countPerPage;
        curPageIndex--;
        if (fromIdxJackpotRecord < 0)
        {
            curPageIndex = 0;
            fromIdxJackpotRecord = 0;
        }
        SetUIContent();
    }



    void OnChangeUpdateAtLaunch(int gameId, bool isSelect)
    {
        LobbyGamesManager.Instance.SetLocalValue<bool>(gameId, "update_at_launch", isSelect);
    }
    void OnClickActive(int gameId){
    
    }

    void OnClickStartUpdate(int gameId){

        MaskPopupHandler.Instance.OpenPopup();

        string modName = LobbyGamesManager.Instance.GetSeverValue<string>(gameId,"module_name");

        if (!string.IsNullOrEmpty(modName))
        {
            ModuleDownloadManager.Instance.ChechDownloadTotalSize(modName,
                (size) =>
                {
                    MaskPopupHandler.Instance.ClosePopup();

                    CommonPopupHandler.Instance.OpenPopup(new CommonPopupInfo()
                    {
                        // 只能修改一次线号机台号，确定要修改？
                        type = CommonPopupType.YesNo,
                        text =string.Format( I18nMgr.T("The total download size for the game is {0}. Are you sure you want to download?"), FileUtils.FormatFileSize(size)),
                        buttonText1 = I18nMgr.T("Cancel"),
                        buttonText2 = I18nMgr.T("OK"),
                        callback1 = null,
                        callback2 = () =>
                        {
                            OnDownloadGame(gameId,modName);
                        },
                    });
                }, 
                (msg) =>
                {
                    MaskPopupHandler.Instance.ClosePopup();

                    TipPopupHandler.Instance.OpenPopup(msg);
                });
        }
        else
        {
            MaskPopupHandler.Instance.ClosePopup();
        }
    }

    void OnDownloadGame( int gameId, string modName)
    {
        ModuleDownloadManager.Instance.HotDownloadGame(modName, (res) =>
        {
            if ((int)res[0]!= 0)
            {
                TipPopupHandler.Instance.OpenPopup((string)res[1]);
            }
            TipPopupHandler.Instance.OpenPopup(I18nMgr.T("Download successful"));
        });
    }
}


/// <summary>
/// 模糊搜索
/// </summary>
public class FuzzyQueryInfo {
    public int? gameId;
    public string gameType;
    public string gameName;
}
public class SelectedGameInfo
{
    public int gameId;
    public string gameName;
    public string gameType;
    public string gameSoftwareVer;
    public string gameAlgorithmVer;
    public bool updateAtlaunch;
    public bool active;
    public string gameIconUrl;
    public bool displayInLobby;
    public bool isAvailable;
}