using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace FTSharp
{
    public static class FT
    {

        // Constants
        public const int FT_FACE_FLAG_SCALABLE = (1 << 0);
        public const int FT_FACE_FLAG_FIXED_SIZES = (1 << 1);
        public const int FT_FACE_FLAG_FIXED_WIDTH = (1 << 2);
        public const int FT_FACE_FLAG_SFNT = (1 << 3);
        public const int FT_FACE_FLAG_HORIZONTAL = (1 << 4);
        public const int FT_FACE_FLAG_VERTICAL = (1 << 5);
        public const int FT_FACE_FLAG_KERNING = (1 << 6);
        public const int FT_FACE_FLAG_FAST_GLYPHS = (1 << 7);
        public const int FT_FACE_FLAG_MULTIPLE_MASTERS = (1 << 8);
        public const int FT_FACE_FLAG_GLYPH_NAMES = (1 << 9);
        public const int FT_FACE_FLAG_EXTERNAL_STREAM = (1 << 10);
        public const int FT_FACE_FLAG_HINTER = (1 << 11);
        public const int FT_FACE_FLAG_CID_KEYED = (1 << 12);
        public const int FT_FACE_FLAG_TRICKY = (1 << 13);

        public const int FT_KERNING_DEFAULT  = 0;
        public const int FT_KERNING_UNFITTED = 1;
        public const int FT_KERNING_UNSCALED = 2;

        public const int FT_LOAD_DEFAULT = 0x0;
        public const int FT_LOAD_NO_SCALE = 0x1;
        public const int FT_LOAD_NO_HINTING = 0x2;
        public const int FT_LOAD_RENDER = 0x4;
        public const int FT_LOAD_NO_BITMAP = 0x8;
        public const int FT_LOAD_VERTICAL_LAYOUT = 0x10;
        public const int FT_LOAD_FORCE_AUTOHINT = 0x20;
        public const int FT_LOAD_CROP_BITMAP = 0x40;
        public const int FT_LOAD_PEDANTIC = 0x80;
        public const int FT_LOAD_IGNORE_GLOBAL_ADVANCE_WIDTH = 0x200;
        public const int FT_LOAD_NO_RECURSE = 0x400;
        public const int FT_LOAD_IGNORE_TRANSFORM = 0x800;
        public const int FT_LOAD_MONOCHROME = 0x1000;
        public const int FT_LOAD_LINEAR_DESIGN = 0x2000;
        public const int FT_LOAD_NO_AUTOHINT = (int)0x8000U;

        public const int GLYPH_FORMAT_NONE = (int)((uint)0 << 24 | (uint)0 << 16 | (uint)0 << 8 | (uint)0);
        public const int GLYPH_FORMAT_COMPOSITE = (int)((uint)'c' << 24 | (uint)'o' << 16 | (uint)'m' << 8 | (uint)'p');
        public const int GLYPH_FORMAT_BITMAP = (int)((uint)'b' << 24 | (uint)'i' << 16 | (uint)'t' << 8 | (uint)'s');
        public const int GLYPH_FORMAT_OUTLINE = (int)((uint)'o' << 24 | (uint)'u' << 16 | (uint)'t' << 8 | (uint)'l');
        public const int GLYPH_FORMAT_PLOTTER = (int)((uint)'p' << 24 | (uint)'l' << 16 | (uint)'o' << 8 | (uint)'t');


        // Callbacks declaration for outline decomposition

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int OutlineMoveToFunc(IntPtr to, IntPtr data);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int OutlineLineToFunc(IntPtr to, IntPtr data);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int OutlineConicToFunc(IntPtr control, IntPtr to, IntPtr data);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int OutlineCubicToFunc(IntPtr c1, IntPtr c2, IntPtr to, IntPtr data);

        #region "Data Structures"

        // Data Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct FT_Vector
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_BBox
        {
            public int xMin;
            public int yMin;
            public int xMax;
            public int yMax;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct FT_Generic
        {
            public IntPtr data;
            public IntPtr finalizer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_FaceRec
        {
            public int num_faces;
            public int face_index;

            public int face_flags;
            public int style_flags;

            public int num_glyphs;


            public string family_name;
            public string style_name;

            /*
            public IntPtr family_name;
            public IntPtr style_name;
            */

            public int num_fixed_sizes;
            public IntPtr available_sizes;

            public int num_charmaps;
            public IntPtr charmaps;

            public FT_Generic generic;


            /*# The following member variables (down to `underline_thickness') */
            /*# are only relevant to scalable outlines; cf. @FT_Bitmap_Size    */
            /*# for bitmap fonts.                                              */

            public FT_BBox bbox;

            public ushort units_per_EM;
            public short ascender;
            public short descender;
            public short height;

            public short max_advance_width;
            public short max_advance_height;

            public short underline_position;
            public short underline_thickness;

            public IntPtr glyph;


            public IntPtr size;
            public IntPtr charmap;

            /*@private begin */

            IntPtr driver;
            IntPtr memory;
            IntPtr stream;

            IntPtr sizes_list;

            FT_Generic autohint;
            IntPtr extensions;

            IntPtr _internal;

            /*@private end */

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_Glyph_Metrics
        {
            public int width;
            public int height;

            public int horiBearingX;
            public int horiBearingY;
            public int horiAdvance;

            public int vertBearingX;
            public int vertBearingY;
            public int vertAdvance;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_Bitmap
        {
            public int rows;
            public int width;
            public int pitch;
            public IntPtr buffer;
            public short num_grays;
            public char pixel_mode;
            public char palette_mode;
            public IntPtr palette;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_GlyphSlotRec
        {
            public IntPtr library;
            public IntPtr face;
            public IntPtr next;
            public uint reserved;       /* retained for binary compatibility */
            public FT_Generic generic;

            public FT_Glyph_Metrics metrics;
            public int linearHoriAdvance;
            public int linearVertAdvance;
            public FT_Vector advance;

            public int format;

            public FT_Bitmap bitmap;
            public int bitmap_left;
            public int bitmap_top;

            public FT_Outline outline;

            public uint num_subglyphs;
            public IntPtr subglyphs;

            public IntPtr control_data;
            long control_len;

            public int lsb_delta;
            public int rsb_delta;

            public IntPtr other;

            public IntPtr _internal;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct FT_OutlineGlyphRec
        {
            public IntPtr library;
            public IntPtr clazz;
            public int format;
            public FT_Vector advance;
            public FT_Outline outline;
        }

        public const int OUTLINE_OFFSET = 20;

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_Outline
        {
            public short n_contours;      /* number of contours in glyph        */
            public short n_points;        /* number of points in the glyph      */

            public IntPtr points;          /* the outline's points               */
            public IntPtr tags;            /* the points flags                   */
            public IntPtr contours;        /* the contour end points             */

            public int flags;           /* outline masks                      */

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FT_Outline_Funcs
        {
            public IntPtr move_to;
            public IntPtr line_to;
            public IntPtr conic_to;
            public IntPtr cubic_to;

            public int shift;
            public int delta;
        }


        #endregion

        // Helper functions

        // Convert a IntPtr to a structure
        // Copy data from unmanaged memory into managed space
        public static Rec HandleToRecord<Rec>(IntPtr handle)
        {
            return (Rec)Marshal.PtrToStructure(handle, typeof(Rec));
        }


        // convert a 26.6 value into the usual double representation
        public static double F26Dot6toDouble(int x)
        {
            return (double)(x / 64) + (double)(x % 64) / 64.0;
        }

        // convert a 26.6 value into the usual float representation
        public static float F26Dot6toFloat(int x)
        {
            return (float)(x / 64) + (float)(x % 64) / 64.0f;
        }

        static FT()
        {
            initErrors();
        }

        /* check that freetype nativ lib is accessible
         * WARNING: it seems that it cannot be tested more that once per runtime
         *          if freetype is not found, then the dll is placed in search path the application must restart to find it
         * todo: try to avoid this limitation ...
         */
        public static bool CheckForFreetype()
        {
            try
            {
                IntPtr lib;
                int code = FT_Init_FreeType(out lib);
                FT_Done_FreeType(lib);
                return true;
            }
            catch (Exception) // DllNotFound
            {
                return false;
            }

        }

        public const string FT_DLL = "freetype248";

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Init_FreeType(out IntPtr library);

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Done_FreeType(IntPtr library);

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_New_Face(IntPtr library, string fname, int index, out IntPtr face);

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Done_Face(IntPtr face);

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Set_Char_Size(IntPtr face, int charwidth, int charheight, int horzres, int vertres);


        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Set_Pixel_Size(IntPtr face, int width, int height);


        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Get_Char_Index(IntPtr face, int code);

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Load_Glyph(IntPtr face, int index, int flags);


        // same as getcharindex + loadglyph
        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Load_Char(IntPtr face, uint char_code, int load_flags);


        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Get_Glyph(IntPtr slot, out IntPtr glyph);


        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FT_Done_Glyph(IntPtr glyph);

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Glyph_Copy(IntPtr src, out IntPtr dest);

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Glyph_Get_CBox(IntPtr glyph, int bboxmode, out IntPtr cbox);

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Outline_Decompose(IntPtr outline, ref FT_Outline_Funcs func_interface, IntPtr user);
        
        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int FT_Outline_Embolden(IntPtr outline, int strength);
        
        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FT_Glyph_Get_CBox(IntPtr glyph, int bbox_mode, out FT_BBox obbox);

        [DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void FT_Get_Kerning(IntPtr face, int left, int right, int kernmode, out FT_Vector kerning);

       


        // Additional native Freetype helper functions
        //[DllImport(FT_DLL, CallingConvention = CallingConvention.Cdecl)]
        //public static extern IntPtr fthelper_glyph_get_outline_address(IntPtr glyph);

        // .Net implementation
        public static IntPtr fthelper_glyph_get_outline_address(IntPtr glyph)
        {
            return new IntPtr(glyph.ToInt32() + OUTLINE_OFFSET);
        }


        #region "Error Handling"

        static Dictionary<int, string> errorStrings;

        static void initErrors()
        {
            errorStrings = new Dictionary<int, string>();

            errorStrings[0x00] = "no error";

            errorStrings[0x01] = "cannot open resource";
            errorStrings[0x02] = "unknown file format";
            errorStrings[0x03] = "broken file";
            errorStrings[0x04] = "invalid FreeType version";
            errorStrings[0x05] = "module version is too low";
            errorStrings[0x06] = "invalid argument";
            errorStrings[0x07] = "unimplemented feature";
            errorStrings[0x08] = "broken table";
            errorStrings[0x09] = "broken offset within table";

            errorStrings[0x10] = "invalid glyph index";
            errorStrings[0x11] = "invalid character code";
            errorStrings[0x12] = "unsupported glyph image format";
            errorStrings[0x13] = "cannot render this glyph format";
            errorStrings[0x14] = "invalid outline";
            errorStrings[0x15] = "invalid composite glyph";
            errorStrings[0x16] = "too many hints";
            errorStrings[0x17] = "invalid pixel size";

            errorStrings[0x20] = "invalid object handle";
            errorStrings[0x21] = "invalid library handle";
            errorStrings[0x22] = "invalid module handle";
            errorStrings[0x23] = "invalid face handle";
            errorStrings[0x24] = "invalid size handle";
            errorStrings[0x25] = "invalid glyph slot handle";
            errorStrings[0x26] = "invalid charmap handle";
            errorStrings[0x27] = "invalid cache manager handle";
            errorStrings[0x28] = "invalid stream handle";

            errorStrings[0x30] = "too many modules";
            errorStrings[0x31] = "too many extensions";

            errorStrings[0x40] = "out of memory";
            errorStrings[0x41] = "unlisted object";

            errorStrings[0x51] = "cannot open stream";
            errorStrings[0x52] = "invalid stream seek";
            errorStrings[0x53] = "invalid stream skip";
            errorStrings[0x54] = "invalid stream read";
            errorStrings[0x55] = "invalid stream operation";
            errorStrings[0x56] = "invalid frame operation";
            errorStrings[0x57] = "nested frame access";
            errorStrings[0x58] = "invalid frame read";

            errorStrings[0x60] = "raster uninitialized";
            errorStrings[0x61] = "raster corrupted";
            errorStrings[0x62] = "raster overflow";
            errorStrings[0x63] = "negative height while rastering";

            errorStrings[0x70] = "too many registered caches";

            errorStrings[0x80] = "invalid opcode";
            errorStrings[0x81] = "too few arguments";
            errorStrings[0x82] = "stack overflow";
            errorStrings[0x83] = "code overflow";
            errorStrings[0x84] = "bad argument";
            errorStrings[0x85] = "division by zero";
            errorStrings[0x86] = "invalid reference";
            errorStrings[0x87] = "found debug opcode";
            errorStrings[0x88] = "found ENDF opcode in execution stream";
            errorStrings[0x89] = "nested DEFS";
            errorStrings[0x8A] = "invalid code range";
            errorStrings[0x8B] = "execution context too long";
            errorStrings[0x8C] = "too many function definitions";
            errorStrings[0x8D] = "too many instruction definitions";
            errorStrings[0x8E] = "SFNT font table missing";
            errorStrings[0x8F] = "horizontal header (hhea) table missing";
            errorStrings[0x90] = "locations (loca) table missing";
            errorStrings[0x91] = "name table missing";
            errorStrings[0x92] = "character map (cmap) table missing";
            errorStrings[0x93] = "horizontal metrics (hmtx) table missing";
            errorStrings[0x94] = "PostScript (post) table missing";
            errorStrings[0x95] = "invalid horizontal metrics";
            errorStrings[0x96] = "invalid character map (cmap) format";
            errorStrings[0x97] = "invalid ppem value";
            errorStrings[0x98] = "invalid vertical metrics";
            errorStrings[0x99] = "could not find context";
            errorStrings[0x9A] = "invalid PostScript (post) table format";
            errorStrings[0x9B] = "invalid PostScript (post) table";

            errorStrings[0xA0] = "opcode syntax error";
            errorStrings[0xA1] = "argument stack underflow";
            errorStrings[0xA2] = "ignore";

            errorStrings[0xB0] = "`STARTFONT' field missing";
            errorStrings[0xB1] = "`FONT' field missing";
            errorStrings[0xB2] = "`SIZE' field missing";
            errorStrings[0xB3] = "`CHARS' field missing";
            errorStrings[0xB4] = "`STARTCHAR' field missing";
            errorStrings[0xB5] = "`ENCODING' field missing";
            errorStrings[0xB6] = "`BBX' field missing";

            // Custom errors

            errorStrings[0xD0] = "Bad glyph format";
        }


        public static void CheckError(int code)
        {
            if (code != 0)
            {
                throw new FTError(code);
            }
        }

        public static string ErrorMessage(int code)
        {
            if (errorStrings.ContainsKey(code))
            {
                return errorStrings[code];
            }
            else
            {
                return "Unexpected Error";
            }
        }

        #endregion

    }
}

