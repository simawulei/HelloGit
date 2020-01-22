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
        /// 获取页面传入参数
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
                ayName.Add(thePage.Request.QueryString.AllKeys[i].ToString());//得到参数名称
                ayValue.Add(thePage.Request.QueryString[i]);
            }
            return ayName.Count;
        }
        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        static public string getClientIPAddress()
        {
            string result = String.Empty;

            result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            // 如果使用代理，获取真实IP 
            if (result != null && result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式 
                result = null;
            else if (result != null)
            {
                if (result.IndexOf(",") != -1)
                {
                    //有“,”，估计多个代理。取第一个不是内网的IP。 
                    result = result.Replace(" ", "").Replace("'", "");
                    string[] temparyip = result.Split(",;".ToCharArray());
                    for (int i = 0; i < temparyip.Length; i++)
                    {
                        if (IsIPAddress(temparyip[i])
                            && temparyip[i].Substring(0, 3) != "10."
                            && temparyip[i].Substring(0, 7) != "192.168"
                            && temparyip[i].Substring(0, 7) != "172.16.")
                        {
                            return temparyip[i];    //找到不是内网的地址 
                        }
                    }
                }
                else if (IsIPAddress(result)) //代理即是IP格式 
                    return result;
                else
                    result = null;    //代理中的内容 非IP，取IP 
            }
            if (null == result || result == String.Empty)
                result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (result == null || result == String.Empty)
                result = System.Web.HttpContext.Current.Request.UserHostAddress;

            return result;
        }        
        /// <summary>
        /// 判断是否是IP地址格式 0.0.0.0
        /// </summary>
        /// <param name="str1">待判断的IP地址</param>
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
                // 设置请求的参数形式
                request.ContentType = "application/x-www-form-urlencoded";

                if (postData.Length > 0)
                {
                    byte[] byte1 = Encoding.UTF8.GetBytes(postData);
                    // 设置请求参数的长度.
                    request.ContentLength = byte1.Length;
                    // 取得发向服务器的流
                    Stream newStream = request.GetRequestStream();
                    // 使用 POST 方法请求的时候，实际的参数通过请求的 Body 部分以流的形式传送
                    newStream.Write(byte1, 0, byte1.Length);
                    // 完成后，关闭请求流.
                    newStream.Close();
                }
                // GetResponse 方法才真的发送请求，等待服务器返回
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream receiveStream = response.GetResponseStream();
                // 还可以将字节流包装为高级的字符流，以便于读取文本内容
                // 需要注意编码
                System.IO.StreamReader readStream = new System.IO.StreamReader(receiveStream, Encoding.UTF8);
                msg = readStream.ReadToEnd();
                // 完成后要关闭字符流，字符流底层的字节流将会自动关闭
                response.Close();
                readStream.Close();
                bRet = true;
            }
            catch (System.Exception ex)
            {
                sError = "网络连接失败";
            }
            return bRet;
        }

    }
}
