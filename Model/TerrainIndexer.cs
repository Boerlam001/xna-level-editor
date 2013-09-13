using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EditorModel
{
    public class TerrainIndexer : Subject, IObserver
    {
        private int[,] indices;

        public int[,] Indices
        {
            get { return indices; }
            set { indices = value; }
        }

        private Vector3[] closestVertices;

        public Vector3[] ClosestVertices
        {
            get { return closestVertices; }
            set { closestVertices = value; }
        }

        private Vector3[] screenLocations;

        public Vector3[] ScreenLocations
        {
            get { return screenLocations; }
            set { screenLocations = value; }
        }

        private bool ready;

        public bool Ready
        {
            get { return ready; }
            set { ready = value; }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private Camera camera;
        private GraphicsDevice graphicsDevice;
        private Terrain terrain;
        private int length;
        private int width;
        private int height;

        public TerrainIndexer(Terrain terrain, Camera camera, GraphicsDevice graphicsDevice)
        {
            terrain.Attach(this);
            this.terrain = terrain;
            camera.Attach(this);
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;
            length = (graphicsDevice.Viewport.Width + 1) * (graphicsDevice.Viewport.Height + 1);
            //indices = new List<int>[length];
            width = graphicsDevice.Viewport.Width + 1;
            height = graphicsDevice.Viewport.Height + 1;
            indices = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    indices[i, j] = -1;
                }
            }

            closestVertices = new Vector3[2]
            {
                new Vector3(-1, -1, float.MaxValue),
                new Vector3(-1, -1, float.MaxValue)
            };

            screenLocations = new Vector3[terrain.Vertices.Length];
            ready = false;
            text = "";
        }

        public void Resize()
        {
            width = graphicsDevice.Viewport.Width + 1;
            height = graphicsDevice.Viewport.Height + 1;
            indices = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    indices[i, j] = -1;
                }
            }
        }

        private bool OutOfBounds(Vector3 pos)
        {
            return !(pos.X >= 0 && pos.X <= graphicsDevice.Viewport.Width && pos.Y >= 0 && pos.Y <= graphicsDevice.Viewport.Height);
        }

        public void UpdateObserver()
        {
            ready = false;
            try
            {
                if (camera.IsMoving)
                    return;

                int i;
                for (i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        indices[i, j] = -1;
                    }
                }

                float[] dists = new float[terrain.Vertices.Length];
                for (i = 0; i < terrain.Vertices.Length; i++)
                {
                    screenLocations[i] = graphicsDevice.Viewport.Project(terrain.Vertices[i].Position, camera.Projection, camera.World, Matrix.Identity);
                    if (!OutOfBounds(screenLocations[i]))
                    {
                        dists[i] = (terrain.Vertices[i].Position - camera.Position).Length();
                    }
                    else
                    {
                        dists[i] = float.MaxValue;
                    }
                }

                for (i = 0; i < terrain.Indices.Length; )
                {
                    int ia = terrain.Indices[i++],
                        ib = terrain.Indices[i++],
                        ic = terrain.Indices[i++];
                    Vector3 a = screenLocations[ia],
                            b = screenLocations[ib],
                            c = screenLocations[ic];

                    if (OutOfBounds(a) || OutOfBounds(b) || OutOfBounds(c))
                    {
                        continue;
                    }

                    SortTriangleByY(ref a, ref b, ref c);

                    int closestVertex = ia;
                    float dist = dists[ia];
                    if (dists[ib] < dist)
                    {
                        dist = dists[ib];
                        closestVertex = ib;
                    }
                    if (dists[ic] < dist)
                    {
                        dist = dists[ic];
                        closestVertex = ic;
                    }

                    Vector3 ac = c - a, ab = b - a, bc = b - c;
                    int j = 0;
                    do
                    {
                        float y = j + a.Y;
                        int screenX1 = (int)Math.Round(((ac.X * j) / ac.Y) + a.X);
                        int screenX2 = (int)((y < b.Y) ? Math.Round(((ab.X * j) / ab.Y) + a.X) : ((bc.X * (y - b.Y)) / bc.Y) + b.X);
                        int screenY = (int)Math.Round(y);

                        if (screenX2 < screenX1)
                        {
                            int temp = screenX1;
                            screenX1 = screenX2;
                            screenX2 = temp;
                        }

                        int screenX = screenX1;
                        do
                        {
                            if (indices[screenX, screenY] == -1)
                                indices[screenX, screenY] = closestVertex;
                            else
                            {
                                float dist2 = dists[indices[screenX, screenY]];
                                if (dist < dist2)
                                {
                                    indices[screenX, screenY] = closestVertex;
                                }
                            }
                            screenX++;
                        } while (screenX <= screenX2);

                        j++;
                    } while (j <= ac.Y);
                }
                text = i.ToString();

                ready = true;

                Notify();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public static void SortTriangleByY(ref Vector3 p, ref Vector3 q, ref Vector3 r)
        {
            Vector3 temp;

            if (q.Y < p.Y)
            {
                temp = p;
                if (r.Y < q.Y) { p = r; r = temp; }
                else
                {
                    if (r.Y < temp.Y) { p = q; q = r; r = temp; }
                    else { p = q; q = temp; }
                }
            }
            else
            {
                if (r.Y < q.Y)
                {
                    temp = r; r = q;
                    if (temp.Y < p.Y) { q = p; p = temp; }
                    else { q = temp; }
                }
            }
        }
    }
}
