﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using UnityEngine;
using System.Net;
using System.Linq;

namespace HD
{
  public class TcpConnectedClient
  {
    #region Data
    /// <summary>
    /// For Clients, the connection to the server.
    /// For Servers, the connection to a client.
    /// </summary>
    readonly TcpClient connection;

    readonly byte[] readBuffer = new byte[5000];

    NetworkStream stream
    {
      get
      {
        return connection.GetStream();
      }
    }
    #endregion

    #region Init
    public TcpConnectedClient(TcpClient tcpClient)
    {
      this.connection = tcpClient;
      this.connection.NoDelay = true; // Disable Nagle's cache algorithm
      if(TCPChat.instance.isServer)
      { // Client is awaiting EndConnect
        stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
      }
    }

    internal void Close()
    {
      connection.Close();
    }
    #endregion

    #region Async Events
    void OnRead(IAsyncResult ar)
    {
      int length = stream.EndRead(ar);
      if(length <= 0)
      { // Connection closed
        TCPChat.instance.OnDisconnect(this);
        return;
      }
      byte[] tmp = new byte[length];
      System.Buffer.BlockCopy(readBuffer, 0, tmp, 0, length);
      TCPChat.instance.OnRead(this,tmp);
           
      /*string newMessage = System.Text.Encoding.UTF8.GetString(readBuffer, 0, length);
      /*TCPChat.messageToDisplay += newMessage + Environment.NewLine;

      if(TCPChat.instance.isServer)
      {
        TCPChat.BroadcastChatMessage(newMessage);
      }*/
      
      stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    internal void EndConnect(IAsyncResult ar)
    {
      connection.EndConnect(ar);

      stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }
        #endregion

        #region API
        internal byte[] toQString(string message) {
    
            byte[] buffer = System.Text.Encoding.BigEndianUnicode.GetBytes(message);
            byte[] len = BitConverter.GetBytes(buffer.Length);
            Array.Reverse(len);
            byte[] rv = new byte[len.Length + buffer.Length];
            System.Buffer.BlockCopy(len, 0, rv, 0, len.Length);
            System.Buffer.BlockCopy(buffer, 0, rv, len.Length, buffer.Length);
            return rv;
        }
    internal void Send(string message)
    {

            byte[] buffer = toQString(message);
            byte[] len = BitConverter.GetBytes(buffer.Length + 4);
            Array.Reverse(len);

            byte[] rv = new byte[len.Length + buffer.Length];
            System.Buffer.BlockCopy(len, 0, rv, 0, len.Length);
            System.Buffer.BlockCopy(buffer, 0, rv, len.Length, buffer.Length);
            //byte[] rv = toQString(message);

            Debug.Log("Length: "+rv.Length);
            for (var i = 0; i< rv.Length; i++) {
                Debug.Log(rv[i]);
            }
            stream.Write(rv, 0, rv.Length);
    }
    internal void Send(byte[] message)
    {
        stream.Write(message, 0, message.Length);
    }
        #endregion
    }
}
