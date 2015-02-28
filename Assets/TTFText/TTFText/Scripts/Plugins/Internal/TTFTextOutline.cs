//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
using TTF = FTSharp;  
#endif




// This class is designed to store a possibly 3d outline
[System.Serializable]
public class TTFTextOutline : System.Object
{
	
	public const float Epsilon = 0.001f;
		
	
	[SerializeField]
	Vector3 min, max, size;
	
	[SerializeField]
	int nvertices;
	public List<Vector3> points;
	public List<int> blengthes;
	
	
	public class BoundaryUniformTraverser
	{
		Boundary b;
		float[] ncumlengthes;
		
		public BoundaryUniformTraverser (Boundary bn)
		{
			b = bn;
			float cs = 0;
			ncumlengthes = new float[b.Count];
			for (int i=0; i<b.Count; i++) {
				cs += (b [(i + 1) % b.Count] - b [i]).magnitude;
				ncumlengthes [i] = cs;
			}
			for (int i=0; i<b.Count; i++) {
				ncumlengthes [i] /= ncumlengthes [b.Count - 1];
			}
		}
		
		// percent is actually in between 0 and 1 (for 100%)
		public Vector3 GetPositionAt (float percent)
		{
			percent = percent % 1f;
			int d = (int)Mathf.Floor (Mathf.Log (ncumlengthes.Length - 1, 2));
			
			int s = 0;
			for (int c=d; c>=0; c--) {
				if ((s + (1 << c)) < (ncumlengthes.Length - 1)) {
					if (ncumlengthes [s + (1 << c)] <= percent) {
						s += (1 << c);
					}
				}
			}
			
			float r;
			if (s < (b.Count - 1)) {
				r = (percent - ncumlengthes [s]) / (ncumlengthes [(s + 1)] - ncumlengthes [s]);
			} else {
				r = (percent - ncumlengthes [s]) / (ncumlengthes [(0)]);
			}
			//Debug.Log(System.String.Format("{0}/{4}:{1} {2} {3}",s, ncumlengthes[s], percent, ncumlengthes[(s+1)%b.Count],b.Count));
			
			return Vector3.Lerp (b [(s + 1) % b.Count], b [(2 + s) % b.Count], r);
		}
	}
	
	public class Boundary : IList<Vector3>
	{
		
		TTFTextOutline outline;
		int begin_, end_;
		
		public bool IsReadOnly { get { return true; } }
		
		public void Add (Vector3 item)
		{
			throw new System.NotSupportedException ();
		}

		public void Clear ()
		{
			throw new System.NotSupportedException ();
		}

		public bool Remove (Vector3 item)
		{
			throw new System.NotSupportedException ();
		}

		public void Insert (int index, Vector3 item)
		{
			throw new System.NotSupportedException ();
		}

		public void RemoveAt (int index)
		{
			throw new System.NotSupportedException ();
		}
		
		public int IndexOf (Vector3 item)
		{
			int idx = 0;
			foreach (Vector3 v in this) {
				if (v == item) {
					return idx;
				}
				++idx;
			}
			return -1;
		}
		
		public bool Contains (Vector3 item)
		{
			foreach (Vector3 v in this) {
				if (v == item) {
					return true;
				}
			}
			return false;
		}
		
		public void CopyTo (Vector3[] array, int idx)
		{
			foreach (Vector3 v in this) {
				array [idx] = v;
				++idx;
			}
		}
		
		public class BEnumerator : IEnumerator<Vector3>
		{
			TTFTextOutline outline;
			int begin_, curr_, end_;
			
			public BEnumerator (TTFTextOutline o, int begin, int end)
			{
				outline = o;
				begin_ = begin;
				end_ = end;
				curr_ = begin_ - 1;
			}
			
			public void Dispose ()
			{
			}
			
			public void Reset ()
			{
				curr_ = begin_ - 1;
			}
			
			public Vector3 Current {
				get { return outline.points [curr_]; }
			}
			
			object IEnumerator.Current {
				get { return (object)Current; }
			}

			public bool MoveNext ()
			{
				++curr_;
				return (curr_ < end_);
			}
		}
		
		public Vector3 this [int index] {
			get {
				if (index >= end_ - begin_) {
					throw new System.ArgumentOutOfRangeException ();
				}
				return outline.points [begin_ + index];
			} 
			set { 
				if (index >= end_ - begin_) {
					throw new System.ArgumentOutOfRangeException ();
				}
				outline.points [begin_ + index] = value;
			}
		}
		
		public float [] cumDists() {
			float [] r=new float[Count-1];
			int i=0;
			Vector3 lv=this[0];
			float prev=0;
			foreach(Vector3 v in this) {
				if (i>0) {
					prev+=(lv-v).magnitude;
					r[i-1]=prev;
					lv=v;
				}
				i++;
			}
			return r;
		}

		public float MaxDist() {
			float r=0; 
			float d=0;
			Vector3 lv=this[0];
			
			foreach(Vector3 v in this) {				
				d=(lv-v).magnitude;
				if (d>r) {
						r=d;
				}
				lv=v;				
			}
			
		    d=(lv-this[0]).magnitude;
			if (d>r) r=d;
			
			return r;
		}

		
		public Vector3 interpolatePosition1(float [] cumDists, float percent) {
			float eps=0.0000000001f;
			if ((percent<0)||(percent>1)) {
				percent=(((percent%1f)+1f)%1f);
			}
			if (percent<eps) return this[0];
			if (Mathf.Abs(percent-1)<eps) return this[Count-1];
			float tl=cumDists[cumDists.Length-1];			
			float c=percent*tl;
//			int bi=0;
			int v=0;
			
			
			int step=1;
			if (cumDists.Length>2) {
				step=1<<(int)Mathf.Floor(Mathf.Log(cumDists.Length-1,2));
			}
			//Debug.Log(step);
			
			
			while (step>0) {
				if (v+step<cumDists.Length) {
				  if (cumDists[v+step]<c) {
					v+=step;				
				  }	
				}
				step>>=1;
			}
			
			Vector3 b0=this[v],b1=this[v+1];
			
			float gamma=(v>0)?(cumDists[v]):0;
			float beta=cumDists[v+1]-gamma;
			float alpha=((c-gamma)/(beta+eps));
			//Debug.Log(System.String.Format("{0}-{1}-{2}:{3}/{4}~{5}",gamma,c,cumDists[v],v,cumDists.Length,alpha));
			
			return b0*(1-alpha)+b1*(alpha);
		}
		
		public int Count {
			get { return end_ - begin_; }
		}
		
		public Boundary (TTFTextOutline o, int begin, int end)
		{
			
			if (end > o.points.Count) {
				Debug.LogError ("Bad boundary!");
			}
			
			outline = o;
			begin_ = begin;
			end_ = end;
		}
		
		public BEnumerator GetEnumerator ()
		{
			return new BEnumerator (outline, begin_, end_);
		}
		
		IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator ()
		{
			return (IEnumerator<Vector3>)GetEnumerator ();
		}
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return (IEnumerator)GetEnumerator ();
		}
		
		public  BoundaryUniformTraverser GetUniformTraverser ()
		{
			return new BoundaryUniformTraverser (this);
		}
	}
	
	
	
	public IEnumerable<TTFTextOutline.Boundary> boundaries {
		get {
			int p = 0;
			int i = 0;
			foreach (int l in blengthes) {
				//Debug.Log("Boundary" + i + "/" + blengthes.Count + ": b="  + p + "e=" + (p+l) + "nvertice=" + nvertices);
				yield return new Boundary(this, p, p+l);
				p = p + l;
				i++;
			}
		}
		
	}
	
	
	
	
	// Position of the pen for  following text
	[SerializeField]
	public Vector3 advance;
	
	public int numVertices {
		get { return nvertices; }
	}
	
	public Vector3 Min {
		get { return min; }
	}
	
	public Vector3 Max {
		get { return max; }
	}
	
	public Vector3 Size {
		get { return size; }
	}
	
	public Vector3 GetSize ()
	{
		return size;
	}

	public void GetMinMax (out Vector3 _min, out Vector3 _max)
	{
		_min = min;
		_max = max;
	}
	
	public TTFTextOutline ()
	{
		nvertices = 0;
		min = new Vector3 (float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		max = new Vector3 (float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
		size = Vector3.zero;		
		points = new List<Vector3> ();
		blengthes = new List<int> ();
		advance = Vector3.zero;
	}
	
	// Copy an outline
	public TTFTextOutline (TTFTextOutline other)
	{
		
		nvertices = other.nvertices;
		points = new List<Vector3> (other.points);
		blengthes = new List<int> (other.blengthes);
		max = other.max;
		min = other.min;
		size = other.size;
		advance = other.advance;
	}

	
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR

	// Construct from a TTF.Outline	
	
	public TTFTextOutline(TTF.Outline ttfoutline, TTF.Outline.Point adv, bool reversed) {
		
		min = new Vector3(float.PositiveInfinity,float.PositiveInfinity,float.PositiveInfinity);
		max = new Vector3(float.NegativeInfinity,float.NegativeInfinity,float.NegativeInfinity);
		size = Vector3.zero;
		
		nvertices = 0;
		
		points = new List<Vector3>(nvertices);
		blengthes = new List<int>();
		
		advance = new Vector3((float)adv.X, (float)adv.Y, 0);
		
		
		int i = 0;
		List<Vector3> cb = new List<Vector3>();
		
		while (i < ttfoutline.Size) {			
			TTF.Outline.Point p = ttfoutline.Path[i];
			Vector3 v = new Vector3((float) p.X, (float) p.Y, 0);
			
			if (ttfoutline.Types[i] == TTF.Outline.PointType.MoveTo) { // new contour
			
				cb = ClearDup(cb);
				
				if (cb.Count > 0) {
					if (reversed) cb.Reverse();
					AddBoundary(cb);
				}
				cb.Clear();
			}
		
			cb.Add(v);
			++i;
		}
		
		cb = ClearDup(cb);
		if (cb.Count > 0) { if (reversed) cb.Reverse(); AddBoundary(cb); }
		
		//CheckDups("Constructor");
	}

	
	
	
	
	public bool HasDups() {
		foreach (Boundary b in boundaries) {
			for (int i = 0; i < b.Count; ++i) {
				if (EpsEqual(b[i], b[(i+1)%b.Count])) { return true; }
			}
		}
		return false;
	}
	
	public bool HasDupsOld() {
		foreach (Boundary b in boundaries) {
			for (int i = 0; i < b.Count; ++i) {
				if (b[i] == b[(i+1) % b.Count]) { return true; }
			}
		}
		return false;
	}
	
	
	public void CheckDups(string msg) {
		if (HasDups()) { Debug.LogError("Error:" + msg + ": Outline with duplicates vertices"); }
	}
	
	
	static List<Vector3> ClearDup(IList<Vector3> contour) {		
		List<Vector3> res = new List<Vector3>();
	
		int len = contour.Count;
		while (len > 0 &&  EpsEqual(contour[len-1], contour[0])) { len--;}		
		if (len == 0) { return res; }
		
		Vector3 last = contour[0];
		res.Add(last);
		
		for (int i = 1; i < len; ++i) {
			Vector3 v = contour[i];
			if (! EpsEqual(v,last)) {
				res.Add(v);
				last = v;
			}
		}
		
		return res;
	}
	
	static List<Vector3> ClearDupOld(IList<Vector3> contour) {
		
		List<Vector3> res = new List<Vector3>();
	
		int len = contour.Count;
		while (len > 0 &&  contour[len-1] == contour[0]) { len--;}
		
		if (len == 0) { return res; }
		
		Vector3 last = contour[0];
		res.Add(last);
		
		for (int i = 1; i < len; ++i) {
			Vector3 v = contour[i];
			if (v != last) {
				res.Add(v);
				last = v;
			}
		}
		
		return res;
	}
 	

	
	
	static bool EpsEqual(Vector3 u, Vector3 v) {
		return Vector3.Distance(u, v) <= Epsilon;
	}
	
	// static constructor
	public static TTFTextOutline TTF2Outline(TTF.Outline ttfoutline, TTF.Outline.Point adv, bool reversed) {
		return new TTFTextOutline(ttfoutline, adv,reversed);
	}
#endif
	
	public void Append (TTFTextOutline o, Vector3 offset)
	{
		foreach (TTFTextOutline.Boundary boundary in o.boundaries) {
			List<Vector3> lst = new List<Vector3> (boundary.Count);
			foreach (Vector3 v in boundary) { 
				lst.Add (v + offset);
			}
			AddBoundary (lst);
		}
		
		advance += o.advance;
	}
	
	// Slant
	public void Slant (float delta)
	{
		foreach (Boundary cb in boundaries) {
			int clen = cb.Count;
			for (int ctr=0; ctr<clen; ctr++) {
				Vector3 v = cb [ctr];
				v.x += v.y * delta;
				cb [ctr] = v;
			}
		}
		//advance.x = advance.x + delta * advance.y;
		max.x += max.y * delta;
		min.x += min.y * delta;
		size = max - min;
	}
	
	// Translate
	public void Translate (Vector3 t)
	{
		foreach (Boundary cb in boundaries) {
			int clen = cb.Count;
			for (int ctr=0; ctr<clen; ctr++) {
				cb [ctr] = cb [ctr] + t;
			}
		}	
		min += t;
		max += t;
		advance += t;
	}
	
	// Scale
	public void Rescale (Vector3 scale)
	{
		foreach (Boundary cb in boundaries) {
			int clen = cb.Count;
			for (int ctr = 0; ctr < clen; ctr++) {
				Vector3 v = cb [ctr];
				v = Vector3.Scale (v, scale);
				cb [ctr] = v;
			}
		}
		min = Vector3.Scale (min, scale);
		max = Vector3.Scale (max, scale);	
		size = max - min;
		advance = Vector3.Scale (advance, scale);
	}
	
	public void Rescale (float scaleX, float scaleY, float scaleZ)
	{
		Rescale (new Vector3 (scaleX, scaleY, scaleZ));
	}
	
	public void Rescale (float scale)
	{
		Rescale (new Vector3 (scale, scale, scale));
	}
	
	
	
	// Rescale outline so that it fits into the specified bounding size
	// if width, height or depth < 0, don't change the corresponding coordinates
	public void Resize (Vector3 tsize)
	{
		Vector3 scale = Vector3.one;
		if (size.x != 0 && tsize.x >= 0) {
			scale.x = tsize.x / size.x;
		}
		if (size.y != 0 && tsize.y >= 0) {
			scale.y = tsize.y / size.y;
		}
		if (size.z != 0 && tsize.z >= 0) {
			scale.z = tsize.z / size.z;
		}
		Rescale (scale);
	}
	
	public void Resize (float width, float height, float depth)
	{
		Resize (new Vector3 (width, height, depth));
	}
	
	
	public float MaxDist() {
		float md=0;
		foreach(Boundary b in boundaries) {
			float td=b.MaxDist();
			if (td>md) md=td;
		}
		return md;
	}
	
	
	// Simplify the outline by deleting the less usefull vertex
	// a vertex v_i is said useless when 
	//
	public TTFTextOutline Simplify (float amount, float fontsize)
	{
		return ApplyMask (SimplifyMask (amount, fontsize));
	}
	
	public bool [] SimplifyMask (float amount, float fontsize)
	{		
		
		bool[] r = new bool[nvertices];
		
		for (int i = 0; i < nvertices; ++i) { // init all to true
			r [i] = true;
		}
		
		if (amount <= 0 || numVertices <= 0) {
			return r;
		}
		
		
		amount /= 25f;
		amount *= amount;
		int cnt = 0;
			
		foreach (Boundary cb in boundaries) {
				
			int clen = cb.Count;
				
			if (clen > 3) {
					
				//Vector3 pv = cb[clen-1], v = cb[0], nv;

				// Always keep cb[0]
				r [cnt++] = true;
				Vector3 pv = cb [0], v = cb [1], nv;
				
				for (int ctr = 1; ctr < clen; ctr++) {
			
					nv = cb [(ctr + 1) % clen];
					 
					Vector2 d1 = v - pv;
					Vector3 d2 = nv - v;
					Vector3 d3 = nv - pv;
//#if !TTFTEXT_LITE						
					float af = (Mathf.Abs (d1.x * d3.y - d3.x * d1.y) + Mathf.Abs (d2.x * d3.y - d3.x * d2.y))
						/ (d3.magnitude * fontsize);
					bool a = (af >= amount);
//#else
//                    bool a = true;
//#endif						
					
						
					r [cnt] = a;
					if (a) {
						pv = v;
					}
					v = nv;
					++cnt;
				}
			
			} else { // clen <=3
				for (int i = 0; i < clen; ++i) {
					r [cnt++] = true;
				}
			}
		}
			
		return r;
	}

	public void AddBoundary (IEnumerable<Vector3> b)
	{		
		int cnt = 0;
		foreach (Vector3 v in b) {
			min = Vector3.Min (min, v);
			max = Vector3.Max (max, v);
			points.Add (v);
			++cnt;
		}
		blengthes.Add (cnt);
		nvertices += cnt;
		size = max - min;
	}
	
	
	
	public void AddBoundary(Vector3[] array, int count) {
		AddBoundary(EnumerateArray(array, count));
	}
	
	static IEnumerable<Vector3> EnumerateArray(Vector3[] array, int count) {
		for (int idx = 0; idx < count; ++idx) {
			yield return array[idx];
		}
	}

	
	public TTFTextOutline ApplyMask (bool [] b)
	{
		int cnt = 0;
		
		TTFTextOutline r = new TTFTextOutline ();
		foreach (Boundary cb in boundaries) {
			List<Vector3> nb = new List<Vector3> ();
			foreach (Vector3 v in cb) {
				if (b [cnt++]) {
					nb.Add (v);
				}
			}
			if (nb.Count > 0) {
				r.AddBoundary (nb);
			}
		}
		r.advance = advance;	
		return r;
	}
	

	
	#region OUTLINE_EFFECTS
	public class TTFTextOutlineEffect : System.Object
	{
		[System.Serializable]
		public class NParameters {}
		public int id;
		public string name;
		public virtual object GetDefaultParameters() { return new NParameters ();}
		public virtual TTFTextOutline Apply(TTFTextOutline outline,object parameters) { 
			return outline;
		}
	}
	
	
	
	public class EmboldenN2Effect  : TTFTextOutlineEffect
	{
		public EmboldenN2Effect() {name="Embold N2";}
		
		[System.Serializable]
		public class Parameters {
			public float Amount;
		}
		public override object GetDefaultParameters() { Parameters p= new Parameters ();p.Amount=0; return p;}
		public override TTFTextOutline Apply(TTFTextOutline outline, object parameters) {
		float rotate;
		float strength=(parameters as Parameters).Amount;
		strength /= 40f;
			
			
		//Vector3 so = new Vector3(strength/2f,strength/2f,0);
		//Vector3 so = new Vector3 (strength, strength, 0);
		Vector3 tr = Vector3.zero;
		Vector3 so = Vector3.zero;
        
		if (Mathf.Abs (strength) < 0.001f)
			return new TTFTextOutline (outline);

		int orientation = 1;

		if (orientation == 1) { // True type ?
			rotate = -Mathf.PI / 2;
		} else {
			rotate = Mathf.PI / 2;
		}

		TTFTextOutline r = new TTFTextOutline ();
        
		foreach (Boundary cb in outline.boundaries) {
			int clen = cb.Count;
			Vector3 [] nb = new Vector3 [clen]; 

			if (clen > 0) {				
				Vector3 pv = cb [clen - 1], v = cb [0], nv;
					
					
				for (int ctr = 0; ctr < (clen+20); ctr++) {					
					nv = cb [(ctr + 1) % clen];					
					bool skip_this_vertex = false;
					
					// We have actually keep all vertice 
					// because extrusion rely on all outline having the same num of vertices					
					if ((nv - v).magnitude < 0.001f) {						
						if (ctr == 0) {
							//nb [0] = (v + so + tr);
							nb [ctr % clen] = (v + so );
						} else {
							nb [ctr % clen] = nb [(ctr - 1) % clen];
						}
						
						skip_this_vertex = true;
					}
					 

					if (!skip_this_vertex) {
						Vector2 d1 = v - pv;
						Vector3 d2 = nv - v;
						Vector3 d3;
																	
						float angle_in = Mathf.Atan2 (d1.x, d1.y);
						float angle_out = Mathf.Atan2 (d2.x, d2.y);
						float angle_diff2 = ((angle_out - angle_in + 3*Mathf.PI) % (Mathf.PI * 2) - Mathf.PI) / 2f;
						
						float scale = Mathf.Sign (Mathf.Cos (angle_diff2));// * Mathf.Cos( angle_diff2/8 );
							
						
						//float scale=Mathf.Tan(Mathf.Cos(angle_diff2)*1000)/Mathf.PI;
						//if (scale==0) {scale=1;}
						scale=1;
							
						//float e = angle_in /*+ rotate*/ + angle_diff2;
						float e = angle_in + rotate + angle_diff2;
							
						// we progress in the direction of the bissetrix
						//d3=(new Vector3(Mathf.Cos(-e),Mathf.Sin(-e)))/strength;
						//d3=(new Vector3(Mathf.Cos(-e),Mathf.Sin(-e)))*strength;
						//d3=nv-pv;d3.Normalize(); 
						//d3=(new Vector3(Mathf.Cos(-e),Mathf.Sin(-e)))*(strength / scale);							
						//d3=(new Vector3(Mathf.Cos(-e),Mathf.Sin(-e)))*strength;
						d3 = (new Vector3 (Mathf.Cos (-e), Mathf.Sin (-e))) * (strength * scale);							
						tr = new Vector2 (-d3.y, d3.x);						
						nb [ctr % clen] = (v + so + tr);
							
						//nb [ctr % clen] =v+so;
						//nb [ctr % clen] = (v + tr);
						//}
						pv = v;
						v = nv;
							
					}
					
				}

			}
			
			
			if (nb.Length > 0) {
				r.AddBoundary (nb);
			}
	
		}

		r.advance = outline.advance;
		
		//Debug.Log(r.MaxDist());	
		//Debug.Log(r.boundaries)
		//r.Check();
		return r;
			
		}
	}
	

	
	
	
	public class EmboldenN2XEffect  : TTFTextOutlineEffect
	{
		public EmboldenN2XEffect() {name="Embold N2X";}
		
		[System.Serializable]
		public class Parameters {
			public float Amount;
			public int Interpolation=3;
		}
		public override object GetDefaultParameters() { Parameters p= new Parameters ();p.Amount=0; return p;}
		public override TTFTextOutline Apply(TTFTextOutline outline, object parameters) {
		float rotate;
		int octr=0;
			
		float strength=(parameters as Parameters).Amount;
		int interpolation=(parameters as Parameters).Interpolation;
		
		strength /= 40f;
			
			
		//Vector3 so = new Vector3(strength/2f,strength/2f,0);
		//Vector3 so = new Vector3 (strength, strength, 0);
		Vector3 tr = Vector3.zero;
		Vector3 so = Vector3.zero;
        
		if (Mathf.Abs (strength) < 0.001f)
			return new TTFTextOutline (outline);

		int orientation = 1;

		if (orientation == 1) { // True type ?
			rotate = -Mathf.PI / 2;
		} else {
			rotate = Mathf.PI / 2;
		}

		TTFTextOutline r = new TTFTextOutline ();
        
		foreach (Boundary cb in outline.boundaries) {
			int clen = cb.Count;
			Vector3 [] nb = new Vector3 [clen*interpolation]; 

			if (clen > 0) {				
				Vector3 pv = cb [clen - 1], v = cb [0], nv;
					
					
				for (int ctr = 0; ctr < (clen+20); ctr++) {					
					nv = cb [(ctr + 1) % clen];					
					bool skip_this_vertex = false;
					
					// We have actually keep all vertice 
					// because extrusion rely on all outline having the same num of vertices					
					if ((nv - v).magnitude < 0.001f) {						
						if (ctr == 0) {
							//nb [0] = (v + so + tr);	
							for (int ci=0;ci<interpolation;ci++) {
								nb [octr++ % (clen*interpolation)] = (v + so );
							}
						} else {
							for (int ci=0;ci<interpolation;ci++) {	
								nb [octr++ % (clen*interpolation)] = nb [(ctr - 1) % clen];
							}
						}
						
						skip_this_vertex = true;
					}
					 

					if (!skip_this_vertex) {
						Vector2 d1 = v - pv;
						Vector3 d2 = nv - v;
						Vector3 d3;
																	
						float angle_in = Mathf.Atan2 (d1.x, d1.y);
						float angle_out = Mathf.Atan2 (d2.x, d2.y);
							
							
						for (int ci=0;ci<interpolation;ci++) {		
						float angle_diff2 = ((angle_out - angle_in + 3*Mathf.PI) % (Mathf.PI * 2) - Mathf.PI) *((float)(1+ci))/(1+interpolation);
						
						float scale = Mathf.Sign (Mathf.Cos (angle_diff2));// * Mathf.Cos( angle_diff2/8 );
						scale=1;
							
						float e = angle_in + rotate + angle_diff2;
							
						d3 = (new Vector3 (Mathf.Cos (-e), Mathf.Sin (-e))) * (strength * scale);							
						tr = new Vector2 (-d3.y, d3.x);						
						nb [octr++ % (clen*interpolation)] = (v + so + tr);
							
						}		
						pv = v;
						v = nv;
							
					}
					
				}

			}
			
			
			if (nb.Length > 0) {
				r.AddBoundary (nb);
			}
	
		}

		r.advance = outline.advance;
		
		return r;
			
		}
	}

	
	
	
	
	
	public class EmboldenOBEffect  : TTFTextOutlineEffect
	{
		public EmboldenOBEffect() {name="Embold OB(Legacy)";}
		[System.Serializable]
		public class Parameters {
			public float Amount;
		}
		public override object GetDefaultParameters() { Parameters p= new Parameters ();p.Amount=0; return p;}
		public override TTFTextOutline Apply(TTFTextOutline outline, object parameters) {
			float strength=(parameters as Parameters).Amount;
			strength /= 40f;
			if (strength == 0f) { return new TTFTextOutline(outline); }
		    Vector3 so = new Vector3 (strength, strength, 0);
			
			TTFTextOutline res = new TTFTextOutline();
		
			Vector3[] nv = new Vector3[outline.nvertices];
			Vector3[] d = new Vector3[outline.nvertices];
			foreach (Boundary v in outline.boundaries) {
				int vlen = v.Count;
			
				for (int i = 0; i < vlen; ++i) {
					d[i] = (Quaternion.AngleAxis(90f, Vector3.forward) * (v[(i+1) % vlen] - v[i])).normalized *  strength;
				}
		
				for (int i = 0;  i < vlen; ++i) {				
					int ni = (i+1) % vlen;
				
					if (! TTFTextInternal.Intersection.GetIntersection(v[i] + d[i], v[ni] + d[i], v[ni] + d[ni], v[(i+2) % vlen] + d[ni], out nv[ni])) {
						nv[ni] = so+ Vector3.Lerp(v[ni] + d[i], v[ni] + d[ni], 0.5f);
					}
				}
			
				res.AddBoundary(nv, vlen);
			}
		
			res.advance = outline.advance;
		
			return res;
		}		
	}

	public class SimplifyEffect  : TTFTextOutlineEffect
	{
		public SimplifyEffect() {name="Simplify";}
		[System.Serializable]
		public class Parameters {
			public float Amount;
		}
		public override object GetDefaultParameters() { Parameters p= new Parameters ();p.Amount=0; return p;}
		public override TTFTextOutline Apply(TTFTextOutline outline, object parameters) {
			float amount=(parameters as Parameters).Amount;
			return outline.Simplify(amount,1);
		}		
	}

	public class KeypointsEffect  : TTFTextOutlineEffect
	{
		public KeypointsEffect() {name="Keypoints";}
		[System.Serializable]
		public class Parameters {
			public float radius=1;
			public int ngons=4;
			public float phase=0;
		}
		public override object GetDefaultParameters() { Parameters p= new Parameters (); return p;}
		public override TTFTextOutline Apply(TTFTextOutline outline, object parameters) {
			Parameters p=(parameters as Parameters);
			TTFTextOutline no=new TTFTextOutline();	
			Vector3 [] tv=new Vector3[p.ngons];
			foreach (Boundary v in outline.boundaries) {			
				int  vlen=v.Count;
				for (int i = 0;  i < vlen; ++i) {
					for (int r=0;r<p.ngons;r++) {
						float a=-(p.phase+(((float) r)/p.ngons))*Mathf.PI*2;
						tv[r]=v[i]+new Vector3(Mathf.Cos(a)*p.radius*0.01f,Mathf.Sin(a)*p.radius*0.01f,0);
					}
					no.AddBoundary(tv);
				}
			}
		
			
			return no;
			
		}		
	}

	public class PassBandEffect  : TTFTextOutlineEffect
	{
		public PassBandEffect() {name="Passband";}
		[System.Serializable]
		public class Parameters {
			public int interpolation=100;
			public TTFTextStyle.SerializableAnimationCurve ac;//=SerializableAnimationCurve.Linear(0,1,1,1);
		}
		public  override object GetDefaultParameters() { Parameters p= new Parameters (); p.ac=new TTFTextStyle.SerializableAnimationCurve(); return p;}
		public override TTFTextOutline Apply(TTFTextOutline outline, object parameters) {
			Parameters p=(parameters as Parameters);
			TTFTextOutline no=new TTFTextOutline();	
			//Vector3 [] tv=new Vector3[p.interpolation];
			
			foreach (Boundary v in outline.boundaries) {			
				float [] cumdist=v.cumDists();
				Vector3 [] sg=new Vector3[p.interpolation];				
				Vector3 [] sr=new Vector3[p.interpolation];
				Vector3 [] si=new Vector3[p.interpolation];
				Vector3 [] sm=new Vector3[p.interpolation];
				Vector3 [] sp=new Vector3[p.interpolation];
				
				//Debug.Log(cumdist[cumdist.Length-1]);
				
				for (int i = 0;  i < p.interpolation; ++i) {
					sg[i]=v.interpolatePosition1(cumdist,
						(((float)i)/((float)(p.interpolation-1)))
						);
				}
				
				float sf=(2*Mathf.PI)/((float)p.interpolation);
				float af=1f/((float)p.interpolation);
				
				for (int i = 0;  i < p.interpolation; ++i) {
					for (int j = 0;  j < p.interpolation; ++j) {
						sr[i]+=af*Mathf.Cos(j*i*sf)*sg[j];
						si[i]+=af*Mathf.Sin(j*i*sf)*sg[j];
					}
					float q=p.ac.GetAnimcurve().Evaluate(1-Mathf.Abs((((float)i)/p.interpolation)*2-1));
					sm[i]=new Vector3(
						Mathf.Sqrt(sr[i].x*sr[i].x+si[i].x*si[i].x)*q,
					    Mathf.Sqrt(sr[i].y*sr[i].y+si[i].y*si[i].y)*q,
					    Mathf.Sqrt(sr[i].z*sr[i].z+si[i].z*si[i].z)*q);
					sp[i]=new Vector3(
						(sm[i].x!=0)?Mathf.Atan2(si[i].x,sr[i].x):0,
						(sm[i].y!=0)?Mathf.Atan2(si[i].y,sr[i].y):0,
						(sm[i].z!=0)?Mathf.Atan2(si[i].z,sr[i].z):0
						)
						;
										
				}
				
				for (int j = 0;  j < p.interpolation; ++j) {
					sg[j]=Vector3.zero;
				}
				
				for (int i = 0;  i < p.interpolation; ++i) {
					for (int j = 0;  j < p.interpolation; ++j) {
						float alphax=sp[j].x+i*j*sf;
						float alphay=sp[j].y+i*j*sf;
						float alphaz=sp[j].z+i*j*sf;						
						sg[i]+=/*af**/new Vector3(
							Mathf.Cos(alphax)*sm[j].x/*+Mathf.Sin(alphax)*sm[i].x*/,
							Mathf.Cos(alphay)*sm[j].y/*+Mathf.Sin(alphay)*sm[i].y*/,
							Mathf.Cos(alphaz)*sm[j].z/*+Mathf.Sin(alphaz)*sm[i].z*/
							)
							;
					}
				}
				
				no.AddBoundary(sg);
			}
		
			//Debug.Log(no.Min);
			//Debug.Log(no.Max);
			return no;
			
		}		
	}
	
	
	public class FreeYStretchEffect : TTFTextOutlineEffect
	{
		public FreeYStretchEffect() {name="Free YStretch";}
		[System.Serializable]
		public class Parameters {
			public TTFTextStyle.SerializableAnimationCurve YMorph;
		}
		public  override object GetDefaultParameters() { Parameters p= new Parameters (); p.YMorph=new TTFTextStyle.SerializableAnimationCurve(); return p;}
		public override TTFTextOutline Apply(TTFTextOutline outline, object parameters) {
			AnimationCurve ac1=((TTFTextStyle.SerializableAnimationCurve)((parameters as Parameters).YMorph)).GetAnimcurve();
			
			foreach (Boundary v in outline.boundaries) {			
				int vlen = v.Count;

				for (int i = 0;  i < vlen; ++i) {
					v[i]=new Vector3(v[i].x,ac1.Evaluate(v[i].y),v[i].z);
				}
			}
		
			
			return outline;
		}		
	}

	
	public class FreeXStretchEffect : TTFTextOutlineEffect
	{
		public FreeXStretchEffect() {name="Free XStretch";}
		[System.Serializable]
		public class Parameters {
			public TTFTextStyle.SerializableAnimationCurve XMorph;
		}
		public  override object GetDefaultParameters() { Parameters p= new Parameters (); p.XMorph=new TTFTextStyle.SerializableAnimationCurve(); return p;}
		public override TTFTextOutline Apply(TTFTextOutline outline, object parameters) {
			AnimationCurve ac1=((TTFTextStyle.SerializableAnimationCurve)((parameters as Parameters).XMorph)).GetAnimcurve();
			
			foreach (Boundary v in outline.boundaries) {			
				int vlen = v.Count;

				for (int i = 0;  i < vlen; ++i) {
					v[i]=new Vector3(ac1.Evaluate(v[i].x),v[i].y,v[i].z);
				}
			}
		
			
			return outline;
		}		
	}
	
	
	public class NoisifyEffect : TTFTextOutlineEffect
	{
		public NoisifyEffect() {name="Noisify";}
		[System.Serializable]
		public class Parameters {
			public float Amount=0;
		}
		public  override object GetDefaultParameters() { Parameters p= new Parameters ();  return p;}
		public override TTFTextOutline Apply(TTFTextOutline outline, object parameters) {
			Parameters p=(parameters as Parameters);
			float amount=p.Amount/100f;
			foreach (Boundary v in outline.boundaries) {			
				int vlen = v.Count;

				for (int i = 0;  i < vlen; ++i) {
					v[i]+=new Vector3(Random.Range(-amount,amount),Random.Range(-amount,amount),Random.Range(-amount,amount));
				}
			}
		
			
			return outline;
		}		
	}
	
	
	
	
	
	
	public static TTFTextOutlineEffect [] AvailableOutlineEffects = {
		new EmboldenOBEffect(),
		new EmboldenN2Effect(),
		new FreeXStretchEffect(),
		new FreeYStretchEffect(),		
		new SimplifyEffect(),
		new KeypointsEffect(),
		new NoisifyEffect(),
		new PassBandEffect(),
		new EmboldenN2XEffect(),
	};
	
	private static string [] _AvailableOutlineEffectNames=null;
	public static string [] AvailableOutlineEffectNames {
		get {
			if (_AvailableOutlineEffectNames==null) {
				_AvailableOutlineEffectNames=new string[AvailableOutlineEffects.Length];
				for (int i=0;i<_AvailableOutlineEffectNames.Length;i++) {
					_AvailableOutlineEffectNames[i]=AvailableOutlineEffects[i].name;
				}
			}
			return _AvailableOutlineEffectNames;
		}
	}
	
	
	public TTFTextOutline Embolden(float strength) {
		TTFTextOutlineEffect i=AvailableOutlineEffects[1];
		//EmboldenN1Effect.Parameters p=i.GetDefaultParameters() as EmboldenN1Effect.Parameters;
		EmboldenN2Effect.Parameters p=i.GetDefaultParameters() as EmboldenN2Effect.Parameters;
		p.Amount=strength;
		return i.Apply(this,p);
	}
	
    #endregion
	
	

	
	
	
	#region CHECKS
	static bool Check (float f)
	{
		return !(float.IsNaN (f) || float.IsInfinity (f));
	}
	
	static bool Check (Vector3 v)
	{
		return Check (v.x) && Check (v.y) && Check (v.z);
	}
	
	public bool Check ()
	{
		foreach (Vector3 v in points) {
			if (! Check (v)) {
				Debug.LogError ("BAD Outline=" + v);
				return false;
			}
		}
		
		if (! Check (min)) {
			Debug.LogError ("Bad min=" + min);
			return false;
		}
		if (! Check (max)) {
			Debug.LogError ("Bad max=" + max);
			return false;
		}
		if (! Check (advance)) {
			Debug.LogError ("Bad adv=" + advance);
		}
		
		return true;
	}
    #endregion
}
