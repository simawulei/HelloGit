using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace UtilModel
{
    public class webModel
    {
        /// <summary>
        /// ��ȡҳ�洫�����
        /// </summary>
        /// <param name="thePage"></param>
        /// <param name="ayName"></param>
        /// <param name="ayValue"></param>
        /// <returns></returns>
        static public int GetPageArgs(System.Web.UI.Page thePage, out List<string> ayName, out List<string> ayValue)
        {
            int i, iNums = thePage.Request.QueryString.Count;
            ayName = new List<string>();
            ayValue = new List<string>();
            for (i = 0; i < iNums; i++)
            {
                ayName.Add(thePage.Request.QueryString.AllKeys[i].ToString());//�õ���������
                ayValue.Add(thePage.Request.QueryString[i]);
            }
            return ayName.Count;
        }
        /// <summary>
        /// ��ȡIP��ַ
        /// </summary>
        /// <returns></returns>
        static public string getClientIPAddress()
        {
            string result = String.Empty;

            result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            // ���ʹ�ô�����ȡ��ʵIP 
            if (result != null && result.IndexOf(".") == -1)    //û�С�.���϶��Ƿ�IPv4��ʽ 
                result = null;
            else if (result != null)
            {
                if (result.IndexOf(",") != -1)
                {
                    //�С�,�������ƶ������ȡ��һ������������IP�� 
                    result = result.Replace(" ", "").Replace("'", "");
                    string[] temparyip = result.Split(",;".ToCharArray());
                    for (int i = 0; i < temparyip.Length; i++)
                    {
                        if (IsIPAddress(temparyip[i])
                            && temparyip[i].Substring(0, 3) != "10."
                            && temparyip[i].Substring(0, 7) != "192.168"
                            && temparyip[i].Substring(0, 7) != "172.16.")
                        {
                            return temparyip[i];    //�ҵ����������ĵ�ַ 
                        }
                    }
                }
                else if (IsIPAddress(result)) //������IP��ʽ 
                    return result;
                else
                    result = null;    //�����е����� ��IP��ȡIP 
            }
            if (null == result || result == String.Empty)
                result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (result == null || result == String.Empty)
                result = System.Web.HttpContext.Current.Request.UserHostAddress;

            return result;
        }        
        /// <summary>
        /// �ж��Ƿ���IP��ַ��ʽ 0.0.0.0
        /// </summary>
        /// <param name="str1">���жϵ�IP��ַ</param>
        /// <returns>true or false</returns>
        static private bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15) return false;

            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(regformat, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }

        static public string GetDataRowScript(string sDataVarName, DataRow dr)
        {
            int i;
            DataColumn dc;
            string sScript = "", sField, sValue;

            for (i = 0; i < dr.Table.Columns.Count; i++)
            {
                dc = dr.Table.Columns[i];
                sField = dc.ColumnName;
                if (!Convert.IsDBNull(dr[sField]))
                {
                    sValue = dr[sField].ToString();
                    sValue = sValue.Replace("\n", " ");
                }
                else { sValue = ""; }
                if (sScript.Length == 0)
                {
                    sScript = string.Format("{0}:'{1}'", sField, sValue);
                }
                else { sScript = sScript + "," + string.Format("{0}:'{1}'", sField, sValue); }
            }
            sScript = sDataVarName + ".push({" + sScript + "});";
            return sScript;
        }
        static public void TransCodeToClient(System.Web.UI.Page thePage, string sCode, string sKey)
        {
            string scriptText = "";

            scriptText = "<script type=\"text/javascript\">";
            scriptText = scriptText + sCode;
            scriptText = scriptText + "</script>";
            thePage.ClientScript.RegisterClientScriptBlock(thePage.GetType(), sKey, scriptText, false);
        }
        static public bool httpSendRequest(string url, string postData, out string msg, out string sError)
        {
            bool bRet = false;
            msg = "";
            sError = "";

            try
            {
                System.Net.HttpWebRequest request = System.Net.HttpWebRequest.Create(url) as System.Net.HttpWebRequest;
                request.Method = "POST";
                // ��������Ĳ�����ʽ
                request.ContentType = "application/x-www-form-urlencoded";

                if (postData.Length > 0)
                {
                    byte[] byte1 = Encoding.UTF8.GetBytes(postData);
                    // ������������ĳ���.
                    request.ContentLength = byte1.Length;
                    // ȡ�÷������������
                    Stream newStream = request.GetRequestStream();
                    // ʹ�� POST ���������ʱ��ʵ�ʵĲ���ͨ������� Body ������������ʽ����
                    newStream.Write(byte1, 0, byte1.Length);
                    // ��ɺ󣬹ر�������.
                    newStream.Close();
                }
                // GetResponse ��������ķ������󣬵ȴ�����������
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream receiveStream = response.GetResponseStream();
                // �����Խ��ֽ�����װΪ�߼����ַ������Ա��ڶ�ȡ�ı�����
                // ��Ҫע�����
                System.IO.StreamReader readStream = new System.IO.StreamReader(receiveStream, Encoding.UTF8);
                msg = readStream.ReadToEnd();
                // ��ɺ�Ҫ�ر��ַ������ַ����ײ���ֽ��������Զ��ر�
                response.Close();
                readStream.Close();
                bRet = true;
            }
            catch (System.Exception ex)
            {
                sError = "��������ʧ��";
            }
            return bRet;
        }

    }
}
