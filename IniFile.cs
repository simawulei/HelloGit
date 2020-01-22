using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace UtilModel
{
    /// <summary>
    /// IniFile 的摘要说明
    /// </summary>
    public class IniFile
    {
        public string FileName; //INI文件名
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);
        //类的构造函数，传递INI文件名 
        public IniFile()
        {
        }
        public IniFile(string sFileName)
        {
            // 判断文件是否存在 
            //FileInfo fileInfo = new FileInfo(sFileName);
            //if ((!fileInfo.Exists))
            //{
            //    System.IO.StreamWriter sw = new System.IO.StreamWriter(sFileName, false, System.Text.Encoding.Default);
            //    File.WriteAllText(sFileName, "", System.Text.Encoding.Default);
            //}
            //FileName = fileInfo.FullName;
            if (!File.Exists(sFileName))
            {
                File.WriteAllText(sFileName, "", System.Text.Encoding.Default);
            }
            FileName = sFileName;
        }
        /// <summary>
        /// 创建一个空ini文件，如果文件已经存在，则不覆盖
        /// </summary>
        /// <param name="sFileName"></param>       
        public void SetFileName(string sFileName)
        {
            // 判断文件是否存在 
            //FileInfo fileInfo = new FileInfo(sFileName);
            //if ((!fileInfo.Exists))
            //{
            //    System.IO.StreamWriter sw = new System.IO.StreamWriter(sFileName, false, System.Text.Encoding.Default); 
            //}
            //FileName = fileInfo.FullName;
            if (!File.Exists(sFileName))
            {
                File.WriteAllText(sFileName, "", System.Text.Encoding.Default);
            }
            FileName = sFileName;
        }
        //写INI文件 
        public void WriteString(string Section, string Ident, string Value)
        {
            if (!WritePrivateProfileString(Section, Ident, Value, FileName))
            {
                throw (new ApplicationException("Write Ini File Error"));
            }
        }
        //读取INI文件指定 
        public string ReadString(string Section, string Ident, string Default)
        {
            Byte[] Buffer = new Byte[65535];
            int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FileName);
            string s = Encoding.GetEncoding(0).GetString(Buffer);
            s = s.Substring(0, bufLen);
            s = s.Trim();

            int i;
            i = s.Length;
            if (i > 0)
            {
                while (char.IsControl(s, i - 1) && i > 0)
                {
                    s = s.Substring(0, i - 1);
                    i = s.Length;
                }
            }
            return s;

        }
        //读整数 
        public int ReadInteger(string Section, string Ident, int Default)
        {
            string intStr = ReadString(Section, Ident, Convert.ToString(Default));
            try
            {
                return Convert.ToInt32(intStr);
            }
            catch (Exception)
            {
                return Default;
            }
        }
        //写整数 
        public void WriteInteger(string Section, string Ident, int Value)
        {
            WriteString(Section, Ident, Value.ToString());
        }
        //读实数
        public double ReadDouble(string Section, string Ident, double Default)
        {
            string intStr = ReadString(Section, Ident, Convert.ToString(Default));
            try
            {
                return Convert.ToDouble (intStr);
            }
            catch (Exception)
            {
                return Default;
            }
        }
        //写实数
        public void WriteDouble(string Section, string Ident, double Value)
        {
            WriteString(Section, Ident, Value.ToString());
        }
        //读布尔 
        public bool ReadBool(string Section, string Ident, bool Default)
        {
            try
            {
                return Convert.ToBoolean(ReadString(Section, Ident, Convert.ToString(Default)));
            }
            catch (Exception)
            {
                return Default;
            }
        }
        //写Bool 
        public void WriteBool(string Section, string Ident, bool Value)
        {
            WriteString(Section, Ident, Convert.ToString(Value));
        }
        //从Ini文件中，将指定的Section名称中的所有Ident添加到列表中 
        public void ReadSection(string Section, StringCollection Idents)
        {
            Byte[] Buffer = new Byte[16384];
            int bufLen = GetPrivateProfileString(Section, null, null, Buffer, Buffer.GetUpperBound(0), FileName);
            //对Section进行解析 
            GetStringsFromBuffer(Buffer, bufLen, Idents);
        }
        private void GetStringsFromBuffer(Byte[] Buffer, int bufLen, StringCollection Strings)
        {
            Strings.Clear();
            if (bufLen != 0)
            {
                int start = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((Buffer[i] == 0) && ((i - start) > 0))
                    {
                        String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);
                        Strings.Add(s);
                        start = i + 1;
                    }
                }
            }
        }
        //从Ini文件中，读取所有的Sections的名称 
        public void ReadSections(StringCollection SectionList)
        {
            //Note:必须得用Bytes来实现，StringBuilder只能取到第一个Section 
            byte[] Buffer = new byte[65535];
            int bufLen = 0;
            bufLen = GetPrivateProfileString(null, null, null, Buffer,
            Buffer.GetUpperBound(0), FileName);
            GetStringsFromBuffer(Buffer, bufLen, SectionList);
        }
        //读取指定的Section的所有Value到列表中 
        public void ReadSectionValues(string Section, NameValueCollection Values)
        {
            StringCollection KeyList = new StringCollection();
            ReadSection(Section, KeyList);
            Values.Clear();
            foreach (string key in KeyList)
            {
                Values.Add(key, ReadString(Section, key, ""));
            }
        }
        //清除某个Section 
        public void EraseSection(string Section)
        {
            if (!WritePrivateProfileString(Section, null, null, FileName))
            {
                throw (new ApplicationException("Can't clear Section in inifile"));
            }
        }
        //删除某个Section下的键 
        public void DeleteKey(string Section, string Ident)
        {
            WritePrivateProfileString(Section, Ident, null, FileName);
        }
        //检查某个Section下的某个键值是否存在 
        public bool ValueExists(string Section, string Ident)
        {
            // 
            StringCollection Idents = new StringCollection();
            ReadSection(Section, Idents);
            return Idents.IndexOf(Ident) > -1;
        }
        //Note:对于Win9X，来说需要实现UpdateFile方法将缓冲中的数据写入文件 
        //在Win NT, 2000和XP上，都是直接写文件，没有缓冲，所以，无须实现UpdateFile 
        //执行完对Ini文件的修改之后，应该调用本方法更新缓冲区。 
        public void UpdateFile()
        {
            WritePrivateProfileString(null, null, null, FileName);
        }
        //确保资源的释放 
        ~IniFile()
        {
            UpdateFile();
        }
    }
}