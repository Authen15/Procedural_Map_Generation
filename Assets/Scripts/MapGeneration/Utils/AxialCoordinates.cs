using System;

public class AxialCoordinates
{
    public int S;           // bot-left to top right diagonal   /
    public int R;           // col
    public int Q => -R -S;  // top left to bot right diagonal   \

    public AxialCoordinates(int x, int z)
    {
        S = x;
        R = z;
    }

    public int MaxAbs()
    {
        return Math.Max(Math.Max(Math.Abs(S), Math.Abs(R)), Math.Abs(Q));
    }

    public static AxialCoordinates operator +(AxialCoordinates a, AxialCoordinates b)
    {
        return new AxialCoordinates(a.S + b.S, a.R + b.R);
    }

    public static AxialCoordinates operator -(AxialCoordinates a, AxialCoordinates b)
    {
        return new AxialCoordinates(a.S - b.S, a.R - b.R);
    }

    public static AxialCoordinates operator *(AxialCoordinates a, int b)
    {
        return new AxialCoordinates(a.S * b, a.R * b);
    }
    
    public static bool operator ==(AxialCoordinates a, AxialCoordinates b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.S == b.S && a.R == b.R;
    }

    public static bool operator !=(AxialCoordinates a, AxialCoordinates b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is AxialCoordinates)) return false;
        AxialCoordinates other = (AxialCoordinates)obj;
        return this == other;
    }

    public override int GetHashCode()
    {
        return (S, R).GetHashCode();
    }

    public override string ToString()
    {
        return "(" + S + ", " + R + ")";
    }
}