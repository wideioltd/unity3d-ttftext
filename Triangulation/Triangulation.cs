using System;

namespace Triangulation
{

    // Not so generic Triangulation functions...
    // Better use directly Tesselator class for more specific needs

    public static class Triangulation
    {

        public delegate TResult vToLoc<in T, out TResult>(T arg);

        public static void Triangulate<V>(Tesselator<V> tess, Outline<V> outline, vToLoc<V, double[]> vertexToLocation)
        {
            
            bool inContour = false;

            tess.BeginPolygon();

            for (int i = 0; i < outline.Size; ++i)
            {

                if (outline.Types[i] == Outline<V>.PathType.MoveTo) // new contour
                {
                    if (inContour) { tess.EndContour(); }
                    tess.BeginContour();
                    inContour = true;
                }

                V v = outline.Path[i];
                tess.AddVertex(vertexToLocation(v), v);
            }

            if (inContour) { tess.EndContour(); }

            tess.EndPolygon();
        }


        public static Mesh Triangulate(Tesselator<Vertex> tess, Polygon p)
        {
            
            tess.BeginPolygon ();
            
            foreach (Contour c in p.Contours) {
                
                tess.BeginContour ();
            
                foreach (Vertex v in c.path) {
                    tess.AddVertex (v.location, v);
                }
            
                tess.EndContour ();
            }
            tess.EndPolygon ();
        
            return  new Mesh(tess.Vertice, tess.Indices);
        }
        
         public static Mesh Triangulate(Polygon p) {
            
            Tesselator<Vertex> tess = new Tesselator<Vertex> (Vertex.Combine);
            
            Mesh m = Triangulate(tess, p);
            
            tess.Dispose();
            return m;
        }
        
         public static Mesh TriangulateVerbose(Polygon p) {
            
            Tesselator<Vertex> tess = new Tesselator<Vertex> (Vertex.Combine);
            
            tess.BeginEv += verboseBegin;
            tess.EndEv += verboseEnd;
            tess.CombineEv += verboseCombine;
            tess.EdgeFlagsEv += verboseEdgeFlag;
            tess.VertexEv += verboseVertex;
            tess.ErrorEv += verboseError;
        
            Mesh m = Triangulate(tess, p);
            
            tess.Dispose();
            return m;
        }
        
        static void verboseBegin (Tesselator<Vertex> tess, Glu.PrimitiveType type)
        {
            Console.WriteLine ("BEGIN:" + type);
        }

        static void verboseCombine (Tesselator<Vertex> tess, double[] location, Vertex[] inVertices, float[] weight, 
                                   Vertex outVertex)
        {

            Console.WriteLine ("COMBINE: " + outVertex);
        }
        
        static void verboseEdgeFlag (Tesselator<Vertex> tess, int flag)
        {
            Console.WriteLine ("EDGEFLAG:" + flag);
        }
        
        static void verboseEnd (Tesselator<Vertex> tess)
        {
            Console.WriteLine ("END");
        }

        static void verboseError (Tesselator<Vertex> tess, Glu.TessError errorCode)
        {
            //string msg = Tesselator<Vertex>.ErrorString (errorCode);
            Console.WriteLine ("ERROR: {0} ({1})", errorCode, errorCode);
        }

        static void verboseVertex (Tesselator<Vertex> tess, Vertex vertex)
        {
            Console.WriteLine ("VERTEX:" + vertex);
        }
    }
}

