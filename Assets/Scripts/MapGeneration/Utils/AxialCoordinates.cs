public class AxialCoordinates
{
    public int x;
    public int z;

    public AxialCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static AxialCoordinates operator +(AxialCoordinates a, AxialCoordinates b)
    {
        return new AxialCoordinates(a.x + b.x, a.z + b.z);
    }

    public static AxialCoordinates operator *(AxialCoordinates a, int b)
    {
        return new AxialCoordinates(a.x * b, a.z * b);
    }
    
    public static bool operator ==(AxialCoordinates a, AxialCoordinates b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        return a.x == b.x && a.z == b.z;
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
        return (x, z).GetHashCode();
    }

    public override string ToString()
    {
        return "(" + x + ", " + z + ")";
    }
}