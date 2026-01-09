/**
 * @file    
 * @author  Huang Wen <Email:ww1383@163.com, QQ:214890094, WeChat:w18926268887>
 * @version 1.0
 *
 * @section LICENSE
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * @section DESCRIPTION
 *
 * This file is ...
 */
using Hal;
using Newtonsoft.Json;
using SimpleJSON;
using System;
using UnityEngine;

namespace SBoxApi
{
    public enum SBOX_IDEA_COIN_PUSH_KEYVALUE
    {
        KV_SCORE_UP = (1<<0),
        KV_SCORE_DOWN = (1<<1),
        KV_SET = (1<<2),
        KV_ACCOUNT = (1<<3),
        KV_AUTO = (1<<4),
        KV_PAYOUT = (1<<5),
        KV_HELP = (1<<6),
        KV_SWITCH = (1<<7),
        KV_SPIN = (1<<8),
    }

    public partial class SBoxIdea
    {




        public static void CoinPushReset(int pid)
        {
            
           // Debug.LogError($"调用 CoinPushReset pid={pid}");
            
            
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20103, source: 1, target: 2, size: 1);
            sBoxPacket.data[0] = pid;
            
            SBoxIOEvent.AddListener(sBoxPacket.cmd, CoinPushResetR);
            SBoxIOStream.Write(sBoxPacket);
        }

        public static void CoinPushResetR(SBoxPacket sBoxPacket)
        {
            
        }
        
        
		/*
		 * 玩家按下按钮时调用一次
		 * pid:玩家id
		 * coin:玩家的投币数
		 */
		public static void CoinPushSpin(int pid,int coin)
		{
            //Debug.LogError($"调用 CoinPushSpin pid={pid}  coin={coin}");
            
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20100, source: 1, target: 2, size: 2);
            sBoxPacket.data[0] = pid;
            sBoxPacket.data[1] = coin;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, CoinPushSpinR);
            SBoxIOStream.Write(sBoxPacket);
        }

        private static void CoinPushSpinR(SBoxPacket sBoxPacket)
        {
            /*
             * ret:0表示成功，-1表示传参失败，-2表示分数不足,-3表示发币失败
             */
            int ret = sBoxPacket.data[0];
            //TODO 根据ret做相应的处理


            EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_BEGIN_TURN, ret);
        }

        /*
         * 获取滚轮结果
         * pid:玩家id
         */
        public static void CoinPushGetSpinResult(int pid)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20101, source: 1, target: 2, size: 1);
            sBoxPacket.data[0] = pid;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, CoinPushGetSpinResultR);
            SBoxIOStream.Write(sBoxPacket);
        }

        private static void CoinPushGetSpinResultR(SBoxPacket sBoxPacket)
        {
            //Debug.LogError($"CoinPushGetSpinResultR 函数被调用");
            JSONNode result = JSONNode.Parse("{}");
            /*
             * ret:0表示成功，-1表示传参失败
             */
            int pos = 0;
            int ret = sBoxPacket.data[pos++];
            if(ret == 0)
            {

                /*
                 * 当前游戏状态
                     #define GS_NORMAL		0	//什么都没有
                    #define GS_START		1
                    #define GS_END			2
                    #define GS_WINLINE		3	//赢线
                    #define GS_FREEGAME		4   //免费游戏
                    #define GS_BONUS		5	//送球
                    #define GS_JPSMALM		6	//中了小中彩金
                    #define GS_OPERATER		7
                 */
                int gameState = sBoxPacket.data[pos++];
                int lineNum = sBoxPacket.data[pos++];       //中了几条线
                int lineMark = sBoxPacket.data[pos++];      //中线的组合数

                result["gameState"] = gameState;
                result["lineMark"] = lineMark;
                /*【num 值含义：】
                 * 1.当gameState = GS_WINLINE 时 为0
                 * 2.当gameState = GS_FREEGAME 时 为免费游戏的图标个数
                 * 3.当gameState = GS_BONUS 时 为中球的个数
                 * 4.当gameState = GS_JPSMALM 时 为彩金的值
                 */
                int num = sBoxPacket.data[pos++]; // 球的个数 、免费游戏次数、 500mini/ 1000minor
                int curRound = sBoxPacket.data[pos++];  //当前免费游戏的次数
                int maxRound = sBoxPacket.data[pos++];  //免费游戏的最大次数

                result["num"] = num; 
                result["curRound"] = curRound;
                result["maxRound"] = maxRound;
                result["lineNum"] = lineNum;
                
                result["lineData"] = new JSONArray();

                bool isError = false;
                for(int i = 0;i < lineNum; i++)
                {
                    int icon = sBoxPacket.data[pos++];          //线的图标
                    int link = sBoxPacket.data[pos++];          //图标的几连线
                    int reward = sBoxPacket.data[pos++];        //当前线的得分
                    
                    JSONNode node = new JSONObject();
                    node["icon"] = icon;
                    node["link"] = link;
                    node["reward"] = reward;
                    result["lineData"].Add(node);

                    if (icon == 0)
                        isError = true;
                    
                }


                if (isError)
                    Debug.LogError($"【Error】icon: 0  ;  data: {JsonConvert.SerializeObject(sBoxPacket)}");
            }
            
            result["code"] = ret;
            
            //Debug.LogError($"拿到的数据： {result.ToString()}");
            EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_SPIN,result.ToString());
        }
        
        
        /*
         * 滚轮停止后发送
         */
        public static void CoinPushGetSpinEnd(int pid)
        {
            //Debug.LogError($"CoinPushGetSpinEnd pid={pid}");
            
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20102, source: 1, target: 2, size: 1);
            sBoxPacket.data[0] = pid;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, CoinPushGetSpinEndR);
            SBoxIOStream.Write(sBoxPacket);
        }

        private static void CoinPushGetSpinEndR(SBoxPacket sBoxPacket)
        {
            /*
             * ret:0表示成功，-1表示传参失败
             */
            int ret = sBoxPacket.data[0];
            int credit = sBoxPacket.data[1]; //玩家的分数
            //TODO 根据ret做相应的处理

            JSONNode result = new JSONObject();
            result["code"] = ret;
            result["credit"] = credit;
            

            foreach (SBoxPlayerScoreInfo item in sBoxInfo.PlayerScoreInfoList)
            {
                if (item.PlayerId == 1)
                {
                    result["playerCredit"] = item.Score;
                }
            }
            
            
            EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_SPIN_END,result.ToString());
        }


        /// <summary>
        /// app获取major和grand的贡献值
        /// </summary>
        /// <param name="pid"></param>
        public static void GetJpMajorGrandContribution(int pid)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20104, source: 1, target: 2, size: 1);
            sBoxPacket.data[0] = pid;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, GetJpMajorGrandContributionR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void GetJpMajorGrandContributionR(SBoxPacket sBoxPacket)
        {
            /*
             * ret:0表示成功，-1表示传参失败
             */
            int ret = sBoxPacket.data[0];

            JSONNode result = new JSONObject();
            result["code"] = ret;

            if(ret == 0)
            {
                int major = sBoxPacket.data[1];
                int grand = sBoxPacket.data[2];

                result["major"] = major;
                result["grand"] = grand;
            }

            EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_GET_JP_CONTRIBUTION, result.ToString());
        }



        /// <summary>
        /// 获取到彩金服中奖金额(Major 或 Grand)，存入算法卡
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="wins"></param>
        public static void SetJpMajorGrandWin(int pid, int majorWins, int grandWins)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20105, source: 1, target: 2, size: 3);
            sBoxPacket.data[0] = pid;
            sBoxPacket.data[1] = majorWins;
            sBoxPacket.data[2] = grandWins;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, SetJpMajorGrandWinR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void SetJpMajorGrandWinR(SBoxPacket sBoxPacket)
        {
            int ret = sBoxPacket.data[0];
            //JSONNode result = new JSONObject();
            //result["code"] = ret;

            EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_SET_MAJOR_GRAND_WIN, ret);
        }



        /// <summary>
        /// 返还Major、Grand彩金贡献值
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="credit"></param>
        /// <remarks>
        /// * 本地累计 大于10,则放回彩金贡献值
        /// </remarks>
        public static void ReturnJpMajorGrandContribution(int pid, int majorCredit, int grandCredit)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20106, source: 1, target: 2, size: 3);
            sBoxPacket.data[0] = pid;
            sBoxPacket.data[1] = majorCredit;
            sBoxPacket.data[2] = grandCredit;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, ReturnJpMajorGrandContributionR);
            SBoxIOStream.Write(sBoxPacket);
        }

        private static void ReturnJpMajorGrandContributionR(SBoxPacket sBoxPacket)
        {
            int ret = sBoxPacket.data[0];
            //JSONNode result = new JSONObject();
            //result["code"] = ret;

            EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_RETURN_JP_CONTRIBUTION, ret);
        }





        /// <summary>
        /// 推币机硬件测试“开始”或“停止”
        /// </summary>
        /// <param name="oper">
        /// 1:发币测试
        /// 2:发球测试
        /// 3:推盘测试
        /// 4:雨刷测试
        /// 5:铃铛测试
        /// 6:雨刷测试
        /// 7:铃铛测试
        /// </param>
        /// <remarks>
        /// *
        /// </remarks>
        public static void CheckCoinPushHardware(int oper)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20107, source: 1, target: 2, size: 1);
            sBoxPacket.data[0] = oper;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, CheckCoinPushHardwareR);
            SBoxIOStream.Write(sBoxPacket);
        }

        private static void CheckCoinPushHardwareR(SBoxPacket sBoxPacket)
        {
            int ret = sBoxPacket.data[0];

            // 新加
            int coinPushTestCoins = sBoxPacket.data[1];  // 当前发币总个数
            int coinPushTestBalls = sBoxPacket.data[2];  // 当前发球总个数
            int coinPushTestPlate = sBoxPacket.data[3]; // 是否测试推盘 1:是 0:否
            int coinPushTestWiper = sBoxPacket.data[4]; // 是否测试雨刷 1:是 0:否


            JSONNode result = new JSONObject();
            result["code"] = ret;
            result["coinPushTestCoins"] = coinPushTestCoins;
            result["coinPushTestBalls"] = coinPushTestBalls;
            result["coinPushTestPlate"] = coinPushTestPlate;
            result["coinPushTestWiper"] = coinPushTestWiper;


            Version currentVersion = new Version(version);
            Version targetVersion = new Version("1.0.6");
            if (currentVersion >= targetVersion)
            {
                int coinPushTestRegainCoins = sBoxPacket.data[5];  // 回币总个数
                int coinPushTestRegainBalls = sBoxPacket.data[6];  // 回球总个数

                result["coinPushTestRegainCoins"] = coinPushTestRegainCoins;
                result["coinPushTestRegainBalls"] = coinPushTestRegainBalls;
            }


            EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_HARDWARE_TEST_START_END, result.ToString());

        }








        /// <summary>
        /// 获取硬件状态
        /// </summary>
        public static void GetHardwareFlag()
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20108, source: 1, target: 2, size: 1);

            sBoxPacket.data[0] = 0;

            SBoxIOEvent.AddListener(sBoxPacket.cmd, GetHardwareFlagR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void GetHardwareFlagR(SBoxPacket sBoxPacket)
        {
            int ret = sBoxPacket.data[0];

            //返回两个值，data[0]表示成功，data[1]是标志位。

            JSONNode result = new JSONObject();
            result["code"] = ret; 
            result["flag"] = sBoxPacket.data[1]; //1bit 发币，2bit 发球

            // int bit0 = number & 1;  bool isCoinDown = bit0 != 0;
            // int bit1 = number & 2;  bool isBallDown = bit1 != 0;
            EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_CONSOLE_HARDWARE_FLAG, result.ToString());
        }



        /// <summary>
        /// 获取硬件状态
        /// </summary>
        public static void GetHardwareResult()
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20109, source: 1, target: 2, size: 1);
            sBoxPacket.data[0] = 0;
            SBoxIOEvent.AddListener(sBoxPacket.cmd, GetHardwareResultR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void GetHardwareResultR(SBoxPacket sBoxPacket)
        {
            JSONNode result = new JSONObject();
            result["code"] = sBoxPacket.data[0]; //data[0]表示成功
            result["coinPushTestCoins"] = sBoxPacket.data[1];
            result["coinPushTestBalls"] = sBoxPacket.data[2];
            result["coinPushTestPlate"] = sBoxPacket.data[3];
            result["coinPushTestWiper"] = sBoxPacket.data[4];

            Version currentVersion = new Version(version);
            Version targetVersion = new Version("1.0.6");
            if (currentVersion >= targetVersion)
            {
                int coinPushTestRegainCoins = sBoxPacket.data[5];  // 回币总个数
                int coinPushTestRegainBalls = sBoxPacket.data[6];  // 回球总个数

                result["coinPushTestRegainCoins"] = coinPushTestRegainCoins;
                result["coinPushTestRegainBalls"] = coinPushTestRegainBalls;
            }

            EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_CONSOLE_HARDWARE_RESULT, result.ToString());
        }



        /// <summary>
        /// 是否进入后台
        /// </summary>
        /// <param name="isIn">1:进入， 0：退出</param>
        public static void IntoConsolePage(int isIn)
        {
            SBoxPacket sBoxPacket = new SBoxPacket(cmd: 20110, source: 1, target: 2, size: 1);
            sBoxPacket.data[0] = isIn;
            SBoxIOEvent.AddListener(sBoxPacket.cmd, IntoConsolePageR);
            SBoxIOStream.Write(sBoxPacket);
        }
        private static void IntoConsolePageR(SBoxPacket sBoxPacket)
        {
            JSONNode result = new JSONObject();
            result["code"] = sBoxPacket.data[0]; //data[0]表示成功
           // EventCenter.Instance.EventTrigger(SBoxEventHandle.SBOX_COIN_PUSH_CONSOLE_HARDWARE_RESULT, result.ToString());
        }

    }
}
