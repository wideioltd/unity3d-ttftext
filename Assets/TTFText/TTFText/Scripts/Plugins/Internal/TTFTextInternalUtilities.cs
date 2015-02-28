using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
using TTF = FTSharp;
#endif


namespace TTFTextInternal {
	public static class Utilities {
		
		
		

	
	/// <summary>
	/// Utility function use to serializes an object.
	/// Sometime Unity serialization just don't work as we expect
	/// </summary>
	/// <returns>
	/// The object.
	/// </returns>
	/// <param name='o'>
	/// O.
	/// </param>
	public static byte [] SerializeObject(object o) {
		if (o==null) return new byte[0]{};
    	if (!o.GetType().IsSerializable) { return null; }

    	using (MemoryStream stream = new MemoryStream())  {
        	new BinaryFormatter().Serialize(stream, o);
        	return stream.ToArray();
    	}
	}
	
	/// <summary>
	/// Utility function used to deserializes the object serialized by the previous function
	/// </summary>
	/// <returns>
	/// The object.
	/// </returns>
	/// <param name='bytes'>
	/// Bytes.
	/// </param>
	public static object DeserializeObject(byte [] bytes) {
		if (bytes.Length==0) return null;
		using (MemoryStream stream = new MemoryStream(bytes))
    	{
			try {
        		return new BinaryFormatter().Deserialize(stream);
			}
			catch (System.Exception se) {
					Debug.LogError(se.ToString());
					return null;
				}
    	}
	}

	
	
	// Try Open Font specified by fontId
	// 1. if fontID = "", set it to a sane value first
	// 2. try open it
	// 3. if fail, try fallback fonts
	// 5. if fail return null
	// When no more in use, fhe font should be disposed using font.Dispose() method

	
	static public TTF.Font TryOpenFont (string fontId, ref bool reversed, string runtimeFontFallback, int resolution)
	{

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
        TTFTextFontListManager flm = TTFTextFontListManager.Instance;
        TTF.Font font = null;

        if (flm.Count == 0) { return null; }

        // 1. set fontId to a sane value
        // do not override fontId if it's already set
        if (fontId == "")
        {
            fontId = flm.GetOneId();
        }

        // Try Open it

        font = flm.OpenFont(fontId, 1, ref reversed, resolution);

        if (font != null) { return font; }

        Debug.LogWarning("Font '" + fontId + "' not found.");

#if !TTFTEXT_LITE
        char[] sep = new char[] { ';' };
		
        foreach (string s in runtimeFontFallback.Split(sep, System.StringSplitOptions.RemoveEmptyEntries))
        {

            font = flm.OpenFont(s, 1, ref reversed, resolution);

            if (font != null)
            {
                Debug.Log("Found fallback font " + font.Name);
                return font;
            }
        }
#endif
        // Last resort try another font

        font = flm.OpenFont(flm.GetOneId(), 1, ref reversed, resolution);

        if (font != null)
        {
            Debug.Log("Found fallback font " + font.Name);
            return font;
        }
		
		

#endif
		return null;
	}
	
	
	
	
	
	
	
	
	/// <summary>
	/// Utility function that tries to open a font.
	/// </summary>
	/// <returns>
	/// The open font.
	/// </returns>
	/// <param name='ts'>
	/// Ts.
	/// </param>
	/// <param name='size'>
	/// Size.
	/// </param>
	static public TTF.Font TryOpenFont (TTFTextStyle ts, float size, int resolution)
	{

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR

        TTFTextFontListManager flm = TTFTextFontListManager.Instance;
        TTF.Font font = null;

        if (flm.Count == 0) { return null; }

        // 1. set fontId to a sane value
        // do not override fontId if it's already set
        if (ts.FontId == "")
        {
            ts.FontId = flm.GetOneId();
        }

        // Try Open it

        font = flm.OpenFont(ts.FontId, size, ref ts.orientationReversed,resolution);

        if (font != null) { return font; }

        Debug.LogWarning("Font '" + ts.FontId + "' not found.");


        // Try fallback fonts
#if !TTFTEXT_LITE
        char[] sep = new char[] { ';' };
		
        foreach (string s in ts.runtimeFontFallback.Split(sep, System.StringSplitOptions.RemoveEmptyEntries))
        {

            font = flm.OpenFont(s, size, ref ts.orientationReversed, resolution);

            if (font != null)
            {
                Debug.Log("Found fallback font " + font.Name);
                return font;
            }
        }
#endif
        // Last resort try another font

        font = flm.OpenFont(flm.GetOneId(), size, ref ts.orientationReversed, resolution);

        if (font != null)
        {
            Debug.Log("Found fallback font " + font.Name);
            return font;
        }

#endif
		ts.orientationReversed = false;
		return null;
	}
	
	
	public static TTF.Font TryOpenFont (TTFTextStyle ts, int resolution)
	{
		return TryOpenFont (ts, ts.Size, resolution);
	}	
	
		
		
	static public void DestroyObj (Object o)
	{
		if ((Application.isEditor) && (!(Application.isPlaying))) {
			GameObject.DestroyImmediate (o);
		} else {
			GameObject.DestroyObject (o);
		}
	}

		
	
	static public void ReverseTriangles (Mesh m)
	{
	
		int[] t = m.triangles;
		
		// reverse triangles orientation 
		
		for (int i = 0; i < t.Length; i += 3) {
			int k = t [i + 1];
			t [i + 1] = t [i + 2];
			t [i + 2] = k;
		}
		
		m.triangles = t;
	}
	
	static public void TranslateMesh (Mesh m, Vector3 tr)
	{
		Vector3[] vertices = m.vertices;
		for (int i = 0; i < m.vertexCount; ++i) {
			vertices [i] += tr;
		}
		m.vertices = vertices;
	}
	
		
		/// <summary>
	/// Utility function returns the correct translation according to the 3d attachment of the object
	/// </summary>
	/// <returns>
	/// The object from bounds.
	/// </returns>
	/// <param name='tm'>
	/// Tm.
	/// </param>
	/// <param name='bounds'>
	/// Bounds.
	/// </param>
	public static Vector3 TranslateObjectFromBounds (TTFText tm, Bounds bounds)
	{
		Vector3 tr = Vector3.zero;
		switch (tm.HJust) {
		case TTFText.HJustEnum.Center:
			tr.x = -bounds.center.x;
			break;
		case TTFText.HJustEnum.Left:
			tr.x = -bounds.min.x;
			break;
		case TTFText.HJustEnum.Right:
			tr.x = -bounds.max.x;
			break;
		}
			
		switch (tm.VJust) {
		case TTFText.VJustEnum.Center:
			tr.y = -bounds.center.y;
			break;
		case TTFText.VJustEnum.Top:
			tr.y = -bounds.max.y;
			break;
		case TTFText.VJustEnum.Bottom:
			tr.y = -bounds.min.y;
			break;
		}
		tr.z = - bounds.center.z; 		
		return tr;
	}
	
	
	
	// Merge Submeshes into one
	public static void MergeSubmeshes (Mesh mesh)
	{		
		List<int> triangles = new List<int> ();
		
		for (int i = 0; i < mesh.subMeshCount; ++i) {
			triangles.AddRange (mesh.GetTriangles (i));
		}
			
		mesh.triangles = triangles.ToArray ();
	}
		 
	
	/*
	static int BuildTTFTextInternal.LineLayout(TTFTextInternal.LineLayout ll,  int idx, Vector3 pos, TTFText tm, out Bounds bounds) {
		return BuildTextViaSubObjects(ll,lineSplitters[(int)tm.TokenMode],idx,pos,tm, out bounds);
	}
	*/

	static public void MergeBounds (ref Bounds b, Bounds other)
	{
		if (other.size == Vector3.zero) {
			return;
		}
		if (b.size == Vector3.zero) {
			b = other;
			return;
		}
		b.Encapsulate (other);
	}
		
		

	// THESE FUNCTION MAY BE USE TO CALCULATE AN APPROXIMATIVE BOUNDING BOX FROM FANCY OUTLINES
	public static Vector2 StdDiff (Vector2 [] a)
	{
		Vector2 m = Vector2.zero;
		Vector2 e = Vector2.zero;
		Vector2 f = Vector2.zero;
		if (a.Length == 0)
			return Vector3.zero;	
		Vector2 pv = a [a.Length - 1];
		foreach (Vector2 nv in a) {
			m += nv;
			f.x += (nv.y - pv.y);
			f.y += (nv.x - pv.x);
			e.x += (nv.x + pv.x) * (nv.x + pv.x) * f.x;
			e.y += (nv.y + pv.y) * (nv.y + pv.y) * f.y;
			pv = nv;
		}
		m /= a.Length;
		e = new Vector2 (e.x / (4 * f.x), e.y / (4 * f.y));
		return new Vector2 (Mathf.Sqrt (e.x - m.x * m.x), Mathf.Sqrt (e.y - m.y * m.y));
	}
		
	public static Vector3 StdDiff (Vector3 [] a)
	{
		Vector3 m = Vector3.zero;
		Vector3 e = Vector3.zero;
		Vector3 f = Vector3.zero;
		Vector3 g = Vector3.zero;
		Vector3 sv = Vector3.zero;
		
		if (a.Length == 0)
			return Vector3.zero;	
			
			
		Vector3 pv = a [a.Length - 1];
			
		foreach (Vector3 nv in a) {
			//m+=nv;
			sv = nv + pv;
			f.x = Mathf.Abs (nv.y - pv.y);
			f.y = Mathf.Abs (nv.x - pv.x);
			g += f;
			e.x += sv.x * sv.x * f.x;
			e.y += sv.y * sv.y * f.y;
			m.x += sv.x * f.x;
			m.y += sv.y * f.y;					
			pv = nv;
		}
			
		m = new Vector3 (m.x / 2 * g.x, m.y / 2 * g.y, 0);
		e = new Vector3 (e.x / (4 * g.x), e.y / (4 * g.y), 0);
		return new Vector3 (Mathf.Sqrt (Mathf.Abs (e.x - m.x * m.x)), Mathf.Sqrt (Mathf.Abs (e.y - m.y * m.y)), 0);
	}
		
	public static Vector2 StdXDiff (Vector2 [] a, float deg)
	{
		Vector2 m = Vector2.zero;
		Vector2 e = Vector2.zero;
		Vector2 f = Vector2.zero;
		Vector2 g = Vector2.zero;
		if (a.Length == 0)
			return Vector2.zero;	
		Vector2 pv = a [a.Length - 1];
		foreach (Vector2 nv in a) {
			m += nv;
			f.x = Mathf.Abs (nv.y - pv.y);
			f.y = Mathf.Abs (nv.x - pv.x);
			g += f;
			e.x += Mathf.Pow (Mathf.Abs (nv.x + pv.x), deg) * f.x;
			e.y += Mathf.Pow (Mathf.Abs (nv.y + pv.y), deg) * f.y;
			pv = nv;
		}
		m /= a.Length;
		e = new Vector2 (e.x / (Mathf.Pow (2, deg) * g.x), e.y / (Mathf.Pow (2, deg) * g.y));
		return new Vector2 (Mathf.Pow (e.x - Mathf.Pow (m.x, deg), 1.0f / deg), Mathf.Pow (e.y - Mathf.Pow (m.x, deg), 1.0f / deg));
	}
	
		
		
		
		
		
	}
}
