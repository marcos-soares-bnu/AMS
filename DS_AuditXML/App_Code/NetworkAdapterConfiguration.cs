using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DS_AuditXML.App_Code
{
    public class NetworkAdapterConfiguration
    {
        public string Description { get; set; }
        public string Index  { get; set; }
        public string MACAddress  { get; set; }
        public string IPAddress  { get; set; }
        public string IPSubnet  { get; set; }
        public string DefaultIPGateway  { get; set; }
        public string DNSServerSearchOrder { get; set; }
    }
}