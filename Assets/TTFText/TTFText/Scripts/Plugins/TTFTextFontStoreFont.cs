using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTF = FTSharp;



[System.Serializable]
public class TTFTextFontStoreFont {	
#if !TTFTEXT_LITE							
	public string fontid;

	private GameObject [] clients; // (DEPRECATED) if this list becomes empty then the font may be discarded	
	public string source=null;     // (UNUSED) where the font may be found if required (either local file / either URL)
	
	public bool orientationReversed; // THIS SHOULD BECOME DEPRECATED (CK) (LEGACY)
	public float height=1;
	private bool _needRebuild=false;
	public bool needRebuild { get { return _needRebuild;}}
	
	
	/// <summary>
	/// Basic ASCII Character set
	/// </summary>
	[SerializeField] public TTFTextOutline [] charset=null;

    /// <summary>
    /// Non ASCCI characters
    /// </summary>
    [SerializeField] 
	public string additionalChar = "";

    /// <summary>
    /// Non ASCCI character glyphs
    /// </summary>	
    [SerializeField] 
	public TTFTextOutline[] addCharset = null;

	//[SerializeField] private List<List<Vector3>> [] charset_boundaries=null;
	[SerializeField] private Vector3 [] charset_advance=null;
	
	

	
	
	#region REFERENCE COUNTING	
	public int refcount=0;
	/// <summary>
	/// Increments the reference counts
	/// </summary>
	public void incref() {
		refcount++;
	}
	
	
	/// <summary>
	/// Decrease the reference count
	/// </summary>
	/// TODO: DEPRECATE	
	public void decref() {
		refcount--;
		if (refcount <= 0) {
			if (TTFTextFontStore.IsInstanciated) {
				TTFTextFontStore.Instance.SetGarbageCollectUnusedFonts();
			}
			//GameObject.Find("/TTFText Font Store").SendMessage("SetGarbageCollectUnusedFonts",SendMessageOptions.DontRequireReceiver);
		}
	}
	
	/// <summary>
	/// Return the reference count of current font.
	/// </summary>
	/// TODO: DEPRECATE
	public int GetRefCount() {return refcount;}
	#endregion
	
		
	/// <summary>
	/// Determines whether this embedded has the character c.
	/// </summary>
	public bool HasCharacter(char c) {
		if (c >= 0x20 && c < 0x7f) { // Ascii charset
			return true;
		} else { 
			int idx = additionalChar.IndexOf(c);
			return (idx >= 0 && idx < addCharset.Length);
		}
	}

	/// <summary>
	/// Adds some special characters
	/// </summary>
	public void AddRequiredCharacters(string s) {
		foreach(char  c in s) {
			if (!(c >= 0x20 && c < 0x7f)) { // Ascii charset
				if (additionalChar.IndexOf(c)==-1) {
					additionalChar+=s;
					_needRebuild=true;
				}
			}
		}
	}
	
	/// <summary>
	/// Gives a small idea of the complexity of the font
	/// </summary>	
	public int BoundaryMemoryUsage() {
		int sz=0;
		if (charset!=null) {
			foreach(TTFTextOutline o in charset) {
				if ((o!=null)&&(o.blengthes!=null)) {
					foreach(int l in o.blengthes) sz+=l;
				}
			}
		}
		if (addCharset!=null) {
			foreach(TTFTextOutline o in addCharset) {
				if ((o!=null)&&(o.blengthes!=null)) {
					foreach(int l in o.blengthes) sz+=l;
				}
			}
		}
		return sz*8; // 2 times 4 bytes for each point
	}
	
	
	/// <summary>
	/// Tries to build a whole charset according current parameters of a textmesh object
	/// </summary>
	public void BuildCharSet(TTFText tm) {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
		int istep=TTFTextFontStore.Instance.defaultInterpolationSteps;
		charset = new TTFTextOutline[0x80];
		charset_advance = new Vector3[0x80];
		System.Text.ASCIIEncoding ae = new System.Text.ASCIIEncoding();
		
        TTF.Font font = TTFTextInternal.Utilities.TryOpenFont(tm.InitTextStyle,1);

        if (font == null) {
            Debug.LogError("(TTFText) BuildCharSet: no font found");
            return;
        }
		height=font.Height;
        for (byte i = 0x20; i < 0x7F; i++) {
			
            string s = ae.GetString(new byte [] {i});
            charset[i] = TTFTextInternal.Engine.MakeNativeOutline(s, 1,0,font,tm.OrientationReversed,istep);
            charset_advance[i] = charset[i].advance;
        }


        // Additional Custom Characters
		AddRequiredCharacters(TTFTextFontStore.Instance.defaultAdditionalCharacters + (tm.InitTextStyle.GetFontEngineParameters(1) 
			as TTFTextInternal.FontStoreFontEngine.Parameters).additionalCharacters);
		
        addCharset = new TTFTextOutline[additionalChar.Length];

        int idx = 0;
        foreach (char c in additionalChar) {
            addCharset[idx] = TTFTextInternal.Engine.MakeNativeOutline("" + c,  1, 0, font,tm.OrientationReversed,istep);
            ++idx;
        }

        font.Dispose();
		_needRebuild=false;
#endif				
	}
	
	

	/// <summary>
	/// Builds a whole charset for a specific fontid
	/// </summary>
	public void BuildCharSet(string fontid) {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
		int istep=TTFTextFontStore.Instance.defaultInterpolationSteps;
		bool reversed=false;
		charset = new TTFTextOutline[0x80];
		charset_advance = new Vector3[0x80];
		System.Text.ASCIIEncoding ae = new System.Text.ASCIIEncoding();
		
        TTF.Font font = TTFTextInternal.Utilities.TryOpenFont(fontid,ref reversed,"",72); // TODO: < Check the last bool

        if (font == null) {
            Debug.LogError("BuildCharSet: no font found");
            return;
        }

        for (byte i = 0x20; i < 0x7F; i++) {
			
            string s = ae.GetString(new byte [] {i});
            charset[i] = TTFTextInternal.Engine.MakeNativeOutline(s, 1,0,font,reversed,istep);
			//TTFTextInternal.MakeOutline(s, font, 1, 0, null,false,null,null);
            charset_advance[i] = charset[i].advance;
        }


        addCharset = new TTFTextOutline[additionalChar.Length];

        int idx = 0;
        foreach (char c in additionalChar) {
            addCharset[idx] = TTFTextInternal.Engine.MakeNativeOutline(
												  "" + c, 1,0,font,
												  reversed,istep);
            ++idx;
        }

        font.Dispose();
		_needRebuild=false;
#endif		
		
	}
	
	
	/// <summary>
	/// Gets the glyph outline associated with a character
	/// </summary>
	public TTFTextOutline GetGlyph(char c) {		
		TTFTextOutline o;
		
		if (c >= 0x20 && c < 0x7f) { // Ascii charset
					    o = charset[(byte)c];
        } else { // try custom char list
                        int idx = additionalChar.IndexOf(c);
                        if (idx >= 0 && idx < addCharset.Length) {
							o = addCharset[idx];                        
					    } else { // fallback on space char
                            Debug.LogWarning("Character '" + c + "' not found. Consider adding it in additonnal char text field.");
							o = charset[(byte) ' '];
                        }
         }
		
		return o;
	}
	
	
	/// <summary>
	/// Returns the advance vector associated with a character
	/// </summary>
	public Vector3 GetAdvance(char c) {		
		if (c >= 0x20 && c < 0x7f) { // Ascii charset
					    return charset_advance[c];
        } else { // try custom char list
                        int idx = additionalChar.IndexOf(c);
                        if (idx >= 0 && idx < addCharset.Length) {
							return addCharset[idx].advance;                        
					    } else { // fallback on space char
                            Debug.LogWarning("Character '" + c + "' not found. Consider adding it in additonnal char text field.");
							return charset[(byte) ' '].advance;
                        }
         }
	}
	
	
#endif
}



