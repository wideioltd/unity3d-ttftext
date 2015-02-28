//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TTFTextInternalMeshGenerators {
	
	// This method modifies the mesh extruded mesh by creating an extrusion in between the front face mesh and the back face mesh
	// the outlines may be decimated/simplified qccouting to a vertice_filter
	public static void ExtrudeOutlines(ref Mesh extrudedMesh,
								Mesh front, 
								Mesh back, 
								TTFTextOutline[] outlines, 
								bool[] vertice_filter
								) {
		extrudedMesh.Clear();
		
		if (outlines.Length <= 1) {
			Debug.LogError("Extrude not enough outlines :" + outlines.Length);
			return ;
		}
		
		
		int[] frontTriangles = front != null ? front.triangles : new int[0];
		Vector3[] frontVertices = front != null ? front.vertices : new Vector3[0];		
		int[] backTriangles = back.triangles;
		Vector3[] backVertices = back.vertices;
		
		
		int npos=0;		
		for (int i = 0; i < vertice_filter.Length; i++) {
			if (vertice_filter[i]) {
				npos++;
			}
		}
		
		Vector3[] vertices = new Vector3[outlines.Length * npos + frontVertices.Length + backVertices.Length];
		int[] et = new int[(outlines.Length - 1) * npos * 6 + frontTriangles.Length + backTriangles.Length];

	    int cnt=0;
		for (int i = 0; i < outlines.Length; i++) {
			if (outlines[i].numVertices != vertice_filter.Length) {
				throw new System.Exception("Incompatible outlines");
			}
			
			int cj=0;
			foreach(TTFTextOutline.Boundary vl in outlines[i].boundaries) {
				for(int ii=0;ii<vl.Count;ii++) {
					if (vertice_filter[cj]) {
			      		vertices[cnt++]=vl[ii];			      		
					}
					cj++;
				}
			}
			if (cj!=outlines[i].numVertices) { Debug.Log("Error in vertice count");}	
		}
		
		int frontIdx = cnt;
		int backIdx = frontIdx + frontVertices.Length;
		
		System.Array.Copy(frontVertices, 0, vertices, frontIdx, frontVertices.Length);
		System.Array.Copy(backVertices, 0, vertices, backIdx, backVertices.Length);
		
		int xcnt,ecnt=0;
		int j,jb; 
	    for (int i=0;i<(outlines.Length-1);i++) {				
			j=0;
		    xcnt=0;
			foreach(TTFTextOutline.Boundary vl in outlines[i].boundaries) {				
				if (vl.Count<=1) {
					Debug.LogWarning("Pathlogical boundary");
				}
				jb=j; // the first index of that boundary
				
				for(int kc=0;kc<vl.Count;kc++) {
					if (vertice_filter[xcnt]) { 
						et[ecnt++]=npos*i+(j+1); 									
						et[ecnt++]=npos*i+j;
						et[ecnt++]=npos*(i+1)+j;

						et[ecnt++]=npos*(i+1)+(j+1);				
						et[ecnt++]=npos*i+(j+1);					
						et[ecnt++]=npos*(i+1)+j;
						
						j++;
					}
					xcnt++;
				}
				
				
				if (ecnt>=6) {
				// fixup last triangle
				et[ecnt-6]=npos*i+jb;									
				et[ecnt-5]=npos*i+(j-1);					
				et[ecnt-4]=npos*(i+1)+(j-1);

				et[ecnt-3]=npos*(i+1)+jb;
				et[ecnt-2]=npos*i+jb;					
				et[ecnt-1]=npos*(i+1)+(j-1);
				}
				
			}			
		}
		
		// reindex front side		
		for (int i = 0; i < frontTriangles.Length; ++i) {
			frontTriangles[i] = frontTriangles[i] + frontIdx;
		}
		
		// reindex back side
		for (int i = 0; i < backTriangles.Length; ++i) {
			backTriangles[i] = backTriangles[i] + backIdx;
		}
		
		extrudedMesh.name= "Extruded";
		extrudedMesh.vertices = vertices;
		extrudedMesh.subMeshCount = 3;
		extrudedMesh.SetTriangles(frontTriangles, 0);
		extrudedMesh.SetTriangles(et, 1);
		extrudedMesh.SetTriangles(backTriangles, 2);
		extrudedMesh.RecalculateNormals();
		//extrudedMesh.Optimize();
		//extrudedMesh.RecalculateBounds();
	}

	
	// This is an alternative way of generating a mesh from a contour 
	// TODO: Improve /ensure pipe orientation is correct
	public static void Piped(ref Mesh extrudedMesh, TTFTextOutline outline , float r, int nr) {		
	    extrudedMesh.Clear();
		if (outline.numVertices < 2) { return; }
		
		int nc=outline.numVertices * nr;

		Vector3 [] vertices=new Vector3[nc];
		Vector2 [] uvs=new Vector2[nc];
		int []et =new int[nc*6];


	    int cnt=0;
		int ecnt=0;

		foreach(TTFTextOutline.Boundary contour in outline.boundaries) {
			if (contour.Count>=2) {
			Vector3 pv=contour[contour.Count-1];
			Vector3 cv=contour[0];
			Vector3 nv=contour[1];
			int bcnt=cnt;
			for(int i=0;i<contour.Count;i++) {
				//Vector3 ov=(nv-pv);
				Quaternion q1=Quaternion.FromToRotation((nv-cv),(cv-pv));				
				Quaternion q2=Quaternion.Slerp(Quaternion.identity,q1,0.5f);
				Vector3 ov=q2*(nv-cv);
				ov.Normalize();				
				for (int j=0;j<nr;j++) {
					//vertices[cnt]=cv+Quaternion.AngleAxis(j*360f/nr,ov)*Quaternion.Euler(0,90,90)*ov*r;
					vertices[cnt]=cv+Quaternion.AngleAxis(j*360f/nr,ov)*Vector3.forward*r; 
					uvs[cnt]=Vector2.zero;
					int ni=(i+1)%contour.Count;
					  et[ecnt++]=bcnt+nr*i+((j+1)%nr);									
					  et[ecnt++]=bcnt+nr*i+j;
					  et[ecnt++]=bcnt+nr*ni+j;
					  et[ecnt++]=bcnt+nr*ni+((j+1)%nr);				
					  et[ecnt++]=bcnt+nr*i+((j+1)%nr);					
					  et[ecnt++]=bcnt+nr*ni+j;
					cnt++;
					
				}
				
				
				pv=cv;
				cv=nv;
				nv=contour[(i+2)%contour.Count];
			}
			}
		}
		
		extrudedMesh.Clear();
		extrudedMesh.name= "piped";
		extrudedMesh.vertices = vertices;
		extrudedMesh.uv = uvs;
		extrudedMesh.subMeshCount = 1;
		extrudedMesh.SetTriangles(et, 0);
		extrudedMesh.RecalculateNormals();
	}

	
}