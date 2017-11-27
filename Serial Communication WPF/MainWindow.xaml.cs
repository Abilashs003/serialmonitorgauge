using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Concurrent;

namespace Serial_Communication_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Serial 
        SerialPort serial = new SerialPort();
        
        public ConcurrentQueue<string> serialDataQueue = new ConcurrentQueue<string>();
        public ConcurrentQueue<string> serialFirstDataQueue = new ConcurrentQueue<string>();
        public ConcurrentQueue<string> serialSecondDataQueue = new ConcurrentQueue<string>();


        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();
            //overwite to ensure state
            Connect_btn.Content = "Connect";
        }

        private void Connect_Comms(object sender, RoutedEventArgs e)
        {
            if ((string)Connect_btn.Content == "Connect")
            {
                if (!(string.IsNullOrEmpty(Comm_Port_Names.Text) && string.IsNullOrEmpty(Baud_Rates.Text)))
                {
                    //Sets up serial port
                    serial.PortName = Comm_Port_Names.Text;
                    serial.BaudRate = Convert.ToInt32(Baud_Rates.Text);
                    serial.Handshake = System.IO.Ports.Handshake.None;
                    serial.Parity = Parity.None;
                    serial.DataBits = 8;
                    serial.StopBits = StopBits.One;
                    serial.ReadTimeout = 200;
                    serial.WriteTimeout = 50;
                    serial.Open();                  
                    Connect_btn.Content = "Disconnect";
                    serial.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Recieve);
                }
                else
                {
                    MessageBox.Show("Select port and baudrate");
                }

            }
            else
            {
                try // just in case serial port is not open could also be acheved using if(serial.IsOpen)
                {
                    serial.Close();
                    Connect_btn.Content = "Connect";
                }
                catch
                {
                }
            }
        }

        #region Recieving

        private delegate void UpdateUiTextDelegate(ConcurrentQueue<string> text);
        private void Recieve(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            int bytesAvailable = serial.BytesToRead;
            byte[] recBuf = new byte[bytesAvailable];
            

            //string s= System.Text.ASCIIEncoding.ASCII.GetString(recBuf);

            //


            string s = serial.ReadLine();
            serialDataQueue.Enqueue(s);
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), serialDataQueue);

            //if (s.Contains('s'))
            //{
            //    //s.Remove(0);
                
            //}
            //else if (s.Contains('a'))
            //{
            //    //s.Remove(0);
            //    serialFirstDataQueue.Enqueue(s);
            //    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), serialFirstDataQueue);
            //}
            //else if (s.Contains('d'))
            //{
            //    //s.Remove(0);
            //    serialSecondDataQueue.Enqueue(s);
            //    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WriteData), serialSecondDataQueue);
            //}
        }
        private void WriteData(ConcurrentQueue<string> text)
        {
            string ch;

            try
            {
                string set = string.Empty; ;
                while (serialDataQueue.TryDequeue(out ch))
                {
                    if (ch != "\n" & ch != "\r")
                    {
                        if (ch.Contains('s'))
                        {
                            ch=ch.Remove(0,1);
                            this.arcvalue.EndAngle = this.arcvalue.StartAngle + Convert.ToDouble(ch) * 2;
                            this.ReadingValue.Text = ch.ToString();
                        }
                        else if (ch.Contains('d'))
                        {
                            ch=ch.Remove(0, 1);
                            this.arcvalue1.EndAngle = this.arcvalue.StartAngle + Convert.ToDouble(ch) * 2;
                            this.ReadingValue1.Text = ch.ToString();
                        }
                        else if (ch.Contains('a'))
                        {
                            ch=ch.Remove(0, 1);
                            this.arcvalue2.EndAngle = this.arcvalue.StartAngle + Convert.ToDouble(ch) * 2;
                            this.ReadingValue2.Text = ch.ToString();
                        }
                        
                    }
                    
                }
            }

            catch (Exception ex)
            {
            }
                    //// Assign the value of the recieved_data to the RichTextBox.
                    ////para.Inlines.Add(text);
                    ////mcFlowDoc.Blocks.Add(para);
                    // ;
            
        }

        #endregion


        #region Sending        
        
        //private void Send_Data(object sender, RoutedEventArgs e)
        //{
        //    SerialCmdSend(SerialData.Text);
        //    SerialData.Text = "";
        //}
        //public void SerialCmdSend(string data)
        //{
        //    if (serial.IsOpen)
        //    {
        //        try
        //        {
        //            // Send the binary data out the port
        //            byte[] hexstring = Encoding.ASCII.GetBytes(data);
        //            //There is a intermitant problem that I came across
        //            //If I write more than one byte in succesion without a 
        //            //delay the PIC i'm communicating with will Crash
        //            //I expect this id due to PC timing issues ad they are
        //            //not directley connected to the COM port the solution
        //            //Is a ver small 1 millisecound delay between chracters
        //            foreach (byte hexval in hexstring)
        //            {
        //                byte[] _hexval = new byte[] { hexval }; // need to convert byte to byte[] to write
        //                serial.Write(_hexval, 0, 1);
        //                Thread.Sleep(1);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            para.Inlines.Add("Failed to SEND" + data + "\n" + ex + "\n");
        //            mcFlowDoc.Blocks.Add(para);
        //            Commdata.Document = mcFlowDoc;
        //        }
        //    }
        //    else
        //    {
        //    }
        //}

        #endregion


    }
}
