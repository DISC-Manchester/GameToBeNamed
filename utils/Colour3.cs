using System;

namespace SquareSmash.utils
{
    [Serializable]
    public readonly struct Colour3
    {
        public readonly float R;
        public readonly float G;
        public readonly float B;

        public Colour3(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Colour3(byte r, byte g, byte b)
        {
            R = r / 255f;
            G = g / 255f;
            B = b / 255f;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Colour3 colour)
                return Equals(colour);
            return false;
        }

        public bool Equals(Colour3 other)
        {
            if (R == other.R && G == other.G)
                return B == other.B;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B);
        }

        public static bool operator ==(Colour3 left, Colour3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Colour3 left, Colour3 right)
        {
            return !(left == right);
        }
    }
}
