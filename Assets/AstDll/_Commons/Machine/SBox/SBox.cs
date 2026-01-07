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
using UnityEngine;

namespace SBoxApi
{
    public class SBoxEventHandle
    {
        // IDEA
        public const string SBOX_IDEA_VERSION = "SBOX_IDEA_VERSION";
        public const string SBOX_RESET = "SBOX_RESET";
        public const string SBOX_READ_CONF = "SBOX_READ_CONF";
        public const string SBOX_WRITE_CONF = "SBOX_WRITE_CONF";
        public const string SBOX_CHECK_PASSWORD = "SBOX_CHECK_PASSWORD";
        public const string SBOX_CHANGE_PASSWORD = "SBOX_CHANGE_PASSWORD";
        public const string SBOX_REQUEST_CODER = "SBOX_REQUEST_CODER";
        public const string SBOX_CODER = "SBOX_CODER";
        public const string SBOX_GET_ODDS = "SBOX_GET_ODDS";
        public const string SBOX_GET_PRIZE_PREPARE = "SBOX_GET_PRIZE_PREPARE";
        public const string SBOX_GET_PRIZE = "SBOX_GET_PRIZE";
        public const string SBOX_GET_PRIZE_PLAYER = "SBOX_GET_PRIZE_PLAYER";
        public const string SBOX_SET_PLAYER_BETS = "SBOX_SET_PLAYER_BETS";
        public const string SBOX_SET_COIN_TO_HOLE_COUNT = "SBOX_SET_COIN_TO_HOLE_COUNT";
        public const string SBOX_GET_SUMMARY = "SBOX_GET_SUMMARY";
        public const string SBOX_GET_ACCOUNT = "SBOX_GET_ACCOUNT";
        public const string SBOX_MOVE_PLAYER_SCORE = "SBOX_MOVE_PLAYER_SCORE";
        public const string SBOX_REQUEST_START = "SBOX_REQUEST_START";
        public const string SBOX_BETS_START = "SBOX_BETS_START";
        public const string SBOX_BETS_STOP = "SBOX_BETS_STOP";
        public const string SBOX_BATS_COUNT_DOWN = "SBOX_BATS_COUNT_DOWN";
        public const string SBOX_IS_MACHINE_ID_READY = "SBOX_IS_MACHINE_ID_READY";
        public const string SBOX_IDEA_INFO = "SBOX_IDEA_INFO";

        public const string SBOX_BATTLE_GET_STATE = "SBOX_BATTLE_GET_STATE";
        public const string SBOX_BATTLE_GAME_NUMBER = "SBOX_BATTLE_GAME_NUMBER";
        public const string SBOX_BATTLE_GET_COMPLETED_GAME = "SBOX_BATTLE_GET_COMPLETED_GAME";
        public const string SBOX_BATTLE_RESET_GAME = "SBOX_BATTLE_RESET_GAME";
        public const string SBOX_BATTLE_NEW_ROUND = "SBOX_BATTLE_NEW_ROUND";
        public const string SBOX_BATTLE_END_ROUND = "SBOX_BATTLE_END_ROUND";
        public const string SBOX_BATTLE_REQUEST_RESULT = "SBOX_BATTLE_REQUEST_RESULT";
        public const string SBOX_BATTLE_LEAD_START = "SBOX_BATTLE_LEAD_START";
        public const string SBOX_BATTLE_LEAD_STOP = "SBOX_BATTLE_LEAD_STOP";
        public const string SBOX_BATTLE_LUCK_SHOW = "SBOX_BATTLE_LUCK_SHOW";
        public const string SBOX_BATTLE_LUCK_PRIZE = "SBOX_BATTLE_LUCK_PRIZE";
        public const string SBOX_BATTLE_PRINTER_OPEN_BOX = "SBOX_BATTLE_PRINTER_OPEN_BOX";
        public const string SBOX_BATTLE_PLAYER_OUT_STATE = "SBOX_BATTLE_PLAYER_OUT_STATE";

        public const string SBOX_SICBO_RESET_DATA = "SBOX_SICBO_RESET_DATA";
        public const string SBOX_SICBO_REQUEST_GOODLUCK = "SBOX_SICBO_REQUEST_GOODLUCK";
        public const string SBOX_SICBO_CALCULATE = "SBOX_SICBO_CALCULATE";
        public const string SBOX_SICBO_SET_DIFFICULTY = "SBOX_SICBO_SET_DIFFICULTY";
        public const string SBOX_GET_SUMMARY_SICBO = "SBOX_GET_SUMMARY_SICBO";


        // SANDBOX
        public const string SBOX_SANDBOX_VERSION = "SBOX_SANDBOX_VERSION";
        public const string SBOX_SANDBOX_USN = "SBOX_SANDBOX_USN";
        public const string SBOX_SANDBOX_GET_DATETIME = "SBOX_SANDBOX_GET_DATETIME";
        public const string SBOX_SANDBOX_SET_DATETIME = "SBOX_SANDBOX_SET_DATETIME";
        public const string SBOX_SADNBOX_RESET = "SBOX_SADNBOX_RESET";
        public const string SBOX_SADNBOX_COIN_OUT_START = "SBOX_SADNBOX_COIN_OUT_START";
        public const string SBOX_SADNBOX_COIN_OUT_STOP = "SBOX_SADNBOX_COIN_OUT_STOP";
        public const string SBOX_SADNBOX_METER_SET = "SBOX_SADNBOX_METER_SET";
        public const string SBOX_SADNBOX_MOTOR_TOUCH = "SBOX_SADNBOX_MOTOR_TOUCH";

        // bill
        public const string SBOX_SADNBOX_BILL_LIST_GET = "SBOX_SADNBOX_BILL_LIST_GET";
        public const string SBOX_SADNBOX_BILL_SELECT = "SBOX_SADNBOX_BILL_SELECT";
        public const string SBOX_SADNBOX_BILL_APPROVE = "SBOX_SADNBOX_BILL_APPROVE";
        public const string SBOX_SADNBOX_BILL_REJECT = "SBOX_SADNBOX_BILL_REJECT";

        // printer
        public const string SBOX_SADNBOX_PRINTER_LIST_GET = "SBOX_SADNBOX_PRINTER_LIST_GET";
        public const string SBOX_SADNBOX_PRINTER_SELECT = "SBOX_SADNBOX_PRINTER_SELECT";
        public const string SBOX_SADNBOX_PRINTER_RESET = "SBOX_SADNBOX_PRINTER_RESET";
        public const string SBOX_SADNBOX_PRINTER_FONTSIZE = "SBOX_SADNBOX_PRINTER_FONTSIZE";
        public const string SBOX_SADNBOX_PRINTER_PAPERCUT = "SBOX_SADNBOX_PRINTER_PAPERCUT";
        public const string SBOX_SADNBOX_PRINTER_MESSAGE = "SBOX_SADNBOX_PRINTER_MESSAGE";
        public const string SBOX_SADNBOX_PRINTER_DATESET = "SBOX_SADNBOX_PRINTER_DATESET";
        public const string SBOX_SADNBOX_PRINTER_DATEGET = "SBOX_SADNBOX_PRINTER_DATEGET";



        // roulette
        public const string SBOX_SADNBOX_ROULETTE_CTRL = "SBOX_SADNBOX_ROULETTE_CTRL";
        public const string SBOX_SADNBOX_ROULETTE_MOTOR_MODE = "SBOX_SADNBOX_ROULETTE_MOTOR_MODE";
        public const string SBOX_SADNBOX_ROULETTE_RUN = "SBOX_SADNBOX_ROULETTE_RUN";
        public const string SBOX_SADNBOX_ROULETTE_LED_DEMO = "SBOX_SADNBOX_ROULETTE_LED_DEMO";
        public const string SBOX_SADNBOX_ROULETTE_LED_MODE = "SBOX_SADNBOX_ROULETTE_LED_MODE";
        public const string SBOX_SADNBOX_ROULETTE_TOUCH = "SBOX_SADNBOX_ROULETTE_TOUCH";
        public const string SBOX_SADNBOX_ROULETTE_RESULT = "SBOX_SADNBOX_ROULETTE_RESULT";
        public const string SBOX_SADNBOX_ROULETTE_LED_STATE = "SBOX_SADNBOX_ROULETTE_LED_STATE";
        public const string SBOX_SADNBOX_ROULETTE_STATE = "SBOX_SADNBOX_ROULETTE_STATE";
        public const string SBOX_SADNBOX_ROULETTE_RESULT_COLOR = "SBOX_SADNBOX_ROULETTE_RESULT_COLOR";

        // eject
        public const string SBOX_SADNBOX_EJECT_STATE = "SBOX_SADNBOX_EJECT_STATE";
        public const string SBOX_SADNBOX_EJECT_RESULT_NUMBER = "SBOX_SADNBOX_EJECT_RESULT_NUMBER";
        public const string SBOX_SADNBOX_EJECT_OPEN = "SBOX_SADNBOX_EJECT_OPEN";
        public const string SBOX_SADNBOX_EJECT_CLOSE = "SBOX_SADNBOX_EJECT_CLOSE";
        public const string SBOX_SADNBOX_EJECT_RESET = "SBOX_SADNBOX_EJECT_RESET";
        public const string SBOX_SADNBOX_EJECT_SET = "SBOX_SADNBOX_EJECT_SET";

        //jackpot
        public const string SBOX_JACKPOT_HOST_INIT = "SBOX_JACKPOT_HOST_INIT";
        public const string SBOX_JACKPOT_BET_HOST = "SBOX_JACKPOT_BET_HOST";
        public const string SBOX_JACKPOT_WRITE_CONFIG = "SBOX_JACKPOT_WRITE_CONFIG";
        public const string SBOX_JACKPOT_READ_CONFIG = "SBOX_JACKPOT_READ_CONFIG";



        // ========================PssOn00152
        // game
        public const string SBOX_SLOT_SPIN = "SBOX_SLOT_SPIN";
        public const string SBOX_JACKPOT_GAME = "SBOX_JACKPOT_GAME";

        //sas
        public const string SBOX_CASH_SEQ_SCORE_UP = "SBOX_CASH_SEQ_SCORE_UP";
        public const string SBOX_CASH_SEQ_SCORE_DOWN = "SBOX_CASH_SEQ_SCORE_DOWN";


        // 推币机
        public const string SBOX_IDEA_USN = "SBOX_IDEA_USN";
        public const string SBOX_COIN_PUSH_BEGIN_TURN = "SBOX_COIN_PUSH_BEGIN_TURN";
        public const string SBOX_COIN_PUSH_SPIN = "SBOX_COIN_PUSH_SPIN";
        public const string SBOX_COIN_PUSH_SPIN_END = "SBOX_COIN_PUSH_SPIN_END";

        public const string SBOX_COIN_PUSH_GET_JP_CONTRIBUTION = "SBOX_COIN_PUSH_GET_JP_CONTRIBUTION";
        public const string SBOX_COIN_PUSH_RETURN_JP_CONTRIBUTION = "SBOX_COIN_PUSH_RETURN_CONTRIBUTION";
        public const string SBOX_COIN_PUSH_SET_MAJOR_GRAND_WIN = "SBOX_COIN_PUSH_SET_MAJOR_GRAND_WIN";



        // 推币机新-后台测试 【弃用】
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_LIGHTSPIN = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_LIGHTSPIN";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_LIGHTWIPEROFF = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_LIGHTWIPEROFF";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_TOP_COIN_IN = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_TOP_COIN_IN";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_COIN = "SBOX_COIN_PUSH_CONSOLE_COIN"; // 投币数
        //public const string SBOX_COIN_PUSH_CONSOLE_COIN_BLOCK = "SBOX_COIN_PUSH_CONSOLE_COIN_BLOCK"; // 出币满
        //public const string SBOX_COIN_PUSH_CONSOLE_SPIN = "SBOX_COIN_PUSH_CONSOLE_SPIN"; // 开始键
        //public const string SBOX_COIN_PUSH_CONSOLE_SHAKE = "SBOX_COIN_PUSH_CONSOLE_SHAKE"; // 摇动
        //public const string SBOX_COIN_PUSH_CONSOLE_TOP_COIN_IN = "SBOX_COIN_PUSH_CONSOLE_TOP_COIN_IN"; 
        //public const string SBOX_COIN_PUSH_CONSOLE_TOUCH_CHANNEL = "SBOX_COIN_PUSH_CONSOLE_CHANNEL"; // 通道数
        //public const string SBOX_COIN_PUSH_CONSOLE_TOUCH_SP = "SBOX_COIN_PUSH_CONSOLE_TOUCH_SP"; // Sp数
        //public const string SBOX_COIN_PUSH_CONSOLE_RETURN_COIN = "SBOX_COIN_PUSH_CONSOLE_RETURN_COIN"; // 回币
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_BONUS_IN = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_BONUS_IN";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_COLLECT_COIN = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_COLLECT_COIN";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_TICKETER = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_TICKETER";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_WIPER = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_WIPER";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_PUSHPLATE = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_PUSHPLATE";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_BELL = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_BELL";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_CHANNEILIGHT = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_CHANNEILIGHT";//通道灯


        
        public const string SBOX_COIN_PUSH_HARDWARE_TEST_START_END = "SBOX_COIN_PUSH_HARDWARE_TEST_START_END"; // 后台开始测试
        public const string SBOX_COIN_PUSH_CONSOLE_TOP_COIN_IN2 = "SBOX_COIN_PUSH_CONSOLE_TOP_COIN_IN2";
        public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_BONUS_IN2 = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_BONUS_IN2";  // 1 开始 0 停止
        public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_WIPER2 = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_WIPER2";  // 1 开始 0 停止
        public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_PUSHPLATE2 = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_PUSHPLATE2";  // 1 开始 0 停止
        //public const string SBOX_COIN_PUSH_CONSOLE_TOGGLE_CHANNEILIGHT = "SBOX_COIN_PUSH_CONSOLE_TOGGLE_CHANNEILIGHT";//通道灯

        public const string SBOX_COIN_PUSH_CONSOLE_HARDWARE_FLAG = "SBOX_COIN_PUSH_CONSOLE_HARDWARE_FLAG";
        public const string SBOX_COIN_PUSH_CONSOLE_HARDWARE_RESULT = "SBOX_COIN_PUSH_CONSOLE_HARDWARE_RESULT";
     //public const string SBOX_COIN_PUSH_CONSOLE_HARDWARE_RESULT = "SBOX_COIN_PUSH_CONSOLE_HARDWARE_RESULT";
    }

    public class SBoxBaseData
    {
        public int[] value;
        public SBoxBaseData(int[] value)
        {
            this.value = value;
        }
    }

    public class SBox : MonoBehaviour
    {
        private static bool bInit = false;

        public static void Init()
        {
            bInit = SBoxIOStream.Init();
            if (bInit)
            {
                SBoxIdea.Init();

                //##SBoxSandbox.Init();
            }
        }

        public static void Exit()
        {
            //##SBoxSandbox.Exit();

            SBoxIdea.Exit();

            SBoxIOStream.Exit();

            bInit = false;
        }


        /**
          *  @brief          
          *  @param          无
          *  @return         无
          *  @details        
          */
        private void Update()
        {
            if(bInit == true)
            {
                int counter = 0;
                int millisecond = (int)(Time.deltaTime * 1000);

                SBoxIOStream.Exec();

               //##SBoxSandbox.Exec(millisecond);

                SBoxIdea.Exec(millisecond);

                counter = 0;

                //while (bExecRun == true)
                while (counter++ < 50)
                {
                    SBoxPacket packet = SBoxIOStream.Read();
                    if (packet != null)
                    {
                        SBoxIOEvent.SendEvent(packet.cmd, packet);
                    }
                    else
                    {
                        break;
                        //Thread.Sleep(5);
                    }
                }
            }
        }
    }
}