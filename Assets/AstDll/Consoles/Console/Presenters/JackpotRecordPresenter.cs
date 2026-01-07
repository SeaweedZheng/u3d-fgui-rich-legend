using FairyGUI;
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IVJackpotRecord
{
    event Action<int> onSelectDate;
    event Action onClickNext;
    event Action onClickPrev;

    int countPerPage
    {
        get;
    }

    /// <summary>  清除所有内容（包括：页尾、内容、日期） </summary>
    void ClearAll();
    void SetDateList(List<string> date);
    void SetSelectDateIndex(int index);
    void SetContent(List<TableJackpotRecordItem> content, int curPageIndex, int pageCount);
}
public class JackpotRecordPresenter {

    IVJackpotRecord view;
    public void InitParam(IVJackpotRecord v)
    {
        if (this.view!=null)
        {
            this.view.onSelectDate -= OnDropdownChangedJackpotRecord;
            this.view.onClickPrev -= OnClickPrevPage;
            this.view.onClickNext -= OnClickNextPage;
        }

        this.view = v;
        this.view.onSelectDate += OnDropdownChangedJackpotRecord;
        this.view.onClickPrev += OnClickPrevPage;
        this.view.onClickNext += OnClickNextPage;

        InitJackpotRecordInfo();
    }

    List<string> dropdownDateLstJackpotRecord;
    List<TableJackpotRecordItem> resJackpotRecord;
    const string FORMAT_DATE_DAY = "yyyy-MM-dd";
    int fromIdxJackpotRecord = 0;
    int curPageIndex = 0;
    int pageCount = 1;

    void InitJackpotRecordInfo()
    {
        string sql = $"SELECT created_at FROM {ConsoleTableName.TABLE_JACKPOT_RECORD}";

        List<long> date = new List<long>();
        dropdownDateLstJackpotRecord = new List<string>();
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
                // DateTime utcDateTime = dateTimeOffset.UtcDateTime;
                // string time = utcDateTime.ToString(FORMAT_DATE_DAY);

                DateTime localDateTime = dateTimeOffset.LocalDateTime;
                string time = localDateTime.ToString(FORMAT_DATE_DAY);
                if (!dropdownDateLstJackpotRecord.Contains(time))
                {
                    //dropdownDateLstJackpotRecord.Add(time);
                    dropdownDateLstJackpotRecord.Insert(0, time);
                    //DebugUtil.Log($"时间搓：{timestamp} 时间 ：{time}");
                }
            }

            view.ClearAll();
            if (dropdownDateLstJackpotRecord.Count > 0)
            {
                view.SetDateList(dropdownDateLstJackpotRecord);
                view.SetSelectDateIndex(0);
                OnDropdownChangedJackpotRecord(0);
            }
        });
    }


    void OnDropdownChangedJackpotRecord(int index)
    {
        string sql2 = $"SELECT * FROM {ConsoleTableName.TABLE_JACKPOT_RECORD} WHERE DATE(DATETIME(created_at / 1000, 'unixepoch', 'localtime')) = '{dropdownDateLstJackpotRecord[index]}'"; //可以用
        
        SQLiteAsyncHelper.Instance.ExecuteQueryAsync(sql2, null, (SqliteDataReader sdr) =>
        {
            resJackpotRecord = new List<TableJackpotRecordItem>();
            while (sdr.Read())
            {
                resJackpotRecord.Insert(0,
                new TableJackpotRecordItem()
                {
                    user_id = sdr.GetString(sdr.GetOrdinal("user_id")),
                    game_id = sdr.GetInt64(sdr.GetOrdinal("game_id")),
                    game_uid = sdr.GetString(sdr.GetOrdinal("game_uid")),
                    jp_name = sdr.GetString(sdr.GetOrdinal("jp_name")),
                    jp_level = sdr.GetInt64(sdr.GetOrdinal("jp_level")),
                    win_credit = sdr.GetInt64(sdr.GetOrdinal("win_credit")),
                    credit_before = sdr.GetInt64(sdr.GetOrdinal("credit_before")),
                    credit_after = sdr.GetInt64(sdr.GetOrdinal("credit_after")),
                    created_at = sdr.GetInt64(sdr.GetOrdinal("created_at")),
                });
            }
            pageCount = (resJackpotRecord.Count + (view.countPerPage - 1)) / view.countPerPage; //向上取整
            fromIdxJackpotRecord = 0;
            curPageIndex = 0;
            SetUIContent();
        });
    }


    void SetUIContent()
    {
        int lastIdx = fromIdxJackpotRecord + view.countPerPage - 1;
        if (lastIdx > resJackpotRecord.Count - 1)
        {
            lastIdx = resJackpotRecord.Count - 1;
        }

        List< TableJackpotRecordItem > result = new List< TableJackpotRecordItem >();
        
        for (int i = 0; i <= lastIdx - fromIdxJackpotRecord; i++)
        {

            TableJackpotRecordItem res = resJackpotRecord[i + fromIdxJackpotRecord];
            result.Add(res);
            /*
            item.Find("JP Name/Text").GetComponent<TextMeshProUGUI>().text = I18nMgr.T(res.jp_name);
            item.Find("Game Number/Text").GetComponent<TextMeshProUGUI>().text = res.game_uid;
            item.Find("Credit/Text").GetComponent<TextMeshProUGUI>().text = res.win_credit.ToString();  // res.win_credit.ToString("N2");  // 游戏彩金是带有小数的
            item.Find("Before Credit/Text").GetComponent<TextMeshProUGUI>().text = res.credit_before.ToString();
            item.Find("After Credit/Text").GetComponent<TextMeshProUGUI>().text = res.credit_after.ToString();

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(res.created_at);
            //DateTime utcDateTime = dateTimeOffset.UtcDateTime;
            DateTime localDateTime = dateTimeOffset.LocalDateTime;
            item.Find("Date/Text").GetComponent<TextMeshProUGUI>().text = localDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            */
        }

        view.SetContent(result, curPageIndex, pageCount);
    }

    private void OnClickNextPage()
    {
        if (fromIdxJackpotRecord + view.countPerPage >= resJackpotRecord.Count)
            return;

        fromIdxJackpotRecord += view.countPerPage;
        curPageIndex++;
        SetUIContent();
    }
    private void OnClickPrevPage()
    {
        if (fromIdxJackpotRecord <= 0)
            return;

        fromIdxJackpotRecord -= view.countPerPage;
        curPageIndex--;
        if (fromIdxJackpotRecord < 0)
        {
            curPageIndex = 0;
            fromIdxJackpotRecord = 0;
        }
        SetUIContent();
    }
}
