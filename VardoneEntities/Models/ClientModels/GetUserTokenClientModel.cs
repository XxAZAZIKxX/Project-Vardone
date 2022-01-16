using System.Linq;
using System.Net.NetworkInformation;

namespace VardoneEntities.Models.ClientModels
{
    public record GetUserTokenClientModel
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string MacAddress => GetMacAddress();

        private static string GetMacAddress() => NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();
    }
}