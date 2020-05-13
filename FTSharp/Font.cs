using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace FTSharp
{

    public enum HorizontalJustification
    {
        Origin, Left, Right, Center
    }

    public enum VerticalJustification
    {
        Origin, Top, Bottom, Center
    }

    public unsafe class Font : IDisposable
    {
        FTLibrary lib;
        IntPtr face_;
        private bool disposed_ = false;

        readonly short face_height_;
        readonly bool face_has_kerning_;
        readonly ushort face_units_per_EM_;


        string familyName_ = "";
        string styleName_ = "";

        int point_size_ = 12; // 1pt = 1/72th of a inch
        int resolution_ = 100; // DPI
        uint dpi_ = 72;
        float height_;  // default distance between 2 lines

        // Scaling vector
        float vectorScale_; // (point_size * resolution)/ (72 * face->units_per_em)


        public Font(string path, float height, uint dpi)
        {
            lib = FTLibrary.Instance;
            dpi_ = dpi;

            int code = FT.FT_New_Face(lib.Handle, path, 0, out face_);
            FT.CheckError(code);

            FT.FT_FaceRec facerec = FT.HandleToRecord<FT.FT_FaceRec>(face_);

            familyName_ = facerec.family_name;
            styleName_ = facerec.style_name;

            face_has_kerning_ = ((facerec.face_flags & FT.FT_FACE_FLAG_KERNING) != 0);
            face_units_per_EM_ = facerec.units_per_EM;
            face_height_ = facerec.height;

            //Console.WriteLine("new FONT = unit-per-em=" + face_units_per_EM_);
            //setCharSize(pointsize);

            setCharHeight(height);
        }

        public Font(string path) : this(path, 12, 72) { }

        ~Font()
        {
            Dispose(false); // called from GC
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed_) { return; }

            if (disposing)
            { // dispose managed ressources

            }

            // dispose unmanaged resources here
            if (face_ != IntPtr.Zero)
            {
                FT.FT_Done_Face(face_);
            }
            face_ = IntPtr.Zero;
        }


        public string FamilyName
        {
            get { return familyName_; }
        }

        public string StyleName
        {
            get { return styleName_; }
        }

        public string Name
        {
            get { return familyName_ + " (" + styleName_ + ")"; }
        }

        public bool HasKerning
        {
            get { return face_has_kerning_; }
        }

        #region "Font Size Handling"

        public int Size
        {
            get
            {
                return point_size_;
            }
            set
            {
                setCharSize(value);
            }
        }

        // Default Distance between two lines
        public float Height
        {
            get { return height_; }
            set { setCharHeight(value); }
        }



        // for vector outline rendering
        void setCharHeight(float height)
        {
            int code = FT.FT_Set_Char_Size(face_, 0, face_units_per_EM_ * 64, (uint)dpi_, (uint)dpi_);
            FT.CheckError(code);

            height_ = height;
            vectorScale_ = (height_ * 100) / (dpi_ * face_height_);
            //vectorScale_ = (height_ *100)/ (dpi_*face_height_);
        }

        // for bitmap rendering
        void setCharSize(int pointsize)
        {
            // Set face size
            point_size_ = pointsize;
            resolution_ = 100;

            //int code = FT.FT_Set_Char_Size(face_, 0, face_units_per_EM_ * 64, 72, 72);

            int code = FT.FT_Set_Char_Size(face_, 0, pointsize, (uint)dpi_, (uint)dpi_);
            FT.CheckError(code);


            // should look for this
            vectorScale_ = (point_size_ * resolution_) / (dpi_ * face_units_per_EM_);
            height_ = face_height_ * vectorScale_;
        }

        #endregion

        // Compute outline for a single glyph
        // if flatten is true convert curve into a list of segments
        // out parameter advance is set to the default glyph advancement
        // if embold <> 0, embold the outline by the desired strength
        public Outline GetGlyphOutline(uint idx, out Outline.Point advance, bool flatten, float embold, int isteps)
        {

            // TODO: add outline caching ?


            // load glyph for c in face slot
            //int code = FT.FT_Load_Char(face_, (uint)c, FT.FT_LOAD_DEFAULT); // FT_LOAD_DEFAULT, FT_LOAD_NO_BITMAP, FT_LOAD_NO_SCALE ?

            int code = FT.FT_Load_Glyph(face_, idx, FT.FT_LOAD_DEFAULT);
            FT.CheckError(code);


            // Check that the glyph is in Outline format

            FT.FT_FaceRec facerec = FT.HandleToRecord<FT.FT_FaceRec>(face_);

            IntPtr slot = facerec.glyph;

            FT.FT_GlyphSlotRec slotrec = FT.HandleToRecord<FT.FT_GlyphSlotRec>(slot);

            if (slotrec.format != FT.GLYPH_FORMAT_OUTLINE)
            {
                throw new FTError(string.Format("Bad glyph format (0x{0:x4})", slotrec.format));
                //throw new FTError("Bad glyph format (" + slotrec.format + ")");
            }

            // Get glyph outline in gptr
            IntPtr gptr;
            code = FT.FT_Get_Glyph(slotrec, out gptr);
            FT.CheckError(code);


            // Embold outline
            if (embold != 0)
            {
                // Console.WriteLine("EMBOLD=" + embold);
                // IntPtr optr = FT.fthelper_glyph_get_outline_address(slot);
                FT.FT_Outline_Embolden(slotrec.outline, (int)(embold * 64 * 100)); // convert to 26.6 fractional format 
            }

            // Decompose outline
            Outline outline = null;

            if (flatten)
            {
                outline = Outline.FlattenGlyph(slotrec, vectorScale_, isteps);
            }
            else
            {
                outline = Outline.DecomposeGlyph(slotrec, vectorScale_, isteps);
            }

            FT.FT_Done_Glyph(gptr);

            advance = new Outline.Point(slotrec.advance, vectorScale_);

            return outline;
        }
        public Outline GetGlyphOutline(char c, out Outline.Point advance, bool flatten, float embold, int isteps)
        {
            uint idx = (uint)FT.FT_Get_Char_Index(face_, c);
            return GetGlyphOutline(idx, out advance, flatten, embold, isteps);
        }

        public Outline GetGlyphOutline(char c, out Outline.Point advance, bool flatten, int isteps)
        {
            uint idx = (uint)FT.FT_Get_Char_Index(face_, c);
            return GetGlyphOutline(idx, out advance, flatten, 0, isteps);
        }


        public Outline GetGlyphOutline(char c, out Outline.Point advance, bool flatten)
        {
            uint idx = (uint)FT.FT_Get_Char_Index(face_, c);
            return GetGlyphOutline(idx, out advance, flatten, 0, 4);
        }



        /*
         * public Outline GetGlyphOutline(int idx, out Outline.Point advance, bool flatten)
        {
            return GetGlyphOutline(idx, out advance, flatten, 0, 4);
        }
        */

        public Outline GetGlyphOutline(char c, out Outline.Point advance)
        {
            return GetGlyphOutline(c, out advance, true);
        }

        public Outline GetGlyphOutline(char c, bool flatten)
        {
            Outline.Point adv;
            return GetGlyphOutline(c, out adv, flatten);
        }


        public Outline GetGlyphOutline(char c)
        {
            Outline.Point adv;
            return GetGlyphOutline(c, out adv, true);
        }


        public Outline.Point GetKerning(uint left, uint right)
        {
            FT.FT_Vector delta = new FT.FT_Vector();
            FT.FT_Get_Kerning(face_, left, right, FT.FT_KERNING_UNFITTED, out delta);
            return new Outline.Point(delta, vectorScale_);
        }

        // Compute the outline for a whole line
        public Outline GetStringOutline(string txt, ref Outline.Point position, HorizontalJustification h, VerticalJustification v, bool useKerning, float spacing, float emboldStrenght, int isteps)
        {

            Outline res = new Outline();

            // first set pen position according to requested text justification

            if (h != HorizontalJustification.Origin || v != VerticalJustification.Origin)
            {
                // Compute Bounding box and set original advancement according to justification
                BBox bbox = Measure(txt);

                switch (h)
                {
                    case HorizontalJustification.Left:
                        position.X = position.X - bbox.xMin;
                        break;
                    case HorizontalJustification.Right:
                        position.X = position.X - bbox.xMax;
                        break;
                    case HorizontalJustification.Center:
                        position.X = position.X - (bbox.xMin + bbox.xMax) / 2;
                        break;
                }

                switch (v)
                {
                    case VerticalJustification.Top:
                        position.Y = position.Y - bbox.yMax;
                        break;
                    case VerticalJustification.Bottom:
                        position.Y = position.Y - bbox.yMin;
                        break;
                    case VerticalJustification.Center:
                        position.Y = position.Y - (bbox.yMin + bbox.yMax) / 2;
                        break;
                }
            }

            // Console.WriteLine("Original pen position=" + position);

            if (!HasKerning) { useKerning = false; }

            uint prev = 0;

            for (int i = 0; i < txt.Length; ++i)
            {

                uint idx = (uint)FT.FT_Get_Char_Index(face_, txt[i]);

                if (useKerning && prev != 0 && idx != 0) // adjust with kerning properties
                {
                    position.Translate(GetKerning((uint)prev, idx));
                }

                Outline.Point adv;
                Outline o = GetGlyphOutline(idx, out adv, true, emboldStrenght, isteps);

                o.Translate(position);
                res.AddOutline(o);

                adv.Scale(spacing);
                position.Translate(adv);

                prev = idx;
            }

            /*
            foreach (char c in txt)
            {
                Outline.Point adv;
                Outline o = GetGlyphOutline(c, out adv, true);
                o.Translate(advance);
                res.AddOutline(o);
                advance.Translate(adv);
            }
            */

            return res;
        }


        public Outline GetStringOutline(string txt, ref Outline.Point position, bool useKerning, float spacing, float emboldStrenght)
        {
            return GetStringOutline(txt, ref position, HorizontalJustification.Origin, VerticalJustification.Origin, useKerning, spacing, emboldStrenght, 4);
        }

        public Outline GetStringOutline(string txt, ref Outline.Point position, float spacing, float emboldStrenght)
        {
            return GetStringOutline(txt, ref position, HorizontalJustification.Origin, VerticalJustification.Origin, false, spacing, emboldStrenght, 4);
        }

        public Outline GetStringOutline(string txt, bool useKerning, float spacing, float emboldStrenght)
        {
            Outline.Point adv = new Outline.Point();
            return GetStringOutline(txt, ref adv, useKerning, spacing, emboldStrenght);
        }

        public Outline GetStringOutline(string txt, float spacing, float emboldStrenght)
        {
            Outline.Point adv = new Outline.Point();
            return GetStringOutline(txt, ref adv, false, spacing, emboldStrenght);
        }

        public Outline GetStringOutline(string txt, ref Outline.Point position, HorizontalJustification h, VerticalJustification v, bool useKerning, float spacing)
        {
            return GetStringOutline(txt, ref position, h, v, useKerning, spacing, 0, 4);
        }

        public Outline GetStringOutline(string txt, ref Outline.Point position, HorizontalJustification h, VerticalJustification v, bool useKerning)
        {
            return GetStringOutline(txt, ref position, h, v, useKerning, 1);
        }

        public Outline GetStringOutline(string txt, ref Outline.Point advance, HorizontalJustification h, VerticalJustification v)
        {
            return GetStringOutline(txt, ref advance, h, v, false);
        }

        public Outline GetStringOutline(string txt, HorizontalJustification h, VerticalJustification v, bool useKerning, float hspacing, float embold)
        {
            Outline.Point adv = new Outline.Point();
            return GetStringOutline(txt, ref adv, h, v, useKerning, hspacing, embold, 4);
        }

        public Outline GetStringOutline(string txt, HorizontalJustification h, VerticalJustification v, bool useKerning, float hspacing)
        {
            Outline.Point adv = new Outline.Point();
            return GetStringOutline(txt, ref adv, h, v, useKerning, hspacing);
        }

        public Outline GetStringOutline(string txt, HorizontalJustification h, VerticalJustification v, bool useKerning)
        {
            Outline.Point adv = new Outline.Point();
            return GetStringOutline(txt, ref adv, h, v, useKerning);
        }


        public Outline GetStringOutline(string txt, HorizontalJustification h, VerticalJustification v)
        {
            Outline.Point adv = new Outline.Point();
            return GetStringOutline(txt, ref adv, h, v, false);
        }

        public Outline GetStringOutline(string txt)
        {
            Outline.Point adv = new Outline.Point();
            return GetStringOutline(txt, ref adv, HorizontalJustification.Origin, VerticalJustification.Origin, false);
        }


        public Outline GetTextOutline(string[] lines, ref Outline.Point position, HorizontalJustification h, VerticalJustification v, bool useKerning, float interline)
        {

            //int nblines = lines.Length;

            // total cbox
            BBox bbox = Measure(lines, interline);

            float startY = 0;
            switch (v)
            {
                case VerticalJustification.Origin:
                    startY = position.Y;
                    break;
                case VerticalJustification.Top:
                    startY = position.Y - bbox.yMax;
                    break;
                case VerticalJustification.Bottom:
                    startY = position.Y - bbox.yMin;
                    break;
                case VerticalJustification.Center:
                    startY = position.Y + (bbox.yMax - bbox.yMin) / 2 - bbox.yMax;
                    break;
            }

            Outline outline = new Outline();
            Outline.Point pos = new Outline.Point();

            foreach (string line in lines)
            {

                pos.X = position.X;
                pos.Y = startY;

                Outline o = GetStringOutline(line, ref pos, h, VerticalJustification.Origin, useKerning);

                outline.AddOutline(o);

                startY -= Height * interline;
            }

            return outline;
        }




        // Compute the Bounding box of a single glyph
        public BBox Measure(char c, out Outline.Point advance)
        {

            // load glyph for c in face slot
            int code = FT.FT_Load_Char(face_, (uint)c, (int)(FT.FT_LOAD_DEFAULT | (1 << 3))); // FT_LOAD_DEFAULT, FT_LOAD_NO_BITMAP, FT_LOAD_NO_SCALE ?
            FT.CheckError(code);


            // Check that the glyph is in Outline format

            FT.FT_FaceRec facerec = FT.HandleToRecord<FT.FT_FaceRec>(face_);

            IntPtr slot = facerec.glyph;

            FT.FT_GlyphSlotRec slotrec = FT.HandleToRecord<FT.FT_GlyphSlotRec>(slot);

            if (slotrec.format != FT.GLYPH_FORMAT_OUTLINE)
            {
                throw new FTError("Bad glyph format (" + slotrec.format + ")");
            }


            // get glyph in gptr
            IntPtr gptr;
            code = FT.FT_Get_Glyph(slotrec, out gptr);
            FT.CheckError(code);

            FT.FT_BBox ft_bbox;
            FT.FT_Glyph_Get_CBox(gptr, 0, out ft_bbox);

            FT.FT_Done_Glyph(gptr);

            BBox bbox = new BBox(ft_bbox, vectorScale_);

            advance = new Outline.Point(slotrec.advance, vectorScale_);

            return bbox;
        }

        // Compute the Bounding Box a text line
        public BBox Measure(string txt)
        {
            BBox bbox = new BBox();
            Outline.Point advance = new Outline.Point();

            foreach (char c in txt)
            {
                Outline.Point cadv;
                BBox cbbox = Measure(c, out cadv);

                // translate char bbox to total advancement so far
                cbbox.Translate(advance);

                bbox.Merge(cbbox);

                // add char adv to total adv
                advance.Translate(cadv);
            }

            return bbox;
        }

        // Compute the Bounding Box of multilines
        public BBox Measure(string[] lines, float interline)
        {
            Outline.Point position = new Outline.Point(); // start at 0,0
            Outline.Point lineTransition = new Outline.Point(0, -Height * interline);

            BBox bbox = new BBox();

            foreach (string line in lines)
            {
                BBox lbox = Measure(line);
                lbox.Translate(position);
                bbox.Merge(lbox);
                position.Translate(lineTransition);
            }

            return bbox;
        }


        //public delegate void DrawBitmapF(Bitmap bitmap, int posx, int posy);
        public delegate void DrawBitmapF(FT.FT_Bitmap bitmap, int posx, int posy);

        public void RenderText(string text, DrawBitmapF drawBitmap, int penx, int peny)
        {
            int code;

            FT.FT_FaceRec facerec = FT.HandleToRecord<FT.FT_FaceRec>(face_);
            IntPtr slot = facerec.glyph;

            foreach (char c in text)
            {
                code = FT.FT_Load_Char(face_, c, FT.FT_LOAD_RENDER/*|FT.FT_LOAD_TARGET_NORMAL*/);
                if (code != 0) { continue; }

                FT.FT_GlyphSlotRec slotrec = FT.HandleToRecord<FT.FT_GlyphSlotRec>(slot);
                //Bitmap bitmap = new Bitmap(slotrec.bitmap);

                //                drawBitmap(bitmap, penx + slotrec.bitmap_left, peny + slotrec.bitmap_top);
                //drawBitmap(bitmap, penx + slotrec.bitmap_left, peny - slotrec.bitmap_top);
                drawBitmap(slotrec.bitmap, penx + slotrec.bitmap_left, peny - slotrec.bitmap_top);
                penx += (slotrec.advance.x >> 6);
            }
        }


        public unsafe UnityEngine.Texture2D RenderIntoTexture(char c) {
            int code;
	    
            FT.FT_FaceRec facerec = FT.HandleToRecord<FT.FT_FaceRec>(face_);
            IntPtr slot = facerec.glyph;
            
            code = FT.FT_Load_Char(face_, c, FT.FT_LOAD_RENDER/*|FT.FT_LOAD_TARGET_NORMAL*/);
            if (code != 0) { return null; }

            FT.FT_GlyphSlotRec slotrec = FT.HandleToRecord<FT.FT_GlyphSlotRec>(slot);
            FTSharp.FT.FT_Bitmap  ftbm=slotrec.bitmap;    
            
	    UnityEngine.Texture2D texbuffer=new UnityEngine.Texture2D((int)ftbm.width, (int)ftbm.rows,UnityEngine.TextureFormat.ARGB32,false);
	    if (((int)ftbm.pixel_mode)!=2) {
			UnityEngine.Debug.LogError("Unsupported bitmap depth :"+((int)ftbm.pixel_mode).ToString());
			return null;
	    }
	    //  FT_PIXEL_MODE_MONO,
	    //FT_PIXEL_MODE_GRAY,
    	
	    UnityEngine.Color32 [] clrs=texbuffer.GetPixels32();
	    int i=0;
	    
		
		
		
	  // UNSAFE DOES NOT WORK IN THE WEB PLAYER !
	    for (int y=0;y<ftbm.rows;y++) {		      
			long j=(((ftbm.rows-(1+y))*ftbm.pitch));
			byte * bo=((byte *)ftbm.buffer);
			for (int x=0;x<ftbm.width;x++) {
			  clrs[i].a=clrs[i].r=clrs[i].g=clrs[i].b=(bo[j+x]);
			  i++;
			}
	    }
	    texbuffer.SetPixels32(clrs);
	    texbuffer.Apply();
	    return texbuffer;
	}

    }
}