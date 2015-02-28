using UnityEngine;
using System.Collections.Generic;

namespace TTFTextInternal {

public static class Intersection {
	

	// All vertex should have the same z coordinate
	
	// returns 0 if c in on the line (a,b)
	//         >0 if c is on the right of (a,b)
	//         <0 if c is on the left
	static float isLeft(Vector3 a, Vector3 b, Vector3 c) {
		return (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
	}
	
	
	public static bool intersects(Vector3 u1, Vector3 u2, Vector3 v1, Vector3 v2) {
			
		//Debug.Log("Test for intersect");
			
		// test for existence of an intersect point
		float lsign = Intersection.isLeft(u1, u2, v1);    // s2 left point sign
		float rsign = isLeft(u1, u2, v2);    // s2 right point sign
		
		if (lsign * rsign > 0) // s2 endpoints have same sign relative to s1
			return false;      // => on same side => no intersect is possible
			
		lsign = isLeft(v1, v2, u1);    // s1 left point sign
		rsign = isLeft(v1, v2, u2);    // s1 right point sign
		
		if (lsign * rsign > 0) // s1 endpoints have same sign relative to s2
			return false;      // => on same side => no intersect is possible
			
		// the segments s1 and s2 straddle each other
		return true;           // => an intersect exists
			
	}
	
	//http://www.exaflop.org/docs/cgafaq/cga1.html#Subject 1.03: How do I find intersections of 2 2D line segments?
	
	public static Vector3 GetIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
		
		float r = ((a.y - c.y) * (d.x - c.x) - (a.x - c.x) * (d.y - c.y)) 
			    / ((b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x)); 
	
		if (float.IsNaN(r)) {  // Segments are paralllel !@#!!!
		
			// Assume a < b && c < d
			
			if (compareVector3(a, c) < 0) {
				if (b == c) {
					return b;
				} else{
					throw new System.ArgumentException("Parallel segments");
				}
			} else {
				if (d == a) {
					return d;
				} else {
					throw new System.ArgumentException("Parallel segments");
				}
			}
		}
		
		return a + r * (b - a);
	}
	
	
	public static bool GetIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 I) {
		
		float r = ((a.y - c.y) * (d.x - c.x) - (a.x - c.x) * (d.y - c.y)) 
			    / ((b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x)); 
		
		if (float.IsNaN(r)||float.IsInfinity(r)) { //segment parallell
			
			if (compareVector3(a, c) < 0) {
				if (b == c) {
					I = b;
					return true;
				} else{
					I = Vector3.zero;
					return false;
				}
				
			} else {
				if (d == a) {
					I = d;
					return true;
				} else {
					I = Vector3.zero;
					return false;
				}				
			}
		}
		
		I = a + r * (b - a);
		return true;
		/*
		if (intersects(a,b, c,d)) {
			I = GetIntersection(a,b,c,d);
			return true;
		} else {
			I = Vector3.zero;
			return false;
		}
		*/
	}
	
	static int compareVector3(Vector3 v1, Vector3 v2) {
		if (v1.x > v2.x) { return 1; }
		if (v1.x < v2.x) { return -1; }
		if (v1.y > v2.y) { return 1; }
		if (v1.y < v2.y) { return -1; }
		return 0;
	}
	
	class VertexEv : System.IComparable<VertexEv> {
		
		public enum vType { LEFT, RIGHT, INTERSECT }
		
		public int edge;
		public int edge2; // for intersection point
		public vType type;
		Vector3 v;
			
		public VertexEv(Vector3 vertex, int edg, vType typ) { // segment end
			v = vertex;
			edge = edg;
			type = typ;
		}
		
		public VertexEv(Vector3 vertex, int e1, int e2, vType typ) { // intersection point
			v = vertex;
			edge = e1;
			edge2 = e2;
			type = typ;
		}
		
		public VertexEv(IntersectionPoint I) {
			v = I.P;
			edge = I.Edge1;
			edge2 = I.Edge2;
			type = vType.INTERSECT;
		}
		
		public int CompareTo(VertexEv other) {
			
			int res = Intersection.compareVector3(v, other.v);
			
			if ( res != 0) { return res; }
			
			if (type != other.type) {
				
				if (type == vType.LEFT) { // left < intersect < right
					return -1;
					
				} else if (other.type == vType.LEFT) { 
					return 1;
			
				} else if (type == vType.INTERSECT) {
				
					return -1;
			
				} else {
				
					return 1;
				}
			}
			
			return 0;
		}
	}

	
	static PriorityHeap<VertexEv> setupEvQueue(IList<Vector3> poly) {
		
		PriorityHeap<VertexEv> queue = new PriorityHeap<VertexEv>();
		
		for (int i = 0; i < poly.Count; ++i) {

			Vector3 pv = poly[i];
			Vector3 nv = poly[(i+1) % poly.Count];
			
			// [v_i, v_i+1]
			VertexEv ev1 = new VertexEv(pv, i, VertexEv.vType.LEFT);
			VertexEv ev2 = new VertexEv(nv, i, VertexEv.vType.LEFT);
			
			if (compareVector3(pv, nv) < 0) { // pv is left of nv
				ev2.type = VertexEv.vType.RIGHT;
			} else {
				ev1.type = VertexEv.vType.RIGHT;
			}
		
			queue.Add(ev1);
			queue.Add(ev2);
		}
		return queue;
	}
	
	class IntersectionPoint {
		
		public Vector3 P;
		// always edge1 < edge2
		public int Edge1; 
		public int Edge2;
		
		public IntersectionPoint(Vector3 v, int e1, int e2) {
			P = v;
			Edge1 = Mathf.Min(e1, e2);
			Edge2 = Mathf.Max(e1, e2);
		}
		
		public int Id(int n) {
			return Edge1 * n + Edge2;
		}
	}
	
	class SweepLine {
		
		public class Segment {

			public int Edge { get; private set; }
			public Vector3 Left { get; private set; }
			public Vector3 Right { get; private set; }
			public float Tan { get; private set; }

			public Segment(int edge, IList<Vector3> Pn) {
				
				Edge = edge;
				
				Vector3 v1 = Pn[edge];
				Vector3 v2 = Pn[(edge+1)% Pn.Count];
				
				if (compareVector3(v1, v2) <= 0) {
					Left = v1;
					Right = v2;
				} else {
					Left = v2;
					Right = v1;
				}
				
				Tan = (Right.y - Left.y) / (Right.x - Left.x);
			}
			
			public float Y(float x) {
				return Left.y + (x - Left.x) * Tan;
			}
		}
		
		IList<Vector3> Pn; // the polygon
		List<Segment> currSegs; // Tree would be better for insertion
		
		
		public int Count { get { return currSegs.Count; }}
		
		
		public SweepLine(IList<Vector3> Poly) {
			Pn = Poly;
			currSegs = new List<Segment>();
		}
		
		public int Add(int edge) {
			
			Segment seg = new Segment(edge, Pn);

			float currX = seg.Left.x;
			float currY = seg.Left.y;
			
			int idx = 0;
			while (idx < currSegs.Count && currSegs[idx].Y(currX) < currY) { //linear search
				++idx;
			}
			
			currSegs.Insert(idx, seg);
			return idx;
		}

		public void Remove(int idx) {
			currSegs.RemoveAt(idx);
		}
		
		
		
		public int Find(int edge) {
			for (int i = 0; i < currSegs.Count; ++i) {
				if (currSegs[i].Edge == edge) { return i; }
			}
			//LOG.ERR("Edge " + edge + " Not FOund");
			return -1;
		}
		
		public bool intersect(int a, int b) {
			
			if (a < 0 || a >= currSegs.Count || b < 0 || b >= currSegs.Count) { return false; }
			
			Segment s1 = currSegs[a];
			Segment s2 = currSegs[b];
			
			int e1 = s1.Edge, e2 = s2.Edge;
			
			if ( (((e1 + 1) % Pn.Count) == e2) || (e1 == ((e2 +1) % Pn.Count))) { // consecutive edges
				return false;
			}
			
			return Intersection.intersects(s1.Left, s1.Right, s2.Left, s2.Right);
		}
		
		// assumes intersect(idx1, idx2) == true
		public IntersectionPoint GetIntersectionPoint(int idx1, int idx2) {
			
			if (! intersect(idx1, idx2)) { return null; }
			
			Segment s1 = currSegs[idx1];
			Segment s2 = currSegs[idx2];
			
			Vector3 P = Intersection.GetIntersection(s1.Left, s1.Right, s2.Left, s2.Right);
			//Debug.Log("Intersec("+s1.Left +","+s1.Right+","+s2.Left+","+s2.Right+"):" +P + "(" + s1.Edge + "," + s2.Edge + ")");
			return new IntersectionPoint(P, s1.Edge, s2.Edge);
		}
	}
	
	// brute force implementation
	public static bool isSimple0(IList<Vector3> poly) {
		for (int i = 0; i < poly.Count; ++i) {
			Vector3 v1 = poly[i];
			Vector3 v2 = poly[(i+1) % poly.Count];
			
			for (int j = i + 2; j < poly.Count; ++j) {
			
				Vector3 u1 = poly[j]; 
				Vector3 u2 = poly[(j+1) % poly.Count];
				
				if (i == 0 && (j + 1) == poly.Count) { break; }
			
				if (intersects(v1, v2, u1, u2)) { return false; }
			}
		}
		return true;
	}
	
	public static bool isSimple(IList<Vector3> poly) {
		
		PriorityHeap<VertexEv> evQueue = setupEvQueue(poly);
		SweepLine SL = new SweepLine(poly);
		
		//Debug.LogWarning("Queue size=" + evQueue.Count);
		
		while (! evQueue.Empty) {
			
			VertexEv ev = evQueue.Pop();
			
			if (ev.type == VertexEv.vType.LEFT) { 
			
				
				int idx = SL.Add(ev.edge);
				
				//Debug.Log("Left: " + idx + "/" + SL.Count);
				
				if (SL.intersect(idx, idx - 1)) { return false; }
				if (SL.intersect(idx, idx + 1)) { return false; }
			
			} else { // right vertex
			
				
				int idx = SL.Find(ev.edge);
				
				//Debug.Log("Right: " + idx + "/" + SL.Count);
				
				if (SL.intersect(idx - 1, idx + 1)) { return false; }
				SL.Remove(idx);
			}
		}
		
		return true;
	}
	
	public static void BentleyOttmann(IList<Vector3> poly) {
		
		PriorityHeap<VertexEv> evQ = setupEvQueue(poly);
		SweepLine SL = new SweepLine(poly);
		
		Dictionary<int, IntersectionPoint> intersectionPoints = new Dictionary<int, IntersectionPoint>();
		
		IntersectionPoint I;
		
		while (! evQ.Empty) {
			
			VertexEv ev = evQ.Pop();
			
			if (ev.type == VertexEv.vType.LEFT) {
				
				int idx = SL.Add(ev.edge);
				
				I = SL.GetIntersectionPoint(idx, idx -1);
				
				if (I != null) { // Intersection
					
					int ID = I.Id(poly.Count);
					if (! intersectionPoints.ContainsKey(ID)) {
						intersectionPoints[ID] = I;
						//evQ.Add(new VertexEv(I.v, I.Edge1, I.Edge2, VertexEv.vType.INTERSECT));
						evQ.Add(new VertexEv(I));
					}
				}
				
				
				I = SL.GetIntersectionPoint(idx, idx + 1);
				
				if (I != null) { // intersection
				
					int ID = I.Id(poly.Count);
					if (! intersectionPoints.ContainsKey(ID)) {
						intersectionPoints[ID] = I;
						evQ.Add(new VertexEv(I.P, I.Edge1, I.Edge2, VertexEv.vType.INTERSECT));
					}
				}
			
			} else if (ev.type == VertexEv.vType.RIGHT) {
				
				int idx = SL.Find(ev.edge);
				
				I = SL.GetIntersectionPoint(idx - 1, idx + 1);
				
				if (I != null) { 
				
					int ID = I.Id(poly.Count);
					if (! intersectionPoints.ContainsKey(ID)) {
						intersectionPoints[ID] = I;
						evQ.Add(new VertexEv(I));
					}
				}
				
			} else { // intersection point
				
				// unfinished business ...
			}
		}
	}
	/*
	List<List<Vector3>> decompose(List<Vector3> poly) {
		
	}
	*/
	
	// returns one intersection point or null if polygon is simple

	static IntersectionPoint oneIntersection(IList<Vector3> poly) {
		
		PriorityHeap<VertexEv> evQueue = setupEvQueue(poly);
		SweepLine SL = new SweepLine(poly);
		
		IntersectionPoint I;
		
		while (! evQueue.Empty) {
			
			VertexEv ev = evQueue.Pop();
			
			if (ev.type == VertexEv.vType.LEFT) { 
			
				int idx = SL.Add(ev.edge);
				
				I = SL.GetIntersectionPoint(idx, idx - 1);
				if (I != null) { return I; }
				
				I = SL.GetIntersectionPoint(idx, idx + 1);
				if (I != null) { return I; }
			
			} else { // right vertex
			
				int idx = SL.Find(ev.edge);
				
				I = SL.GetIntersectionPoint(idx - 1, idx + 1);
				if (I != null) { return I; }
				SL.Remove(idx);
			}
		}
		
		return null;
	}


	public static IEnumerable<List<Vector3>> Decompose(IList<Vector3> poly) {
		
		Queue<List<Vector3>> Q = new Queue<List<Vector3>>();
		
		Q.Enqueue(new List<Vector3>(poly));
		
		while (Q.Count != 0) {
			
			List<Vector3> P = Q.Dequeue();
			
			IntersectionPoint I = oneIntersection(P);
			
			if (I != null) { // decompose p into p1 and p2
				
				//Debug.LogWarning("INTERSECTION=" + I.P);
				
				List<Vector3> p1 = new List<Vector3>();
				List<Vector3> p2 = new List<Vector3>();
				
				// p1
				for (int i = 0; i <= I.Edge1; ++i) {
					p1.Add(P[i]);
				}
				p1.Add(I.P);
				for (int i = I.Edge2 + 1; i < P.Count; ++i) {
					p1.Add(P[i]);
				}

				//P2
				p2.Add(I.P);
				for (int i = I.Edge1 + 1; i <= I.Edge2; ++i) {
					p2.Add(P[i]);
				}
				
				Q.Enqueue(p1);
				Q.Enqueue(p2);
				
			} else { // P is simple
				yield return P;
			}
		}
	}
	
	// dummy function
	public static IEnumerable<IList<Vector3>> DontDecompose(IList<Vector3> poly) {
		yield return poly;
	}
}

	
};