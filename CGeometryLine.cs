using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GeometryEx
{
    public class GE_Line
    {
        GE_Point m_StartPoint;
        GE_Point m_EndPoint;
        GE_Point m_CenterPoint;        
        int m_Kind=0;
        double m_Radiu=0;

        public GE_Line() {}
        public void set(GE_Point ptStart,GE_Point ptEnd)
        {
            m_StartPoint=new GE_Point(ptStart.X,ptStart.Y);
            m_EndPoint=new GE_Point(ptEnd.X,ptEnd.Y);
            m_Radiu = 0;
            m_Kind=0;
        }
        public void set(GE_Point ptCenter,GE_Point ptStart, GE_Point ptEnd)
        {
            m_StartPoint = new GE_Point(ptStart.X, ptStart.Y);
            m_EndPoint = new GE_Point(ptEnd.X, ptEnd.Y);
            m_CenterPoint = new GE_Point(ptCenter.X, ptCenter.Y);
            m_Radiu = m_CenterPoint.distanceTo(m_StartPoint);
            m_Kind = 1;
        }
        public GE_Vector Vector()
        {
            return Geo.GetVector(m_StartPoint, m_EndPoint);
        }
        public double Length()
        {
            double dRet=0;
            if (m_Kind == 0) { dRet = m_StartPoint.distanceTo(m_EndPoint); }
            else { dRet = Geo.GetArcLengthByPoint(m_CenterPoint, m_StartPoint, m_EndPoint); }
            return dRet;
        }
        public void reverse()
        {
            Geo.Exchange_point(ref m_StartPoint, ref m_EndPoint);
        }
        public GE_Point PerpendPoint(GE_Point pt)
        {
            GE_Point ptRet;
            if (m_Kind==1) {
                GE_Vector v = Geo.GetVector(m_CenterPoint, pt);                
                ptRet = m_CenterPoint + v * m_Radiu;
            }
            else {
                double dL = Length();
                List<GE_Point> ayPoints = new List<GE_Point>();
                ayPoints.Add(m_StartPoint);
                ayPoints.Add(m_EndPoint);
                ayPoints.Add(pt);
                double dArea=Geo.AreaRange(ayPoints);
                double dDist = dArea * 2 / dL;
                GE_Vector vp = Vector();
                if (Geo.AcGePosition(m_StartPoint,m_EndPoint,pt)>0) {
                    vp.RotateBy(Geo.PI2);
                }
                else {
                    vp.RotateBy(-Geo.PI2);
                }
                ptRet = pt + vp * dDist;
            }
            return ptRet;
        }
        public double distanceTo(GE_Point pt)
        {
            GE_Point ptPerp = PerpendPoint(pt);
            return pt.distanceTo(ptPerp);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lin"></param>
        /// <param name="ptInt1"></param>
        /// <param name="ptInt2"></param>
        /// <returns></returns>
        public int intersectWith(GE_Line lin,out GE_Point ptInt1,out GE_Point ptInt2)
        {
            int nRet = 0;            
            ptInt1 = new GE_Point(0, 0);
            ptInt2 = new GE_Point(0, 0);
            double A, B, a, b, c, d, P, Q, AA, BB, CC, X1, X2, Y1, Y2;

            double u;

            if (m_Kind==0 && lin.m_Kind==0) {
                PointF point1 = new PointF((float)m_StartPoint.X, (float)m_StartPoint.Y);
                PointF point2 = new PointF((float)m_EndPoint.X, (float)m_EndPoint.Y);
                PointF point3 = new PointF((float)lin.m_StartPoint.X, (float)lin.m_StartPoint.Y);
                PointF point4 = new PointF((float)lin.m_EndPoint.X, (float)lin.m_EndPoint.Y);

                PointF pointResult;
                if (GetIntersection(point1, point2, point3, point4, out pointResult))
                {
                    ptInt1.X = pointResult.X;
                    ptInt1.Y = pointResult.Y;
                    nRet = 1;
                }
                
                
                //double x11, y11, x12, y12, x21, y21, x22, y22;

                //x11 = m_StartPoint.X;
                //y11 = m_StartPoint.Y;
                //x12 = m_EndPoint.X;
                //y12 = m_EndPoint.Y;
                //x21 = lin.m_StartPoint.X;
                //y21 = lin.m_StartPoint.Y;
                //x22 = lin.m_EndPoint.X;
                //y22 = lin.m_EndPoint.Y;

                //u = (x11 - x21) / ((x22 - x21) - (x12 - x11));
                //ptInt1.X = x11 + u * (x12 - x11);
                //ptInt1.Y = y11 + u * (y12 - y11);
                //nRet = 1;
            }
            else if (m_Kind==1 && lin.m_Kind==1) {
                
                A = m_CenterPoint.distanceTo(m_StartPoint);
                B = lin.m_CenterPoint.distanceTo(lin.m_StartPoint);
                a = m_CenterPoint.X;
                b = m_CenterPoint.Y;
                c = lin.m_CenterPoint.X;
                d = lin.m_CenterPoint.Y;

                P = ((A * A - B * B) - (a * a + b * b - c * c - d * d)) / (2 * d - 2 * b);
                Q = (c - a) / (d - b);
                AA = 1 + Q * Q;
                BB = 2 * b * Q - 2 * P * Q - 2 * a;
                CC = P * P - 2 * b * P - A * A + a * a + b * b;
                if (BB * BB - 4 * AA * CC > 0)
                {
                    X1 = (BB * -1 + Math.Sqrt(BB * BB - 4 * AA * CC)) / (2 * AA);
                    X2 = (BB * -1 - Math.Sqrt(BB * BB - 4 * AA * CC)) / (2 * AA);
                    Y1 = P - Q * X1;
                    Y2 = P - Q * X2;
                    ptInt1.X = X1; ptInt1.Y = Y1;
                    ptInt2.X = X2; ptInt2.Y = Y2;
                    nRet = 2;
                }
                else
                {
                    nRet = 0;
                }
            }

            return nRet;
        }

        /// <summary>        
        /// 计算两条直线的交点        
        /// </summary>        
        /// <param name="lineFirstStar">L1的点1坐标</param>        
        /// <param name="lineFirstEnd">L1的点2坐标</param>        
        /// <param name="lineSecondStar">L2的点1坐标</param>        
        /// <param name="lineSecondEnd">L2的点2坐标</param>        
        /// <returns></returns>        
        private bool GetIntersection(PointF lineFirstStar, PointF lineFirstEnd, PointF lineSecondStar, PointF lineSecondEnd, out PointF linInt)        
        {
            //* L1，L2都存在斜率的情况：             
            //* 直线方程L1: ( y - y1 ) / ( y2 - y1 ) = ( x - x1 ) / ( x2 - x1 )              
            //* => y = [ ( y2 - y1 ) / ( x2 - x1 ) ]( x - x1 ) + y1             
            //* 令 a = ( y2 - y1 ) / ( x2 - x1 )             
            //* 有 y = a * x - a * x1 + y1   .........1             
            //* 直线方程L2: ( y - y3 ) / ( y4 - y3 ) = ( x - x3 ) / ( x4 - x3 )             
            //* 令 b = ( y4 - y3 ) / ( x4 - x3 )             
            //* 有 y = b * x - b * x3 + y3 ..........2            
            //* 如果 a = b，则两直线平等，否则， 联解方程 1,2，得:             
            //* x = ( a * x1 - b * x3 - y1 + y3 ) / ( a - b )             
            //* y = a * x - a * x1 + y1                           
            //* L1存在斜率, L2平行Y轴的情况：             
            //* x = x3             
            //* y = a * x3 - a * x1 + y1                          
            //* L1 平行Y轴，L2存在斜率的情况：             
            //* x = x1             
            //* y = b * x - b * x3 + y3                          
            //* L1与L2都平行Y轴的情况：             
            //* 如果 x1 = x3，那么L1与L2重合，否则平等
            bool bRet = false;
            linInt = new PointF(0, 0);
            float a = 0, b = 0,x,y;
            int state = 0;
            if (lineFirstStar.X != lineFirstEnd.X)
            {
                a = (lineFirstEnd.Y - lineFirstStar.Y) / (lineFirstEnd.X - lineFirstStar.X);
                state |= 1;
            }
            if (lineSecondStar.X != lineSecondEnd.X)
            {
                b = (lineSecondEnd.Y - lineSecondStar.Y) / (lineSecondEnd.X - lineSecondStar.X);
                state |= 2;
            }
            switch (state)
            {
                case 0: //L1与L2都平行Y轴         
                    if (lineFirstStar.X == lineSecondStar.X)
                    {
                        //throw new Exception("两条直线互相重合，且平行于Y轴，无法计算交点。");       
                        //return new PointF(0, 0);
                    }
                    else
                    {
                        //throw new Exception("两条直线互相平行，且平行于Y轴，无法计算交点。");        
                        //return new PointF(0, 0);
                    }
                    break;
                case 1: //L1存在斜率, L2平行Y轴     
                    x = lineSecondStar.X;
                    y = (lineFirstStar.X - x) * (-a) + lineFirstStar.Y;
                    linInt = new PointF(x, y);
                    bRet = true;
                    break;
                case 2: //L1 平行Y轴，L2存在斜率      
                    x = lineFirstStar.X;
                    //网上有相似代码的，这一处是错误的。你可以对比case 1 的逻辑 进行分析                            //源code:lineSecondStar * x + lineSecondStar * lineSecondStar.X + p3.Y; 
                    y = (lineSecondStar.X - x) * (-b) + lineSecondStar.Y;
                    linInt = new PointF(x, y);
                    bRet = true;
                    break;
                case 3: //L1，L2都存在斜率                      
                    if (a == b)
                    {
                        // throw new Exception("两条直线平行或重合，无法计算交点。");     
                        //return new PointF(0, 0);
                    }
                    x = (a * lineFirstStar.X - b * lineSecondStar.X - lineFirstStar.Y + lineSecondStar.Y) / (a - b);
                    y = a * x - a * lineFirstStar.X + lineFirstStar.Y;
                    linInt = new PointF(x, y);
                    bRet = true;
                    break;
            }
            return bRet;
        }
    }
}
