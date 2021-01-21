using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ArcheryXServer
{
    public class ArcheryXServer
    {
        enum EnumGameType
        {
            Classic,
            Rush
        }

        enum EnumGameDistance
        {
            M18,
            M30,
            M50,
            M70
        }

        enum EnumGameTurn
        {
            Both,
            Left,
            Right
        }

        enum EnumMessageType
        {
            QuickMatch, //When user does not specify the game parameters. We match him with anyone waiting for match.
            MatchRequest, //User specifies Game type and distance.
            StartGame,
            Shoot
        }

        class Player
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public Socket Socket { get; set; }

        }

        class Game
        {
            public EnumGameType Type { get; set; }
            public EnumGameTurn Turn { get; set; }
            public int TotalRound { get; set; }
            public int CurrentRound { get; set; }
            public int TotalArrows { get; set; } //0 = unlimited
            public Player PlayerLeft { get; set; }
            public Player PlayerRight { get; set; }
            public int[] PlayerLeftPoints { get; set; }
            public int[] PlayerRightPoints { get; set; }

        }

        static Dictionary<EnumGameType, Dictionary<EnumGameDistance, List<Player>>> WaitingDictionary = new Dictionary<EnumGameType, Dictionary<EnumGameDistance, List<Player>>>();
        static List<Game> GameList = new List<Game>();

        static List<Player> PlayerList = new List<Player>();

        public static void Main()
        {
            TcpListener tcpListener = new TcpListener(new IPAddress(new byte[] { 127, 0, 0, 1 }), 8888);
            tcpListener.Start();

            Console.WriteLine("Server is listening...");

            while (true)
            {
                Socket client = tcpListener.AcceptSocket();


            if (!client.Connected)
            {
                Console.WriteLine("Sunucu baslatilamiyor...");
            }
            else
            {
                Player newPlayer = new Player();
                newPlayer.Socket = client;
                PlayerList.Add(newPlayer);
                Console.WriteLine("new Player connected!");

                    while (true)
                {

                    NetworkStream networkStream = new NetworkStream(client);

                    StreamWriter streamWriter = new StreamWriter(networkStream);
                    StreamReader streamReader = new StreamReader(networkStream);

                    try
                    {
                        string data = streamReader.ReadLine();

                        Console.WriteLine("Gelen Bilgi:" + data);

                        streamWriter.WriteLine(data.Length.ToString());

                        streamWriter.Flush();
                    }

                    catch
                    {
                        Console.WriteLine("Sunucu kapatiliyor...");
                        return;
                    }
                }
                }
            }
        }
    }
}