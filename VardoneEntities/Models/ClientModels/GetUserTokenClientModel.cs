using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace VardoneEntities.Models.ClientModels
{
    public class GetUserTokenClientModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string MacAddress => GetMacAddress();
        public string IpAddress => GetIpAddress();

        private static string GetMacAddress() =>
            (NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => nic.GetPhysicalAddress().ToString())).FirstOrDefault();

        private static string GetIpAddress()
        {
            string address;
            var request = WebRequest.Create("http://checkip.dyndns.org/");
            using (var response = request.GetResponse())
            using (var stream = new StreamReader(response.GetResponseStream()!))
            {
                address = stream.ReadToEnd();
            }

            var first = address.IndexOf("Address: ", StringComparison.Ordinal) + 9;
            var last = address.LastIndexOf("</body>", StringComparison.Ordinal);
            address = address.Substring(first, last - first);

            return address;
        }
    }
}