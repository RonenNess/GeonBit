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
// A basic renderable bounding box.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.Core.Graphics
{

    /// <summary>
    /// Draw a bounding box.
    /// Note: for debug purposes only, don't use in actual game.
    /// </summary>
    public class BoundingBoxEntity : BaseRenderableEntity
    {
        // bounding box to draw.
        private BoundingBox _boundingBox;

        /// <summary>
        /// Get / Set the bounding box to draw.
        /// </summary>
        public BoundingBox Box {

            // get bounding box
            get
            {
                return _boundingBox;
            }

            // set bounding box
            set
            {
                // only if changed, update bounding box
                if (_boundingBox == null || !_boundingBox.Equals(value))
                {
                    _boundingBox = value;
                    OnBoundingBoxUpdate();
                }
            }
        }

        // drawing effect
        BasicEffect _boxEffect;

        /// <summary>
        /// Get effect we draw box with.
        /// </summary>
        public BasicEffect BoxEffect { get { return _boxEffect; } }

        /// <summary>
        /// If true, it means bounding box is already transformed and we don't need to apply world matrix on it.
        /// </summary>
        public bool IsBoxAlreadyTransformed = true;

        // Initialize an array of indices for the box. 12 lines require 24 indices
        static private short[] _bBoxIndices = {
                0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
            };

        // vertex list (1 per box corner, total of 8 corners)
        VertexPositionColor[] _primitiveList = new VertexPositionColor[8];

        /// <summary>
        /// Create the bounding box entity.
        /// </summary>
        public BoundingBoxEntity()
        {
            // create effect
            _boxEffect = new BasicEffect(GraphicsManager.GraphicsDevice);
            _boxEffect.TextureEnabled = false;
        }

        /// <summary>
        /// If true, this entity will only show in debug / editor mode.
        /// </summary>
        public override bool IsDebugEntity
        {
            get { return true; }
        }

        /// <summary>
        /// Called when bounding box changes.
        /// </summary>
        public void OnBoundingBoxUpdate()
        {
            // get bounding box corners
            Vector3[] corners = Box.GetCorners();

            // Assign the 8 box vertices
            for (int i = 0; i < corners.Length; i++)
            {
                _primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
            }
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(ref Matrix worldTransformations)
        {
            // not visible / no active camera? skip
            if (!Visible || GraphicsManager.ActiveCamera == null)
            {
                return;
            }

            // set world / view / projection matrix
            _boxEffect.World = IsBoxAlreadyTransformed ? Matrix.Identity : worldTransformations;
            _boxEffect.View = GraphicsManager.ActiveCamera.View;
            _boxEffect.Projection = GraphicsManager.ActiveCamera.Projection;

            // Draw the box with a LineList
            foreach (EffectPass pass in _boxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsManager.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList, _primitiveList, 0, 8,
                    _bBoxIndices, 0, 12);
            }
        }
    }
}