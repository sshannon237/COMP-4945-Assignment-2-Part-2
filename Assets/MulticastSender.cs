using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using SnakeBehaviour;
using UnityEngine.SceneManagement;

namespace MulticastSend {
    //Worked on By Christopher Spooner, Ethan Sadowski, and Sam Shannon
    public class MulticastSender : MonoBehaviour {
        ClientWebSocket ws;
        CancellationTokenSource source;

        void Start() {

        }

        public void setSocket(ClientWebSocket ws, CancellationTokenSource source)
        {
            this.ws = ws;
            this.source = source;
        }
        public async void send(string snakeInfo) {
            if (this.ws != null)
            {
                try
                {

                    if (ws.State == WebSocketState.Open)
                    {
                        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(snakeInfo));
*/                        await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, source.Token);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            

        }
        // Update is called once per frame
        void Update() {
        }
    }
}
