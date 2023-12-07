using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class TCPManager : MonoBehaviour
{
    private InputField _inputIP;
    private InputField _inputPort;

    [SerializeField] private InputField _inputStatus;

    private StreamReader _streamReader;
    private StreamWriter _streamWriter;

    public InputField _inputMessageBox;
    private ChatManager _chatManager;

    private void Update()
    {
        ShowStatus();
    }

    private Queue<string> log = new Queue<string>();
    private void ShowStatus()
    {
        if (log.Count > 0)
            _inputStatus.text = log.Dequeue();
    }

    #region Server
    public void OpenServer()
    {
        _chatManager = FindObjectOfType<ChatManager>();
        Thread thread = new Thread(ConnectServer);
        thread.IsBackground = true;
        thread.Start();
    }

    /// <summary>
    /// 서버를 생성하고 연결 대기
    /// </summary>
    private void ConnectServer()
    {
        try
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(_inputIP.text), int.Parse(_inputPort.text));
            listener.Start();
            log.Enqueue("Server opened");

            TcpClient client = listener.AcceptTcpClient();
            log.Enqueue($"Client {client.Client} connected to server");

            _streamReader = new StreamReader(client.GetStream());
            _streamWriter = new StreamWriter(client.GetStream())
            {
                AutoFlush = true
            };

            while (client.Connected)
            {
                string readData = _streamReader.ReadLine();
                _chatManager.OnMessage(readData);
            }
        }
        catch (Exception e)
        {
            log.Enqueue(e.Message);
            throw;
        }
    }
    #endregion

    #region Client
    public void OpenClient()
    {
        _chatManager = FindObjectOfType<ChatManager>();
        Thread thread = new Thread(ConnectClient);
        thread.IsBackground = true;
        thread.Start();
    }

    private void ConnectClient()
    {
        try
        {
            TcpClient client = new TcpClient();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_inputIP.text), int.Parse(_inputPort.text));
            client.Connect(endPoint);
            log.Enqueue($"Client connecting to {endPoint.Address}:{endPoint.Port}");

            _streamReader = new StreamReader(client.GetStream());
            _streamWriter = new StreamWriter(client.GetStream())
            {
                AutoFlush = true
            };

            while (client.Connected)
            {
                string readData = _streamReader.ReadLine();
                _chatManager.OnMessage(readData);
            }
        }
        catch (Exception e)
        {
            log.Enqueue(e.Message);
            throw;
        }
    }
    #endregion

    public void BtnSend()
    {
        string message = $"{SQLManager.UserInfo.ID} : {_inputMessageBox.text}";
        if (TrySendMessage(message))
        {
            _chatManager.OnMessage(message);
            _inputMessageBox.text = string.Empty;
        }
    }

    private bool TrySendMessage(string message)
    {
        if (_streamWriter != null)
        {
            _streamWriter.WriteLine(message);
            return true;
        }
        else
        {
            Debug.Log("Writer is null");
            return false;
        }
    }
}
