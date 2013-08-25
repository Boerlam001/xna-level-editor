using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EditorModel;

namespace View
{
    public class EditorMode_Terrain : EditorMode
    {
        protected Color hoveredVertexOriginalColor;

        public EditorMode_Terrain(Editor editor) : base(editor)
        {
        }

        public EditorMode_Terrain() : base()
        {
        }

        private bool OutOfBounds(Vector2 pos)
        {
            return !(pos.X >= 0 && pos.X <= editor.GraphicsDevice.Viewport.Bounds.Width && pos.Y >= 0 && pos.Y <= editor.GraphicsDevice.Viewport.Bounds.Height);
        }

        public override void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseMove(sender, e);
            if (editor.Camera.IsMoving)
                return;
            Vector2 pos = new Vector2(e.X, e.Y);
            if (OutOfBounds(pos))
            {
                return;
            }
            //List<int> vertexIndices = new List<int>();
            //
            //try
            //{
            //    Terrain terrain = editor.Terrain;
            //    TerrainIndexer terrainIndexer = editor.Terrain.TerrainIndexer;
            //    HashSet<Vector2> markedIndices = new HashSet<Vector2>();
            //    vertexIndices = DfsClosestIndex(pos, pos, 1, ref markedIndices);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //
            //Ray ray = Helper.Pick(editor.GraphicsDevice, editor.Camera, e.X, e.Y);
            //float minDist = float.MaxValue;
            //int closestVertex = -1;
            //
            //foreach (int i in vertexIndices)
            //{
            //    float dist = (editor.Terrain.Vertices[i].Position - ray.Position).Length();
            //    if (dist < minDist)
            //    {
            //        minDist = dist;
            //        closestVertex = i;
            //    }
            //}
            //try
            //{
                //int ia = editor.Terrain.Indices[editor.Terrain.TerrainIndexer.Indices[e.X, e.Y] * 3],
                //    ib = editor.Terrain.Indices[editor.Terrain.TerrainIndexer.Indices[e.X, e.Y] * 3 + 1],
                //    ic = editor.Terrain.Indices[editor.Terrain.TerrainIndexer.Indices[e.X, e.Y] * 3 + 2];
                //Vector3 a3 = editor.Terrain.TerrainIndexer.ScreenLocations[ia],
                //        b3 = editor.Terrain.TerrainIndexer.ScreenLocations[ib],
                //        c3 = editor.Terrain.TerrainIndexer.ScreenLocations[ic];
                //Vector2 a = new Vector2(a3.X, a3.Y),
                //        b = new Vector2(b3.X, b3.Y),
                //        c = new Vector2(c3.X, c3.Y);
                //float dist = (pos - a).Length(), dist2;
                //int closestVertex = ia;
                //dist2 = (pos - b).Length();
                //if (dist2 < dist)
                //{
                //    closestVertex = ib;
                //}
                //dist2 = (pos - c).Length();
                //if (dist2 < dist)
                //{
                //    closestVertex = ic;
                //}
                editor.TerrainBrush.OnMouseMove(editor.Terrain.TerrainIndexer.Indices[e.X, e.Y]);
                editor.Camera.Notify();
            //}
            //catch
            //{
            //}
        }

        private List<int> DfsClosestIndex(Vector2 start, Vector2 root, float threshold, ref HashSet<Vector2> markedIndices)
        {
            try
            {
                List<int> result = new List<int>();

                float length = (root - start).Length();
                if (threshold == 0 || length > threshold)
                    return result;

                List<Vector2> neighbours = GetAllNeighbours2(root);
                
                markedIndices.Add(root);
                foreach (Vector2 neighbour in neighbours)
                {
                    if (OutOfBounds(neighbour) || markedIndices.Contains(neighbour))
                        continue;
                    if (editor.Terrain.TerrainIndexer.Indices[(int)root.X, (int)root.Y] != -1)
                        result.Add(editor.Terrain.TerrainIndexer.Indices[(int)root.X, (int)root.Y]);
                    result.AddRange(DfsClosestIndex(start, neighbour, threshold, ref markedIndices));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected int BfsClosestIndex(Vector2 start)
        {
            HashSet<Vector2> markedIndices = new HashSet<Vector2>();
            Queue<Vector2> queue = new Queue<Vector2>();
            markedIndices.Add(start);
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                Vector2 t = queue.Dequeue();
                if (editor.Terrain.TerrainIndexer.Indices[(int)t.X, (int)t.Y] != -1)
                    return editor.Terrain.TerrainIndexer.Indices[(int)t.X, (int)t.Y];
                List<Vector2> neighbours = GetAllNeighbours(t);
                foreach (Vector2 neighbour in neighbours)
                {
                    if (!markedIndices.Contains(neighbour))
                    {
                        markedIndices.Add(neighbour);
                        queue.Enqueue(neighbour);
                    }
                }
            }
            return -1;
        }

        protected List<Vector2> GetAllNeighbours2(Vector2 pivot)
        {
            List<Vector2> neighbours = new List<Vector2>();
            Vector2 top = pivot + new Vector2(0, -1),
                    left = pivot + new Vector2(-1, 0),
                    right = pivot + new Vector2(1, 0),
                    bottom = pivot + new Vector2(0, 1);
            if (!OutOfBounds(top)) { neighbours.Add(top); }
            if (!OutOfBounds(left)) { neighbours.Add(left); }
            if (!OutOfBounds(right)) { neighbours.Add(right); }
            if (!OutOfBounds(bottom)) { neighbours.Add(bottom); }
            return neighbours;
        }

        protected List<Vector2> GetAllNeighbours(Vector2 pivot)
        {
            List<Vector2> neighbours = new List<Vector2>();
            Vector2 topLeft     = pivot + new Vector2(-1, -1),
                    top         = pivot + new Vector2(0, -1),
                    topRight    = pivot + new Vector2(1, -1),
                    left        = pivot + new Vector2(-1, 0),
                    right       = pivot + new Vector2(1, 0),
                    bottomLeft  = pivot + new Vector2(-1, 1),
                    bottom      = pivot + new Vector2(0, 1),
                    bottomRight = pivot + new Vector2(1, 1);
            if (!OutOfBounds(topLeft))     { neighbours.Add(topLeft    ); }
            if (!OutOfBounds(top))         { neighbours.Add(top        ); }
            if (!OutOfBounds(topRight))    { neighbours.Add(topRight   ); }
            if (!OutOfBounds(left))        { neighbours.Add(left       ); }
            if (!OutOfBounds(right))       { neighbours.Add(right      ); }
            if (!OutOfBounds(bottomLeft))  { neighbours.Add(bottomLeft ); }
            if (!OutOfBounds(bottom))      { neighbours.Add(bottom     ); }
            if (!OutOfBounds(bottomRight)) { neighbours.Add(bottomRight); }
            return neighbours;
        }

        protected bool ListContainsPair(List<KeyValuePair<int, int>> list, KeyValuePair<int, int> value)
        {
            foreach (KeyValuePair<int, int> el in list)
            {
                if (KeyValueEqual<int, int>(el, value))
                    return true;
            }
            return false;
        }

        protected bool KeyValueEqual<TKey, TValue>(KeyValuePair<TKey, TValue> fst,
                                          KeyValuePair<TKey, TValue> snd)
            where TValue : IComparable
            where TKey : IComparable
        {
            return (fst.Value.CompareTo(snd.Value) == 0)
                     && (snd.Key.CompareTo(fst.Key) == 0);
        }

        public override void PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
