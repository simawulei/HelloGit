using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace FtpModel
{
    public class FtpUpDown
    {
        string ftpServerIP;
        string ftpUserID;
        string ftpPassword;

        public string m_Error;
        FtpWebRequest reqFTP;
        private void Connect(String path)//����ftp
        {
            // ����uri����FtpWebRequest����
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
            // �޴��������һ���������п�����
            reqFTP.Proxy = null;
            // ָ�����ݴ�������
            reqFTP.UseBinary = true;
            // ftp�û���������
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }

        public FtpUpDown(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIP = ftpServerIP;
            this.ftpUserID = ftpUserID;
            this.ftpPassword = ftpPassword;
        }

        //���������
        private string[] GetFileList(string path, string WRMethods)//����Ĵ���ʾ������δ�ftp�������ϻ���ļ��б�
        {
            string[] downloadFiles=null;
            StringBuilder result = new StringBuilder();
            int iLast;
            try
            {
                m_Error = "";
                Connect(path);
                reqFTP.Method = WRMethods;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);//�����ļ���
                string line = reader.ReadLine();
                if (line != null)
                {
                    while (line != null)
                    {
                        result.Append(line);
                        result.Append("\n");
                        line = reader.ReadLine();
                    }
                    // to remove the trailing '\n'
                    result.Remove(result.ToString().LastIndexOf('\n'), 1);
                    downloadFiles = result.ToString().Split('\n');
                }
                reader.Close();
                response.Close();
                return downloadFiles;
                
            }
            catch (Exception ex)
            {
                m_Error=ex.Message;
                downloadFiles = null;
                return downloadFiles;
            }
        }
        
        public string[] GetFileList(string path)//����Ĵ���ʾ������δ�ftp�������ϻ���ļ��б�
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectory);
        }
        public string[] GetFileList()//����Ĵ���ʾ������δ�ftp�������ϻ���ļ��б�
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectory);
        }
        public bool UploadRename(string filename, string path, string sfilerename,out string errorinfo)
        {
            try
            {
                FileInfo fileInf = new FileInfo(filename);
                string uri = "ftp://" + ftpServerIP + "/" + path + "/" + sfilerename;
                Connect(uri);//����         
                // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر�
                // ��һ������֮��ִ��
                reqFTP.KeepAlive = false;
                // ָ��ִ��ʲô����
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                // �ϴ��ļ�ʱ֪ͨ�������ļ��Ĵ�С
                reqFTP.ContentLength = fileInf.Length;
                // �����С����Ϊkb 
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                // ��һ���ļ���(System.IO.FileStream) ȥ���ϴ����ļ�
                FileStream fs = fileInf.OpenRead();

                // ���ϴ����ļ�д����
                Stream strm = reqFTP.GetRequestStream();
                // ÿ�ζ��ļ�����kb
                contentLen = fs.Read(buff, 0, buffLength);
                // ������û�н���
                while (contentLen != 0)
                {
                    // �����ݴ�file stream д��upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // �ر�������
                strm.Close();
                fs.Close();
                errorinfo = "";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo = ex.Message;
                return false;
            }
        }
        public bool Upload(string filename, string path, out string errorinfo) //����Ĵ���ʵ���˴�ftp�����������ļ��Ĺ���
        {
            try
            {
                FileInfo fileInf = new FileInfo(filename);
                string uri = "ftp://" + ftpServerIP + "/" + path + "/" + fileInf.Name;
                Connect(uri);//����         
                // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر�
                // ��һ������֮��ִ��
                reqFTP.KeepAlive = false;
                // ָ��ִ��ʲô����
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                // �ϴ��ļ�ʱ֪ͨ�������ļ��Ĵ�С
                reqFTP.ContentLength = fileInf.Length;
                // �����С����Ϊkb 
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                // ��һ���ļ���(System.IO.FileStream) ȥ���ϴ����ļ�
                FileStream fs = fileInf.OpenRead();

                // ���ϴ����ļ�д����
                Stream strm = reqFTP.GetRequestStream();
                // ÿ�ζ��ļ�����kb
                contentLen = fs.Read(buff, 0, buffLength);
                // ������û�н���
                while (contentLen != 0)
                {
                    // �����ݴ�file stream д��upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // �ر�������
                strm.Close();
                fs.Close();
                errorinfo = "";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo = ex.Message;
                return false;
            }
        }
        public bool Upload(byte[] byteData, string sfilerename, out string errorinfo)
        {
            try
            {
                List<byte> lstData = new List<byte>();
                int i, iIndex, uploadLen, contentLen = byteData.Length;
                byte[] buff;
                string uri = "ftp://" + ftpServerIP + "/" + sfilerename;
                Connect(uri);//����         
                // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر�
                // ��һ������֮��ִ��
                reqFTP.KeepAlive = false;
                // ָ��ִ��ʲô����
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                // �ϴ��ļ�ʱ֪ͨ�������ļ��Ĵ�С
                reqFTP.ContentLength = contentLen;
                // �����С����Ϊkb 
                int buffLength = 2048;
                for (i = 0; i < contentLen; i++)
                {
                    lstData.Add(byteData[i]);
                }
                // ���ϴ����ļ�д����
                Stream strm = reqFTP.GetRequestStream();
                // ������û�н���
                iIndex = 0;
                while (contentLen != 0)
                {
                    buff = new byte[buffLength];
                    uploadLen = 0;
                    for (i = 0; i < buffLength && iIndex < lstData.Count && contentLen > 0; i++)
                    {
                        buff[i] = lstData[iIndex];
                        contentLen--;
                        uploadLen++;
                        iIndex++;
                    }
                    // �����ݴ�file stream д��upload stream 
                    strm.Write(buff, 0, uploadLen);
                }
                // �ر�������
                strm.Close();
                errorinfo = "";
                return true;
            }
            catch (System.Exception ex)
            {
                errorinfo = ex.Message;
                return false;
            }
        }
        public bool GetFileLastModified(string fileName, out DateTime deTime, out string errorinfo)
        {
            bool bRet = false;
            FtpWebResponse response=null;
            errorinfo = "";

            deTime = DateTime.MinValue;
            try
            {
                string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//����  
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                response = (FtpWebResponse)reqFTP.GetResponse();
                deTime = response.LastModified;
                bRet = true;
            }
            catch (System.Exception ex)
            {
                errorinfo = ex.Message;
            }
            finally
            {
                response.Close();
            }

            return bRet;
        }
        public bool Download(out byte[] byteData, string fileName, out string errorinfo)////����Ĵ���ʵ���˴�ftp�����������ļ��Ĺ���
        {
            try
            {
                List<byte> lstData = new List<byte>();
                string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//����  
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount,i;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);                
                while (readCount > 0)
                {
                    for (i = 0; i < readCount; i++)
                    {
                        lstData.Add(buffer[i]);
                    }
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                response.Close();
                byteData = new byte[lstData.Count];
                for (i = 0; i < byteData.Length;i++) {
                    byteData[i] = lstData[i];
                }

                errorinfo = "";

                return true;

            }
            catch (Exception ex)
            {
                errorinfo = string.Format("��{0},�޷�����", ex.Message);
                byteData = null;
                return false;
            }

        }
        public bool Download(string filePath, string fileName, out string errorinfo)////����Ĵ���ʵ���˴�ftp�����������ļ��Ĺ���
        {
            try
            {
                String onlyFileName = Path.GetFileName(fileName);
                string newFileName = filePath + "\\" + onlyFileName;
                if (File.Exists(newFileName))
                {
                    errorinfo = string.Format("�����ļ�{0}�Ѵ���,�޷�����", newFileName);
                    return false;
                }
                string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//����  
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();                
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);

                FileStream outputStream = new FileStream(newFileName, FileMode.Create);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();

                errorinfo = "";

                return true;

            }
            catch (Exception ex)
            {
                errorinfo = string.Format("��{0},�޷�����", ex.Message);
                return false;
            }

        }
        public bool DownloadRename(string newFileName, string fileName, out string errorinfo)////����Ĵ���ʵ���˴�ftp�����������ļ��Ĺ���
        {
            try
            {
                if (File.Exists(newFileName))
                {
                    errorinfo = string.Format("�����ļ�{0}�Ѵ���,�޷�����", newFileName);
                    return false;
                }
                string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//����  
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);

                FileStream outputStream = new FileStream(newFileName, FileMode.Create);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();

                errorinfo = "";

                return true;

            }
            catch (Exception ex)
            {
                errorinfo = string.Format("��{0},�޷�����", ex.Message);
                return false;
            }

        }
        //�����ļ�
        public bool CopyFile(string sourceFileName, string objectFileName, out string errorinfo)
        {
            byte[] byteData;
            errorinfo = "";
            bool bRet = Download(out byteData, sourceFileName, out errorinfo);
            if (bRet)
            {
                bRet = Upload(byteData, objectFileName, out errorinfo);
            }
            return bRet;
        }
        //ɾ���ļ�

        public bool DeleteFileName(string fileName,out string errorinfo)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(uri);//����         
                // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر�
                // ��һ������֮��ִ��
                reqFTP.KeepAlive = false;
                // ָ��ִ��ʲô����
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                errorinfo="";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo=ex.Message;
                return false;
            }
        }

        //����Ŀ¼
        public bool MakeDir(string dirName,out string errorinfo)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//����      
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                errorinfo="";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo=ex.Message;
                return false;
            }

        }
        public bool MakeDirEx(string dirName, out string errorinfo)
        {
            string[] sDir;
            string sFullDir = "";
            int i;
            bool bRet = false;

            errorinfo = "";
            try
            {
                dirName=dirName.Replace("\\", "/");
                if (dirName.IndexOf("/") >= 0)
                {
                    sDir = dirName.Split('/');
                }
                else
                {
                    sDir = new string[1];
                    sDir[0] = dirName;
                }
                for (i = 0; i < sDir.Length; i++)
                {
                    if (sDir[i].Length > 0)
                    {
                        sFullDir = sFullDir + "/" + sDir[i];
                        if (sFullDir[0] == '/') { sFullDir = sFullDir.Substring(1); }
                        bRet = MakeDir(sFullDir, out errorinfo);
                    }
                }
            }
            catch (System.Exception ex)
            {
                errorinfo = ex.Message;
            }
            
            
            return bRet;
        }
        //ɾ��Ŀ¼

        public bool delDir(string dirName,out string errorinfo)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//����      
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                errorinfo="";
                return true;
            }
            catch (Exception ex)
            {
                errorinfo=ex.Message;
                return false;
            }
        }

        //����ļ���С

        public long GetFileSize(string filename,out string errorinfo)
        {
            long fileSize = 0;
            try
            {
                FileInfo fileInf = new FileInfo(filename);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                Connect(uri);//����      
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                fileSize = response.ContentLength;
                response.Close();
                errorinfo = "";
            }
            catch (Exception ex)
            {
                errorinfo=ex.Message;
                return 0;
            }
            return fileSize;

        }

        //�ļ�����
        public bool Rename(string currentFilename, string newFilename,out string errorinfo)
        {
            try
            {
                FileInfo fileInf = new FileInfo(currentFilename);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                Connect(uri);//����
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
                errorinfo="";
                return true;
            }

            catch (Exception ex)
            {
                errorinfo=ex.Message;
                return false;
            }

        }

        //����ļ�����

        public string[] GetFilesDetailList()
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        //����ļ�����

        public string[] GetFilesDetailList(string path)
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectoryDetails);
        }

    }
}
