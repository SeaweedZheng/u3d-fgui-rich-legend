#define SQLITE_ASYNC
using GameMaker;
using IOT;
using Newtonsoft.Json;
using SimpleJSON;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeviceIOTPayment : MonoSingleton<DeviceIOTPayment>  //MonoBehaviour
{

    void Start()
    {
        // if (!ApplicationSettings.Instance.isMachine) return;

        // 注册获取二维码
        EventCenter.Instance.AddEventListener<List<QrCodeData>>(IOTEventHandle.REGISTER_DEV, OnEventRegisterQrCode);
        // 二维码码过期重新刷新
        //EventCenter.Instance.AddEventListener<>(EventHandle.REFRESH_QRCORD,);
        // 监听投币信息
        EventCenter.Instance.AddEventListener<CoinData>(IOTEventHandle.COINT_IN, OnEventQrCoinIn);
        //退票
        EventCenter.Instance.AddEventListener<TicketOutData>(IOTEventHandle.TICKET_OUT, OnEventIotTicketOut);


    }
    protected override void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<List<QrCodeData>>(IOTEventHandle.REGISTER_DEV, OnEventRegisterQrCode);
        EventCenter.Instance.RemoveEventListener<CoinData>(IOTEventHandle.COINT_IN, OnEventQrCoinIn);
        EventCenter.Instance.RemoveEventListener<TicketOutData>(IOTEventHandle.TICKET_OUT, OnEventIotTicketOut);

        base.OnDestroy();
    }


    void ClearCo(Coroutine co)
    {
        if (co != null)
            StopCoroutine(co);
        co = null;
    }
    IEnumerator DelayTask(Action task, int timeMS)
    {
        yield return new WaitForSeconds((float)timeMS / 1000f);
        task?.Invoke();
    }

    static public IEnumerator RepeatTask(Action task, int timeMS)
    {
        while (true)
        {
            yield return new WaitForSeconds((float)timeMS / 1000f);
            task?.Invoke();
        }
    }

    Coroutine coReconnectIot = null, 
        coCheckIotConnect = null,
        coCheckIotCoinIn = null;


    void CheckIotSignIn()
    {
        _CheckIotSignIn();

        ClearCo(coCheckIotConnect);
        coCheckIotConnect = StartCoroutine(RepeatTask(_CheckIotSignIn, 5000));

        //DoCor(COR_CHECK_IOT_CONNECT, DoTaskRepeat(_CheckIotSignIn, 5000));
    }
    void _CheckIotSignIn()
    {
        if (SBoxModel.Instance.isConnectIot != isIOTSignInGetQRCode)
        {
            SBoxModel.Instance.isConnectIot = isIOTSignInGetQRCode;
        }

        if (SBoxModel.Instance.isUseIot == false)
        {
            ClearCo(coCheckIotConnect);
        }
    }

    /// <summary>
    /// 链接或关闭好酷
    /// </summary>
    public void CheckIOT()
    {
        //开启或关闭好酷
        if (SBoxModel.Instance.isUseIot)
            Init();
        else
            Close();

        //断网重连
        if (SBoxModel.Instance.isUseIot) { 

            ClearCo(coReconnectIot);
            coReconnectIot = StartCoroutine(RepeatTask(() =>
            {
                if (SBoxModel.Instance.isUseIot && !isIOTSignInGetQRCode)
                    Init();
            }, 10000));
            /*
                DoCor(COR_RECONNECT_IOT, DoTaskRepeat(() =>
                {
                    if (SBoxModel.Instance.isUseIot && !isIOTSignInGetQRCode)
                        Init();
                }, 10000));
            */
        }
        else
            ClearCo(coReconnectIot);
            //ClearCor(COR_RECONNECT_IOT);
    }





    [Button]
    void Init()
    {
        StartCoroutine(_InitIOT());
    }
    void Close()
    {
        IoTPayment.Instance.Disconnect();

        //ClearCor(COR_CHECK_IOT_CONNECT);
        ClearCo(coCheckIotConnect);
        SBoxModel.Instance.isConnectIot = false;
    }


    /// <summary> 好酷是否链接 </summary>
    public bool isIOTConneted => IoTPayment.Instance.IsConnected;


    /// <summary>
    ///  是否登录好酷注册二维码
    /// </summary>
    public bool isIOTSignInGetQRCode
    {
        get => isIOTConneted && IOTModel.Instance.LinkIOT;  // 注册二维码
    }

    /// <summary>
    /// 是否已经投币绑定微信号
    /// </summary>
    public bool isCoinInBindWeChatAccount
    {
        get => !string.IsNullOrEmpty(IOTModel.Instance.LinkId);  // 已经先投币
    }

    /// <summary>
    /// 已经登录好酷账号，且链接
    /// </summary>
    public bool isAllowIotCoinOut
    {
        get => isIOTSignInGetQRCode   // 注册二维码
            && isCoinInBindWeChatAccount;  // 已经先投币
    }





    IEnumerator _InitIOT()
    {
        SBoxModel.Instance.isConnectIot = false;

        //M2MqttUnityClient.Instance.is
        //原来连接上，先关掉
        while (isIOTConneted)
        {
            IoTPayment.Instance.Disconnect();
            yield return new WaitForSeconds(3f);

            DebugUtils.Log($"【IOT】等待好酷关闭");
        }

        DebugUtils.Log($"【IOT】好酷初始化 {IoTConst.GetDevParamURL}");

        if (PlayerPrefsUtils.isUseReleaseIot)
        {
            //正式账号 SBoxModel.Instance.pid,
            IoTPayment.Instance.Init(SBoxModel.Instance.MachineId, SBoxModel.Instance.iotPort, SBoxModel.Instance.iotAccessMethods, 32, 1,
            (err) =>
            {
                if (!MainModel.Instance.isOpenConsolePage)
                    TipPopupHandler.Instance.OpenPopupOnce("【好酷】：" + err);
            });
        }
        else
        {

            IoTPayment.Instance.Init("10600001", 1, 1, 32, 1,
            (err) =>
            {
                TipPopupHandler.Instance.OpenPopupOnce("【好酷提醒】：" + err);
            });
        }

        CheckIotSignIn();

    }




    /// <summary> 注册，获取二维码 </summary> 
    void OnEventRegisterQrCode(List<QrCodeData> res)
    {

        if (res != null && res.Count > 0)
        {
            IOTModel.Instance.LinkIOT = true;
            IOTModel.Instance.qrCodeDatas = res;
            DebugUtils.Log("【IOT】好酷二维码注册成功");
        }
        else
        {
            DebugUtils.Log("【IOT】好酷二维码注册失败");
        }

    }

    const string DEVICE_IOT_COIN_OUT_ORDER = "device_iot_coin_out_order";
    const string DEVICE_IOT_COIN_IN_ORDER = "device_iot_coin_in_order";



    JSONNode _cacheIOTCoinOutOrder;

    JSONNode cacheIOTCoinInOrder
    {
        get
        {
            if (_cacheIOTCoinInOrder == null)
                _cacheIOTCoinInOrder = JSONNode.Parse(SQLitePlayerPrefs03.Instance.GetString(DEVICE_IOT_COIN_IN_ORDER, "{}"));
            return _cacheIOTCoinInOrder;
        }
        //set => _cacheCoinInOrder = value;

    }



    JSONNode _cacheIOTCoinInOrder;

    JSONNode cacheIOTCoinOutOrder
    {
        get
        {
            if (_cacheIOTCoinOutOrder == null)
                _cacheIOTCoinOutOrder = JSONNode.Parse(SQLitePlayerPrefs03.Instance.GetString(DEVICE_IOT_COIN_OUT_ORDER, "{}"));
            return _cacheIOTCoinOutOrder;
        }
        //set => _cacheCoinInOrder = value;
    }



    /// <summary>
    /// 每次好酷冲钱时，获取linkId
    /// </summary>
    string curPlayerLinkId = "";

    #region 好酷投币
    /// <summary>
    /// 二维码充值
    /// </summary>
    /// <param name="data"></param>
    void OnEventQrCoinIn(CoinData data)
    {
        //收到投币信息，数据存入算法卡

        if (data.Num <= 0)
            return;

        string LinkId = data.orderNum;
        IOTModel.Instance.LinkId = LinkId;
        curPlayerLinkId = LinkId;

        string orderId = OrderIdCreater.Instance.CreatOrderId(OrderIdCreater.IOTCoinIn); // Guid.NewGuid().ToString();

        int credit = data.Num * SBoxModel.Instance.CoinInScale;

        JSONNode nodeOrder = JSONNode.Parse("{}");
        nodeOrder.Add("device_number", 0);
        nodeOrder.Add("type", "iot_coin_in");
        nodeOrder.Add("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        nodeOrder.Add("count", data.Num); //多少币
        nodeOrder.Add("order_id", orderId);
        nodeOrder.Add("link_id", curPlayerLinkId);
        nodeOrder.Add("scale", SBoxModel.Instance.CoinInScale); //1币多少分
        nodeOrder.Add("credit", credit);
        nodeOrder.Add("credit_before", SBoxModel.Instance.myCredit);
        nodeOrder.Add("credit_after", SBoxModel.Instance.myCredit + credit);
        nodeOrder.Add("code", ErrorCode.DEVICE_IOT_COIN_IN_SUCCESS);
        nodeOrder.Add("msg", "");
        cacheIOTCoinInOrder[orderId] = nodeOrder;
        SQLitePlayerPrefs03.Instance.SetString(DEVICE_IOT_COIN_IN_ORDER, cacheIOTCoinInOrder.ToString());


        DebugUtils.Log($"【Iot Coin In】好酷数据：{JsonConvert.SerializeObject(data)}");
        DebugUtils.Log($"【Iot Coin In】订单号：{nodeOrder.ToString()}");

        // 加钱超时：
        ClearCo(coCheckIotCoinIn);
        coCheckIotCoinIn = StartCoroutine(DelayTask(() =>
        {
            coCheckIotCoinIn = null;

            IoTPayment.Instance.ReplyCoinIn(
               IOTModel.Instance.PortId,  //IOTModel.Instance.qrCodeDatas[0].portid,
               data.Num,
               data.orderNum,
               false,
               "请求超时");
        }, 10000));
        
        /*DoCor(COR_CHECK_IOT_COIN_IN, DoTask(() =>
        {
            IoTPayment.Instance.ReplyCoinIn(
                IOTModel.Instance.PortId,  //IOTModel.Instance.qrCodeDatas[0].portid,
                data.Num,
                data.orderNum,
                false,
                "请求超时");

            // 弹出提示
        }, 10000));
        */

        MachineDataManager.Instance.RequestCoinIn(data.Num, (Action<object>)((res) =>
        {

            DebugUtils.Log($"好酷上分成功 : {credit}");

            //ClearCor(COR_CHECK_IOT_COIN_IN);
            ClearCo(coCheckIotCoinIn);

            JSONNode nodeOrder = cacheIOTCoinInOrder[orderId];

            int iotCoinInCredit = (int)nodeOrder["credit"];
            /*
            (int)nodeOrder["credit"];
            long myCredit = BlackboardUtils.GetValue<long>("@console/myCredit");//SBoxModel.Instance.myCredit;
            cacheIOTCoinInOrder[orderIdCoinIn]["credit_before"] = myCredit;
            cacheIOTCoinInOrder[orderIdCoinIn]["credit_after"] = myCredit + coinInCredit;
            */


            // 清掉缓存订单
            cacheIOTCoinInOrder.Remove(orderId);
            SQLitePlayerPrefs03.Instance.SetString(DEVICE_IOT_COIN_IN_ORDER, cacheIOTCoinInOrder.ToString());


            string sql = SQLiteAsyncHelper.SQLInsertTableData<TableCoinInOutRecordItem>(
                ConsoleTableName.TABLE_COIN_IN_OUT_RECORD,
                new TableCoinInOutRecordItem()
                {
                    device_type = nodeOrder["type"],
                    device_number = nodeOrder["device_number"],
                    order_id = nodeOrder["order_id"],
                    count = nodeOrder["count"],
                    credit = nodeOrder["credit"],
                    credit_after = SBoxModel.Instance.myCredit + iotCoinInCredit,
                    credit_before = SBoxModel.Instance.myCredit,
                    in_out = 1,
                    created_at = nodeOrder["timestamp"],
                });
            DebugUtils.Log($"【SQL - Iot Coin In】 : {sql}");
            SQLiteAsyncHelper.Instance.ExecuteNonQueryAsync(sql);


            //加钱动画
            SBoxModel.Instance.myCredit += iotCoinInCredit;
            MainBlackboardController.Instance.AddOrSyncMyCreditToReal(iotCoinInCredit);


            //每日统计
            TableBusniessDayRecordAsyncManager.Instance.AddTotalCoinIn((long)iotCoinInCredit, SBoxModel.Instance.myCredit);


            //刷新缓存
            if (true)
            {
                IoTPayment.Instance.ReplyCoinIn(
                    IOTModel.Instance.PortId,
                    data.Num,
                    data.orderNum,
                    true,
                    "");
            }
            else
            {
                // 可能失败？？
                /*IoTPayment.Instance.ReplyCoinIn(
                    IOTModel.Instance.PortId,
                    //IOTModel.Instance.qrCodeDatas[0].portid,
                    data.Num,
                    data.orderNum,
                    false,
                    "报错信息");*/
            }

            EventCenter.Instance.EventTrigger<EventData>(GlobalEvent.ON_COIN_IN_OUT_EVENT,
                new EventData<int>(GlobalEvent.IOTCoinInCompleted, iotCoinInCredit));
        }));


    }


    /// <summary>
    /// 好酷充值
    /// </summary>
    [Button]
    public void DoQrCoinIn()
    {
        if (!isIOTSignInGetQRCode)
        {
            TipPopupHandler.Instance.OpenPopupOnce(string.Format(I18nMgr.T("IOT connection failed [{0}]"), ErrorCode.DEVICE_IOT_NOT_SIGN_IN));
            //TipPopupHandler.Instance.OpenPopup("\"二维码投退币功能\"未开启");
            return;
        }


        string urlQRCode = IOTModel.Instance.qrCodeDatas[0].qrcodeUrl;
        DebugUtils.Log($"好酷二维码地址： {urlQRCode}");
       
        PageManager.Instance.OpenPage(PageName.ConsolePopupConsoleCommon002,
            new EventData<CommonPopupInfo002>(
                "",
                new CommonPopupInfo002()
                {
                    url = urlQRCode,
                    type = CommonPopupType.TextOnly,
                    text = "Scan the QR code\nto earn more points",
                    isUseXButton = true,
                    mark = MARK_POP,
                }
            ));
    }

    const string MARK_POP = nameof(MARK_POP) + "_" + nameof(DeviceIOTPayment);

    #endregion

















    #region 好酷退票

    Coroutine coIsIotCoinOuting = null,
        coIotCoinOutTimeout = null,
        coDelayIotTickerOut = null;



    public bool isRegularIOTCoinOuting
    {
        get => coIsIotCoinOuting != null || coIotCoinOutTimeout != null;
    }


    //string COR_DELAY_IOT_TICKER_OUT = "COR_DELAY_IOT_TICKER_OUT";
    string orderIdIOTCoinOut;
    public void DoIotTickerOut()
    {
        ClearCo(coDelayIotTickerOut);
        coDelayIotTickerOut = StartCoroutine(DelayTask(()=>
        {
            coDelayIotTickerOut = null;

            _DoIotTickerOut();
        },500)); // 延时退票，避免退票和游戏开完同时开始

        //DoCor(COR_DELAY_IOT_TICKER_OUT, DoTask(_DoIotTickerOut, 500)); // 延时退票，避免退票和游戏开完同时开始
    }

    public void _DoIotTickerOut()
    {
        if ( MainModel.Instance.isSpin)
        {
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("<size=24>Cannot coin out during the game period</size>"));
            return;
        }

        DebugUtils.Log($"是否连接好酷: {isIOTConneted}， 是否登录注册: {isIOTSignInGetQRCode} 是否绑定微信号: {isCoinInBindWeChatAccount}  isAllowIotCoinOut:{isAllowIotCoinOut}");
        if (!isAllowIotCoinOut)
        {
            //TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("IOT connection failed"));
            return;
        }



        int coinOutNum = DeviceUtils.GetCoinOutNum();
        if (coinOutNum <= 0)
        {
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("Too few game credits, ticket refund failed."));
            return;
        }


        if ( coIsIotCoinOuting != null) //if (IsCor(COR_IS_IOT_COIN_OUT_ING))
            return;

        ClearCo(coIsIotCoinOuting);
        coIsIotCoinOuting = StartCoroutine(DelayTask(() =>
        {
            coIsIotCoinOuting = null;
        }, 4001)); //延时避免重复触发

        //DoCor(COR_IS_IOT_COIN_OUT_ING, DoTask(() => { }, 4001)); //延时避免重复触发


        long credit = DeviceUtils.GetCoinOutCredit(coinOutNum);

        // 打开遮罩
        MaskPopupHandler.Instance.OpenPopup();

        //好酷退票超时。
        ClearCo(coIotCoinOutTimeout);
        coIotCoinOutTimeout = StartCoroutine(DelayTask(() =>
        {
            coIotCoinOutTimeout = null;

            MaskPopupHandler.Instance.ClosePopup();
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("IOT refund timeout"));
            CheckIOT(); //重连好酷

        }, 40001));

        /*
        DoCor(COR_IOT_COIN_OUT_TIMEOUT, DoTask(() => {
            MaskPopupHandler.Instance.ClosePopup();
            TipPopupHandler.Instance.OpenPopupOnce(I18nMgr.T("IOT refund timeout"));
            CheckIOT(); //重连好酷
        }, 40001));
        */

        long timeMS = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        orderIdIOTCoinOut = OrderIdCreater.Instance.CreatOrderId(OrderIdCreater.IOTCoinOut);  // Guid.NewGuid().ToString();

        JSONNode nodeOrder = JSONNode.Parse("{}");
        nodeOrder.Add("device_number", 0);
        nodeOrder.Add("type", "iot_coin_out");
        nodeOrder.Add("timestamp", timeMS);
        nodeOrder.Add("count", coinOutNum); //多少票
        nodeOrder.Add("order_id", orderIdIOTCoinOut);
        nodeOrder.Add("scale_credit_per_ticket", SBoxModel.Instance.CoinOutScaleCreditPerTicket); //1币多少分
        nodeOrder.Add("scale_ticket_per_credit", SBoxModel.Instance.CoinOutScaleTicketPerCredit); //1币多少分
        nodeOrder.Add("credit", credit);
        nodeOrder.Add("credit_before", SBoxModel.Instance.myCredit);
        nodeOrder.Add("credit_after", SBoxModel.Instance.myCredit - credit);
        //nodeOrder.Add("link_id", IOTModel.Instance.LinkId);
        nodeOrder.Add("link_id", curPlayerLinkId);
        nodeOrder.Add("code", ErrorCode.DEVICE_CREAT_ORDER_NUMBER);
        nodeOrder.Add("msg", "");
        cacheIOTCoinOutOrder[orderIdIOTCoinOut] = nodeOrder;
        SQLitePlayerPrefs03.Instance.SetString(DEVICE_IOT_COIN_OUT_ORDER, cacheIOTCoinOutOrder.ToString());

        DebugUtils.Log($"【Iot Coin Out】创建本地订单：{nodeOrder.ToString()}");



        TicketOutData data = new TicketOutData();
        data.type = (int)IOTModel.Instance.ticketOutType;
        data.num = coinOutNum;
        data.seq = $"{timeMS / 1000}"; //(Utils.GetTimeStamp()/1000).ToString();
        data.orderNum = IOTModel.Instance.LinkId;

        DebugUtils.Log($"【Iot Coin Out】给好酷发数据：{JsonConvert.SerializeObject(data)}");

        IOTModel.Instance.unfinishTicketOutDatas.Add(data);
        IoTPayment.Instance.DeviceTicketOut(
            IOTModel.Instance.PortId,
            data,
            (err) => {

                CommonPopupHandler.Instance.OpenPopupSingle(
                new CommonPopupInfo()
                {
                    isUseXButton = false,
                    buttonAutoClose1 = true,
                    type = CommonPopupType.OK,
                    text = string.Format(
                                    I18nMgr.T("Request failed: [{0}]"),
                                    ErrorCode.DEVICE_IOT_COIN_OUT_API_ERR),
                    buttonText1 = I18nMgr.T("OK"),
                });

                cacheIOTCoinOutOrder.Remove(orderIdIOTCoinOut);
                OnOverIOTCoinOut();
            });
    }

    void OnOverIOTCoinOut()
    {
        MaskPopupHandler.Instance.ClosePopup();
        ClearCo(coIsIotCoinOuting);
        ClearCo(coIotCoinOutTimeout);
        //ClearCor(COR_IS_IOT_COIN_OUT_ING);
        //ClearCor(COR_IOT_COIN_OUT_TIMEOUT);
        orderIdIOTCoinOut = "";
    }
    void OnEventIotTicketOut(TicketOutData data)
    {
        if (data.num <= 0)
            return;

        string linkId = curPlayerLinkId;

        //退票成功 。这里要订单补发（这里要订单确认）
        cacheIOTCoinOutOrder[orderIdIOTCoinOut]["code"] = ErrorCode.DEVICE_IOT_COIN_OUT_SUCCESS;
        SQLitePlayerPrefs03.Instance.SetString(DEVICE_IOT_COIN_OUT_ORDER, cacheIOTCoinOutOrder.ToString());


        //DebugUtil.Log($"【Iot Ticket out】 : 好酷数据 = {JSONNodeUtil.ObjectToJsonStr(data)}");
        //DebugUtil.Log($"【Iot Ticket out】 : 缓存 = {JSONNodeUtil.ObjectToJsonStr(IOTModel.Instance.unfinishTicketOutDatas)}");
        //DebugUtil.Log($"【Iot Ticket out】 : 本地订单号 = {cacheIOTCoinOutOrder[orderIdIOTCoinOut].ToString()}");
        //DebugUtil.Log($"【Iot Ticket out】 : @@ data.seq = {data.seq} ; IOTModel.Instance.LinkId= {linkId}");
        //删除缓存订单
        TicketOutData lastData = null;
        for (int i = 0; i < IOTModel.Instance.unfinishTicketOutDatas.Count; i++)
        {
            var item = IOTModel.Instance.unfinishTicketOutDatas[i];
            //DebugUtil.Log($"【Iot Ticket out】 : item.seq = {item.seq} ; item.orderNum = {item.orderNum}");
            if (item.seq == data.seq && item.orderNum == linkId)
            {
                lastData = item;
                IOTModel.Instance.unfinishTicketOutDatas.Remove(item);
                break;
            }
        }

        IOTModel.Instance.LinkId = null;
        curPlayerLinkId = null;

        if (lastData != null)
        {

            int creditOut = cacheIOTCoinOutOrder[orderIdIOTCoinOut]["credit"];
            int coinOutNum = cacheIOTCoinOutOrder[orderIdIOTCoinOut]["count"];

            MachineDataManager.Instance.RequestCoinOut(coinOutNum, (Action<object>)((res) =>
            {

                DebugUtils.Log($"好酷退票成功 : {creditOut}");
                JSONNode nodeOrder = cacheIOTCoinOutOrder[orderIdIOTCoinOut];

                // 清掉缓存订单
                cacheIOTCoinOutOrder.Remove(orderIdIOTCoinOut);
                SQLitePlayerPrefs03.Instance.SetString(DEVICE_IOT_COIN_OUT_ORDER, cacheIOTCoinOutOrder.ToString());

                if (SBoxModel.Instance.myCredit - creditOut < 0)
                {
                    DebugUtils.LogError($"好酷退的积分: {creditOut}  超过自身积分： {SBoxModel.Instance.myCredit}");
                    creditOut = (int)SBoxModel.Instance.myCredit;
                }

                string sql = SQLiteAsyncHelper.SQLInsertTableData<TableCoinInOutRecordItem>(
                    ConsoleTableName.TABLE_COIN_IN_OUT_RECORD,
                    new TableCoinInOutRecordItem()
                    {
                        device_type = nodeOrder["type"],
                        device_number = nodeOrder["device_number"],
                        order_id = nodeOrder["order_id"],
                        count = nodeOrder["count"],
                        credit = creditOut,
                        credit_after = SBoxModel.Instance.myCredit - creditOut, //nodeOrder["credit_after"],
                        credit_before = SBoxModel.Instance.myCredit,//nodeOrder["credit_before"],
                        in_out = 0,
                        created_at = nodeOrder["timestamp"],
                    });

                DebugUtils.Log($"【SQL - Iot Coin Out】 : {sql}");
                SQLiteAsyncHelper.Instance.ExecuteNonQueryAsync(sql);



                //加钱动画
                SBoxModel.Instance.myCredit -= creditOut;
                MainBlackboardController.Instance.MinusOrSyncMyCreditToReal(creditOut);


                //每日统计
                TableBusniessDayRecordAsyncManager.Instance.AddTotalCoinOut((long)creditOut, SBoxModel.Instance.myCredit);

                OnOverIOTCoinOut();


                EventCenter.Instance.EventTrigger<EventData>(GlobalEvent.ON_COIN_IN_OUT_EVENT,
                    new EventData<int>(GlobalEvent.IOTCoinOutCompleted, creditOut));
            }));

        }
        else
        {

            CommonPopupHandler.Instance.OpenPopupSingle(
            new CommonPopupInfo()
            {
                isUseXButton = false,
                buttonAutoClose1 = true,
                type = CommonPopupType.OK,
                text = string.Format(
                                I18nMgr.T("Request failed: [{0}]"),
                                ErrorCode.DEVICE_IOT_COIN_OUT_CACHE_ORDER_IS_NOT_FIND),
                buttonText1 = I18nMgr.T("OK"),
            });


            DebugUtils.LogWarning("iot cache order is not find");

            cacheIOTCoinOutOrder[orderIdIOTCoinOut]["code"] = ErrorCode.DEVICE_IOT_COIN_OUT_CACHE_NOT_FIND;
            cacheIOTCoinOutOrder[orderIdIOTCoinOut]["msg"] = "找不到对应的缓存的订单";
            SQLitePlayerPrefs03.Instance.SetString(DEVICE_IOT_COIN_OUT_ORDER, cacheIOTCoinOutOrder.ToString());
            OnOverIOTCoinOut();
        }

    }
    #endregion 
}
