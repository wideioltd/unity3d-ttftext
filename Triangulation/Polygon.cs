using System;
using System.Collections.Generic;

namespace Triangulation
{
     public class Polygon
    {
        
        public List<Contour> Contours;
        
        public int ContourCount {
            get { return Contours.Count; }
        }
        
        public Polygon() {
            Contours = new List<Contour>();
        }
        
        public Polygon (params Contour[] cs)
        {
            Contours = new List<Contour> ();
            foreach (Contour c in cs) {
                Contours.Add (c);
            }
        }
        
        public void AddContour(Contour c) {
            Contours.Add(c);
        }
        
        public void PopContour() {
            if (Contours.Count > 0) {
                Contours.RemoveAt(Contours.Count - 1);
            }
        }
    }
}

