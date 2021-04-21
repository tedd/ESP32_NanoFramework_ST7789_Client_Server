using System;
using System.Text;
using System.Threading;
using Windows.Devices.WiFi;

namespace Tedd.House1.Client.Esp32.WifiClient
{
    public class WifiConfig
    {
        public string SSID { get; set; }
        public string Key { get; set; }
        public bool EnableDhcp { get; set; } = true;
        public bool EnableDhcpDns { get; set; } = true;

        public bool UseIpv4 { get; set; }
        public string Ipv4Address { get; set; }
        public string Ipv4SubnetMask { get; set; }
        public string Ipv4GatewayAddress { get; set; }
        public bool UseIpv4Dns { get; set; }
        public string[] Ipv4DnsAddresses { get; set; }
        public bool UseIpv6 { get; set; }
        public string Ipv6Address { get; set; }
        public string Ipv6SubnetMask { get; set; }
        public string Ipv6GatewayAddress { get; set; }
        public bool UseIpv6Dns { get; set; }
        public string[] Ipv6DnsAddresses { get; set; }

        public WifiConfig() { }

        public WifiConfig(string ssid, string key)
        {
            SSID = ssid;
            Key = key;
        }

        public void SetWifiLogin(string ssid, string key)
        {
            SSID = ssid;
            Key = key;
        }

        public void SetDhcp()
        {
            EnableDhcp = EnableDhcpDns = true;
            UseIpv4 = UseIpv4Dns = false;
            UseIpv6 = UseIpv6Dns = false;
        }

        public void SetIpv4(string address, string mask, string gw, params string[] dns)
        {
            EnableDhcp = EnableDhcpDns = false;
            UseIpv4 = UseIpv4Dns = true;
            Ipv4Address = address;
            Ipv4SubnetMask = mask;
            Ipv4GatewayAddress = gw;
            Ipv4DnsAddresses = dns;
        }
        public void SetIpv6(string address, string mask, string gw, params string[] dns)
        {
            UseIpv6 = UseIpv6Dns = true;
            Ipv6Address = address;
            Ipv6SubnetMask = mask;
            Ipv6GatewayAddress = gw;
            Ipv6DnsAddresses = dns;
        }
    }
    public class WifiClient
    {
        private readonly WifiConfig _wifiConfig;
        private NetworkState _state;

        [Flags]
        public enum NetworkState : byte
        {
            None = 0,
            WifiScanning = 1,
            WifiScanned = 1 << 1,
            WifiConnecting = 1 << 2,
            WifiConnected = 1 << 3,
            NetworkConnecting = 1 << 4,
            Connected = WifiConnected | (1 << 5),
            WifiConnectFailed = 1 << 6,
            NoMatchingSSIDFound = 1 << 7
        };

        public NetworkState State
        {
            get => _state;
            private set
            {
                var old = _state;
                _state = value;
                StateChanged?.Invoke(this,old,value);
            }
        }

        public delegate void NetworkStateChangedDelegate(WifiClient sender, NetworkState oldState,
            NetworkState newState);

        public event NetworkStateChangedDelegate StateChanged;

        public WifiClient(WifiConfig wifiConfig)
        {
            _wifiConfig = wifiConfig;
        }

        public void Connect(bool neverGiveUp = true)
        {

            // Get the first WiFI Adapter
            var wifi = WiFiAdapter.FindAllAdapters()[0];

            // Set up the AvailableNetworksChanged event to pick up when scan has completed
            wifi.AvailableNetworksChanged += Wifi_AvailableNetworksChanged;

            // Loop forever until we are connected
            State = NetworkState.None;
            while (neverGiveUp && State != NetworkState.Connected)
            {
                Log("Starting WiFi scan...");
                State = NetworkState.WifiScanning;
                wifi.ScanAsync();

                while (State == NetworkState.WifiScanning)
                {
                    Thread.Sleep(10);
                }

                // Attempt to connect to wifi network
                ConnectToNetwork(wifi);

                var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                var nic = nics[wifi.NetworkInterface];
                while (State == NetworkState.NetworkConnecting)
                {
                    Thread.Sleep(10);
                    if (nic.IPv4Address != null && nic.IPv4Address != "" && nic.IPv4Address != "0.0.0.0")
                        State = NetworkState.Connected;
                }
                Log($"IPv4: {nic.IPv4Address}");
            }
        }

        /// <summary>
        /// Event handler for when WiFi scan completes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Wifi_AvailableNetworksChanged(WiFiAdapter sender, object e)
        {
            Log("Wifi_AvailableNetworksChanged - get report");
            State = NetworkState.WifiScanned;
        }

        private void ConnectToNetwork(WiFiAdapter sender)
        {

            // Enumerate though networks looking for the strongest candidate for our network
            WiFiAvailableNetwork strongest = null;
            var ssid = _wifiConfig.SSID.ToLower();
            Log("Found these networks:");
            foreach (var net in sender.NetworkReport.AvailableNetworks)
            {
                // Show all networks found
                Log(NetToString(net));


                // If its target network se store the one with strongest signal
                if (net.Ssid.ToLower() == ssid && (strongest == null || net.NetworkRssiInDecibelMilliwatts < strongest.NetworkRssiInDecibelMilliwatts))
                    strongest = net;
            }

            if (strongest == null)
            {
                State = NetworkState.NoMatchingSSIDFound;
                return;
            }

            State = NetworkState.WifiConnecting;

            Log("Chose strongest:");
            Log($"Net SSID :{strongest.Ssid},  BSSID : {strongest.Bsid},  rssi : {strongest.NetworkRssiInDecibelMilliwatts.ToString()},  signal : {strongest.SignalBars.ToString()}");

            // Disconnect in case we are already connected
            sender.Disconnect();

            // Connect to network
            var result = sender.Connect(strongest, WiFiReconnectionKind.Automatic, _wifiConfig.Key);


            // Display status
            if (result.ConnectionStatus == WiFiConnectionStatus.Success)
            {
                Log("Connected to Wifi network");
                var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                //for (var i = 0; i < nic.Length; i++)
                {
                    Log("Enabling DHCP, waiting for ip address...");
                    var nic = nics[sender.NetworkInterface];
                    if (_wifiConfig.EnableDhcp)
                    {
                        Log("Enabling DHCP");
                        nic.EnableDhcp();
                        if (_wifiConfig.EnableDhcpDns)
                        {
                            Log("Enabling DNS from DHCP");
                            nic.EnableAutomaticDns();
                        }
                        //Log("Renewing DHCP");
                        //nic.RenewDhcpLease();
                    }
                    else
                    {
                        if (_wifiConfig.UseIpv4)
                        {
                            Log($"Setting IPv4: ip: {_wifiConfig.Ipv4Address} mask: {_wifiConfig.Ipv4SubnetMask} gw:{_wifiConfig.Ipv4GatewayAddress}");
                            nic.EnableStaticIPv4(_wifiConfig.Ipv4Address, _wifiConfig.Ipv4SubnetMask,
                                _wifiConfig.Ipv4GatewayAddress);
                        }

                        if (_wifiConfig.UseIpv4Dns)
                        {
                            Log($"Setting IPv4 DNS:");
                            foreach (var dns in _wifiConfig.Ipv4DnsAddresses)
                                Log(dns);
                            nic.EnableStaticIPv4Dns(_wifiConfig.Ipv4DnsAddresses);
                        }

                        if (_wifiConfig.UseIpv6)
                        {
                            Log($"Setting IPv6: ip: {_wifiConfig.Ipv6Address} mask: {_wifiConfig.Ipv6SubnetMask} gw:{_wifiConfig.Ipv6GatewayAddress}");
                            nic.EnableStaticIPv6(_wifiConfig.Ipv6Address, _wifiConfig.Ipv6SubnetMask,
                                _wifiConfig.Ipv6GatewayAddress);
                        }

                        if (_wifiConfig.UseIpv6Dns)
                        {
                            Log($"Setting IPv6 DNS:");
                            foreach (var dns in _wifiConfig.Ipv6DnsAddresses)
                                Log(dns);
                            nic.EnableStaticIPv6Dns(_wifiConfig.Ipv6DnsAddresses);
                        }
                    }

                    State = NetworkState.NetworkConnecting;
                }
            }
            else
            {
                Log($"Error {result.ConnectionStatus.ToString()} connecting to Wifi network");
                State = NetworkState.WifiConnectFailed;
            }
        }

        private void Log(string s)
        {
            Console.WriteLine(s);
        }


        private static string NetToString(WiFiAvailableNetwork net) => $"Net SSID :{net.Ssid},  BSSID : {net.Bsid},  rssi : {net.NetworkRssiInDecibelMilliwatts.ToString()},  signal : {net.SignalBars.ToString()}";

    }
}
