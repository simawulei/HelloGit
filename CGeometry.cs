using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GeometryEx
{
    public class Geo
    {
        public const double MINL = 0.000000001;
        public const double PI2 = Math.PI / 2;

        static public double Mid_angle(double a1,double a2)
        {
            double a = a2 - a1;
            if (a < 0.0) a = 2 * Math.PI + a;
            return a;
        }
        //角度->弧度
        static public double Dtr(double x)
        {
            double re;
            re = x * Math.PI / 180.0;
            return re;
        }
        //弧度->角度
        static public double Rtd(double x)
        {
            double re;
            re = x * 180.0 / Math.PI;
            return re;
        }
        static public GE_Vector GetVector(GE_Point ptFrom,GE_Point ptTo)
        {
            GE_Vector v=new GE_Vector(ptTo.X-ptFrom.X,ptTo.Y-ptFrom.Y);
            v.normalize();
            return v;
        }
        static public GE_Vector GetVectorByAngle(double dAngle)
        {
            GE_Point pt1 = new GE_Point(0, 0);
            GE_Point pt2 = AcGePolar(pt1, dAngle, 10);
            return GetVector(pt1, pt2);
        }
        static public double GetAngle(GE_Point ptFrom,GE_Point ptTo)
        {
            GE_Vector v = GetVector(ptFrom, ptTo);
            return v.GetAngle();
        }
        static public double GetAngleByPoint(GE_Point ptFrom,GE_Point ptTo)
        {
            GE_Vector v = GetVector(ptFrom, ptTo);
            return v.GetAngle();
        }
        static public GE_Point AcGePolar(GE_Point pt, double dAngle, double dRadiu)
        {
            GE_Point ptResult = new GE_Point(pt.X + (float)(dRadiu * Math.Cos(dAngle)), pt.Y + (float)(dRadiu * Math.Sin(dAngle)));
            return ptResult;
        }
        static public bool fequ(double dval1,double dval2)
        {
            bool bRet=false;
            if ((double)Math.Abs(dval1-dval2)<MINL) {bRet=true;}
            return bRet;
        }
        static public double DistanceTo(GE_Point pt1, GE_Point pt2)
        {
            return Math.Sqrt((pt2.X - pt1.X) * (pt2.X - pt1.X) + (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y));
        }
        static public GE_Point AcGeMidPoint(GE_Point pt1, GE_Point pt2)
        {
            GE_Point pt = new GE_Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
            return pt;
        }        
        static public void Exchange_point(ref GE_Point pt1,ref GE_Point pt2)
        {
            GE_Point pt = new GE_Point(pt1.X,pt1.Y);
            pt1 = pt2;
            pt2 = pt;
        }
        static public double AcGePosition(GE_Point p1, GE_Point p2, GE_Point p)
        {
            double x, y, x1, y1, x2, y2, ret;
            x = p.X; y = p.Y;
            x1 = p1.X; y1 = p1.Y;
            x2 = p2.X; y2 = p2.Y;
            ret = x * y1 + x1 * y2 + x2 * y - x2 * y1 - x1 * y - x * y2;
            return ret;
        }
        //弦长，半径求角度
        static public double GetAngleByLength(double dLength, double dRad)
        {
            double dRet=0;
            try
            {
                if (dRad <= dLength / 2) { dRet = 0; }
                else
                {
                    double dTmp;
                    dTmp = dLength / 2;
                    dRet = Math.Sqrt(dRad * dRad - dTmp * dTmp);
                    dRet = Math.Atan(dTmp / dRet) * 2;
                }
            }
            catch (System.Exception) {}
            
            return dRet;
        }
        //弧长，半径求角度
        static public double GetAngleByRadiu(double dLength, double dRad)
        {
            double dRet = 0;
            try
            {
                dRet = dLength / dRad;
            }
            catch (System.Exception) { }
            return dRet;
        }

        //弦长，半径求弦高
        static public double GetArcHeight(double dLength, double dRad)
        {
            double dRet=0;
            if (dRad > dLength / 2)
            {
                dRet = dRad - Math.Sqrt(dRad * dRad - (dLength * dLength) / 4);
            }
            else { dRet = 0; }
            return dRet;
        }
        //如果P1到P2到P3为顺时针,则返回1,否则返回0
        static public int AcGeClock(GE_Point p1, GE_Point p2, GE_Point p3)
        {            
            double ang1, ang2;
            int v;
                        
            ang1 = GetAngle(p1, p2);
            ang2 = GetAngle(p2, p3);
            if ((ang1 < Math.PI) || (Math.Abs(ang1 - Math.PI) < 0.0000001))
            {
                if ((ang2 < Math.PI) || (Math.Abs(ang2 - Math.PI) < 0.0000001))
                {
                    if (ang2 > ang1) v = 0;
                    else v = 1;
                }
                else
                {
                    ang1 = ang1 + Math.PI;
                    if (ang2 > ang1) v = 1;
                    else v = 0;
                }
            }
            else
            {
                if ((ang2 < Math.PI) || (Math.Abs(ang2 - Math.PI) < 0.0000001))
                {
                    ang1 = ang1 - Math.PI;
                    if (ang2 > ang1) v = 1;
                    else v = 0;
                }
                else
                {
                    if (ang2 > ang1) v = 0;
                    else v = 1;
                }
            }
            return v;
        }
        //圆心ptCen,点p1到p2的弧长(逆时针,半径为ptCen到p1的距离)
        static public double GetArcLengthByPoint(GE_Point ptCen, GE_Point p1, GE_Point p2)
        {
            double dRad, dAng1, dAng2;
            dRad = ptCen.distanceTo(p1);            
            dAng1 = GetAngleByPoint(ptCen, p1); 
            dAng2 = GetAngleByPoint(ptCen, p2);
            dAng1 = Mid_angle(dAng1, dAng2);
            return dRad * dAng1;
        }
        //任意多边形面积
        static public double AreaRange(List<GE_Point> ayPoint)
        {
            double ret = 0.0;
            GE_Point p1, p2;
            int i;

            for (i = 0; i < ayPoint.Count - 1; i++)
            {
                p1 = ayPoint[i];
                p2 = ayPoint[i + 1];
                ret = ret + (p1.X * p2.Y - p1.Y * p2.X);
            }
            p1 = ayPoint[ayPoint.Count - 1];
            p2 = ayPoint[0];
            ret = ret + (p1.X * p2.Y - p1.Y * p2.X);
            ret =Math.Abs(ret) * 0.5;
            return ret;
        }
        static public bool GetArcByPoint(GE_Point pt1, GE_Point pt2, GE_Point pt3, out GE_Point ptCenter)
        {
            bool bRet = false;
            ptCenter = new GE_Point();

            GE_Line lin1 = new GE_Line();
            GE_Line lin2 = new GE_Line();
            GE_Point ptM1 = Geo.AcGeMidPoint(pt1, pt2);
            GE_Point ptM2 = Geo.AcGeMidPoint(pt2, pt3);
            GE_Vector vp = Geo.GetVector(pt1, pt2);
            vp.RotateBy(PI2);
            lin1.set(ptM1, ptM1 + vp * 100.0);
            vp = Geo.GetVector(pt2, pt3);
            vp.RotateBy(PI2);
            lin2.set(ptM2, ptM2 + vp * 100.0);
            GE_Point ptCen1, ptCen2;
            int nRet = lin1.intersectWith(lin2, out ptCen1, out ptCen2);
            if (nRet > 0)
            {
                ptCenter = ptCen1;
                bRet = true;
            }
            return bRet;
        }
        static public List<GE_Point> ExplodeArc(GE_Point ptCenter, double dRadiu, double dAngleStart, double dAngleEnd, double dStep)
        {
            List<GE_Point> ayResult = new List<GE_Point>();
            double dAng = Mid_angle(dAngleStart, dAngleEnd);
            double dLength = dAng * dRadiu;
            int iNums = (int)(dLength / dStep);
            if (iNums <16) { iNums = 16; }
            double dAngBase = dAngleStart;
            GE_Point px;
            int i;

            if (iNums != 0)
            {
                dAng = dAng / iNums;
                for (i = 0; i < iNums; i++)
                {
                    px = AcGePolar(ptCenter, dAngBase, dRadiu);
                    ayResult.Add(px);
                    dAngBase = dAngBase + dAng;
                }
            }
            else
            {
                px = AcGePolar(ptCenter, dAngleStart, dRadiu);
                ayResult.Add(px);
            }
            px = AcGePolar(ptCenter, dAngleEnd, dRadiu);
            ayResult.Add(px);
            return ayResult;
        }

        static public bool GetExtents(List<GE_Point> ayPt, out GE_Point ptMin, out GE_Point ptMax)
        {
            bool bRet = false;
            int i;

            ptMin = new GE_Point();
            ptMax = new GE_Point();
            if (ayPt != null && ayPt.Count > 0)
            {
                ptMin = new GE_Point(ayPt[0]);
                ptMax = new GE_Point(ayPt[0]);
                for (i = 1; i < ayPt.Count; i++)
                {
                    ptMin.X = Math.Min(ptMin.X, ayPt[i].X);
                    ptMin.Y = Math.Min(ptMin.Y, ayPt[i].Y);
                    ptMax.X = Math.Max(ptMax.X, ayPt[i].X);
                    ptMax.Y = Math.Max(ptMax.Y, ayPt[i].Y);
                }
                bRet = true;
            }
            return bRet;
        }
    }
}
