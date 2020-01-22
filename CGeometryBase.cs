using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GeometryEx
{
    public class GE_Vector
    {
        private double m_X;
        private double m_Y;

        private const double MINL = 0.000000001;
        static public GE_Vector VECTOR_X = new GE_Vector(1, 0);
        static public GE_Vector VECTOR_Y = new GE_Vector(0, 1);
        public GE_Vector(GE_Vector v)
        {
            m_X = v.X;
            m_Y = v.Y;
        }
        public GE_Vector()
        {
            m_X = 0;
            m_Y = 0;
        }
        public GE_Vector(double x, double y)
        {
            m_X = x;
            m_Y = y;
        }
        public double X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        public double Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        public static GE_Vector XAxis
        {
            get { return new GE_Vector(1, 0); }
        }
        public static GE_Vector YAxis
        {
            get { return new GE_Vector(0, 1); }
        }
        public static GE_Vector operator -(GE_Vector a)
        {
            return new GE_Vector(-a.X, -a.Y);
        }
        public static GE_Vector operator -(GE_Vector a, GE_Vector b)
        {
            return new GE_Vector(a.X + b.X, a.Y + b.Y);
        }
        public static bool operator !=(GE_Vector a, GE_Vector b)
        {
            return (Math.Abs(a.X - b.X) > MINL || Math.Abs(a.Y - b.Y) > MINL);
        }
        public static GE_Vector operator *(double factor, GE_Vector a)
        {
            return new GE_Vector(a.X * factor, a.Y * factor);
        }
        public static GE_Vector operator *(GE_Vector a, double factor)
        {
            return new GE_Vector(a.X * factor, a.Y * factor);
        }
        public static GE_Vector operator /(GE_Vector a, double factor)
        {
            if (Math.Abs(factor) > MINL)
            {
                return new GE_Vector(a.X / factor, a.Y / factor);
            }
            else
            {
                return new GE_Vector(a.X, a.Y);
            }
        }
        public static GE_Vector operator +(GE_Vector a, GE_Vector b)
        {
            return new GE_Vector(a.X + b.X, a.Y + b.Y);
        }
        public static bool operator ==(GE_Vector a, GE_Vector b)
        {
            return (Math.Abs(a.X - b.X) < MINL && Math.Abs(a.Y - b.Y) < MINL);
        }
        public double Length
        {
            get { return Math.Sqrt(m_X * m_X + m_Y * m_Y); }
        }
        public void normalize()
        {
            double dLen = Math.Sqrt(m_X * m_X + m_Y * m_Y);
            if (dLen > MINL)
            {
                m_X = m_X / dLen;
                m_Y = m_Y / dLen;
            }
        }
        public GE_Vector RotateBy(double dRotate)
        {
            double dLen = Math.Sqrt(m_X * m_X + m_Y * m_Y);
            double dAng;

            if (Geo.fequ(m_X, 0))
            {
                if (m_Y > 0) { dAng = Math.PI / 2; }
                else if (m_Y < 0) { dAng = Math.PI * 1.5; }
                else { dAng = 0; }
            }
            else { dAng = Math.Atan(m_Y / m_X); }
            dAng = dAng + dRotate;
            m_X = dLen * Math.Cos(dAng);
            m_Y = dLen * Math.Sin(dAng);
            return new GE_Vector(m_X, m_Y);
        }
        //public double GetAngleTo(GE_Vector vector)
        //{
        //    double dAng1, dAng2;

        //    if (Geo.fequ(m_X, 0)) { dAng1 = 0; }
        //    else { dAng1 = Math.Atan(m_Y / m_X); }
        //    if (Geo.fequ(vector.X, 0)) { dAng2 = 0; }
        //    else { dAng2 = Math.Atan(vector.Y / vector.X); }

        //    return Geo.Mid_angle(dAng1, dAng2);
        //}
        public double GetAngle()
        {
            double dAngle=0;
            if (Geo.fequ(m_Y, 0))
            {
                if (m_X >= 0) { dAngle = 0; }
                else { dAngle = Math.PI; }
            }
            else
            {
                if (Geo.fequ(m_X, 0))
                {
                    if (m_Y > 0) { dAngle = Math.PI / 2; }
                    else { dAngle = Math.PI * 1.5; }
                }
                else
                {
                    dAngle = Math.Atan(Math.Abs(m_Y) / Math.Abs(m_X));
                    if (m_X > 0 && m_Y > 0)
                    {
                        //第一象限
                    }
                    else if (m_X > 0 && m_Y < 0)
                    {
                        //第四象限
                        dAngle = Math.PI * 2 - dAngle;
                    }
                    else if (m_X < 0 && m_Y > 0)
                    {
                        //第二象限
                        dAngle = Math.PI - dAngle;
                    }
                    else
                    {
                        //第三象限
                        dAngle = Math.PI + dAngle;
                    }
                }
            }
            return dAngle;
        }
    }
    public class GE_Point
    {
        private double m_X;
        private double m_Y;
        private const double MINL = 0.000000001;

        public GE_Point(GE_Point p)
        {
            m_X = p.X;
            m_Y = p.Y;
        }
        public GE_Point()
        {
            m_X = 0;
            m_Y = 0;
        }
        public GE_Point(double x, double y)
        {
            m_X = x;
            m_Y = y;
        }
        public double X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        public double Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        public static GE_Vector operator -(GE_Point a, GE_Point b)
        {
            return new GE_Vector(a.X - b.X, a.Y - b.Y);
        }
        public static GE_Point operator -(GE_Point a, GE_Vector b)
        {
            return new GE_Point(a.X - b.X, a.Y - b.Y);
        }
        public static bool operator !=(GE_Point a, GE_Point b)
        {
            return (Math.Abs(a.X - b.X) > MINL || Math.Abs(a.Y - b.Y) > MINL);
        }
        public static GE_Point operator *(double value, GE_Point a)
        {
            return new GE_Point(a.X * value, a.Y * value);
        }
        public static GE_Point operator *(GE_Point a, double value)
        {
            return new GE_Point(a.X * value, a.Y * value);
        }
        public static GE_Point operator /(GE_Point a, double value)
        {
            if (Math.Abs(value) > MINL)
            {
                return new GE_Point(a.X / value, a.Y / value);
            }
            else
            {
                return new GE_Point(a.X, a.Y);
            }
        }
        public static GE_Point operator +(GE_Point a, GE_Vector vector)
        {
            return new GE_Point(a.X + vector.X, a.Y + vector.Y);
        }
        public static bool operator ==(GE_Point a, GE_Point b)
        {
            return (Math.Abs(a.X - b.X) < MINL && Math.Abs(a.Y - b.Y) < MINL);
        }
        public static implicit operator PointF(GE_Point f)
        {
            return new PointF((float)f.X, (float)f.Y);
        }

        public double distanceTo(GE_Point pt)
        {
            return Math.Sqrt((pt.X - m_X) * (pt.X - m_X) + (pt.Y - m_Y) * (pt.Y - m_Y));
        }
        public void set(double x, double y)
        {
            m_X = x;
            m_Y = y;
        }
        public bool set(Object objInput)
        {
            bool bRet = false;
            if (objInput != null)
            {
                try
                {
                    Array pt = (Array)objInput;
                    if (pt.Length >= 2)
                    {
                        m_X = Convert.ToDouble(pt.GetValue(0));
                        m_Y = Convert.ToDouble(pt.GetValue(1));
                        bRet = true;
                    }
                }
                catch (System.Exception ex)
                {
                    bRet = false;
                }
            }
            return bRet;
        }
    }
}
