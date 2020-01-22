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
        private void Connect(String path)//连接ftp
        {
            // 根据uri创建FtpWebRequest对象
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
            // 无代理，否则第一次连接运行可能慢
            reqFTP.Proxy = null;
            // 指定数据传输类型
            reqFTP.UseBinary = true;
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }

        public FtpUpDown(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIP = ftpServerIP;
            this.ftpUserID = ftpUserID;
            this.ftpPassword = ftpPassword;
        }

        //都调用这个
        private string[] GetFileList(string path, string WRMethods)//上面的代码示例了如何从ftp服务器上获得文件列表
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
                StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);//中文文件名
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
        
        public string[] GetFileList(string path)//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectory);
        }
        public string[] GetFileList()//上面的代码示例了如何从ftp服务器上获得文件列表
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectory);
        }
        public bool UploadRename(string filename, string path, string sfilerename,out string errorinfo)
        {
            try
            {
                FileInfo fileInf = new FileInfo(filename);
                string uri = "ftp://" + ftpServerIP + "/" + path + "/" + sfilerename;
                Connect(uri);//连接         
                // 默认为true，连接不会被关闭
                // 在一个命令之后被执行
                reqFTP.KeepAlive = false;
                // 指定执行什么命令
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                // 上传文件时通知服务器文件的大小
                reqFTP.ContentLength = fileInf.Length;
                // 缓冲大小设置为kb 
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                // 打开一个文件流(System.IO.FileStream) 去读上传的文件
                FileStream fs = fileInf.OpenRead();

                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的kb
                contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // 关闭两个流
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
        public bool Upload(string filename, string path, out string errorinfo) //上面的代码实现了从ftp服务器上载文件的功能
        {
            try
            {
                FileInfo fileInf = new FileInfo(filename);
                string uri = "ftp://" + ftpServerIP + "/" + path + "/" + fileInf.Name;
                Connect(uri);//连接         
                // 默认为true，连接不会被关闭
                // 在一个命令之后被执行
                reqFTP.KeepAlive = false;
                // 指定执行什么命令
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                // 上传文件时通知服务器文件的大小
                reqFTP.ContentLength = fileInf.Length;
                // 缓冲大小设置为kb 
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                // 打开一个文件流(System.IO.FileStream) 去读上传的文件
                FileStream fs = fileInf.OpenRead();

                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的kb
                contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // 关闭两个流
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
                Connect(uri);//连接         
                // 默认为true，连接不会被关闭
                // 在一个命令之后被执行
                reqFTP.KeepAlive = false;
                // 指定执行什么命令
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                // 上传文件时通知服务器文件的大小
                reqFTP.ContentLength = contentLen;
                // 缓冲大小设置为kb 
                int buffLength = 2048;
                for (i = 0; i < contentLen; i++)
                {
                    lstData.Add(byteData[i]);
                }
                // 把上传的文件写入流
                Stream strm = reqFTP.GetRequestStream();
                // 流内容没有结束
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
                    // 把内容从file stream 写入upload stream 
                    strm.Write(buff, 0, uploadLen);
                }
                // 关闭两个流
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
                Connect(url);//连接  
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
        public bool Download(out byte[] byteData, string fileName, out string errorinfo)////上面的代码实现了从ftp服务器下载文件的功能
        {
            try
            {
                List<byte> lstData = new List<byte>();
                string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//连接  
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
                errorinfo = string.Format("因{0},无法下载", ex.Message);
                byteData = null;
                return false;
            }

        }
        public bool Download(string filePath, string fileName, out string errorinfo)////上面的代码实现了从ftp服务器下载文件的功能
        {
            try
            {
                String onlyFileName = Path.GetFileName(fileName);
                string newFileName = filePath + "\\" + onlyFileName;
                if (File.Exists(newFileName))
                {
                    errorinfo = string.Format("本地文件{0}已存在,无法下载", newFileName);
                    return false;
                }
                string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//连接  
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
                errorinfo = string.Format("因{0},无法下载", ex.Message);
                return false;
            }

        }
        public bool DownloadRename(string newFileName, string fileName, out string errorinfo)////上面的代码实现了从ftp服务器下载文件的功能
        {
            try
            {
                if (File.Exists(newFileName))
                {
                    errorinfo = string.Format("本地文件{0}已存在,无法下载", newFileName);
                    return false;
                }
                string url = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(url);//连接  
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
                errorinfo = string.Format("因{0},无法下载", ex.Message);
                return false;
            }

        }
        //复制文件
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
        //删除文件

        public bool DeleteFileName(string fileName,out string errorinfo)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + fileName;
                Connect(uri);//连接         
                // 默认为true，连接不会被关闭
                // 在一个命令之后被执行
                reqFTP.KeepAlive = false;
                // 指定执行什么命令
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

        //创建目录
        public bool MakeDir(string dirName,out string errorinfo)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//连接      
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
        //删除目录

        public bool delDir(string dirName,out string errorinfo)
        {
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + dirName;
                Connect(uri);//连接      
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

        //获得文件大小

        public long GetFileSize(string filename,out string errorinfo)
        {
            long fileSize = 0;
            try
            {
                FileInfo fileInf = new FileInfo(filename);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                Connect(uri);//连接      
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

        //文件改名
        public bool Rename(string currentFilename, string newFilename,out string errorinfo)
        {
            try
            {
                FileInfo fileInf = new FileInfo(currentFilename);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                Connect(uri);//连接
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

        //获得文件明晰

        public string[] GetFilesDetailList()
        {
            return GetFileList("ftp://" + ftpServerIP + "/", WebRequestMethods.Ftp.ListDirectoryDetails);
        }

        //获得文件明晰

        public string[] GetFilesDetailList(string path)
        {
            return GetFileList("ftp://" + ftpServerIP + "/" + path, WebRequestMethods.Ftp.ListDirectoryDetails);
        }

    }
}
