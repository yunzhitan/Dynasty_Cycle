using System.Collections.Generic;
using UnityEngine;

namespace province
{
    
    /**
     * border文件 StreamingAssets/province/border.txt
     * 每一行的前两个数字为边界连接的两个省份的ID，
     */
    public class Border
    {
        public BorderPair Pair { get; set; }
        public List<Point> pointList { get; set; }

        public Color kShowColor = Color.white;

        public Border(BorderPair pair, List<Point> pointList)
        {
            Pair = pair;
            this.pointList = pointList;
        }

        protected bool Equals(Border other)
        {
            return Equals(Pair, other.Pair) && Equals(pointList, other.pointList);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Border) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Pair != null ? Pair.GetHashCode() : 0) * 397) ^ (pointList != null ? pointList.GetHashCode() : 0);
            }
        }
    }
}