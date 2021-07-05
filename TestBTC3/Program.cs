using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TestBTC3
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            File.WriteAllLines("output.txt", OutputWebResponse().Result);
        }

        private static async Task<string[]> OutputWebResponse()
        {
            string byc = await getWebResponse("https://bittrex.com/api/v1.1/public/getmarketsummaries");

            int pos1 = byc.IndexOf("[{");
            int pos2 = byc.IndexOf("]", pos1);

            string[] str1 = byc.Substring(pos1 + 2, pos2 - pos1 - 3).Replace(",{", "").Replace("\"", "").Split('}');

            string[] s = new string[13];
            List<BitcoinVolume> btc_vol = new List<BitcoinVolume>();

            //Убираем ненужные значения, оставляем название и объём в BTC, заполняем интерфейс IComparable
            for (int i = 0; i < str1.Length; i++)
            {
                s = str1[i].Split(',');
                if (!s[5].Split(':')[1].Contains("null") && s[0].Split(':')[1].Contains("BTC-"))
                {
                    btc_vol.Add(new BitcoinVolume(s[0].Split(':')[1], Convert.ToDouble(s[4].Split(':')[1].Replace(".", ",")),
                        Convert.ToDouble(s[5].Split(':')[1].Replace(".", ","))));
                }
            }

            //Выполняем сортировку по убыванию
            btc_vol.Sort();

            string[] str2 = new string[btc_vol.Count];
            int f = 0;

            foreach (BitcoinVolume a in btc_vol)
            {
                str2[f] = a.ToString();
                f++;
            }

            return str2;
        }

        private static async Task<string> getWebResponse(string url)
        {
            // create request..
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            // use GET method
            webRequest.Method = "GET";

            // POST!
            HttpWebResponse webResponse = await webRequest.GetResponseAsync() as HttpWebResponse;

            // read response into StreamReader
            Stream responseStream = webResponse.GetResponseStream();
            StreamReader _responseStream = new StreamReader(responseStream);

            // get raw result
            return _responseStream.ReadToEnd();
        }
    }

    class BitcoinVolume : IComparable<BitcoinVolume>
    {
        public string AltcoinName { set; get; }
        public double Price { get; set; }
        public double Volume { get; set; }


        public BitcoinVolume() { }
        public BitcoinVolume(string AltcoinName, double Price, double Volume)
        {
            this.AltcoinName = AltcoinName;
            this.Price = Price;
            this.Volume = Volume;
        }

        // Реализуем интерфейс IComparable<T>
        public int CompareTo(BitcoinVolume obj)
        {
            if (this.Volume < obj.Volume)
                return 1;
            if (this.Volume > obj.Volume)
                return -1;
            else
                return 0;
        }

        public override string ToString()
        {
            //return String.Format("{0}\t{1:0.00000000}\t{2:0.00}", this.AltcoinName, this.Price, this.Volume);
            return $"{AltcoinName,-20}{Price,15:0.00000000}{Volume,20:0.00}";
        }
    }
}