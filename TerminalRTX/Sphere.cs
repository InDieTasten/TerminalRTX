using System;
using System.Numerics;

namespace TerminalRTX
{
    public class Sphere : IRenderObject
    {
        public Vector3 Center { get; set; }
        public float Radius { get; set; }
        public RenderMaterial Material { get; set; }

        public Vector3? FindIntersect(Vector3 position, Vector3 direction)
        {
            var oc = position - Center;
            var a = Vector3.Dot(direction, direction);
            var b = 2 * Vector3.Dot(oc, direction);
            var c = Vector3.Dot(oc, oc) - Radius * Radius;
            var discriminant = b * b - 4 * a * c;
            
            if (discriminant < 0)
                return null;

            var distance = (-b - Math.Sqrt(discriminant)) / (2 * a);

            if (distance <= 0)
                return null;

            return position + direction * (float)distance;
        }

        public Vector3 GetNormal(Vector3 point)
        {
            return Vector3.Normalize(point - Center);
        }
    }
}
