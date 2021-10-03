using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net.NetworkInformation;
using System;
using System.IO;

public class ServerBehaviour : MonoBehaviour {

    IPEndPoint ipEP;
    UdpClient client;
    private const int PORT = 8011;
    public volatile bool tapped;
    public volatile bool stopThread;
    public volatile float timeSinceLastMsg;
    Thread startClient;
    List<string> eventLog;
    string saveFName = "";

    public GameObject infoPanelObj;

	// Use this for initialization
	void Start () {
        //string ipaddress = Network.player.ipAddress;
        //print("IP: " + ipaddress);
        eventLog = new List<string>();
        DisplayIPAddresses();
        // TODO save IP to logs

        tapped = false;
        stopThread = true;
        ipEP = new IPEndPoint(IPAddress.Any, PORT);

        infoPanelObj.GetComponent<Renderer>().enabled = false;
        //client = new UdpClient(PORT);
        //startClient = new Thread(new ThreadStart(StartServer));
        //startClient.Start();
        //StartReceiving();
	}
	
	// Update is called once per frame
	void Update () {
        if (!stopThread)
        {
            timeSinceLastMsg += Time.deltaTime;
            if (timeSinceLastMsg > 10)
            {
                infoPanelObj.GetComponent<Renderer>().enabled = true;
                timeSinceLastMsg = 11;
            }
            else
            {
                infoPanelObj.GetComponent<Renderer>().enabled = false;
            }
        }
	}

    void OnLevelWasLoaded(int level)
    {
        infoPanelObj = GameObject.Find("NetworkStatusInfo");
        infoPanelObj.GetComponent<Renderer>().enabled = false;
    }


    // http://stackoverflow.com/questions/14860613/how-to-use-asynchronous-receive-for-udpclient-in-a-loop
    /*public void StartServer()
    {
        try
        {
            while (!stopThread)
            {
                byte[] recData = client.Receive(ref ipEP);

                System.Text.ASCIIEncoding encode = new System.Text.ASCIIEncoding();
                string data = encode.GetString(recData);
                print("Data recieved: " + data);
                tapped = true;
            }
        }
        catch { }
    }*/

    public void StartReceiving()
    {
        string now = DateTime.Now.ToString();
        eventLog.Add(now + " Starting to Recieve.");
        client = new UdpClient(ipEP);
        stopThread = false;
        Receive(); // initial start of our "loop"
    }

    public void StopReceiving()
    {
        string now = DateTime.Now.ToString();
        eventLog.Add(now + " Stopping Recieve.");
        //print("Stopping Recieve.");
        stopThread = true;
        if (client != null)
        {
            client.Client.Close();
        }
        infoPanelObj.GetComponent<Renderer>().enabled = false;
    }

    private void Receive()
    {
        //print("Beginning recieve");
        client.BeginReceive(new AsyncCallback(MyReceiveCallback), null);
    }

    private void MyReceiveCallback(IAsyncResult result)
    {
        //print("Got something.");
        //IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
        byte[] read = client.EndReceive(result, ref ipEP);

        if (read.Length > 0)
        {
            string data = System.Text.Encoding.ASCII.GetString(read);
            string now = DateTime.Now.ToString();
            eventLog.Add(now + " " + data);
            print(now + " " + data);

            if (data.Equals("Alive"))
            {
                timeSinceLastMsg = 0;
            }
            else
            {
                tapped = true;
            }
        }

        if (!stopThread)
        {
            Receive(); // <-- this will be our loop
        }
    }

    public void setSaveFName(string fName) {
        this.saveFName = fName;
    }

    void OnApplicationQuit()
    {
        StopReceiving();

        if (eventLog.Count > 0 && saveFName.Length > 0)
        {
            using (StreamWriter file = new StreamWriter(saveFName + "_network.dat"))
            {
                foreach (string line in eventLog)
                {
                    file.WriteLine(line);
                }
            }
        }
    }

    /// <summary> 
    /// This utility function displays all the IP (v4, not v6) addresses of the local computer. 
    /// </summary> 
    public void DisplayIPAddresses()
    {
        StringBuilder sb = new StringBuilder();

        // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection) 
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface network in networkInterfaces)
        {
            // Read the IP configuration for each network 
            IPInterfaceProperties properties = network.GetIPProperties();

            // Each network interface may have multiple IP addresses 
            foreach (IPAddressInformation address in properties.UnicastAddresses)
            {
                // We're only interested in IPv4 addresses for now 
                if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                    continue;

                // Ignore loopback addresses (e.g., 127.0.0.1) 
                if (IPAddress.IsLoopback(address.Address))
                    continue;

                sb.AppendLine(address.Address.ToString() + " (" + network.Name + ")");
            }
        }

        print(sb.ToString());
        eventLog.Add("IP Address Info: " + sb.ToString());
    }
}
