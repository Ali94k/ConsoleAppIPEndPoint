using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppIPEndPoint
{
    class Program
    {
        public delegate IPEndPoint delel(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount);

        static void Main(string[] args)
        {
            var req = HttpWebRequest.CreateHttp("http://localhost:53937/api/values/1");

            IPAddress IpV6Localhost = IPAddress.Parse("::1"); // IPAddress.IPv6Loopback; //
            IPAddress IpV4Localhost = IPAddress.Parse("127.0.0.1"); //IPAddress.Loopback; //

            req.ServicePoint.BindIPEndPointDelegate =
                                (servicePoint, remoteEndPoint, retryCount) =>
                                {
                                    Console.WriteLine($"inside BindIPEndPointDelegate\tretryCount\t{retryCount}");

                                    //return null;
                                    if (retryCount == 1)
                                    {
                                        Console.WriteLine("Exception");

                                        throw new OverflowException(
                                                "Reached maximum number of BindIPEndPointDelegate retries");
                                    }

                                    var iPEndPoint = new IPEndPoint(IpV4Localhost, 0);

                                    return iPEndPoint;
                                };

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            Stream receiveStream = response.GetResponseStream();

            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            StreamReader readStream = new StreamReader(receiveStream, encode);

            Console.WriteLine("\r\nResponse stream received.");

            Char[] read = new Char[256];
 
            int count = readStream.Read(read, 0, 256);

            Console.WriteLine("HTML...\r\n");

            while (count > 0)
            {
                String str = new String(read, 0, count);
                Console.Write(str);
                count = readStream.Read(read, 0, 256);
            }

            Console.WriteLine("");

            response.Close();

            readStream.Close();

            Console.ReadLine();
        }
    }
}
