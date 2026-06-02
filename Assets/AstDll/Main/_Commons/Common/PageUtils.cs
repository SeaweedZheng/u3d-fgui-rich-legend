using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageUtils  
{

    /// <summary> 回到大厅 </summary>
    public static void GoBackToLobby()
    {
        int index = PageManager.Instance.IndexOf(PageName.Lobby89000000PageLobbyMain);
        if (index != -1)
        {
            while (true)
            {
                PageBase pg = PageManager.Instance.GetTopPage();

                if (pg.pageName == PageName.Lobby89000000PageLobbyMain)
                {
                    MainModel.Instance.gameID = -1;
                    return;
                }
                else
                {
                    PageManager.Instance.ClosePage(pg);
                }
            }
        }
    }

    public static void EnterGame(int gameId, Action onFinishCalllback = null)
    {
        GameUpdateChecker.Instance.CheckPlayabilityWhenEnterGame(gameId, (isPlayable) =>
        {
            if (isPlayable)
            {
                MaskPopupHandler.Instance.OpenPopup();

                ModuleDownloadManager.Instance.AddModeToRuning(gameId);

                string enterPageName = LobbyGamesManager.Instance.GetGameValueFromServer<string>(gameId, "enter_page");
                PageName pn = (PageName)Enum.Parse(typeof(PageName), enterPageName);
                PageManager.Instance.OpenPage(pn,
                onFinishCalllback: (page) =>
                {
                    MainModel.Instance.gameID = gameId;
                    MaskPopupHandler.Instance.ClosePopup();

                    onFinishCalllback?.Invoke();
                });
            }
        });
    }
    /// <summary>
    /// 是否存在大厅
    /// </summary>
    public static bool HasLobby => PageManager.Instance.IndexOf(PageName.Lobby89000000PageLobbyMain) != -1;
}
