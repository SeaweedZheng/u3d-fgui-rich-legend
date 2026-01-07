using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class ServerWS : MonoBehaviour
{
    private WebSockets.WebSocketServer mServer;

    //主机相关
    private UdpClient mUdpclient = null; //主机和分机的udpclient
    private IPEndPoint endpoint;
    ServerInfo serverinfo;

    private bool IsStop = false;
    private Thread RcvThread = null;

    public void StartServer(int port, int broadcastPort)
    {
        serverinfo = new ServerInfo();
        serverinfo.IP = Utils.LocalIP();
        serverinfo.port = port;
        StartUdp(broadcastPort);
        InitSocket(port);
    }

    public void StopServer()
    {
        if (mServer != null)
        {
            mServer.Stop();
            mServer = null;
        }
    }

    protected void StartUdp(int broadcastPort)
    {
        mUdpclient = new UdpClient(new IPEndPoint(IPAddress.Any, broadcastPort));
        endpoint = new IPEndPoint(IPAddress.Any, 0);
        IsStop = false;
        RcvThread = new Thread(new ThreadStart(ReciveUdpMsg))
        {
            IsBackground = true
        };
        RcvThread.Start();
    }

    public void InitSocket(int port)
    {
        StopServer();
        mServer = new WebSockets.WebSocketServer(IPAddress.Any, port);
        mServer.OnClientConnected += OnClientConnected;
        mServer.Start();
    }


    int recriveId = 0;

    private void ReciveUdpMsg()
    {
        if (++recriveId > 1000)
            recriveId = 0;
        int id = recriveId;

        DebugUtils.LogWarning($"【UDP-WS】==== <color=red>ReciveUdpMsg Start</color> id: {id}");
        //#seaweed# 网络重连

        while (!IsStop && mUdpclient != null)
        {
            try
            {

                //IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);

                DebugUtils.Log("【UDP-WS】wait for next udp msg");
                byte[] buf = mUdpclient.Receive(ref endpoint);   // 这里会一直卡主,直到读取到数据。
                if (buf != null)
                {
                    string msg = Encoding.UTF8.GetString(buf);
                    DebugUtils.Log($"【UDP-WS】<color=yellow>UDP down</color>: {msg}");

                    if (!string.IsNullOrEmpty(msg))
                    {
                        ServerInfo srvInfo = new ServerInfo
                        {
                            IP = serverinfo.IP,
                            port = serverinfo.port
                        };
                        SendUpdMsg(JsonConvert.SerializeObject(srvInfo), endpoint);
                    }
                }
                Thread.Sleep(500); //500毛秒
            }
            catch (Exception e)
            {
                DebugUtils.LogWarning("【UDP-WS】Recive Udp error: " + e.Message);
            }
        }

        DebugUtils.LogWarning($"【UDP-WS】==== <color=red>ReciveUdpMsg End</color> id: {id}");
    }

    //使用udp发送消息
    public void SendUpdMsg(string strMsg, IPEndPoint endPoint)
    {
        try
        {
            if (mUdpclient != null)
            {
                byte[] bf = Encoding.UTF8.GetBytes(strMsg);
                mUdpclient.Send(bf, bf.Length, endPoint);
                DebugUtils.Log($"【UDP-WS】<color=green>UDP up</color>: {strMsg}"); //#seaweed#
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void OnClientConnected(WebSockets.ClientConnection client)
    {
        client.ReceivedTextualData += OnReceivedTextualData;
        client.Disconnected += OnClientDisconnected;
        client.StartReceiving();

        DebugUtils.Log(string.Format("【UDP-WS】Client {0} Connected...", client.Id));
    }

    private void OnClientDisconnected(WebSockets.ClientConnection client)
    {
        client.ReceivedTextualData -= OnReceivedTextualData;
        client.Disconnected -= OnClientDisconnected;
        DebugUtils.Log(string.Format("【UDP-WS】Client {0} Disconnected...", client.Id));
        EventCenter.Instance.EventTrigger(EventHandle.PLAYER_DISCONNECT, client);
    }

    private void OnReceivedTextualData(WebSockets.ClientConnection client, string data)
    {
        WSSrvMsgData wmd = new WSSrvMsgData
        {
            Client = client,
            Data = data
        };
        Loom.QueueOnMainThread((wmd) =>
        {
            Messenger.Broadcast<WSSrvMsgData>(MessageName.Event_NetworkWSServerData, (WSSrvMsgData)wmd);
            wmd = null;
        }, wmd);
    }

    public void SendToClient(WebSockets.ClientConnection client, string msg)
    {
        client.Send(msg);
        OnDebugUtils(msg, false);
    }

    public void SendToAllClient(string msg)
    {
        if (mServer != null)
        {
            mServer.SendToAllClient(msg);
            OnDebugUtils(msg, false);
        }
    }

    private void OnDestroy()
    {
        IsStop = true;
        if (RcvThread != null)
        {
            RcvThread.Abort();
            RcvThread = null;
        }
        // StopCoroutine(CheckHostServerInfo(3.0f));
        if (mUdpclient != null)
        {
            mUdpclient.Close();
            mUdpclient = null;
        }
        StopServer();
    }

    public void OnDebugUtils(string strMsg, bool C2S = true)
    {
        try
        {
            string cmdValue = strMsg.Split(new[] { "\"cmd\":" }, StringSplitOptions.None)[1].Split(',')[0].Trim();
            string rpcName = C2S ?
               $"{Enum.GetName(typeof(C2S_CMD), (C2S_CMD)(int.Parse(cmdValue)))} -" :
                $"{Enum.GetName(typeof(S2C_CMD), (S2C_CMD)(int.Parse(cmdValue)))} -";

            DebugUtils.LogWarning($"【UDP-WS】WS/{rpcName} -  {strMsg}");
        }
        catch (Exception ex) { }
    }
}
