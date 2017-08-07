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
// Collision shape for a height map.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Physics.CollisionShapes
{
    /// <summary>
    /// Height map collision shape (used for terrains).
    /// </summary>
    public class CollisionHeightMap : ICollisionShape
    {
        // handle to the heightmap raw data
        System.Runtime.InteropServices.GCHandle _rawDataHandle;

        /// <summary>
        /// Destroy the collision shape.
        /// </summary>
        ~CollisionHeightMap()
        {
            // free the int ptr
            if (_rawDataHandle != null) { _rawDataHandle.Free(); }
        }

        /// <summary>
        /// Create the collision height map.
        /// </summary>
        /// <param name="heightData">Height map data (should be defined as byte[size.X * size.Y * 4]).</param>
        /// <param name="size">Tilemap size (Y is actually Z axis).</param>
        /// <param name="scale">Scale tiles width and depth.</param>
        /// <param name="minHeight">Min height value.</param>
        /// <param name="maxHeight">Max height value.</param>
        /// <param name="heightScale">Optional height scale.</param>
        /// <param name="useDiamondSubdivision">Divide the tiles into diamond shapes for more accurare results.</param>
        public CollisionHeightMap(byte[] heightData, Point size, Vector2 scale, float minHeight = 0f, float maxHeight = 100f, float heightScale = 1f, bool useDiamondSubdivision = false)
        {
            // build shape
            Build(heightData, size, scale, minHeight, maxHeight, heightScale, 1, useDiamondSubdivision);
        }

        /// <summary>
        /// Create the collision height map.
        /// </summary>
        /// <param name="heightData">Height map data (use vector Y component).</param>
        /// <param name="scale">Scale tiles width and depth.</param>
        /// <param name="minHeight">Min height value.</param>
        /// <param name="maxHeight">Max height value.</param>
        /// <param name="heightScale">Optional height scale.</param>
        /// <param name="useDiamondSubdivision">Divide the tiles into diamond shapes for more accurare results.</param>
        public CollisionHeightMap(Vector3[,] heightData, Vector2 scale, float minHeight = 0f, float maxHeight = 100f, float heightScale = 1f, bool useDiamondSubdivision = false)
        {
            // get size and convert to bytes array
            Point size = new Point(heightData.GetLength(0), heightData.GetLength(1));
            float[] array = new float[size.X * size.Y * 4];
            for (int i = 0; i < size.X; ++i)
            {
                for (int j = 0; j < size.Y; ++j)
                {
                    array[i + j * size.X] = heightData[i, j].Y;
                }
            }

            // build shape
            Build(array, size, scale, minHeight, maxHeight, heightScale, 1, useDiamondSubdivision);
        }

        /// <summary>
        /// Create the collision height map.
        /// </summary>
        /// <param name="heightData">Height map data.</param>
        /// <param name="scale">Scale tiles width and depth.</param>
        /// <param name="minHeight">Min height value.</param>
        /// <param name="maxHeight">Max height value.</param>
        /// <param name="heightScale">Optional height scale.</param>
        /// <param name="useDiamondSubdivision">Divide the tiles into diamond shapes for more accurare results.</param>
        public CollisionHeightMap(byte[,] heightData, Vector2 scale, float minHeight = 0f, float maxHeight = 100f, float heightScale = 1f, bool useDiamondSubdivision = false)
        {
            // get size and convert to bytes array
            Point size = new Point(heightData.GetLength(0), heightData.GetLength(1));
            var array = new byte[size.X * size.Y * 4];
            for (int i = 0; i < size.X; ++i)
            {
                for (int j = 0; j < size.Y; ++j)
                {
                    array[i + j * size.X] = (byte)heightData[i, j];
                }
            }

            // build shape
            Build(array, size, scale, minHeight, maxHeight, heightScale, 1, useDiamondSubdivision);
        }

        /// <summary>
        /// Create the collision height map.
        /// </summary>
        /// <param name="heightData">Height map data (should be defined as byte[size.X * size.Y * 4]).</param>
        /// <param name="size">Tilemap size (Y is actually Z axis).</param>
        /// <param name="scale">Scale tiles width and depth.</param>
        /// <param name="minHeight">Min height value.</param>
        /// <param name="maxHeight">Max height value.</param>
        /// <param name="heightScale">Optional height scale.</param>
        /// <param name="upIndex">Up index.</param>
        /// <param name="useDiamondSubdivision">Divide the tiles into diamond shapes for more accurare results.</param>
        private void Build(byte[] heightData, Point size, Vector2 scale, float minHeight = 0f, float maxHeight = 100f, float heightScale = 1f, int upIndex = 1, bool useDiamondSubdivision = false)
        {
            // get int ptr for data bytes
            _rawDataHandle = System.Runtime.InteropServices.GCHandle.Alloc(heightData, System.Runtime.InteropServices.GCHandleType.Pinned);
            var address = _rawDataHandle.AddrOfPinnedObject();

            // create heightmap
            var heightmap = new BulletSharp.HeightfieldTerrainShape(size.X, size.Y, address,
                heightScale, minHeight, maxHeight, upIndex,
                BulletSharp.PhyScalarType.Byte, false);

            // set transform and diamond subdivision
            heightmap.SetUseDiamondSubdivision(useDiamondSubdivision);
            heightmap.LocalScaling = ToBullet.Vector(new Vector3(scale.X, 1, scale.Y));

            // set shape
            _shape = heightmap;
        }

        /// <summary>
        /// Create the collision height map.
        /// </summary>
        /// <param name="heightData">Height map data (should be defined as byte[size.X * size.Y * 4]).</param>
        /// <param name="size">Tilemap size (Y is actually Z axis).</param>
        /// <param name="scale">Scale tiles width and depth.</param>
        /// <param name="minHeight">Min height value.</param>
        /// <param name="maxHeight">Max height value.</param>
        /// <param name="heightScale">Optional height scale.</param>
        /// <param name="useDiamondSubdivision">Divide the tiles into diamond shapes for more accurare results.</param>
        public CollisionHeightMap(float[] heightData, Point size, Vector2 scale, float minHeight = 0f, float maxHeight = 100f, float heightScale = 1f, bool useDiamondSubdivision = false)
        {
            // build shape
            Build(heightData, size, scale, minHeight, maxHeight, heightScale, 1, useDiamondSubdivision);
        }

        /// <summary>
        /// Create the collision height map.
        /// </summary>
        /// <param name="heightData">Height map data.</param>
        /// <param name="scale">Scale tiles width and depth.</param>
        /// <param name="minHeight">Min height value.</param>
        /// <param name="maxHeight">Max height value.</param>
        /// <param name="heightScale">Optional height scale.</param>
        /// <param name="useDiamondSubdivision">Divide the tiles into diamond shapes for more accurare results.</param>
        public CollisionHeightMap(float[,] heightData, Vector2 scale, float minHeight = 0f, float maxHeight = 100f, float heightScale = 1f, bool useDiamondSubdivision = false)
        {
            // get size and convert to bytes array
            Point size = new Point(heightData.GetLength(0), heightData.GetLength(1));
            var array = new byte[size.X * size.Y * 4];
            for (int i = 0; i < size.X; ++i)
            {
                for (int j = 0; j < size.Y; ++j)
                {
                    array[i + j * size.X] = (byte)heightData[i, j];
                }
            }

            // build shape
            Build(array, size, scale, minHeight, maxHeight, heightScale, 1, useDiamondSubdivision);
        }

        /// <summary>
        /// Create the collision height map.
        /// </summary>
        /// <param name="heightData">Height map data (should be defined as byte[size.X * size.Y * 4]).</param>
        /// <param name="size">Tilemap size (Y is actually Z axis).</param>
        /// <param name="scale">Scale tiles width and depth.</param>
        /// <param name="minHeight">Min height value.</param>
        /// <param name="maxHeight">Max height value.</param>
        /// <param name="heightScale">Optional height scale.</param>
        /// <param name="upIndex">Up index.</param>
        /// <param name="useDiamondSubdivision">Divide the tiles into diamond shapes for more accurare results.</param>
        private void Build(float[] heightData, Point size, Vector2 scale, float minHeight = 0f, float maxHeight = 100f, float heightScale = 1f, int upIndex = 1, bool useDiamondSubdivision = false)
        {
            // get int ptr for data bytes
            _rawDataHandle = System.Runtime.InteropServices.GCHandle.Alloc(heightData, System.Runtime.InteropServices.GCHandleType.Pinned);
            var address = _rawDataHandle.AddrOfPinnedObject();

            // create heightmap
            var heightmap = new BulletSharp.HeightfieldTerrainShape(size.X, size.Y, address,
                heightScale, minHeight, maxHeight, upIndex,
                BulletSharp.PhyScalarType.Single, false);

            // set transform and diamond subdivision
            heightmap.SetUseDiamondSubdivision(useDiamondSubdivision);
            heightmap.LocalScaling = ToBullet.Vector(new Vector3(scale.X, 1, scale.Y));

            // set shape
            _shape = heightmap;
        }

        /// <summary>
        /// Clone the physical shape.
        /// </summary>
        /// <returns>Cloned shape.</returns>
        protected override ICollisionShape CloneImp()
        {
            throw new Exceptions.InvalidActionException("Cannot clone height-map physical shape!");
        }
    }
}
