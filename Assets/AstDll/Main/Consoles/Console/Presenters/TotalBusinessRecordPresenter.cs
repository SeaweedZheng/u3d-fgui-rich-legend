using SBoxApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMaker;
using FairyGUI;

public interface IVTotalBusinessRecord
{
    void ClearAll();
    void SetTotalBet(long credit);
    void SetTotalWin(long credit);
    void SetTotalProfitlBet(long credit);

    void SetTotalCoinIn(long credit);
    void SetTotalCoinOut(long credit);
    void SetTotalProfitlCoinInOut(long credit);

    void SetTotalScoreUp(long credit);
    void SetTotalScoreDown(long credit);
    void SetTotalProfitlScoreUpDown(long credit);

    void SetMyCredit(long credit);

    void SetBillInfo(string data);
}


/// <summary> 【未使用】 </summary>
public class TotalBusinessRecordPresenter 
{
    IVTotalBusinessRecord view;
    public void InitParam(IVTotalBusinessRecord v)
    {
        view = v;

        OnPropertyChangeSBoxPlayerAccount();

        MachineDataManager02.Instance.RequestGetPlayerInfo((res) =>
        {

            SBoxAccount data = (SBoxAccount)res;
            int pid = SBoxModel.Instance.pid;
            List<SBoxPlayerAccount> playerAccountList = data.PlayerAccountList;
            for (int i = 0; i < playerAccountList.Count; i++)
            {
                if (playerAccountList[i].PlayerId == pid)
                {
                    SBoxModel.Instance.SboxPlayerAccount = playerAccountList[i];
                    break;
                }
            }

        }, (BagelCodeError err) =>
        {

            DebugUtils.Log(err.msg);
        });
    }

    public void Enable()
    {
        EventCenter.Instance.AddEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
    }

    public void Disable()
    {
        EventCenter.Instance.RemoveEventListener<EventData>(Observer.ON_PROPERTY_CHANGED_EVENT, OnPropertyChange);
    }



    void OnPropertyChange(EventData res = null)
    {
        string name = res.name;
        switch (name)
        {
            case "SBoxModel/SboxPlayerAccount":
                OnPropertyChangeSBoxPlayerAccount(res);
                break;
        }
    }


    void OnPropertyChangeSBoxPlayerAccount(EventData res = null)
    {
        SetDataTotalWinInfo();
        SetDataTotalCoinInOutScoreUpDown();
        SetDataBillInfo();
    }

    void SetDataTotalWinInfo()
    {
        view.SetTotalWin(SBoxModel.Instance.HistoryTotalWin);
        view.SetTotalProfitlBet(SBoxModel.Instance.HistoryTotalBet);
        view.SetTotalProfitlBet(SBoxModel.Instance.HistoryTotalProfitBet);
        view.SetMyCredit(SBoxModel.Instance.myCredit);
    }

    /// <summary>
    /// 设置账单信息
    /// </summary>
    void SetDataBillInfo()
    {
        string res = $"{SBoxModel.Instance.BillInfoTime}\n{SBoxModel.Instance.BillInfoLineMachineNumber}\n{SBoxModel.Instance.BillInfoHardwareAlgorithmVer}";
        view.SetBillInfo(res);
    }


    /// <summary>
    /// 历史总上下分、总投退币
    /// </summary>
    void SetDataTotalCoinInOutScoreUpDown()
    {
        view.SetTotalCoinIn(SBoxModel.Instance.HistoryTotalCoinInCredit);
        view.SetTotalCoinOut(SBoxModel.Instance.HistoryTotalCoinOutCredit);
        view.SetTotalProfitlCoinInOut(SBoxModel.Instance.HistoryTotalProfitCoinIn);

        view.SetTotalScoreUp(SBoxModel.Instance.HistoryTotalScoreUpCredit);
        view.SetTotalScoreDown(SBoxModel.Instance.HistoryTotalScoreDownCredit);
        view.SetTotalProfitlScoreUpDown(SBoxModel.Instance.HistoryTotalProfitScoreUp);
    }
}
