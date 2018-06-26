using UnityEngine;

public static class MathsUtil {
    
    public static Vector2 GetSmallestFraction(int x, int y) {
        int divider = x;

        while (divider > 0)
        {
            if (y % divider == 0 && x % divider == 0)
            {
                return new Vector2(x / divider, y / divider);
            }

            divider--;
        }

        return new Vector2(x, y);
    }

    public static Vector3 GetBezierPosition(Vector3 startPos, Vector3 endPos, float t, float curveAmount) {
        Vector3 p0 = startPos;
        Vector3 p1 = p0 + new Vector3(0, curveAmount, 0);
        Vector3 p3 = endPos;
        Vector3 p2 = p3 + new Vector3(0, curveAmount, 0);

        // here is where the magic happens!
        return Mathf.Pow(1f - t, 3f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * t * p1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * p2 + Mathf.Pow(t, 3f) * p3;
    }

}
