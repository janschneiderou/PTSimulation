using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PTSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread storingAndSending;
        bool isRunning = true;
        public ConnectorHub.ConnectorHub myConectorHub;
        public ConnectorHub.FeedbackHub myFeedbackHub;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                myConectorHub = new ConnectorHub.ConnectorHub();

                myConectorHub.Init();
                setValuesNames();
                myConectorHub.SendReady();
                
                myConectorHub.StartRecordingEvent += MyConectorHub_startRecordingEvent;
                myConectorHub.StopRecordingEvent += MyConectorHub_stopRecordingEvent;
                myFeedbackHub = new ConnectorHub.FeedbackHub();
                myFeedbackHub.Init();
                myFeedbackHub.FeedbackReceivedEvent += MyFeedbackHub_FeedbackReceivedEvent;
               

            }
            catch (Exception e)
            {
                int x = 1;
            }

            storingAndSending = new Thread(new ThreadStart(storingAndSendingStart));
            storingAndSending.Start();


        }

        private void MyFeedbackHub_FeedbackReceivedEvent(object sender, string feedback)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    feedbackLabel.Content = feedback;
                }
                catch
                {

                }

            });
        }

        private void MyConectorHub_stopRecordingEvent(object sender)
        {
            
        }

        private void MyConectorHub_startRecordingEvent(object sender)
        {
            
        }

        private void setValuesNames()
        {
            List<string> names = new List<string>();
            string temp = "feedback";
            names.Add(temp);
            myConectorHub.SetValuesName(names);
        }

        private void storingAndSendingStart()
        {
            while(isRunning)
            {
                string feedbackString = "";


                List<string> values = new List<string>();

                Dispatcher.Invoke(() =>
                {

                    if (rPosture.IsChecked == true)
                    {
                        feedbackString = "Reset Posture";
                    }
                    else if (rStayStill.IsChecked == true)
                    {
                        feedbackString = "Stand Still";
                    }
                    else if (rGesture.IsChecked == true)
                    {
                        feedbackString = "Move Hands";
                    }
                    else if (rSpeakLouder.IsChecked == true)
                    {
                        feedbackString = "Speak Louder";
                    }
                    else if (rSpeakSofter.IsChecked == true)
                    {
                        feedbackString = "Speak Softer";
                    }
                    else if (rStopSpeaking.IsChecked == true)
                    {
                        feedbackString = "Stop Speaking";
                    }
                    else if (rStartSpeaking.IsChecked == true)
                    {
                        feedbackString = "Start Speaking";
                    }
                    else if (rStopHmm.IsChecked == true)
                    {
                        feedbackString = "Stop Hmmmm";
                    }
                    else if (rStayStill.IsChecked == true)
                    {
                        feedbackString = "Smile";
                    }
                    else if (rSmile.IsChecked == true)
                    {
                        feedbackString = "Reset Posture";
                    }
                    else if (rgood.IsChecked == true)
                    {
                        feedbackString = "Good!!!";
                    }
                });

                try
                {
                    Socket udpSendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPAddress serverAddr = IPAddress.Parse("192.168.178.23");
                    IPEndPoint UDPendPoint = new IPEndPoint(serverAddr, 16002);
                    byte[] send_buffer = Encoding.ASCII.GetBytes(feedbackString);
                    udpSendingSocket.SendTo(send_buffer, UDPendPoint);
                }
                catch
                {

                }

                try
                {
                    values.Add(feedbackString);
                    myConectorHub.StoreFrame(values);

                    myConectorHub.SendFeedback(feedbackString);

                  

                }
                catch
                {

                }

                
                Thread.Sleep(1000);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            isRunning = false;
            myConectorHub.Close();
            

        }
    }
}
