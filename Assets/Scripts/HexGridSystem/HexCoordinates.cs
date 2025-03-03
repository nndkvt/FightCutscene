using UnityEngine;

[System.Serializable]
public struct HexCoordinates{

	public int x;
	public int y;
	public int z;

	public HexCoordinates (int x, int z) 
    {
		this.x = x;
		this.z = z;

		y = -x - z;
	}

    public static HexCoordinates FromOffsetCoordinates (int x, int z) 
    {
		return new HexCoordinates(x - z / 2, z);
	}

    public static HexCoordinates FromPosition (Vector3 position) 
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
		float y = -x;

        float offset = position.z / (HexMetrics.outerRadius * 3f);
		x -= offset;
		y -= offset;

		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(y);
		int iZ = Mathf.RoundToInt(-x -y);

        if (iX + iY + iZ != 0) 
        {
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(y - iY);
			float dZ = Mathf.Abs(-x -y - iZ);

			if (dX > dY && dX > dZ) {
				iX = -iY - iZ;
			}
			else if (dZ > dY) {
				iZ = -iX - iY;
			}
		}

		return new HexCoordinates(iX, iZ);
	}

	public static int CalculateDistance(HexCoordinates start, HexCoordinates target)
	{
		int xDiff = Mathf.Abs(start.x - target.x);
		int yDiff = Mathf.Abs(start.y - target.y);
		int zDiff = Mathf.Abs(start.z - target.z);

		return (xDiff + yDiff + zDiff) / 2;
	}

    public override string ToString () 
    {
		return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
	}

	public static bool operator ==(HexCoordinates a, HexCoordinates b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator !=(HexCoordinates a, HexCoordinates b)
	{
		return a.x != b.x || a.y != b.y || a.z != b.z;
	}

	public string ToStringOnSeparateLines () 
    {
		return x.ToString() + "\n" + y.ToString() + "\n" + z.ToString();
	}
}