using System;
using System.Collections.Generic;
using System.Text;

namespace UtilModel
{
     /// <summary>
    /// 管理一组参数
    /// </summary>
    public class CMToolsParameter
    {
        List<string> m_ayNames;
        List<string> m_ayValues;

        int m_MaxSelect=20;
        string m_SelectKey="Sel";

        public CMToolsParameter()
        {
            m_ayNames = new List<string>();
            m_ayValues = new List<string>();
            m_MaxSelect = 20;
            m_SelectKey = "Sel";
        }

        public int MaxSelect
        {
            get { return m_MaxSelect; }
            set { m_MaxSelect = value; }
        }
        public string SelectKey
        {
            get { return m_SelectKey; }
            set { m_SelectKey = value; }
        }
        public string GetParameter(string sName)
        {
            string sRet;
            GetParameter(sName, out sRet);
            return sRet;
        }

        public bool GetParameter(string sName,out string sValue)
        {
            int i;
            bool bExists = false;
            sValue = "";
            for (i = 0; i < m_ayNames.Count; i++)
            {
                if (string.Compare(sName, m_ayNames[i], true) == 0)
                {
                    sValue = m_ayValues[i];
                    bExists = true;
                    break;
                }
            }
            return bExists;
        }
        public bool GetParameter(string sName, out int iValue)
        {
            iValue = 0;
            string sValue;
            double dVal;
            bool bRet = GetParameter(sName, out sValue);

            if (bRet)
            {
                bRet=myUtil.isNumeric(sValue, out dVal);
                if (bRet) {iValue=(int)dVal;}
            }
            return bRet;
        }
        public bool GetParameter(string sName, out double dValue)
        {
            dValue = 0;
            string sValue;
            double dVal;
            bool bRet = GetParameter(sName, out sValue);

            if (bRet)
            {
                bRet = myUtil.isNumeric(sValue, out dVal);
                if (bRet) { dValue = dVal; }
            }
            return bRet;
        }

        public void SetParameter(string sName, string sValue)
        {
            int i;
            bool bExists = false;
            for (i = 0; i < m_ayNames.Count; i++)
            {
                if (string.Compare(sName, m_ayNames[i], true) == 0)
                {
                    m_ayValues[i] = sValue;
                    bExists = true;
                    break;
                }
            }
            if (!bExists)
            {
                m_ayNames.Add(sName);
                m_ayValues.Add(sValue);
            }
        }
        public void SetParameter(string sName, int iValue)
        {
            SetParameter(sName, iValue.ToString());
        }
        public void SetParameter(string sName, double dValue)
        {
            SetParameter(sName, dValue.ToString());
        }
        
        public void SaveParameter(string sIniFile, string sSect)
        {
            IniFile cFile = new IniFile(sIniFile);
            int i;
            string sName;
            string sValue;
            for (i = 0; i < m_ayNames.Count && i<m_ayValues.Count; i++)
            {
                sName = m_ayNames[i];
                sValue = m_ayValues[i];
                cFile.WriteString(sSect, sName, sValue);
            }
        }
        public void ReadParameter(string sIniFile, string sSect)
        {
            IniFile cFile = new IniFile(sIniFile);
            int i;
            string sName;
            string sValue;
            for (i = 0; i < m_ayNames.Count; i++)
            {
                sName = m_ayNames[i];
                sValue = cFile.ReadString(sSect, sName, "");
                SetParameter(sName, sValue);
            }
        }
        public List<string> ReadParameterSelect(string sIniFile, string sSect)
        {
            IniFile cFile = new IniFile(sIniFile);
            int i;
            string sName, sValue;
            List<string> ayResult = new List<string>();
            for (i = 0; i < m_MaxSelect; i++)
            {
                sName = string.Format("{0}{1}", m_SelectKey, i);
                sValue = cFile.ReadString(sSect, sName, "");
                if (sValue.Length > 0)
                {
                    ayResult.Add(sValue);
                }
                else { break; }
            }
            return ayResult;
        }
        public void SaveParameterSelect(string sIniFile, string sSect, List<string> aySelect)
        {
            IniFile cFile = new IniFile(sIniFile);
            int i;
            string sName, sValue;

            for (i = 0; i < m_MaxSelect; i++)
            {
                sName = string.Format("{0}{1}", m_SelectKey, i);
                if (i < aySelect.Count) { sValue = aySelect[i]; }
                else { sValue = ""; }
                cFile.WriteString(sSect, sName, sValue);
            }
        }
    }
}

