using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestBTC3
{
    class BitcoinVolume : IComparable<BitcoinVolume>
    {
        public string AltcoinName { set; get; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public int ID { get; set; }


        public BitcoinVolume() { }
        public BitcoinVolume(string AltcoinName, double Volume, double Price, int ID)
        {
            this.AltcoinName = AltcoinName;
            this.Volume = Volume;
            this.Price = Price;
            this.ID = ID;
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
            //return String.Format("MarketName: {0}\tVolume: {1}", this.AltcoinName, this.Volume);
            return String.Format("{0}\t{1:0.00}\t{2:0.00000000}", this.AltcoinName, this.Volume, this.Price);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            File.WriteAllLines("output.txt", getWebResponse());
            //Console.ReadKey();
        }

        private static string[] getWebResponse()
        {
            string byc = getWebResponse("https://bittrex.com/api/v1.1/public/getmarketsummaries");

            int pos1 = byc.IndexOf(":[", 0);
            int pos2 = byc.IndexOf("[", pos1);
            int pos3 = byc.IndexOf("]", pos2);

            string[] str = byc.Substring(pos2 + 1, pos3 - pos2 - 1).Split('}');
            string[] str2 = new string[str.Length];

            //Выбираем только маркет BTC
            for (int i = 0, b = 0; i < str.Length; i++)
            {
                if (str[i].Contains("BTC-"))
                {
                    str2[b] = str[i].Replace(",{", "").Replace("{", "");
                    b++;
                }
            }

            //Удаляем пустые строки
            str2 = str2.Where(x => x != null).ToArray();

            string[] str3 = new string[str2.Length];
            string[] s = new string[13];
            List<BitcoinVolume> btc_vol = new List<BitcoinVolume>();

            //Убираем ненужные значения, оставляем название и объём в BTC, заполняем интерфейс IComparable
            for (int i = 0; i < str2.Length; i++)
            {
                s = str2[i].Split(',');
                btc_vol.Add(new BitcoinVolume(s[0].Split(':')[1].Replace("\"", ""), Convert.ToDouble(s[5].Split(':')[1].Replace(".", ",")), 
                    Convert.ToDouble(s[4].Split(':')[1].Replace(".", ",")), i));
            }

            //Выполняем сортировку по убыванию
            btc_vol.Sort();

            //Записываем в массив строк отсортированные значения
            int f = 0;
            foreach (BitcoinVolume a in btc_vol)
            {
                //str3[f] = a.AltcoinName + "\t" + a.Volume;
                str3[f] = a.ToString();
                f++;
            }

            return str3;
        }

        public static string getWebResponse(string url)
        {
            // create request..
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            // use GET method
            webRequest.Method = "GET";

            // POST!
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            // read response into StreamReader
            Stream responseStream = webResponse.GetResponseStream();
            StreamReader _responseStream = new StreamReader(responseStream);

            // get raw result
            return _responseStream.ReadToEnd();
        }
    }
}