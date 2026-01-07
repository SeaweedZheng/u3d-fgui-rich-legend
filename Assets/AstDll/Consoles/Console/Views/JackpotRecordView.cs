using FairyGUI;
using System;
using System.Collections.Generic;


public class JackpotRecordView : IVJackpotRecord, IVTable
{
    GComponent goContent;
    GComboBox combDate;
    List<GComponent> rows = new List<GComponent>();


    int _curPageIndex, _pageCount;


    public void InitParam(GComponent content)
    {
        goContent = content;
        //内容
        rows.Clear();
        for (int i =0; i< countPerPage; i++)
        {
            rows.Add(goContent.GetChild("row"+i).asCom);
        }

        //日期
        combDate = goContent.GetChild("date").asCom.GetChild("value").asComboBox;
        combDate.onChanged.Clear();
        combDate.onChanged.Add(OnSelectData);

        ClearAll();
    }

    public event Action<int> onSelectDate;
    public event Action onClickNext;
    public event Action onClickPrev;

    public int countPerPage => 13;

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

    /*
   void SetNavBottomTitle(int curPageIndex, int pageCount)
    {
        string title = string.Format(I18nMgr.T("Jackpot History, Page {0} of {1}"), curPageIndex, pageCount);
        goNavBottom.GetChild("title").asTextField.text = title;
    }*/



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
    public void SetContent(List<TableJackpotRecordItem> content, int curPageIndex, int pageCount)
    {
        this._curPageIndex = curPageIndex;
        this._pageCount = pageCount;
        foreach (var item in rows)
        {
            item.visible = false;
        }

        for  (int i = 0; i< content.Count; i++)
        {
            TableJackpotRecordItem res = content[i];
            GComponent comp = rows[i];
            comp.visible = true;

            comp.GetChild("col0").asTextField.text = I18nMgr.T(res.jp_name);
            comp.GetChild("col1").asTextField.text = res.game_uid;
            comp.GetChild("col2").asTextField.text = res.win_credit.ToString();
            comp.GetChild("col3").asTextField.text = res.credit_before.ToString();
            comp.GetChild("col4").asTextField.text = res.credit_after.ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(res.created_at);
            DateTime localDateTime = dateTimeOffset.LocalDateTime;
            comp.GetChild("col5").asTextField.text = localDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

       onChangeNavBottomTitle?.Invoke(curPageIndex, pageCount);
    }
    void OnSelectData()
    {
        int index = combDate.selectedIndex;
        onSelectDate?.Invoke(index);
    }


    #region IVTable
    public int curPageIndex => _curPageIndex;
    public int pageCount => _pageCount;
    public void OnClickNext()=> onClickNext?.Invoke();

    public void OnClickPrev()=> onClickPrev?.Invoke();

    public event Action<int, int> onChangeNavBottomTitle;
    #endregion
}
