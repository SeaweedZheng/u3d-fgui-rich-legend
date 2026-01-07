using GameMaker;
using SBoxApi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IVSettingInOutPresnter
{

    event Action onClickCoinInScale;
    event Action onClickCoinOutScale;
    event Action onClickScoreScale;

    void SetCoinInScaleTxt(string content);
    void SetCoinOutScaleTxt(string content);
    void SetScoreScaleTxt(string content);
}
public class SettingInOutPresnter {
    IVSettingInOutPresnter view;

    public void InitParam(IVSettingInOutPresnter v)
    {
        view = v;


        view.onClickCoinInScale -= OnClickCoinInScale;
        view.onClickCoinOutScale -= OnClickCoinOutScale;
        view.onClickScoreScale -= OnClickScoreScale;

        view.onClickCoinInScale += OnClickCoinInScale;
        view.onClickCoinOutScale += OnClickCoinOutScale;
        view.onClickScoreScale += OnClickScoreScale;




        SetCoinInScaleTxt();
        SetCoinOutScaleTxt();
        SetScoreScaleTxt();
    }

    void SetCoinInScaleTxt()
    {
        view.SetCoinInScaleTxt($"1:{SBoxModel.Instance.CoinInScale}");
    }

    void SetCoinOutScaleTxt()
    {
        long perCredit2Ticket = SBoxModel.Instance.CoinOutScaleTicketPerCredit;
        long perTicket2Credit = SBoxModel.Instance.CoinOutScaleCreditPerTicket;
        string str = perCredit2Ticket > 1 ? $"{perCredit2Ticket}:1" : $"1:{perTicket2Credit}";
        view.SetCoinOutScaleTxt(str);
    }
    void SetScoreScaleTxt()
    {
        view.SetScoreScaleTxt($"1:{SBoxModel.Instance.ScoreUpDownScale}");
    }


    async void OnClickCoinOutScale()
    {
        //int curValue = SBoxModel.Instance.CoinOutScaleTicketPerCredit > 1?

        Func<int, string> onChangeUI = (int val) => {
            string str = val < 0 ? $"{-val}:1" : $"1:{val}";

            DebugUtils.Log(str + $"{val}");
            return str;
        };

        int curValue = SBoxModel.Instance.CoinOutScaleTicketPerCredit > 1 ? -SBoxModel.Instance.CoinOutScaleTicketPerCredit :
            SBoxModel.Instance.CoinOutScaleCreditPerTicket;

        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleSlideSetting,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Coin Out Scale(Ticket : Credit):"),
                    ["valueMin"] = -DefaultSettingsUtils.maxCoinOutTicketPerCredit,//50; // 1分多少票
                    ["valueMax"] = DefaultSettingsUtils.maxCoinOutCreditPerTicket,//200;// 1票多少分
                    ["valueCur"] = curValue, // 1币多少分
                    ["onChangeUI"] = onChangeUI,
                    ["isUseKeyboard"] = false,
                })
        );

        if (res.value != null)
        {
            int data = (int)res.value;
            //DebugUtil.Log($"@@ 1分几票   {data["valueLeft"]};  1票多少分 {data["valueRight"]}");

            int perCredit2Ticket = SBoxModel.Instance.CoinOutScaleTicketPerCredit;
            int perTicket2Credit = SBoxModel.Instance.CoinOutScaleCreditPerTicket;

            if (data < 0)
            {
                perCredit2Ticket = -data;
                perTicket2Credit = 1;
            }
            else
            {
                perCredit2Ticket = 1;
                perTicket2Credit = data;
            }

            if (perCredit2Ticket == SBoxModel.Instance.CoinOutScaleTicketPerCredit
                && perTicket2Credit == SBoxModel.Instance.CoinOutScaleCreditPerTicket)
                return;

            MachineDataUtils.RequestSetCoinInCoinOutScale(null, perTicket2Credit, perCredit2Ticket, null,
            (res01) => {
                SBoxPermissionsData data = res01 as SBoxPermissionsData;
                if (data.result == 0)
                {

                    SetCoinOutScaleTxt();
                }
                else
                    TipPopupHandler.Instance.OpenPopup(I18nMgr.T("Save failed, clear first with a reset code."));
            },
            (err) =>
            {
                TipPopupHandler.Instance.OpenPopup(I18nMgr.T(err.msg));
            });

        }

    }

    async void OnClickCoinInScale()
    {
        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleSlideSetting,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Coin In Scale(Coin : Credit):"),
                    ["valueMax"] = DefaultSettingsUtils.maxCoinInScale,//200;
                    ["valueMin"] = DefaultSettingsUtils.minCoinInScale,//200;
                    ["valueCur"] = SBoxModel.Instance.CoinInScale, // 1币多少分
                })
        );

        if (res.value != null)
        {
            int data = (int)res.value;

            int coinInScale = data;

            //SBoxModel.Instance.coinInScale = coinInScale;
            //btnCoinInScale.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"1:{coinInScale}";

            if (coinInScale == SBoxModel.Instance.CoinInScale) return;

            MachineDataUtils.RequestSetCoinInCoinOutScale(coinInScale, null, null, null,
            (res) => {
                SBoxPermissionsData data = res as SBoxPermissionsData;
                if (data.result == 0)
                {
                    SetCoinInScaleTxt();
                }
                else
                    TipPopupHandler.Instance.OpenPopup(I18nMgr.T("Save failed, clear first with a reset code."));
            },
            (err) =>
            {
                TipPopupHandler.Instance.OpenPopup(I18nMgr.T(err.msg));
            });
        }

    }

    async void OnClickScoreScale()
    {

        EventData res = await PageManager.Instance.OpenPageAsync(PageName.ConsolePopupConsoleSlideSetting,
            new EventData<Dictionary<string, object>>("",
                new Dictionary<string, object>()
                {
                    ["title"] = I18nMgr.T("Score Scale(Time : Credit):"),
                    ["valueMax"] = DefaultSettingsUtils.maxScoreUpDownScale,
                    ["valueMin"] = DefaultSettingsUtils.minScoreUpDownScale,
                    ["valueCur"] = SBoxModel.Instance.ScoreUpDownScale, // 1次多少分
                })
        );


        if (res.value != null)
        {
            int data = (int)res.value;

            int scoreUpDownScale = data;

            if (scoreUpDownScale == SBoxModel.Instance.ScoreUpDownScale) return;

            MachineDataUtils.RequestSetCoinInCoinOutScale(null, null, null, scoreUpDownScale,
            (res) => {
                SBoxPermissionsData data = res as SBoxPermissionsData;
                if (data.result == 0)
                {
                    SetScoreScaleTxt();
                }
                else
                    TipPopupHandler.Instance.OpenPopup(I18nMgr.T("Save failed, clear first with a reset code."));
            },
            (err) =>
            {
                TipPopupHandler.Instance.OpenPopup(I18nMgr.T(err.msg));
            });
        }

    }


}
