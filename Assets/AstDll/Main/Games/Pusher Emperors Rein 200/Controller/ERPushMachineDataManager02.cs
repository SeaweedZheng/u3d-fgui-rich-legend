using SBoxApi;
using System;
using System.Collections.Generic;

namespace PusherEmperorsRein
{

    public partial class ERPushMachineDataManager02 : ProxyHelper<ERPushMachineDataManager02>
    {

        bool isMock => ApplicationSettings.Instance.isMock;



        protected void Awake()
        {
            receiveOvertimeS = 1.5f;
            isDebugLog = true;
            prefix = "【SBox】";
        }



        protected  void Start()
        {


            // ===推币机游戏数据
            EventCenter.Instance.AddEventListener<string>(SBoxEventHandle.SBOX_COIN_PUSH_SPIN, OnResponseCoinPushSpin);
            EventCenter.Instance.AddEventListener<string>(SBoxEventHandle.SBOX_COIN_PUSH_SPIN_END, OnResponseCoinPushSpinEnd);
            EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_COIN_PUSH_BEGIN_TURN, OnResponseBeginTurn);

            EventCenter.Instance.AddEventListener<string>(SBoxEventHandle.SBOX_COIN_PUSH_GET_JP_CONTRIBUTION, OnResponseGetJpMajorGrandContribution);
            EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_COIN_PUSH_RETURN_JP_CONTRIBUTION, OnResponseReturnMajorGrandContribution);
            EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_COIN_PUSH_SET_MAJOR_GRAND_WIN, OnResponseSetMajorGrandWin);
        }


        protected override void OnDestroy()
        {

            // ===推币机游戏数据
            EventCenter.Instance.RemoveEventListener<string>(SBoxEventHandle.SBOX_COIN_PUSH_SPIN, OnResponseCoinPushSpin);
            EventCenter.Instance.RemoveEventListener<string>(SBoxEventHandle.SBOX_COIN_PUSH_SPIN_END, OnResponseCoinPushSpinEnd);
            EventCenter.Instance.RemoveEventListener<int>(SBoxEventHandle.SBOX_COIN_PUSH_BEGIN_TURN, OnResponseBeginTurn);


            EventCenter.Instance.RemoveEventListener<string>(SBoxEventHandle.SBOX_COIN_PUSH_GET_JP_CONTRIBUTION, OnResponseGetJpMajorGrandContribution);
            EventCenter.Instance.RemoveEventListener<int>(SBoxEventHandle.SBOX_COIN_PUSH_RETURN_JP_CONTRIBUTION, OnResponseReturnMajorGrandContribution);
            EventCenter.Instance.RemoveEventListener<int>(SBoxEventHandle.SBOX_COIN_PUSH_SET_MAJOR_GRAND_WIN, OnResponseSetMajorGrandWin);

            base.OnDestroy();

        }






        ///////////////////////////游戏数据
        ///<summary> 获取游戏彩金 </summary>
        public int RequestCoinPushSpin(Action<object> finishCallback, string mark = null)
        {
            int seqId = OnRequestBefore(SBoxEventHandle.SBOX_COIN_PUSH_SPIN, null, finishCallback, null, mark);

            if (isMock)
            {
                //OnMockActive(null);

                DebugUtils.Log("待实现");
            }
            else
            {
                SBoxIdea.CoinPushGetSpinResult(0);
            }
            return seqId;

        }
        void OnResponseCoinPushSpin(string res) => OnResponse(SBoxEventHandle.SBOX_COIN_PUSH_SPIN, res);



        public int RequestCoinPushSpinEnd(Action<object> finishCallback, string mark = null)
        {
            int seqId = OnRequestBefore(SBoxEventHandle.SBOX_COIN_PUSH_SPIN_END, null, finishCallback, null, mark);

            if (isMock)
            {
                OnMockCoinPushSpinEnd(null);
            }
            else
            {
                // 滚轮停止
                SBoxIdea.CoinPushGetSpinEnd(0);
            }
            return seqId;
        }
        void OnResponseCoinPushSpinEnd(string res) => OnResponse(SBoxEventHandle.SBOX_COIN_PUSH_SPIN_END, res);



        public int RequestCoinCountDown(int winCredit, Action<object> finishCallback, string mark = null)
        {
            int seqId = OnRequestBefore(RpcNameCoinCountDown, winCredit, finishCallback, null, mark);

            if (isMock)
            {
                OnMockCoinCountDown(winCredit);
            }
            else
            {
                OnResponseCoinCountDown(SBoxIdea.Jackpot());
            }
            return seqId;
        }
        void OnResponseCoinCountDown(int res) => OnResponse(RpcNameCoinCountDown, res);
        const string RpcNameCoinCountDown = "RpcNameCoinCountDown";



        public int RequestGetMyCredit(Action<object> finishCallback, string mark = null)
        {
            int seqId = OnRequestBefore(RpcNameGetMyCredit, null, finishCallback, null, mark);

            if (isMock)
            {
                OnMockGetMyCredit(null);
            }
            else
            {

                int myCredit = 0;

                foreach (SBoxPlayerScoreInfo item in SBoxIdea.sBoxInfo.PlayerScoreInfoList)
                {
                    if (item.PlayerId == 1)
                    {
                        myCredit = item.Score;
                    }
                }
                OnResponseGetMyCredit(myCredit);
            }
            return seqId;
        }
        void OnResponseGetMyCredit(int res) => OnResponse(RpcNameGetMyCredit, res);
        const string RpcNameGetMyCredit = "RpcNameGetMyCredit";





        /// <summary> 获取major和grand的贡献值 </summary>
        public int RequestGetJpMajorGrandContribution(Action<object> finishCallback, string mark = null)
        {
            int seqId = OnRequestBefore(SBoxEventHandle.SBOX_COIN_PUSH_GET_JP_CONTRIBUTION, null, finishCallback, null, mark);

            if (isMock)
            {
                OnMockCoinPushGetJPContribution(null);
            }
            else
            {
                SBoxIdea.GetJpMajorGrandContribution(SBoxModel.Instance.pid);
            }
            return seqId;
        }
        void OnResponseGetJpMajorGrandContribution(string res) => OnResponse(SBoxEventHandle.SBOX_COIN_PUSH_GET_JP_CONTRIBUTION, res);




        /// <summary> 通知算方法卡 Major 、Grand 赢的分数 </summary>
        public int RequestSetMajorGrandWin(int majorWins, int grandWins ,Action<object> finishCallback, string mark = null)
        {
            Dictionary<string, object> req = new Dictionary<string, object>()
            {
                ["pid"] = SBoxModel.Instance.pid,
                ["majorWins"] = majorWins,
                ["grandWins"] = grandWins,
            };

            int seqId = OnRequestBefore(SBoxEventHandle.SBOX_COIN_PUSH_SET_MAJOR_GRAND_WIN, req, finishCallback, null, mark);

            if (isMock)
            {
                OnMockCoinPushSetMajorGrandWin(req);
            }
            else
            {
                SBoxIdea.SetJpMajorGrandWin(SBoxModel.Instance.pid, majorWins,  grandWins);
            }
            return seqId;
        }
        void OnResponseSetMajorGrandWin(int res) => OnResponse(SBoxEventHandle.SBOX_COIN_PUSH_SET_MAJOR_GRAND_WIN, res);





        /// <summary> 返还Major、Grand彩金贡献值 </summary>
        public int RequestReturnMajorGrandContribution(int majorCredit, int grandCredit, Action<object> finishCallback, string mark = null)
        {
            Dictionary<string, int> data = new Dictionary<string, int>()
            {
                ["pid"] = SBoxModel.Instance.pid,
                ["major"] = majorCredit,
                ["grand"] = grandCredit,
            };

            int seqId = OnRequestBefore(SBoxEventHandle.SBOX_COIN_PUSH_RETURN_JP_CONTRIBUTION, data, finishCallback, null, mark);

            if (isMock)
            {
                OnMockCoinPushReturnJPContribution(data);
            }
            else
            {
                SBoxIdea.ReturnJpMajorGrandContribution(SBoxModel.Instance.pid, majorCredit, grandCredit);
            }
            return seqId;
        }
        void OnResponseReturnMajorGrandContribution(int res) => OnResponse(SBoxEventHandle.SBOX_COIN_PUSH_RETURN_JP_CONTRIBUTION, res);



        /// <summary>
        /// 开始一局游戏
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="finishCallback"></param>
        /// <returns></returns>
        /// <remarks>
        /// * 每次掉这条协议，会发一个币扣15分！
        /// </remarks>
        public int RequesBeginTurn(Action<object> finishCallback, string mark = null)
        {
            int seqId = OnRequestBefore(SBoxEventHandle.SBOX_COIN_PUSH_BEGIN_TURN, null, finishCallback, null, mark);

            if (isMock)
            {
                OnMockBeginTurn(null);
            }
            else
            {
                SBoxIdea.CoinPushSpin(0, 1);
            }
            return seqId;
        }
        void OnResponseBeginTurn(int res) => OnResponse(SBoxEventHandle.SBOX_COIN_PUSH_BEGIN_TURN, res);

        //            return new object[] { 408, $"【ERPushMachineDataMgr - Real】没有实现方法：{rpcName}" };


    }

}