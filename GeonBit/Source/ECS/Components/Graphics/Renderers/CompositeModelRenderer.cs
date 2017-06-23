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
// A component that renders a composite 3D model.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeonBit.ECS.Components.Graphics
{
    /// <summary>
    /// This component renders a 3D model but also provide a fine control over its internal meshes.
    /// Its slightly slower than SimpleModelRenderer and ModelRenderer, but provide more control and better distance-from-camera sorting for transparency.
    /// </summary>
    public class CompositeModelRenderer : BaseRendererComponent
    {
        /// <summary>
        /// The entity from the core layer used to draw the model.
        /// </summary>
        protected Core.Graphics.CompositeModelEntity _entity;

        /// <summary>
        /// Get the main entity instance of this renderer.
        /// </summary>
        protected override Core.Graphics.BaseRenderableEntity Entity { get { return _entity; } }

        /// <summary>
        /// Return meshes count.
        /// </summary>
        public int MeshesCount
        {
            get { return _entity.MeshesCount; }
        }

        /// <summary>
        /// Get mesh entity by index.
        /// </summary>
        /// <param name="index">Mesh index to get.</param>
        /// <returns>MeshEntity instance for this mesh.</returns>
        public Core.Graphics.MeshEntity GetMesh(int index)
        {
            return _entity.GetMesh(index);
        }

        /// <summary>
        /// Get mesh entity by name.
        /// </summary>
        /// <param name="name">Mesh name to get.</param>
        /// <returns>MeshEntity instance for this mesh.</returns>
        public Core.Graphics.MeshEntity GetMesh(string name)
        {
            return _entity.GetMesh(name);
        }


        /// <summary>
        /// Protected constructor without params to use without creating entity, for inheriting classes.
        /// </summary>
        protected CompositeModelRenderer()
        {
        }

        /// <summary>
        /// Create the model renderer component.
        /// </summary>
        /// <param name="model">Model to draw.</param>
        public CompositeModelRenderer(Model model)
        {
            _entity = new Core.Graphics.CompositeModelEntity(model);
        }

        /// <summary>
        /// Create the model renderer component.
        /// </summary>
        /// <param name="model">Path of the model asset to draw.</param>
        public CompositeModelRenderer(string model) : this(Resources.GetModel(model))
        {
        }

        /// <summary>
        /// Copy basic properties to another component (helper function to help with Cloning).
        /// </summary>
        /// <param name="copyTo">Other component to copy values to.</param>
        /// <returns>The object we are copying properties to.</returns>
        protected override BaseComponent CopyBasics(BaseComponent copyTo)
        {
            ModelRenderer other = copyTo as ModelRenderer;
            return base.CopyBasics(other);
        }

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            CompositeModelRenderer ret = new CompositeModelRenderer(_entity.Model);
            CopyBasics(ret);
            for (int i = 0; i < _entity.MeshesCount; ++i)
            {
                Core.Graphics.MeshEntity other = ret.GetMesh(i);
                Core.Graphics.MeshEntity self = GetMesh(i);
                other.MaterialOverride = self.MaterialOverride.Clone();
                other.BlendingState = self.BlendingState;
                other.SetMaterials(self.OverrideMaterials);
            }
            return ret;
        }
    }
}
