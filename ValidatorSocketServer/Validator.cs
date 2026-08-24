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
        private string IPAddress;
        private string Country;
        private string RegionName;
        private string City;
        private string PostalCode;
        private string Lat;
        private string Lon;
        private string ISP;
        private bool Mobile;
        private bool Proxy;
        private bool Hosted;


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
                        this.Mobile = ResponseJson.mobile;
                        this.Proxy = ResponseJson.proxy;
                        this.Hosted = ResponseJson.hosting;
                    }
                }


            }
        }

        internal int Send(byte[] Message)
        {
            return ValidatorSocket.Send(Message);
        }
    }
}