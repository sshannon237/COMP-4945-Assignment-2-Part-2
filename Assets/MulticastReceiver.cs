using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.IO;
using SnakeBehaviour;
using UnityEngine.SceneManagement;
using SnakeMovementController;
using SnakeCreation;
using UnityEngine;

namespace MulticastReceive {
    //Worked on By Christopher Spooner, Ethan Sadowski, and Sam Shannon
    public class MulticastReceiver : MonoBehaviour {
        public SnakeMovement snakeMovement;
        public SnakeCreator snakeCreator;
        private static IPAddress mcastAddress;
        private static int mcastPort;
        private static Socket mcastSocket;
        private static MulticastOption mcastOption;
        private Guid id;
        IPAddress localIP;
        EndPoint localEP;
        IPEndPoint groupEP;
        EndPoint remoteEP;
        Thread socketThread;
        bool socketThreadRunning;
        Tuple<Guid, List<Vector2>> newSnakeData;
        List<Vector2> coordinateList;
        Guid parsedUid;
        ClientWebSocket ws;
        CancellationTokenSource source;
        // Start is called before the first frame update
        void Start() {
            
        }

        public void setSocket(ClientWebSocket ws, CancellationTokenSource source)
        {
            this.ws = ws;
            this.source = source;
        }

        public void setId(Guid id) {
            this.id = id;
        }

        public void doListen()
        {
            socketThread = new Thread(listen);
            socketThreadRunning = true;
            socketThread.Start();
        }

        async void listen() {
            try
            {
                while (ws.State == WebSocketState.Open)
                {
                    //Receive buffer
                    var receiveBuffer = new byte[10000];

                    //Multipacket response
                    var offset = 0;
                    var dataPerPacket = 1; //Just for example

                    string receivedSnakeInfo = "";

                    ArraySegment<byte> bytesReceived =
                                new ArraySegment<byte>(receiveBuffer, offset, receiveBuffer.Length);
                    WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived,
                                                                    source.Token);
                    //Partial data received
                    Console.WriteLine("Data:{0}",
                                        Encoding.UTF8.GetString(receiveBuffer, offset, result.Count));
                    receivedSnakeInfo += Encoding.UTF8.GetString(receiveBuffer, offset, result.Count);
                    offset += result.Count;
                    if (result.EndOfMessage)
                    {
                        if (receivedSnakeInfo.Length > 1)
                        {
                            if (checkForDisconnect(receivedSnakeInfo))
                            {
                                Debug.Log(receivedSnakeInfo.Substring(14));
                                Guid snakeId = Guid.Parse(receivedSnakeInfo.Substring(14));
                                removeNetworkSnake(snakeId);
                            } 
                            else
                            {
                                parseSnake(receivedSnakeInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        bool checkForDisconnect(string data)
        {
            if (data.Contains("Remove Player"))
            {
                return true;
            }
            return false;
        }

        void parseSnake(string snakeInfo) {
            // Parse x coordinate of the snake
            int xStart = snakeInfo.IndexOf("xcoordinate: ") + 13;
            int xEnd = snakeInfo.IndexOf("---end-x---");
            int xcoordinate = int.Parse(snakeInfo.Substring(xStart, xEnd - xStart));

            // Parse y coordinate of the snake
            int yStart = snakeInfo.IndexOf("ycoordinate: ") + 13;
            int yEnd = snakeInfo.IndexOf("---end-y---");
            float ycoordinate = float.Parse(snakeInfo.Substring(yStart, yEnd - yStart));

            // Parse UID of the snake
            int uidStart = snakeInfo.IndexOf("uid: ") + 5;
            int uidEnd = snakeInfo.IndexOf("---end-uid---");
            string uid = snakeInfo.Substring(uidStart, uidEnd - uidStart);
            parsedUid = Guid.Parse(uid);

            int bodyStart = snakeInfo.IndexOf("body: ") + 6;
            int bodyEnd = snakeInfo.IndexOf("---end-body---");
            string bodyStr = snakeInfo.Substring(bodyStart, bodyEnd - bodyStart);

            // If the snake is a new connection create a new snake
            bool isNewSnake = !snakeMovement.checkIfSnakeExists(parsedUid);

            // TODO refactor this to create a list of all the snake's coordinates
            coordinateList = new List<Vector2>();
            coordinateList.Add(new Vector2(xcoordinate, ycoordinate));

            string[] bodyArr = bodyStr.Split(' ');
            for(int i = 0; i < bodyArr.Length - 2; i += 2) {
                coordinateList.Add(new Vector2(float.Parse(bodyArr[i]), float.Parse(bodyArr[i + 1])));
            }
            if(isNewSnake && parsedUid != this.id) {

                newSnakeData = new Tuple<Guid, List<Vector2>>(parsedUid, coordinateList);
                executeOnMain();
            } else if(uid != this.id.ToString()) {
                mainThreadUpdate();
            }
        }

        // Update is called once per frame
        void Update() {}

        ~MulticastReceiver() {
            mcastSocket.Close();
        }

        public IEnumerator performDelete(Guid snakeId)
        {
            snakeMovement.deleteSnake(snakeId);
            yield return null;
        }

        public void removeNetworkSnake(Guid snakeId)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(performDelete(snakeId));
        }

        public IEnumerator functionExecution() {
            int bodySize = snakeMovement.getBodySizeById(parsedUid);
            if(bodySize < coordinateList.Count) {
                for(int i = bodySize - 1; i < coordinateList.Count; i++) {
                    GameObject newSnakePart = snakeCreator.createSnakePart(coordinateList[i]);
                    snakeMovement.addSnakePart(parsedUid, newSnakePart);
                }
            }
            if(bodySize > coordinateList.Count) {

                snakeMovement.removeSnakeParts(parsedUid, bodySize - coordinateList.Count);
            }
            snakeMovement.updateSnakeLocation(parsedUid, coordinateList);
            yield return null;
        }

        public void mainThreadUpdate() {
            UnityMainThreadDispatcher.Instance().Enqueue(functionExecution());
        }

        IEnumerator addNetworkSnake()
        {
            GameObject newSnake = snakeCreator.instantiateSnake(newSnakeData.Item1, newSnakeData.Item2);
            newSnake.GetComponent<BoxCollider2D>().isTrigger = false;
            newSnake.GetComponent<BoxCollider2D>().enabled = false;
            bool success = true;
            try
            {
                snakeMovement.addSnake(newSnake);

            } catch (Exception e)
            {
                Destroy(newSnake);
                success = false;
            }
            if (success)
            {
                snakeCreator.instantiateBody(newSnake, newSnakeData.Item1, newSnakeData.Item2);
            }
            yield return null;
        }

        void executeOnMain()
        {
            UnityMainThreadDispatcher.Instance().Enqueue(addNetworkSnake());
        }

    }
}