using System;

namespace province
{
    public class BorderPair
    {
        public int provinceA;
        public int provinceB;

        public BorderPair(int provinceA, int provinceB)
        {
            this.provinceA = provinceA;
            this.provinceB = provinceB;
        }

        public BorderPair()
        {
        }

        protected bool Equals(BorderPair other)
        {
            return (provinceA == other.provinceA && provinceB == other.provinceB);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BorderPair) obj);
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