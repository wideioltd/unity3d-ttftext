//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   

using UnityEngine;
using System.Collections.Generic;

// GLUT requires GLUT thus we probably cannot expect this to work 
// on other platforms to stay on the safe side let's disable if we are not the correct platform
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
using T = Triangulation;
#endif

// Currently buggy (consecutive points at same pos..) and not as well finished as GLUT (Experimental)
#if !TTFTEXT_LITE
using Poly2Tri;
#endif

namespace TTFTextInternal {

public static class Tesselators {


#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
	
	public static Vector3 combineVector3(double[] loc, Vector3[] inVertices, float[] weight) {
		return new Vector3((float) loc[0], (float) loc[1], (float) loc[2]);
	}
	

	//public static Mesh Triangulate(T.Tesselator<Vector3> tess, IEnumerable<Vector3> path, Mesh outMesh) {
	public static Mesh Triangulate(T.Tesselator<Vector3> tess,
		                           IList<Vector3> path, 
		                           Mesh outMesh) {
		tess.BeginPolygon();
		tess.BeginContour();
		int i=0;
		double[][] coords = new double[path.Count][];
		foreach (Vector3 v in path) {
			double [] tv={ v.x, v.y, v.z };
			coords[i]=tv;
			
			tess.AddVertex(coords[i], v);
			i++;
		}
		tess.EndContour();
		tess.EndPolygon();
		
		outMesh.Clear();
		outMesh.vertices = tess.Vertice.ToArray();
		outMesh.triangles = tess.Indices.ToArray();
		
		return outMesh;
	}
	
	
	delegate int combineIdxD(double[] loc, int[] inVertices, float[] weight);
	
	
	
	/*
	public static Mesh Triangulate(IList<Vector3> path, Mesh outMesh 
		
		List<Vector3> vertices = new List<Vector3>(path.Count * 2);
		List<int> triangles = new List<int>();
			Debug.Log("prev tess");		
		T.Tesselator<int>.combineVertexD combineIdx = delegate (double[] loc, int[] inVertices, float[] weight) {

			int idx = vertices.Count;
			
			vertices.Add(new Vector3((float) loc[0], (float) loc[1], (float) loc[2]));
			
			return idx;			
		};
		
		using (T.Tesselator<int> tess = new T.Tesselator<int>(combineIdx)) {
		
			tess.VertexEv += delegate(T.Tesselator<int> t, int idx) {
				triangles.Add(idx);
			};
		
			tess.BeginPolygon();
			tess.BeginContour();
		
			for (int i = 0; i < path.Count; ++i) {
				int idx = vertices.Count;
				Vector3 v = path[i];
				vertices.Add(v);
			//	uv.Add(inUV[i]);
				double[] coords = { v.x, v.y, v.z };
				tess.AddVertex(coords, idx);
			}
		
			tess.EndContour();
			tess.EndPolygon();
		}
		
		outMesh.Clear();
		outMesh.vertices = vertices.ToArray();
		outMesh.triangles = triangles.ToArray();
		//outMesh.uv = uv.ToArray();
		
		return outMesh;
	}

	
	public static Mesh Triangulate(IList<Vector3> path) {
		Mesh mesh = new Mesh();
		mesh.name = "triangulation";
		return Triangulate(path, mesh);		
	}
	
	*/
	
	public static Mesh TTriangulate(TTFTextOutline outline, Mesh outMesh) {
		
		List<Vector3> vertices = new List<Vector3>(outline.numVertices * 2);
		List<int> triangles = new List<int>();
		
		T.Tesselator<int>.combineVertexD combineIdx = 
		  delegate (double[] loc, int[] inVertices, float[] weight) {		
			 int idx = vertices.Count;			
			 vertices.Add(new Vector3((float) loc[0], (float) loc[1], (float) loc[2]));			
			 return idx;			
		  }; 
		
		
		using (T.Tesselator<int> tess = new T.Tesselator<int>(combineIdx)) {
			
			//Debug.Log("SetWindingRule");
			tess.SetWindingRule(Triangulation.Glu.TessWinding.WindingPositive);
			
			tess.VertexEv += delegate(T.Tesselator<int> t, int idx) {
				triangles.Add(idx);
			};
			
			
			
			double [][][] tcoords=new double[outline.blengthes.Count][][];
			
			
			
			//using () {
			tess.BeginPolygon();
//			int i=0;
			int cp=0;
			//Debug.Log ("Tess Beg Poly");
			foreach(TTFTextOutline.Boundary lv in outline.boundaries) {				
				tcoords[cp]=new double[lv.Count][];
				int ci=0;
				tess.BeginContour();
				foreach (Vector3 v in lv) {
					int idx = vertices.Count;
					vertices.Add(v);	
					//double[] coords = { v.x, v.y, v.z };
					double [] ttcoords= { v.x, v.y, v.z };
					tcoords[cp][ci] =ttcoords;
					tess.AddVertex(tcoords[cp][ci], idx);
					//tess.AddVertex(coords, idx);
					//i++;
					ci++;
				}
				
				tess.EndContour(); 
				cp++;
			}
			//Debug.Log ("Tess End Poly");
			tess.EndPolygon();
				
			//for (int ix=0;ix<tcoords.Length;ix++) {
			//	tcoords[ix]=null;
			//}
					
			//}
		}
		
		
		outMesh.Clear();
		outMesh.vertices = vertices.ToArray();
		outMesh.triangles = triangles.ToArray();
		//outMesh.uv = uv.ToArray();
			
		return outMesh;
	}
#endif	
	


	public static Mesh Triangulate(TTFTextOutline outline, bool expecode) {
		Mesh mesh = new Mesh();
#if !TTFTEXT_LITE
		if (expecode) {
			return P2TTriangulate(outline, mesh);
		}
		else {
#endif
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
			return TTriangulate(outline, mesh);			
#else
			Debug.LogError("Triangulate need to be called with Poly2Tri Lib");		
			return null;			
#endif
#if !TTFTEXT_LITE	
		}
#endif
    }

#if !TTFTEXT_LITE	
	public static List<Poly2Tri.PolygonPoint> ToPolyPoint(IEnumerable<Vector3> v) {
		List<Poly2Tri.PolygonPoint> l=new List<Poly2Tri.PolygonPoint>();
		foreach(Vector3 cv in v) {
			l.Add(new Poly2Tri.PolygonPoint((double)cv.x,(double)cv.y));
		}
		//l.RemoveAt(l.Count-1);// REMOVE THE LAST POINT THAT IS ALSO THE FIRST POINT AND THAT WILL CONFUSE THE ALGOS
		return l;
	}
	
	
    static bool PointInPolygon(PolygonPoint p, List<PolygonPoint> poly) {
			// on trace une ligne ligne verticale et on compte le nombre de fois ou on passe une frontiere
            PolygonPoint p1, p2;
            bool inside = false;
		
			// the last point
			//we consider each segment independantly
            PolygonPoint op = new PolygonPoint(poly[poly.Count - 1].X, poly[poly.Count - 1].Y);

            for (int i = 0; i < poly.Count; i++)
            {
                var cp = new PolygonPoint(poly[i].X, poly[i].Y);
		
				// on organize p1 et p2 en sens croissant pour etre sur que l'on se trouvera toujours du meme cote
                if (cp.X > op.X) { p1 = op; p2 = cp; }
                else { p1 = cp; p2 = op; }
				
                if (
				(cp.X < p.X) == (p.X <= op.X)  // one est dans la projection vertical d'un segment du contour
					&& ((p.Y - p1.Y) * (p2.X - p1.X) - (p2.Y - p1.Y) * (p.X - p1.X) >0)
				    
				)
                { inside = !inside; }
                op = cp;
            }
            return inside;
        }
	
	
	
	 static bool CheckOrientation(List<PolygonPoint> poly) {
            double f=0;
			PolygonPoint  cv,nv;
		    cv=poly[0];
            for (int i = 0; i < poly.Count; i++)
            {
                nv = poly[(i+1)%poly.Count];
			     f+=((cv.X * nv.Y) - (nv.X * cv.Y));
			    cv=nv;
            }
            return (f>=0);
        }
	

        static bool CheckIfInside( List<PolygonPoint> contained,  List<PolygonPoint> container)
        {
            int t = 0;
            for (int i = 0; i < contained.Count; ++i) {
                if (PointInPolygon(contained[i], container)) t++;
            }
			//Debug.Log(System.String.Format("t={0} count={1}",t,contained.Count));
            return (((float)t) >= (contained.Count *0.6f));
        }
	
	/*
	
	class CPolygon {
		public Polygon p;
		public List<PolygonPoint> points;
		public List<List<PolygonPoint>> holes;
	}
	
		
	static bool Contained_In_Polygon(List<PolygonPoint> contained, CPolygon polygon)
    {
            if (!CheckIfInside(contained,polygon.points)) {
			//	Debug.Log("NI");
			    return false;
		    }
			
			foreach(List<PolygonPoint> h in polygon.holes) {
			//	Debug.Log("In H");
				if (CheckIfInside(contained,h)) return false;
			}
		
			return true;
    }

	*/	
	
	
	public static List<ClipperLib.IntPoint> Vec2LIP(IEnumerable<Vector3> vl) {
		List<ClipperLib.IntPoint> r=new List<ClipperLib.IntPoint>();
		foreach(Vector3 v in vl) {
			r.Add(new ClipperLib.IntPoint((int)(v.x*4096f),(int)(v.y*4096f)));
		}
		return r;
	}

	public static List<PolygonPoint> Vec2PP(List<Vector3> vl) {
		List<PolygonPoint> r=new List<PolygonPoint>();
		foreach(Vector3 v in vl) {
			r.Add(new PolygonPoint(((double)v.x),((double)v.y)));
		}
		return r;
	}
	
	public static List<PolygonPoint> LIP2PP(List<ClipperLib.IntPoint> vl) {
		List<PolygonPoint> r=new List<PolygonPoint>();
		foreach(ClipperLib.IntPoint v in vl) {
			r.Add(new PolygonPoint(((double)v.X)/4096d,((double)v.Y)/4096d));
		}
		return r;
	}
	
	public static List<Vector3> LIP2Vec(List<ClipperLib.IntPoint> vl) {
		List<Vector3> r=new List<Vector3>();
		foreach(ClipperLib.IntPoint v in vl) {
			r.Add(new Vector3(((float)v.X)/4096f,((float)v.Y)/4096f,0f));
		}
		return r;
	}
	
	private static List<Vector3> Simplify(List <Vector3> cb,float amount) {				
		float fs=1;
		List<Vector3> res=new List<Vector3>();
		amount /= 25f;
		amount *= amount;
		int cnt = 0;
		int clen = cb.Count;
												
		res.Add(cb[0]);
		Vector3 pv = cb[0], v = cb[1], nv;				
		//Debug.Log("+");
		for(int ctr = 1; ctr < clen; ctr++) {
					nv = cb[(ctr+1) % clen];
					 
					Vector2 d1 = v - pv;
					Vector3 d2 = nv - v;
					Vector3 d3 = nv - pv;
					float d3m=d3.magnitude;
					//Debug.Log(d2);
					if (d3m<=0.0001f) { d3m=float.PositiveInfinity; Debug.Log("d3m is zero");}
					float af = (Mathf.Abs(d1.x*d3.y-d3.x*d1.y)+Mathf.Abs(d2.x*d3.y-d3.x*d2.y))
						/ (d3m*fs);
                    bool a = (af >= amount);
					if (d1.magnitude<0.0001f) {a=false;}
					if (d2.magnitude<0.0001f) {a=false;}			
					if (d3.magnitude<0.0001f) {a=false;}			
					if (a) {
						//Debug.Log(v);
						res.Add(v);
						pv = v;
					}
					v = nv;
					++cnt;
		}
		for(int ctr = 0; ctr < clen; ctr++) {
					nv = cb[(ctr+1) % clen];
					 
					Vector2 d1 = v - pv;
					Vector3 d2 = nv - v;
					Vector3 d3 = nv - pv;
					float d3m=d3.magnitude;
					//Debug.Log(d2);
					if (d3m<=0.0001f) { d3m=float.PositiveInfinity; Debug.Log("d3m is zero");}
					float af = (Mathf.Abs(d1.x*d3.y-d3.x*d1.y)+Mathf.Abs(d2.x*d3.y-d3.x*d2.y))
						/ (d3m*fs);
                    bool a = (af >= amount);
					if (d1.magnitude<0.0001f) {a=false;}
					if (d2.magnitude<0.0001f) {a=false;}						
					if (a) {
						//Debug.Log(v);
						res.Add(v);
						pv = v;
						break;
					}
					v = nv;
					//++cnt;
		
		}
		res.RemoveAt(0);
		return res;
	}

	
	private static List<Vector3> Embolden(List<Vector3> cb, float strength) {

        float rotate;
		strength /= 40f;
		//Vector3 so = new Vector3(strength/2f,strength/2f,0);
		//Vector3 so = new Vector3(strength,strength,0);
		Vector3 tr = Vector3.zero;
		Vector3 so = Vector3.zero;

        int orientation = 1;

        if ( orientation == 1) { // True type ?
            rotate = -Mathf.PI/2;
		} else {
            rotate = Mathf.PI/2;
		}

		int clen = cb.Count;
        //List<Vector3> nb = new List<Vector3>(clen);
		Vector3 [] nb = new Vector3 [clen]; 

            if (clen > 0) {
				
				Vector3 pv = cb[clen-1], v = cb[0], nv;
                for (int ctr = 0; ctr < (clen+20); ctr++) {					
					nv = cb[(ctr+1)%clen];
					
					bool skip_this_vertex = false;
					
					// We have to keep all vertice 
					// because extrusion rely on all outline having the same num of vertices
					
                    if ((nv - v).magnitude < 0.001f) {						
						if (ctr==0) {
							nb[0]=(v + so + tr);
                        } else {
							nb[ctr%clen]=nb[(ctr-1)%clen];
						}
						
						skip_this_vertex = true;
					}
					 

					if (!skip_this_vertex) {
                        Vector2 d1=v-pv;
						Vector3 d2=nv-v;
						Vector3 d3;
																	
						float angle_in   = Mathf.Atan2( d1.x, d1.y );
						float angle_out  = Mathf.Atan2( d2.x, d2.y );
						float angle_diff2 = (( angle_out - angle_in + Mathf.PI)%(Mathf.PI*2)-Mathf.PI)/2f;
						
						float scale      = Mathf.Sign(Mathf.Cos( angle_diff2 ));// * Mathf.Cos( angle_diff2/8 );
							
						float e= angle_in +rotate + angle_diff2;
							
							// we progress in the direction of the bissetrix
							//d3=(new Vector3(Mathf.Cos(-e),Mathf.Sin(-e)))/strength;
							//d3=(new Vector3(Mathf.Cos(-e),Mathf.Sin(-e)))*strength;
							//d3=nv-pv;d3.Normalize(); 
						//d3=(new Vector3(Mathf.Cos(-e),Mathf.Sin(-e)))*(strength / scale);							
						//d3=(new Vector3(Mathf.Cos(-e),Mathf.Sin(-e)))*strength;
						d3=(new Vector3(Mathf.Cos(-e),Mathf.Sin(-e)))*(strength * scale);							
						tr=new Vector2(-d3.y,d3.x);
						nb[ctr%clen]=(v+so+tr);
						//}
						pv = v;
						v = nv;
					}
					
				}

		}
			
		return new List<Vector3>(nb);
	}	
	
	static bool ContainsAny(HashSet<Vector3> hs, List<Vector3> l) {
		foreach (Vector3 v in l) {
			if (hs.Contains(v)) { return true; }
		}
		return false;
	}
	
	
	
	static bool HasDup(List<Vector3> p) {
		for (int i = 0; i < p.Count; ++i) {
			if (p.IndexOf(p[i]) != i) { return true;}
		}
		return false;
	}
	
	private static List<Vector3> StripDup(List<Vector3> cb) {
		List<Vector3> nb=new List<Vector3>();
		for (int i=0;i<cb.Count;i++) {
			if (cb.IndexOf(cb[i])==i) {
				nb.Add (cb[i]);
			}
		}
		return nb;
	}	
	
	public static Mesh P2TTriangulate(TTFTextOutline outline,  Mesh outMesh) {
		
		float Z = outline.Min.z; // Assume all vertices have the same Z coord
		
		List< List<ClipperLib.IntPoint> > ipolygones= new List<List<ClipperLib.IntPoint>>();
		
		foreach(TTFTextOutline.Boundary lv in outline.boundaries) {
			
			if (Intersection.isSimple(lv)) { 
				
				ipolygones.Add(Vec2LIP(lv));
				
			} else {
				
				//Debug.LogWarning("Complex Contour"); 
			
				try {
				
					foreach (List<Vector3> p in Intersection.Decompose(lv)) { // decompose outlines
						
						ipolygones.Add(Vec2LIP(p));
					}
				
				} catch (System.Exception) { // parallel segments
				
					//Debug.LogWarning("Bad contour:" + ex.ToString());
				}
			}
		}
		
			
		ClipperLib.Clipper c=new ClipperLib.Clipper();
		List<ClipperLib.ExPolygon> res=new List<ClipperLib.ExPolygon>();
		
		
		c.AddPolygons(ipolygones,ClipperLib.PolyType.ptSubject);				
		c.Execute(ClipperLib.ClipType.ctUnion,res,ClipperLib.PolyFillType.pftNonZero,ClipperLib.PolyFillType.pftNonZero);
		
		//c.Execute(ClipperLib.ClipType.ctUnion,res,ClipperLib.PolyFillType.pftEvenOdd,ClipperLib.PolyFillType.pftEvenOdd);
		//c.Execute(ClipperLib.ClipType.ctUnion,res,ClipperLib.PolyFillType.pftPositive,ClipperLib.PolyFillType.pftPositive);
		//c.Execute(ClipperLib.ClipType.ctUnion,res,ClipperLib.PolyFillType.pftPositive,ClipperLib.PolyFillType.pftPositive);
			
		
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		
		Dictionary<Vector3, int> vDic = new Dictionary<Vector3, int>();

		
		foreach(ClipperLib.ExPolygon p in res) {
			
			Vector3 mv=new Vector3(float.PositiveInfinity,float.PositiveInfinity,float.PositiveInfinity);
			Vector3 Mv=new Vector3(float.NegativeInfinity,float.NegativeInfinity,float.NegativeInfinity);
			
			List<Vector3> np=LIP2Vec(p.outer);
			
			for (int i=0;i<np.Count;i++) {
				mv=Vector3.Min(mv,np[i]);
				Mv=Vector3.Max(Mv,np[i]);
			}
			
			Vector3 sv=(Mv-mv);
			//int oc=np.Count;
			np = Simplify(np,0.05f);
			
			// set of all vertices for this triangulation step
			HashSet<Vector3> whole = new HashSet<Vector3>();
			
			if ((np.Count>=4) && ((sv.x*sv.y)>0.001f) && Intersection.isSimple(np) && ! HasDup(np)) {
				
				foreach (Vector3 v in np) {
					whole.Add(v);
				}
				
				Polygon cp=new Polygon(Vec2PP(np));		
				
				foreach(List<ClipperLib.IntPoint> h in p.holes) {
					
					List<Vector3> ph = Simplify(LIP2Vec(h), 0.05f);
					
					// Poli2tri doesnt support intersection and dupplicate vertices
					// skip all holes which does not respect thoses conditions
					
					if ((! Intersection.isSimple(ph)) || HasDup(ph)) {
						
						//Debug.LogWarning("Skipping Complex hole");
						
					} else if (ContainsAny(whole, ph)) {
						
						//Debug.LogWarning("Duplicate vertices in holes");
					
					} else {
						
						foreach (Vector3 v in ph) { whole.Add(v); }
						
						cp.AddHole(new Polygon(Vec2PP(ph)));
					}
				}
				
				
				try {
				
					P2T.Triangulate(cp);
				
					foreach(DelaunayTriangle dt in cp.Triangles) {
						
						Vector3 v;
						int idx;
						
						for (int i = 0; i < 3; ++i) { 
							v = new Vector3(dt.Points[i].Xf, dt.Points[i].Yf, Z);
							if (! vDic.TryGetValue(v, out idx)) {
								idx = vertices.Count;
								vertices.Add(v);
								vDic.Add(v, idx);
							}
							triangles.Add(idx);
						}
						
					}					
				
				} catch (System.Exception e) {
					Debug.LogWarning(e);
				}
				
			} else {
				
				//Debug.LogWarning("Skip Polygon: count=" + np.Count + " size=" + sv + " simple=" + Intersection.isSimple(np) + " dup:" + HasDup(np));
			}
		}
		
		outMesh.Clear();
		
		outMesh.vertices = vertices.ToArray();
		outMesh.triangles = triangles.ToArray();
		outMesh.RecalculateBounds();
		outMesh.RecalculateNormals();
		outMesh.Optimize();
		
		return outMesh;
	}
	
#endif
	
}

}