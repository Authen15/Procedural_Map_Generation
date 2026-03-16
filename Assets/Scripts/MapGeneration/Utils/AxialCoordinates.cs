using System;
using UnityEngine;

public class AxialCoordinates
{
    public int Q;  // top left to bot right diagonal   \
    public int R;           // col
    public int S => -Q-R;           // bot-left to top right diagonal   /

    public AxialCoordinates(int q, int r)
    {
        Q = q;
        R = r;
    }

    public int RingNumber()
    {
        return Math.Max(Math.Max(Math.Abs(Q), Math.Abs(R)), Math.Abs(S));
    }

    public float HexAxialDistance(int q1, int r1)
    {
        return (Mathf.Abs(q1 - Q) + Mathf.Abs(q1 + r1 - Q - R) + Mathf.Abs(r1 - R)) / 2f;
    }

    public float Norm()
    {
        return (Mathf.Abs(Q) + Mathf.Abs(- Q - R) + Mathf.Abs(R)) / 2f;
    }

    public static AxialCoordinates operator +(AxialCoordinates a, AxialCoordinates b)
    {
        return new AxialCoordinates(a.Q + b.Q, a.R + b.R);
    }

    public static AxialCoordinates operator -(AxialCoordinates a, AxialCoordinates b)
    {
        return new AxialCoordinates(a.Q - b.Q, a.R - b.R);
    }

    public static AxialCoordinates operator *(AxialCoordinates a, int b)
    {
        return new AxialCoordinates(a.Q * b, a.R * b);
    }
    
    public static bool operator ==(AxialCoordinates a, AxialCoordinates b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.Q == b.Q && a.R == b.R;
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
        return (Q, R).GetHashCode();
    }

    public override string ToString()
    {
        return "(" + Q + ", " + R + ")";
    }
}