﻿using System;
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

        private static string GetMacAddress() =>
            (NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => nic.GetPhysicalAddress().ToString())).FirstOrDefault();
    }
}