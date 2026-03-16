using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameRecordFilterManager : Singleton<GameRecordFilterManager>
{

    TotalGameFilterOptions totalGameRecordFilter;
    public void GetAllFilterOptions(Action<TotalGameFilterOptions> onFinish)
    {

        TotalGameFilterOptions filterValues = new TotalGameFilterOptions();

        int count = 6;
        Action callback = () =>
        {
            if (--count ==0)
            {
                totalGameRecordFilter = filterValues;
                onFinish?.Invoke(filterValues);
            }
        };

        _GetDistinctLongValues("game_id", (val) =>
       {
           filterValues.gameIds = val;
           callback?.Invoke();
       });
        _GetDistinctStringValues("game_type", (val) =>
        {
            filterValues.gameTypes = val;
            callback?.Invoke();
        });
        _GetDistinctStringValues("turn_type", (val) =>
        {
            filterValues.turnTypes = val;
            callback?.Invoke();
        });
        _GetDistinctStringValues("hit_jackpot_type", (val) =>
       {
           filterValues.hitJackpotTypes = val;
           callback?.Invoke();
       });
        _GetDistinctStringValues("hit_bonus_type", (val) =>
        {
            filterValues.hitBonusTypes = val;
            callback?.Invoke();
        });

        _GetDistinctFullDates((val) =>
        {
            filterValues.fullDates = val;
            callback?.Invoke();
        });
    }





    /// <summary>
    /// 通用方法：查询指定字符串字段的所有唯一值（过滤NULL和空字符串）
    /// </summary>
    private void _GetDistinctStringValues(string fieldName, Action<List<string> >onFinishCallback)
    {
        List<string> values = new List<string>();
        // 去重 + 过滤NULL/空字符串 + 排序
        string sql = $"SELECT DISTINCT {fieldName} FROM {ConsoleTableName.TABLE_GAME_RECORD} WHERE {fieldName} IS NOT NULL AND {fieldName} != '' ORDER BY {fieldName}";

        SQLiteHelper.Instance.ExecuteQueryAfterOpenDB(sql, (SqliteDataReader sdr) =>
        {
            while (sdr.Read())
            {
                string val = sdr.GetString(0).Trim(); // 去除首尾空格，避免无效重复
                if (!string.IsNullOrEmpty(val))
                {
                    values.Insert(0,val);
                }
            }
            onFinishCallback?.Invoke(values);
        });

        /*
        using (IDbCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = sql;
            using (IDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string val = reader.GetString(0).Trim(); // 去除首尾空格，避免无效重复
                    if (!string.IsNullOrEmpty(val))
                    {
                        values.Add(val);
                    }
                }
            }
        }
        return values;
        */
    }



    private void  _GetDistinctLongValues(string fieldName, Action<List<long>> onFinishCallback)
    {
        List<long> values = new List<long>();
        // 去重 + 过滤NULL + 排序
        string sql = $"SELECT DISTINCT {fieldName} FROM  {ConsoleTableName.TABLE_GAME_RECORD}  WHERE {fieldName} IS NOT NULL ORDER BY {fieldName}";

        SQLiteHelper.Instance.ExecuteQueryAfterOpenDB(sql, (SqliteDataReader sdr) =>
        {
            while (sdr.Read())
            {
                values.Insert(0, sdr.GetInt64(0));
            }

            onFinishCallback?.Invoke(values);
        });

    }

    const string FORMAT_DATE_DAY = "yyyy-MM-dd";
    private void _GetDistinctFullDates(Action<List<string>> onFinishCallback)
    {
        List<string> fullDates = new List<string>();

        // 查询所有非空的 created_at 时间戳
        string sql = $"SELECT DISTINCT created_at FROM  {ConsoleTableName.TABLE_GAME_RECORD}  WHERE created_at IS NOT NULL ORDER BY created_at";


        SQLiteHelper.Instance.ExecuteQueryAfterOpenDB(sql, (SqliteDataReader sdr) =>
        {
            while (sdr.Read())
            {
                // 1. 转换时间戳（毫秒级）为本地时间
                long timestamp = sdr.GetInt64(0);

                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                //DateTime utcDateTime = dateTimeOffset.UtcDateTime;
                //string time = utcDateTime.ToString(FORMAT_DATE_DAY);

                DateTime localDateTime = dateTimeOffset.LocalDateTime;
                string fullDate = localDateTime.ToString(FORMAT_DATE_DAY);

                // 3. 去重添加（避免重复日期）
                if (!fullDates.Contains(fullDate))
                {
                    fullDates.Add(fullDate);
                }
            }

            onFinishCallback?.Invoke(fullDates);
        });

    }



    public string GetGameRecordFilterCondition(SelectGameRecordFilterInfo select = null)
    {
        bool isAll = select == null;

        string gameType = null, turnType = null, hitJackpotType = null, hitBonusType = null, dateTimeStr = null;
        long? gameId = null;
        long? startTimestampMs = null ,endTimestampMS = null;

        if (!isAll)
        {
            gameType = select.selectedIndexGameId == SelectGameRecordFilterInfo.ALL ? null :
                totalGameRecordFilter.gameTypes[select.selectedIndexGameId];

            gameId = null;
            if (select.selectedIndexGameId != SelectGameRecordFilterInfo.ALL )
            {
                gameId = totalGameRecordFilter.gameIds[(int)select.selectedIndexGameId];
            }
  
            turnType = select.selectedIndexTurnType == SelectGameRecordFilterInfo.ALL ? null :
                totalGameRecordFilter.turnTypes[select.selectedIndexTurnType];

           hitJackpotType = select.selectedIndexHitJackpotTypes == SelectGameRecordFilterInfo.ALL ? null :
                totalGameRecordFilter.hitJackpotTypes[select.selectedIndexHitJackpotTypes];

            hitBonusType = select.selectedIndexHitBonusTypes == SelectGameRecordFilterInfo.ALL ? null :
                totalGameRecordFilter.hitBonusTypes[select.selectedIndexHitBonusTypes];

            dateTimeStr = select.selectedIndexDate == SelectGameRecordFilterInfo.ALL ? null :
                totalGameRecordFilter.fullDates[select.selectedIndexDate];

            if (!string.IsNullOrEmpty(dateTimeStr))
            {
                // 获取UTC时区下该日期的起始和结束毫秒级时间戳
                if (TryGetUTCDayStartAndEndTimestamp(dateTimeStr, out long startTimestamp001, out long endTimestamp001))
                {
                    Debug.Log($"UTC日期 {dateTimeStr} 的时间范围：");
                    Debug.Log($"UTC起始时间戳(毫秒)：{startTimestamp001} -> 对应UTC时间：{TimestampToUTCDateTime(startTimestamp001)}");
                    Debug.Log($"UTC结束时间戳(毫秒)：{endTimestamp001} -> 对应UTC时间：{TimestampToUTCDateTime(endTimestamp001)}");

                    startTimestampMs = startTimestamp001;
                    endTimestampMS = endTimestamp001;
                }
                else
                {
                    Debug.LogError("日期格式解析失败，请检查输入的日期字符串格式");
                }
            }
            else
            {
                if (select.selectedStartTimestampMs != SelectGameRecordFilterInfo.ALL)
                {
                    startTimestampMs = select.selectedStartTimestampMs;
                }

                if (select.selectedEndTimestampMs != SelectGameRecordFilterInfo.ALL)
                {
                    endTimestampMS = select.selectedEndTimestampMs;
                }
            }
        }






        return GetTotalFilterCondition(gameType, gameId, turnType, hitJackpotType, hitBonusType, startTimestampMs, endTimestampMS);


        /*
        string sql = $"SELECT * FROM {ConsoleTableName.TABLE_GAME_RECORD}";

        if (!isAll && !string.IsNullOrEmpty(totalFilterCondition))
        {
            sql = sql + $" WHERE {totalFilterCondition}"; // WHERE type
        }

        */

        // Sqlite 的 SELECT * FROM game_record  WHERE ... And ...
        //想得到   “总共有多少条数据”， 如果按n条进行分页，能分多少页，且当前请求第m页，返回第n页的数据

    }



     void GetTotalCout(SelectGameRecordFilterInfo select, Action<string, int> onFinishCallback)
    {
        string filterCondition = GetGameRecordFilterCondition(select);
        SQLiteHelper.Instance.OpenDB(ConsoleTableName.DB_NAME, (connect) =>
        {
            string sql = $"SELECT COUNT(*) FROM {ConsoleTableName.TABLE_GAME_RECORD}";
            if (!string.IsNullOrEmpty(filterCondition))
            {
                sql = sql + $" WHERE {filterCondition}"; // WHERE type
            }
            DebugUtils.Log(sql);
            using (var command1 = new SqliteCommand(sql, connect))
            {
                //int totalCount = (int)command1.ExecuteScalar(); // 【BUG】SQLite 的 COUNT(*) 函数返回值的实际类型是 long（64 位整数），而非 int（32 位整数）；
                //onFinishCallback?.Invoke(filterCondition, totalCount);


                // 修复核心：先转为long，再转int（或直接用long接收）
                object result = command1.ExecuteScalar();

                // 安全转换逻辑：处理DBNull/空值 + 先转long再转int
                int totalCount = 0;
                if (result != null && result != DBNull.Value)
                {
                    // 第一步：将结果转为long（SQLite COUNT(*)的原生类型）
                    long countLong = Convert.ToInt64(result);
                    // 第二步：再转为int（如需兼容大数值，可直接用long接收）
                    totalCount = Convert.ToInt32(countLong);
                }
                DebugUtils.LogError($"【Test】filterCondition={filterCondition} ; totalCount={totalCount} ");
                onFinishCallback?.Invoke(filterCondition, totalCount);
            }
        });
    }


    public void GetGameRecord(SelectGameRecordFilterInfo select, SelectGameRecordPageInfo pageInfo, Action<SelectGameRecordPageResult> onFinishCallback)
    {
        GetTotalCout(select, (filterCondition , totalCount) =>
        {
            SelectGameRecordPageResult result = new SelectGameRecordPageResult();
            if (totalCount == 0)
            {
                result.totalCount = totalCount;
                result.totalCountPerPage = pageInfo.totalCountPerPage;
                result.selectNumberPage = pageInfo.selectNumberPage;
                result.totalPageCount = 0;

                onFinishCallback?.Invoke(result);
            }
            else
            {
                result.totalCount = totalCount;
                result.totalCountPerPage = pageInfo.totalCountPerPage;

                // 总页数 = (总条数 + 每页条数 - 1) / 每页条数
                result.totalPageCount = (totalCount + pageInfo.totalCountPerPage -1) / pageInfo.totalCountPerPage;

                int validPageNum = pageInfo.selectNumberPage;
                if (validPageNum < 1)
                    validPageNum = 1;
                else if (validPageNum > result.totalPageCount)
                    validPageNum = result.totalPageCount;

                result.selectNumberPage = validPageNum;


                int offset = (validPageNum - 1) * pageInfo.totalCountPerPage;
                /*
                    int offset = (validPageNum - 1) * pageInfo.totalCountPerPage; 这行代码核心逻辑是正确的，但存在边界场景的潜在风险，我会帮你拆解清楚问题和优化方案。
                    先明确核心逻辑（正确的部分）
                    这行代码的作用是计算分页的偏移量（即查询时需要跳过的记录数），公式完全符合分页规则：
                    第 1 页：(1-1)*每页条数 = 0 → 跳过 0 条，取前 N 条
                    第 2 页：(2-1)*每页条数 = N → 跳过前 N 条，取第 N+1 到 2N 条
                    第 m 页：(m-1)*每页条数 → 跳过前 (m-1)*N 条，精准定位当前页数据
                 */

                string sql = $"SELECT * FROM {ConsoleTableName.TABLE_GAME_RECORD}";
                if (!string.IsNullOrEmpty(filterCondition))
                {
                    sql = sql + $" WHERE {filterCondition}"; // WHERE type
                }
                sql += " ORDER BY id DESC"; //按主键倒序，保证分页有序(实现了 “最近的信息排在最前面” 的需求)
                sql += $" LIMIT {pageInfo.totalCountPerPage}";
                sql += $" OFFSET {offset}";

                SQLiteHelper.Instance.ExecuteQueryAfterOpenDB(sql, (SqliteDataReader sdr) =>
                {
                    List<TableGameRecordItem> resGameRecord = new List<TableGameRecordItem>();
                    while (sdr.Read())
                    {
                        //resGameRecord.Insert(0,
                        resGameRecord.Add(
                        new TableGameRecordItem()
                        {
                            user_id = sdr.GetString(sdr.GetOrdinal("user_id")),
                            game_id = sdr.GetInt32(sdr.GetOrdinal("game_id")),
                            game_uid = sdr.GetString(sdr.GetOrdinal("game_uid")),

                            game_type = sdr.GetString(sdr.GetOrdinal("game_type")),
                            turn_type = sdr.GetString(sdr.GetOrdinal("turn_type")),
                            hit_jackpot_type = sdr.GetString(sdr.GetOrdinal("hit_jackpot_type")),
                            hit_bonus_type = sdr.GetString(sdr.GetOrdinal("hit_bonus_type")),
                            template_name = sdr.GetString(sdr.GetOrdinal("template_name")),
                            template_data = sdr.GetString(sdr.GetOrdinal("template_data")),
                            created_at = sdr.GetInt64(sdr.GetOrdinal("created_at")),
                        });
                    }
                    result.pageItems = resGameRecord;

                    onFinishCallback?.Invoke(result);
                });
            }
        });
    }




    /// <summary>
    /// 根据日期字符串获取UTC时区下当天的起始和结束毫秒级时间戳
    /// </summary>
    /// <param name="dateStr">日期字符串，格式：yyyy-MM-dd</param>
    /// <param name="startTimestamp">UTC当天起始时间戳(毫秒)：yyyy-MM-dd 00:00:00.000 UTC</param>
    /// <param name="endTimestamp">UTC当天结束时间戳(毫秒)：yyyy-MM-dd 23:59:59.999 UTC</param>
    /// <returns>解析是否成功</returns>
    public static bool TryGetUTCDayStartAndEndTimestamp(string dateStr, out long startTimestamp, out long endTimestamp)
    {
        // 初始化输出参数
        startTimestamp = 0;
        endTimestamp = 0;

        // 定义日期格式
        string format = "yyyy-MM-dd";
        DateTime date;

        // 尝试解析日期字符串
        if (!DateTime.TryParseExact(dateStr, format, System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out date))
        {
            return false;
        }

        // 直接构造UTC时间，避免时区转换
        // 起始时间：UTC 2026-10-21 00:00:00.000
        DateTime utcStartTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, DateTimeKind.Utc);
        // 结束时间：UTC 2026-10-21 23:59:59.999
        DateTime utcEndTime = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, DateTimeKind.Utc);

        // 定义Unix纪元时间（1970-01-01 00:00:00 UTC）
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // 计算UTC时间戳（毫秒）
        startTimestamp = (long)(utcStartTime - epoch).TotalMilliseconds;
        endTimestamp = (long)(utcEndTime - epoch).TotalMilliseconds;

        return true;
    }
  
    /// <summary>
    /// 将毫秒级时间戳转换回UTC DateTime对象（方便验证）
    /// </summary>
    /// <param name="timestamp">毫秒级时间戳</param>
    /// <returns>对应的UTC DateTime对象</returns>
    public static DateTime TimestampToUTCDateTime(long timestamp)
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddMilliseconds(timestamp);
    }





    string totalFilterCondition = ""; //"1=1";

    public string GetTotalFilterCondition(string gameType, long? gameId, string turnType, string hitJackpotType, string hitBonusType, long? startTime, long? endTime)
    {

        totalFilterCondition = "";
        // 游戏类型（字符串替换方式）
        if (!string.IsNullOrEmpty(gameType))
        {
            totalFilterCondition += " AND game_type = @GameType";
            totalFilterCondition = totalFilterCondition.Replace("@GameType", EscapeSqlString(gameType));
        }

        // 游戏ID（字符串替换方式）
        if (gameId.HasValue)
        {
            totalFilterCondition += " AND game_id = @GameId";
            totalFilterCondition = totalFilterCondition.Replace("@GameId", gameId.Value.ToString());
        }

        // 回合类型（字符串替换方式）
        if (!string.IsNullOrEmpty(turnType))
        {
            totalFilterCondition += " AND turn_type = @TurnType";
            totalFilterCondition = totalFilterCondition.Replace("@TurnType", EscapeSqlString(turnType));
        }

        // 中奖类型（字符串替换方式）
        if (!string.IsNullOrEmpty(hitJackpotType))
        {
            totalFilterCondition += " AND hit_jackpot_type = @HitJackpotType";
            totalFilterCondition = totalFilterCondition.Replace("@HitJackpotType", EscapeSqlString(hitJackpotType));
        }

        // 奖励类型（字符串替换方式）
        if (!string.IsNullOrEmpty(hitBonusType))
        {
            totalFilterCondition += " AND hit_bonus_type = @HitBonusType";
            totalFilterCondition = totalFilterCondition.Replace("@HitBonusType", EscapeSqlString(hitBonusType));
        }

        // 开始时间（字符串替换方式）
        if (startTime.HasValue)
        {
            totalFilterCondition += " AND created_at >= @StartTime";
            totalFilterCondition = totalFilterCondition.Replace("@StartTime", startTime.Value.ToString());
        }

        // 结束时间（字符串替换方式）
        if (endTime.HasValue)
        {
            totalFilterCondition += " AND created_at <= @EndTime";
            totalFilterCondition = totalFilterCondition.Replace("@EndTime", endTime.Value.ToString());
        }


        if (!string.IsNullOrEmpty(totalFilterCondition))
        {
            //totalFilterCondition = $" 1=1 {totalFilterCondition}";

            // 先去除开头的所有空格（处理 " AND..." "   AND..." 等情况）
            totalFilterCondition = totalFilterCondition.TrimStart();

            // 检查是否以 "AND" 开头（不区分大小写可选，根据需求调整）
            if (totalFilterCondition.StartsWith("AND", StringComparison.Ordinal))
            {
                // 截取 "AND" 之后的部分，并再次去除开头空格（处理 "AND  xxx" 中AND后的空格）
                return totalFilterCondition.Substring(3).TrimStart();
            }
        }

        return totalFilterCondition;
    }

    private string EscapeSqlString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "''";
        // SQLite中字符串用单引号包裹，需要转义输入中的单引号（两个单引号表示一个）
        return "'" + input.Replace("'", "''") + "'";
    }

}





public class TotalGameFilterOptions
{
    // 核心字段枚举值
    public List<long> gameIds { get; set; } = new List<long>();          // game_id 所有可能值
    public List<string> gameTypes { get; set; } = new List<string>();   // game_type 所有可能值
    public List<string> turnTypes { get; set; } = new List<string>();   // turn_type 所有可能值
    public List<string> hitJackpotTypes { get; set; } = new List<string>(); // hit_jackpot_type 所有可能值
    public List<string> hitBonusTypes { get; set; } = new List<string>();  // hit_bonus_type 所有可能值（修正笔误）

    // 日期维度：仅保留 yyyy-MM-dd 格式的完整日期列表
    public List<string> fullDates { get; set; } = new List<string>();   // 所有出现过的完整日期（yyyy-MM-dd）
}


/// <summary> 选择的游戏内容类型 </summary>
public class SelectGameRecordFilterInfo 
{
    public const int ALL = -1;

    public int selectedIndexGameType = ALL;
    public int selectedIndexGameId = ALL;
    public int selectedIndexTurnType = ALL;
    public int selectedIndexHitJackpotTypes = ALL;
    public int selectedIndexHitBonusTypes = ALL;
    public int selectedIndexDate = ALL;  // 选择那天

    /// <summary> 选择开始时间的时间戳 </summary>
    public long selectedStartTimestampMs = ALL;
    /// <summary> 选择结束时间的时间戳 </summary>
    public long selectedEndTimestampMs = ALL;
}


/// <summary> 选择的游戏内容类型 </summary>
public class SelectGameRecordPageInfo
{
    public int totalCountPerPage = 1;
    public int selectNumberPage = 0;
}

public class SelectGameRecordPageResult
{
    /// <summary> 总共多少条 </summary>
    public int totalCount = 0;
    /// <summary> 每页多少条 </summary>
    public int totalCountPerPage = 1;
    /// <summary> 选择第几页 </summary>
    public int selectNumberPage = 0;
    /// <summary> 总共多少页 </summary>
    public int totalPageCount = 0;
    /// <summary> 每页内容 </summary>
    public List<TableGameRecordItem> pageItems;
}