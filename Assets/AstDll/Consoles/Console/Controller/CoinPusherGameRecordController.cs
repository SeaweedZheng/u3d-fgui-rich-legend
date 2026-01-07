using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IVCoinPusherGameRecordRecord
{
    event Action<int> onSelectDate;
    event Action onClickNext;
    event Action onClickPrev;


    /// <summary>
    /// 清除所有内容（包括：页尾、内容、日期）
    /// </summary>
    void ClearAll();
    void SetDateList(List<string> date);
    void SetSelectDateIndex(int index);
    void SetContent(TableCoinPusherGameRecordItem content, int curPageIndex, int pageCount);
}
public class CoinPusherGameRecordController
{

    IVCoinPusherGameRecordRecord _view;

    List<string> dropdownDateLstGameRecord;
    int curPageIndex = 0;
    int pageCount = 1;
    const string FORMAT_DATE_DAY = "yyyy-MM-dd";
    TableCoinPusherGameRecordItem curGame;

    public void InitParam(IVCoinPusherGameRecordRecord view)
    {
        if (this._view != null)
        {
            this._view.onSelectDate -= OnDropdownChangedGameRecord;
            this._view.onClickPrev -= OnClickPrevPage;
            this._view.onClickNext -= OnClickNextPage;
        }

        this._view = view;
        this._view.onSelectDate += OnDropdownChangedGameRecord;
        this._view.onClickPrev += OnClickPrevPage;
        this._view.onClickNext += OnClickNextPage;


        InitGameRecordInfo();
    }

    void InitGameRecordInfo()
    {


        string sql = $"SELECT created_at FROM {ConsoleTableName.TABLE_COIN_PUSHER_GAME_RECORD}";

        List<long> date = new List<long>();
        dropdownDateLstGameRecord = new List<string>();
        SQLiteHelper.Instance.ExecuteQueryAfterOpenDB(sql, (SqliteDataReader sdr) =>
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

                if (!dropdownDateLstGameRecord.Contains(time))
                {
                    //dropdownDateLstGameRecord.Add(time);
                    dropdownDateLstGameRecord.Insert(0, time);
                    //DebugUtil.Log($"时间搓：{timestamp} 时间 ：{time}");
                }
            }
            dropdownDateLstGameRecord.Insert(0, I18nMgr.T("All"));

            _view.ClearAll();
            if (dropdownDateLstGameRecord.Count > 0)
            {
                idxDate = 0;
                _view.SetDateList(dropdownDateLstGameRecord);
                _view.SetSelectDateIndex(0);
                OnDropdownChangedGameRecord(0);
            }
        });

    }

    List<TableCoinPusherGameRecordItem> resGameRecord;


    int idxDate = 0;
    void OnDropdownChangedGameRecord(int index)
    {
        idxDate = index;

        if (index == 0)  // ALL
        {
            SQLiteHelper.Instance.OpenDB(ConsoleTableName.DB_NAME, (connect) =>
            {
                string sql = $"SELECT COUNT(*) FROM sqlite_master WHERE type ='table' and name='{ConsoleTableName.TABLE_COIN_PUSHER_GAME_RECORD}';";
                //创建命令
                var command = connect.CreateCommand();
                command.CommandText = sql;
                int count = Convert.ToInt32(command.ExecuteScalar());

                //如果结果为1则表示存在该表格
                bool isExists = count == 1;

                if (isExists)
                {
                    sql = $"SELECT COUNT(*) FROM {ConsoleTableName.TABLE_COIN_PUSHER_GAME_RECORD}";
                    DebugUtils.Log(sql);
                    using (var command1 = new SqliteCommand(sql, connect))
                    {
                        pageCount = (int)command1.ExecuteScalar();
                        curPageIndex = 0;

                        if (pageCount > 0)
                            GetGameData(0);
                    }
                }
                else
                {
                    DebugUtils.Log($"不存在表 {ConsoleTableName.TABLE_COIN_PUSHER_GAME_RECORD} ");
                }
            });
        }
        else  // 日期
        {

            string yyyy_MM_dd = dropdownDateLstGameRecord[(int)index];

            string sql2 = $"SELECT * FROM {ConsoleTableName.TABLE_COIN_PUSHER_GAME_RECORD} WHERE DATE(DATETIME(created_at / 1000, 'unixepoch', 'localtime')) = '{yyyy_MM_dd}'"; //可以用

            List<string> result = new List<string>();
            SQLiteHelper.Instance.ExecuteQueryAfterOpenDB(sql2, (SqliteDataReader sdr) =>
            {
                resGameRecord = new List<TableCoinPusherGameRecordItem>();
                while (sdr.Read())
                {
                    resGameRecord.Insert(0,
                    new TableCoinPusherGameRecordItem()
                    {
                        scene = sdr.GetString(sdr.GetOrdinal("scene")),
                        game_id = sdr.GetInt32(sdr.GetOrdinal("game_id")),
                        game_type = sdr.GetString(sdr.GetOrdinal("game_type")),
                        total_bet = sdr.GetInt64(sdr.GetOrdinal("total_bet")),
                        credit_per_coin_in = sdr.GetInt64(sdr.GetOrdinal("credit_per_coin_in")),
                        lines_played = sdr.GetInt32(sdr.GetOrdinal("lines_played")),
                        credit_before = sdr.GetInt64(sdr.GetOrdinal("credit_before")),
                        credit_after = sdr.GetInt64(sdr.GetOrdinal("credit_after")),
                        base_game_win_coins = sdr.GetInt64(sdr.GetOrdinal("base_game_win_coins")),
                        jackpot_win_coins = sdr.GetInt64(sdr.GetOrdinal("jackpot_win_coins")),
                        jackpot_type = sdr.GetString(sdr.GetOrdinal("jackpot_type")),
                        bonus_game_win_coins = sdr.GetInt64(sdr.GetOrdinal("bonus_game_win_credit")),
                        bonus_game_type = sdr.GetString(sdr.GetOrdinal("bonus_game_type")),
                        game_uid = sdr.GetString(sdr.GetOrdinal("game_uid")),
                        created_at = sdr.GetInt64(sdr.GetOrdinal("created_at")),
                    });
                }

                pageCount = resGameRecord.Count;
                curPageIndex = 0;

                GetGameRecordWhen(0);
            });

        }
    }

    void GetGameRecordWhen(int index)
    {
        _view.SetContent(resGameRecord[index], curPageIndex, pageCount);
    }



    void GetGameData(long index)
    {
        string sql = $"SELECT * FROM {ConsoleTableName.TABLE_COIN_PUSHER_GAME_RECORD} LIMIT 1 OFFSET {pageCount - 1 - index}";

        SQLiteHelper.Instance.ExecuteQueryAfterOpenDB(sql, (SqliteDataReader sdr) =>
        {

            while (sdr.Read())
            {
                curGame = new TableCoinPusherGameRecordItem()
                {
                    scene = sdr.GetString(sdr.GetOrdinal("scene")),
                    game_id = sdr.GetInt32(sdr.GetOrdinal("game_id")),
                    game_type = sdr.GetString(sdr.GetOrdinal("game_type")),
                    total_bet = sdr.GetInt64(sdr.GetOrdinal("total_bet")),
                    credit_per_coin_in = sdr.GetInt64(sdr.GetOrdinal("credit_per_coin_in")),
                    lines_played = sdr.GetInt32(sdr.GetOrdinal("lines_played")),
                    credit_before = sdr.GetInt64(sdr.GetOrdinal("credit_before")),
                    credit_after = sdr.GetInt64(sdr.GetOrdinal("credit_after")),
                    base_game_win_coins = sdr.GetInt64(sdr.GetOrdinal("base_game_win_coins")),
                    jackpot_win_coins = sdr.GetInt64(sdr.GetOrdinal("jackpot_win_coins")),
                    jackpot_type = sdr.GetString(sdr.GetOrdinal("jackpot_type")),
                    bonus_game_win_coins = sdr.GetInt64(sdr.GetOrdinal("bonus_game_win_credit")),
                    bonus_game_type = sdr.GetString(sdr.GetOrdinal("bonus_game_type")),
                    game_uid = sdr.GetString(sdr.GetOrdinal("game_uid")),
                    created_at = sdr.GetInt64(sdr.GetOrdinal("created_at")),
                };
            }

            _view.SetContent(curGame, curPageIndex, pageCount);
        });
    }


    private void OnClickNextPage()
    {
        if (curPageIndex + 1 >= pageCount)
            return;

        curPageIndex++;


        if (idxDate == 0)
            GetGameData(curPageIndex); //全部
        else
            GetGameRecordWhen(curPageIndex); //日期
    }
    private void OnClickPrevPage()
    {
        if (curPageIndex <= 0)
            return;

        curPageIndex--;

        if (idxDate == 0)
            GetGameData(curPageIndex); //全部
        else
            GetGameRecordWhen(curPageIndex); //日期
    }
}
