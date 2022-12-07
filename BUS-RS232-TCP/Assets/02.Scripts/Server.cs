using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;

public class Server : MonoBehaviour
{
    private static Server instance = null;
    private void Awake()
    {
        if (null == instance) instance = this;
        else Destroy(gameObject);
    }

    public static Server Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public TMP_InputField PortInput;

    List<ServerClient> clients;
    List<ServerClient> disconnectList;

    TcpListener server;
    bool serverStarted;

    public static List<bool> isStartList = new List<bool>();

	public void ServerCreate()
	{
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();
        
        try
        {
            // 자기 IPv4 주소를 server에 입력
            // Client ConnectToServer 쪽에서도 자기 ip로 입력해서 들어오게 하기       
            
            int port = PortInput.text == "" ? 7777 : int.Parse(PortInput.text);

            // 집에서 할 시
            //server = new TcpListener(IPAddress.Any, port);

            // 빌드시
            IPAddress ip = IPAddress.Parse("192.168.0.13");
            server = new TcpListener(ip, port);

            server.Start();

            StartListening();
            serverStarted = true;
            Chat.instance.ShowMessage($"서버가 {port}에서 시작되었습니다.");
        }
        catch (Exception e) 
        {
            Chat.instance.ShowMessage($"Socket error: {e.Message}");
        }
	}

	void Update()
	{
        if (!serverStarted) return;

        //clients.Count
        foreach (ServerClient c in clients) 
        {
            // 클라이언트가 여전히 연결되있나?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            // 클라이언트로부터 체크 메시지를 받는다
            else 
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable) 
                {
                    string data = new StreamReader(s, true).ReadLine();
                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }

		for (int i = 0; i < disconnectList.Count - 1; i++)
		{
            Broadcast($"{disconnectList[i].clientName} 연결이 끊어졌습니다", clients);

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
		}
	}	

	bool IsConnected(TcpClient c)
	{
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;
        }
        catch 
        {
            return false;
        }
	}

	void StartListening()
	{
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
	}

    void AcceptTcpClient(IAsyncResult ar) 
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        // 메시지를 연결된 모두에게 보냄
        Broadcast("%NAME", new List<ServerClient>() { clients[clients.Count - 1] });
    }


    void OnIncomingData(ServerClient c, string data)
    {
        if (data.Contains("&NAME")) 
        {
            c.clientName = data.Split('|')[1];
            Broadcast($"{c.clientName}이 연결되었습니다", clients);

            isStartList.Add(false);

            return;
        }

        Broadcast($"{c.clientName} : {data}", clients);
    }

    void Broadcast(string data, List<ServerClient> cl) 
    {
        foreach (var c in cl) 
        {
            try 
            {
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e) 
            {
                Chat.instance.ShowMessage($"쓰기 에러 : {e.Message}를 클라이언트에게 {c.clientName}");
            }
        }
    }

    public void OnStartButton()
    {
        Broadcast("4", clients);
    }
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket) 
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
