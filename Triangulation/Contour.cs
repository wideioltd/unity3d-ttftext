using System;
using System.Collections.Generic;

namespace Triangulation
{
    public class Contour
    {
        public List<Vertex> path;
        
        public int Size {
            get { return path.Count; }
        }
        
        public Contour (params Vertex[] p)
        {
            path = new List<Vertex> ();
            foreach (Vertex v in p) {
                path.Add (v);
            }
        }
        
        public void Clear() { path.Clear(); }
    }
}

