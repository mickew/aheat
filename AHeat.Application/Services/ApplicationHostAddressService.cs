using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AHeat.Application.Interfaces;

namespace AHeat.Application.Services;
public class ApplicationHostAddressService : IApplicationHostAddressService
{
    private string? _ipAddress;

    public string IpAddress
    {
        get
        {
            if (string.IsNullOrEmpty(_ipAddress))
            {
                _ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList
                    .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)!
                    .ToString();
            }
            return _ipAddress;
        }
    }
}
