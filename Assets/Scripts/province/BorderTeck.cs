using System;

namespace province
{
    public class BorderTeck
    {
        public int provinceA;
        public int provinceB;

        public BorderTeck(int provinceA, int provinceB)
        {
            if (provinceA > provinceB)
            {
                var temp = provinceB;
                provinceB = provinceA;
                provinceA = temp;
            }
            this.provinceA = provinceA;
            this.provinceB = provinceB;
        }

        public BorderTeck()
        {
        }

        protected bool Equals(BorderTeck other)
        {
            return (provinceA == other.provinceA && provinceB == other.provinceB);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BorderTeck) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (provinceA * 397) ^ provinceB;
            }
        }

        public override string ToString()
        {
            return String.Format("{0},{1}",provinceA,provinceB);
        }
    }
    
    
}