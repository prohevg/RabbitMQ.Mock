using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AMQP_0_9_1.Transport.Factories
{
    public interface IAddressFactory
    {
        /// <summary>
        /// Parse and creates list of IP addresses
        /// </summary>
        /// <param name="host">Host address</param>
        /// <returns>List of Ip addresses</returns>
        List<IPAddress> Create(string host);
    }


    public class AddressFactory : IAddressFactory
    {
        /// <summary>
        /// Parse and creates list of IP addresses
        /// </summary>
        /// <param name="host">Host address</param>
        /// <returns>List of Ip addresses</returns>
        public List<IPAddress> Create(string host)
        {
            var addresses = new List<IPAddress>();

            IPAddress ipAddress;
            if (IPAddress.TryParse(host, out ipAddress))
            {
                addresses.Add(ipAddress);
            }
            else if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                    ||
                    host.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase))
            {
                if (Socket.OSSupportsIPv4)
                {
                    addresses.Add(IPAddress.Any);
                }

                if (Socket.OSSupportsIPv6)
                {
                    addresses.Add(IPAddress.IPv6Any);
                }
            }

            return addresses;
        }
    }
}
