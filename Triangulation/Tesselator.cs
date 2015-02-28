using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Triangulation
{
    
      
    public class Tesselator<Vertex> : IDisposable
    {
  
        //events delegates
        
        //tesselation result
        public delegate void BeginEventHandler<V>(Tesselator<V> obj, Glu.PrimitiveType type);
        public delegate void EdgeFlagEventHandler<V>(Tesselator<V> tess, int flag);        
        public delegate void VertexEventHandler<V>(Tesselator<V> tess, V vertex);
        public delegate void EndEventHandler<V>(Tesselator<V> tess);
        
        //new vertex creation
        public delegate void CombineEventHandler<V>(Tesselator<V> tess, double[] location, V[] inVertices, float[] weight, V outVertex);
        
        //on error
        public delegate void ErrorEventHandler<V>(Tesselator<V> tess, Glu.TessError errorCode);
       
        
        //events
        
        public event BeginEventHandler<Vertex> BeginEv;
        public event EdgeFlagEventHandler<Vertex> EdgeFlagsEv;
        public event VertexEventHandler<Vertex> VertexEv;
        public event EndEventHandler<Vertex> EndEv;
        
        public event CombineEventHandler<Vertex> CombineEv;
        public event ErrorEventHandler<Vertex> ErrorEv;
       
        
        // TODO: this should not be here, but constructed in tesselation result event handler if needed
        private List<Vertex> vertices = new List<Vertex> ();
    
        public List<Vertex> Vertice {
            get {
                return vertices;
            }
        }
        
        //tesselation result
        private List<int> indices = new List<int> ();
        
        public List<int> Indices {
            get {
                return indices;
            }
        }
 
       
        protected static IntPtr tess=System.IntPtr.Zero;
  
        
        // callbacks
        protected Glu.BeginCallback beginCallback;
        protected Glu.EdgeFlagCallback edgeFlagCallback;
        protected Glu.VertexCallback vertexCallback;
        protected Glu.EndCallback endCallback;
        protected Glu.CombineCallback combineCallback;
        protected Glu.ErrorCallback errorCallback;
        
        
        // the combine function (vertex type specific)
        public delegate Vertex combineVertexD (double[] location, Vertex[] vertices, float[] weight);

        combineVertexD combineVertex;
    
        
        public Tesselator (combineVertexD combine)
        {
            combineVertex = combine;
            if (tess == System.IntPtr.Zero)
            {
                tess = Glu.NewTess();
            }
            //setup callbacks

            beginCallback = new Glu.BeginCallback(this.OnBegin);
            edgeFlagCallback = new Glu.EdgeFlagCallback(this.OnEdgeFlag);
            endCallback = new Glu.EndCallback(this.OnEnd);
            vertexCallback = new Glu.VertexCallback(this.OnVertex);
            
            combineCallback = new Glu.CombineCallback(this.OnCombine);
            errorCallback = new Glu.ErrorCallback(this.OnError);
                
            Glu.TessBeginCallBack(tess, beginCallback);
            Glu.TessEdgeFlagCallBack(tess, edgeFlagCallback);
            Glu.TessEndCallBack(tess, endCallback);

            Glu.TessVertexCallBack(tess, vertexCallback);
            Glu.TessCombineCallBack(tess, combineCallback);
            Glu.TessErrorCallBack(tess, errorCallback);                       
        }
    
        ~Tesselator ()
        {
            Dispose ();
        }
    
        public void Dispose ()
        {
            Clear();
            //Console.WriteLine ("Tesselator.Dispose");
            
            //if (tess != IntPtr.Zero) {
            //    Glu.DeleteTess (tess);
            //    tess = IntPtr.Zero;
            // }
        }
    
        
        public void Clear() {
            vertices.Clear();
            indices.Clear ();
        }
        
        
        public void BeginPolygon ()
        {
            Clear();
            Glu.TessBeginPolygon(tess, IntPtr.Zero);
        }
    
        public void EndPolygon ()
        {
            Glu.TessEndPolygon(tess);
        }
    
        public void BeginContour ()
        {
            Glu.TessBeginContour(tess);
        }
    
        public void EndContour ()
        {
            Glu.TessEndContour (tess);
        }
    
        public void AddVertex (double [] location, Vertex v)
        {        
            int idx = vertices.Count;
            vertices.Add(v);
            Glu.TessVertex(tess, location, (IntPtr) idx);
        }
    
    
        // TODO: properties
    
        public void SetNormal (double x, double y, double z)
        {
            Glu.TessNormal (tess, x, y, z);
        }

        public void SetWindingRule(Glu.TessWinding rule)
        {
            Glu.TessSetWindingRule(tess, rule);
        }
    
        // callbacks
        protected void OnBegin (Glu.PrimitiveType type)
        {
            // ASSERT type == TRIANGLE
            
            if (type != Glu.PrimitiveType.Triangles) {
               // Console.WriteLine ("ERROR: NOT TRIANGLES TYPE (type={0})", type);
            }
            
            if (BeginEv != null) {
                BeginEv(this, type);
            }
        }
    
        
        
        protected void OnCombine([In] IntPtr coordsx, [In] IntPtr inDatax, [In] IntPtr weightx, [Out] out IntPtr outVertex)
        {
    
            //Console.WriteLine("OnCombine! do some Marshalling stuff!");
            
            double[] coords = new double[3];
            Marshal.Copy(coordsx, coords, 0, 3);
            
            IntPtr[] inData = new IntPtr[4];
#if DATA_NOT_COORDS
            Marshal.Copy(inDatax, inData, 0, 4);
#endif            
            float[] weight = new float[4];
            Marshal.Copy(weightx, weight, 0, 4);
            

            
            Vertex[] inVertices = new Vertex[4];
#if DATA_NOT_COORDS
            for (int i = 0; i < 4; ++i) {
                //ERROR: index out of Range !
                //Console.WriteLine("i=" + i);
                //Console.WriteLine("OnCombine: idx = " + (int) inData[i]);
                inVertices[i] = vertices[(int) inData[i]]; 
            }
#endif            

            Vertex v = combineVertex (coords, inVertices, weight);
            
            int idx = vertices.Count;
            vertices.Add (v);
            outVertex = (IntPtr) idx;
            
            if (CombineEv != null) {
                CombineEv(this, coords, inVertices, weight, v);
            }
        }
       
      
    
        protected void OnEdgeFlag (int flag)
        {
            if (EdgeFlagsEv != null) {
                EdgeFlagsEv(this, flag);
            }
        }
    

        
        protected void OnEnd ()
        {
            if (EndEv != null) {
                EndEv(this);
            }
        }
    
        protected void OnError (Glu.TessError errorCode)
        {
            //Console.WriteLine("Tesselation error: " + errorCode);
            if (ErrorEv != null) {
                ErrorEv(this, errorCode);
            }
        }

        protected void OnVertex ([In] IntPtr vData)
        {
            int idx = (int) vData;
            indices.Add(idx);
            if (VertexEv != null) {
                VertexEv(this, vertices[idx]);
            }
        }
    }
}

