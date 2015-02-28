using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FTSharp
{

    public class Outline
    {


        float flattenSteps = 4;

        public enum PointType
        {
            MoveTo, LineTo, ConicTo, CubicTo
        }

        
        public class Point
        {
            public float X;
            public float Y;

            public Point() : this(0, 0) { }

            public Point(float x, float y) {
               X = x;
               Y = y;
            }

            public Point(FT.FT_Vector vec, float scale)
            {
                X = FT.F26Dot6toFloat(vec.x) * scale;
                Y = FT.F26Dot6toFloat(vec.y) * scale;
            }

            public Point(FT.FT_Vector vec) : this(vec, 1) {}

            public Point Translate(float x, float y)
            {
                X += x;
                Y += y;
                return this;
            }

            public Point Translate(Point t)
            {
                X += t.X;
                Y += t.Y;
                return this;
            }

            public Point Scale(float scale)
            {
                X *= scale;
                Y *= scale;
                return this;
            }

            static public Point operator +(Point u,Point t)
            {
                return new Point(u.X + t.X, u.Y + t.Y);
            }


            static public Point operator *(Point p,float s)
            {
                return new Point(p.X *s, p.Y *s);
            }

            public override string ToString()
            {
                return "(" + X + "," + Y + ")";
            }
        }


        // TODO: change flatten curve algo without fixed number of steps

        public static float Bezier(float a, float b, float t)
        {
            // a + (b - a) * t == a + t * b - t * a == (1 - t) * a + t * b
            return (1 - t) * a + t * b;
        }

        // conic
        public static float Bezier(float a, float b, float c, float t)
        {
            float q0 = Bezier(a, c, t);
            float q1 = Bezier(c, b, t);
            return Bezier(q0, q1, t);
        }

        // cubic
        public static float Bezier(float a, float b, float c1, float c2, float t)
        {
            float q0 = Bezier(a, c1, t);
            float q1 = Bezier(c1, c2, t);
            float q2 = Bezier(c2, b, t);
            return Bezier(q0, q1, q2, t);
        }


        public static Point Bezier(Point from, Point to, Point c, float t)
        {
          return new Point(Bezier(from.X, to.X, c.X, t), Bezier(from.Y, to.Y, c.Y, t));
        }

        public static Point Bezier(Point from, Point to, Point c1, Point c2, float t)
        {
            return new Point(Bezier(from.X, to.X, c1.X, c2.X, t), Bezier(from.Y, to.Y, c1.Y, c2.Y, t));
        }


        public List<Point> Path;
        public List<PointType> Types;

        public int Size
        {
            get { return Path.Count; }
        }


        public Outline()
        {
            Path = new List<Point>();
            Types = new List<PointType>();
        }

        public Outline(int _flattenSteps)
        {
            flattenSteps = _flattenSteps;
            Path = new List<Point>();
            Types = new List<PointType>();
        }


        public void AddPoint(Point p, PointType type)
        {
            Path.Add(p);
            Types.Add(type);
        }

        public void MoveTo(Point p)
        {
            AddPoint(p, PointType.MoveTo);
        }

        public void LineTo(Point p)
        {
            AddPoint(p, PointType.LineTo);
        }

        public void ConicTo(Point c, Point p)
        {
            AddPoint(c, PointType.ConicTo);
            AddPoint(p, PointType.ConicTo);
        }

        public void CubicTo(Point c1, Point c2, Point p)
        {
            AddPoint(c1, PointType.CubicTo);
            AddPoint(c2, PointType.CubicTo);
            AddPoint(p, PointType.CubicTo);
        }


        public void FlattenConicTo(Point c, Point to) {

            Point from = Path[Path.Count - 1]; // Path sould not be empty

            for (int i = 0; i <= flattenSteps; ++i)
            {
                float t = (float) i / (float) flattenSteps; 
                //Point p = new Point(BezierC.Bezier(from.X, to.X, t), BezierC.Bezier(from.Y, to.Y, t));
                AddPoint(Bezier(from, to, c, t), PointType.LineTo);
            }
        }

        public void FlattenCubicTo(Point c1, Point c2, Point to)
        {
            Point from = Path[Path.Count - 1];
            float u,t2,u2;
            

            for (int i = 0; i <= flattenSteps; ++i)
            {
                float t = (float)i / (float)flattenSteps;
                //AddPoint(Bezier(from, to, c1, c2, t), PointType.LineTo);
                u = 1f - t;
                t2 = t * t;
                u2 = u * u; 
                AddPoint((from*(u2*u)+c1*(3f*u2*t)+c2*(3f*t2*u)+to*(t2*t)), PointType.LineTo);
            }
        }


        public void Translate(float x, float y)
        {
            foreach (Point p in Path)
            {
                p.X += x;
                p.Y += y;
            }
        }


        public void Translate(Point t)
        {
            foreach (Point p in Path)
            {
                p.X += t.X;
                p.Y += t.Y;
            }
        }

        public void AddOutline(Outline other)
        {
            for (int i = 0; i < other.Size; ++i)
            {
                AddPoint(other.Path[i], other.Types[i]);
            }
        }



        static OutlineDecomposer odecomposer;

        public static Outline DecomposeGlyph(IntPtr glyph, float scale, int isteps)
        {
            Outline outline = new Outline(isteps);

            if (odecomposer == null)
            {
                odecomposer = new OutlineDecomposer(scale);
            }

            lock (odecomposer)
            {
                odecomposer.scale = scale;

                odecomposer.LineToEv = outline.LineTo;
                odecomposer.MoveToEv = outline.MoveTo;
                odecomposer.ConicToEv = outline.ConicTo;
                odecomposer.CubicToEv = outline.CubicTo;

                odecomposer.Decompose(glyph);
            }

            return outline;
        }

        public static Outline FlattenGlyph(IntPtr glyph, float scale, int isteps)
        {
            Outline outline = new Outline(isteps);

            if (odecomposer == null)
            {
                odecomposer = new OutlineDecomposer(scale);
            }

            lock (odecomposer)
            {

                odecomposer.scale = scale;

                odecomposer.LineToEv = outline.LineTo;
                odecomposer.MoveToEv = outline.MoveTo;
                odecomposer.ConicToEv = outline.FlattenConicTo;
                odecomposer.CubicToEv = outline.FlattenCubicTo;

                odecomposer.Decompose(glyph);
            }


            return outline;
        }
    }
}
