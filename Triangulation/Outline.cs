using System;
using System.Collections.Generic;
using System.Text;

namespace Triangulation
{
    public class Outline<V>
    {

        public enum PathType
        {
            MoveTo, LineTo
        }

        public List<V> Path;
        public List<PathType> Types;

        public Outline()
        {
            Path = new List<V>();
            Types = new List<PathType>();
        }


        public int Size
        {
            get { return Path.Count; }
        }

        public void AddVertex(V v, PathType type)
        {
            Path.Add(v);
            Types.Add(type);
        }

        public void MoveTo(V v)
        {
            AddVertex(v, PathType.MoveTo);
        }

        public void LineTo(V v)
        {
            AddVertex(v, PathType.LineTo);
        }

        public void AddOutline(Outline<V> outline)
        {
            for (int i = 0; i < outline.Size; ++i)
            {
                AddVertex(outline.Path[i], outline.Types[i]);
            }
        }
    }
}

