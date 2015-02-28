using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FTSharp
{
    class OutlineDecomposer
    {

        public delegate void MoveToEventHandler(Outline.Point p);
        public delegate void LineToEventHandler(Outline.Point p);
        public delegate void ConicToEventHandler(Outline.Point c, Outline.Point p);
        public delegate void CubicToEventHandler(Outline.Point c1, Outline.Point c2, Outline.Point p);

        

        public MoveToEventHandler MoveToEv=null;
        public LineToEventHandler LineToEv=null;
        public ConicToEventHandler ConicToEv=null;
        public CubicToEventHandler CubicToEv=null;

        float scale_;

        public float scale {
           get { return scale_; }
           set { scale_ = value; }
        }

        public OutlineDecomposer(float scale)
        {
            scale_ = scale;
        }


        int moveto_(IntPtr to, IntPtr data)
        {
            FT.FT_Vector vec = FT.HandleToRecord<FT.FT_Vector>(to);

            if (MoveToEv != null)
            {
                MoveToEv(new Outline.Point(vec, scale_));
            }
            return 0;
        }


        int lineto_(IntPtr to, IntPtr data)
        {
            FT.FT_Vector vec = FT.HandleToRecord<FT.FT_Vector>(to);

            if (LineToEv != null)
            {
                LineToEv(new Outline.Point(vec, scale_));
            }
            return 0;
        }

        int conicto_(IntPtr c, IntPtr to, IntPtr data)
        {
            FT.FT_Vector cvec = FT.HandleToRecord<FT.FT_Vector>(c);
            FT.FT_Vector tovec = FT.HandleToRecord<FT.FT_Vector>(to);

            if (ConicToEv != null)
            {
                ConicToEv(new Outline.Point(cvec, scale_), new Outline.Point(tovec, scale_));
            }
            return 0;

        }

        int cubicto_(IntPtr c1, IntPtr c2, IntPtr to, IntPtr data)
        {
            FT.FT_Vector c1vec = FT.HandleToRecord<FT.FT_Vector>(c1);
            FT.FT_Vector c2vec = FT.HandleToRecord<FT.FT_Vector>(c2);
            FT.FT_Vector tovec = FT.HandleToRecord<FT.FT_Vector>(to);

            if (ConicToEv != null)
            {
                CubicToEv(new Outline.Point(c1vec, scale_), new Outline.Point(c2vec, scale_), new Outline.Point(tovec, scale_));
            }
            return 0;
        }

        // delegate
        FT.OutlineMoveToFunc movetoF;
        FT.OutlineLineToFunc linetoF;
        FT.OutlineCubicToFunc cubictoF;
        FT.OutlineConicToFunc conictoF;

        FT.FT_Outline_Funcs decomposer_funcs = new FT.FT_Outline_Funcs();
        bool init_done=false;

         void init_funcs() {
            if (!init_done)
            {
                movetoF = moveto_;
                linetoF = lineto_;
                cubictoF = cubicto_;
                conictoF = conicto_;

                decomposer_funcs.move_to = Marshal.GetFunctionPointerForDelegate(movetoF);
                decomposer_funcs.line_to = Marshal.GetFunctionPointerForDelegate(linetoF);
                decomposer_funcs.conic_to = Marshal.GetFunctionPointerForDelegate(conictoF);
                decomposer_funcs.cubic_to = Marshal.GetFunctionPointerForDelegate(cubictoF);
                init_done = true;
            }
        }


        public void Decompose(IntPtr glyph)
        {
            init_funcs();
            decomposer_funcs.delta = 0;
            decomposer_funcs.shift = 0;

            IntPtr outline = FT.fthelper_glyph_get_outline_address(glyph);

            int code = FT.FT_Outline_Decompose(outline, ref decomposer_funcs, IntPtr.Zero);
            FT.CheckError(code);
        }
    }
}
