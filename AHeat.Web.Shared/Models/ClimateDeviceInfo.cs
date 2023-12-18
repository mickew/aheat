using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHeat.Web.Shared;
[Serializable]
public record ClimateDeviceInfo(string DeviceId, string Name, double? Temperature, double? Humidity, DateTime Time);
