// MODIFIED BY B.NOUVEL TO BE LINKED WITH TTF TEXT

/*
 * Copyright (c) 2012 Mario Freitas (imkira@gmail.com)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

 
      //com.computerdreams.ttftext.androidsysfont.AndroidSysFont
package com.computerdreams.ttftext.androidsysfont;

import android.app.Activity;
import android.util.Log;

import android.graphics.Typeface;
import android.graphics.Paint;
import android.graphics.Path;
import android.graphics.PathMeasure;
//import android.graphics.Bitmap;
//import android.graphics.Canvas;

import android.text.Layout;
import android.text.StaticLayout;
import android.text.TextPaint;


//import com.unity3d.player.UnityPlayer;
import java.util.Map;
import java.util.HashMap;
import java.util.Iterator;



import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.RandomAccessFile;
import java.util.HashMap;
 




public class AndroidSysFont {
  private static AndroidSysFont instance = null;
  
  Paint paint;
  HashMap< String, String > fontlisting;

  
    public static String GetTtfFontFamilyName( String fontFilename )
    {
      Typeface typeface = Typeface.createFromFile(fontFilename );
      //return typeface.familyName;
      return fontFilename;
    }
  
    static public HashMap< String, String > enumerateFonts() {
        String[] fontdirs = { "/system/fonts", "/system/font", "/data/fonts", "/Android/data/fonts" };
        HashMap< String, String > fonts = new HashMap< String, String >();
        
 
        for ( String fontdir : fontdirs )
        {
            File dir = new File( fontdir );
 
            if ( !dir.exists() )
                continue;
 
            File[] files = dir.listFiles();
 
            if ( files == null )
                continue;
 
            for ( File file : files )
            {
                String fontname = GetTtfFontFamilyName( file.getAbsolutePath() );
 
                if ( fontname != null ) {
                    fonts.put( fontname, file.getAbsolutePath() );
                }
            }
        }
 
        return  fonts;
    }
 
  
  public AndroidSysFont() {
    paint=new Paint();    
    fontlisting=enumerateFonts();
    lastfontname="@@@~~none@~@~@";
    lastit=lastbld=false;
  }

  
  String lastfontname; boolean lastit; boolean lastbld;
  Typeface lastopened;
  
  public  Typeface GetTypeface( String fontName, boolean isItalic, boolean isBold) {
    if ((fontName==lastfontname) 
    && (isItalic=lastit)	
      && (lastbld=isBold)) {
      return lastopened;
    }
  
    Typeface typeface = null;
    
    int style = Typeface.NORMAL;

    if (isBold == true) {
      style |= Typeface.BOLD;
    }
    if (isItalic == true) {
      style |= Typeface.ITALIC;
    }

    if (fontName.length() > 0) {
       if (fontName == "default") {
	typeface = Typeface.defaultFromStyle(style);
      }
      else {
        typeface = Typeface.create(fontName, style);
      }
    }
    if (typeface == null) {
	// DEFAULT         = create((String)null, 0);
        // DEFAULT_BOLD    = create((String)null, Typeface.BOLD);
        // SANS_SERIF      = create("sans-serif", 0);
        // SERIF           = create("serif", 0);
        // MONOSPACE       = create("monospace", 0);
    
      typeface = Typeface.defaultFromStyle(style);
    }
    
    lastfontname=fontName; 
    lastit=isItalic; 
    lastbld=isBold;
    lastopened=typeface;
    
    return typeface;
  }

  
  public Path GetGlyph(Typeface tf,char c) {
     if ((paint.getTypeface()!=tf)||(paint.getTextSize()!=1)) {
       paint.setTypeface(tf);
       paint.setTextSize(1);
     }
     Path p = new Path();
     paint.getTextPath(""+c,0,1,0,0,p);
     return p;
  }
  
  public float GetGlyphWidth (Typeface tf, char c) {
    if (tf==null) return 0;
     if ((paint.getTypeface()!=tf)||(paint.getTextSize()!=1)) {
       paint.setTypeface(tf);
       paint.setTextSize(1);
     }    
     float [] f= new float [1];
     paint.getTextWidths	(""+c,0,1,f);
     return f[0];
     
     //String s=""+c;
     //return paint.measureText(s,0,s.length());
  }
  public Path GetGlyph(Typeface tf,String c) {
     if ((paint.getTypeface()!=tf)||(paint.getTextSize()!=1)) {
       paint.setTypeface(tf);
       paint.setTextSize(1);
     }
     Path p = new Path();
     paint.getTextPath(""+c,0,1,0,0,p);
     return p;
  }
  
  public android.graphics.Paint.FontMetrics GetFontMetrics(Typeface tf) {
     if ((paint.getTypeface()!=tf)||(paint.getTextSize()!=1)) {
       paint.setTypeface(tf);
       paint.setTextSize(1);
     }  
    return paint.getFontMetrics();
  }
  
  public float GetGlyphWidth (Typeface tf, String c) {
    if (tf==null) return 0;
     if ((paint.getTypeface()!=tf)||(paint.getTextSize()!=1)) {
       paint.setTypeface(tf);
       paint.setTextSize(1);
     }    
     
     float [] f= new float [1];
     paint.getTextWidths(c,0,1,f);
     return f[0];
   
 //    return paint.measureText(c,0,c.length());

  }
  
  public PathMeasure GetPathMeasure(Path p) {
    if (p==null) return null;
    return new PathMeasure( p,false);
  }
  
  public boolean NextBoundary(PathMeasure pm) {
    return pm.nextContour();
  }
  
  public float [] GetBoundary(PathMeasure pm , int n) {
    // 2n values
    if (pm==null) return null;
    float [] td = new float[2];
    float len=pm.getLength();
    int ns=64;
    float [] res=new float[ns*2];
    
    for (int i=0;i<ns;i++) {
      pm.getPosTan((((float)i)/((float)ns))*len,td,null);
      res[i*2]=td[0];
      res[i*2+1]=-td[1];
    }
    
    
    return res;
  }
  
  public String [] EnumFonts() {
      String [] res=new String[fontlisting.size()];
      fontlisting.keySet().toArray(res);
      return res;
  }
  
  public static synchronized AndroidSysFont GetInstance() {
    if (instance == null) {
      instance = new AndroidSysFont();
    }
    return instance;
  }

  public static synchronized AndroidSysFont getInstance() {
    if (instance == null) {
      instance = new AndroidSysFont();
    }
    return instance;
  }

}
  
