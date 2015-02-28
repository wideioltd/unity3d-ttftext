using UnityEngine;
using System.Collections.Generic;

namespace TTFTextInternal {
	class TextureMapManager : System.IDisposable {
	
		
	public static int maxMaps=10;	
	static LinkedList<TTFTextInternal.TextureMapManager> fontcache=new LinkedList<TTFTextInternal.TextureMapManager>();
	
	public static TTFTextInternal.TextureMapManager GetTextureMapManager(string fontprovider,string fontid,System.Func<char,TTFTextTexturePortion> gen) {
		int i=0;
		foreach(TTFTextInternal.TextureMapManager tam in fontcache) {
			if ((tam.fontprovider==fontprovider)&&(tam.name==fontid)) {
			  if (i==0) {
				return tam;
			  }
		   	  else {
				fontcache.Remove(tam);
				fontcache.AddFirst(tam);
				return tam;
			  }
			}
			i++;
			
		}
			
		TTFTextInternal.TextureMapManager xtam=new TextureMapManager(fontprovider, fontid,gen);
		fontcache.AddFirst(xtam);
		if (fontcache.Count>maxMaps) {
				fontcache.Last.Value.Dispose();
				fontcache.RemoveLast();
		}
		return xtam;
	}
		
		public string fontprovider;				
		public string name;
		public int UseCount=0;
		public bool updated=false;
		public string requested_characters="";
		
		private System.Collections.Generic.Dictionary<char , TTFTextTexturePortion> texs=new System.Collections.Generic.Dictionary<char, TTFTextTexturePortion>();
		private System.Collections.Generic.Dictionary<char , Rect > quads=new System.Collections.Generic.Dictionary<char, Rect>();						
		private System.Collections.Generic.Dictionary<char , Rect > rects=new System.Collections.Generic.Dictionary<char, Rect>();				
		private Texture2D packed_textures;
		private Material packed_material;

		public void Prepare(System.Func<char,TTFTextTexturePortion> texgen) {
			texs.Clear();
			foreach (char c in requested_characters) {
				texs.Add(c,texgen(c));
			}
			UpdatePackedTexture();
		}
		
		public TextureMapManager(string fp, string n) {
			fontprovider=fp;
			name=n;
			updated=true;
			for(byte i=0x20;i<0x7F;i++) {
					requested_characters+=System.Text.ASCIIEncoding.ASCII.GetChars(new byte [] {i})[0];
			}
		}
		public TextureMapManager(string fp, string n,System.Func<char,TTFTextTexturePortion> texgen) {
			fontprovider=fp;
			name=n;
			updated=true;
			for(byte i=0x20;i<0x7F;i++) {
					requested_characters+=System.Text.ASCIIEncoding.ASCII.GetChars(new byte [] {i})[0];
			}
			Prepare(texgen);
		}
		
		public void Reset() {
			packed_textures=null;
			texs.Clear();
			rects.Clear();
			requested_characters="";
		}
		
		public void AddRequestedCharacters(string s) {
			foreach(char c in s) {
				if (requested_characters.IndexOf(c)==-1) {
					updated=true;
					requested_characters+=c;
				}
			}
		}
		
		public void AddRequestedCharacters(params char [] c) {
			foreach(char cc in c) {
				if (requested_characters.IndexOf(cc)==-1) {
					updated=true;
					requested_characters+=cc;
				}
			}
		}
		
		public bool IsRequestedInMap(char c) {
			return (requested_characters.IndexOf(c)!=-1);
		}
		
		public TTFTextTexturePortion GetChar(char c) {
			//int idx=requested_characters.IndexOf(c);
			Rect q=quads[c];
			Rect r=rects[c];
			TTFTextTexturePortion portion=null;
			if (!packed_material) {
			  portion=new TTFTextTexturePortion(packed_textures,
				r.x,
				r.y,
				
				r.width,
				r.height,
				
				q.width,
				q.height,
				q.x,
				q.y,
				false
				);
			}
			else {
			  portion=new TTFTextTexturePortion(packed_material,
				r.x,
				r.y,				
				r.width,
				r.height,				
				q.width,
				q.height,
				q.x,
				q.y,
				false
				);
				
			}
			return portion;
		}
			
		
		public void EnablePackedMaterial(string s) {
			if (!packed_material) {
				packed_material=new Material(Shader.Find(s));
				packed_material.mainTexture=packed_textures;
			}
		}
		
		public void UpdatePackedTexture() {
			if (updated) {
			
			Texture2D [] texa = new Texture2D[texs.Count];
			int ci=0;
			foreach(TTFTextTexturePortion tex in texs.Values) {
				texa[ci++]=(Texture2D)tex.texture;
			}
				
				
			if (packed_textures==null) {
					packed_textures=new Texture2D(1,1,TextureFormat.RGBA32,false);	
				}
				
			Rect [] r=packed_textures.PackTextures(texa,0);
			
			rects.Clear();
			int i=0;
			foreach(char c in texs.Keys) {
				TTFTextTexturePortion portion=texs[c];
				rects.Add(c,r[i]);
				quads.Add(c,new Rect(portion.x,portion.y,portion.w,portion.h));
				i++;
				if (portion.shouldReleaseTexture) {					
					TTFTextInternal.Utilities.DestroyObj(portion.texture);
				}
			}
			
				
			texs.Clear();
			}
			
			
		}
		
	 public void IncRef() {
			UseCount++;
		}
		
		public void DecRef() {
			UseCount--;
		}
		
		public void Dispose() {
			TTFTextInternal.Utilities.DestroyObj(this.packed_textures);	
		}

	}
	
}
