using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IVDemo
{

    event Action onClickLineIdMachineId;


    /// <summary>  清除所有内容（包括：页尾、内容、日期） </summary>
    void ClearAll();
    void SetDateList(List<string> date);
    void SetSelectDateIndex(int index);
    void SetContent(List<TableJackpotRecordItem> content, int curPageIndex, int pageCount);
}
public class DemoPresenter
{
    IVDemo view;

    public void InitParam(IVDemo v)
    {
        view = v;
    }
}
