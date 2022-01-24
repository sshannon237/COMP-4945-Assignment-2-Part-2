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
        //Uri serverUri;
        //CancellationTokenSource source;

        void Start() {
            //this.ws = new ClientWebSocket();
            //this.serverUri = new Uri("ws://localhost:80/ws.ashx");
            //this.source = new CancellationTokenSource();
            //this.source.CancelAfter(5000);
            //ws.ConnectAsync(serverUri, source.Token);

        }

        public void setSocket(ClientWebSocket ws, CancellationTokenSource source)
        {
            this.ws = ws;
            this.source = source;
        }
        public async void send(string snakeInfo) {
            //try {

            //    Debug.Log(snakeInfo);


            //    ArraySegment<byte> bytesToSend =
            //          new ArraySegment<byte>(Encoding.UTF8.GetBytes(snakeInfo));
            //    Debug.Log(bytesToSend);
            //    Debug.Log(ws);
            //    Debug.Log(source);
            //    await ws.SendAsync(bytesToSend, WebSocketMessageType.Text,
            //                         true, source.Token);

            //} catch(Exception e) {
            //    //Debug.Log("\n" + e.Message);
            //}
            if (this.ws != null)
            {
                try
                {


                    if (ws.State == WebSocketState.Open)
                    {
                        ArraySegment<byte> bytesToSend =
                                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(snakeInfo));
/*                        Debug.Log(snakeInfo);
*/                        await ws.SendAsync(bytesToSend, WebSocketMessageType.Text,
                                             true, source.Token);
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
