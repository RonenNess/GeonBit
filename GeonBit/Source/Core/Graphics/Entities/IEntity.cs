#region LICENSE
/**
 * For the purpose of making video games only, GeonBit is distributed under the MIT license.
 * to use this source code or GeonBit as a whole for any other purpose, please seek written 
 * permission from the library author.
 * 
 * Copyright (c) 2017 Ronen Ness [ronenness@gmail.com].
 * You may not remove this license notice.
 */
#endregion
#region File Description
//-----------------------------------------------------------------------------
// An entity is something you can draw, eg actual objects and not nodes.
// Entities don't have transformations of their own; instead, you put them inside
// a node which handle matrices and transformations for them.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;


namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// A basic renderable entity.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Draw this entity.
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        void Draw(Node parent, Matrix localTransformations, Matrix worldTransformations);

        /// <summary>
        /// Get the bounding box of this entity (in world space).
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding box of the entity, in world space.</returns>
        BoundingBox GetBoundingBox(Node parent, Matrix localTransformations, Matrix worldTransformations);

        /// <summary>
        /// Get the bounding sphere of this entity (in world space).
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding sphere of the entity, in world space.</returns>
        BoundingSphere GetBoundingSphere(Node parent, Matrix localTransformations, Matrix worldTransformations);

        /// <summary>
        /// Return if the entity is currently visible.
        /// </summary>
        /// <returns>If the entity is visible or not.</returns>
        bool Visible { get; set; }
    }
}
