#region LICENSE
//-----------------------------------------------------------------------------
// For the purpose of making video games, educational projects or gamification,
// GeonBit is distributed under the MIT license and is totally free to use.
// To use this source code or GeonBit as a whole for other purposes, please seek 
// permission from the library author, Ronen Ness.
// 
// Copyright (c) 2017 Ronen Ness [ronenness@gmail.com].
// Do not remove this license notice.
//-----------------------------------------------------------------------------
#endregion
#region File Description
//-----------------------------------------------------------------------------
// A component that help build a 2d tilemap.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GeonBit.ECS.Components.Misc
{
    /// <summary>
    /// A callback to call per tile when you want to process all tiles in a tilemap.
    /// </summary>
    /// <param name="tile">Current tile.</param>
    /// <param name="index">Tile index.</param>
    public delegate void ProcessTileCallback(GameObject tile, Point index);

    /// <summary>
    /// This component help implement an optimized 2d tiles map.
    /// </summary>
    public class TileMap : BaseComponent
    {
        // root tile
        GameObject _root;

        /// <summary>
        /// How many tiles are in a single batch (for culling optimizations).
        /// </summary>
        public ushort BatchSize { set; private get; }

        /// <summary>
        /// The size of a single tile. This affect the distance between tiles (eg the position of the game objects).
        /// </summary>
        public Vector2 TileSize;

        // unique identifier for the internal data we attach to gameobjects.
        private static readonly string InternalDataKey = "tilemap_index";

        /// <summary>
        /// A batch of tiles.
        /// </summary>
        private class TilesBatch
        {
            // the tiles themselves
            public GameObject[,] Tiles;

            // batch root GameObject
            public GameObject BatchRoot;

            // batch index
            public Point Index;

            /// <summary>
            /// Create a new tiles batch.
            /// </summary>
            /// <param name="tilemap">Parent tilemap.</param>
            /// <param name="index">Batch index.</param>
            public TilesBatch(TileMap tilemap, Point index)
            {
                // create batch root game object, and add to tilesmap root
                BatchRoot = new GameObject("tiles-batch-" + index.ToString(), SceneNodeType.BoundingBoxCulling);
                BatchRoot.UpdateWhenNotVisible = false;
                BatchRoot.Parent = tilemap._root;
                BatchRoot.AddComponentDebug(new Graphics.BoundingBoxRenderer());
                Index = index;

                // create tiles matrix
                Tiles = new GameObject[tilemap.BatchSize, tilemap.BatchSize];
            }

            /// <summary>
            /// Destroy the tiles batch.
            /// </summary>
            public void Destroy()
            {
                BatchRoot.Parent = null;
                Tiles = null;
            }
        }

        // matrix of tile batches
        Dictionary<int, Dictionary<int, TilesBatch>> _batches;

        /// <summary>
        /// Create the tilemap component.
        /// </summary>
        /// <param name="tileSize">The size of a single tile.</param>
        /// <param name="batchSize">How many tiles there are on X and Y axis of a single tiles batch (used for culling optimizations).</param>
        public TileMap(Vector2 tileSize, ushort batchSize = 10)
        {
            // create root
            _root = new GameObject("tilemap", SceneNodeType.BoundingBoxCulling);

            // store tile size and batch size
            TileSize = tileSize;
            BatchSize = batchSize;

            // create batches
            _batches = new Dictionary<int, Dictionary<int, TilesBatch>>();
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            // create new tilemap and set basic properties
            TileMap ret = new TileMap(TileSize, BatchSize);
            CopyBasics(ret);

            // clone all tiles
            string key = InternalDataKey;
            foreach (KeyValuePair<int, Dictionary<int, TilesBatch>> row in _batches)
            {
                foreach (KeyValuePair<int, TilesBatch> batch in row.Value)
                {
                    foreach (GameObject tile in batch.Value.Tiles)
                    {
                        if (tile != null)
                        {
                            SetTile((Point)tile.GetInternalData(ref key), tile);
                        }
                    }
                }
            }

            // return new tilemap
            return ret;
        }

        /// <summary>
        /// Execute a callback on all tiles in tilemap.
        /// </summary>
        /// <param name="callback">Function to execute (called with tile GameObject and index).</param>
        public void ProcessTiles(ProcessTileCallback callback)
        {
            string intDataKey = InternalDataKey;
            foreach (KeyValuePair<int, Dictionary<int, TilesBatch>> row in _batches)
            {
                foreach (KeyValuePair<int, TilesBatch> batch in row.Value)
                {
                    foreach (GameObject tile in batch.Value.Tiles)
                    {
                        if (tile != null)
                        {
                            callback(tile, (Point)tile.GetInternalData(ref intDataKey));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Execute a callback on all tile batches in tilemap.
        /// </summary>
        /// <param name="callback">Function to execute (called with batch root GameObject and index).</param>
        public void ProcessTileBatches(ProcessTileCallback callback)
        {
            foreach (KeyValuePair<int, Dictionary<int, TilesBatch>> row in _batches)
            {
                foreach (KeyValuePair<int, TilesBatch> batch in row.Value)
                {
                    callback(batch.Value.BatchRoot, batch.Value.Index);
                }
            }
        }

        /// <summary>
        /// Create and return a new batch instance.
        /// </summary>
        /// <param name="batchIndex">Batch index.</param>
        /// <returns>New tiles batch.</returns>
        private TilesBatch CreateBatch(Point batchIndex)
        {
            // create and return the new batch
            return new TilesBatch(this, batchIndex);
        }

        /// <summary>
        /// Get tiles batch by index. Creates a new batch if needed.
        /// </summary>
        /// <param name="index">Batch index to get.</param>
        /// <param name="createIfNeeded">If true and batch doesn't exist yet, create and return it. If false, may return null.</param>
        /// <returns>Tiles batch.</returns>
        private TilesBatch GetBatch(Point index, bool createIfNeeded = true)
        {
            // try to get batch
            try
            {
                return _batches[index.X][index.Y];
            }
            // not found?
            catch (KeyNotFoundException)
            {
                // create new batch
                if (createIfNeeded)
                {
                    // create row if needed
                    if (!_batches.ContainsKey(index.X))
                    {
                        _batches[index.X] = new Dictionary<int, TilesBatch>();
                    }

                    // create new batch
                    TilesBatch newBatch = CreateBatch(index);
                    _batches[index.X][index.Y] = newBatch;

                    // return newly created batch
                    return newBatch;
                }
            }

            // if got here it means batch wasn't found and create-new was set to false. return null.
            return null;
        }

        /// <summary>
        /// Set the basic properties of a game object before pushing it into the tilemap.
        /// </summary>
        /// <param name="batch">Batch we are adding the tile to.</param>
        /// <param name="obj">GameObject to set.</param>
        /// <param name="index">Tile index.</param>
        private void SetTileProperties(TilesBatch batch, GameObject obj, Point index)
        {
            // set name and position
            obj.Name = "tile-" + index.ToString();
            obj.SceneNode.Position = new Vector3(TileSize.X * index.X, 0, TileSize.Y * index.Y);

            // put index as attached data
            string intDataKey = InternalDataKey;
            obj.SetInternalData(ref intDataKey, index);

            // set to not update when not visible
            obj.UpdateWhenNotVisible = false;

            // add to root tile
            obj.Parent = batch.BatchRoot;
        }

        // current batch index and relative index, used internally.
        // note: we store these as members to reduce garbage when creating huge tilemaps.
        Point batchIndex = new Point();
        Point relativeIndex = new Point();

        /// <summary>
        /// Calculate batch index and relative index for a given index, and store them in batchIndex and relativeIndex.
        /// Note: the reason we don't create and return new points and use private members to store this data instead is to reduce
        /// garbage when creating huge tilemaps.
        /// </summary>
        /// <param name="tileIndex">Tile index to get batch index for.</param>
        void CalcBatchIndexAndRelativeIndex(Point tileIndex)
        {
            // calc batch index
            batchIndex.X = tileIndex.X / BatchSize;
            batchIndex.Y = tileIndex.Y / BatchSize;

            // calc relative index
            relativeIndex.X = tileIndex.X % BatchSize;
            relativeIndex.Y = tileIndex.Y % BatchSize;
        }

        /// <summary>
        /// Get tile at a given index (create new tile GameObject if doesn't exist).
        /// </summary>
        /// <param name="index">Tile index to get.</param>
        /// <param name="createIfNeeded">If true and tile doesn't exist yet, create and return it. If false, may return null.</param>
        /// <returns>Tile GameObject.</returns>
        public GameObject GetTile(Point index, bool createIfNeeded = true)
        {
            // make sure index is valid
            if (index.X < 0 || index.Y < 0)
            {
                throw new Exceptions.OutOfRangeException("Tile index must be positive!");
            }

            // calc batch index and relative index
            CalcBatchIndexAndRelativeIndex(index);

            // get batch
            TilesBatch batch = GetBatch(batchIndex, createIfNeeded);

            // no batch? return null
            if (batch == null)
            {
                return null;
            }

            // try to get tile
            GameObject ret = batch.Tiles[relativeIndex.X, relativeIndex.Y];

            // check if need to create new tile
            if (ret == null && createIfNeeded)
            {
                // create the new game object and set its basic properties
                ret = new GameObject(string.Empty, SceneNodeType.BoundingBoxCulling);
                SetTileProperties(batch, ret, index);

                // add to batch
                batch.Tiles[relativeIndex.X, relativeIndex.Y] = ret;
            }

            // return tile (or null, if not found and not creating new)
            return ret;
        }

        /// <summary>
        /// Destroy a specific tile.
        /// </summary>
        /// <param name="index">Tile index to destroy.</param>
        public void DestroyTile(Point index)
        {
            // calc batch index and relative index
            CalcBatchIndexAndRelativeIndex(index);

            // get batch
            TilesBatch batch = GetBatch(batchIndex, false);

            // no batch? return
            if (batch == null)
            {
                return;
            }

            // try to get tile
            GameObject toRemove = batch.Tiles[relativeIndex.X, relativeIndex.Y];

            // if tile exists, destroy it
            if (toRemove != null)
            {
                toRemove.Parent = null;
                batch.Tiles[relativeIndex.X, relativeIndex.Y] = null;
            }
        }

        /// <summary>
        /// Called when GameObject is destroyed.
        /// </summary>
        protected override void OnDestroyed()
        {
            foreach (var row in _batches)
            {
                foreach (var entry in row.Value)
                {
                    entry.Value.Destroy();
                }
                row.Value.Clear();
            }
            _batches.Clear();
            _root = null;
        }

        /// <summary>
        /// Set the tile GameObject at a given index (will replace previous tile if exists).
        /// </summary>
        /// <param name="index">Tile index to set.</param>
        /// <param name="tile">GameObject to set as tile (note: will be cloned).</param>
        public void SetTile(Point index, GameObject tile)
        {
            // calc batch index and relative index
            CalcBatchIndexAndRelativeIndex(index);

            // get batch
            TilesBatch batch = GetBatch(batchIndex, true);

            // set tile
            tile = tile.Clone();
            SetTileProperties(batch, tile, index);
            batch.Tiles[relativeIndex.X, relativeIndex.Y] = tile;
        }

        /// <summary>
        /// Called when GameObject turned disabled.
        /// </summary>
        protected override void OnDisabled()
        {
            _root.Visible = false;
        }

        /// <summary>
        /// Called when GameObject is enabled.
        /// </summary>
        protected override void OnEnabled()
        {
            _root.Visible = true;
        }

        /// <summary>
        /// Change component parent GameObject.
        /// </summary>
        /// <param name="prevParent">Previous parent.</param>
        /// <param name="newParent">New parent.</param>
        override protected void OnParentChange(GameObject prevParent, GameObject newParent)
        {
            _root.Parent = newParent;
        }
    }
}
