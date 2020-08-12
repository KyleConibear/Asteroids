using UnityEngine;

namespace KyleConibear
{
    public static class ExtensionMethods
    {
        public static bool OutOfRange(this Transform transform, Vector3 target, float range)
        {
            float distance = Vector3.Distance(transform.position, target);
            return distance > range;
        }

        public static Vector3 GetBoundsExtents(this Collider collider, bool isScaled)
        {
            Vector3 bounds = collider.bounds.extents;

            if (isScaled)
                bounds *= collider.transform.localScale.x;
            return bounds;
        }

        /// <summary>
        /// Get the bounds of a collider in a float array
        /// </summary>
        /// <param name="collider">The collider which to receive the bounds</param>
        /// <returns>i0=minX, i1=maxX, i2=minY, i3=maxY, i4=minZ, i5=maxZ</returns>
        public static float[] Bounds(this Collider collider)
        {
            float[] bounds = new float[6];

            bounds[0] = collider.bounds.min.x;
            bounds[1] = collider.bounds.max.x;
            bounds[2] = collider.bounds.min.y;
            bounds[3] = collider.bounds.max.y;
            bounds[4] = collider.bounds.min.z;
            bounds[5] = collider.bounds.max.z;

            return bounds;
        }
    }
}