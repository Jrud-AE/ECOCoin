using EcoCoinSharedTypes;
using System;
using System.Collections.Generic;
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
            byte[] buffer = new byte[1024];
            int bytesRead;
            while (true)
            {
                try
                {
                    bytesRead = ValidatorSocket.Receive(buffer);
                    if (bytesRead > 0)
                    {
                        byte[] receivedData = new byte[bytesRead];
                        Array.Copy(buffer, receivedData, bytesRead);

                        TransactionValidationResponseEnvelope TVRE = System.Text.Json.JsonSerializer.Deserialize<TransactionValidationResponseEnvelope>(receivedData);

                        Controller.ProcessValidatorResponse(TVRE);
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Socket exception: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
        }

        internal int Send(byte[] Message)
        {
            return ValidatorSocket.Send(Message);
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