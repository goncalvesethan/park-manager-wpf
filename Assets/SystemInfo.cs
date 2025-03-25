using ParkManagerWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ParkManagerWPF.Assets
{
    public class DeviceInformations
    {
        public static Device GetDeviceInfo()
        {
            Device device = new Device();
            device.Brand = GetBrand();
            device.Processor = GetProcessor();
            device.RAM = GetRAM();
            device.Storage = GetStorage();
            device.MacAddress = GetMACAddress();
            device.IpAddress = GetIPAddress();

            return device;
        }

        private static string GetBrand()
        {
            string brand = string.Empty;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    brand = obj["Manufacturer"].ToString();
                }
            }
            return brand;
        }

        private static string GetProcessor()
        {
            string processor = string.Empty;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    processor = obj["Name"].ToString();
                }
            }
            return processor;
        }

        private static long GetRAM()
        {
            long ram = 0;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    ram = Convert.ToInt64(obj["Capacity"]) / (1024 * 1024); // Conversion en Mo
                }
            }
            return ram;
        }

        private static long GetStorage()
        {
            long storage = 0;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    storage = Convert.ToInt64(obj["Size"]) / (1024 * 1024 * 1024); // Conversion en Go
                }
            }
            return storage;
        }


        private static string GetMACAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    return ni.GetPhysicalAddress().ToString();
                }
            }
            return string.Empty;
        }

        private static string GetIPAddress()
        {
            string ip = string.Empty;
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var ipAddress in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ipAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ip = ipAddress.Address.ToString();
                            return ip;
                        }
                    }
                }
            }
            return ip;
        }
    }
}
