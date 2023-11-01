using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHeat.Application.Models.Shelly;
public record DeviceInfo(
    string Name,
    string Id,
    string Mac,
    int Slot,
    string Model,
    int Gen,
    string FwId,
    string Ver,
    string App,
    bool AuthEn,
    object AuthDomain
//        public string Name { get; set; }
//        public string Id { get; set; }
//        public string Mac { get; set; }
//        public int Slot { get; set; }
//        public string Model { get; set; }
//        public int Gen { get; set; }
//        public string FwId { get; set; }
//        public string Ver { get; set; }
//        public string App { get; set; }
//        public bool AuthEn { get; set; }
//        public object AuthDomain { get; set; }
);
