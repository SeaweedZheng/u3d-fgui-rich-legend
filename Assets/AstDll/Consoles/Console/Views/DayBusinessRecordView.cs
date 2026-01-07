using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DayBusinessRecordView : IVDayBusinessRecord
{

    GComponent ui;


    GRichTextField rtxtTotalBet, rtxtTotalWin, rtxtTotalProfitBet,

    rtxtTotalCoinIn, rtxtTotalCoinOut, rtxtTotalProfitCoinInOut,

    rtxtTotalScoreUp, rtxtTotalScoreDown, rtxtTotalProfitlScoreUpDown,

    rtxtTipDayBusniess;

    GComboBox comboDateBusinessDayRecord;



    public void InitParam(GComponent u)
    {
        ui = u;

        rtxtTotalBet = ui.GetChild("totalBetDaly").asCom.GetChild("value").asRichTextField;
        rtxtTotalWin = ui.GetChild("totalWinDaly").asCom.GetChild("value").asRichTextField;
        rtxtTotalProfitBet = ui.GetChild("totalProfitBetDaly").asCom.GetChild("value").asRichTextField;
        rtxtTotalCoinIn = ui.GetChild("totalCoinInDaly").asCom.GetChild("value").asRichTextField;
        rtxtTotalCoinOut = ui.GetChild("totalCoinOutDaly").asCom.GetChild("value").asRichTextField;
        rtxtTotalProfitCoinInOut = ui.GetChild("totalProfitCoinInOutDaly").asCom.GetChild("value").asRichTextField;
        rtxtTotalScoreUp = ui.GetChild("totalScoreUpDaly").asCom.GetChild("value").asRichTextField;
        rtxtTotalScoreDown = ui.GetChild("totalScoreDownDaly").asCom.GetChild("value").asRichTextField;
        rtxtTotalProfitlScoreUpDown = ui.GetChild("totalProfitScoreUpDownDaly").asCom.GetChild("value").asRichTextField;
        comboDateBusinessDayRecord = ui.GetChild("dateDalyBusiness").asCom.GetChild("value").asComboBox;


        comboDateBusinessDayRecord.onChanged.Clear();
        comboDateBusinessDayRecord.onChanged.Add(OnDropdownChangedDate);

        rtxtTipDayBusniess = ui.GetChild("tipDayBusniess").asCom.GetChild("title").asRichTextField;
        rtxtTipDayBusniess.text = string.Format(I18nMgr.T("[Note]: Only retain the most recent {0} business day data."), SBoxModel.Instance.businiessDayRecordMax);


        ClearAll();
    }


    void OnDropdownChangedDate() => onSelectDate?.Invoke(comboDateBusinessDayRecord.selectedIndex);
    

    public event Action<int> onSelectDate;

    /// <summary>  清除所有内容（包括：页尾、内容、日期） </summary>
    public void ClearAll()
    {
        rtxtTotalBet.text = "";
        rtxtTotalWin.text = "";
        rtxtTotalProfitBet.text = "";

        rtxtTotalCoinIn.text = "";
        rtxtTotalCoinOut.text = "";
        rtxtTotalProfitCoinInOut.text = "";

        rtxtTotalScoreUp.text = "";
        rtxtTotalScoreDown.text = "";
        rtxtTotalProfitlScoreUpDown.text = "";

    }
    public void SetDateList(List<string> date)
    {
        comboDateBusinessDayRecord.items = date.ToArray();
        comboDateBusinessDayRecord.values = date.ToArray();
    }

    public void SetSelectDateIndex(int index)
    {
        comboDateBusinessDayRecord.selectedIndex = 0;
    }


    public void SetTotalBet(long credit)=> rtxtTotalBet.text = $"{credit}";
    public void SetTotalWin(long credit) => rtxtTotalWin.text = $"{credit}";
    public void SetTotalProfitlBet(long credit) => rtxtTotalProfitBet.text = $"{credit}";
    public void SetTotalCoinIn(long credit) => rtxtTotalCoinIn.text = $"{credit}";
    public void SetTotalCoinOut(long credit) => rtxtTotalCoinOut.text = $"{credit}";
    public void SetTotalProfitlCoinInOut(long credit) => rtxtTotalProfitCoinInOut.text = $"{credit}";
    public void SetTotalScoreUp(long credit) => rtxtTotalScoreUp.text = $"{credit}";
    public void SetTotalScoreDown(long credit) => rtxtTotalScoreDown.text = $"{credit}";
    public void SetTotalProfitlScoreUpDown(long credit) => rtxtTotalProfitlScoreUpDown.text = $"{credit}";



}
