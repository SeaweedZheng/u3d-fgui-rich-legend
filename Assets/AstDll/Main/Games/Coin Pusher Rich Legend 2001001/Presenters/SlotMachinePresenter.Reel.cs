using Sirenix.OdinInspector;
using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameMaker;
public partial class SlotMachinePresenter
{




    void ClearReelTween(int reelIndex)
    {
        view.ClearTween(reelIndex);
        ClearReelCo(reelIndex);
    }

    void ClearReelCo(int reelIndex)
    {
        if (coReelToStopLst[reelIndex] != null) StopCoroutine(coReelToStopLst[reelIndex]);
        coReelToStopLst[reelIndex] = null;

        if (coReelTurnLst[reelIndex] != null) StopCoroutine(coReelTurnLst[reelIndex]);
        coReelTurnLst[reelIndex] = null;
    }
    public void SetReelsDeck(string strDeckRowCol = "1,1,1,1,1#2,2,2,2,2#3,3,3,3,3")
    {
        //停止特效显示
        SkipWinLine(false);

        reelsResult = SlotTool.GetDeckColRow02(strDeckRowCol);

        //这个还要判断特殊图标 如果有还需要改变滚轮滚的次数 还有特殊表现效果
        //模拟图标
        for (int col = 0; col < config.Column; col++)
        {
            view.SetReelDeck(col, reelsResult[col]);
        }
    }
    void StartTurn(int reelIndex, int targetRollTime, Action reelStopCallback)
    {
        this.needRollTimeLst[reelIndex] = isReelPointeringLst[reelIndex] ? 1 : targetRollTime;
        this.curRollTimeLst[reelIndex] = 0;
        this.reelStopCallbackLst[reelIndex] = reelStopCallback;

        ClearReelTween(reelIndex);
        if (coReelTurnLst[reelIndex] != null) StopCoroutine(coReelTurnLst[reelIndex]);
        coReelTurnLst[reelIndex] = StartCoroutine(_ReelTurn(reelIndex));
    }


    IEnumerator StartTurnReels()
    {

        int reelsCount = config.Column;

        bool isNext = false;

        for (int reelIdx = 0; reelIdx < config.Column; reelIdx++)
        {
            if (config.GetTimeTurnStartDelay(reelIdx) > 0)
            {
                yield return new WaitForSeconds(config.GetTimeTurnStartDelay(reelIdx));
            }

            int _reelIdx = reelIdx;

            StartTurn(
                reelIdx,
                config.GetNumReelTurn(reelIdx) + reelIdx * config.GetNumReelTurnGap(reelIdx),
                () =>
                {
                    onSlotDetailEvent?.Invoke(new EventData<int>(SlotMachineEvent.PrepareStoppedReel, _reelIdx));

                    if (isSymbolAppearEffectWhenReelStop)
                        view.SymbolAppearEffect(_reelIdx);

                    if (--reelsCount <= 0)
                    {
                        isNext = true;
                    }

                }
            );
        }

        yield return new WaitUntil(() => isNext == true);
        isNext = false;

        for (int i = 0; i < reelStateLst.Count; i++)
        {
            reelStateLst[i] = ReelState.Idle;
        }

        onSlotEvent?.Invoke(new EventData(SlotMachineEvent.StoppedSlotMachine));

    }

    IEnumerator _ReelTurn(int reelIndex, bool isOnce = false)
    {
        if (needRollTimeLst[reelIndex] == 0) yield break;

        bool isNext = false;
        reelStateLst[reelIndex] = ReelState.StartTurn;

        if (config.GetTimeReboundStart(reelIndex) > 0)
        {
            yield return view.Rebound(reelIndex,
                config.GetOffsetYReboundStart(reelIndex),
                config.GetTimeReboundStart(reelIndex)
            );
        }

        while (curRollTimeLst[reelIndex] < needRollTimeLst[reelIndex])
        {
            view.ResetIconData(reelIndex);

            if (curRollTimeLst[reelIndex] == needRollTimeLst[reelIndex] - 1)
            {
                reelStateLst[reelIndex] = ReelState.StartStop;
                //这里开始设置结果
                view.SetReelEndResult(reelIndex, reelsResult[reelIndex]);
            }

            view.MoveY(reelIndex, 0, config.GetTimeTurnOnce(reelIndex), () => { isNext = true; });
            //reelTweenLst[reelIndex] = TweenUtils.DOLocalMoveY(anchorSymbolsLst[reelIndex], 0, GetTimeTurnOnce(reelIndex), EaseType.Linear, () => { isNext = true; });

            yield return new WaitUntil(() => isNext);
            isNext = false;


            view.ClearTween(reelIndex);
            //reelTweenLst[reelIndex] = null;

            if (++curRollTimeLst[reelIndex] >= needRollTimeLst[reelIndex])
            {
                break;
            }
        }

        if (config.GetTimeReboundEnd(reelIndex) > 0)
        {
            yield return view.Rebound(
                reelIndex,
                config.GetOffsetYReboundEnd(reelIndex),
                config.GetTimeReboundEnd(reelIndex)
            );
        }
        reelStateLst[reelIndex] = ReelState.EndStop;
        if (reelIndex < reelStopCallbackLst.Count)
            reelStopCallbackLst[reelIndex]?.Invoke();
    }


    public IEnumerator ReelsToStopOrTurnOnce(Action finishCallback)
    {

        int reelsCount = config.Column;

        bool isNext = false;

        for (int reelIdx = 0; reelIdx < config.Column; reelIdx++)
        {
            if (reelStateLst[reelIdx] == ReelState.EndStop)
            {
                reelsCount--;
                continue;
            }

            if (reelStateLst[reelIdx] == ReelState.Idle)
            {
                if (config.GetTimeTurnStartDelay(reelIdx) > 0)
                {
                    yield return new WaitForSeconds(config.GetTimeTurnStartDelay(reelIdx));
                }
            }

            int _reelIdx = reelIdx;

            ReelToStopOrTurnOnce(reelIdx,
                () =>
                {
                    onSlotDetailEvent?.Invoke(new EventData<int>(SlotMachineEvent.PrepareStoppedReel, _reelIdx));

                    if (isSymbolAppearEffectWhenReelStop)
                        view.SymbolAppearEffect(reelIdx);

                    if (--reelsCount <= 0)
                    {
                        isNext = true;
                    }
                }
            );
        }

        yield return new WaitUntil(() => isNext == true);
        isNext = false;


        for (int i = 0; i < reelStateLst.Count; i++)
        {
            reelStateLst[i] = ReelState.Idle;
        }

        onSlotEvent?.Invoke(new EventData(SlotMachineEvent.StoppedSlotMachine));

        finishCallback?.Invoke();
    }



    public void ReelToStopOrTurnOnce(int reelIndex, Action action = null)
    {
        this.reelStopCallbackLst[reelIndex] = action;

        if (reelStateLst[reelIndex] == ReelState.StartStop)
            return;

        if (reelStateLst[reelIndex] == ReelState.EndStop)
            return;


        if (reelStateLst[reelIndex] == ReelState.Idle)
        {
            StartTurn(reelIndex, 1, action);
        }
        else if (reelStateLst[reelIndex] == ReelState.StartTurn)
        {
            ReelToStop(reelIndex);
        }
    }

    void ReelToStop(int reelIndex)
    {
        if (reelStateLst[reelIndex] == ReelState.StartTurn)
        {
            reelStateLst[reelIndex] = ReelState.StartStop;
            ClearReelTween(reelIndex);

            if (coReelToStopLst[reelIndex] != null)
                StopCoroutine(coReelToStopLst[reelIndex]);
            coReelToStopLst[reelIndex] = StartCoroutine(_ReelToStop(reelIndex));
        }
    }
    IEnumerator _ReelToStop(int reelIndex)
    {
        bool isNext = false;
        reelStateLst[reelIndex] = ReelState.StartStop;

        view.SetReelEndResult(reelIndex, reelsResult[reelIndex]);

        view.MoveY(reelIndex, 0, config.GetTimeTurnOnce(reelIndex), () => { isNext = true; });
        //reelTweenLst[reelIndex] = TweenUtils.DOLocalMoveY(anchorSymbolsLst[reelIndex], 0,  GetTimeTurnOnce(reelIndex), EaseType.Linear, () => { isNext = true; });

        yield return new WaitUntil(() => isNext);
        isNext = false;
        //reelTweenLst[reelIndex] = null;

        view.ClearTween(reelIndex);

        if (config.GetTimeReboundEnd(reelIndex) > 0)
        {
            yield return view.Rebound(
                reelIndex,
                config.GetOffsetYReboundEnd(reelIndex),
                config.GetTimeReboundEnd(reelIndex)
            );
        }

        reelStateLst[reelIndex] = ReelState.EndStop;
        if (reelIndex < reelStopCallbackLst.Count)
            reelStopCallbackLst[reelIndex]?.Invoke();
    }



    public IEnumerator TurnReelsOnce(string strDeckRowCol = "1,1,1,1,1#2,2,6,2,2#3,3,3,3,3", Action finishCallback = null)
    {

        SkipWinLine(false);

        reelsResult = SlotTool.GetDeckColRow02(strDeckRowCol);

        yield return ReelsToStopOrTurnOnce(null);
        // 算分

        finishCallback?.Invoke();
    }


    public IEnumerator TurnReelsNormal(string strDeckRowCol = "1,1,1,1,1#2,2,6,2,2#3,3,3,3,3", Action finishCallback = null)
    {
        //停止特效显示
        SkipWinLine(false);

        reelsResult = SlotTool.GetDeckColRow02(strDeckRowCol);


        yield return StartTurnReels();

        finishCallback?.Invoke();
    }




    #region 测试
    [Button]
    void TestTuren()
    {
        StartCoroutine(TurnReelsNormal());
    }


    #endregion
}
