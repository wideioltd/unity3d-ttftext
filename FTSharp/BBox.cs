using System;
using System.Collections.Generic;
using System.Text;

namespace FTSharp
{
    public class BBox
    {

        public float xMin;
        public float yMin;
        public float xMax;
        public float yMax;

        public BBox()
        {
            xMin = 0;
            yMin = 0;
            xMax = 0;
            yMax = 0;
        }

        public BBox(FT.FT_BBox ft_bbox, float scale)
        {
            xMin = FT.F26Dot6toFloat(ft_bbox.xMin) * scale;
            yMin = FT.F26Dot6toFloat(ft_bbox.yMin) * scale;
            xMax = FT.F26Dot6toFloat(ft_bbox.xMax) * scale;
            yMax = FT.F26Dot6toFloat(ft_bbox.yMax) * scale;
        }


        public void Scale(float factor)
        {
            xMin *= factor;
            yMin *= factor;
            xMax *= factor;
            yMax *= factor;
        }

        public void Translate(Outline.Point p)
        {
            xMin += p.X;
            yMin += p.Y;
            xMax += p.X;
            yMax += p.Y;
        }

        public void Merge(BBox b)
        {
            xMin = Math.Min(xMin, b.xMin);
            yMin = Math.Min(yMin, b.yMin);
            xMax = Math.Max(xMax, b.xMax);
            yMax = Math.Max(yMax, b.yMax);
        }

        public override string ToString()
        {
            return "[" + xMin + "x" + yMin + "," + xMax + "x" + yMax + "]";
        }
    }
}
