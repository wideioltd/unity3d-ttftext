using System;
using System.Collections.Generic;

namespace Triangulation
{
    public class Mesh
    {

        public List<Vertex> Vertices;
        public List<int> Edges;

        public Mesh()
        {
            Vertices = new List<Vertex>();
            Edges = new List<int>();
        }

        public Mesh (List<Vertex> v, List<int> e)
        {
            Vertices = new List<Vertex>(v);
            Edges = new List<int>(e);
            
            //Console.WriteLine("new Mesh {0} vertices {1} edges", Vertices.Count, Edges.Count);
            
            if (Edges.Count % 3 != 0) {
               // Console.WriteLine ("ERROR: Edge.Count = " + Edges.Count);
            }
        }
    }
}

