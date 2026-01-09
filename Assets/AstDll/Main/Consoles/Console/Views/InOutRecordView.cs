using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InOutRecordView : IVInOutRecord, IVTable
{
    GComponent ui;




    GRichTextField rtxtDayInOutTotalCoinIn, rtxtDayInOutTotalCoinOut, rtxtDayInOutTotalProfitlCoinInOut,
        rtxtDayInOutTotalScoreUp, rtxtDayInOutTotalScoreDown, rtxtDayInOutTotalProfitlScoreUpDown,

        rtxtTipDayInOut;

    GComboBox combDate;

    List<GComponent> rows;

    public void InitParam(GComponent u)
    {
        ui = u;

        rtxtDayInOutTotalCoinIn = ui.GetChild("totalCoinInSummary").asCom.GetChild("value").asRichTextField;
        rtxtDayInOutTotalCoinOut = ui.GetChild("totalCoinOutSummary").asCom.GetChild("value").asRichTextField;
        rtxtDayInOutTotalProfitlCoinInOut = ui.GetChild("totalProfitCoinSummary").asCom.GetChild("value").asRichTextField;
        rtxtDayInOutTotalScoreUp = ui.GetChild("totalScoreUpSummary").asCom.GetChild("value").asRichTextField;
        rtxtDayInOutTotalScoreDown = ui.GetChild("totalScoreDownSummary").asCom.GetChild("value").asRichTextField;
        rtxtDayInOutTotalProfitlScoreUpDown = ui.GetChild("totalProfitScoreSummary").asCom.GetChild("value").asRichTextField;
        combDate = ui.GetChild("dateInOut").asCom.GetChild("value").asComboBox;
        combDate.onChanged.Clear();
        combDate.onChanged.Add(OnSelectData);


        rtxtTipDayInOut = ui.GetChild("tipDayInOut").asCom.GetChild("title").asRichTextField;
        rtxtTipDayInOut.text = string.Format(I18nMgr.T("[Note]: Only retain the most recent {0} In-Out data."), SBoxModel.Instance.coinInOutRecordMax);


        rows = new List<GComponent>();
        for (int i = 0; i <= 9; i++)
        {
            rows.Add(ui.GetChild($"row{i}").asCom);
        }

        ClearAll();
    }




    #region IVInOutRecord
    public event Action<int> onSelectDate;
    public event Action onClickNext;
    public event Action onClickPrev;

    public int countPerPage => 10;

    void OnSelectData()
    {
        int index = combDate.selectedIndex;
        onSelectDate?.Invoke(index);
    }

    /// <summary>
    /// 清除所有内容（包括：页尾、内容、日期）
    /// </summary>
    public void ClearAll()
    {
        combDate.items = new string[] { };
        combDate.values = new string[] { };
        combDate.selectedIndex = 0;

        foreach (var item in rows)
        {
            item.visible = false;
        }
        _curPageIndex = 1;
        _pageCount = 1;
        // SetNavBottomTitle(1,1);
    }

    public void SetTotalCoinIn(long credit) => rtxtDayInOutTotalCoinIn.text = $"{credit}";
    public void SetTotalCoinOut(long credit) => rtxtDayInOutTotalCoinOut.text = $"{credit}";
    public void SetTotalProfitlCoinInOut(long credit) => rtxtDayInOutTotalProfitlCoinInOut.text = $"{credit}";
    public void SetTotalScoreUp(long credit) => rtxtDayInOutTotalScoreUp.text = $"{credit}";
    public void SetTotalScoreDown(long credit) => rtxtDayInOutTotalScoreDown.text = $"{credit}";
    public void SetTotalProfitlScoreUpDown(long credit) => rtxtDayInOutTotalProfitlScoreUpDown.text = $"{credit}";



    public void SetDateList(List<string> date)
    {
        combDate.items = date.ToArray();
        combDate.values = date.ToArray();
        combDate.selectedIndex = 0;
    }
    public void SetSelectDateIndex(int index)
    {
        combDate.selectedIndex = index;
    }
    public void SetContent(List<TableCoinInOutRecordItem> content, int curPageIndex, int pageCount)
    {
        this._curPageIndex = curPageIndex;
        this._pageCount = pageCount;
        foreach (var item in rows)
        {
            item.visible = false;
        }

        for (int i = 0; i < content.Count; i++)
        {
            TableCoinInOutRecordItem res = content[i];
            GComponent comp = rows[i];
            comp.visible = true;

            comp.GetChild("col0").asTextField.text = I18nMgr.T(res.in_out == 1 ? "In" : "Out");
            comp.GetChild("col1").asTextField.text = I18nMgr.T(res.device_type);
            comp.GetChild("col2").asTextField.text = res.credit.ToString();
            comp.GetChild("col3").asTextField.text = res.credit_before.ToString();
            comp.GetChild("col4").asTextField.text = res.credit_after.ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(res.created_at);
            DateTime localDateTime = dateTimeOffset.LocalDateTime;
            comp.GetChild("col5").asTextField.text = localDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }


        onChangeNavBottomTitle?.Invoke(curPageIndex, pageCount);
        //SetNavBottomTitle(curPageIndex, pageCount);
    }


    #endregion






    int _curPageIndex, _pageCount;

    #region IVTable

    public int curPageIndex => _curPageIndex;
    public int pageCount => _pageCount;
    public void OnClickNext() => onClickNext?.Invoke();

    public void OnClickPrev() => onClickPrev?.Invoke();

    public event Action<int, int> onChangeNavBottomTitle;
    #endregion
}
