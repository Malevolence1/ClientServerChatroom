using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.Sockets;
using System.Text;

public class Message
{
    public int Id {get; set;}
    public string Sender {get; set;}
    public DateTime PostDate {get; set;}
    public string Subject {get; set;}
    public string Content {get; set;}

}

public class ChatServer
{
    private int m_port;
    private int m_serverSocket;
    private List<Thread> m_clientThreads;
    private Mutex m_mutex ;
    private Dictionary<string, Socket> m_clients;
    private Dictionary<string, List<Message>> m_boards;
    private Dictionary<string, List<Tuple<string, bool>>> m_boardMembers;
    private Socket m_listener;

    private int message_id_counter;

    public ChatServer(int port)
    {
        //Consturctor
        //Initialize Global Variables
        m_port = port;
        m_serverSocket = -1;
        message_id_counter = 0;
        m_clientThreads = List<Thread>();
        m_mutex = new Mutex();
        m_clients = new Dictionary<string, Socket>();
        m_boards = new Dictionary<string, List<Message>>();
        m_boardMembers = new Dictionary<string, List<Tuple<string, bool>>>();

        //Initialize ChatServer's Boards
        m_boards["Public:"] = new List<Message>();
        m_boardMembers["Public:"] = new List<Tuple<string, bool>>();
        for (int i=1; i<=5; ++i)
        {
            m_boards["Private" + i.ToString() + ":"] = new List<Message>();
            m_boardMembers["Private" + i.ToString() + ":"] = new List<Tuple<string, bool>>();
        }
    }

    ~ChatServer()
    {
        //Deconstructor
    }

    private void HandleClient(Socket clientSocket)
    {
        // Construct a Connection message and send to the Client
        byte[] messageBytes = Encoding.ASCII.GetBytes("CONNECTED");
        clientSocket.send(messageBytes);

        //Set up a buffer lenght and client variables
        byte[] buffer = new byte[1024];
        string username;
        string currentBoard = "None";

        while (true)
        {
            //Read incoming message from client and convert to string
            int received = clientSocet(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, received);

            //Split the command from the content
            string[] parts = message.Split(new char[] {' '}, 2);

            string command = parts[0];
            string content = parts[1];

            if (command == "LOGIN")
            {
                username = content;

                m_mutex.WaitOne();

                if (m_clients.ContainsKey(username))
                {
                    messageBytes = Encoding.ASCII.GetBytes("ERROR: Username already exists");
                    clientSocket.send(messageBytes);
                }
                else
                {
                    m_clients.Add(username, clientSocket);

                    Tuple<string, bool> c_tuple= new Tuple<string, bool>(username, false);

                    m_boardMembers["Public:"].Add(c_tuple);

                    Console.WriteLine("[CLIENT] Loggen in as : " + username);

                    messageBytes = Encoding.ASCII.GetBytes("SUCCESFULL");
                    clientSocket.send(messageBytes);

                    messageBytes = Encoding.ASCII.GetBytes("[SERVER] New User Joined: " + username);

                    foreach (string user in m_clients.Keys)
                    {
                        if (user != username)
                        {
                            m_clients[user].send(messageBytes);
                        }
                    }

                }
                m_mutex.ReleaseMutex();

                

            }

        }
        else if (command == "POST")
        {
            Message msg = new Message();
            msg.Id = message_id_counter++;
            msg.Sender = username;
            msg.PostDate = DateTime.Now;
            string[] parts = content.Split(new char[] {':'}, 2)
            msg.Subject = parts[0]
            msg.Content = parts[1]

            m_mutex.WaitOne();

            if (currentBoard != "None")
            {
                string messageString = "\n\nMessage ID: " + msg.Id + 
                "\nSender: " + msg.Sender + "\nPost Date: " + msg.PostDate + 
                "\nSubject: " + msg.Subject + "\nContent: " + msg.Content;

                messageBytes = Encoding.ASCII.GetBytes(messageString);

                foreach (string user in m_boardMembers[currentBoard])
                {
                    if (user.Item1 != username && user.Item2)
                    {
                        m_clients[user.Item1].send(messageBytes);
                    }
                }
                m_boards[currentBoard].Add(msg);
            }
            else
            {
                messageBytes = Encoding.ASCII.GetBytes("ERROR: You are not on a board.");
                clientSocket.send(messageBytes);
            }
            m_mutex.ReleaseMutex();

        }
        else if (command == "RET")
        {
            it messageID = int.Parse(content);
            bool msgFound = false;
            
            m_mutex.WaitOne();
            foreach (Message msg in m_boards[currentBoard])
            {
                if (msg.Id == messageID)
                {
                    string messageString = "\n\nMessage ID: " + msg.Id + 
                        "\nSender: " + msg.Sender + "\nPost Date: " + msg.PostDate + 
                        "\nSubject: " + msg.Subject + "\nContent: " + msg.Content;

                    messageBytes = Encoding.ASCII.GetBytes(messageString);
                    clientSocket.send(messageBytes);

                    msgFound = true;
                }
            }

            if (!msgFound)
            {
                messageBytes = Encoding.ASCII.GetBytes("ERROR: Message not found");
                clientSocket.send(messageBytes);
            }

            m_mutex.ReleaseMutex();
        }
        else if (command == "USERS")
        {
            m_mutex.WaitOne();

            string userList = "Users: "

            foreach ( Tuple user in m_boardMembers[currentBoard])
            {
                userList += user.Item1 + "\n"
            }
            messageBytes = Encoding.ASCII.GetBytes(userList);
            clientSocket.send(messageBytes);

            m_mutex.ReleaseMutex();


        }
        else if (command == "BOARD")
        {
            m_mutex.WaitOne();
            string boardList = "All Boards: "
            for (string boardName in m_boards.Keys)
            {
                boardList += boardList + "\n";
            }

            messageBytes = Encodeing.ASCII.GetBytes(boardList);
            clientSocket.send(messageBytes);

            m_mutex.ReleaseMutex();

        }
        else if (command == "JOIN")
        {
            string boardName = content;

            m_mutex.WaitOne();

            if ( m_boards.ContainsKey(boardName))
            {
                bool partOfGroup = false;
                foreach (Tuple user in m_boardMembers[boardName])
                {
                    if(user.Item1 == username)
                    {   
                        partOfGroup = true;
                        Tuple<string,bool> userTuple = new Tuple<string, bool>(username, false);
                        int index = m_boardMembers[boardName].FindIndex(this => t.Equals(userTuple));

                        if (index != -1)
                        {
                            userTuple = new Tuple<string, bool>(username, true);
                            m_boardMembers[boardName][index] = userTuple;

                            currentBoard = boardName;

                            messageBytes = Encoding.ASCII.GetBytes("[SERVER] You are now logged into: " + boardName);
                            clientSocket.send(messageBytes);

                            foreach (Tuple boardUser in m_boardMembers[boardName])
                            {
                                messageBytes = Encoding.ASCII.GetBytes("[SERVER] User " + username + " Joined board " + boardName);

                                if (boardUser.Item1 != username && boardUser.Item2)
                                {
                                    m_clients[boardUser.Item1].send(messageBytes);
                                }
                            }
                        }
                        else
                        {
                            messageBytes = Encoding.ASCII.GetBytes("ERROR: You are already on this board");
                            clientSocket.send(messageBytes);
                        }
                    }
                    
                }

                if (!partOfGroup)
                {
                    messageBytes = Encoding.ASCII.GetBytes("ERROR: You are not a part of the user group");
                    clientSocket.send(messageBytes);
                }
            }
            else
            {
                messageBytes = Encoding.ASCII.GetBytes("ERROR: Board does not Exist");
                clientSocket.send(messageBytes);
            }
            m_mutex.ReleaseMutex();
        }
        else if (command == "GJOIN")
        {
            string groupName = content;

            m_mutex.WaitOne();
            if (m_boardMembers.ContainsKey(groupName))
            {
                if (m_boardMembers[groupName].ContainsKey(new Tuple<string, bool>(username, false) || m_boardMembers[groupName].ContainsKey(new Tuple<string, bool>(username, true)))
                {
                    messageBytes = Encoding.ASCII.GetBytes("ERROR: Already a part of the group");
                    clientSocket.send(messageBytes);
                }
                else
                {
                    m_boardMembers[groupName].Add(new Tuple<string, bool>(username, false));
                    messageBytes = Encoding.ASCII.GetBytes("[SERVER] You joinded Board group: " + groupName);
                    clientSocket.send(messageBytes);
                }
            }
            else
            {
                messageBytes = Encoding.ASCII.GetBytes("ERROR: Board Group Does not EXIST");
                clientSocket.send(messageBytes);
            }

            m_mutex.ReleaseMutex();


        }
        else if (command == "DISC")
        {
            m_mutex.WaitOne();

            foreach (board in m_boardMembers.Keys)
            {
                if( m_boardMembers[board].Contains(new Tuple<string, bool>(username, false)) || m_boardMembers[m_boardMembers].Contains(new Tuple<string, bool>(username, true)))
                {
                    int index = m_boardMembers[board].FindIndex(t => t.Equals(new Tuple<string, bool>(username,false) || new Tuple<string, bool>(username, true)));
                    if (index != -1)
                    {
                        m_boardMembers[board].RemoveAt(index)
                    }
                }
            }

            if (m_clients.ContainsKey(username))
            {
                m_clients.Remove(username);
            }

            messageBytes = Encoding.ASCII.GetBytes("[SERVER] User " + username + "Disconnected.")
        
            foreach ( string user in m_clients.Keys)
            {
                m_clients[user].send(messageBytes);      
            }

            m_mutex.ReleaseMutex();

            messageBytes = Encoding.ASCII.GetBytes("[SERVER] You have disconnected from server.");
            clientSocket.send(messageBytes);

            clientSocket.Shutdown(SocketShutown.Both);
            clientSocket.Close();
            break;
        }
        else
        {
            messageBytes = Encoding.ASCII.GetBytes("ERROR: Command is wrong");
            clientSocket.send(messageBytes);
        }
        


    }

    public void Setup()
    {
        m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, m_port):

        try
        {
            {
                m_listener.Bind(localEndPoint);

                Console.WriteLine("[STARTUP] Server Starting Port: " + m_port);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error Creatign or Binding Socket: " + e.ToString());
            
            throw;
        }

    }

    public void Run()
    {
        _serverStateThread = new Thread(ServerState);
        _serverStateThread.Start();

        m_listener.Listen(10);

        while(true)
        {
            try
            {
                Socket clientSocket = m_listener.Accept();
                Console.WriteLine("[CONNECTION] Client Connected With Socket: " + clientSocket.RemoteEndPoint);

                Thread clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error accepting client connection " + ex.Message);
                continue;
            }
        }

    }

    public void ServerState()
    {
        Console.WriteLine("Server State Not Set Up Yet");
    }