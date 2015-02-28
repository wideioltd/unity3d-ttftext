using System;
using System.Runtime.InteropServices;

namespace Triangulation
{
    public static class Glu
    {
     
        public enum TessProperty
        {
            WindingRule = 100140,
            BoundaryOnly = 100141,
            Tolerance = 100142
        }

        public enum TessWinding
        {
            WindingOdd = 100130,
            WindingNonzero = 100131,
            WindingPositive = 100132,
            WindingNegative = 100133,
            WindingAbsGeqTwo = 100134
        }

        public enum PrimitiveType
        { 
            Points = 0x0000,
            Lines = 0x0001,
            LineLoop = 0x0002,
            LineStrip = 0x0003,
            Triangles = 0x0004,
            TriangleStrip = 0x0005,
            TriangleFan = 0x0006,
            Quads = 0x0007,
            QuadStrip = 0x0008,
            Polygon = 0x0009
        }

        public enum CallbackName
        { 
            Begin = 100100,
            Vertex = 100101,
            End = 100102,
            Error = 100103,
            EdgeFlag = 100104,
            Combine = 100105
        } 

        //todo : set more meaningful names
        public enum TessError
        {
            Error1 = 100151,
            Error2 = 100152,
            Error3 = 100153,
            Error4 = 100154,
            Error5 = 100155,
            Error6 = 100156,
            NeedCombineCallback = Error6,
            Error7 = 100157,
            Error8 = 100158
        }


        // start of a triangle primitive
        public delegate void BeginCallback (PrimitiveType type);

        //end of a primitive
        public delegate void EndCallback ();
  
        public delegate void EdgeFlagCallback(int flag);
        
        public delegate void VertexCallback (IntPtr data);

        public delegate void ErrorCallback (TessError errorCode);

        public delegate void CombineCallback ([In] IntPtr coords, [In] IntPtr data, [In] IntPtr weight, [Out] out IntPtr outData);
        //public delegate void CombineCallback ([In] double[] coords,[In] IntPtr[] data, [In] float[] weight, [Out] IntPtr outData);
  
        
        [DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
        private extern static void TessBeginCallBack (
         IntPtr tess, 
         CallbackName which, 
         BeginCallback callback); 
  
        public static void TessBeginCallBack(IntPtr tess, BeginCallback callback) {
            TessBeginCallBack(tess, CallbackName.Begin, callback);                              
        }
                                             
        [DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
        private extern static void TessEndCallBack (
         IntPtr tesselationObject, 
         CallbackName which, 
         EndCallback callback);
        
        public static void TessEndCallBack(IntPtr tess, EndCallback callback) {
            TessEndCallBack(tess, CallbackName.End, callback);
        }
                                          
        [DllImport("glu32.dll", EntryPoint = "gluTessCallback")]
        private extern static void TessEdgeFlagCallBack(
            IntPtr tess,
            CallbackName which,
            EdgeFlagCallback callback);
        
        public static void TessEdgeFlagCallBack(IntPtr tess, EdgeFlagCallback callback) {
            TessEdgeFlagCallBack(tess, CallbackName.EdgeFlag, callback);
        }

        [DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
        private extern static void TessVertexCallBack (
         IntPtr tesselationObject, 
         CallbackName which, 
         VertexCallback callback); 
  
        public static void TessVertexCallBack(IntPtr tess, VertexCallback callback) {
            TessVertexCallBack(tess, CallbackName.Vertex, callback);
        }
        
        [DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
        private extern static void TessErrorCallBack (
         IntPtr tesselationObject, 
         CallbackName which, 
         ErrorCallback callback); 
  
        public static void TessErrorCallBack(IntPtr tess, ErrorCallback callback) {
            TessErrorCallBack(tess, CallbackName.Error, callback);
        }
        
        [DllImport("glu32.dll", EntryPoint="gluTessCallback")] 
        private extern static void TessCombineCallBack (
         IntPtr tesselationObject, 
         CallbackName which, 
         CombineCallback callback);

        public static void TessCombineCallBack(IntPtr tess, CombineCallback callback) {
            TessCombineCallBack(tess, CallbackName.Combine, callback);
        }


     
        [DllImport("glu32.dll", EntryPoint="gluNewTess")]
        public extern static IntPtr NewTess ();

        [DllImport("glu32.dll", EntryPoint="gluTessNormal")]
        public extern static void TessNormal (IntPtr tess, double x, double y, double z);

        [DllImport("glu32.dll", EntryPoint="gluTessProperty")]
        public extern static void TessSetProperty (IntPtr tess, TessProperty property, double value);

        public static void TessSetWindingRule (IntPtr tess, TessWinding rule)
        {
            TessSetProperty (tess, TessProperty.WindingRule, (double)rule);
        }

        [DllImport("glu32.dll", EntryPoint="gluTessBeginPolygon")]
        public extern static void TessBeginPolygon (IntPtr tess, IntPtr data);

        [DllImport("glu32.dll", EntryPoint="gluTessBeginContour")]
        public extern static void TessBeginContour (IntPtr tess);

        [DllImport("glu32.dll", EntryPoint="gluTessVertex")]
        public extern static void TessVertex (IntPtr tess, double[] coords, IntPtr data);
  
        [DllImport("glu32.dll", EntryPoint="gluTessEndContour")]
        public extern static void TessEndContour (IntPtr tess);

        [DllImport("glu32.dll", EntryPoint="gluTessEndPolygon")]
        public extern static void TessEndPolygon (IntPtr tess);

        [DllImport("glu32.dll", EntryPoint="gluDeleteTess")]
        public extern static void DeleteTess (IntPtr tess);

    }
}

