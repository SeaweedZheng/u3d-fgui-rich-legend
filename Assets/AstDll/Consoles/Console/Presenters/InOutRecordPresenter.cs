using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;



public interface IVInOutRecord
{

    event Action<int> onSelectDate;
    event Action onClickNext;
    event Action onClickPrev;

    /// <summary>  清除所有内容（包括：页尾、内容、日期） </summary>
    void ClearAll();
    void SetDateList(List<string> date);
    void SetSelectDateIndex(int index);
    void SetContent(List<TableCoinInOutRecordItem> content, int curPageIndex, int pageCount);
    int countPerPage
    {
        get;
    }
    void SetTotalCoinIn(long credit);

    void SetTotalCoinOut(long credit);

    void SetTotalProfitlCoinInOut(long credit);

    void SetTotalScoreUp(long credit);
    void SetTotalScoreDown(long credit);
    void SetTotalProfitlScoreUpDown(long credit);

}


public class InOutRecordPresenter 
{
    IVInOutRecord view;

    //const int perPageNumCoinInOut = 10;
    int fromIdxCoinInOut = 0;
    int curPageIndex = 0;
    int pageCount = 0;
    int indexDate = 0;

    const string FORMAT_DATE_SECOND = "yyyy-MM-dd HH:mm:ss";
    const string FORMAT_DATE_DAY = "yyyy-MM-dd";
    List<string> dropdownDateLst;
    List<TableCoinInOutRecordItem> resCoinInOutRecord = new List<TableCoinInOutRecordItem>();
    string TABLE_COIN_IN_OUT_RECORD => ConsoleTableName.TABLE_COIN_IN_OUT_RECORD;
    public void Enable(){}
    public void Disable(){}


    public void InitParam(IVInOutRecord v)
    {

        if (this.view != null)
        {
            this.view.onSelectDate -= OnDropdownChangedDate;
            this.view.onClickPrev -= OnClickPrevPage;
            this.view.onClickNext -= OnClickNextPage;
        }

        this.view = v;
        this.view.onSelectDate += OnDropdownChangedDate;
        this.view.onClickPrev += OnClickPrevPage;
        this.view.onClickNext += OnClickNextPage;



        view = v;


        InitCoinInOutRecordInfo();

    }

    void InitCoinInOutRecordInfo()
    {
        //string sql = $"select distinct date(created_at) from {tableName}";
        string sql = $"SELECT created_at FROM {TABLE_COIN_IN_OUT_RECORD}";

        DebugUtils.Log(sql);
        List<long> date = new List<long>();
        dropdownDateLst = new List<string>();
        SQLiteAsyncHelper.Instance.ExecuteQueryAsync(sql, null, (SqliteDataReader sdr) =>
        {
            while (sdr.Read())
            {
                long d = sdr.GetInt64(0);
                date.Add(d);
            }
            foreach (long timestamp in date)
            {
                //DebugUtil.Log($"时间搓：{timestamp}");
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                //DateTime utcDateTime = dateTimeOffset.UtcDateTime;
                //string time = utcDateTime.ToString(FORMAT_DATE_DAY);

                DateTime localDateTime = dateTimeOffset.LocalDateTime;
                string time = localDateTime.ToString(FORMAT_DATE_DAY);

                if (!dropdownDateLst.Contains(time))
                {
                    //dropdownDateLst.Add(time);
                    DebugUtils.Log($"时间搓：{timestamp} 时间 ：{time}");
                    dropdownDateLst.Insert(0, time); //最新排在最前
                }
            }


            view.ClearAll();
            if (dropdownDateLst.Count>0)
            {
                view.SetDateList(dropdownDateLst);
                view.SetSelectDateIndex(0);
                OnDropdownChangedDate(0);
            }

        });


    }


    /// <summary>
    /// 选择日期
    /// </summary>
    void OnDropdownChangedDate(int index)
    {
        indexDate = index;

        if (dropdownDateLst == null || dropdownDateLst.Count == 0 || indexDate > dropdownDateLst.Count)
            return;

        //_comboDateInOut.value

        string sql2 = $"SELECT * FROM {TABLE_COIN_IN_OUT_RECORD} WHERE DATE(DATETIME(created_at / 1000, 'unixepoch', 'localtime')) = '{dropdownDateLst[indexDate]}'"; //可以用
                                                                                                                                                                  //string sql = $"SELECT * FROM {TABLE_COIN_IN_OUT_RECORD} WHERE DATE(created_at) = '{dropdownDateLst[index]}'";  //不可以用
                                                                                                                                                                  //DebugUtils.Log(sql2);
        SQLiteAsyncHelper.Instance.ExecuteQueryAsync(sql2, null, (SqliteDataReader sdr) =>
        {

            resCoinInOutRecord = new List<TableCoinInOutRecordItem>();

            int i = 0;
            while (sdr.Read())
            {

                resCoinInOutRecord.Insert(0,
                new TableCoinInOutRecordItem()
                {
                    device_type = sdr.GetString(sdr.GetOrdinal("device_type")),
                    count = sdr.GetInt64(sdr.GetOrdinal("count")),
                    as_money = sdr.GetInt64(sdr.GetOrdinal("as_money")),
                    credit = sdr.GetInt64(sdr.GetOrdinal("credit")),
                    credit_before = sdr.GetInt64(sdr.GetOrdinal("credit_before")),
                    credit_after = sdr.GetInt64(sdr.GetOrdinal("credit_after")),
                    order_id = sdr.GetString(sdr.GetOrdinal("order_id")),
                    created_at = sdr.GetInt64(sdr.GetOrdinal("created_at")),
                    in_out = sdr.GetInt32(sdr.GetOrdinal("in_out")),
                });
            }


            curPageIndex = 0;
            pageCount = (resCoinInOutRecord.Count + (view.countPerPage - 1)) / view.countPerPage; //向上取整
            fromIdxCoinInOut = 0;

            SetUICoinInOut();
            SetInOutTotal(dropdownDateLst[index]);
        });
    }


    void SetUICoinInOut()
    {
        int lastIdx = fromIdxCoinInOut + view.countPerPage - 1;
        if (lastIdx > resCoinInOutRecord.Count - 1)
        {
            lastIdx = resCoinInOutRecord.Count - 1;
        }

        List< TableCoinInOutRecordItem > result = new List< TableCoinInOutRecordItem >();
        for (int i = 0; i <= lastIdx - fromIdxCoinInOut; i++)
        {
            TableCoinInOutRecordItem res = resCoinInOutRecord[i + fromIdxCoinInOut];

            result.Add(res);
            /*
            GComponent item = _rows[i];
            item.visible = true;
            item.GetChild("col0").asTextField.text = I18nMgr.T(res.in_out == 1 ? "In" : "Out");
            item.GetChild("col1").asTextField.text = res.credit.ToString();
            item.GetChild("col2").asTextField.text = I18nMgr.T(res.device_type);
            item.GetChild("col3").asTextField.text = res.credit_before.ToString();
            item.GetChild("col4").asTextField.text = res.credit_after.ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(res.created_at);
            DateTime localDateTime = dateTimeOffset.LocalDateTime;
            item.GetChild("col5").asTextField.text = localDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            */
        }

        view.SetContent(result, curPageIndex, pageCount);
    }


    public void OnClickNextPage()
    {
        if (fromIdxCoinInOut + view.countPerPage >= resCoinInOutRecord.Count)
            return;

        fromIdxCoinInOut += view.countPerPage;
        curPageIndex++;
        SetUICoinInOut();
    }

    public void OnClickPrevPage()
    {

        if (fromIdxCoinInOut <= 0)
            return;

        fromIdxCoinInOut -= view.countPerPage;
        curPageIndex--;
        if (fromIdxCoinInOut < 0)
        {
            curPageIndex = 0;
            fromIdxCoinInOut = 0;
        }
        SetUICoinInOut();
    }




    /// <summary>
    /// 投退汇总
    /// </summary>
    /// <param name="yyyyMMdd"></param>
    async void SetInOutTotal(string yyyyMMdd)
    {

        string dbName = ConsoleTableName.DB_NAME;
        string tableName = ConsoleTableName.TABLE_COIN_IN_OUT_RECORD;

        bool isNext = false;

        if (!SQLiteAsyncHelper.Instance.isConnect)
        {
            DebugUtils.LogError($"【Check Record】{dbName} is close");
            return;
        }


        long totalScoreUp = 0;
        string sql = $"SELECT SUM(credit) AS total_credit FROM {tableName} WHERE in_out = 1 AND device_type = 'score_up' AND DATE(DATETIME(created_at / 1000, 'unixepoch', 'localtime')) = '{yyyyMMdd}';";
        SQLiteAsyncHelper.Instance.ExecuteQueryAsync(sql, null, (dataReader) =>
        {
            while (dataReader.Read())
            {
                try
                {
                    totalScoreUp = dataReader.GetInt64(0);
                }
                catch
                {
                    totalScoreUp = 0;
                }
            }
            view.SetTotalScoreUp(totalScoreUp);
            isNext = true;
        });
        await WaitUntil(() => isNext == true);
        isNext = false;


        long totalScoreDown = 0;
        sql = $"SELECT SUM(credit) AS total_credit FROM {tableName} WHERE in_out = 0 AND device_type = 'score_down' AND DATE(DATETIME(created_at / 1000, 'unixepoch', 'localtime')) = '{yyyyMMdd}';";
        SQLiteAsyncHelper.Instance.ExecuteQueryAsync(sql, null, (dataReader) =>
        {
            while (dataReader.Read())
            {
                try
                {
                    totalScoreDown = dataReader.GetInt64(0);
                }
                catch
                {
                    totalScoreDown = 0;
                }
            }
            view.SetTotalScoreDown(totalScoreDown);
            view.SetTotalProfitlScoreUpDown(totalScoreUp - totalScoreDown);
            isNext = true;
        });
        await WaitUntil(() => isNext == true);
        isNext = false;


        long totalCoinIn = 0;
        sql = $"SELECT SUM(credit) AS total_credit FROM {tableName} WHERE in_out = 1 AND device_type != 'score_up' AND DATE(DATETIME(created_at / 1000, 'unixepoch', 'localtime')) = '{yyyyMMdd}';";
        SQLiteAsyncHelper.Instance.ExecuteQueryAsync(sql, null, (dataReader) =>
        {
            while (dataReader.Read())
            {
                try
                {
                    totalCoinIn = dataReader.GetInt64(0);
                }
                catch
                {
                    totalCoinIn = 0;
                }
            }

            view.SetTotalCoinIn(totalCoinIn);
            isNext = true;
        });
        await WaitUntil(() => isNext == true);
        isNext = false;


        long totalCoinOut = 0;
        sql = $"SELECT SUM(credit) AS total_credit FROM {tableName} WHERE in_out = 0 AND device_type != 'score_down' AND DATE(DATETIME(created_at / 1000, 'unixepoch', 'localtime')) = '{yyyyMMdd}';";
        SQLiteAsyncHelper.Instance.ExecuteQueryAsync(sql, null, (dataReader) =>
        {
            while (dataReader.Read())
            {
                try
                {
                    totalCoinOut = dataReader.GetInt64(0);
                }
                catch
                {
                    totalCoinOut = 0;
                }
            }

            view.SetTotalCoinOut(totalCoinOut);
            view.SetTotalProfitlCoinInOut(totalCoinIn - totalCoinOut);

            isNext = true;
        });
        await WaitUntil(() => isNext == true);
        isNext = false;
    }

    private static async Task WaitUntil(Func<bool> condition)
    {
        while (!condition())
        {
            await Task.Delay(10);  // 每10ms检查一次
            // 避免“编辑器-非运行”模式下，死循环导致u3d编辑器卡死
            if (Application.isEditor && !Application.isPlaying) return;
        }
    }
}
