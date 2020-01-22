using System;
using System.Collections.Generic;
using System.Text;
//using System.Net;
using System.Management;
//using System.Runtime.InteropServices;

namespace UtilModel
{
    public class HardwareModel
    {
        //[DllImport("Iphlpapi.dll")]
        //private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        //[DllImport("Ws2_32.dll")]
        //private static extern Int32 inet_addr(string ip);

        //static public string GetMacByIp(string IP)
        //{
        //    Int32 ldest = inet_addr(IP);
        //    Int64 macinfo = new Int64();
        //    Int32 len = 6;
        //    int res = SendARP(ldest, 0, ref macinfo, ref len);
        //    string mac_src = macinfo.ToString("X");

        //    while (mac_src.Length < 12)
        //    {
        //        mac_src = mac_src.Insert(0, "0");
        //    }

        //    string mac_dest = "";

        //    for (int i = 0; i < 11; i++)
        //    {
        //        if (0 == (i % 2))
        //        {
        //            if (i == 10)
        //            {
        //                mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
        //            }
        //            else
        //            {
        //                mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
        //            }
        //        }
        //    }

        //    return mac_dest;
        //}
        static public void ListIpMac(out List<string> ayIp,out List<string> ayMac)
        {
            ayIp = new List<string>();
            ayMac = new List<string>();
            string[] sip;
            string mac;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_NetworkAdapterConfiguration");
                foreach (ManagementObject mo in searcher.Get())
                {
                    // mo["IPAddress"]，IP地址  
                    // mo["DefaultIPGateway"]，默认网关 
                    // mo["DNSServerSearchOrder"]，DNS地址，如果有备用则会以数组形式返回 
                    // mo["IPSubnet"]，子网掩码 
                    // mo["MACAddress"]，MAC地址     
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        sip = (string[])mo["IPAddress"];
                        ayIp.Add(sip[0]);
                        ayMac.Add(mac);
                    }
                }
            }
            catch
            {

            }       
        }

        static public void GetBaseBoardInfo(out List<string> ayName,out List<string> ayValue)
        {
            ayName = new List<string>();
            ayValue = new List<string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From  Win32_BaseBoard");
                foreach (ManagementObject mo in searcher.Get())
                {                    
                }
            }
            catch
            {

            }       
        }

        static public void GetBiosInfo(out List<string> ayName, out List<string> ayValue)
        {
            ayName = new List<string>();
            ayValue = new List<string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_BIOS");
                foreach (ManagementObject mo in searcher.Get())
                {
                }
            }
            catch
            {

            }
        }

        static public void GetPhysicalDiskInfo(out List<string> ayName, out List<string> ayValue)
        {
            ayName = new List<string>();
            ayValue = new List<string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From  Win32_DiskDrive");
                foreach (ManagementObject mo in searcher.Get())
                {
                }
            }
            catch
            {

            }
        }
        static public void GettProcessoInfo(out List<string> ayName, out List<string> ayValue)
        {
            ayName = new List<string>();
            ayValue = new List<string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From   Win32_Processor");
                foreach (ManagementObject mo in searcher.Get())
                {
                }
            }
            catch
            {

            }
        }
        static public void GetttLogicalDiskInfo(out List<string> ayName, out List<string> ayValue)
        {
            ayName = new List<string>();
            ayValue = new List<string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From  Win32_LogicalDis");
                foreach (ManagementObject mo in searcher.Get())
                {
                }
            }
            catch
            {

            }
        }

        static public int CheckLic(string sLicCode,out int iMaxNums)
        {
            int nRet = 100;
            List<string> ayIp, ayMac;
            string sMacCode;
            int i, j, iCount, iVal, n;
            string sCode, sVal, sNums, sDeCode;
            char c;

            iMaxNums = 0;
            sLicCode = sLicCode.Replace(" ", "");

            ListIpMac(out ayIp, out ayMac);
            iCount = ayMac.Count;
            if (iCount == 0) { return 110; }

            sNums = sLicCode.Substring(sLicCode.Length - 4, 4);
            while (sNums[0] == '0' && sNums.Length > 0)
            {
                sNums = sNums.Substring(1);
            }
            if (myUtil.isInteger(sNums, out iVal)) { iMaxNums = iVal; }

            ClassEncrypt cEpy = new ClassEncrypt();
            cEpy.EncryptKey = "Ink2012CAD";
            string sCheckCode1 = sLicCode.Substring(0, sLicCode.Length - 4);
            string sCheckCode2;
            for (i = 0; i < iCount; i++)
            {
                sCode = ayMac[i];
                sCode = sCode.Replace(":", "");
                sCode = sCode.Replace("-", "");
                sMacCode = "";
                sCheckCode2 = "";
                for (j = 0; j < sCode.Length; j++)
                {
                    if (j == 0 || j == 2 || j == 6 || j == 8 || j == 10 || j == 11)
                    {
                        sVal = sCode.Substring(j, 1);
                        sMacCode += sVal;
                    }
                }
                sDeCode = cEpy.CodeText(sMacCode);
                for (j = 0; j < sDeCode.Length; j++)
                {
                    c = sDeCode[j];
                    n = (int)c;
                    sVal = n.ToString();
                    while (sVal.Length < 3) { sVal = "0" + sVal; }
                    sCheckCode2 += sVal;
                }
                if (string.Compare(sCheckCode1, sCheckCode2, true) == 0)
                {
                    nRet = 0;
                    break;
                }
            }

            return nRet;
        }

        //static public int CheckLic(string sServerIp, string sLicCode, out int iMaxNums)
        //{
        //    int nRet = 1;
        //    List<string> ayMac=new List<string>();
        //    string[] sMacCode;
        //    int i, j, iCount, iVal;
        //    string sCode, sVal, sNums, sDeCode;
        //    char c;

        //    iMaxNums = 0;
        //    sLicCode = sLicCode.Replace(" ", "");

        //    //HardwareModel.ListIpMac(out ayIp, out ayMac);
        //    string sMac = GetMacByIp(sServerIp);
        //    if (sMac.Length > 0) { ayMac.Add(sMac); }

        //    iCount = ayMac.Count;
        //    if (iCount == 0) { return 1; }
        //    sMacCode = new string[iCount];

        //    for (i = 0; i < iCount; i++)
        //    {
        //        sCode = ayMac[i].Replace(":", "");
        //        sCode = ayMac[i].Replace("-", "");
        //        for (j = 0; j < sCode.Length; j++)
        //        {
        //            if (j == 0 || j == 2 || j == 6 || j == 8 || j == 10 || j == 11)
        //            {
        //                sVal = sCode.Substring(j, 1);
        //                sMacCode[i] += sVal;
        //            }
        //        }
        //    }

        //    sNums = sLicCode.Substring(sLicCode.Length - 4, 4);
        //    while (sNums[0] == '0' && sNums.Length > 0)
        //    {
        //        sNums = sNums.Substring(1);
        //    }
        //    if (myUtil.isInteger(sNums, out iVal)) { iMaxNums = iVal; }

        //    sDeCode = "";
        //    sLicCode = sLicCode.Substring(0, sLicCode.Length - 4);
        //    for (i = 0; i <= sLicCode.Length - 3; i = i + 3)
        //    {
        //        sCode = sLicCode.Substring(i, 3);
        //        while (sCode[0] == '0' && sCode.Length > 0) { sCode = sCode.Substring(1); }
        //        if (myUtil.isInteger(sCode, out iVal))
        //        {
        //            c = (char)iVal;
        //            sDeCode += c.ToString();
        //        }
        //    }
        //    ClassEncrypt cEpy = new ClassEncrypt();
        //    cEpy.EncryptKey = "Ink2012CAD";
        //    sCode = cEpy.DeCodeText(sDeCode);

        //    for (i = 0; i < sMacCode.Length; i++)
        //    {
        //        if (sMacCode[i] == sCode) { nRet = 0; break; }
        //    }

        //    return nRet;
        //}
    }
}
