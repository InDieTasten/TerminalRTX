using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace TerminalRTX
{
    public class Program
    {
        static float fov = (float)Math.PI / 3f;
        static Vector3[] framebuffer;
        static Random random = new Random();

        static void Main(string[] args)
        {
            framebuffer = new Vector3[0];

            var scene = new Scene
            {
                Objects = new Sphere[]
                {
                    new Sphere
                    {
                        Center = new Vector3(4, 0, -10),
                        Radius = 3,
                        Material = new RenderMaterial
                        {
                            DiffuseColor = new Vector3(0.2f, 0, 0)
                        }
                    }
                }
            };

            while (true)
            {
                Render(scene);
                Display();
            }
        }

        public static void Render(Scene scene)
        {
            framebuffer = new Vector3[Console.WindowHeight * Console.WindowWidth * 2];
            var origin = new Vector3();

            try
            {
                for (int j = 0; j < Console.WindowHeight * 2; j++)
                {
                    for (int i = 0; i < Console.WindowWidth; i++)
                    {
                        var x = (2 * (i + 0.5f) / Console.WindowWidth - 1) * (float)Math.Tan(fov / 2) * Console.WindowWidth / (Console.WindowHeight * 2);
                        var y = (2 * (j + 0.5f) / (Console.WindowHeight * 2) - 2) * (float)Math.Tan(fov / 2);
                        var direction = Vector3.Normalize(new Vector3(x, y, -1));

                        framebuffer[i + j * Console.WindowWidth] = CastRay(origin, direction, scene);
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
            }
        }

        private static Vector3 CastRay(Vector3 origin, Vector3 direction, Scene scene)
        {
            float closestDistance = float.MaxValue;
            Vector3? closestIntersect = null;
            IRenderObject closestObject = null;

            foreach (var renderObject in scene.Objects)
            {
                var intersection = renderObject.FindIntersect(origin, direction);
                if (intersection.HasValue)
                {
                    var distance = Vector3.Distance(Vector3.Zero, intersection.Value);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestIntersect = intersection;
                        closestObject = renderObject;
                    }
                }
            }

            if (closestObject != null)
            {
                return closestObject.Material.DiffuseColor;
            }
            else
            {
                return new Vector3(0.2f, 0.5f, 0.5f);
            }
        }

        //public static void Render()
        //{
        //    framebuffer = new Vector3[Console.WindowHeight * Console.WindowWidth * 2];
        //    for (int i = 0; i < framebuffer.Length; i++)
        //    {
        //        framebuffer[i] = new Vector3(
        //            (float)random.NextDouble(),
        //            (float)random.NextDouble(),
        //            (float)random.NextDouble());
        //    }
        //}

        public static void Display()
        {
            // TODO move normalization of color values into castRay method

            string charBuffer = "";

            try
            {
                for (int y = 0; y < Console.WindowHeight * 2; y += 2)
                {
                    for (int x = 0; x < Console.WindowWidth; x++)
                    {
                        var topPixel = NormalizePixelComponents(framebuffer[y * Console.WindowWidth + x]);
                        var bottomPixel = NormalizePixelComponents(framebuffer[(y + 1) * Console.WindowWidth + x]);
                        charBuffer += (SetTextColor(topPixel.X, topPixel.Y, topPixel.Z)
                            + SetBackgroundColor(bottomPixel.X, bottomPixel.Y, bottomPixel.Z)
                            + '\u2580');
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                // swallow and pray for the best next frame
            }
            Console.SetCursorPosition(0, 0);
            Console.Out.Write(charBuffer);
            Console.Out.Flush();
        }

        public static Vector3 NormalizePixelComponents(Vector3 pixel)
        {
            float maxComponent = new float[]{
                pixel.X,
                pixel.Y,
                pixel.Z
            }.Max();

            if (maxComponent > 1)
            {
                return pixel * (1 / maxComponent);
            }
            else
            {
                return pixel;
            }
        }

        public static string SetTextColor(float r, float g, float b)
            => $"\x1b[38;2;{Math.Floor(r * 255)};{Math.Floor(g * 255)};{Math.Floor(b * 255)}m";
        public static string SetBackgroundColor(float r, float g, float b)
            => $"\x1b[48;2;{Math.Floor(r * 255)};{Math.Floor(g * 255)};{Math.Floor(b * 255)}m";
    }
}
