using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;

namespace GeometryEx
{
    public class CGeoEntity_Data
    {
        public Color m_Color=Color.White;
        public float m_LineWeight=1;
        public DashStyle m_DashStyle = DashStyle.Solid;

        public void DrawLine(GE_Point ptStart,GE_Point ptEnd,Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            Pen myPen = new Pen(m_Color, m_LineWeight);
            myPen.DashStyle = m_DashStyle;
            Single x1, y1, x2, y2;
            x1 = (Single)(ptStart.X * dScale);
            y1 = (Single)(ptStart.Y * dScale);
            x2 = (Single)(ptEnd.X * dScale);
            y2 = (Single)(ptEnd.Y * dScale);
            y1 = nHeight - y1;
            y2 = nHeight - y2;
            x1 += iOffsetX;
            y1 -= iOffsetY;
            x2 += iOffsetX;
            y2 -= iOffsetY;
            g.DrawLine(myPen, x1, y1, x2, y2);
        }
        
        public void FillSolid(List<GE_Point> ayEdg,Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            Pen myPen = new Pen(m_Color, m_LineWeight);
            myPen.DashStyle = m_DashStyle;
            Single x1, y1, x2, y2;
            GE_Point pt1, pt2;

            int i;
            SolidBrush mySolidBrush = new SolidBrush(m_Color);
            GraphicsPath myPath = new GraphicsPath();

            for (i = 0; i < ayEdg.Count - 1; i++)
            {
                pt1 = ayEdg[i];
                pt2 = ayEdg[i + 1];
                x1 = (Single)(pt1.X * dScale);
                y1 = (Single)(pt1.Y * dScale);
                x2 = (Single)(pt2.X * dScale);
                y2 = (Single)(pt2.Y * dScale);
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
        public void FillSolid(GE_Point pt1, GE_Point pt2, GE_Point pt3, GE_Point pt4, Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            List<GE_Point> ayEdg = new List<GE_Point>();
            ayEdg.Add(pt1);
            ayEdg.Add(pt2);
            ayEdg.Add(pt3);
            ayEdg.Add(pt4);
            FillSolid(ayEdg, g, iOffsetX, iOffsetY,dScale, nWidth, nHeight);
        }
        public void FillSolid(GE_Point pt1, GE_Point pt2, GE_Point pt3, Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            List<GE_Point> ayEdg = new List<GE_Point>();
            ayEdg.Add(pt1);
            ayEdg.Add(pt2);
            ayEdg.Add(pt3);
            FillSolid(ayEdg, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
        }
        public void DrawArrow(GE_Point ptIns, GE_Vector v, double dSize, Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight, int nArrorType = 0)
        {
            GE_Point pt1, pt2;
            GE_Vector vV = new GE_Vector(v);

            if (nArrorType == 0)
            {
                vV.RotateBy(Geo.PI2);
                pt1 = ptIns + v * dSize;
                pt1 = pt1 + vV * (dSize * 0.15);
                pt2 = pt1 - vV * (dSize * 0.3);
                FillSolid(ptIns, pt1, pt2, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }
        }
        public void DrawArc(GE_Point ptCenter, double dRadiu, double dStartAngle, double dEndAngle, Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight, int nArrorType = 0)
        {
            Pen myPen = new Pen(m_Color, m_LineWeight);
            myPen.DashStyle = m_DashStyle;
            Single x1, y1, x2, y2;
            GE_Point pt1, pt2;

            double dSweep = Geo.Mid_angle(dStartAngle, dEndAngle);
            List<GE_Point> ayPoints = Geo.ExplodeArc(ptCenter, dRadiu, dStartAngle, dEndAngle, dSweep * dRadiu / (Math.PI * 2));
            int i;

            for (i = 0; i < ayPoints.Count - 1; i++)
            {
                pt1 = ayPoints[i];
                pt2 = ayPoints[i + 1];
                x1 = (Single)(pt1.X * dScale);
                y1 = (Single)(pt1.Y * dScale);
                x2 = (Single)(pt2.X * dScale);
                y2 = (Single)(pt2.Y * dScale);
                y1 = nHeight - y1;
                y2 = nHeight - y2;
                x1 += iOffsetX;
                y1 -= iOffsetY;
                x2 += iOffsetX;
                y2 -= iOffsetY;
                g.DrawLine(myPen, x1, y1, x2, y2);
            }
        }
    }
    public class CGeoEntity_DataLine : CGeoEntity_Data
    {
        public GE_Point m_start;
        public GE_Point m_end;

        public bool getExtent(out GE_Point ptMin,out GE_Point ptMax)
        {
            ptMin = new GE_Point(Math.Min(m_start.X, m_end.X), Math.Min(m_start.Y, m_end.Y));
            ptMax = new GE_Point(Math.Max(m_start.X, m_end.X), Math.Max(m_start.Y, m_end.Y));
            return true;
        }
        public void DrawImage(Graphics g, int iOffsetX,int iOffsetY,double dScale,int nWidth,int nHeight)
        {
            DrawLine(m_start, m_end, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
        }
    }
    public class CGeoEntity_DataText : CGeoEntity_Data
    {
        //文字高度按实际输入字高绘
        //文字尺寸不计入图形尺寸计算
        public Color m_TextColor = Color.Yellow;
        public GE_Point m_ptText;
        public string m_Text;
        public string m_FontName = "SimSun";
        public double m_TxtHeight = 10;
        public double m_Rotate = 0;     //文字角度（角度制）
        public int m_Frame = 0;         //边框
        public System.Drawing.Color m_BackgroundColor=System.Drawing.Color.Black;
        public ContentAlignment m_Align = ContentAlignment.MiddleCenter;

        private void GetTextBorder(out GE_Point[] ptBor)
        {
            Font font = new Font(m_FontName, (Single)(m_TxtHeight));
            SizeF sim = System.Windows.Forms.TextRenderer.MeasureText(m_Text, font);
            GE_Vector vH = Geo.GetVectorByAngle(Geo.Dtr(m_Rotate));
            GE_Vector vV = Geo.GetVectorByAngle(Geo.Dtr(m_Rotate + 90));
            GE_Point ptBase = new GE_Point();
            switch (m_Align)
            {
                case ContentAlignment.TopLeft:
                    ptBase = m_ptText - vV * sim.Height;
                    break;
                case ContentAlignment.TopCenter:
                    ptBase = m_ptText - vH * (sim.Width / 2);
                    ptBase = ptBase - vV * sim.Height;
                    break;
                case ContentAlignment.TopRight:
                    ptBase = m_ptText - vH * sim.Width;
                    ptBase = ptBase - vV * sim.Height;
                    break;
                case ContentAlignment.MiddleLeft:
                    ptBase = m_ptText - vV * (sim.Height / 2);
                    break;
                case ContentAlignment.MiddleCenter:
                    ptBase = m_ptText - vH * (sim.Width / 2);
                    ptBase = ptBase - (vV * sim.Height / 2);
                    break;
                case ContentAlignment.MiddleRight:
                    ptBase = m_ptText - vH * sim.Width;
                    ptBase = ptBase - (vV * sim.Height / 2);
                    break;
                case ContentAlignment.BottomLeft:
                    ptBase = m_ptText;
                    break;
                case ContentAlignment.BottomCenter:
                    ptBase = m_ptText - vH * (sim.Width / 2);
                    break;
                case ContentAlignment.BottomRight:
                    ptBase = m_ptText - vH * sim.Width;
                    break;
            }
            ptBor = new GE_Point[4];
            ptBor[0] = new GE_Point(ptBase.X,ptBase.Y);
            ptBor[1] = ptBor[0] + vH * sim.Width;
            ptBor[2] = ptBor[1] + vV * sim.Height;
            ptBor[3] = ptBor[2] - vH * sim.Width;
        }
        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            ptMin = new GE_Point(m_ptText.X, m_ptText.Y);
            ptMax = new GE_Point(m_ptText.X, m_ptText.Y);

            GE_Point[] ptBor;
            GetTextBorder(out ptBor);
            ptMin.X = Math.Min(Math.Min(ptBor[0].X, ptBor[1].X), Math.Min(ptBor[2].X, ptBor[3].X));
            ptMin.Y = Math.Min(Math.Min(ptBor[0].Y, ptBor[1].Y), Math.Min(ptBor[2].Y, ptBor[3].Y));
            ptMax.X = Math.Max(Math.Max(ptBor[0].X, ptBor[1].X), Math.Max(ptBor[2].X, ptBor[3].X));
            ptMax.Y = Math.Max(Math.Max(ptBor[0].Y, ptBor[1].Y), Math.Max(ptBor[2].Y, ptBor[3].Y));
            
            return true;
        }
        public void DrawTextBorder(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            GE_Point[] ptBor;
            GetTextBorder(out ptBor);
            int i;
            Pen myPen;
            Single x1, y1, x2 = 0, y2 = 0;

            if (m_Frame == 1)
            {
                myPen = new Pen(m_Color, m_LineWeight);
                myPen.DashStyle = m_DashStyle;
                for (i = 0; i < 4; i++)
                {
                    x1 = (Single)(ptBor[i].X * dScale);
                    y1 = (Single)(ptBor[i].Y * dScale);
                    if (i == 3)
                    {
                        x2 = (Single)(ptBor[0].X * dScale);
                        y2 = (Single)(ptBor[0].Y * dScale);
                    }
                    else
                    {
                        x2 = (Single)(ptBor[i + 1].X * dScale);
                        y2 = (Single)(ptBor[i + 1].Y * dScale);
                    }
                    y1 = nHeight - y1;
                    y2 = nHeight - y2;
                    x1 += iOffsetX;
                    y1 -= iOffsetY;
                    x2 += iOffsetX;
                    y2 -= iOffsetY;
                    g.DrawLine(myPen, x1, y1, x2, y2);
                }
            }
            else if (m_Frame == 2)
            {
                GE_Point pt1=Geo.AcGeMidPoint(ptBor[0],ptBor[3]);
                GE_Point pt2=Geo.AcGeMidPoint(ptBor[1],ptBor[2]);
                double dWid=ptBor[0].distanceTo(ptBor[3])*dScale;

                myPen = new Pen(m_BackgroundColor, (float)dWid);
                x1 = (Single)(pt1.X * dScale);
                y1 = (Single)(pt1.Y * dScale);
                x2 = (Single)(pt2.X * dScale);
                y2 = (Single)(pt2.Y * dScale);
                y1 = nHeight - y1;
                y2 = nHeight - y2;
                x1 += iOffsetX;
                y1 -= iOffsetY;
                x2 += iOffsetX;
                y2 -= iOffsetY;
                g.DrawLine(myPen, x1, y1, x2, y2);
            }
        }
        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            if (m_Frame != 0)
            {
                DrawTextBorder(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }

            Font font = new Font(m_FontName, (Single)(m_TxtHeight));
            SizeF sim = System.Windows.Forms.TextRenderer.MeasureText(m_Text, font);
            GE_Vector vH = Geo.GetVectorByAngle(Geo.Dtr(m_Rotate));
            GE_Vector vV = Geo.GetVectorByAngle(Geo.Dtr(m_Rotate + 90));

            GE_Point ptCenter = new GE_Point(m_ptText.X, m_ptText.Y);
            switch (m_Align)
            {
                case ContentAlignment.TopLeft:
                    ptCenter = m_ptText + vH * (sim.Width / 2);
                    ptCenter = ptCenter - vV * (sim.Height / 2);
                    break;
                case ContentAlignment.TopCenter:
                    ptCenter = m_ptText - vV * (sim.Height / 2);
                    break;
                case ContentAlignment.TopRight:
                    ptCenter = m_ptText - vH * (sim.Width / 2);
                    ptCenter = ptCenter - vV * (sim.Height / 2);
                    break;
                case ContentAlignment.MiddleLeft:
                    ptCenter = m_ptText + vH * (sim.Width / 2);
                    break;
                case ContentAlignment.MiddleCenter:
                    ptCenter = m_ptText;
                    break;
                case ContentAlignment.MiddleRight:
                    ptCenter = m_ptText - vH * (sim.Width / 2);
                    break;
                case ContentAlignment.BottomLeft:
                    ptCenter = m_ptText + vH * (sim.Width / 2);
                    ptCenter = ptCenter + vV * (sim.Height / 2);
                    break;
                case ContentAlignment.BottomCenter:
                    ptCenter = m_ptText + vV * (sim.Height / 2);
                    break;
                case ContentAlignment.BottomRight:
                    ptCenter = m_ptText - vH * (sim.Width / 2);
                    ptCenter = ptCenter + vV * (sim.Height / 2);
                    break;
            }



            //设置字体           
            double dFontHeight = m_TxtHeight * dScale;
            if (m_FontName.Length > 0)
            {
                font = new Font(m_FontName, Convert.ToInt32(dFontHeight));
            }
            else
            {
                font = new Font("黑体", Convert.ToInt32(dFontHeight));
            }
            //设置颜色
            SolidBrush brush = new SolidBrush(m_TextColor);
            //SolidBrush brush = new SolidBrush(m_Color);

            //设置字体居中对齐
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            float dRotate = 0;
            float x1, y1;
            x1 = (float)(ptCenter.X * dScale);
            y1 = (float)(ptCenter.Y * dScale);
            y1 = nHeight - y1;
            x1 += iOffsetX;
            y1 -= iOffsetY;

            if (Math.Abs(m_Rotate - 0) < 0.1)
            {
                //水平文字
                g.DrawString(m_Text, font, brush, x1, y1, sf);
            }
            else if (Math.Abs(m_Rotate - 90) < 0.1)
            {
                //垂直文字
                dRotate = -90;// (float)(Math.PI / 2);
                g.TranslateTransform(x1, y1);
                g.RotateTransform(dRotate);
                g.DrawString(m_Text, font, brush, 0, 0, sf);
                g.RotateTransform(-dRotate);
                g.TranslateTransform(-x1, -y1);
            }
            else
            {
                //带角度文字
                dRotate = (float)m_Rotate * -1;
                g.TranslateTransform(x1, y1);
                g.RotateTransform(dRotate);
                g.DrawString(m_Text, font, brush, 0, 0, sf);
                g.RotateTransform(-dRotate);
                g.TranslateTransform(-x1, -y1);
            }
            
        }
    }
    public class CGeoEntity_DataBox : CGeoEntity_Data
    {
        public double m_Width=0;
        public double m_Height=0;
        public double m_Rotate=0;
        public bool m_Fill = false;
        public Color m_FillColor;
        public GE_Point m_BasePoint=new GE_Point(0,0);
        public ContentAlignment m_Align=ContentAlignment.MiddleCenter;
        private GE_Point[] m_ptNode = new GE_Point[4];

        public CGeoEntity_DataBox()
        {
            m_FillColor = m_Color;
        }
        public void Create()
        {
            GE_Vector vX = new GE_Vector(GE_Vector.XAxis);
            GE_Vector vY = new GE_Vector(GE_Vector.YAxis);
            vX.RotateBy(m_Rotate);
            vY.RotateBy(m_Rotate);
            switch (m_Align)
            {
                case ContentAlignment.BottomCenter:
                    m_ptNode[0] = m_BasePoint - vX * (m_Width / 2);
            	    break;
                case ContentAlignment.BottomLeft:
                    m_ptNode[0] = m_BasePoint;
                    break;
                case ContentAlignment.BottomRight:
                    m_ptNode[0] = m_BasePoint - vX * m_Width;
                    break;
                case ContentAlignment.MiddleCenter:
                    m_ptNode[0] = m_BasePoint - vX * (m_Width / 2);
                    m_ptNode[0] = m_ptNode[0] - vY * (m_Height / 2);
                    break;
                case ContentAlignment.MiddleLeft:
                    m_ptNode[0] = m_BasePoint - vY * (m_Height / 2);
                    break;
                case ContentAlignment.MiddleRight:
                    m_ptNode[0] = m_BasePoint - vX * m_Width;
                    m_ptNode[0] = m_ptNode[0] - vY * (m_Height / 2);
                    break;
                case ContentAlignment.TopCenter:
                    m_ptNode[0] = m_BasePoint - vX * (m_Width / 2);
                    m_ptNode[0] = m_ptNode[0] - vY * m_Height;
                    break;
                case ContentAlignment.TopLeft:
                    m_ptNode[0] = m_BasePoint - vY * m_Height;
                    break;
                case ContentAlignment.TopRight:
                    m_ptNode[0] = m_BasePoint - vX * m_Width;
                    m_ptNode[0] = m_ptNode[0] - vY * m_Height;
                    break;
                default:
                    m_ptNode[0] = m_BasePoint - vX * (m_Width / 2);
                    m_ptNode[0] = m_ptNode[0] - vY * (m_Height / 2);
                    break;
            }
            m_ptNode[1] = m_ptNode[0] + vX * m_Width;
            m_ptNode[2] = m_ptNode[1] + vY * m_Height;
            m_ptNode[3] = m_ptNode[2] - vX * m_Width;
        }
        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            ptMin =new GE_Point(m_ptNode[0]);
            ptMax =new GE_Point(m_ptNode[0]);
            for (int i = 1; i < 4; i++)
            {
                ptMin.X = Math.Min(ptMin.X, m_ptNode[i].X);
                ptMin.Y = Math.Min(ptMin.Y, m_ptNode[i].Y);
                ptMax.X = Math.Max(ptMax.X, m_ptNode[i].X);
                ptMax.Y = Math.Max(ptMax.Y, m_ptNode[i].Y);
            }
            return true;
        }
        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            Pen myPen = new Pen(m_Color, m_LineWeight);
            myPen.DashStyle = m_DashStyle;
            Single x1, y1, x2, y2;
            int i;
            GE_Point[] pt = new GE_Point[2];

            if (!m_Fill || (m_FillColor!=m_Color && m_Fill))
            {
                for (i = 0; i < 4; i++)
                {
                    if (i < 3)
                    {
                        pt[0] = m_ptNode[i];
                        pt[1] = m_ptNode[i + 1];
                    }
                    else
                    {
                        pt[0] = m_ptNode[i];
                        pt[1] = m_ptNode[0];
                    }
                    x1 = (Single)(pt[0].X * dScale);
                    y1 = (Single)(pt[0].Y * dScale);
                    x2 = (Single)(pt[1].X * dScale);
                    y2 = (Single)(pt[1].Y * dScale);
                    y1 = nHeight - y1;
                    y2 = nHeight - y2;
                    x1 += iOffsetX;
                    y1 -= iOffsetY;
                    x2 += iOffsetX;
                    y2 -= iOffsetY;
                    g.DrawLine(myPen, x1, y1, x2, y2);
                }
            }
            
            if (m_Fill) {
                SolidBrush mySolidBrush = new SolidBrush(m_FillColor);
                GraphicsPath myPath = new GraphicsPath();
                for (i = 0; i < 4; i++)
                {
                    if (i < 3)
                    {
                        pt[0] = m_ptNode[i];
                        pt[1] = m_ptNode[i + 1];
                    }
                    else
                    {
                        pt[0] = m_ptNode[i];
                        pt[1] = m_ptNode[0];
                    }
                    x1 = (Single)(pt[0].X * dScale);
                    y1 = (Single)(pt[0].Y * dScale);
                    x2 = (Single)(pt[1].X * dScale);
                    y2 = (Single)(pt[1].Y * dScale);
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
    public class CGeoEntity_DataArc : CGeoEntity_Data
    {
        public GE_Point m_center;
        public float m_radiu;
        public float m_startAngle;
        public float m_endAngle;

        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            GE_Point pt;
            double dSweep = Geo.Mid_angle(m_startAngle, m_endAngle);
            List<GE_Point> ayPoints = Geo.ExplodeArc(m_center, m_radiu, m_startAngle, m_endAngle, dSweep * m_radiu / (Math.PI * 2));
            int i;
            pt = ayPoints[0];
            ptMin = new GE_Point(pt.X, pt.Y);
            ptMax = new GE_Point(pt.X, pt.Y);
            for (i=1;i<ayPoints.Count;i++) {
                pt = ayPoints[i];
                ptMin.X = Math.Min(ptMin.X, pt.X);
                ptMin.Y = Math.Min(ptMin.Y, pt.Y);
                ptMax.X = Math.Max(ptMax.X, pt.X);
                ptMax.Y = Math.Max(ptMax.Y, pt.Y);
            }
            return true;
        }
        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            DrawArc(m_center,m_radiu,m_startAngle,m_endAngle,g,iOffsetX,iOffsetY,dScale,nWidth,nHeight);
        }
    }
    public class CGeoEntity_DataCircle : CGeoEntity_Data
    {
        public GE_Point m_center;
        public float m_radiu;
        public bool m_fill = false;

        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            ptMin = new GE_Point(m_center.X - m_radiu, m_center.Y - m_radiu);
            ptMax = new GE_Point(m_center.X + m_radiu, m_center.Y + m_radiu);
            return true;
        }
        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            Pen myPen = new Pen(m_Color, m_LineWeight);
            myPen.DashStyle = m_DashStyle;
            Single x1, y1, x2, y2;
            GE_Point pt1, pt2;

            List<GE_Point> ayPoints = Geo.ExplodeArc(m_center, m_radiu, 0, Math.PI * 2, m_radiu * Math.PI / 180.0);
            int i;
            SolidBrush mySolidBrush = new SolidBrush(m_Color);
            GraphicsPath myPath = new GraphicsPath();

            for (i = 0; i < ayPoints.Count - 1; i++)
            {
                pt1 = ayPoints[i];
                pt2 = ayPoints[i + 1];
                x1 = (Single)(pt1.X * dScale);
                y1 = (Single)(pt1.Y * dScale);
                x2 = (Single)(pt2.X * dScale);
                y2 = (Single)(pt2.Y * dScale);
                y1 = nHeight - y1;
                y2 = nHeight - y2;
                x1 += iOffsetX;
                y1 -= iOffsetY;
                x2 += iOffsetX;
                y2 -= iOffsetY;
                if (!m_fill)
                {
                    g.DrawLine(myPen, x1, y1, x2, y2);
                }
                else {
                    myPath.AddLine(x1, y1, x2, y2);
                }
            }
            if (m_fill) {
                g.FillPath(mySolidBrush, myPath);
            }
        }
    }
    public class CGeoEntity_DataSolid : CGeoEntity_Data
    {
        public List<GE_Point> m_edg;
        public CGeoEntity_DataSolid()
        {
            m_edg = new List<GE_Point>();
        }
        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            bool bRet = false;
            ptMin = new GE_Point();
            ptMax = new GE_Point();
            if (m_edg!=null) {
                int i;
                GE_Point pt;
                ptMin = new GE_Point(m_edg[0].X,m_edg[0].Y);
                ptMax = new GE_Point(m_edg[0].X,m_edg[0].Y);
                for (i=1;i<m_edg.Count;i++) {
                    pt = m_edg[i];
                    ptMin.X = Math.Min(ptMin.X, pt.X);
                    ptMin.Y = Math.Min(ptMin.Y, pt.Y);
                    ptMax.X = Math.Max(ptMax.X, pt.X);
                    ptMax.Y = Math.Max(ptMax.Y, pt.Y);
                }
                bRet = true;
            }
            return bRet;
        }
        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            
            FillSolid(m_edg, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
        }
    }
    public class CGeoEntity_DataPolyline : CGeoEntity_Data
    {
        public List<GE_Point> m_edg;
        public List<double> m_bulge;
        public bool m_closed = false;
        public bool m_fill = false;

        public CGeoEntity_DataPolyline()
        {
            m_edg = new List<GE_Point>();
            m_bulge = new List<double>();
        }
        public void addPoint(GE_Point pt,double dBulge)
        {
            m_edg.Add(pt);
            m_bulge.Add(dBulge);
        }
        

    }


    public class CGeoEntity_DataDimension_Aligned : CGeoEntity_DataText
    {
        public GE_Point m_ptStart;
        public GE_Point m_ptEnd;
        public GE_Point m_ptPosition;
        public string m_DimText;

        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            bool bRet = true;
            List<GE_Point> ayPts = new List<GE_Point>();

            GE_Line linPos, linDimStart, linDimEnd;
            GE_Point ptInsStart, ptInsEnd, ptInsStart1, ptInsEnd1, pt1, pt2;
            GE_Vector vDim, vPos;

            vPos = Geo.GetVector(m_ptStart, m_ptEnd);
            linPos = new GE_Line();
            linPos.set(m_ptPosition, m_ptPosition + vPos * 10);
            vDim = new GE_Vector(vPos);
            vDim.RotateBy(Geo.PI2);

            linDimStart = new GE_Line();
            linDimStart.set(m_ptStart, m_ptStart + vDim * 10);
            linDimEnd = new GE_Line();
            linDimEnd.set(m_ptEnd, m_ptEnd + vDim * 10);
            linPos.intersectWith(linDimStart, out ptInsStart, out pt1);
            linPos.intersectWith(linDimEnd, out ptInsEnd, out pt1);

            vDim = Geo.GetVector(m_ptStart, ptInsStart);
            ptInsStart1 = ptInsStart + vDim * (m_TxtHeight / 2);
            ptInsEnd1 = ptInsEnd + vDim * (m_TxtHeight / 2);

            base.m_ptText = Geo.AcGeMidPoint(ptInsStart, ptInsEnd);
            base.m_Text = m_DimText;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = Geo.Rtd(vPos.GetAngle());

            ayPts.Add(m_ptStart);
            ayPts.Add(ptInsStart1);
            ayPts.Add(m_ptEnd);
            ayPts.Add(ptInsEnd1);
            ayPts.Add(ptInsStart);
            ayPts.Add(ptInsEnd);
            GE_Point[] ptBor = new GE_Point[2];
            base.getExtent(out ptBor[0], out ptBor[1]);
            ayPts.Add(ptBor[0]);
            ayPts.Add(ptBor[1]);

            bRet = Geo.GetExtents(ayPts, out ptMin, out ptMax);
            return bRet;
        }
        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            GE_Line linPos, linDimStart, linDimEnd;
            GE_Point ptInsStart, ptInsEnd, ptInsStart1, ptInsEnd1, pt1, pt2;
            GE_Vector vDim, vPos;

            vPos = Geo.GetVector(m_ptStart, m_ptEnd);
            linPos = new GE_Line();
            linPos.set(m_ptPosition, m_ptPosition + vPos * 10);
            vDim = new GE_Vector(vPos);
            vDim.RotateBy(Geo.PI2);

            linDimStart = new GE_Line();
            linDimStart.set(m_ptStart, m_ptStart + vDim * 10);
            linDimEnd = new GE_Line();
            linDimEnd.set(m_ptEnd, m_ptEnd + vDim * 10);
            linPos.intersectWith(linDimStart, out ptInsStart, out pt1);
            linPos.intersectWith(linDimEnd, out ptInsEnd, out pt1);

            vDim = Geo.GetVector(m_ptStart, ptInsStart);
            ptInsStart1 = ptInsStart + vDim * (m_TxtHeight / 2);
            ptInsEnd1 = ptInsEnd + vDim * (m_TxtHeight / 2);

            base.m_ptText = Geo.AcGeMidPoint(ptInsStart, ptInsEnd);
            base.m_Text = m_DimText;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = Geo.Rtd(vPos.GetAngle());

            //绘尺寸线
            if (m_ptStart.distanceTo(ptInsStart) > 1)
            {
                DrawLine(m_ptStart, ptInsStart1, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                DrawLine(m_ptEnd, ptInsEnd1, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }
            if (ptInsStart.distanceTo(ptInsEnd) > m_TxtHeight * 3)
            {
                DrawLine(ptInsStart, ptInsEnd, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                //绘箭头
                DrawArrow(ptInsStart, vPos, m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                DrawArrow(ptInsEnd, -vPos, m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }
            else
            {
                DrawLine(ptInsStart-vPos*(m_TxtHeight*1.5), ptInsEnd+vPos*(m_TxtHeight*1.5), g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                //绘箭头
                DrawArrow(ptInsStart, -vPos, m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                DrawArrow(ptInsEnd, vPos, m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }
            //绘文字
            base.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
        }
    }
    public class CGeoEntity_DataDimension_Rotated : CGeoEntity_DataText
    {
        public GE_Point m_ptStart;
        public GE_Point m_ptEnd;
        public GE_Point m_ptPosition;
        public double m_DimRotate;    //0/90;
        public string m_DimText;

        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            bool bRet = true;
            List<GE_Point> ayPts = new List<GE_Point>();

            GE_Line linPos, linDimStart, linDimEnd;
            GE_Point ptInsStart, ptInsEnd, ptInsStart1, ptInsEnd1, pt1, pt2;
            GE_Vector vDim, vPos;

            vPos = Geo.GetVectorByAngle(Geo.Dtr(m_DimRotate));
            linPos = new GE_Line();
            linPos.set(m_ptPosition, m_ptPosition + vPos * 10);
            vDim = Geo.GetVectorByAngle(Geo.Dtr(m_DimRotate + 90));
            linDimStart = new GE_Line();
            linDimStart.set(m_ptStart, m_ptStart + vDim * 10);
            linDimEnd = new GE_Line();
            linDimEnd.set(m_ptEnd, m_ptEnd + vDim * 10);
            linPos.intersectWith(linDimStart, out ptInsStart, out pt1);
            linPos.intersectWith(linDimEnd, out ptInsEnd, out pt1);

            vDim = Geo.GetVector(m_ptStart, ptInsStart);
            ptInsStart1 = ptInsStart + vDim * (m_TxtHeight / 2);
            ptInsEnd1 = ptInsEnd + vDim * (m_TxtHeight / 2);

            base.m_ptText = Geo.AcGeMidPoint(ptInsStart, ptInsEnd);
            base.m_Text = m_DimText;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = m_DimRotate;

            ayPts.Add(m_ptStart);
            ayPts.Add(ptInsStart1);
            ayPts.Add(m_ptEnd);
            ayPts.Add(ptInsEnd1);
            ayPts.Add(ptInsStart);
            ayPts.Add(ptInsEnd);
            GE_Point[] ptBor = new GE_Point[2];
            base.getExtent(out ptBor[0], out ptBor[1]);
            ayPts.Add(ptBor[0]);
            ayPts.Add(ptBor[1]);

            bRet = Geo.GetExtents(ayPts, out ptMin, out ptMax);
            return bRet;
        }

        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            GE_Line linPos, linDimStart, linDimEnd;
            GE_Point ptInsStart, ptInsEnd, ptInsStart1, ptInsEnd1,pt1,pt2;
            GE_Vector vDim, vPos;

            vPos = Geo.GetVectorByAngle(Geo.Dtr(m_DimRotate));
            linPos = new GE_Line();
            linPos.set(m_ptPosition, m_ptPosition + vPos * 10);
            vDim = Geo.GetVectorByAngle(Geo.Dtr(m_DimRotate + 90));
            linDimStart = new GE_Line();
            linDimStart.set(m_ptStart, m_ptStart + vDim * 10);
            linDimEnd = new GE_Line();
            linDimEnd.set(m_ptEnd, m_ptEnd + vDim * 10);
            linPos.intersectWith(linDimStart, out ptInsStart, out pt1);
            linPos.intersectWith(linDimEnd, out ptInsEnd, out pt1);

            vDim = Geo.GetVector(m_ptStart, ptInsStart);
            ptInsStart1 = ptInsStart + vDim * (m_TxtHeight / 2);
            ptInsEnd1 = ptInsEnd + vDim * (m_TxtHeight / 2);

            base.m_ptText = Geo.AcGeMidPoint(ptInsStart, ptInsEnd);
            base.m_Text = m_DimText;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = m_DimRotate;

            //绘尺寸线
            if (ptInsStart.distanceTo(m_ptStart) > 1)
            {
                DrawLine(m_ptStart, ptInsStart1, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                DrawLine(m_ptEnd, ptInsEnd1, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }
            if (ptInsStart.distanceTo(ptInsEnd) > m_TxtHeight * 3)
            {
                DrawLine(ptInsStart, ptInsEnd, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                //绘箭头
                DrawArrow(ptInsStart, vPos, m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                DrawArrow(ptInsEnd, -vPos, m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }
            else
            {
                DrawLine(ptInsStart - vPos * (m_TxtHeight * 1.5), ptInsEnd + vPos * (m_TxtHeight * 1.5), g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                //绘箭头
                DrawArrow(ptInsStart, -vPos, m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                DrawArrow(ptInsEnd, vPos, m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }
            //绘文字
            base.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
        }
    }
    //双端近似弧线标注
    public class CGeoEntity_DataDimension_SimilarRadiuAngle : CGeoEntity_DataText
    {
        public GE_Point m_ptCenter;
        public GE_Point m_ptRadiu;
        public GE_Point m_ptArcStart;
        public GE_Point m_ptArcEnd;
        public string m_DimText;
        public List<GE_Point> m_lstPt;

        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            bool bRet = true;
            List<GE_Point> ayPts = new List<GE_Point>();
            double dTxtAngle;

            ayPts.Add(m_ptCenter);
            ayPts.Add(m_ptRadiu);
            ayPts.Add(m_ptArcStart * 1.2);
            ayPts.Add(m_ptArcEnd * 1.2);
            if (m_ptRadiu.X >= m_ptCenter.X)
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptCenter, m_ptRadiu);
            }
            else
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptRadiu, m_ptCenter);
            }
            base.m_ptText = Geo.AcGeMidPoint(m_ptCenter, m_ptRadiu);
            base.m_Text = m_DimText;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = Geo.Rtd(dTxtAngle);
            GE_Point[] ptBor = new GE_Point[2];
            base.getExtent(out ptBor[0], out ptBor[1]);
            ayPts.Add(ptBor[0]);
            ayPts.Add(ptBor[1]);

            bRet = Geo.GetExtents(ayPts, out ptMin, out ptMax);
            return bRet;
        }
        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            GE_Vector vPos = Geo.GetVector(m_ptCenter, m_ptArcStart);
            if ((m_ptArcEnd - m_ptCenter).GetAngle() >= 3 * Geo.PI2)
                DrawArrow(m_ptArcStart * 1.2, vPos.RotateBy(-Geo.PI2 + Math.Atan(0.15)), m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            else
                DrawArrow(m_ptArcStart * 1.2, vPos.RotateBy(Geo.PI2 /*- Math.Atan(0.15)*/), m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);

            vPos = Geo.GetVector(m_ptCenter, m_ptArcEnd);
            DrawArrow(m_ptArcEnd * 1.2, vPos.RotateBy(-Geo.PI2 /*+ Math.Atan(0.15)*/), m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);

            double dTxtAngle;
            if (m_ptRadiu.X >= m_ptCenter.X)
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptCenter, m_ptRadiu);
            }
            else
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptRadiu, m_ptCenter);
            }
            GE_Vector vOffset = m_ptRadiu - m_ptCenter;
            vOffset.normalize();
            base.m_ptText = m_ptRadiu + 2.5 * m_TxtHeight * vOffset;
            //base.m_ptText = Geo.AcGeMidPoint(m_ptCenter, m_ptRadiu);
            base.m_Text = m_DimText;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = Geo.Rtd(dTxtAngle);
            //绘文字
            base.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);

            if (m_ptArcStart.distanceTo(m_ptArcEnd) > 1)
            {
                double dStart = Geo.GetAngleByPoint(m_ptCenter, m_ptArcStart);
                double dEnd = Geo.GetAngleByPoint(m_ptCenter, m_ptArcEnd);
                double dRad = m_ptCenter.distanceTo(m_ptRadiu);
                //DrawArc(m_ptCenter, dRad, dStart, dEnd, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                for (int i = 0; i <m_lstPt.Count - 1; i++)
                {
                    //m_cPreview.addLine(lstPt[i], lstPt[i + 1], m_BorderColor, m_BorderWeight);
                    DrawLine(m_lstPt[i] * 1.2, m_lstPt[i + 1] * 1.2,g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                }
                DrawLine(m_lstPt[0], m_lstPt[0] * 1.2, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                DrawLine(m_lstPt[m_lstPt.Count - 1], m_lstPt[m_lstPt.Count - 1] * 1.2, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);


            }
        }
    }
    //双端标注弧线
    public class CGeoEntity_DataDimension_RadiuAngle : CGeoEntity_DataText
    {
        public GE_Point m_ptCenter;
        public GE_Point m_ptRadiu;
        public GE_Point m_ptArcStart;
        public GE_Point m_ptArcEnd;
        public string m_DimText;

        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            bool bRet = true;
            List<GE_Point> ayPts = new List<GE_Point>();
            double dTxtAngle;

            ayPts.Add(m_ptCenter);
            ayPts.Add(m_ptRadiu);
            ayPts.Add(m_ptArcStart);
            ayPts.Add(m_ptArcEnd);
            if (m_ptRadiu.X >= m_ptCenter.X)
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptCenter, m_ptRadiu);
            }
            else
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptRadiu, m_ptCenter);
            }
            base.m_ptText = Geo.AcGeMidPoint(m_ptCenter, m_ptRadiu) ;
            base.m_Text = m_DimText;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = Geo.Rtd(dTxtAngle);
            GE_Point[] ptBor = new GE_Point[2];
            base.getExtent(out ptBor[0], out ptBor[1]);
            ayPts.Add(ptBor[0]);
            ayPts.Add(ptBor[1]);

            bRet = Geo.GetExtents(ayPts, out ptMin, out ptMax);
            return bRet;
        }
        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            GE_Vector vPos = Geo.GetVector(m_ptCenter, m_ptArcStart);
            if ((m_ptArcEnd - m_ptCenter).GetAngle() >= 3 * Geo.PI2)
                DrawArrow(m_ptArcStart, vPos.RotateBy(-Geo.PI2 + Math.Atan(0.15)), m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            else
                DrawArrow(m_ptArcStart, vPos.RotateBy(Geo.PI2 + Math.Atan(0.15)), m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);

            vPos = Geo.GetVector(m_ptCenter, m_ptArcEnd);
                DrawArrow(m_ptArcEnd, vPos.RotateBy(-Geo.PI2 - Math.Atan(0.15)), m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);

            double dTxtAngle;
            if (m_ptRadiu.X >= m_ptCenter.X)
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptCenter, m_ptRadiu);
            }
            else
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptRadiu, m_ptCenter);
            }
            GE_Vector vOffset = m_ptRadiu - m_ptCenter;
            vOffset.normalize();
            base.m_ptText = m_ptRadiu + 2.5 * m_TxtHeight * vOffset;
            //base.m_ptText = Geo.AcGeMidPoint(m_ptCenter, m_ptRadiu);
            base.m_Text = m_DimText ;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = Geo.Rtd(dTxtAngle);
            //绘文字
            base.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);

            if (/*m_hasArc &&*/ m_ptArcStart.distanceTo(m_ptArcEnd) > 1)
            {
                double dStart = Geo.GetAngleByPoint(m_ptCenter, m_ptArcStart);
                double dEnd = Geo.GetAngleByPoint(m_ptCenter, m_ptArcEnd);
                double dRad = m_ptCenter.distanceTo(m_ptRadiu);
                DrawArc(m_ptCenter, dRad, dStart, dEnd, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }
        }
    }

    public class CGeoEntity_DataDimension_Radiu : CGeoEntity_DataText
    {
        public GE_Point m_ptCenter;
        public GE_Point m_ptRadiu;
        public GE_Point m_ptArcStart;
        public GE_Point m_ptArcEnd;
        public string m_DimText;
        public bool m_hasArc = true;

        public bool getExtent(out GE_Point ptMin, out GE_Point ptMax)
        {
            bool bRet = true;
            List<GE_Point> ayPts = new List<GE_Point>();
            double dTxtAngle;

            ayPts.Add(m_ptCenter);
            ayPts.Add(m_ptRadiu);
            if (m_hasArc) { 
                ayPts.Add(m_ptArcStart);
                ayPts.Add(m_ptArcEnd);
            }
            if (m_ptRadiu.X >= m_ptCenter.X)
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptCenter, m_ptRadiu);
            }
            else
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptRadiu, m_ptCenter);
            }
            base.m_ptText = Geo.AcGeMidPoint(m_ptCenter, m_ptRadiu);
            base.m_Text = m_DimText;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = Geo.Rtd(dTxtAngle);
            GE_Point[] ptBor = new GE_Point[2];
            base.getExtent(out ptBor[0], out ptBor[1]);
            ayPts.Add(ptBor[0]);
            ayPts.Add(ptBor[1]);

            bRet = Geo.GetExtents(ayPts, out ptMin, out ptMax);
            return bRet;
        }
        public void DrawImage(Graphics g, int iOffsetX, int iOffsetY, double dScale, int nWidth, int nHeight)
        {
            DrawLine(m_ptCenter, m_ptRadiu, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            GE_Vector vPos = Geo.GetVector(m_ptRadiu, m_ptCenter);
            DrawArrow(m_ptRadiu, vPos, m_TxtHeight * 0.75, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);

            double dTxtAngle;
            if (m_ptRadiu.X >= m_ptCenter.X)
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptCenter, m_ptRadiu);
            }
            else
            {
                dTxtAngle = Geo.GetAngleByPoint(m_ptRadiu, m_ptCenter);
            }
            base.m_ptText = Geo.AcGeMidPoint(m_ptCenter, m_ptRadiu);
            base.m_Text = m_DimText;
            base.m_Align = ContentAlignment.BottomCenter;
            base.m_Rotate = Geo.Rtd(dTxtAngle);
            //绘文字
            base.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);

            if (m_hasArc && m_ptArcStart.distanceTo(m_ptArcEnd)>1)
            {
                double dStart = Geo.GetAngleByPoint(m_ptCenter, m_ptArcStart);
                double dEnd = Geo.GetAngleByPoint(m_ptCenter, m_ptArcEnd);
                double dRad = m_ptCenter.distanceTo(m_ptRadiu);
                DrawArc(m_ptCenter, dRad, dStart, dEnd, g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
            }
        }
    }
    public class CGeoEntity
    {
        public Color m_Background = Color.Black;
        public int m_Border = 20;
        private List<CGeoEntity_Data> m_lstData = new List<CGeoEntity_Data>();
        public float m_EntityWidth = 0;
        public float m_EntityHeight = 0;
        public GE_Point m_EntityPointMin;
        public GE_Point m_EntityPointMax;

        public int m_iOffsetX = 0;
        public int m_iOffsetY = 0;
        public double m_dScale = 1.0;

        public CGeoEntity()
        {            
        }
        static public CGeoEntity_DataLine CreateLine(GE_Point ptStart, GE_Point ptEnd, System.Drawing.Color iColor)
        {
            CGeoEntity_DataLine cLine = new CGeoEntity_DataLine();
            cLine.m_Color = iColor;
            cLine.m_start = ptStart;
            cLine.m_end = ptEnd;

            return cLine;
        }
        static public CGeoEntity_DataArc CreateArc(GE_Point ptCenter, double dRadiu, double dStartAngle, double dEndAngle, System.Drawing.Color iColor)
        {
            CGeoEntity_DataArc cArc = new CGeoEntity_DataArc();
            cArc.m_Color = iColor;
            cArc.m_center = ptCenter;
            cArc.m_radiu = (float)dRadiu;
            cArc.m_startAngle = (float)dStartAngle;
            cArc.m_endAngle = (float)dEndAngle;
            return cArc;
        }
        static public CGeoEntity_DataArc CreateArc(GE_Point pt1,GE_Point pt2,GE_Point pt3, System.Drawing.Color iColor)
        {
            GE_Point ptCenter;
            double dRadiu, dStartPoint, dEndPoint;
            if (Geo.GetArcByPoint(pt1, pt2, pt3, out ptCenter))
            {
                CGeoEntity_DataArc cArc = new CGeoEntity_DataArc();
                cArc.m_Color = iColor;
                cArc.m_center = ptCenter;
                cArc.m_radiu = (float)pt1.distanceTo(ptCenter);
                cArc.m_startAngle = (float)Geo.GetVector(ptCenter, pt1).GetAngle();
                cArc.m_endAngle = (float)Geo.GetVector(ptCenter, pt3).GetAngle();
                return cArc;
            }
            else
            {
                return null;
            }
        }
        public void clear()
        {
            m_lstData = new List<CGeoEntity_Data>();
            m_EntityWidth = 0;
            m_EntityHeight = 0;
            m_EntityPointMin=new GE_Point(0,0);
            m_EntityPointMax=new GE_Point(0,0);
        }
        public void addDrawData(CGeoEntity_Data obj)
        {
            if (obj != null)
            {
                m_lstData.Add(obj);
            }
        }
        public int getDrawDataCount()
        {
            int iCount = 0;
            if (m_lstData != null) { iCount = m_lstData.Count; }
            return iCount;
        }
 
        /// </summary>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        public object getDrawData(int iIndex)
        {
            object obj = null;
            int iCount = getDrawDataCount();
            if (iIndex >= 0 && iIndex < iCount)
            {
                obj = m_lstData[iIndex];
                
            }
            return obj;
        }

        public void InitImage()
        {
            int i;
            object obj;
            Type typeObj;
            GE_Point ptMin, ptMax;
            bool bFirst = true;
            bool bGet;

            ptMin = new GE_Point();
            ptMax = new GE_Point();
            for (i=0;i<m_lstData.Count;i++) {
                obj = m_lstData[i];
                typeObj = obj.GetType();
                bGet = false;
                if (typeObj.IsClass) {
                    if (string.Compare(typeObj.Name,"CGeoEntity_DataLine",true)==0) {
                        CGeoEntity_DataLine datLine = (CGeoEntity_DataLine)obj;
                        datLine.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name, "CGeoEntity_DataArc", true) == 0)
                    {
                        CGeoEntity_DataArc datArc = (CGeoEntity_DataArc)obj;
                        datArc.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name, "CGeoEntity_DataCircle", true) == 0)
                    {
                        CGeoEntity_DataCircle datCir = (CGeoEntity_DataCircle)obj;
                        datCir.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name, "CGeoEntity_DataSolid", true) == 0)
                    {
                        CGeoEntity_DataSolid datSolid = (CGeoEntity_DataSolid)obj;
                        datSolid.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name,"CGeoEntity_DataBox",true)==0)
                    {
                        CGeoEntity_DataBox datBox = (CGeoEntity_DataBox)obj;
                        datBox.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_Aligned", true) == 0)
                    {
                        CGeoEntity_DataDimension_Aligned datDimAlig = (CGeoEntity_DataDimension_Aligned)obj;
                        datDimAlig.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_Rotated", true) == 0)
                    {
                        CGeoEntity_DataDimension_Rotated datDimRot = (CGeoEntity_DataDimension_Rotated)obj;
                        datDimRot.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_Radiu", true) == 0)
                    {
                        CGeoEntity_DataDimension_Radiu datDimRad = (CGeoEntity_DataDimension_Radiu)obj;
                        datDimRad.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_RadiuAngle", true) == 0)
                    {
                        CGeoEntity_DataDimension_RadiuAngle datDimRad = (CGeoEntity_DataDimension_RadiuAngle)obj;
                        datDimRad.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name, "CGeoEntity_DataText", true) == 0)
                    {
                        CGeoEntity_DataText datText = (CGeoEntity_DataText)obj;
                        datText.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                    else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_SimilarRadiuAngle", true) == 0)
                    {
                        CGeoEntity_DataDimension_SimilarRadiuAngle datDimRad = (CGeoEntity_DataDimension_SimilarRadiuAngle)obj;
                        datDimRad.getExtent(out ptMin, out ptMax);
                        bGet = true;
                    }
                }
                if (bGet) {
                    if (bFirst) 
                    {
                        bFirst = false;
                        m_EntityPointMin = ptMin;
                        m_EntityPointMax = ptMax;
                    }
                    else {
                        m_EntityPointMin.X = Math.Min(m_EntityPointMin.X, ptMin.X);
                        m_EntityPointMin.Y = Math.Min(m_EntityPointMin.Y, ptMin.Y);
                        m_EntityPointMax.X = Math.Max(m_EntityPointMax.X, ptMax.X);
                        m_EntityPointMax.Y = Math.Max(m_EntityPointMax.Y, ptMax.Y);
                    }
                }
            }
            try
            {
                m_EntityWidth = (float)Math.Abs(m_EntityPointMax.X - m_EntityPointMin.X);
            }
            catch (System.Exception)
            {
                m_EntityWidth = 0;
            }
            try
            {
                m_EntityHeight = (float)Math.Abs(m_EntityPointMax.Y - m_EntityPointMin.Y);
            }
            catch (System.Exception)
            {
                m_EntityHeight = 0;
            }
            
        }
        public void Draw(int nWidth, int nHeight,out Bitmap bitmap)
        {
            bitmap = new Bitmap(nWidth, nHeight);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(m_Background);

            Draw(nWidth, nHeight, ref g);
        }

        public void Draw(int nWidth, int nHeight, ref Graphics g)
        {
            int i;
            object obj;
            Type typeObj;

            //rzb
            m_iOffsetX = 0;
            m_iOffsetY = 0;
//             nWidth -= 20;
//             nHeight -= 20;
            m_dScale = 1.0;

            if (m_EntityHeight > 0 && m_EntityWidth > 0)
            {
                int iOffsetX, iOffsetY;
                int iSldWidth = (int)m_EntityWidth;
                int iSldHeight = (int)m_EntityHeight;
                double dScaleX, dScaleY, dScale;

                dScaleX = (nWidth - m_Border * 2) * 1.0 / iSldWidth;
                dScaleY = (nHeight - m_Border * 2) * 1.0 / iSldHeight;
                if (dScaleX < dScaleY)
                {
                    dScale = dScaleX;
                    iOffsetX = m_Border;
                    iOffsetY = (int)((nHeight - iSldHeight * dScale) / 2);
                    iOffsetY -= m_Border;
                }
                else
                {
                    dScale = dScaleY;
                    iOffsetX = (int)((nWidth - iSldWidth * dScale) / 2);
                    iOffsetX += m_Border;
                    iOffsetY = m_Border;
                }
                iOffsetX -= (int)(m_EntityPointMin.X * dScale);
                iOffsetY -= (int)(m_EntityPointMin.Y * dScale);

                m_iOffsetX = iOffsetX;
                m_iOffsetY = iOffsetY;
                m_dScale = dScale;
                for (i = 0; i < m_lstData.Count; i++)
                {
                    obj = m_lstData[i];
                    typeObj = obj.GetType();
                    if (typeObj.IsClass)
                    {
                        if (string.Compare(typeObj.Name, "CGeoEntity_DataLine", true) == 0)
                        {
                            CGeoEntity_DataLine datLine = (CGeoEntity_DataLine)obj;
                            datLine.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataArc", true) == 0)
                        {
                            CGeoEntity_DataArc datArc = (CGeoEntity_DataArc)obj;
                            datArc.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataCircle", true) == 0)
                        {
                            CGeoEntity_DataCircle datCir = (CGeoEntity_DataCircle)obj;
                            datCir.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataSolid", true) == 0)
                        {
                            CGeoEntity_DataSolid datSolid = (CGeoEntity_DataSolid)obj;
                            datSolid.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataBox", true) == 0)
                        {
                            CGeoEntity_DataBox datBox = (CGeoEntity_DataBox)obj;
                            datBox.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataText", true) == 0)
                        {
                            CGeoEntity_DataText datText = (CGeoEntity_DataText)obj;
                            datText.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_Aligned", true) == 0)
                        {
                            CGeoEntity_DataDimension_Aligned datDimAlig = (CGeoEntity_DataDimension_Aligned)obj;
                            datDimAlig.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_Rotated", true) == 0)
                        {
                            CGeoEntity_DataDimension_Rotated datDimRot = (CGeoEntity_DataDimension_Rotated)obj;
                            datDimRot.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_Radiu", true) == 0)
                        {
                            CGeoEntity_DataDimension_Radiu datDimRad = (CGeoEntity_DataDimension_Radiu)obj;
                            datDimRad.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_RadiuAngle", true) == 0)
                        {
                            CGeoEntity_DataDimension_RadiuAngle datDimRad = (CGeoEntity_DataDimension_RadiuAngle)obj;
                            datDimRad.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                        else if (string.Compare(typeObj.Name, "CGeoEntity_DataDimension_SimilarRadiuAngle", true) == 0)
                        {
                            CGeoEntity_DataDimension_SimilarRadiuAngle datDimRad = (CGeoEntity_DataDimension_SimilarRadiuAngle)obj;
                            datDimRad.DrawImage(g, iOffsetX, iOffsetY, dScale, nWidth, nHeight);
                        }
                    }
                }
            }
        }

        public void FillPreview(out Bitmap bitmap, int nWidth, int nHeight)
        {
            InitImage();
            Draw(nWidth, nHeight, out bitmap);
        }
        public void FillPreview(System.Web.UI.Page thePage, int nWidth, int nHeight)
        {
            Bitmap bitmap;
            InitImage();
            Draw(nWidth, nHeight, out bitmap);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

            thePage.Response.Expires = 0;
            thePage.Response.Buffer = true;
            thePage.Response.Clear();
            thePage.Response.ContentType = "image/Gif";
            thePage.Response.BinaryWrite(ms.ToArray());
            thePage.Response.End();
        }

        /// <summary>
        /// 获取图形坐标点ptInput在图像中的坐标
        /// </summary>
        /// <param name="ptInput">图像坐标</param>
        /// <param name="nHeight">图像高度，必须与调用Draw时的一致</param>
        /// <param name="ptPosition">图像坐标</param>
        public void CalculatePoint(GE_Point ptInput,int nHeight, out GE_Point ptPosition)
        {
            Single x1, y1;
            x1 = (Single)(ptInput.X * m_dScale);
            y1 = (Single)(ptInput.Y * m_dScale);
            y1 = nHeight - y1;
            x1 += m_iOffsetX;
            y1 -= m_iOffsetY;
            ptPosition = new GE_Point(x1, y1);
        }
    }
}
