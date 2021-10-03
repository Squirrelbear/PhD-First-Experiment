package com.example.peter.touchnetworkapp;

import android.util.Log;

import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;

public class Sender implements Runnable {
    public static int tapCount = 0;
    public static int SERVER_PORT = 8011;
    public static String SERVER_IP = "192.168.43.79"; // "PeterOGNLAPTOP";//

    private String nextMessage;

    public Sender(String nextMessage) {
        this.nextMessage = nextMessage;
    }

    @Override
    public void run() {
        try {
            if(nextMessage.equals("Touched")) {
                tapCount++;
                nextMessage += tapCount;
            }
            // send message to Pi
            InetAddress serverAddr = InetAddress.getByName(SERVER_IP);
            DatagramSocket clientSocket = new DatagramSocket();
            byte[] sendData = new byte[1024];
            String sentence = nextMessage;//"tap" + tapCount;
            Log.d("Sender", "Sending message: " + sentence);
            sendData = sentence.getBytes();
            DatagramPacket sendPacket = new DatagramPacket(sendData, sendData.length, serverAddr, SERVER_PORT);
            clientSocket.send(sendPacket);
/*
            // get reply back from Pi
            byte[] receiveData1 = new byte[1024];
            DatagramPacket receivePacket = new DatagramPacket(receiveData1, receiveData1.length);
            clientSocket.receive(receivePacket);
*/
            clientSocket.close();
        }
        catch (Exception e) {
            //e.printStackTrace();
        }
    }
}