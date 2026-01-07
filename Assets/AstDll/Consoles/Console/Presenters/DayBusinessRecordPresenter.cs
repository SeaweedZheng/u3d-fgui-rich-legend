using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IVDayBusinessRecord
{

    event Action<int> onSelectDate;

    /// <summary>  清除所有内容（包括：页尾、内容、日期） </summary>
    void ClearAll();
    void SetDateList(List<string> date);
    void SetSelectDateIndex(int index);


    void SetTotalBet(long credit);
    void SetTotalWin(long credit);
    void SetTotalProfitlBet(long credit);

    void SetTotalCoinIn(long credit);
    void SetTotalCoinOut(long credit);
    void SetTotalProfitlCoinInOut(long credit);

    void SetTotalScoreUp(long credit);
    void SetTotalScoreDown(long credit);
    void SetTotalProfitlScoreUpDown(long credit);

}



public class DayBusinessRecordPresenter 
{
    IVDayBusinessRecord view;

    List<string> dateList;
    const string FORMAT_DATE_DAY = "yyyy-MM-dd";

    int idxDate;

    public void InitParam(IVDayBusinessRecord v)
    {

        if(view != null)
        {
            view.onSelectDate -= OnSelectDate;
        }

        view = v;
        view.onSelectDate += OnSelectDate;

        InitBusinessDayRecordInfo();
    }

    void InitBusinessDayRecordInfo()
    {
        dateList = new List<string>();

        string sql = $"SELECT created_at FROM {ConsoleTableName.TABLE_BUSINESS_DAY_RECORD}";
        DebugUtils.Log(sql);
        List<long> date = new List<long>();

        SQLiteAsyncHelper.Instance.ExecuteQueryAsync(sql, null, (SqliteDataReader sdr) =>
        {
            while (sdr.Read())
            {
                long d = sdr.GetInt64(0);
                date.Add(d);
            }
            foreach (long timestamp in date)
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                DateTime localDateTime = dateTimeOffset.LocalDateTime;
                string time = localDateTime.ToString(FORMAT_DATE_DAY);

                if (!dateList.Contains(time))
                {
                    DebugUtils.Log($"时间搓：{timestamp} 时间 ：{time}");
                    dateList.Insert(0, time); //最新排在最前
                }
            }

            //combo.items = new string[] { "选项1", "选项2", "选项3" }; // 设置选项文本
            //combo.values = new string[] { "value1", "value2", "value3" }; // 可选：设置选项值

            view.ClearAll();
            if (dateList.Count >0)
            {
                idxDate = 0;
                view.SetDateList(dateList);
                view.SetSelectDateIndex(idxDate);
                CheckBusinessDayRecord(dateList[idxDate]);
            }

        });

    }

    void OnSelectDate(int index)
    {
        idxDate = index;
        CheckBusinessDayRecord(dateList[idxDate]);
    }


    void CheckBusinessDayRecord(string yyyyMMdd)
    {
        string dbName = ConsoleTableName.DB_NAME;
        string tableName = ConsoleTableName.TABLE_BUSINESS_DAY_RECORD;

        if (!SQLiteAsyncHelper.Instance.isConnect)
        {
            DebugUtils.LogError($"【Check Record】{dbName} is close");
            return;
        }

        string sql = $"SELECT * FROM {tableName} WHERE DATE(DATETIME(created_at / 1000, 'unixepoch', 'localtime')) = '{yyyyMMdd}';";
        SQLiteAsyncHelper.Instance.ExecuteQueryAsync(sql, null, (sdr) =>
        {

            TableBussinessDayRecordItem bussinessDayRecord = new TableBussinessDayRecordItem();
            while (sdr.Read())
            {
                bussinessDayRecord = new TableBussinessDayRecordItem()
                {
                    credit_before = sdr.GetInt32(sdr.GetOrdinal("credit_before")),
                    credit_after = sdr.GetInt32(sdr.GetOrdinal("credit_after")),
                    total_bet_credit = sdr.GetInt32(sdr.GetOrdinal("total_bet_credit")),
                    total_win_credit = sdr.GetInt32(sdr.GetOrdinal("total_win_credit")),
                    total_coin_in_credit = sdr.GetInt32(sdr.GetOrdinal("total_coin_in_credit")),
                    total_coin_out_credit = sdr.GetInt32(sdr.GetOrdinal("total_coin_out_credit")),
                    total_score_up_credit = sdr.GetInt32(sdr.GetOrdinal("total_score_up_credit")),
                    total_score_down_credit = sdr.GetInt32(sdr.GetOrdinal("total_score_down_credit")),
                    total_bill_in_credit = sdr.GetInt32(sdr.GetOrdinal("total_bill_in_credit")),
                    total_bill_in_as_money = sdr.GetInt32(sdr.GetOrdinal("total_bill_in_as_money")),
                    total_printer_out_credit = sdr.GetInt32(sdr.GetOrdinal("total_printer_out_credit")),
                    total_printer_out_as_money = sdr.GetInt32(sdr.GetOrdinal("total_printer_out_as_money")),
                    created_at = sdr.GetInt32(sdr.GetOrdinal("created_at")),
                };
            }

            view.SetTotalBet(bussinessDayRecord.total_bet_credit);

            view.SetTotalWin(bussinessDayRecord.total_win_credit);

            view.SetTotalProfitlBet(bussinessDayRecord.total_bet_credit - bussinessDayRecord.total_win_credit);

            view.SetTotalScoreDown(bussinessDayRecord.total_score_down_credit);

            view.SetTotalScoreUp(bussinessDayRecord.total_score_up_credit);

            view.SetTotalProfitlScoreUpDown(bussinessDayRecord.total_score_up_credit - bussinessDayRecord.total_score_down_credit);

            view.SetTotalCoinOut(bussinessDayRecord.total_coin_out_credit);

            view.SetTotalCoinIn(bussinessDayRecord.total_coin_in_credit);

            view.SetTotalProfitlCoinInOut(bussinessDayRecord.total_coin_in_credit - bussinessDayRecord.total_coin_out_credit);


        });
    }

}
