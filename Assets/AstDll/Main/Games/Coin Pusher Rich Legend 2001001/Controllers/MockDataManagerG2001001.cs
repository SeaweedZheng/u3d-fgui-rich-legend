using GameMaker;
using Newtonsoft.Json;
using SBoxApi;
using SimpleJSON;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtil;
using JetBrains.Annotations;
using System.Linq;

namespace CoinPusherRichLegend2001001
{
    public enum SBoxGameState
    {
        GSNormal = 0,

        GSStart = 1,
        /// <summary> 普通局且不中线 </summary>
        GSEnd = 2,
        /// <summary> 赢线 </summary>
        GSWinline = 3,
        /// <summary> 免费游戏 </summary>
        GSFreeGame = 4,
        /// <summary> 送球 </summary>
        GSBonus = 5,
        /// <summary> 中了中小彩金 </summary>
        GSJp1 = 6,
        /// <summary> 中了大彩金</summary>
        GSJp2 = 7,
        /// <summary> 中了巨大彩金</summary>
        GSJp3 = 8,

        GSJpMega = 9,

        GSBonus1 = 10,

        GSBonus2 = 11,

        GSOperater = 11
    }
    public class MockDataManagerG2001001 : MonoSingleton02<MockDataManagerG2001001>
    {



        //long TotalBet => (long)SBoxModel.Instance.CoinInScale; // ContentModel.Instance.totalBet


        long totalBet;

        public void ParseSlotSpin(long totalBet, JSONNode res, JackpotRes02 jpGameRes = null)
        {

            this.totalBet = totalBet;

            SBoxGameState gameState = (SBoxGameState)((int)res["gameState"]);


            int hitSymbolNumber = (int)res["icon"];
            int hitLinesCount =  (int)res["lineNum"];
            int reward = (int)res["reward"];
            //DeskInfo info = hitLinesCount > 0?  DeckCreater.Instance.CreatDeck(hitSymbolNumber, hitLinesCount)  :DeckCreater.Instance.CreatNotWinDeck();


            int totalEarnCoins = 0;



            // 彩金
            ContentModel.Instance.jpGameRes = jpGameRes;


            ContentModel.Instance.jpGameSymbolWin = null;
            ContentModel.Instance.jpOnlineSymbolWin = null;
            ContentModel.Instance.bonusSymbolWin = null;

            List<SymbolWin> winList = new List<SymbolWin>();


            DeskInfo info = null;

            ContentModel.Instance.bonusResults.Clear();

            switch (gameState)
            {
                case SBoxGameState.GSBonus1:
                    {
                        totalEarnCoins += reward;
                        info = DeckCreater.Instance.CreatDeck(hitSymbolNumber, 1);
                        ContentModel.Instance.bonusSymbolWin = info.winList[0];


                        ContentModel.Instance.bonusResults.Add(1, null);
                    }
                    break;
                case SBoxGameState.GSBonus2:
                    {
                        totalEarnCoins += reward;
                        info = DeckCreater.Instance.CreatDeck(hitSymbolNumber, 1);
                        ContentModel.Instance.bonusSymbolWin = info.winList[0];
                        //ContentModel.Instance.bonusSymbolWin.earnCredit = reward;

                        JSONNode node = new JSONObject();
                        node.Add("totalWinCoins", reward);
                        node.Add("rewardCoins", res["bonusResult"]);
                        ContentModel.Instance.bonusResults.Add(2, node);
                    }
                    break;
                case SBoxGameState.GSJp1:
                    {
                        reward = (int)jpGameRes.jpWinDic[0].winCredit;
                        totalEarnCoins += reward;
                        info = DeckCreater.Instance.CreatDeck(10, 1);
                        ContentModel.Instance.jpGameSymbolWin = info.winList[0];
                    }
                    break;
                case SBoxGameState.GSJp2:
                    {
                        reward = (int)jpGameRes.jpWinDic[1].winCredit;
                        totalEarnCoins += reward;
                        info = DeckCreater.Instance.CreatDeck(11, 1);
                        ContentModel.Instance.jpGameSymbolWin = info.winList[0];
                    }
                    break;
                case SBoxGameState.GSJp3:
                    {
                        reward = (int)jpGameRes.jpWinDic[2].winCredit;
                        totalEarnCoins += reward;
                        info = DeckCreater.Instance.CreatDeck(12, 1);
                        ContentModel.Instance.jpGameSymbolWin = info.winList[0];
                    }
                    break;
                case SBoxGameState.GSJpMega:
                    {
                        reward = (int)jpGameRes.jpWinDic[3].winCredit;
                        totalEarnCoins += reward;
                        info = DeckCreater.Instance.CreatDeck(13, 1);
                        ContentModel.Instance.jpGameSymbolWin = info.winList[0];
                    }
                    break;
                default:
                    {
                         info = hitLinesCount > 0 ?
                            DeckCreater.Instance.CreatDeck(hitSymbolNumber, hitLinesCount)
                            : DeckCreater.Instance.CreatNotWinDeck();


                            // 普通中线
                            if (hitLinesCount > 0)
                            {
                                winList = info.winList;


                                totalEarnCoins += reward;

                                int winCredit = reward / hitLinesCount;

                                foreach (SymbolWin sw in winList)
                                {
                                    sw.earnCredit = winCredit;
                                }
                            }

                    }
                    break;
            }




            DebugUtils.LogWarning($"DeskInfo = {JsonConvert.SerializeObject(info)}");

            if (++MainModel.Instance.gameNumber < 0)
                MainModel.Instance.gameNumber = 1;


            long creditBefore = MainBlackboardController.Instance.myRealCredit;

            int freeSpinTotalTimes, freeSpinPlayTimes;
            freeSpinTotalTimes = 0;// (int)res["maxRound"];
            freeSpinPlayTimes = 0;// (int)res["curRound"];


            bool isFreeSpinTrigger = freeSpinPlayTimes == 0 && freeSpinTotalTimes > 0;
            bool isFreeSpinResult = freeSpinTotalTimes > 0 && freeSpinPlayTimes == freeSpinTotalTimes;
            bool isFreeSpin = freeSpinPlayTimes > 0 && freeSpinTotalTimes > 0;

            string strDeckRowCol = SlotTool.GetDeckRCsByRCd(info.deckRowCol);


            bool isReelsSlowMotion = false;






            ContentModel.Instance.winList = winList;
            ContentModel.Instance.response = res.ToString();
            ContentModel.Instance.strDeckRowCol = strDeckRowCol;




            ContentModel.Instance.totalEarnCoins = totalEarnCoins;
            ContentModel.Instance.baseGameWinCoins = totalEarnCoins;









            ContentModel.Instance.curGameCreatTimeMS = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            ContentModel.Instance.curGameNumber = MainModel.Instance.gameNumber;

            ContentModel.Instance.curGameGuid = isFreeSpin
                ? $"{MainModel.Instance.gameID}-{UnityEngine.Random.Range(100, 1000)}-{ContentModel.Instance.curGameCreatTimeMS}-{MainModel.Instance.gameNumber}-{ContentModel.Instance.gameNumberFreeSpinTrigger}"
                : $"{MainModel.Instance.gameID}-{UnityEngine.Random.Range(100, 1000)}-{ContentModel.Instance.curGameCreatTimeMS}-{MainModel.Instance.gameNumber}";

            if (ContentModel.Instance.isFreeSpinTrigger)
            {
                ContentModel.Instance.gameNumberFreeSpinTrigger = MainModel.Instance.gameNumber;
                ContentModel.Instance.freeSpinTriggerGuid = ContentModel.Instance.curGameGuid;
            }

            long afterBetCredit = !isFreeSpin ? creditBefore - totalBet : creditBefore;



            // 免费游戏累计总赢
            if (isFreeSpin)
            {
                ContentModel.Instance.freeSpinTotalWinCoins += totalEarnCoins;
            }
            else
            {
                ContentModel.Instance.freeSpinTotalWinCoins = 0;
            }

            List<List<int>> deckColRow = SlotTool.GetDeckCRdByRCs(strDeckRowCol);

            ContentModel.Instance.isReelsSlowMotion = isReelsSlowMotion;






            ContentModel.Instance.winList = winList;


            if (isFreeSpin && freeSpinTotalTimes != 0)
            {
                ContentModel.Instance.freeSpinAddNum =
                    freeSpinTotalTimes - ContentModel.Instance.freeSpinTotalTimes;
            }
            else
                ContentModel.Instance.freeSpinAddNum = 0;

            ContentModel.Instance.showFreeSpinRemainTime = isFreeSpin
            ? (ContentModel.Instance.freeSpinTotalTimes - ContentModel.Instance.freeSpinPlayTimes - 1)
            : 0;

            ContentModel.Instance.freeSpinTotalTimes = freeSpinTotalTimes;
            ContentModel.Instance.freeSpinPlayTimes = freeSpinPlayTimes;
            ContentModel.Instance.isFreeSpinTrigger = isFreeSpinTrigger;
            ContentModel.Instance.isFreeSpinResult = isFreeSpinResult;
            //ContentModel.Instance.isBonus1 = isBonusBall;
           // ContentModel.Instance.hitBallCount = hitBallCount;
            //ContentModel.Instance.isHitJackpotGame = isJackpotGrand || isJackpotMajor || isJackpotMinMinor;

            if (isFreeSpinTrigger)
            {
                ContentModel.Instance.curReelStripsIndex = "BS";
                ContentModel.Instance.nextReelStripsIndex = "FS";
            }
            else if (isFreeSpinResult)
            {
                ContentModel.Instance.curReelStripsIndex = "FS";
                ContentModel.Instance.nextReelStripsIndex = "BS";
            }
            else if (isFreeSpin)
            {
                ContentModel.Instance.curReelStripsIndex = "FS";
                ContentModel.Instance.nextReelStripsIndex = "FS";
            }
            else
            {
                ContentModel.Instance.curReelStripsIndex = "BS";
                ContentModel.Instance.nextReelStripsIndex = "BS";
            }
        }

        public void Record()
        {

            // 游戏场景记录
            PusherMaker.GameSenceData gameSenceData = new PusherMaker.GameSenceData();

            if (++MainModel.Instance.reportId < 0)
                MainModel.Instance.reportId = 1;

            gameSenceData.respone = ContentModel.Instance.response;
            gameSenceData.reportId = MainModel.Instance.reportId;
            gameSenceData.timeS = ContentModel.Instance.curGameCreatTimeMS / 1000;
            gameSenceData.gameNumber = MainModel.Instance.gameNumber;
            gameSenceData.gameNumberFreeSpinTrigger = ContentModel.Instance.isFreeSpin ? ContentModel.Instance.gameNumberFreeSpinTrigger : 0;
            gameSenceData.isFreeSpin = ContentModel.Instance.isFreeSpin;
            gameSenceData.freeSpinAddNum = ContentModel.Instance.freeSpinAddNum;

            gameSenceData.curStripsIndex = ContentModel.Instance.curReelStripsIndex;
            gameSenceData.nextStripsIndex = ContentModel.Instance.nextReelStripsIndex;
            gameSenceData.strDeckRowCol = ContentModel.Instance.strDeckRowCol;
            gameSenceData.deckRowCol = SlotTool.GetDeckRClByRCs(ContentModel.Instance.strDeckRowCol);

            gameSenceData.winFreeSpinTrigger = null; // ContentModel.Instance.winFreeSpinTriggerOrAddCopy;
            gameSenceData.winList = ContentModel.Instance.winList;
            gameSenceData.freeSpinPlayTimes = ContentModel.Instance.freeSpinPlayTimes;
            gameSenceData.freeSpinTotalTimes = ContentModel.Instance.freeSpinTotalTimes;
            gameSenceData.freeSpinTotalWinCoins = ContentModel.Instance.freeSpinTotalWinCoins;
            gameSenceData.totalBet = totalBet;
            gameSenceData.creditPerCoinIn = SBoxModel.Instance.CoinInScale;
            gameSenceData.jackpotWinCoins = 0;  //【外设彩金-需要修改】
            gameSenceData.baseGameWinCoins = ContentModel.Instance.baseGameWinCoins;




            string turnType = ContentModel.Instance.isFreeSpin ? "free_spin" :
                    ContentModel.Instance.isFreeSpinTrigger ? "free_spin_trigger" :
                    //ContentModel.Instance.isBonus1 ? "bonus1" :
                    "spin";


            //通用模板
            TemplateDataGameRecordCoinPusher1 templateData = new TemplateDataGameRecordCoinPusher1()
            {
                deckRowCol = ContentModel.Instance.strDeckRowCol,
                gameUid = ContentModel.Instance.curGameGuid,
                turnType = turnType,
                creatAt = ContentModel.Instance.curGameCreatTimeMS,

                coinPerCredit = SBoxModel.Instance.CoinInScale,
                creditPerCoin = 0,
                baseGameWinCoins = (int)ContentModel.Instance.baseGameWinCoins,
                //jackpotWinCoins = 
                //jackpotType =

                // bonusGameWinCoins = 
                // bonusType = 
                freeSpinTotalCount = ContentModel.Instance.freeSpinTotalTimes,
                freeSpinCurNumber = ContentModel.Instance.freeSpinPlayTimes,
                freeSpinAddCount = ContentModel.Instance.freeSpinAddNum,

               // detail = "【测试】 seat:{0} reward:{1}##【测试】 seat:{0} reward:{1}",
               // args = "1,50##2,100",
            };

            if (ContentModel.Instance.winList.Count > 0)
            {
                string detailStr = "", argsStr = "";
                for (int i = 0; i < ContentModel.Instance.winList.Count; i++)
                {
                    SymbolWin sw = ContentModel.Instance.winList[i];
                    if (i > 0)
                    {
                        detailStr += "##";
                        argsStr += "##";
                    }
                    detailStr += "hit line {0}, reward coins {1}";
                    argsStr += $"{sw.lineNumber},{sw.earnCredit}";
                }

                templateData.detail = detailStr;
                templateData.args = argsStr;
            }
         



            //通用游戏记录
            TableGameRecordItem slotGameRecordItem = new TableGameRecordItem()
            {
                game_type = "coin_pusher",
                turn_type = turnType,
                game_id = ConfigUtils.curGameId,
                game_uid = ContentModel.Instance.curGameGuid,
                created_at = ContentModel.Instance.curGameCreatTimeMS,
                respond = ContentModel.Instance.response,
                template_name = nameof(TemplateDataGameRecordCoinPusher1),
                //template_data = JsonConvert.SerializeObject(templateData),
            };


            // 彩金数据
            JackpotRes02 info = ContentModel.Instance.jpGameRes;


            // 数据修改：
            gameSenceData.jpGrand = info.curJackpots[0];
            gameSenceData.jpMajor = info.curJackpots[1];
            gameSenceData.jpMinor = info.curJackpots[2];
            gameSenceData.jpMini = info.curJackpots[3];

            if (info.jpWinDic != null && info.jpWinDic.Count > 0)
            {
                KeyValuePair<int, JackpotWinInfo> kv = info.jpWinDic.ElementAt(0);
                JackpotWinInfo item = kv.Value;


                int winJPCredit = (int)item.winCredit;

                templateData.jackpotWinCoins = winJPCredit;
                templateData.jackpotType = item.name;
                //slotGameRecordItem.jackpot_win_coins = winJPCredit;
                //slotGameRecordItem.jackpot_type = item.name;

                slotGameRecordItem.hit_jackpot_type = item.name;

                gameSenceData.jpWinInfo = item;
                gameSenceData.jackpotWinCoins = winJPCredit;


                // 游戏彩金记录
                TableJackpotRecordAsyncManager.Instance.AddJackpotRecord(item.id, item.name, winJPCredit, -1, -1, ContentModel.Instance.curGameGuid, ContentModel.Instance.curGameCreatTimeMS);

                // 额外奖上报(暂时不用)
                //#seaweed# DeviceBonusReport.Instance.ReportBonus(item.name, item.name, winJPCredit, -1, (msg) => { }, (err) => { });
            }

            // 每日营收统计(暂时不用)
            /*TableBusniessDayRecordAsyncManager.Instance.AddTotalBetWin(
                ContentModel.Instance.curReelStripsIndex == "FS" ? 0 : TotalBet,
             ContentModel.Instance.baseGameWinCoins + gameSenceData.jackpotWinCoins, SBoxModel.Instance.myCredit);
            */


            ContentModel.Instance.totalEarnCoins = ContentModel.Instance.baseGameWinCoins + gameSenceData.jackpotWinCoins;


            slotGameRecordItem.scene = JsonConvert.SerializeObject(gameSenceData);
            slotGameRecordItem.template_data = JsonConvert.SerializeObject(templateData);

            string sql = SQLiteAsyncHelper.SQLInsertTableData<TableGameRecordItem>(ConsoleTableName.TABLE_GAME_RECORD, slotGameRecordItem);
            SQLiteAsyncHelper.Instance.ExecuteNonQueryAsync(sql);

        }




        public void Report()
        {
            JackpotRes02 info = ContentModel.Instance.jpGameRes;
            if (info.jpWinDic != null && info.jpWinDic.Count > 0)
            {
                KeyValuePair<int, JackpotWinInfo> kv = info.jpWinDic.ElementAt(0);
                JackpotWinInfo item = kv.Value;

                Dictionary<string, object> req = new Dictionary<string, object>()
                {
                    ["type"] = "JackpotGame",
                    ["game_number"] = ContentModel.Instance.curGameGuid,
                    ["jp_type"] = $"jp{item.id + 1}",
                    ["coins"] = (int)item.winCredit,
                };
                NetCmdManager.Instance.RpcUpReportWin(req);
            }

            if (ContentModel.Instance.isFreeSpinTrigger)
            {
                Dictionary<string, object> req = new Dictionary<string, object>()
                {
                    ["type"] = "FreeSpinTrigger",
                    ["game_number"] = ContentModel.Instance.curGameGuid,
                    ["total_times"] = ContentModel.Instance.freeSpinTotalTimes,
                };
                NetCmdManager.Instance.RpcUpReportWin(req);
            }

            if (ContentModel.Instance.isFreeSpinResult)
            {
                Dictionary<string, object> req = new Dictionary<string, object>()
                {
                    ["type"] = "FreeSpinResult",
                    ["game_number"] = ContentModel.Instance.freeSpinTriggerGuid,
                    ["total_times"] = ContentModel.Instance.freeSpinTotalTimes,
                };
                NetCmdManager.Instance.RpcUpReportWin(req);
            }


            /*
            if (ContentModel.Instance.isBonus1)
            {
                Dictionary<string, object> req = new Dictionary<string, object>()
                {
                    ["type"] = "Bonus1",
                    ["game_number"] = ContentModel.Instance.curGameGuid,
                    ["count"] = ContentModel.Instance.hitBallCount,
                };
                NetCmdManager.Instance.RpcUpReportWin(req);
            }
            */

            //try
            //{
            //    // 数据数据上报
            //    string str = ReportDataUtils.CreatReportData(gameSenceData, SBoxModel.Instance.sboxPlayerInfo);
            //    DebugUtils.Log($"数据上报成功 {str}");
            //    ReportManager.Instance.SendData(str, null, null);
            //}
            //catch (Exception ex) { }

        }



        void OnEnable()
        {
            EventCenter.Instance.AddEventListener<EventData>(GlobalEvent.ON_GM_EVENT, OnGMEvent);
        }

        void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<EventData>(GlobalEvent.ON_GM_EVENT, OnGMEvent);
        }

        void OnGMEvent(EventData res)
        {
            if (ApplicationSettings.Instance.isMock == false) return;

            if (res.id != 2001001) return;

            switch (res.name)
            {
                case GlobalEvent.GMSingleWinLine:
                    nextSpin = SpinDataType.SingleWinLine;
                    break;
                case GlobalEvent.GMMultipleWinLine:
                    nextSpin = SpinDataType.MultipleWinLine;
                    break;
                case GlobalEvent.GMFreeSpin:
                    nextSpin = SpinDataType.FreeSpin;
                    break;
                case GlobalEvent.GMBigWin:
                    nextSpin = SpinDataType.BigWin;
                    break;
                case GlobalEvent.GMJp1:
                    nextSpin = SpinDataType.Jp1;
                    //GlobalJackpotConsole.NetClientHelper02.Instance.testIsHitJpGrandNext = true;
                    break;
                case GlobalEvent.GMJp2:
                    nextSpin = SpinDataType.Jp2;
                    //GlobalJackpotConsole.NetClientHelper02.Instance.testIsHitJpMajorNext = true;
                    break;
                case GlobalEvent.GMJp3:
                    nextSpin = SpinDataType.Jp3;
                    break;
                case GlobalEvent.GMJpConsole1:
                    nextSpin = SpinDataType.JpConsole1;

                    break;
                case GlobalEvent.GMJpOnline:
                    //nextSpin = SpinDataType.JpOnline;
                    MachineDataManager02.Instance.testIsHitJackpotOnLine = true;
                    break;
                case GlobalEvent.GMBonus1:
                    nextSpin = SpinDataType.Bonus1;
                    break;
                case GlobalEvent.GMBonus2:
                    nextSpin = SpinDataType.Bonus2;
                    break;
            }

        }


        SpinDataType nextSpin = SpinDataType.None;


        enum SpinDataType
        {
            None,
            SingleWinLine,
            MultipleWinLine,
            Normal,
            FreeSpin,
            BigWin,
            Jp1,
            Jp2,
            Jp3,
            Jp4,
            JpConsole1,
            JpOnline,
            Bonus1,
            Bonus2,
        };

        private Dictionary<SpinDataType, List<string[]>> spinDatas = new Dictionary<SpinDataType, List<string[]>>()
        {
            [SpinDataType.Normal] = new List<string[]>()
            {
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__0.json" },
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__1.json" },//单线
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__2.json" },//多线
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__3.json" },
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__4.json" },
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__5.json" },
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__6.json" },
            },
            [SpinDataType.Jp1] = new List<string[]>()
            {
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__jp1.json" },
            },
            [SpinDataType.Jp2] = new List<string[]>()
            {
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__jp2.json" },
            },
            [SpinDataType.Jp3] = new List<string[]>()
            {
                new string[] { "Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__jp3.json" },
            },

            [SpinDataType.JpConsole1] = new List<string[]>()
            {
                new string[] {"Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__jpmega.json" },
            },
            [SpinDataType.Bonus1] = new List<string[]>()
            {
                new string[] {"Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__bonus1.json"},
            },
            [SpinDataType.Bonus2] = new List<string[]>()
            {
                new string[] {"Assets/AstBundle/Games/Coin Pusher Rich Legend 2001001/ABs/Mock/g2001001__slot_spin__bonus2.json"},
            },

            /*
            [SpinDataType.FreeSpin] = new List<string[]>()
            {
               new string[]
                {
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_0.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_1.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_2.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_3.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_4.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_5.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_6.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_7.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_8.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_9.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_10.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_11.json",
                    "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__20251030__free_12.json",
                },
            },

            [SpinDataType.SingleWinLine] = new List<string[]>()
            {
                new string[] { "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__win_1.json" },//单线
            },
            [SpinDataType.MultipleWinLine] = new List<string[]>()
            {
                new string[] { "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__win_2.json" },//多线
            },

            [SpinDataType.Jp4] = new List<string[]>()
            {
                new string[] { "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__jackpot_mini.json" },
            },
            [SpinDataType.Bonus1Ball] = new List<string[]>()
            {
                new string[] { "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__ball_0.json" },
                new string[] { "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__ball_1.json" },
                new string[] { "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__ball_2.json" },
                new string[] { "Assets/HotFix/Games/Mock/Resources/g200_real/g200__slot_spin__ball_3.json" }
            },
            [SpinDataType.BigWin] = new List<string[]>()
            {
                new string[] { "Assets/HotFix/Games/_Mock/Resources/g200_real/g200__slot_spin__Bigwin_0.json" },
                new string[] { "Assets/HotFix/Games/_Mock/Resources/g200_real/g200__slot_spin__Bigwin_1.json" },
                new string[] { "Assets/HotFix/Games/_Mock/Resources/g200_real/g200__slot_spin__Bigwin_2.json" },
                new string[] { "Assets/HotFix/Games/_Mock/Resources/g200_real/g200__slot_spin__Bigwin_3.json" },
            }
            */

        };





        Queue<string> curDatas = new Queue<string>();


        public void RequestSlotSpinFromMock(long totalBet, Action<JSONNode> successCallback,
            Action<BagelCodeError> errorCallback)
        {
            Timer.DelayAction(0.2f, () =>
            {
                if (curDatas.Count == 0)
                {
                    /*  随机数据
                    int dataIndex = UnityEngine.Random.Range(0, spinDatas.Count);
                    List<string[]> target = nextSpin != SpinDataType.None?
                        spinDatas[nextSpin] : spinDatas.ElementAt(dataIndex).Value;
                    nextSpin = SpinDataType.None;
                    */
                    List<string[]> target = null;


                    if (nextSpin != SpinDataType.None && !spinDatas.ContainsKey(nextSpin))
                    {
                        DebugUtils.LogError($"没有key:{nextSpin} 对应的数据");
                    }

                    target = nextSpin != SpinDataType.None && spinDatas.ContainsKey(nextSpin) ? 
                        spinDatas[nextSpin] : spinDatas[SpinDataType.Normal];
                    nextSpin = SpinDataType.None;

                    string[] strs = target[UnityEngine.Random.Range(0, target.Count)];
                    curDatas = new Queue<string>(strs);  // 会改变引用数据  
                }

                string path = curDatas.Dequeue();
                /*int resourcesIndex = path.IndexOf("Resources/");
                string remainingPath = path.Substring(resourcesIndex + "Resources/".Length);
                remainingPath = remainingPath.Split('.')[0];*/

                try
                {
                    DebugUtils.LogWarning($"<color=yellow>mock down</color>: 使用数据: {path}");

                    ResourceManager02.Instance.LoadAsset<TextAsset>(path, (jsn) =>
                    {
                        if (jsn != null && jsn.text != null)
                        {
                            JSONNode res = JSON.Parse(jsn.text);
                            successCallback?.Invoke(res);
                        }
                        else
                        {
                            BagelCodeError err = new BagelCodeError() { code = 404, msg = $"找不到数据: {path}" };
                            errorCallback?.Invoke(err);
                        }
                    });
                    /*
                    TextAsset jsn = Resources.Load<TextAsset>(remainingPath);
                    if (jsn != null && jsn.text != null)
                    {
                        JSONNode res = JSON.Parse(jsn.text);
                        successCallback?.Invoke(res);
                    }
                    else
                    {
                        BagelCodeError err = new BagelCodeError() { code = 404, msg = $"找不到数据: {path}" };
                        errorCallback?.Invoke(err);
                    }*/
                }
                catch (Exception ex)
                {
                    DebugUtils.LogError($"数据报错： {path}");
                }


            });
        }






    }
}