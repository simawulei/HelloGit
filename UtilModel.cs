using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Data;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System.Xml;
using LitJson;

namespace UtilModel
{
    public class myUtil
    {
        static public string m_strCnn = "";
        static public string m_strCnnFile = "";

        static public string ftpServerIP = "";
        static public string ftpLogin = "";
        static public string ftpPassword = "";
        static public string ftpRootPath = "";


        static public bool isLong(string sInput, out long iRet)
        {
            bool bRet = true;
            iRet = 0;
            try
            {
                iRet = Convert.ToInt32(sInput);
            }
            catch (System.Exception)
            {
                bRet = false;
            }
            return bRet;
        }
        static public bool isDouble(string sInput, out double dRet)
        {
            bool bRet = true;
            dRet = 0;
            try
            {
                dRet = Convert.ToDouble(sInput);
            }
            catch (System.Exception)
            {
                bRet = false;
            }
            return bRet;
        }
        static public bool isInteger(string sInput, out int iRet)
        {
            bool bRet = true;
            iRet = 0;
            try
            {
                iRet = Convert.ToInt32(sInput);
            }
            catch (System.Exception)
            {
                bRet = false;
            }
            return bRet;
        }
        static public double GetValue(string sInput)
        {
            double dVal;
            isNumeric(sInput, out dVal);
            return dVal;
        }
        /// <summary>
        /// �ַ����Ƿ��ڱ���
        /// </summary>
        /// <param name="lst">��</param>
        /// <param name="sText">�ַ���</param>
        /// <param name="ignoreCase">�Ƿ���Դ�Сд</param>
        /// <returns>true/false</returns>
        static public bool isInList(List<string> lst, string sText, bool ignoreCase)
        {
            int i;
            bool bIn = false;
            if (lst != null)
            {
                for (i = 0; i < lst.Count; i++)
                {
                    if (string.Compare(sText, lst[i], ignoreCase) == 0)
                    {
                        bIn = true;
                        break;
                    }
                }
            }
            return bIn;
        }

        /// <summary>
        /// ����ض���ʽ�ַ���
        /// sInput="T@1"
        /// getId(sInput,out sType)=1; sType����"T"
        /// </summary>
        /// <param name="sInput"></param>
        /// <param name="sType"></param>
        /// <returns></returns>
        static public long getId(string sInput, out string sType)
        {
            long iId = 0;
            sType = "";
            int iIndex = sInput.IndexOf('@');
            if (iIndex > 0)
            {
                string s = sInput.Substring(iIndex + 1);
                sType = sInput.Substring(0, iIndex);
                try
                {
                    iId = Convert.ToInt32(s);
                }
                catch (System.Exception)
                {
                    iId = 0;
                    sType = "";
                }
            }
            return iId;
        }
        static public DialogResult ExclamationMessage(string sText, string sTitle)
        {
            return MessageBox.Show(sText, sTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        static public DialogResult QuestionMessage(string sText, string sTitle)
        {
            return MessageBox.Show(sText, sTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        static public DialogResult InformationMessage(string sText, string sTitle)
        {
            return MessageBox.Show(sText, sTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        static public double atof(string sInput)
        {
            double dVal = 0;
            isNumeric(sInput, out dVal);
            return dVal;
        }
        static public int atoi(string sInput)
        {
            double dVal = 0;
            isNumeric(sInput, out dVal);
            return (int)dVal;
        }
        static public bool isNumeric(string sInput, out double dResult)
        {
            dResult = 0;
            bool bRet = false;
            try
            {
                dResult = Convert.ToDouble(sInput);
                bRet = true;
            }
            catch (System.Exception) { }
            return bRet;
        }
        static public bool GetNumberInString(string sInput, out double dResult)
        {
            bool bRet;
            double dVal;

            dResult = 0;
            bRet = isNumeric(sInput, out dVal);
            if (bRet)
            {
                dResult = dVal;
                return bRet;
            }
            else
            {
                string sNum = "0123456789.+-",sVal="";
                char s;
                int i, iflag = 0;
                for (i = 0; i < sInput.Length; i++)
                {
                    s = sInput[i];
                    if (sNum.IndexOf(s) >= 0)
                    {
                        if (s == '.')
                        {
                            if (sVal.Length > 0) { sVal += s; }
                            else
                            {
                                if (iflag == 1) { break; }
                            }
                        }
                        else
                        {
                            sVal += s;
                            iflag = 1;
                        }
                    }
                    else
                    {
                        if (iflag == 1) { break; }
                    }
                }
                if (sVal.Length > 0) { bRet = isNumeric(sVal, out dResult); }
            }
            return bRet;
        }
        static public string DeleteLastZero(string sInput)
        {
            string sRet = sInput;
            if (sRet.IndexOf('.') > 0)
            {
                int iLen = sRet.Length;
                char s = sRet[iLen - 1];
                while (s == '0' || s=='.')
                {
                    sRet = sRet.Substring(0, iLen - 1);
                    if (s == '.') { break; }
                    iLen = sRet.Length;
                    s = sRet[iLen - 1];
                }
            }
            return sRet;
        }
        static public bool isDateTime(string sInput,out DateTime deResult)
        {
            deResult = DateTime.MinValue;
            bool bRet = false;
            try
            {
                deResult = Convert.ToDateTime(sInput);
                bRet = true;
            }
            catch (System.Exception) {}
            return bRet;
        }
        static public string m_InvalidChar = "*@|\\/\"#%^~`";
        /// <summary>
        /// �ж��ַ������Ƿ�����ַ�"*@|\\/\"#%^~`"
        /// </summary>
        /// <param name="sInput"></param>
        /// <returns></returns>
        static public bool CheckChar(string sInput)
        {
            int i;
            bool bRet = true;
            for (i = 0; i < m_InvalidChar.Length; i++)
            {
                if (sInput.IndexOf(m_InvalidChar[i]) >= 0)
                {
                    bRet = false;
                }
            }
            return bRet;
        }

        /// <summary>
        /// ����һ��JavaScript����
        /// </summary>
        /// <param name="sCode">�������</param>
        /// <param name="sResult">��������</param>
        /// <returns>0ִ����ȷ/-1ִ�д���</returns>
        static public int JScriptEval(string sCode, out string sResult)
        {
            int nRet = 0;
            sResult = "";

            try
            {
                Microsoft.JScript.Vsa.VsaEngine vEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
                sResult = Microsoft.JScript.Eval.JScriptEvaluate(sCode, vEngine).ToString();
            }
            catch (System.Exception ex)
            {
                nRet = -1;
                sResult = ex.Message;
            }
            return nRet;
        }
        /// <summary>
        /// �ָ��ַ���"[{...{..}},{....}...]"Ϊ����"{...{..}}","{....}"...
        /// ����Ƕ��
        /// </summary>
        /// <param name="sInput"></param>
        /// <returns></returns>
        static public List<string> SpliteListItemData(string sInput)
        {
            List<string> ayItem = new List<string>();
            string sItem;
            int i, iFlag = 0;

            if (sInput.Length == 0) { return ayItem; }
            if (sInput[0] == '[')
            {
                sInput = sInput.Substring(1, sInput.Length - 2);
            }
            i = 0;
            try
            {
                while (i < sInput.Length)
                {
                    if (sInput[i] == '{') { iFlag++; i++; }
                    else if (sInput[i] == '}')
                    {
                        iFlag--;
                        if (iFlag == 0)
                        {
                            sItem = sInput.Substring(0, i + 1);
                            ayItem.Add(sItem);
                            if (i < sInput.Length - 1)
                            {
                                sInput = sInput.Substring(i + 1);
                            }
                            else { break; }
                            if (sInput.Length > 0 && sInput[0] == ',')
                            {
                                sInput = sInput.Substring(1, sInput.Length - 1);
                            }
                            i = 0;
                        }
                        else { i++; }
                    }
                    else { i++; }
                }
                if (sInput.Length > 0)
                {
                    ayItem.Add(sInput);
                }
            }
            catch (System.Exception ex)
            {
                sItem = ex.Message;
            }
            return ayItem;
        }

        /// <summary>
        /// ��ȡ�ַ���������{}�ڵ�����
        /// </summary>
        /// <param name="sInput"></param>
        /// <returns></returns>
        static public List<string> SpliteListData(string sInput)
        {
            List<string> ayList = new List<string>();
            int[] iPos = new int[2];
            string sData, sList;

            sList = sInput;
            iPos[0] = sList.IndexOf('{');
            iPos[1] = sList.IndexOf('}');
            while (iPos[0] >= 0 && iPos[1] > iPos[0])
            {
                sData = sList.Substring(iPos[0] + 1, iPos[1] - iPos[0] - 1);
                ayList.Add(sData);
                if (sList.Length > iPos[1])
                {
                    sList = sList.Substring(iPos[1] + 1);
                    iPos[0] = sList.IndexOf('{');
                    iPos[1] = sList.IndexOf('}');
                }
                else { break; }
            }
            return ayList;
        }
        /// <summary>
        /// ���ء�������Сʱ����_4λ�������
        /// </summary>
        /// <returns></returns>
        static public string getRandIdByTime()
        {
            string sId, sVal;
            int iVal;
            DateTime cur = DateTime.Now;
            sId = cur.Year.ToString();
            iVal = cur.Month;
            if (iVal < 10) { sId += "0" + iVal.ToString(); }
            else { sId += iVal.ToString(); }
            iVal = cur.Day;
            if (iVal < 10) { sId += "0" + iVal.ToString(); }
            else { sId += iVal.ToString(); }
            iVal = cur.Hour;
            if (iVal < 10) { sId += "0" + iVal.ToString(); }
            else { sId += iVal.ToString(); }
            iVal = cur.Minute;
            if (iVal < 10) { sId += "0" + iVal.ToString(); }
            else { sId += iVal.ToString(); }
            iVal = cur.Second;
            if (iVal < 10) { sId += "0" + iVal.ToString(); }
            else { sId += iVal.ToString(); }
            iVal = cur.Millisecond;
            sVal = iVal.ToString();
            while (sVal.Length < 3)
            {
                sVal = "0" + sVal;
            }
            sVal = "_" + sVal;
            sId += sVal;
            Random autoRand = new Random(iVal);
            sVal = autoRand.Next(10000).ToString();
            while (sVal.Length < 4)
            {
                sVal = "0" + sVal;
            }
            sVal = "_" + sVal;
            sId += sVal;
            return sId;
        }

        /// <summary>
        /// ���ո�ֽ��ַ���
        /// </summary>
        /// <param name="sInput"></param>
        /// <returns></returns>
        static public List<string> SpliteRowData(string sInput)
        {
            List<string> ayRet = new List<string>();
            int i;
            string sText = "", str;

            for (i = 0; i < sInput.Length; i++)
            {
                str = sInput.Substring(i, 1);
                if (str == " ")
                {
                    sText = sText.Trim();
                    if (sText.Length > 0)
                    {
                        ayRet.Add(sText);
                    }
                    sText = "";
                }
                else
                {
                    sText = sText + str;
                }
            }
            sText = sText.Trim();
            if (sText.Length > 0)
            {
                ayRet.Add(sText);
            }
            return ayRet;
        }

        static public List<string> SpliteString(string sInput, string sSplitWord)
        {
            List<string> ayRet = new List<string>();
            if (sInput.IndexOf(sSplitWord) < 0)
            {
                ayRet.Add(sInput);
            }
            else
            {
                int i, iLen = sSplitWord.Length;
                string sText;
                i = sInput.IndexOf(sSplitWord);
                while (i >= 0)
                {
                    if (i > 0)
                    {
                        sText = sInput.Substring(0, i);
                        ayRet.Add(sText);
                    }
                    sInput = sInput.Substring(i + iLen);
                    i = sInput.IndexOf(sSplitWord);
                }
                if (sInput.Length > 0) { ayRet.Add(sInput); }
            }
            return ayRet;
        }
        
        static public void getAllChildContrl(System.Windows.Forms.Control ctrlRoot, ref List<System.Windows.Forms.Control> ayChild)
        {
            string sTag;
            foreach (System.Windows.Forms.Control ctrl in ctrlRoot.Controls)
            {
                if (ctrl.Tag != null)
                {
                    sTag = ctrl.Tag.ToString().Trim();
                    if (sTag.Length > 0)
                    {
                        ayChild.Add(ctrl);
                    }
                }
                if (ctrl.Controls.Count > 0)
                {
                    getAllChildContrl(ctrl, ref ayChild);
                }
            }
        }
        static public List<System.Windows.Forms.Control> getAllFormControl(System.Windows.Forms.Form theForm)
        {
            string sTag;
            int i;
            System.Windows.Forms.Control ctrlChild;
            List<System.Windows.Forms.Control> ayRet = new List<Control>();
            foreach (System.Windows.Forms.Control ctrl in theForm.Controls)
            {
                if (ctrl.Tag != null)
                {
                    sTag = ctrl.Tag.ToString().Trim();
                    if (sTag.Length > 0)
                    {
                        ayRet.Add(ctrl);
                    }
                }
                if (ctrl.Controls.Count > 0)
                {
                    List<System.Windows.Forms.Control> ayChild = new List<Control>();
                    getAllChildContrl(ctrl, ref ayChild);
                    for (i = 0; i < ayChild.Count; i++)
                    {
                        ctrlChild = ayChild[i];
                        ayRet.Add(ctrlChild);
                    }
                }
            }
            return ayRet;
        }
        static public void ExchangeItem(ListView lstTable,int iIndex1,int iIndex2)
        {
            ListViewItem itm1 = lstTable.Items[iIndex1];
            ListViewItem itm2 = lstTable.Items[iIndex2];
            int i;
            string[] sText = new string[2];
            string[] sTag = new string[2];
            System.Drawing.Color iColor1=itm1.ForeColor;
            System.Drawing.Color iColor2=itm2.ForeColor;
            System.Drawing.Color iBColor1=itm1.BackColor;
            System.Drawing.Color iBColor2=itm2.BackColor;

            sText[0] = itm1.Text;
            sText[1] = itm2.Text;
            sTag[0] = itm1.Tag.ToString();
            sTag[1] = itm2.Tag.ToString();
            itm1.Text = sText[1];
            itm2.Text = sText[0];
            itm1.Tag = sTag[1];
            itm2.Tag = sTag[0];
            itm1.ForeColor=iColor2;
            itm2.ForeColor=iColor1;
            itm1.BackColor=iBColor2;
            itm2.BackColor=iBColor1;

            for (i = 1; i < lstTable.Columns.Count; i++)
            {
                sText[0] = itm1.SubItems[i].Text;
                sText[1] = itm2.SubItems[i].Text;
                itm1.SubItems[i].Text = sText[1];
                itm2.SubItems[i].Text = sText[0];
            }
            itm2.Selected = true;
            itm2.Focused = true;
            itm1.Selected = false;
            itm1.Focused = false;
            itm2.EnsureVisible();
        }
        static public bool fequ(double dVal1,double dVal2)
        {
            bool bRet = false;
            if (Math.Abs(dVal1 - dVal2) < 0.000000001) { bRet = true; }
            return bRet;
        }

        static public bool IsIPConnect(string sIp)
        {
            bool bRet = true;
            Ping cp = new Ping();
            PingReply pr;
            pr = cp.Send(sIp);
            if (pr.Status != IPStatus.Success)
            {
                bRet = false;
            }
            return bRet;
        }
        static public void ExchangeObject(ref object obj1, ref object obj2)
        {
            object obj = obj1;
            obj1 = obj2;
            obj2 = obj;
        }

        static private bool IsDataRowInOrder(DataRow dr1,DataRow dr2,List<string> aySortCol,List<int> aySortType)
        {
            bool bRet = true;
            int i,iSortType,iCompare;
            string sCol;
            string[] sCell = new string[2];

            for (i=0;i<aySortCol.Count && i<aySortType.Count;i++)
            {
                sCol = aySortCol[i];
                iSortType = aySortType[i];
                sCell[0] = dr1[sCol].ToString();
                sCell[1] = dr2[sCol].ToString();
                iCompare=string.Compare(sCell[0],sCell[1],true);
                if (iSortType==1) {
                    //����
                    if (iCompare<0) {
                        bRet = true;
                        break;
                    }
                    else if (iCompare>0) {
                        bRet = false;
                        break;
                    }
                }
                else if (iSortType==2) {
                    //����
                    if (iCompare < 0)
                    {
                        bRet = false;
                        break;
                    }
                    else if (iCompare > 0)
                    {
                        bRet = true;
                        break;
                    }
                }
                else { break; }
            }
            return bRet;
        }
        static public void ExchangeDaraRow(ref DataRow dr1,ref DataRow dr2)
        {
            int i;
            string[] sVal = new string[2];
            for (i=0;i<dr1.Table.Columns.Count && i<dr2.Table.Columns.Count;i++) {
                sVal[0] = dr1[i].ToString();
                sVal[1] = dr2[i].ToString();
                dr1[i] = sVal[1];
                dr2[i] = sVal[0];
            }
        }
        
        /// <summary>
        /// �����
        /// </summary>
        /// <param name="tblData">��</param>
        /// <param name="aySortCol">������</param>
        /// <param name="aySortType">����ʽ��1����/2����</param>
        static public void SortDataTable(ref DataTable tblData,List<string> aySortCol,List<int> aySortType)
        {
            bool bLoop=true;
            int n;
            DataRow[] dr=new DataRow[2];
            
            while (bLoop) {
                bLoop = false;
                for (n=0;n<tblData.Rows.Count-1;n++) {
                    dr[0] = tblData.Rows[n];
                    dr[1] = tblData.Rows[n + 1];
                    bLoop = !IsDataRowInOrder(dr[0], dr[1], aySortCol, aySortType);
                    if (bLoop) {
                        ExchangeDaraRow(ref dr[0], ref dr[1]);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// ΪligerComboBox����data���ʽ
        /// </summary>
        /// <param name="varData"></param>
        /// <param name="ayText"></param>
        /// <param name="ayId"></param>
        /// <returns></returns>
        static public string ToJScriptData(string varData,List<string> ayText, List<string> ayId)
        {
            string sScript = "";
            if (ayText != null)
            {
                int i;
                string sText, sId,sItem;

                sScript = "var " + varData + "=[";
                for (i = 0; i < ayText.Count; i++)
                {
                    sText = sId = ayText[i];
                    if (ayId != null)
                    {
                        if (i < ayId.Count) { sId = ayId[i]; }
                    }
                    sItem = "{text:'" + sText + "',id:'" + sId + "'}";
                    if (i == 0)
                    {
                        sScript += sItem;
                    }
                    else
                    {
                        sScript += "," + sItem;
                    }
                }
                sScript += "];";
            }
            return sScript;
        }

        ///<summary>
        ///�õ�����ַ�.
        ///</summary>
        ///<param name="intLength">Length of the int.</param>
        ///<param name="booNumber">if set to <c>true</c> [boo number].</param>
        ///<param name="booSign">if set to <c>true</c> [boo sign].</param>
        ///<param name="booSmallword">if set to <c>true</c> [boo smallword].</param>
        ///<param name="booBigword">if set to <c>true</c> [boo bigword].</param>
        ///<returns></returns>
        static public string getRandomizer(int intLength, bool booNumber, bool booSign, bool booSmallword, bool booBigword)
        {
            //����
            Random ranA = new Random();
            int intResultRound = 0;
            int intA = 0;
            string strB = "";
            while (intResultRound < intLength)
            {
                //���������A����ʾ��������
                //1=���֣�2=���ţ�3=Сд��ĸ��4=��д��ĸ
                intA = ranA.Next(1, 5);
                //��������A=1����������������
                //���������A����Χ��0-10
                //�������A��ת���ַ�
                //�����꣬λ��+1���ַ����ۼӣ���������ѭ��
                if (intA == 1 && booNumber)
                {
                    intA = ranA.Next(0, 10);
                    strB = intA.ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }
                //��������A=2�����������ɷ���
                //���������A����ʾ����ֵ��
                //1��33-47ֵ��2��58-64ֵ��3��91-96ֵ��4��123-126ֵ��
                if (intA == 2 && booSign == true)
                {
                    intA = ranA.Next(1, 5);
                    //���A=1
                    //���������A��33-47��Ascii��
                    //�������A��ת���ַ�
                    //�����꣬λ��+1���ַ����ۼӣ���������ѭ��
                    if (intA == 1)
                    {
                        intA = ranA.Next(33, 48);
                        strB = ((char)intA).ToString() + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                    //���A=2
                    //���������A��58-64��Ascii��
                    //�������A��ת���ַ�
                    //�����꣬λ��+1���ַ����ۼӣ���������ѭ��
                    if (intA == 2)
                    {
                        intA = ranA.Next(58, 65);
                        strB = ((char)intA).ToString() + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                    //���A=3
                    //���������A��91-96��Ascii��
                    //�������A��ת���ַ�
                    //�����꣬λ��+1���ַ����ۼӣ���������ѭ��
                    if (intA == 3)
                    {
                        intA = ranA.Next(91, 97);
                        strB = ((char)intA).ToString() + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }

                    //���A=4
                    //���������A��123-126��Ascii��
                    //�������A��ת���ַ�
                    //�����꣬λ��+1���ַ����ۼӣ���������ѭ��
                    if (intA == 4)
                    {
                        intA = ranA.Next(123, 127);
                        strB = ((char)intA).ToString() + strB;
                        intResultRound = intResultRound + 1;
                        continue;
                    }
                }

                //��������A=3������������Сд��ĸ
                //���������A����Χ��97-122
                //�������A��ת���ַ�
                //�����꣬λ��+1���ַ����ۼӣ���������ѭ��
                if (intA == 3 && booSmallword == true)
                {
                    intA = ranA.Next(97, 123);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }

                //��������A=4�����������ɴ�д��ĸ
                //���������A����Χ��65-90
                //�������A��ת���ַ�
                //�����꣬λ��+1���ַ����ۼӣ���������ѭ��
                if (intA == 4 && booBigword == true)
                {
                    intA = ranA.Next(65, 89);
                    strB = ((char)intA).ToString() + strB;
                    intResultRound = intResultRound + 1;
                    continue;
                }
            }
            return strB;
        }

        //����ַ�������������Ҫ�������£� 
        //1��֧���Զ����ַ�������
        //2��֧���Զ����Ƿ��������
        //3��֧���Զ����Ƿ����Сд��ĸ
        //4��֧���Զ����Ƿ������д��ĸ
        //5��֧���Զ����Ƿ�����������
        //6��֧���Զ����ַ���

        ///<summary>
        ///��������ַ���
        ///</summary>
        ///<param name="length">Ŀ���ַ����ĳ���</param>
        ///<param name="useNum">�Ƿ�������֣�1=������Ĭ��Ϊ����</param>
        ///<param name="useLow">�Ƿ����Сд��ĸ��1=������Ĭ��Ϊ����</param>
        ///<param name="useUpp">�Ƿ������д��ĸ��1=������Ĭ��Ϊ����</param>
        ///<param name="useSpe">�Ƿ���������ַ���1=������Ĭ��Ϊ������</param>
        ///<param name="custom">Ҫ�������Զ����ַ���ֱ������Ҫ�������ַ��б�</param>
        ///<returns>ָ�����ȵ�����ַ���</returns>
        static public string GetRnd(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        static public int strcmpi(string str1, string str2)
        {
            return string.Compare(str1, str2, true);
        }

        static public void SlideToGif(string sFileSld, string sFileGif, int iWidth, int iHeight)
        {
            System.IO.MemoryStream ms;
            System.Drawing.Bitmap bitmap;

            if (File.Exists(sFileSld))
            {
                BitmapSlide bmpSlide = new BitmapSlide();
                bmpSlide.m_Background = System.Drawing.Color.Black;
                bmpSlide.ReadSlide(sFileSld);
                bmpSlide.reDrawSlide(iWidth, iHeight, out bitmap);

                ms = new System.IO.MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                File.WriteAllBytes(sFileGif, ms.ToArray());
            }
        }

        static public DialogResult InputMessageBox(string sCaption, ref string sResult)
        {
            dlgInput dlg = new dlgInput();
            dlg.m_Caption = sCaption;
            dlg.m_Text = sResult;
            DialogResult nRet = dlg.ShowDialog();
            if (nRet == DialogResult.OK)
            {
                sResult = dlg.m_Text;
            }
            return nRet;
        }
        static public string GetJsonData(JsonData data, string sKey, string sDefaultValue)
        {
            string sResult=sDefaultValue;

            try
            {
                sResult = data[sKey].ToString();
            }
            catch (System.Exception ex)
            {
            	
            }
            return sResult;
        }
        static public void DrawImgBorder(System.Windows.Forms.PictureBox img, System.Drawing.Color cBorderColor, int iBorderWidth)
        {
            Graphics g;
            Pen myPen = new Pen(cBorderColor, iBorderWidth);
            Point pt;
            Size sz;
            Single x1, y1;

            pt = img.Location;
            sz = img.Size;

            x1 = iBorderWidth / 2;
            y1 = iBorderWidth / 2;
            g = img.CreateGraphics();
            g.DrawRectangle(myPen, x1, y1, sz.Width - iBorderWidth, sz.Height - iBorderWidth);
        }
        static public void GetCombSelectList(System.Windows.Forms.ComboBox cmb, ref List<string> ayList)
        {
            string sText;
            int i;

            if (cmb.SelectedIndex < 0)
            {
                sText = cmb.Text;
                if (ayList.IndexOf(sText) < 0) { ayList.Add(sText); }
            }
            for (i = 0; i < cmb.Items.Count; i++)
            {
                sText = cmb.Items[i].ToString();
                if (ayList.IndexOf(sText) < 0) { ayList.Add(sText); }
            }
        }
        static public string doubletoDu(double dVal)
        {
            string sResult, sTmp;
            double du, dd, dm, ds;
            du = dVal * 180 / Math.PI;

            dm = 60 * (60 * (du - Math.Floor(du)) - Math.Floor(60 * (du - Math.Floor(du))));	//��
            ds = Math.Floor(60 * (du - Math.Floor(du)));						//��
            dd = Math.Floor(du);										//��
            if (fequ(dm, 60))
            {
                dm = 0;
                ds = ds + 1;
                if (fequ(ds, 60))
                {
                    ds = 0;
                    dd = dd + 1;
                }
            }
            sTmp=string.Format("{0:0}��", dd);
            sResult = sTmp;
            if ((!fequ(ds, 0)) && (!fequ(dm, 0)))
            {
                sTmp=string.Format("{0:#}��{1:#}��", ds, dm);
            }
            else if ((fequ(ds, 0)) && (!fequ(dm, 0)))
            {
                sTmp=string.Format("{0:#}��{1:#}��", ds, dm);
            }
            else if ((!fequ(ds, 0)) && (fequ(dm, 0)))
            {
                sTmp=string.Format("{0:#}��", ds);
            }
            else { sTmp = ""; }
            sResult = sResult + sTmp;
            return sResult;
        }
        static public int GetTextDataFromClipboard(out string sData, out string sError)
        {
            int nRet = 1;
            IDataObject data = new DataObject();
            sError = "";
            sData = "";
            try
            {
                data = System.Windows.Forms.Clipboard.GetDataObject();
                if (data.GetDataPresent(DataFormats.Text))
                {
                    sData = (string)data.GetData(DataFormats.Text);
                    nRet = 0;
                }
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
            }
            return nRet;
        }
        static public int SetTextDataToClipboard(string sData, out string sError)
        {
            int nRet = 1;
            sError = "";
            try
            {
                Type myType = sData.GetType();
                DataObject data = new DataObject();
                data.SetData(myType, sData);
                System.Windows.Forms.Clipboard.SetDataObject(data);
                nRet = 0;
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
            }
            return nRet;
        }
        #region [��ɫ��16����ת��RGB]
        /// <summary>
        /// [��ɫ��16����ת��RGB]
        /// </summary>
        /// <param name="strColor">����16������ɫ [����RGB]</param>
        /// <returns></returns>
        public static System.Drawing.Color colorHx16toRGB(string strHxColor)
        {
            try
            {
                if (strHxColor.Length == 0)
                {//���Ϊ��
                    return System.Drawing.Color.FromArgb(0, 0, 0);//��Ϊ��ɫ
                }
                else
                {//ת����ɫ
                    return System.Drawing.Color.FromArgb(System.Int32.Parse(strHxColor.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier), System.Int32.Parse(strHxColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier), System.Int32.Parse(strHxColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                }
            }
            catch
            {//��Ϊ��ɫ
                return System.Drawing.Color.FromArgb(0, 0, 0);
            }
        }
        #endregion
        #region [��ɫ��RGBת��16����]
        /// <summary>
        /// [��ɫ��RGBת��16����]
        /// </summary>
        /// <param name="R">�� int</param>
        /// <param name="G">�� int</param>
        /// <param name="B">�� int</param>
        /// <returns></returns>
        public static string colorRGBtoHx16(int R, int G, int B)
        {
            return System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(R, G, B));
        }
        #endregion
    }
    //public class EvalModel
    //{
    //    static Microsoft.JScript.Vsa.VsaEngine vEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
    //    public int Eval(string sCode,out string sResult)
    //    {
    //        int nRet = 0;
    //        sResult = "";
    //        try
    //        {
    //            sResult = Microsoft.JScript.Eval.JScriptEvaluate(sCode, vEngine).ToString();
    //        }
    //        catch (System.Exception ex)
    //        {
    //            nRet = -1;
    //            sResult = ex.Message;
    //        }
    //        return nRet;
    //    }
    //}
    public class ClassEncrypt
    {
        private DESCryptoServiceProvider des;
        private string Key = "cu4425jv7416";
        
        public ClassEncrypt()
        {
            des = new DESCryptoServiceProvider();
        }
        public string EncryptKey
        {
            get { return Key; }
            set { Key = value; }
        }
        /// <summary>
        /// �ַ�������
        /// </summary>
        /// <param name="str">Ҫ���ܵ��ַ���</param>
        /// <returns></returns>
        public string Encrypt(string str)
        {
            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();//ʵ�������ܽ��ܶ���
            byte[] key = Encoding.Unicode.GetBytes(Key);//�����ֽ����顢�����洢��Կ
            byte[] data = Encoding.Unicode.GetBytes(str);//�����ֽ����顢�����洢Ҫ���ܵ��ַ���
            MemoryStream mstream = new MemoryStream();//ʵ�����ڴ�������
            //ʹ���ڴ���ʵ��������������
            CryptoStream cstream = new CryptoStream(mstream, desc.CreateEncryptor(key, key), CryptoStreamMode.Write);
            cstream.Write(data, 0, data.Length);//���������д������
            cstream.FlushFinalBlock();//�ͷż�����
            return Convert.ToBase64String(mstream.ToArray());//���ؼ��ܺ���ַ���
        }
        /// <summary>
        /// �ַ�������
        /// </summary>
        /// <param name="str">Ҫ���ܵ��ַ���</param>
        /// <returns></returns>
        public string Decrypt(string str)
        {
            DESCryptoServiceProvider desc = new DESCryptoServiceProvider();//ʵ�������ܡ����ܶ���
            byte[] key = Encoding.Unicode.GetBytes(Key);//�����ֽ����������洢��Կ
            byte[] data = Convert.FromBase64String(str);//�����ֽ����������洢��Կ
            MemoryStream mstream = new MemoryStream();//ʵ�����ڴ�������
            //ʹ���ڴ���ʵ�������ܶ���
            CryptoStream cstream = new CryptoStream(mstream, desc.CreateDecryptor(key, key), CryptoStreamMode.Write);
            cstream.Write(data, 0, data.Length);//���������д������
            cstream.FlushFinalBlock();//�ͷŽ�����
            return Encoding.Unicode.GetString(mstream.ToArray());
        }


        public string EncryptString(string sText)
        {
            string sDecode;

            byte[] privateKey1 = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
            byte[] privateKey2 = privateKey1;
            byte[] data = Encoding.UTF8.GetBytes(sText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(privateKey1, privateKey2), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            sDecode = Convert.ToBase64String(ms.ToArray());
            return sDecode;
        }
        public string DecryptString(string sText)
        {
            string sCode;

            byte[] privateKey1 = Encoding.UTF8.GetBytes(Key.Substring(0, 8));
            byte[] privateKey2 = privateKey1;
            byte[] data = Convert.FromBase64String(sText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(privateKey1, privateKey2), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            sCode = Encoding.UTF8.GetString(ms.ToArray());
            return sCode;
        }
        public byte[] CompressWord(string sInput)
        {
            byte[] byteCompress = new byte[sInput.Length + 1];
            int i, j, iLength;
            byte st, btKey;

            iLength = byteCompress.GetUpperBound(0);
            for (i = 0; i < iLength; i++)
            {
                st = System.Convert.ToByte(sInput[i]);
                for (j = 0; j < Key.Length; j++)
                {
                    btKey = System.Convert.ToByte(Key[j]);
                    st ^= btKey;
                }
                byteCompress[i] = st;
            }
            return byteCompress;
        }
        public string UncompressWord(byte[] byteCompress)
        {
            int iLength = byteCompress.Length;
            int i, j;
            byte st, btKey;
            string sWord = "";
            char str;

            for (i = 0; i < iLength - 1; i++)
            {
                st = byteCompress[i];
                for (j = Key.Length - 1; j >= 0; j--)
                {
                    btKey = System.Convert.ToByte(Key[j]);
                    st ^= btKey;
                }
                str = System.Convert.ToChar(st);
                sWord = sWord + str;
            }
            return sWord;
        }
        public string CodeText(string sSource)
        {
            string sKey = Key;

            char cKey, cCode;
            int i, j, iLen, iKey;
            string sCode = "";

            iKey = sKey.Length;
            iLen = sSource.Length;
            for (i = 0; i < iLen; i++)
            {
                cCode = sSource[i];
                for (j = 0; j < iKey; j++)
                {
                    cKey = sKey[j];
                    cCode = (char)(cCode ^ cKey);
                }
                sCode += cCode.ToString();
            }
            return sCode;
        }
        public string DeCodeText(string sCode)
        {
            string sKey = Key;

            char cKey, cCode;
            int i, j, iLen, iKey;
            string sSource = "";

            iKey = sKey.Length;
            iLen = sCode.Length;
            for (i = 0; i < iLen; i++)
            {
                cCode = sCode[i];
                for (j = iKey - 1; j >= 0; j--)
                {
                    cKey = sKey[j];
                    cCode = (char)(cCode ^ cKey);
                }
                sSource += cCode.ToString();
            }
            return sSource;
        }        
    }

    public class DllInvoke
    {
        /// <summary>
        /// Sample:
        /// delegate int doDllFunction(string sDrvPath,string sDrvName,string sResult);
        /// 
        ///    sDllFile = Application.StartupPath + "\\ReadDrvData.dll";            
        ///    DllInvoke dllInvoke = null;
        ///    try
        ///    {
        ///        dllInvoke = new DllInvoke(sDllFile);
        ///        doDllFunction read = (doDllFunction)dllInvoke.Invoke(@"ReadDrvData", typeof(doDllFunction));
        ///        nRet = read(sDrvPath, sDrvName, sResultFile);
        ///    }
        ///    catch (System.Exception ex)
        ///    {
        ///        MessageBox.Show(ex.Message);
        ///    }
        ///    finally
        ///    {
        ///        if (dllInvoke != null)
        ///        {
        ///            dllInvoke.Free();
        ///        }
        ///    }
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// 
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);
        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr lib, string FunName);
        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr lib);
        private IntPtr libr;
        public DllInvoke(string path)
        {
            libr = LoadLibrary(path);
        }
        public Delegate Invoke(string funName, Type type)
        {
            IntPtr api = GetProcAddress(libr, funName);
            return (Delegate)Marshal.GetDelegateForFunctionPointer(api, type);
        }
        public void Free()
        {
            try
            {
                FreeLibrary(libr);
            }
            catch (System.Exception)
            {
            }
        }
        ~DllInvoke()
        {
            FreeLibrary(libr);//�ͷš������  
        }
    }

}
