using System;

public interface IVMultipleGameRecord
{
    event Action<SelectGameRecordFilterInfo, SelectGameRecordPageInfo> onSelectGameRecord;  //游戏选择类型，分页数据

    event Action onClickNext;
    event Action onClickPrev;

    /// <summary>
    /// 清除所有内容（包括：页尾、内容、日期）
    /// </summary>
    void ClearAll();

    /// <summary> 设置为默认的游戏内容选择 </summary>
    SelectGameRecordPageInfo SetDefaultSelect();

    /// <summary> 当前已选的过滤条件  </summary>
    SelectGameRecordFilterInfo curSelectFilterInfo { get; }


    /// <summary> 总的可选过滤条件  </summary>
    void SetTotalGameFilterOptions(TotalGameFilterOptions Filter);
    //void SetSelectDateIndex(int index);
    void SetContent(SelectGameRecordPageResult content);
}

public class MultipleGameRecordPresenter
{

    IVMultipleGameRecord _view;

    const string FORMAT_DATE_DAY = "yyyy-MM-dd";

    SelectGameRecordPageInfo curPageInfo;
    int _totalPageCont = 0;

    public void InitParam(IVMultipleGameRecord view)
    {
        if (this._view != null)
        {
            this._view.onSelectGameRecord -= OnSelectGameRecord;
            this._view.onClickPrev -= OnClickPrevPage;
            this._view.onClickNext -= OnClickNextPage;
        }

        this._view = view;
        this._view.onSelectGameRecord += OnSelectGameRecord;
        this._view.onClickPrev += OnClickPrevPage;
        this._view.onClickNext += OnClickNextPage;

        InitView();
    }


    void OnSelectGameRecord(SelectGameRecordFilterInfo select, SelectGameRecordPageInfo pageInfo)
    {
        curPageInfo = pageInfo;
        _GetGameRecord(select, pageInfo);
    }

    void _GetGameRecord(SelectGameRecordFilterInfo select, SelectGameRecordPageInfo pageInfo)
    {
        GameRecordFilterManager.Instance.GetGameRecord(select, pageInfo, (result) =>
        {
            _totalPageCont = result.totalPageCount;
            _view.SetContent(result);
        });
    }


    void InitView()
    {
        GameRecordFilterManager.Instance.GetAllFilterOptions((totalGameFilterOption) =>
        {
            _view.SetTotalGameFilterOptions(totalGameFilterOption);
            _view.ClearAll();

            curPageInfo = _view.SetDefaultSelect();

            _GetGameRecord(_view.curSelectFilterInfo, curPageInfo);
        });
    }


  
    private void OnClickNextPage()
    {
        if (curPageInfo.selectNumberPage + 1 > _totalPageCont)
            return;

        curPageInfo.selectNumberPage++;


        _GetGameRecord(_view.curSelectFilterInfo, curPageInfo);
    }

    private void OnClickPrevPage()
    {
        if (curPageInfo.selectNumberPage <= 1)
            return;

        curPageInfo.selectNumberPage--;

        _GetGameRecord(_view.curSelectFilterInfo, curPageInfo);
    }
}

