using UnityEngine;


namespace TTFTextInternal {
#region FUNCTIONS_RELATED_TO_SHADERMAPS
	
	
public static class ShaderMaps {	
	/// <summary>
	/// Compute the UV map for a specific mesh
	/// </summary>
	/// <param name='mesh'>
	/// Mesh.
	/// </param>
	/// <param name='uvType'>
	/// Uv type.
	/// </param>
	/// <param name='normalized'>
	/// Normalized.
	/// </param>
	/// <param name='uvscale'>
	/// Uvscale.
	/// </param>
	/// <param name='charmetadata'>
	/// Charmetadata.
	/// </param>
	public static void ComputeUVs2 (Mesh mesh, TTFText.UVTypeEnum uvType, bool normalized, Vector3 uvscale, object charmetadata)
	{
		mesh.RecalculateBounds();
		Bounds b=mesh.bounds;
		//Vector3 e = b.extents;
		Vector3 e = b.size;
		Vector3 c = b.center;
		Vector3 invse = new Vector3 (
				(e.x != 0) ? (1f / e.x) : 1f, 
				(e.y != 0) ? (1f / e.y) : 1f, 
				(e.z != 0) ? (1f / e.z) : 1f
				);
		//Debug.Log(invse);
		Vector3[] vertices = mesh.vertices;
		
		Vector2[] uvs = new Vector2[vertices.Length];
		
		Vector3 a;		
		
		if (vertices.Length == 0) {
			return;
		}
			
		
		
		
	    if ((charmetadata!=null)) {
			if (((charmetadata as NamedTextureElement)!=null)) {
			NamedTextureElement tbi=(charmetadata as NamedTextureElement);				
		    	for (int i = 0; i < vertices.Length; ++i) {			
					//Debug.Log(vertices [i]-b.min);
					a = Vector3.Scale (vertices [i]-b.min, invse);			
					uvs[i].x = tbi.UVstartx+ (a.x * tbi.UVwidth);
					uvs[i].y = tbi.UVstarty+ (a.y * tbi.UVheight);
		    	}
			}
			if (((charmetadata as TextureElement)!=null)) {
			    TextureElement tbi=(charmetadata as TextureElement);				
		    	for (int i = 0; i < vertices.Length; ++i) {			
					a = Vector3.Scale (vertices [i]-b.min, invse);			
					//Debug.Log(a);
					uvs [i].x = tbi.UVstartx+ a.x * tbi.UVwidth;
					uvs [i].y = tbi.UVstarty+ a.y * tbi.UVheight;
		    	}
			}
			
		}
		else {
		  for (int i = 0; i < vertices.Length; ++i) {
			
			if (uvType == TTFText.UVTypeEnum.Box) {
				a = vertices [i];
				if (normalized) {
					a = Vector3.Scale (vertices [i], invse);
				}
			} else { // Spherical
				a = Quaternion.FromToRotation (Vector3.forward, vertices [i] - c).eulerAngles;
				a /= 360;//(Mathf.PI*2);
			}
			
			uvs [i].x = (uvscale.x * a.x + uvscale.z * a.z);
			uvs [i].y = (uvscale.y * a.y + uvscale.z * a.z);
		  }
		}
		
		mesh.uv = uvs;
	}

	

	
	
	
	
	// Compute Normals for the given mesh
	// currently overrides computed value with (0,0,-1) for front side and (0, 0, 1) for back side
	// perhaps this is not a good idea for contour vertices?
	public static void RecalculateNormals (Mesh mesh, int frontCap, int backCap, int size)
	{
		mesh.RecalculateNormals ();
		Vector3[] normals = mesh.normals;
		for (int idx = frontCap; idx < frontCap + size; ++idx) {
			normals [idx] = - Vector3.forward;
		}
		for (int idx = backCap; idx < backCap + size; ++ idx) {
			normals [idx] = Vector3.forward;
		}
		mesh.normals = normals;
	}
	
	
	
	// Compute Mesh Tangents
	// uv and normals array should already be assigned
	// Taken from http://answers.unity3d.com/questions/7789/calculating-tangents-vector4.html
	// which is derived from
	// Lengyel, Eric. 
	//	“Computing Tangent Space Basis Vectors for an Arbitrary Mesh”. Terathon Software 3D Graphics Library, 2001.
	// [url]http://www.terathon.com/code/tangent.html[/url]
	
	public static void ComputeTangents (Mesh mesh)
	{

		//speed up math by copying the mesh arrays
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
	
		//variable definitions
		int triangleCount = triangles.Length;
		int vertexCount = vertices.Length;
	
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
	
		Vector4[] tangents = new Vector4[vertexCount];
	
		for (int a = 0; a < triangleCount; a += 3) {
			int i1 = triangles [a + 0];
			int i2 = triangles [a + 1];
			int i3 = triangles [a + 2];
	
			Vector3 v1 = vertices [i1];
			Vector3 v2 = vertices [i2];
			Vector3 v3 = vertices [i3];
	
			Vector2 w1 = uv [i1];
			Vector2 w2 = uv [i2];
			Vector2 w3 = uv [i3];
	
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
	
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
	
			float r = 1.0f / (s1 * t2 - s2 * t1);
	
			Vector3 sdir = new Vector3 ((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3 ((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
	
			tan1 [i1] += sdir;
			tan1 [i2] += sdir;
			tan1 [i3] += sdir;
	
			tan2 [i1] += tdir;
			tan2 [i2] += tdir;
			tan2 [i3] += tdir;
		}
	
	
		for (int a = 0; a < vertexCount; ++a) {
			
			Vector3 n = normals [a];
			Vector3 t = tan1 [a];
	
			//Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			//tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			Vector3.OrthoNormalize (ref n, ref t);
			tangents [a].x = t.x;
			tangents [a].y = t.y;
			tangents [a].z = t.z;
	
			tangents [a].w = (Vector3.Dot (Vector3.Cross (n, t), tan2 [a]) < 0.0f) ? -1.0f : 1.0f;
		}
	
		mesh.tangents = tangents;
	}
#endregion
	}
}