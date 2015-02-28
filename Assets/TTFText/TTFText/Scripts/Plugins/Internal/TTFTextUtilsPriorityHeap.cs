using System.Collections.Generic;

namespace TTFTextInternal {

public class PriorityHeap<T> where T : System.IComparable<T> {

	List<T> q_ = new List<T>();
	
	public bool Empty { get { return q_.Count == 0; }}
	public int Count { get { return q_.Count; }}
	
	public void Clear() { q_.Clear(); }
	
	
	public void Add(T item) {
		
		int n = q_.Count;
		q_.Add(item);
		
		// buble up item until it's not less than its parent
		while (n != 0) {
			// dad is floor((n-1)/2)
			int p =  (n - 1) / 2;
	
			if (q_[n].CompareTo(q_[p])>=0) { // condition ok
				break;
			}

			// else swap item with its parent
			T tmp = q_[n]; q_[n] = q_[p]; q_[p] = tmp;
			n = p;
		}
	}
	
	public T Peek() {
		return q_[0];
	}
	
	public T Pop() {
		
		T val = q_[0];
		int nMax = q_.Count - 1;
		
		// move the last element to the top
		q_[0] = q_[nMax];
		q_.RemoveAt(nMax);
		
		int p = 0;
		while (true) {
			// child are 2*p+1, 2*p+2
			int c = p * 2 + 1;
			
			if (c >= nMax) break; // no child
			
			// c, c+ 1 are both child for p
			
			if (c + 1 < nMax && q_[c+1].CompareTo(q_[c]) < 0) {
				c++; // compare with the smaller of the two
			}
			
			if (q_[p].CompareTo(q_[c]) <= 0) { // condition ok
				break;
			}
			
			// swap p & c and go on
			T tmp = q_[c]; q_[c] = q_[p]; q_[p] = tmp;
			p = c;
		}
		
		return val;
	}
}

}