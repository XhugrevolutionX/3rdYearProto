using UnityEngine;

public static class HexLayout
{
    // Pointy-top matrix constants
    private const float Sqrt3 = 1.7320508f;

    public static Vector3 HexToWorld(Hex hex, float size)
    {
        float x = size * (Sqrt3 * hex.Q + Sqrt3 / 2f * hex.R);
        float z = size * (3f / 2f * hex.R);
        return new Vector3(x, 0, z);
    }

    public static Hex WorldToHex(Vector3 pos, float size)
    {
        float q = (Sqrt3 / 3f * pos.x - 1f / 3f * pos.z) / size;
        float r = (2f / 3f * pos.z) / size;
        return RoundHex(q, r);
    }

    private static Hex RoundHex(float q, float r)
    {
        float s = -q - r;
        int rq = Mathf.RoundToInt(q);
        int rr = Mathf.RoundToInt(r);
        int rs = Mathf.RoundToInt(s);

        float dq = Mathf.Abs(rq - q);
        float dr = Mathf.Abs(rr - r);
        float ds = Mathf.Abs(rs - s);

        if (dq > dr && dq > ds) rq = -rr - rs;
        else if (dr > ds) rr = -rq - rs;

        return new Hex(rq, rr);
    }
}
