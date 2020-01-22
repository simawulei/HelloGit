using System;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;


namespace UtilModel
{
    /// <summary>
    /// <para>处理幻灯类</para>
    /// </summary>
    public class BitmapSlide
    {
        const int SLD_HAEAR = 31;

        public Color m_Background;
        public int m_Border = 2;

        //private byte[] m_byteData;
        private int m_Position;
        private string m_SlideHead;
        public List<SlideItem> m_aySlideItem;

        public int m_SlideWidth = 0;
        public int m_SlideHeight = 0;
        public Point m_SlidePointMin;
        public Point m_SlidePointMax;

        public class SlideItem
        {
            public Color m_Color;
            public int m_Type;
            public List<Point> m_ayPoints;

            public SlideItem()
            {
                m_Color = Color.Black;
                m_Type = 0;
                m_ayPoints = new List<Point>();
            }
            public void ClearPoints()
            {
                m_ayPoints.Clear();
            }
            public void AddPoint(Point ptItem)
            {
                m_ayPoints.Add(ptItem);
            }
            public void OffsetPoint(int iOffsetX, int iOffsetY)
            {
                int i;
                Point pt;
                for (i = 0; i < m_ayPoints.Count; i++)
                {
                    pt = m_ayPoints[i];
                    pt.X += iOffsetX;
                    pt.Y += iOffsetY;
                    m_ayPoints[i] = pt;
                }

            }
            public void DrawItem(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
            {
                Pen myPen = new Pen(m_Color, 1);
                Single x1, y1, x2, y2;
                if (m_ayPoints.Count > 1)
                {
                    if (m_Type == 0)
                    {
                        x1 = (Single)(m_ayPoints[0].X * dScale);
                        y1 = (Single)(m_ayPoints[0].Y * dScale);
                        x2 = (Single)(m_ayPoints[1].X * dScale);
                        y2 = (Single)(m_ayPoints[1].Y * dScale);
                        y1 = nHeight - y1;
                        y2 = nHeight - y2;
                        x1 += iOffsetX;
                        y1 -= iOffsetY;
                        x2 += iOffsetX;
                        y2 -= iOffsetY;
                        g.DrawLine(myPen, x1, y1, x2, y2);
                    }
                    else
                    {
                        SolidBrush mySolidBrush = new SolidBrush(m_Color);
                        GraphicsPath myPath = new GraphicsPath();
                        int i, iNums = m_ayPoints.Count;
                        if (iNums >= 3)
                        {
                            for (i = 0; i < iNums; i++)
                            {
                                if (i < iNums - 1)
                                {
                                    x1 = (Single)(m_ayPoints[i].X * dScale);
                                    y1 = (Single)(m_ayPoints[i].Y * dScale);
                                    x2 = (Single)(m_ayPoints[i + 1].X * dScale);
                                    y2 = (Single)(m_ayPoints[i + 1].Y * dScale);
                                }
                                else
                                {
                                    x1 = (Single)(m_ayPoints[0].X * dScale);
                                    y1 = (Single)(m_ayPoints[0].Y * dScale);
                                    x2 = (Single)(m_ayPoints[i].X * dScale);
                                    y2 = (Single)(m_ayPoints[i].Y * dScale);
                                }
                                y1 = nHeight - y1;
                                y2 = nHeight - y2;
                                x1 += iOffsetX;
                                y1 -= iOffsetY;
                                x2 += iOffsetX;
                                y2 -= iOffsetY;
                                myPath.AddLine(x1, y1, x2, y2);
                            }
                            g.FillPath(mySolidBrush, myPath);
                        }
                    }
                }
            }
        }

        public BitmapSlide()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            m_SlideHead = "AutoCAD Slide\r\n";
            m_Background = Color.Black;
        }
        //private byte[] GetSlideData(long iLength)
        //{
        //    byte[] byteRet;
        //    int i;
        //    byteRet = new byte[iLength];

        //    for (i = 0; i < iLength; i++)
        //    {
        //        byteRet[i] = m_byteData[i + m_Position];
        //    }        
        //    return byteRet;
        //}
        private string byteToHexString(byte[] bytes, bool fenge)
        {
            string str, strsub;
            str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                strsub = string.Format("{0:X}", bytes[i]);
                if (strsub.Length == 1) { strsub = "0" + strsub; }
                str += strsub;
                if (fenge && (i != bytes.Length - 1))
                {
                    str += string.Format("{0}", ",");
                }
            }
            return str.ToUpper();
        }
        private int byteToInt(byte[] bytes)
        {
            int iHei = (int)(bytes[1] * Math.Pow(16, 2));
            int iLow = bytes[0];
            return iHei + iLow;
        }
        private Color GetColorByIndex(int nIndex)
        {
            int nR = 0, nG = 0, nB = 0;
            if (nIndex == 0 || nIndex == 7 || nIndex == 255 || nIndex < 0 || nIndex > 255)
            {
                if (m_Background == Color.White) { return Color.Black; }
                else if (m_Background == Color.Black) { return Color.White; }
            }

            ////base 0
            if (nIndex < 10)
            {
                switch (nIndex)
                {
                    case 0:
                        nR = 0; nG = 0; nB = 0;
                        break;
                    case 1:
                        nR = 255; nG = 0; nB = 0;
                        break;
                    case 2:
                        nR = 255; nG = 255; nB = 0;
                        break;
                    case 3:
                        nR = 0; nG = 255; nB = 0;
                        break;
                    case 4:
                        nR = 0; nG = 255; nB = 255;
                        break;
                    case 5:
                        nR = 0; nG = 0; nB = 255;
                        break;
                    case 6:
                        nR = 255; nG = 0; nB = 255;
                        break;
                    case 7:
                        nR = 255; nG = 0; nB = 255;
                        break;
                    case 8:
                        nR = 65; nG = 65; nB = 65;
                        break;
                    case 9:
                        nR = 128; nG = 128; nB = 128;
                        break;
                }
            }
            else if (nIndex > 249)
            {
                switch (nIndex)
                {
                    case 250:
                        nR = 51; nG = 51; nB = 51;
                        break;
                    case 251:
                        nR = 80; nG = 80; nB = 80;
                        break;
                    case 252:
                        nR = 105; nG = 105; nB = 105;
                        break;
                    case 253:
                        nR = 130; nG = 130; nB = 130;
                        break;
                    case 254:
                        nR = 190; nG = 190; nB = 190;
                        break;
                    case 255:
                        nR = 255; nG = 255; nB = 255;
                        break;
                }
            }
            else if (nIndex > 9)
            {//RGB
                int m = 0, n = 0, w;
                //int marray[10]={64,32,51,25,38,19,32,16,19,9}; 
                int[] marray = new int[10];
                marray[0] = 64; marray[1] = 32; marray[2] = 51; marray[3] = 25; marray[4] = 38;
                marray[5] = 19; marray[6] = 32; marray[7] = 16; marray[8] = 19; marray[9] = 9;
                if (nIndex < 60)
                {
                    int[,] narray = new int[10, 3] { { 255, 0, 0 }, { 255, 127, 127 }, { 204, 0, 0 }, { 204, 102, 102 }, { 153, 0, 0 }, { 153, 76, 76 }, { 127, 0, 0 }, { 127, 63, 63 }, { 76, 0, 0 }, { 76, 38, 38 } };
                    m = nIndex % 10;
                    n = nIndex / 10 - 1;
                    w = narray[m, 1] + marray[m] * n;
                    if (w > 255)
                        w = 255;
                    //ulC = RGB(narray[m][0],w,narray[m][2]);	
                    nR = narray[m, 0];
                    nG = w;
                    nB = narray[m, 2];
                }
                else if (nIndex < 100)
                {
                    int[,] narray = new int[10, 3] { { 191, 255, 0 }, { 223, 255, 127 }, { 153, 204, 0 }, { 178, 204, 102 }, { 114, 153, 0 }, { 133, 153, 76 }, { 95, 127, 0 }, { 111, 127, 63 }, { 57, 76, 0 }, { 66, 76, 38 } };
                    m = nIndex % 10;
                    n = nIndex / 10 - 6;
                    w = narray[m, 0] - marray[m] * n;
                    if (w < 0)
                        w = 0;
                    //ulC = RGB(w,narray[m][1],narray[m][2]);
                    nR = w;
                    nG = narray[m, 1];
                    nB = narray[m, 2];
                }
                else if (nIndex < 140)
                {
                    int[,] narray = new int[10, 3] { { 0, 255, 63 }, { 127, 255, 159 }, { 0, 204, 51 }, { 102, 204, 127 }, { 0, 153, 38 }, { 76, 153, 95 }, { 0, 127, 31 }, { 63, 127, 79 }, { 0, 76, 19 }, { 38, 76, 47 } };
                    m = nIndex % 10;
                    n = nIndex / 10 - 10;
                    w = narray[m, 2] + marray[m] * n;
                    if (w > 255)
                        w = 255;
                    //ulC = RGB(narray[m][0],narray[m][1],w);
                    nR = narray[m, 0];
                    nG = narray[m, 1];
                    nB = w;
                }
                else if (nIndex < 180)
                {
                    int[,] narray = new int[10, 3] { { 0, 191, 255 }, { 127, 223, 255 }, { 0, 153, 204 }, { 102, 178, 204 }, { 0, 114, 153 }, { 76, 133, 153 }, { 0, 95, 127 }, { 63, 111, 127 }, { 0, 57, 76 }, { 38, 66, 76 } };
                    m = nIndex % 10;
                    n = nIndex / 10 - 14;
                    w = narray[m, 1] - marray[m] * n;
                    if (w < 0)
                        w = 0;
                    //ulC = RGB(narray[m][0],w,narray[m][2]);
                    nR = narray[m, 0];
                    nG = w;
                    nB = narray[m, 2];
                }
                else if (nIndex < 220)
                {
                    int[,] narray = new int[10, 3] { { 63, 0, 255 }, { 159, 127, 255 }, { 51, 0, 204 }, { 127, 102, 204 }, { 38, 0, 153 }, { 95, 76, 153 }, { 31, 0, 127 }, { 79, 63, 127 }, { 19, 0, 76 }, { 47, 38, 76 } };
                    m = nIndex % 10;
                    n = nIndex / 10 - 18;
                    w = narray[m, 0] + marray[m] * n;
                    if (w > 255)
                        w = 255;
                    //ulC = RGB(w,narray[m][1],narray[m][2]);
                    nR = w;
                    nG = narray[m, 1];
                    nB = narray[m, 2];
                }
                else if (nIndex < 250)
                {
                    int[,] narray = new int[10, 3] { { 255, 0, 191 }, { 255, 127, 223 }, { 204, 0, 153 }, { 204, 102, 178 }, { 153, 0, 114 }, { 153, 76, 133 }, { 127, 0, 95 }, { 127, 63, 111 }, { 76, 0, 57 }, { 76, 38, 66 } };
                    m = nIndex % 10;
                    n = nIndex / 10 - 22;
                    w = narray[m, 2] - marray[m] * n;
                    if (w < 0)
                        w = 0;
                    //ulC = RGB(narray[m][0],narray[m][1],narray[m][2] - marray[m]*n);
                    nR = narray[m, 0];
                    nG = narray[m, 1];
                    nB = narray[m, 2] - marray[m] * n;
                }
            }
            return Color.FromArgb(nR, nG, nB);
        }
        /// <summary>
        /// <para>从文件字节数据读取幻灯片</para>
        /// </summary>
        /// <param name="byteSlide"></param>
        /// <returns></returns>
        public bool ReadSlide(byte[] byteSlide)
        {
            byte[] byteHead, byteData;
            string sHead = "";
            int nColor, nHead;
            SlideItem cItem;
            bool bFirst = true;
            Point ptMin, ptMax;
            bool bRet = false;
            int n, iCurrent = 0;
            int iLength = byteSlide.Length;

            m_aySlideItem = new List<SlideItem>();
            m_SlideWidth = 0;
            m_SlideHeight = 0;
            m_SlidePointMin = new Point();
            m_SlidePointMax = new Point();
            ptMin = new Point();
            ptMax = new Point();

            if (iLength > SLD_HAEAR)
            {
                //m_byteData = File.ReadAllBytes(sSlideFile);
                Point[] ptVect;
                int delXY;
                ptVect = new Point[2];
                //FileStream fs = File.OpenRead(sSlideFile);
                byteHead = new byte[15];
                //nByteRead = fs.Read(byteHead, 0, 15);
                for (n = 0; n < 15; n++) { byteHead[n] = byteSlide[n]; }

                sHead = System.Text.Encoding.Default.GetString(byteHead);
                if (string.Compare(sHead, m_SlideHead, true) == 0)
                {
                    m_Position = SLD_HAEAR;
                    byteData = new byte[2];
                    //fs.Seek(m_Position, SeekOrigin.Begin);
                    //nByteRead = fs.Read(byteData, 0, 2);
                    iCurrent = m_Position;
                    byteData[0] = byteSlide[iCurrent];
                    byteData[1] = byteSlide[iCurrent + 1];
                    iCurrent = iCurrent + 2;

                    nHead = byteData[0];
                    byteHead = new byte[2];
                    //nByteRead = fs.Read(byteHead, 0, 2);
                    byteHead[0] = byteSlide[iCurrent];
                    byteHead[1] = byteSlide[iCurrent + 1];
                    iCurrent = iCurrent + 2;
                    nColor = byteData[0];

                    //fs.Seek(-2, SeekOrigin.Current);
                    iCurrent = iCurrent - 2;
                    if (byteData[0] == byteHead[0] && byteData[1] == byteHead[1])
                    {
                        while (true)
                        {
                            byteHead = new byte[2];
                            //if (fs.Read(byteHead, 0, 2) != 2) { break; }
                            if (iCurrent < iLength - 1)
                            {
                                byteHead[0] = byteSlide[iCurrent];
                                byteHead[1] = byteSlide[iCurrent + 1];
                                iCurrent = iCurrent + 2;
                            }
                            else { break; }
                            sHead = byteToHexString(byteHead, true);
                            if (byteHead[1] == 252)
                            {
                                break;
                            }
                            else if (byteHead[1] == 253)
                            {
                                //fill
                                byteData = new byte[2];
                                //if (fs.Read(byteData, 0, 2) != 2) { break; }
                                //fs.Seek(2, SeekOrigin.Current);
                                if (iCurrent < iLength - 1)
                                {
                                    byteData[0] = byteSlide[iCurrent];
                                    byteData[1] = byteSlide[iCurrent + 1];
                                    iCurrent = iCurrent + 2;
                                }
                                else { break; }
                                iCurrent = iCurrent + 2;

                                int i, iNums = byteToInt(byteData);
                                Point ptNode = new Point();

                                cItem = new SlideItem();
                                cItem.m_Type = 1;
                                cItem.m_Color = GetColorByIndex(nColor);
                                for (i = 0; i < iNums; i++)
                                {
                                    //fs.Seek(2, SeekOrigin.Current);
                                    iCurrent = iCurrent + 2;
                                    //if (fs.Read(byteData, 0, 2) != 2) { break; }
                                    if (iCurrent < iLength - 1)
                                    {
                                        byteData[0] = byteSlide[iCurrent];
                                        byteData[1] = byteSlide[iCurrent + 1];
                                        iCurrent = iCurrent + 2;
                                    }
                                    else { break; }

                                    ptNode.X = byteToInt(byteData);
                                    //if (fs.Read(byteData, 0, 2) != 2) { break; }
                                    if (iCurrent < iLength - 1)
                                    {
                                        byteData[0] = byteSlide[iCurrent];
                                        byteData[1] = byteSlide[iCurrent + 1];
                                        iCurrent = iCurrent + 2;
                                    }
                                    else { break; }

                                    ptNode.Y = byteToInt(byteData);
                                    //Add point
                                    if (i < iNums - 1)
                                    {
                                        cItem.AddPoint(ptNode);
                                        if (bFirst)
                                        {
                                            ptMin = ptNode;
                                            ptMax = ptNode;
                                            bFirst = false;
                                        }
                                        else
                                        {
                                            ptMin.X = Math.Min(ptMin.X, ptNode.X);
                                            ptMin.Y = Math.Min(ptMin.Y, ptNode.Y);
                                            ptMax.X = Math.Max(ptMax.X, ptNode.X);
                                            ptMax.Y = Math.Max(ptMax.Y, ptNode.Y);
                                        }
                                    }
                                }
                                m_aySlideItem.Add(cItem);
                            }
                            else if (byteHead[1] == 255)
                            {
                                //color
                                nColor = byteHead[0];
                            }
                            else if (byteHead[1] == 254)
                            {
                                //endpoint vector
                                ptVect[1] = ptVect[0];
                                delXY = byteHead[0];
                                ptVect[1].X += delXY;
                                byteData = new byte[1];
                                //if (fs.Read(byteData, 0, 1) != 1) { break; }
                                if (iCurrent < iLength)
                                {
                                    byteData[0] = byteSlide[iCurrent];
                                    iCurrent = iCurrent + 1;
                                }
                                else { break; }

                                delXY = byteData[0];
                                ptVect[1].Y += delXY;
                                //Add Point
                                cItem = new SlideItem();
                                cItem.AddPoint(ptVect[0]);
                                cItem.AddPoint(ptVect[1]);
                                cItem.m_Type = 0;
                                cItem.m_Color = GetColorByIndex(nColor);
                                m_aySlideItem.Add(cItem);
                                if (bFirst)
                                {
                                    ptMin.X = Math.Min(ptVect[0].X, ptVect[1].X);
                                    ptMin.Y = Math.Min(ptVect[0].Y, ptVect[1].Y);
                                    ptMax.X = Math.Max(ptVect[0].X, ptVect[1].X);
                                    ptMax.Y = Math.Max(ptVect[0].Y, ptVect[1].Y);
                                    bFirst = false;
                                }
                                else
                                {
                                    ptMin.X = Math.Min(ptMin.X, Math.Min(ptVect[0].X, ptVect[1].X));
                                    ptMin.Y = Math.Min(ptMin.Y, Math.Min(ptVect[0].Y, ptVect[1].Y));
                                    ptMax.X = Math.Max(ptMax.X, Math.Max(ptVect[0].X, ptVect[1].X));
                                    ptMax.Y = Math.Max(ptMax.Y, Math.Max(ptVect[0].Y, ptVect[1].Y));
                                }

                                ptVect[0] = ptVect[1];
                            }
                            else if (byteHead[1] == 251)
                            {
                                //Offset vector
                                ptVect[1] = ptVect[0];
                                //from
                                delXY = byteHead[0];
                                ptVect[0].X += delXY;
                                byteData = new byte[1];
                                //if (fs.Read(byteData, 0, 1) != 1) { break; }
                                if (iCurrent < iLength)
                                {
                                    byteData[0] = byteSlide[iCurrent];
                                    iCurrent = iCurrent + 1;
                                }
                                else { break; }

                                delXY = byteData[0];
                                ptVect[0].Y += delXY;
                                //to
                                //if (fs.Read(byteData, 0, 1) != 1) { break; }
                                if (iCurrent < iLength)
                                {
                                    byteData[0] = byteSlide[iCurrent];
                                    iCurrent = iCurrent + 1;
                                }
                                else { break; }

                                delXY = byteData[0];
                                ptVect[1].X += delXY;
                                byteData = new byte[1];
                                //if (fs.Read(byteData, 0, 1) != 1) { break; }
                                if (iCurrent < iLength)
                                {
                                    byteData[0] = byteSlide[iCurrent];
                                    iCurrent = iCurrent + 1;
                                }
                                else { break; }

                                delXY = byteData[0];
                                ptVect[1].Y += delXY;
                                //Add Point
                                cItem = new SlideItem();
                                cItem.AddPoint(ptVect[0]);
                                cItem.AddPoint(ptVect[1]);
                                cItem.m_Type = 0;
                                cItem.m_Color = GetColorByIndex(nColor);
                                m_aySlideItem.Add(cItem);
                                if (bFirst)
                                {
                                    ptMin.X = Math.Min(ptVect[0].X, ptVect[1].X);
                                    ptMin.Y = Math.Min(ptVect[0].Y, ptVect[1].Y);
                                    ptMax.X = Math.Max(ptVect[0].X, ptVect[1].X);
                                    ptMax.Y = Math.Max(ptVect[0].Y, ptVect[1].Y);
                                    bFirst = false;
                                }
                                else
                                {
                                    ptMin.X = Math.Min(ptMin.X, Math.Min(ptVect[0].X, ptVect[1].X));
                                    ptMin.Y = Math.Min(ptMin.Y, Math.Min(ptVect[0].Y, ptVect[1].Y));
                                    ptMax.X = Math.Max(ptMax.X, Math.Max(ptVect[0].X, ptVect[1].X));
                                    ptMax.Y = Math.Max(ptMax.Y, Math.Max(ptVect[0].Y, ptVect[1].Y));
                                }
                            }
                            else if (byteHead[1] < 127)
                            {
                                //vector
                                //fs.Seek(-2, SeekOrigin.Current);
                                iCurrent = iCurrent - 2;
                                byteData = new byte[2];
                                //fs.Read(byteData, 0, 2);
                                byteData[0] = byteSlide[iCurrent];
                                byteData[1] = byteSlide[iCurrent + 1];
                                iCurrent = iCurrent + 2;
                                ptVect[0].X = byteToInt(byteData);
                                //fs.Read(byteData, 0, 2);
                                byteData[0] = byteSlide[iCurrent];
                                byteData[1] = byteSlide[iCurrent + 1];
                                iCurrent = iCurrent + 2;
                                ptVect[0].Y = byteToInt(byteData);
                                //fs.Read(byteData, 0, 2);
                                byteData[0] = byteSlide[iCurrent];
                                byteData[1] = byteSlide[iCurrent + 1];
                                iCurrent = iCurrent + 2;
                                ptVect[1].X = byteToInt(byteData);
                                //fs.Read(byteData, 0, 2);
                                byteData[0] = byteSlide[iCurrent];
                                byteData[1] = byteSlide[iCurrent + 1];
                                iCurrent = iCurrent + 2;
                                ptVect[1].Y = byteToInt(byteData);

                                cItem = new SlideItem();
                                cItem.AddPoint(ptVect[0]);
                                cItem.AddPoint(ptVect[1]);
                                cItem.m_Type = 0;
                                cItem.m_Color = GetColorByIndex(nColor);
                                m_aySlideItem.Add(cItem);
                                if (bFirst)
                                {
                                    ptMin.X = Math.Min(ptVect[0].X, ptVect[1].X);
                                    ptMin.Y = Math.Min(ptVect[0].Y, ptVect[1].Y);
                                    ptMax.X = Math.Max(ptVect[0].X, ptVect[1].X);
                                    ptMax.Y = Math.Max(ptVect[0].Y, ptVect[1].Y);
                                    bFirst = false;
                                }
                                else
                                {
                                    ptMin.X = Math.Min(ptMin.X, Math.Min(ptVect[0].X, ptVect[1].X));
                                    ptMin.Y = Math.Min(ptMin.Y, Math.Min(ptVect[0].Y, ptVect[1].Y));
                                    ptMax.X = Math.Max(ptMax.X, Math.Max(ptVect[0].X, ptVect[1].X));
                                    ptMax.Y = Math.Max(ptMax.Y, Math.Max(ptVect[0].Y, ptVect[1].Y));
                                }
                            }
                        }
                    }
                    //fs.Close();

                    m_SlideWidth = Math.Abs(ptMax.X - ptMin.X);
                    m_SlideHeight = Math.Abs(ptMax.Y - ptMin.Y);
                    m_SlidePointMin = ptMin;
                    m_SlidePointMax = ptMax;
                    bRet = true;
                }
                else { /*fs.Close();*/ }
            }
            return bRet;
        }
        /// <summary>
        /// <para>从文件名读取幻灯片</para>
        /// </summary>
        /// <param name="sSlideFile"></param>
        /// <returns></returns>
        public bool ReadSlide(string sSlideFile)
        {
            byte[] byteHead, byteData;
            string sHead = "";
            int nColor, nHead;
            int nByteRead;
            SlideItem cItem;
            bool bFirst = true;
            Point ptMin, ptMax;
            bool bRet = false;

            m_aySlideItem = new List<SlideItem>();
            m_SlideWidth = 0;
            m_SlideHeight = 0;
            m_SlidePointMin = new Point();
            m_SlidePointMax = new Point();
            ptMin = new Point();
            ptMax = new Point();

            if (File.Exists(sSlideFile))
            {
                //m_byteData = File.ReadAllBytes(sSlideFile);
                Point[] ptVect;
                int delXY;
                ptVect = new Point[2];
                FileStream fs = File.OpenRead(sSlideFile);
                byteHead = new byte[15];
                nByteRead = fs.Read(byteHead, 0, 15);
                sHead = System.Text.Encoding.Default.GetString(byteHead);
                if (string.Compare(sHead, m_SlideHead, true) == 0)
                {
                    m_Position = SLD_HAEAR;
                    byteData = new byte[2];
                    fs.Seek(m_Position, SeekOrigin.Begin);
                    nByteRead = fs.Read(byteData, 0, 2);
                    nHead = byteData[0];
                    byteHead = new byte[2];
                    nByteRead = fs.Read(byteHead, 0, 2);
                    nColor = byteData[0];

                    fs.Seek(-2, SeekOrigin.Current);
                    if (byteData[0] == byteHead[0] && byteData[1] == byteHead[1])
                    {
                        while (true)
                        {
                            byteHead = new byte[2];
                            if (fs.Read(byteHead, 0, 2) != 2) { break; }
                            sHead = byteToHexString(byteHead, true);
                            if (byteHead[1] == 252)
                            {
                                break;
                            }
                            else if (byteHead[1] == 253)
                            {
                                //fill
                                byteData = new byte[2];
                                if (fs.Read(byteData, 0, 2) != 2) { break; }
                                fs.Seek(2, SeekOrigin.Current);
                                int i, iNums = byteToInt(byteData);
                                Point ptNode = new Point();

                                cItem = new SlideItem();
                                cItem.m_Type = 1;
                                cItem.m_Color = GetColorByIndex(nColor);
                                for (i = 0; i < iNums; i++)
                                {
                                    fs.Seek(2, SeekOrigin.Current);
                                    if (fs.Read(byteData, 0, 2) != 2) { break; }
                                    ptNode.X = byteToInt(byteData);
                                    if (fs.Read(byteData, 0, 2) != 2) { break; }
                                    ptNode.Y = byteToInt(byteData);
                                    //Add point
                                    if (i < iNums - 1)
                                    {
                                        cItem.AddPoint(ptNode);
                                        if (bFirst)
                                        {
                                            ptMin = ptNode;
                                            ptMax = ptNode;
                                            bFirst = false;
                                        }
                                        else
                                        {
                                            ptMin.X = Math.Min(ptMin.X, ptNode.X);
                                            ptMin.Y = Math.Min(ptMin.Y, ptNode.Y);
                                            ptMax.X = Math.Max(ptMax.X, ptNode.X);
                                            ptMax.Y = Math.Max(ptMax.Y, ptNode.Y);
                                        }
                                    }
                                }
                                m_aySlideItem.Add(cItem);
                            }
                            else if (byteHead[1] == 255)
                            {
                                //color
                                nColor = byteHead[0];
                            }
                            else if (byteHead[1] == 254)
                            {
                                //endpoint vector
                                ptVect[1] = ptVect[0];
                                delXY = byteHead[0];
                                ptVect[1].X += delXY;
                                byteData = new byte[1];
                                if (fs.Read(byteData, 0, 1) != 1) { break; }
                                delXY = byteData[0];
                                ptVect[1].Y += delXY;
                                //Add Point
                                cItem = new SlideItem();
                                cItem.AddPoint(ptVect[0]);
                                cItem.AddPoint(ptVect[1]);
                                cItem.m_Type = 0;
                                cItem.m_Color = GetColorByIndex(nColor);
                                m_aySlideItem.Add(cItem);
                                if (bFirst)
                                {
                                    ptMin.X = Math.Min(ptVect[0].X, ptVect[1].X);
                                    ptMin.Y = Math.Min(ptVect[0].Y, ptVect[1].Y);
                                    ptMax.X = Math.Max(ptVect[0].X, ptVect[1].X);
                                    ptMax.Y = Math.Max(ptVect[0].Y, ptVect[1].Y);
                                    bFirst = false;
                                }
                                else
                                {
                                    ptMin.X = Math.Min(ptMin.X, Math.Min(ptVect[0].X, ptVect[1].X));
                                    ptMin.Y = Math.Min(ptMin.Y, Math.Min(ptVect[0].Y, ptVect[1].Y));
                                    ptMax.X = Math.Max(ptMax.X, Math.Max(ptVect[0].X, ptVect[1].X));
                                    ptMax.Y = Math.Max(ptMax.Y, Math.Max(ptVect[0].Y, ptVect[1].Y));
                                }

                                ptVect[0] = ptVect[1];
                            }
                            else if (byteHead[1] == 251)
                            {
                                //Offset vector
                                ptVect[1] = ptVect[0];
                                //from
                                delXY = byteHead[0];
                                ptVect[0].X += delXY;
                                byteData = new byte[1];
                                if (fs.Read(byteData, 0, 1) != 1) { break; }
                                delXY = byteData[0];
                                ptVect[0].Y += delXY;
                                //to
                                if (fs.Read(byteData, 0, 1) != 1) { break; }
                                delXY = byteData[0];
                                ptVect[1].X += delXY;
                                byteData = new byte[1];
                                if (fs.Read(byteData, 0, 1) != 1) { break; }
                                delXY = byteData[0];
                                ptVect[1].Y += delXY;
                                //Add Point
                                cItem = new SlideItem();
                                cItem.AddPoint(ptVect[0]);
                                cItem.AddPoint(ptVect[1]);
                                cItem.m_Type = 0;
                                cItem.m_Color = GetColorByIndex(nColor);
                                m_aySlideItem.Add(cItem);
                                if (bFirst)
                                {
                                    ptMin.X = Math.Min(ptVect[0].X, ptVect[1].X);
                                    ptMin.Y = Math.Min(ptVect[0].Y, ptVect[1].Y);
                                    ptMax.X = Math.Max(ptVect[0].X, ptVect[1].X);
                                    ptMax.Y = Math.Max(ptVect[0].Y, ptVect[1].Y);
                                    bFirst = false;
                                }
                                else
                                {
                                    ptMin.X = Math.Min(ptMin.X, Math.Min(ptVect[0].X, ptVect[1].X));
                                    ptMin.Y = Math.Min(ptMin.Y, Math.Min(ptVect[0].Y, ptVect[1].Y));
                                    ptMax.X = Math.Max(ptMax.X, Math.Max(ptVect[0].X, ptVect[1].X));
                                    ptMax.Y = Math.Max(ptMax.Y, Math.Max(ptVect[0].Y, ptVect[1].Y));
                                }
                            }
                            else if (byteHead[1] < 127)
                            {
                                //vector
                                fs.Seek(-2, SeekOrigin.Current);
                                byteData = new byte[2];
                                fs.Read(byteData, 0, 2);
                                ptVect[0].X = byteToInt(byteData);
                                fs.Read(byteData, 0, 2);
                                ptVect[0].Y = byteToInt(byteData);
                                fs.Read(byteData, 0, 2);
                                ptVect[1].X = byteToInt(byteData);
                                fs.Read(byteData, 0, 2);
                                ptVect[1].Y = byteToInt(byteData);

                                cItem = new SlideItem();
                                cItem.AddPoint(ptVect[0]);
                                cItem.AddPoint(ptVect[1]);
                                cItem.m_Type = 0;
                                cItem.m_Color = GetColorByIndex(nColor);
                                m_aySlideItem.Add(cItem);
                                if (bFirst)
                                {
                                    ptMin.X = Math.Min(ptVect[0].X, ptVect[1].X);
                                    ptMin.Y = Math.Min(ptVect[0].Y, ptVect[1].Y);
                                    ptMax.X = Math.Max(ptVect[0].X, ptVect[1].X);
                                    ptMax.Y = Math.Max(ptVect[0].Y, ptVect[1].Y);
                                    bFirst = false;
                                }
                                else
                                {
                                    ptMin.X = Math.Min(ptMin.X, Math.Min(ptVect[0].X, ptVect[1].X));
                                    ptMin.Y = Math.Min(ptMin.Y, Math.Min(ptVect[0].Y, ptVect[1].Y));
                                    ptMax.X = Math.Max(ptMax.X, Math.Max(ptVect[0].X, ptVect[1].X));
                                    ptMax.Y = Math.Max(ptMax.Y, Math.Max(ptVect[0].Y, ptVect[1].Y));
                                }
                            }
                        }
                    }
                    fs.Close();

                    m_SlideWidth = Math.Abs(ptMax.X - ptMin.X);
                    m_SlideHeight = Math.Abs(ptMax.Y - ptMin.Y);
                    m_SlidePointMin = ptMin;
                    m_SlidePointMax = ptMax;
                    bRet = true;
                }
                else { fs.Close(); }
            }
            return bRet;
        }

        /// <summary>
        /// <para>重绘幻灯图像</para>
        /// </summary>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="g"></param>
        public void reDrawSlide(int nWidth, int nHeight, Graphics g)
        {
            int iOffsetX, iOffsetY;
            int iSldWidth, iSldHeight;

            g.Clear(m_Background);

            if (m_SlideHeight > 0 && m_SlideWidth > 0)
            {
                iSldWidth = m_SlideWidth;
                iSldHeight = m_SlideHeight;
                double dScaleX, dScaleY, dScale;

                dScaleX = (nWidth - m_Border * 2) * 1.0 / iSldWidth;
                dScaleY = (nHeight - m_Border * 2) * 1.0 / iSldHeight;
                if (dScaleX < dScaleY)
                {
                    dScale = dScaleX;
                    iOffsetX = m_Border;
                    iOffsetY = (int)((nHeight - iSldHeight * dScale) / 2);
                }
                else
                {
                    dScale = dScaleY;
                    iOffsetX = (int)((nWidth - iSldWidth * dScale) / 2);
                    iOffsetY = m_Border;
                }
                foreach (SlideItem cSubItem in m_aySlideItem)
                {
                    cSubItem.OffsetPoint(-m_SlidePointMin.X, -m_SlidePointMin.Y);
                    cSubItem.DrawItem(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                    cSubItem.OffsetPoint(m_SlidePointMin.X, m_SlidePointMin.Y);
                }
            }
        }
        /// <summary>
        /// <para>重绘幻灯图像,picControl.Image = (Image)bitmap;</para>
        /// <para>适合于图像大小发生变化时采用</para>
        /// <para>首先读取幻灯片</para>
        /// </summary>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="bitmap"></param>
        public void reDrawSlide(int nWidth, int nHeight, out Bitmap bitmap)
        {
            int iOffsetX, iOffsetY;
            int iSldWidth, iSldHeight;
            bitmap = null;

            bitmap = new Bitmap(nWidth, nHeight);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(m_Background);

            if (m_SlideHeight > 0 && m_SlideWidth > 0)
            {
                iSldWidth = m_SlideWidth;
                iSldHeight = m_SlideHeight;
                double dScaleX, dScaleY, dScale;

                dScaleX = (nWidth - m_Border * 2) * 1.0 / iSldWidth;
                dScaleY = (nHeight - m_Border * 2) * 1.0 / iSldHeight;
                if (dScaleX < dScaleY)
                {
                    dScale = dScaleX;
                    iOffsetX = m_Border;
                    iOffsetY = (int)((nHeight - iSldHeight * dScale) / 2);
                }
                else
                {
                    dScale = dScaleY;
                    iOffsetX = (int)((nWidth - iSldWidth * dScale) / 2);
                    iOffsetY = m_Border;
                }
                foreach (SlideItem cSubItem in m_aySlideItem)
                {
                    cSubItem.OffsetPoint(-m_SlidePointMin.X, -m_SlidePointMin.Y);
                    cSubItem.DrawItem(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                    cSubItem.OffsetPoint(m_SlidePointMin.X, m_SlidePointMin.Y);
                }
            }
        }
        /// <summary>
        /// <para>读取幻灯文件并生成图像,picControl.Image = (Image)bitmap;</para>
        /// <para>适合图像不发生大小变化时采用</para>
        /// </summary>
        /// <param name="sSlideFile"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public bool DrawSlide(string sSlideFile, int nWidth, int nHeight, out Bitmap bitmap)
        {
            byte[] byteHead, byteData;
            string sHead = "";
            int nColor, nHead;
            int nByteRead;
            SlideItem cItem;
            bool bFirst = true;
            Point ptMin, ptMax;
            int iOffsetX, iOffsetY;
            int iSldWidth, iSldHeight;
            double dScale;
            bool bRet = false;

            bitmap = null;

            if (File.Exists(sSlideFile))
            {
                //m_byteData = File.ReadAllBytes(sSlideFile);
                Point[] ptVect;
                int delXY;
                ptVect = new Point[2];
                FileStream fs = File.OpenRead(sSlideFile);
                byteHead = new byte[15];
                nByteRead = fs.Read(byteHead, 0, 15);
                sHead = System.Text.Encoding.Default.GetString(byteHead);
                if (string.Compare(sHead, m_SlideHead, true) == 0)
                {
                    bitmap = new Bitmap(nWidth, nHeight);
                    Graphics g = Graphics.FromImage(bitmap);
                    g.Clear(m_Background);

                    ptMin = new Point();
                    ptMax = new Point();
                    m_aySlideItem = new List<SlideItem>();

                    //fs.Seek(19, SeekOrigin.Begin);
                    //byteData = new byte[2];
                    //fs.Read(byteData, 0, 2);
                    //int iWidth = byteToInt(byteData);
                    //fs.Read(byteData, 0, 2);
                    //int iHeight = byteToInt(byteData);

                    m_Position = SLD_HAEAR;
                    byteData = new byte[2];
                    fs.Seek(m_Position, SeekOrigin.Begin);
                    nByteRead = fs.Read(byteData, 0, 2);
                    nHead = byteData[0];
                    byteHead = new byte[2];
                    nByteRead = fs.Read(byteHead, 0, 2);
                    nColor = byteData[0];

                    fs.Seek(-2, SeekOrigin.Current);
                    if (byteData[0] == byteHead[0] && byteData[1] == byteHead[1])
                    {
                        while (true)
                        {
                            byteHead = new byte[2];
                            if (fs.Read(byteHead, 0, 2) != 2) { break; }
                            sHead = byteToHexString(byteHead, true);
                            if (byteHead[1] == 252)
                            {
                                break;
                            }
                            else if (byteHead[1] == 253)
                            {
                                //fill
                                byteData = new byte[2];
                                if (fs.Read(byteData, 0, 2) != 2) { break; }
                                fs.Seek(2, SeekOrigin.Current);
                                int i, iNums = byteToInt(byteData);
                                Point ptNode = new Point();

                                cItem = new SlideItem();
                                cItem.m_Type = 1;
                                cItem.m_Color = GetColorByIndex(nColor);
                                for (i = 0; i < iNums; i++)
                                {
                                    fs.Seek(2, SeekOrigin.Current);
                                    if (fs.Read(byteData, 0, 2) != 2) { break; }
                                    ptNode.X = byteToInt(byteData);
                                    if (fs.Read(byteData, 0, 2) != 2) { break; }
                                    ptNode.Y = byteToInt(byteData);
                                    //Add point
                                    if (i < iNums - 1)
                                    {
                                        cItem.AddPoint(ptNode);
                                        if (bFirst)
                                        {
                                            ptMin = ptNode;
                                            ptMax = ptNode;
                                            bFirst = false;
                                        }
                                        else
                                        {
                                            ptMin.X = Math.Min(ptMin.X, ptNode.X);
                                            ptMin.Y = Math.Min(ptMin.Y, ptNode.Y);
                                            ptMax.X = Math.Max(ptMax.X, ptNode.X);
                                            ptMax.Y = Math.Max(ptMax.Y, ptNode.Y);
                                        }
                                    }
                                }
                                m_aySlideItem.Add(cItem);
                            }
                            else if (byteHead[1] == 255)
                            {
                                //color
                                nColor = byteHead[0];
                            }
                            else if (byteHead[1] == 254)
                            {
                                //endpoint vector
                                ptVect[1] = ptVect[0];
                                delXY = byteHead[0];
                                ptVect[1].X += delXY;
                                byteData = new byte[1];
                                if (fs.Read(byteData, 0, 1) != 1) { break; }
                                delXY = byteData[0];
                                ptVect[1].Y += delXY;
                                //Add Point
                                cItem = new SlideItem();
                                cItem.AddPoint(ptVect[0]);
                                cItem.AddPoint(ptVect[1]);
                                cItem.m_Type = 0;
                                cItem.m_Color = GetColorByIndex(nColor);
                                m_aySlideItem.Add(cItem);
                                if (bFirst)
                                {
                                    ptMin.X = Math.Min(ptVect[0].X, ptVect[1].X);
                                    ptMin.Y = Math.Min(ptVect[0].Y, ptVect[1].Y);
                                    ptMax.X = Math.Max(ptVect[0].X, ptVect[1].X);
                                    ptMax.Y = Math.Max(ptVect[0].Y, ptVect[1].Y);
                                    bFirst = false;
                                }
                                else
                                {
                                    ptMin.X = Math.Min(ptMin.X, Math.Min(ptVect[0].X, ptVect[1].X));
                                    ptMin.Y = Math.Min(ptMin.Y, Math.Min(ptVect[0].Y, ptVect[1].Y));
                                    ptMax.X = Math.Max(ptMax.X, Math.Max(ptVect[0].X, ptVect[1].X));
                                    ptMax.Y = Math.Max(ptMax.Y, Math.Max(ptVect[0].Y, ptVect[1].Y));
                                }

                                ptVect[0] = ptVect[1];
                            }
                            else if (byteHead[1] == 251)
                            {
                                //Offset vector
                                ptVect[1] = ptVect[0];
                                //from
                                delXY = byteHead[0];
                                ptVect[0].X += delXY;
                                byteData = new byte[1];
                                if (fs.Read(byteData, 0, 1) != 1) { break; }
                                delXY = byteData[0];
                                ptVect[0].Y += delXY;
                                //to
                                if (fs.Read(byteData, 0, 1) != 1) { break; }
                                delXY = byteData[0];
                                ptVect[1].X += delXY;
                                byteData = new byte[1];
                                if (fs.Read(byteData, 0, 1) != 1) { break; }
                                delXY = byteData[0];
                                ptVect[1].Y += delXY;
                                //Add Point
                                cItem = new SlideItem();
                                cItem.AddPoint(ptVect[0]);
                                cItem.AddPoint(ptVect[1]);
                                cItem.m_Type = 0;
                                cItem.m_Color = GetColorByIndex(nColor);
                                m_aySlideItem.Add(cItem);
                                if (bFirst)
                                {
                                    ptMin.X = Math.Min(ptVect[0].X, ptVect[1].X);
                                    ptMin.Y = Math.Min(ptVect[0].Y, ptVect[1].Y);
                                    ptMax.X = Math.Max(ptVect[0].X, ptVect[1].X);
                                    ptMax.Y = Math.Max(ptVect[0].Y, ptVect[1].Y);
                                    bFirst = false;
                                }
                                else
                                {
                                    ptMin.X = Math.Min(ptMin.X, Math.Min(ptVect[0].X, ptVect[1].X));
                                    ptMin.Y = Math.Min(ptMin.Y, Math.Min(ptVect[0].Y, ptVect[1].Y));
                                    ptMax.X = Math.Max(ptMax.X, Math.Max(ptVect[0].X, ptVect[1].X));
                                    ptMax.Y = Math.Max(ptMax.Y, Math.Max(ptVect[0].Y, ptVect[1].Y));
                                }
                            }
                            else if (byteHead[1] < 127)
                            {
                                //vector
                                fs.Seek(-2, SeekOrigin.Current);
                                byteData = new byte[2];
                                fs.Read(byteData, 0, 2);
                                ptVect[0].X = byteToInt(byteData);
                                fs.Read(byteData, 0, 2);
                                ptVect[0].Y = byteToInt(byteData);
                                fs.Read(byteData, 0, 2);
                                ptVect[1].X = byteToInt(byteData);
                                fs.Read(byteData, 0, 2);
                                ptVect[1].Y = byteToInt(byteData);

                                cItem = new SlideItem();
                                cItem.AddPoint(ptVect[0]);
                                cItem.AddPoint(ptVect[1]);
                                cItem.m_Type = 0;
                                cItem.m_Color = GetColorByIndex(nColor);
                                m_aySlideItem.Add(cItem);
                                if (bFirst)
                                {
                                    ptMin.X = Math.Min(ptVect[0].X, ptVect[1].X);
                                    ptMin.Y = Math.Min(ptVect[0].Y, ptVect[1].Y);
                                    ptMax.X = Math.Max(ptVect[0].X, ptVect[1].X);
                                    ptMax.Y = Math.Max(ptVect[0].Y, ptVect[1].Y);
                                    bFirst = false;
                                }
                                else
                                {
                                    ptMin.X = Math.Min(ptMin.X, Math.Min(ptVect[0].X, ptVect[1].X));
                                    ptMin.Y = Math.Min(ptMin.Y, Math.Min(ptVect[0].Y, ptVect[1].Y));
                                    ptMax.X = Math.Max(ptMax.X, Math.Max(ptVect[0].X, ptVect[1].X));
                                    ptMax.Y = Math.Max(ptMax.Y, Math.Max(ptVect[0].Y, ptVect[1].Y));
                                }
                            }
                        }
                    }
                    fs.Close();

                    bRet = true;

                    iSldWidth = Math.Abs(ptMax.X - ptMin.X);
                    iSldHeight = Math.Abs(ptMax.Y - ptMin.Y);
                    double dScaleX, dScaleY;

                    dScaleX = (nWidth - m_Border * 2) * 1.0 / iSldWidth;
                    dScaleY = (nHeight - m_Border * 2) * 1.0 / iSldHeight;
                    if (dScaleX < dScaleY)
                    {
                        dScale = dScaleX;
                        iOffsetX = m_Border;
                        iOffsetY = (int)((nHeight - iSldHeight * dScale) / 2);
                    }
                    else
                    {
                        dScale = dScaleY;
                        iOffsetX = (int)((nWidth - iSldWidth * dScale) / 2);
                        iOffsetY = m_Border;
                    }
                    foreach (SlideItem cSubItem in m_aySlideItem)
                    {
                        cSubItem.OffsetPoint(-ptMin.X, -ptMin.Y);
                        cSubItem.DrawItem(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                    }
                }
                else { fs.Close(); }
            }
            return bRet;
        }
        /// <summary>
        /// <para>生成幻灯图像流</para>
        /// </summary>
        /// <param name="sSlideFile"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <returns></returns>
        public System.IO.MemoryStream DrawSlide(string sSlideFile, int nWidth, int nHeight)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Bitmap bitmap;

            if (DrawSlide(sSlideFile, nWidth, nHeight, out bitmap))
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            }

            return ms;
        }
        public bool Slide2Gif(string sSlideFile, int nWidth, int nHeight, string sGifFile)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Bitmap bitmap;

            bool bRet = DrawSlide(sSlideFile, nWidth, nHeight, out bitmap);
            if (bRet)
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                File.WriteAllBytes(sGifFile, ms.ToArray());
            }
            return bRet;
        }
    }
}