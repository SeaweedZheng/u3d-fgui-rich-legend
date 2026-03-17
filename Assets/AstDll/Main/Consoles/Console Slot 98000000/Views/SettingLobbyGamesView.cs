using FairyGUI;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingLobbyGamesView : IVSettingLobbyGames, IVTable
{
    public event Action onClickNext;
    public event Action onClickPrev;
    public event Action<FuzzyQueryInfo> onFuzzyQuery;

    public int countPerPage =>4;


    List<GComponent> comGames = new List<GComponent>();

    GButton btnSearch;
    GLabel glabTip;

    GComboBox combSelect;

    List<string> selectionList;
    public void InitParam(GComponent g)
    {
        GComponent goCoontent = g;
        btnSearch = goCoontent.GetChild("search").asCom.GetChild("value").asButton;
        btnSearch.onClick.Clear();
        btnSearch.onClick.Add(() =>
        {
            //onClickBillValidatorModel?.Invoke();
            // 选择弹窗
        });

        combSelect = goCoontent.GetChild("gameType").asCom.GetChild("value").asComboBox;
        combSelect.onChanged.Clear();
        combSelect.onChanged.Add(OnSelect);

        comGames.Clear();
        for (int i=0; i<4; i++)
        {
            comGames.Add(goCoontent.GetChild($"game{i}").asCom);
        }
    }

    void OnSelect()
    {
        int index = combSelect.selectedIndex;
        onFuzzyQuery?.Invoke(new FuzzyQueryInfo()
        {
            gameType = combSelect.value,
        });
    }

    public void SetSelectionList(List<string> data)
    {
        selectionList = data;
        List<string> showLst = new List<string>();
        foreach (var item in data)
        {
            showLst.Add($"--{I18nMgr.T(item)}");
        }
        combSelect.items = showLst.ToArray(); // 一个用来显示（多语言）
        combSelect.values = data.ToArray();// 原值
        combSelect.selectedIndex = 0;
    }


    public void ClearAll()
    {
        foreach (var item in comGames)
        {
            item.visible = false;
        }
        combSelect.items = new string[] { };// 一个用来显示（多语言），一个是原值
        combSelect.values = new string[] { }; // 一个用来显示（多语言），一个是原值
        combSelect.selectedIndex = 0;
    }

    public void SetSelectionIndex(int index)
    {
        combSelect.selectedIndex = index;
    }
    public void SetContent(List<SelectedGameInfo> content, int curPageIndex, int pageCount)
    {
        this._curPageIndex = curPageIndex;
        this._pageCount = pageCount;
        foreach (var item in comGames)
        {
            item.visible = false;
        }

        for  (int i=0; i< content.Count; i++)
        {
            GComponent goItem = comGames[i];
            goItem.visible = true;


            GLabel glabTip = goItem.GetChild("tip").asLabel;

            GComponent goInfo = goItem.GetChild("info").asCom;

            // 整体变灰色，不可触摸(服务器标明，不可访问)
            goInfo.grayed = !content[i].isAvailable;
            goInfo.touchable = content[i].isAvailable;

            // 添加提示
            glabTip.visible = !content[i].isAvailable;

            // 加载头像
            string pth = Application.isEditor ?
                PathHelper.GetAssetBackupSAPTH(content[i].gameIconUrl) :
                PathHelper.GetAssetBackupLOCPTH(content[i].gameIconUrl);
            FileLoaderManager.Instance.LoadImageAsTexture(pth, (Texture2D texture) =>
            {
                NTexture nTexture = new NTexture(texture);
                GLoader icon = goInfo.GetChild("avatar").asLoader;
                icon.texture = nTexture;                                 
                icon.fill = FillType.ScaleFree;      // 等比缩放，可能留白                                       
            });

            int gameId = content[i].gameId;
            goInfo.GetChild("name").asCom.GetChild("value").asTextField.text = content[i].gameName;
            goInfo.GetChild("gameType").asCom.GetChild("value").asTextField.text = content[i].gameType;
            goInfo.GetChild("gameId").asCom.GetChild("value").asTextField.text = content[i].gameId.ToString();
            goInfo.GetChild("verSoftware").asCom.GetChild("value").asTextField.text = content[i].gameSoftwareVer.ToString();
            goInfo.GetChild("verAlgorithm").asCom.GetChild("value").asTextField.text = content[i].gameAlgorithmVer.ToString();

            GButton btnUpdateAtLaunch = goInfo.GetChild("updateAtLaunch").asCom.GetChild("toggle").asButton;
            btnUpdateAtLaunch.selected = content[i].updateAtlaunch;
            btnUpdateAtLaunch.onChanged.Clear();
            btnUpdateAtLaunch.onChanged.Add(() =>
            {
                onChangeUpdateAtLaunch?.Invoke(gameId, btnUpdateAtLaunch.selected);
            });

            GButton btnStartUpdate = goInfo.GetChild("startUpdate").asCom.GetChild("value").asButton;
            btnStartUpdate.onClick.Clear();
            btnStartUpdate.onClick.Add(() =>
            {
                onClickStartUpdate?.Invoke(gameId);
            });

            GButton btnActive = goInfo.GetChild("active").asCom.GetChild("value").asButton;
            btnActive.onClick.Clear();
            btnActive.onClick.Add(() =>
            {
                onClickActive?.Invoke(gameId);
            });
            btnActive.title = content[i].active? 
                "<img src='ui://Console/icon_active4aff00' width='20' height='20'/>" :
                "<img src='ui://Console/icon_active666666' width='20' height='20'/>";

        }

        onChangeNavBottomTitle?.Invoke(curPageIndex, pageCount);
    }



    public event Action<int, bool> onChangeUpdateAtLaunch;
    public event Action<int> onClickActive;
    public event Action<int> onClickStartUpdate;







    int _curPageIndex, _pageCount;

    #region IVTable

    public int curPageIndex => _curPageIndex;
    public int pageCount => _pageCount;
    public void OnClickNext() => onClickNext?.Invoke();

    public void OnClickPrev() => onClickPrev?.Invoke();

    public event Action<int, int> onChangeNavBottomTitle;
    #endregion


}
