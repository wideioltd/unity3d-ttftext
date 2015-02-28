using System;

namespace Triangulation
{
      public class Vertex
    {
       
        public double[] location = new double[3];
        
        public double x {
            get { return location[0]; }
        }
       
        public double y {
            get { return location[1]; }
        }
        
        public double z {
            get { return location[2]; }
        }
        
        public Vertex (double x, double y, double z)
        {
            location = new double[3];
            location [0] = x;
            location [1] = y;
            location [2] = z;
        }
        
        public Vertex (double x, double y) : this(x, y, 0)
        {
        }
       
        public Vertex () : this(0, 0, 0)
        {
        }
           
        public Vertex (double[] loc) : this(loc[0], loc[1], loc[2])
        {
        }
        
        public static Vertex Combine (double[] location, Vertex[] inVertices, float[] weight)
        {
            return new Vertex (location);
        }
        
        public override string ToString ()
        {
            return string.Format ("[{0},{1},{2}]", location [0], location [1], location [2]);
        }
    };
}

