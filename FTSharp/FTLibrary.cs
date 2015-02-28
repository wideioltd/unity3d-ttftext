using System;

namespace FTSharp
{
    public class FTLibrary
    {
        IntPtr library_;


        private static  FTLibrary instance;
        
        // static singleton constructor
        static FTLibrary() {
            instance = new FTLibrary();
        }

        public static FTLibrary Instance {
            get {
                return instance; 
            }
        }
        
        
        public IntPtr Handle {
            get { return library_; }
        }
        
        private FTLibrary ()
        {
            int code = FT.FT_Init_FreeType(out library_);
            FT.CheckError(code);
        }

        ~FTLibrary()
        {
            //DON'T CALL FT_Done_freetype 
            //cause some ft_face can still remain and are about to be freed
            //FT.FT_Done_FreeType(library_);
        }

        /*
        public void bla()
        {
            Console.WriteLine("FTLibrary handle:" + library_);
        }
          */ 
    }
}

