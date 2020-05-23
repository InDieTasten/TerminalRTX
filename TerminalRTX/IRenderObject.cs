using System.Numerics;

namespace TerminalRTX
{
    public interface IRenderObject
    {
        RenderMaterial Material { get; set; }
        Vector3? FindIntersect(Vector3 position, Vector3 direction);
    }
}
