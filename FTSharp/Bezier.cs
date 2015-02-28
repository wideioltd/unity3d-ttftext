using System;

namespace FTSharp
{
    
    public class point {
        public float x;
        public float y;
    }
    
    public class BezierC
    {
        public BezierC ()
        {
        }

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

        public static point Bezier(point a, point b, float t) {
            point res = new point();
            
            res.x = a.x + (b.x - a.x) * t;
            res.y = a.y + (b.y - a.y) * t;
            return res;
        }
        
        public static point Bezier(point a, point b, point c, float t) {
            point q0 = Bezier(a, c, t);
            point q1 = Bezier(c, b, t);
            return Bezier(q0, q1, t);
        }
        
        public static point Bezier(point a, point b, point c1, point c2, float t) {
            point q0 = Bezier(a, c1, t);
            point q1 = Bezier(c1, c2, t);
            point q2 = Bezier(c2, b, t);
            
            return Bezier(q0, q1, q2, t);
        }
    }
}

