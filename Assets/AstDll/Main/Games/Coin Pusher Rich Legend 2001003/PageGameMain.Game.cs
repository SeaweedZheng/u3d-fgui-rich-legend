using FairyGUI;
using GameMaker;
using Newtonsoft.Json;
using SBoxApi;
using SimpleJSON;
using SlotMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoinPusherRichLegend2001003
{


    public partial class PageGameMain
    {

        Coroutine coInit, coGameOnce, coGameAuto, coGameIdel, coEffectSlowMotion, coReelsTurn;


        long TotalBet => (long)SBoxModel.Instance.CoinInScale; // ContentModel.Instance.totalBet

        void StartGameOnce(Action successCallback = null, Action<object[]> errorCallback = null)
        {
            ContentModel.Instance.totalPlaySpins = 1;
            ContentModel.Instance.remainPlaySpins = 1;
            StartGameTotalSpins(successCallback, errorCallback);
        }

        void StartGameTotalSpins(Action successCallback = null, Action<object[]> errorCallback = null)
        {
            if (coGameAuto != null) mono.StopCoroutine(coGameAuto);
            coGameAuto = mono.StartCoroutine(GameTotalSpins(successCallback, errorCallback));
        }


        void StartGameAuto(Action successCallback = null, Action<object[]> errorCallback = null)
        {
            if (coGameAuto != null) mono.StopCoroutine(coGameAuto);
            coGameAuto = mono.StartCoroutine(GameAuto(successCallback, errorCallback));
        }





        IEnumerator GameAuto(Action successCallback, Action<object[]> errorCallback)
        {
            bool isErr = false;
            Action<object[]> errFunc = (err) =>
            {
                isErr = true;
                errorCallback?.Invoke(err);
            };

            while (ContentModel.Instance.isAuto && !ContentModel.Instance.isRequestToStop)
            {
                yield return GameOnce(null, errFunc);

                if (isErr)
                    yield break;

                /*
                float time = Time.time;
                while (Time.time - time < 1f)
                {
                    yield return new WaitForSeconds(0.1f);
                    if (!ContentModel.Instance.isAuto)
                        break;
                }*/

                yield return new WaitForSeconds(0.1f);

                if (!ContentModel.Instance.isAuto)
                    break;
            }

            if (ContentModel.Instance.isRequestToStop)
            {
                ContentModel.Instance.isRequestToStop = false;
                ContentModel.Instance.isAuto = false;
            }

            if (successCallback != null)
                successCallback.Invoke();
        }

        IEnumerator GameTotalSpins(Action successCallback, Action<object[]> errorCallback)
        {
            bool isErr = false;
            Action<object[]> errFunc = (err) =>
            {
                isErr = true;
                errorCallback?.Invoke(err);
            };

            while (--ContentModel.Instance.remainPlaySpins >= 0 && !ContentModel.Instance.isRequestToStop)
            {
                yield return GameOnce(null, errFunc);

                if (isErr)
                    yield break;

                if (ContentModel.Instance.remainPlaySpins == 0)
                    break;

                yield return new WaitForSeconds(1f);
            }

            ContentModel.Instance.remainPlaySpins = ContentModel.Instance.totalPlaySpins;
            ContentModel.Instance.isRequestToStop = false;

            /*以下在successCallback中调用过
            ContentModel.Instance.isRequestToStop = false;
            ContentModel.Instance.isAuto = false;
            ContentModel.Instance.isSpin = false;
            */
            if (successCallback != null)
                successCallback.Invoke();
        }


        //#seaweed#【待完善】
        IEnumerator GameOnce(Action successCallback, Action<object[]> errorCallback)
        {
            bool isNext = false;
            bool isBreak = false;
            string errMsg = "";



            if (!SBoxModel.Instance.isMachineActive)
            {
                errorCallback?.Invoke(new object[] { ErrorCode.ERROR, "<size=24>Machine not activated!</size>" });/**/
                yield break;
            }

            if (SBoxModel.Instance.myCredit < TotalBet)
            {
                errorCallback?.Invoke(new object[] { ErrorCode.BALANCE_IS_INSUFFICIENT, "<size=15>Balance is insufficient, please recharge first</size>" });
                yield break;
            }


            OnGameReset();

            ContentModel.Instance.gameState = GameState.Spin;

            slotMachineCtrl.BeginTurn();


            TestManager.Instance.ShowTip("请求游戏数据");

            if (ApplicationSettings.Instance.isMock)
            {

                yield return RequestSlotSpinFromMock02(TotalBet,() =>
                {
                    isNext = true;
                }, (err) =>
                {
                    errMsg = err;
                    isNext = true;
                    isBreak = true;
                });

            }
            else
            {
                /*
                yield return RequestSlotSpinFromMachine(() =>
                {
                    isNext = true;
                }, (err) =>
                {
                    errMsg = err;
                    isNext = true;
                    isBreak = true;
                });*/
            }

            yield return new WaitUntil(() => isNext == true);
            isNext = false;

            if (isBreak)
            {
                if (errorCallback != null)
                    errorCallback.Invoke(new object[] { ErrorCode.ERROR, errMsg });
                yield break;
            }



            if (SBoxModel.Instance.isJackpotOnLine)
            {
                if (ApplicationSettings.Instance.isMock)
                {
                    /**/
                    // 模拟在线彩金中奖数据
                    MachineDataManager02.Instance.RequestJackpotOnLine(new JackBetInfo
                    {
                        seat = 1,  // 固定死
                        bet = (int)SBoxModel.Instance.CoinInScale,  // 总压注
                        betPercent = 100, // 固定死
                        scoreRate = SBoxModel.Instance.jackpotScoreRate,      //10000,  // 1 除以 币值 乘以 1000 整形   （联网彩金分值比 ：只能该币值）
                        JPPercent = SBoxModel.Instance.jackpotPercent,    //5  // 千分之几（1 - 100 可调 ；名称： 联网彩金比（千分）  ）
                    });

                }
                else
                {
                    /*
                    JackpotOnLineManager.Instance.RequestsJackpotOnLineData(
                        new JackBetInfo
                        {
                            seat = 1,  // 固定死
                            bet = (int)ContentModel.Instance.totalBet,  // 总压注
                            betPercent = 100, // 固定死
                            scoreRate =  _consoleBB.Instance.jackpotScoreRate,      //10000,  // 1 除以 币值 乘以 1000 整形   （联网彩金分值比 ：只能该币值）
                            JPPercent =  _consoleBB.Instance.jackpotPercent,    //5  // 千分之几（1 - 100 可调 ；名称： 联网彩金比（千分）  ）
                        },
                        null, null
                    );
                    */
                }
            }


            TestManager.Instance.ShowTip("滚轮开始滚动");

            slotMachineCtrl.BeginSpin();

            /*
            if (ContentModel.Instance.isReelsSlowMotion)
            {
            }
            else
            {
            }
            */

            if (slotMachineCtrl.isStopImmediately)
            {
                //reelsTurnType = ReelsTurnType.Once;

                if (coReelsTurn != null) mono.StopCoroutine(coReelsTurn);
                coReelsTurn = mono.StartCoroutine(slotMachineCtrl.TurnReelsOnce(ContentModel.Instance.strDeckRowCol,
                    () =>
                    {
                        isNext = true;
                    }));


                yield return new WaitUntil(() => isNext == true);
                isNext = false;
            }
            else
            {

                if (coReelsTurn != null) mono.StopCoroutine(coReelsTurn);
                coReelsTurn = mono.StartCoroutine(slotMachineCtrl.TurnReelsNormal(ContentModel.Instance.strDeckRowCol,
                    () =>
                    {
                        isNext = true;
                    }));


                yield return new WaitUntil(() => isNext == true || slotMachineCtrl.isStopImmediately == true);
                isNext = false;

                // 等待移动结束
                if (slotMachineCtrl.isStopImmediately && isNext == false)
                {
                    if (coReelsTurn != null) mono.StopCoroutine(coReelsTurn);
                    coReelsTurn = mono.StartCoroutine(slotMachineCtrl.ReelsToStopOrTurnOnce(() =>
                    {
                        isNext = true;
                    }));


                    yield return new WaitUntil(() => isNext == true);
                    isNext = false;
                }
            }




            List<SymbolWin> winList = ContentModel.Instance.winList;
            long allWinCredit = 0;
            if (winList.Count > 0) //if (winList.Count > 0 || ContentModel.Instance.bonusResult.Count > 0)
            {
                

                long totalWinLineCredit = 0;
                totalWinLineCredit = slotMachineCtrl.GetTotalWinCredit(winList);
                allWinCredit = totalWinLineCredit;


                /*
                WinLevelType winLevelType = GetBigWinType();

                if (winList.Count > 0 && winLevelType == WinLevelType.None)
                {
                    yield return ShowWinListOnceAtNormalSpin009(winList);
                }


                if (winLevelType != WinLevelType.None)
                {
                    slotMachineCtrl.ShowSymbolWinDeck(slotMachineCtrl.GetTotalSymbolWin(winList), true);
                    // 大奖弹窗
                    yield return WinPopup(winLevelType, ContentModel.Instance.baseGameWinCoins);

                    slotMachineCtrl.CloseSlotCover();

                    slotMachineCtrl.SkipWinLine(false);

                }
                else
                {
                    bool isAddToCredit = totalWinLineCredit > TotalBet * 4;
                    slotMachineCtrl.SendPrepareTotalWinCreditEvent(totalWinLineCredit, isAddToCredit);
                }
                */

                yield return ShowWinListOnceAtNormalSpin(winList);

               //bool isAddToCredit = totalWinLineCredit > TotalBet * 4;
               //slotMachineCtrl.SendPrepareTotalWinCreditEvent(totalWinLineCredit, isAddToCredit);

                slotMachineCtrl.SendTotalWinCreditEvent(allWinCredit);

                yield return new WaitForSeconds(0.8f);

                //加钱动画
                // MainBlackboardController.Instance.AddMyTempCredit(allWinCredit, true, isAddCreditAnim);
            }



            #region 中游戏彩金

            bool isHitJackpot = ContentModel.Instance.jpGameWinDic.Count > 0;
            Dictionary<int,JackpotWinInfo> jpRes = ContentModel.Instance.jpGameWinDic;
            //List<float> jpCredit = ContentModel.Instance.jpGameWhenCreditLst;

            if (jpRes.Count > 0)
            {
                smConfig.SelectWinEffectSetting("jackpot_game");

                slotMachineCtrl.SkipWinLine(true);
                //int SymbolNumber = GetJpSymbolNumber(jpRes[0]);
                //slotMachineCtrl.ShowSpecailSymbolEffectBySetting(new List<int>() { SymbolNumber }, SymbolEffectType.SymbolHit);
                slotMachineCtrl.ShowSpecailSymbolWinBySetting(ContentModel.Instance.jpGameSymbolWin, SymbolEffectType.SymbolHit);
                yield return slotMachineCtrl.SlotWaitForSeconds(1.5f);


                JackpotWinInfo jpWin = jpRes.ElementAt(0).Value;
                //jpRes.RemoveAt(0);

                Action onJPPoolSubCredit = () =>
                {
                   // SetJPCurCredit(jpWin);
                };

                allWinCredit += (long)jpWin.winCredit;

                //DebugUtils.LogError($"中游戏彩金数据： JackpotWinInfo = {JsonConvert.SerializeObject(jpWin)} ");
                PageManager.Instance.OpenPageAsync(PageName.RichLegend2001003PopupJackpotGame,
                    /*new EventData<Dictionary<string, object>>("", new Dictionary<string, object>
                    {
                        ["jackpotId"] = jpWin.id,
                        ["totalEarnCredit"] = jpWin.winCredit,
                        ["onJPPoolSubCredit"] = onJPPoolSubCredit,
                    }),*/
                    new InParamsPopupJackpotGame()
                    {
                        jackpotId = jpWin.id,
                        totalEarnCredit = jpWin.winCredit,
                        onJPPoolSubCredit = onJPPoolSubCredit,
                    },
                    (res) =>
                    {
                        isNext = true;
                    });


                yield return new WaitUntil(() => isNext == true);
                isNext = false;

                // 总线赢分（同步？？）
                slotMachineCtrl.SendTotalWinCreditEvent(allWinCredit);
            }

            /*#seaweed#废弃
            if (jpRes.Count > 0)
            {
                slotMachineCtrl.SkipWinLine(true);
                slotMachineCtrl.ShowSymbolEffect(TagPoolObject.SymbolHit,new List<int>() {12},true,12,true);
                yield return slotMachineCtrl.SlotWaitForSeconds(1.5f);
                slotMachineCtrl.SkipWinLine(true);
                slotMachineCtrl.ShowSymbolEffect(TagPoolObject.SymbolAppear, new List<int>() { 12 }, true, 12, true);
            }
            while (jpRes.Count > 0)
            {
                JackpotWinInfo jpWin = jpRes[0];
                jpRes.RemoveAt(0);

                Action onJPPoolSubCredit = () =>
                {
                    SetJPCurCredit(jpWin);
                };

                allWinCredit += (long)jpWin.winCredit;

                DebugUtils.LogError($"中游戏彩金数据： JackpotWinInfo = {JsonConvert.SerializeObject(jpWin)} ");
                PageManager.Instance.OpenPageAsync(PageName.PusherEmperorsReinPopupJackpotGame,
                    new EventData<Dictionary<string, object>>("", new Dictionary<string, object>
                    {
                        ["jackpotType"] = jpWin.name,
                        ["totalEarnCredit"] = jpWin.winCredit,
                        ["onJPPoolSubCredit"] = onJPPoolSubCredit,
                        ["jpCredit"] = jpCredit,
                    }),
                    (res) =>
                    {
                        isNext = true;
                    });


                yield return new WaitUntil(() => isNext == true);
                isNext = false;

                // 总线赢分（同步？？）
                slotMachineCtrl.SendTotalWinCreditEvent(allWinCredit);
            }
            */

            //bool isHitJackpot = ContentModel.Instance.jpGameWinLst.Count > 0;
            //List<JackpotWinInfo> jpRes = ContentModel.Instance.jpGameWinLst;




            #endregion



            // #中大厅彩金

            #region 中大厅彩金

            /*
            while (ContentModel.Instance.jpOnlineWin.Count > 0)
            {
                WinJackpotInfo data = ContentModel.Instance.jpOnlineWin[0];
                ContentModel.Instance.jpOnlineWin.RemoveAt(0);

                //long fromCredit = data.win < 1000 ? data.win : data.win - 1000;

                long winCredit = SBoxModel.Instance.CoinInScale * data.win;
                allWinCredit += winCredit;

                PageManager.Instance.OpenPageAsync(PageName.PusherEmperorsReinPopupJackpotOnline,
                    new EventData<Dictionary<string, object>>("", new Dictionary<string, object>
                    {
                        ["toCredit"] = winCredit,
                        ["jackpotType"] = data.jackpotId,
                        //["fromCredit"] = (long)fromCredit
                    }),
                    (res) =>
                    {
                        isNext = true;
                    });

                yield return new WaitUntil(() => isNext == true);
                isNext = false;

                // 总线赢分（同步？？）
                slotMachineCtrl.SendTotalWinCreditEvent(allWinCredit);

                //MainBlackboardController.Instance.AddMyTempCredit(winCredit, true, isAddCreditAnim);
            }
            */

            #endregion



            // #中小游戏(火车)
            if(ContentModel.Instance.bonusResults.ContainsKey(1) && ContentModel.Instance.bonusSymbolWin != null)
            {

                smConfig.SelectWinEffectSetting("bonus1");


                slotMachineCtrl.SkipWinLine(true);
                slotMachineCtrl.ShowSpecailSymbolWinBySetting(ContentModel.Instance.bonusSymbolWin, SymbolEffectType.SymbolHit);
                yield return slotMachineCtrl.SlotWaitForSeconds(2.5f);

                long earnCoins = (long)ContentModel.Instance.bonusSymbolWin.earnCredit;
                allWinCredit += earnCoins;

                //DebugUtils.LogError($"中游戏彩金数据： JackpotWinInfo = {JsonConvert.SerializeObject(jpWin)} ");
                PageManager.Instance.OpenPageAsync(PageName.RichLegend2001003PageGameBonus1,
                    /*
                    new EventData<Dictionary<string, object>>("", new Dictionary<string, object>
                    {
                        ["totalEarnCoins"] = earnCoins,
                        //["onJPPoolSubCredit"] = onJPPoolSubCredit,
                    }),
                    */

                    new InParamsPageGameBonus1()
                    {
                        totalEarnCoins = earnCoins
                    },

                    (res) =>
                    {
                        isNext = true;
                    });

                yield return new WaitUntil(() => isNext == true);
                isNext = false;

                // 总线赢分（同步？？）
                slotMachineCtrl.SendTotalWinCreditEvent(allWinCredit);

            }

            // 小游戏Bonus - 筛子
            if (ContentModel.Instance.bonusResults.ContainsKey(2))
            {
                smConfig.SelectWinEffectSetting("bonus1");

                slotMachineCtrl.SkipWinLine(true);
                slotMachineCtrl.ShowSpecailSymbolWinBySetting(ContentModel.Instance.bonusSymbolWin, SymbolEffectType.SymbolHit);
                yield return slotMachineCtrl.SlotWaitForSeconds(2.5f);

                int earnCoins = (int)ContentModel.Instance.bonusResults[2]["totalWinCoins"];
                allWinCredit += earnCoins;

                PageManager.Instance.OpenPageAsync(PageName.RichLegend2001003PageGameBonus2,
                    /*
                     new EventData<Dictionary<string, object>>("", new Dictionary<string, object>()
                    {
                        ["bonusResults"] = ContentModel.Instance.bonusResults[2]
                    }),
                    */

                    new InParamsPageGameBonus2()
                    {
                        bonusResults = ContentModel.Instance.bonusResults[2]
                    },

                    (res) =>
                    {
                        isNext = true;
                    });

                yield return new WaitUntil(() => isNext == true);
                isNext = false;

                PageManager.Instance.OpenPageAsync(PageName.RichLegend2001003PopupBigWin, 
                    /*new EventData<Dictionary<string,object>>("",new Dictionary<string, object>()
                    {
                        ["totalWinCpions"] = (int)earnCoins,
                    }), 
                    */
                    new InParamsPopupBigWin()
                    {
                        totalWinCpions = (int)earnCoins,
                    },
                    (res) =>
                    {
                        isNext = true;
                    });

                yield return new WaitUntil(() => isNext == true);
                isNext = false;

                // 总线赢分（同步？？）
                slotMachineCtrl.SendTotalWinCreditEvent(allWinCredit);
            }



            // 本剧同步玩家金钱
            //MainBlackboardController.Instance.SyncMyTempCreditToReal(false);
            //MainBlackboardController.Instance.SyncMyTempCreditToReal(true);

            //DebugUtils.LogWarning($"结束分数： SBoxModel Credit = {SBoxModel.Instance.myCredit}   uiCredit={MainModel.Instance.myCredit}");




            // #本局结束
            MachineDataManagerG2001003.Instance.RequestCoinPushSpinEnd(res1 =>
            {
                JSONNode data = JSONObject.Parse((string)res1);

                int code = (int)data["code"];

                if (code != 0)
                {
                    DebugUtils.LogError($" CoinPushSpinEnd(20102) : [0]= {code}");
                }

                isNext = true;
            });

            yield return new WaitUntil(() => isNext == true);
            isNext = false;


            // #掉币
            if (allWinCredit > 0 )
            {
                CheckCoinHit(allWinCredit);

                yield return ShowWinListCoinCountDown(winList, allWinCredit, isHitJackpot);

                //yield return new WaitForSeconds(1f);
            }


            // #读取玩家积分变化
            MachineDataManagerG2001003.Instance.RequestGetMyCredit((int)allWinCredit, (object res) =>
            {
                skipGetMyCreditTimeS = Time.unscaledTime + 3f;

                int curMyCredit = (int)res;
                if (SBoxModel.Instance.myCredit != curMyCredit)
                {
                    SBoxModel.Instance.myCredit = curMyCredit;
                    MainBlackboardController.Instance.SyncMyTempCreditToReal(true,true);
                }
            });


            /*
            // 中后台彩金
            if (ContentModel.Instance.isHitJpGame1JpGame2)
            {
                yield return RequestGrandMahorWin(allWinCredit);
            }
            */




            // 即中即退
            //yield return CoinOutImmediately(allWinCredit);

            /*
            // 免费游戏
            if (ContentModel.Instance.isFreeSpinTrigger)
            {

                GameSoundHelper.Instance.PlaySoundEff(SoundKey.FreeSpinTriggerBG);
                yield return new WaitForSeconds(2.6f);
                yield return FreeSpinTrigger(null, errorCallback);
            }
            */


            DebugUtils.Log("进入空闲模式！！！");
            // 进入空闲模式
            ContentModel.Instance.gameState = GameState.Idle;
            //GameSoundHelper.Instance.PlaySoundEff(SoundKey.SpinBGIdle);

            /**/
            //空闲模式下-轮播赢线
            if (winList.Count > 0 && !ContentModel.Instance.isAuto && !ContentModel.Instance.isFreeSpinTrigger)
            {
                if (coGameIdel != null) mono.StopCoroutine(coGameIdel);
                coGameIdel = mono.StartCoroutine(GameIdle(winList));
            }
            


            if (successCallback != null)
                successCallback.Invoke();
        }




        IEnumerator RequestSlotSpinFromMock02(long totalBet, Action successCallback = null, Action<string> errorCallback = null)
        {
            bool isNext = false;
            bool isBreak = false;


            JSONNode resNode = null;

            MockDataManagerG2001003.Instance.RequestSlotSpinFromMock(totalBet, (res) =>
            {
                resNode = res;
                isNext = true;
            }, (err) =>
            {
                errorCallback?.Invoke(err.msg);
                isNext = true;
                isBreak = true;
            });


            yield return new WaitUntil(() => isNext == true);
            isNext = false;


            int code = (int)resNode["code"]; //:0表示成功，-1表示传参失败

            if (code != 0)
            {
                TestManager.Instance.ShowTip($"Spin数据有误  code:{code}");
                DebugUtils.LogError($"Spin数据有误  code:{code}");
                errorCallback?.Invoke($"Spin数据有误 code:{code}");
                yield break;
            }

            if (isBreak) yield break;

            // 减去押注积分
            MachineDataManagerG2001003.Instance.myCreditMock -= (int)totalBet;
            MainBlackboardController.Instance.MinusMyTempCredit(totalBet, true);


            SBoxGameState gameState = (SBoxGameState)((int)resNode["gameState"]);
            MockJackpotResult jpType = MockJackpotResult.None;
            switch (gameState)
            {
                case SBoxGameState.GSJpMega:
                    jpType = MockJackpotResult.Jp4;
                    break;
                case SBoxGameState.GSJp1:
                    jpType = MockJackpotResult.Jp1;
                    break;
                case SBoxGameState.GSJp2:
                    jpType = MockJackpotResult.Jp2;
                    break;
                case SBoxGameState.GSJp3:
                    jpType = MockJackpotResult.Jp3;
                    break;
            }
            JackpotRes02 jpGameRes = null;
            MachineDataManager02.Instance.RequestJackpotGame(jpType, (res) =>
            {
                jpGameRes = (JackpotRes02)res;

                isNext = true;
            }, (err) =>
            {
                if (errorCallback != null)
                    errorCallback.Invoke(err.msg);
                isNext = true;
                isBreak = true;
            });


            yield return new WaitUntil(() => isNext == true);
            isNext = false;

            if (isBreak) yield break;

            // 解析数据
            MockDataManagerG2001003.Instance.ParseSlotSpin(totalBet, resNode, jpGameRes);


            // 数据入库
            MockDataManagerG2001003.Instance.Record();

            // 游戏彩金滚轮
            SetUIJackpotGameReel();


            // 数据上报
            //MockDataManagerG2001003.Instance.Report();


            if (successCallback != null)
                successCallback.Invoke();
        }




        public void SetUIJackpotGameReel()
        {
            JackpotRes02 info = ContentModel.Instance.jpGameRes;

            ContentModel.Instance.uiJPGrand.nowCredit = uiJP1Ctrl.nowData;
            ContentModel.Instance.uiJPMajor.nowCredit = uiJP2Ctrl.nowData;
            ContentModel.Instance.uiJPMinor.nowCredit = uiJP3Ctrl.nowData;
            ContentModel.Instance.uiJPMega101.nowCredit = uiJPMegaCtrl.nowData;

            ContentModel.Instance.uiJPGrand.curCredit = info.GetCurJackpot(0);
            ContentModel.Instance.uiJPMajor.curCredit = info.GetCurJackpot(1);
            ContentModel.Instance.uiJPMinor.curCredit = info.GetCurJackpot(2);
            ContentModel.Instance.uiJPMega101.curCredit = info.GetCurJackpot(3);

            // 游戏滚轮显示
            uiJP1Ctrl.SetData(ContentModel.Instance.jpGameWhenCreditDic[0]);
            uiJP2Ctrl.SetData(ContentModel.Instance.jpGameWhenCreditDic[1]);
            uiJP3Ctrl.SetData(ContentModel.Instance.jpGameWhenCreditDic[2]);
            uiJPMegaCtrl.SetData(ContentModel.Instance.jpGameWhenCreditDic[3]);

        }

        WinLevelType GetBigWinType(long? winCredit = null)
        {
            long baseGameWinCredit = winCredit != null ? (long)winCredit : ContentModel.Instance.baseGameWinCoins;
            List<WinMultiple> winMultipleList = ContentModel.Instance.winLevelMultiple;

            WinLevelType winLevelType = WinLevelType.None;
            for (int i = 0; i < winMultipleList.Count; i++)
            {
                if (baseGameWinCredit > winMultipleList[i].multiple)
                {
                    winLevelType = winMultipleList[i].winLevelType;
                }
            }

            return winLevelType;
        }

        void OnGameReset()
        {
            if (coGameIdel != null) 
                mono.StopCoroutine(coGameIdel);
            coGameIdel = null;

            //smConfig.SelectReelSetting("regular");
            //smConfig.SelectWinEffectSetting("regular");

            // 设置滚轮配置
            if (ContentModel.Instance.isAuto)
            {
                smConfig.SelectReelSetting("auto");
                smConfig.SelectWinEffectSetting("auto");
            }
            else
            {
                smConfig.SelectReelSetting("regular");
                smConfig.SelectWinEffectSetting("regular");
            }


            slotMachineCtrl.isStopImmediately = false;
            slotMachineCtrl.CloseSlotCover();
            //isEffectSlowMotion2 = false;
            //isStoppedSlotMachine = false;
            //goExpectation.SetActive(false);
            //ComReelEffect2.visible = false;
            slotMachineCtrl.SkipWinLine(true);



            txtRewardCoins.text = 0.ToString();
            txtRemainCoins.text = 0.ToString();
        }

        IEnumerator ShowWinListOnceAtNormalSpin(List<SymbolWin> winList)
        {
            if (smConfig.IsWETotalWinLine)
            {
                yield return slotMachineCtrl.ShowSymbolWinBySetting(
                    slotMachineCtrl.GetTotalSymbolWin(winList), true, SpinWinEvent.TotalWinLine);
            }
            else
            {

                //停止特效显示
                slotMachineCtrl.SkipWinLine(false);

                int idx = 0;
                while (idx < winList.Count)
                {
                    SymbolWin curSymbolWin = winList[idx];
                    /*## 是否是五连线
                    if (slotMachineCtrl.Check5kind(curSymbolWin) && !slotMachineCtrl.isStopImmediately)
                    {
                        yield return Show5KindPoup(curSymbolWin);
                    }*/
                    yield return slotMachineCtrl.ShowSymbolWinBySetting(curSymbolWin, true, SpinWinEvent.SingleWinLine);
                    ++idx;
                }

            }

            //停止特效显示
            slotMachineCtrl.SkipWinLine(false);

            slotMachineCtrl.CloseSlotCover();

        }




        private IEnumerator GameIdle(List<SymbolWin> winList)
        {
            if (winList.Count == 0)
            {
                yield break;
            }

            smConfig.SelectWinEffectSetting("idle");

            yield return new WaitForSeconds(3f);

            yield return slotMachineCtrl.ShowWinListAwayDuringIdle(winList);
        }












        IEnumerator ShowWinListCoinCountDown(List<SymbolWin> winList, long totalWinLineCredit, bool isHitJackpot)
        {
            bool isNext = false;

            //if (!isHitJackpot)
                slotMachineCtrl.ShowSymbolWinDeck(slotMachineCtrl.GetTotalSymbolWin(winList), true);

            yield return ShowCoinCountDown(totalWinLineCredit);

            //停止特效显示
            slotMachineCtrl.SkipWinLine(false);
            //显示遮罩
            slotMachineCtrl.CloseSlotCover();

        }

        IEnumerator ShowCoinCountDown(long totalWinLineCredit)
        {
            bool isNext = false;
            int curCoinCountDown = (int)totalWinLineCredit;
            int lastCoinCountDown = curCoinCountDown;
            float lastRunTimeS = Time.unscaledTime;


            bool isStart = true;
            int tryGetStartCount = 50;




            goEffectCoinDrop.visible = true;

            while (Time.unscaledTime - lastRunTimeS < 10) // 10秒     //1800 = 60 * 30 = 30分钟
            {

                // 防止第一次读到 0 立马退出。（算法卡掉币赋值有延时）
                while (isStart && --tryGetStartCount > 0)
                {
                    MachineDataManagerG2001003.Instance.RequestCoinCountDown((int)totalWinLineCredit, (result) =>
                    {
                        curCoinCountDown = (int)result;

                        if (curCoinCountDown != 0)  // 首次读到币
                        {
                            isStart = false;
                            //DebugUtils.Save($" CoinCountDown start cout : {curCoinCountDown}");
                        }

                        // 金币不发生变化，延时10秒退出循环
                        if (lastCoinCountDown != curCoinCountDown)
                        {
                            lastCoinCountDown = curCoinCountDown;
                            lastRunTimeS = Time.unscaledTime;
                        }
                        txtRewardCoins.text = (totalWinLineCredit-curCoinCountDown).ToString();
                        txtRemainCoins.text = curCoinCountDown.ToString();
                        //CollectGoldCoinsController.DiaoJinBin(curCoinCountDown);
                        isNext = true;
                    });

                    yield return new WaitUntil(() => isNext == true);
                    isNext = false;
                }



                MachineDataManagerG2001003.Instance.RequestCoinCountDown((int)totalWinLineCredit, (result) =>
                {
                    curCoinCountDown = (int)result;

                    // 金币不发生变化，延时10秒退出循环
                    if (lastCoinCountDown != curCoinCountDown)
                    {
                        lastCoinCountDown = curCoinCountDown;
                        lastRunTimeS = Time.unscaledTime;
                    }

                    txtRewardCoins.text = (totalWinLineCredit - curCoinCountDown).ToString();
                    txtRemainCoins.text = curCoinCountDown.ToString();
                    isNext = true;
                });

                yield return new WaitUntil(() => isNext == true);
                isNext = false;

                if (curCoinCountDown == 0)
                    break;
            }

            goEffectCoinDrop.visible = false;

            //if (curCoinCountDown > 0) DebugUtils.LogError($"掉币数量没刷新，数值停留在： {curCoinCountDown}");


            //CollectGoldCoinsController.Close();
        }




        void CheckCoinHit(long allWinCredit) => rewardPanelCtrl.winCoins = (int)allWinCredit;




    }
}
