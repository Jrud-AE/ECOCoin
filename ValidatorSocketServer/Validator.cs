using EcoCoinSharedTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ValidatorSocketServer
{
    internal class Validator
    {
        private Socket ValidatorSocket;
        private Guid gValidatorID;
        private string sIPAddress;
        private string sCountry;
        private string sRegionName;
        private string sCity;
        private string sPostalCode;
        private string sLat;
        private string sLon;
        private string sISP;
        private bool bMobile;
        private bool bProxy;
        private bool bHosted;

        private Thread ReceiveThread;

        internal Validator(Socket ValidatorSocket)
        {
            this.ValidatorSocket = ValidatorSocket;
            this.IPAddress = ValidatorSocket.RemoteEndPoint.ToString();

            WebRequest WR = WebRequest.Create("http://ip-api.com/json/" + this.IPAddress + "?fields=status,message,country,regionName,city,district,zip,lat,lon,isp,mobile,proxy,hosting");

            using (WebResponse Resp = WR.GetResponse())
            {
                using (System.IO.StreamReader SR = new System.IO.StreamReader(Resp.GetResponseStream()))
                {
                    string ResponseText = SR.ReadToEnd();
                    dynamic ResponseJson = Newtonsoft.Json.JsonConvert.DeserializeObject(ResponseText);

                    if (ResponseJson.status == "success")
                    {
                        this.Country = ResponseJson.country;
                        this.RegionName = ResponseJson.regionName;
                        this.City = ResponseJson.city;
                        this.PostalCode = ResponseJson.zip;
                        this.Lat = ResponseJson.lat;
                        this.Lon = ResponseJson.lon;
                        this.ISP = ResponseJson.isp;
                        this.bMobile = ResponseJson.mobile;
                        this.bProxy = ResponseJson.proxy;
                        this.bHosted = ResponseJson.hosting;
                    }

                    if (this.bProxy)
                    {
                        throw new Exception("Validator connection rejected: Proxy connections are not allowed.");
                    }
                }
            }

            ReceiveThread = new Thread(new ThreadStart(ReceiveData));
            ReceiveThread.Start();
        }

        private void ReceiveData()
        {
            List<byte> messageBuffer = new List<byte>();

            while (ValidatorSocket.Connected)
            {
                try
                {
                    byte[] buffer = new byte[ValidatorSocket.Available];

                    if (buffer.Length == 0)
                    {
                        buffer = new byte[1];
                    }

                    ValidatorSocket.Receive(buffer);

                    foreach (byte b in buffer)
                    {
                        messageBuffer.Add(b);

                        if (System.Text.UTF8Encoding.UTF8.GetString(messageBuffer.ToArray()).EndsWith("☺"))
                        {
                            messageBuffer.RemoveAt(messageBuffer.Count - 1);
                            messageBuffer.RemoveAt(messageBuffer.Count - 1);
                            messageBuffer.RemoveAt(messageBuffer.Count - 1);

                            Console.WriteLine("JSON Parsing TVRE: " + System.Text.UTF8Encoding.UTF8.GetString(messageBuffer.ToArray()));

                            TransactionValidationResponseEnvelope TVRE = System.Text.Json.JsonSerializer.Deserialize<TransactionValidationResponseEnvelope>(messageBuffer.ToArray());

                            messageBuffer.Clear();


                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Received validation response from validator " + sIPAddress + ":");
                            Console.WriteLine("    ID: " + TVRE.ValidationResponse.TransactionID);
                            Console.WriteLine("    Approval: " + TVRE.ValidationResponse.Approved.ToString());
                            if (!TVRE.ValidationResponse.Approved)
                            {
                                Console.WriteLine("    Deny Reason: " + TVRE.ValidationResponse.DenyReason);
                            }

                            Controller.ProcessValidatorResponse(TVRE, this);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "An existing connection was forcibly closed by the remote host." || ex.Message == "A request to send or receive data was disallowed because the socket is not connected and (when sending on a datagram socket using a sendto call) no address was supplied.")
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Validator dropped connection");
                        try
                        {
                            Controller.ValidatorConnections.Remove(this);
                        }
                        catch (Exception ex2)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error removing validator connection from controller list: " + ex2.Message);
                        }

                        try
                        {
                            ValidatorSocket.Dispose();
                        }
                        catch (Exception ex2)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error disposing validator connection: " + ex2.Message);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error while receiving validation response from validator: " + ex.Message);
                    }
                }
            }
        }

        internal int Send(byte[] Message)
        {
            int BytesSent =  ValidatorSocket.Send(Message);

            ValidatorSocket.Send(new byte[] { 0x01 });

            return BytesSent;
        }

        public Guid ValidatorID
        {
            get { return gValidatorID; }
            set { gValidatorID = value; }
        }
        public string IPAddress
        {
            get { return sIPAddress; }
            set { sIPAddress = value; }
        }
        public string Country
        {
            get { return sCountry; }
            set { sCountry = value; }
        }
        public string RegionName
        {
            get { return sRegionName; }
            set { sRegionName = value; }
        }
        public string City
        {
            get { return sCity; }
            set { sCity = value; }
        }
        public string PostalCode
        {
            get { return sPostalCode; }
            set { sPostalCode = value; }
        }
        public string Lat
        {
            get { return sLat; }
            set { sLat = value; }
        }
        public string Lon
        {
            get { return sLon; }
            set { sLon = value; }
        }
        public string ISP
        {
            get { return sISP; }
            set { sISP = value; }
        }
        public bool Mobile
        {
            get { return bMobile; }
            set { bMobile = value; }
        }
        public bool Proxy
        {
            get { return bProxy; }
            set { bProxy = value; }
        }
        public bool Hosted
        {
            get { return bHosted; }
            set { bHosted = value; }
        }
    }
}