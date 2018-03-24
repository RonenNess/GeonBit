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
// A 3d sprite entity (eg 2d texture on a 3d quad that always faces camera).
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GeonBit.Core.Graphics
{
    /// <summary>
    /// A sprite entity (3d quad that always faces camera and plays animation from spritesheet texture file).
    /// </summary>
    public class SpriteEntity : BaseRenderableEntity
    {
        /// <summary>
        /// Get the spritesheet this sprite entity currently uses.
        /// </summary>
        public SpriteSheet Spritesheet { get; private set; }

        // current spritesheet step
        SpriteSheetStep _spritesheetStep;

        // currently playing animation (or null, if not playing any clips at the momeny)
        SpriteAnimationClipPlay _currAnimation = null;

        /// <summary>
        /// Callback to call whenever sprite animation cycle ends (when using animation clips).
        /// </summary>
        public OnAnimationClipEnds OnAnimationEnd = null;

        /// <summary>
        /// If true, will always face camera. If false will just use node's rotation.
        /// </summary>
        public bool FaceCamera = true;

        /// <summary>
        /// Optional custom render settings for this specific sprite instance.
        /// These settings will override some of the material's properties before rendering.
        /// Note: this method is much less efficient than using different materials.
        /// </summary>
        public MaterialOverrides MaterialOverride = new MaterialOverrides();

        // dictionary of shared materials. key is texture path, value is material to use for all sprites with this material.
        static Dictionary<string, Materials.MaterialAPI> _sharedMaterials = new Dictionary<string, Materials.MaterialAPI>();

        /// <summary>
        /// Optional position offset to render sprite.
        /// </summary>
        public Vector3 PositionOffset = Vector3.Zero;

        /// <summary>
        /// Get / set sprite material.
        /// </summary>
        public Materials.MaterialAPI Material;

        /// <summary>
        /// Copy spritesheet step from other sprite.
        /// </summary>
        /// <param name="other">Other sprite to copy step from.</param>
        public void CopyStep(SpriteEntity other)
        {
            _spritesheetStep = other._spritesheetStep;
            if (other._currAnimation != null)
            {
                PlayAnimation(other._currAnimation.Clip, other._currAnimation.SpeedFactor, other._currAnimation.CurrentStep);
            }
        }

        /// <summary>
        /// Create the sprite entity.
        /// </summary>
        /// <param name="spritesheet">Spritesheet for this sprite.</param>
        /// <param name="material">Material to use for this sprite.</param>
        public SpriteEntity(SpriteSheet spritesheet, Materials.MaterialAPI material) : base()
        {
            // store spritesheet and material
            Spritesheet = spritesheet;
            Material = material;

            // set default rendering queue
            RenderingQueue = RenderingQueue.Billboards;

            // set default step
            _spritesheetStep = spritesheet.GetStep(0);
        }

        /// <summary>
        /// Create the sprite entity from texture and default material.
        /// </summary>
        /// <param name="spritesheet">Spritesheet data for this sprite.</param>
        /// <param name="texture">Texture to use for this sprite.</param>
        /// <param name="useSharedMaterial">If true, will use a shared material for all sprites with this texture. If false, will create a new material for this specific sprite instance.</param>
        public SpriteEntity(SpriteSheet spritesheet, Texture2D texture, bool useSharedMaterial = true) : 
            this(spritesheet, GetCtorMaterial(texture, useSharedMaterial))
        {
        }

        /// <summary>
        /// Get constructor material for a given texture.
        /// </summary>
        /// <param name="texture">Texture to get sprite material for.</param>
        /// <param name="useSharedMaterial">If true will use shared material, else will return a new material.</param>
        /// <returns></returns>
        static Materials.MaterialAPI GetCtorMaterial(Texture2D texture, bool useSharedMaterial)
        {
            // if using shared material:
            if (useSharedMaterial)
            {
                // material to return
                Materials.MaterialAPI material;

                // try to get material from cache
                if (_sharedMaterials.TryGetValue(texture.Name, out material))
                {
                    return material;
                }

                // if not in cache create and return
                material = GetCtorMaterial(texture, false);
                _sharedMaterials[texture.Name] = material;
                return material;
            }
            // if need to create a unique material for this instance
            else
            {
                // create material
                Materials.MaterialAPI material = new Materials.SpriteMaterial(new AlphaTestEffect(GraphicsManager.GraphicsDevice), true);

                // set material texture and return
                material.Texture = texture;
                return material;
            }
        }

        /// <summary>
        /// Optional axis to constrain rotation to.
        /// </summary>
        public Vector3? LockedAxis = Vector3.Up;

        /// <summary>
        /// Play animation clip.
        /// </summary>
        /// <param name="clip">Animation clip to play.</param>
        /// <param name="speed">Animation playing speed.</param>
        /// <param name="startingStep">Animation starting step.</param>
        public void PlayAnimation(SpriteAnimationClip clip, float speed = 1f, int? startingStep = null)
        {
            _currAnimation = new SpriteAnimationClipPlay(clip, speed, startingStep);
            _currAnimation.OnAnimationEnd = () =>
            {
                this.OnAnimationEnd?.Invoke();
            };
        }

        /// <summary>
        /// Change the spritesheet and current step of this sprite.
        /// </summary>
        /// <param name="newSpritesheet">New spritesheet data to use.</param>
        /// <param name="startingStep">Step to set from new spritesheet.</param>
        public void ChangeSpritesheet(SpriteSheet newSpritesheet, int startingStep = 0)
        {
            Spritesheet = newSpritesheet;
            _spritesheetStep = Spritesheet.GetStep(startingStep);
            _currAnimation = null;
        }

        /// <summary>
        /// Set spritesheet step from string identifier.
        /// </summary>
        /// <param name="identifier">Step identifier (must be set in spriteshet).</param>
        public void SetStep(string identifier)
        {
            _spritesheetStep = Spritesheet.GetStep(identifier);
            _currAnimation = null;
        }

        /// <summary>
        /// Set spritesheet step from index.
        /// </summary>
        /// <param name="index">Step index to set.</param>
        public void SetStep(int index)
        {
            _spritesheetStep = Spritesheet.GetStep(index);
            _currAnimation = null;
        }

        /// <summary>
        /// Update animations if currently playing.
        /// </summary>
        /// <param name="elapsedTime">Elapsed game time, in seconds, since last frame.</param>
        public void Update(float elapsedTime)
        {
            if (_currAnimation != null)
            {
                int index = _currAnimation.Update(elapsedTime);
                _spritesheetStep = Spritesheet.GetStep(index);
            }
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(ref Matrix worldTransformations)
        {
            // call base draw entity
            base.DoEntityDraw(ref worldTransformations);

            // decompose transformations
            Vector3 position; Quaternion rotation; Vector3 scale;
            worldTransformations.Decompose(out scale, out rotation, out position);

            // add position offset
            position += PositionOffset;

            // create a new world matrix for the billboard
            Matrix newWorld;

            // if facing camera, create billboard world matrix
            if (FaceCamera)
            {
                // set rotation based on camera with locked axis
                if (LockedAxis != null)
                {
                    newWorld = Matrix.CreateScale(scale) *
                               Matrix.CreateConstrainedBillboard(position, GraphicsManager.ActiveCamera.Position,
                               LockedAxis.Value, null, null);
                }
                // set rotation based on camera without any locked axis
                else
                {
                    newWorld = Matrix.CreateScale(scale) *
                               Matrix.CreateBillboard(position, GraphicsManager.ActiveCamera.Position,
                               Vector3.Up, null);
                }
            }
            // if not facing camera, just use world transformations
            else
            {
                newWorld = worldTransformations;
            }

            // update per-entity override properties
            Materials.MaterialAPI material = MaterialOverride.Apply(Material);

            // setup material
            material.Apply(ref newWorld, ref _lastBoundingSphere);

            // draw sprite
            // draw the cube vertices
            material.IterateEffectPasses((EffectPass pass) =>
            {
                GraphicsManager.GraphicsDevice.DrawUserIndexedPrimitives
                    <VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    _spritesheetStep.Vertices, 0, 4,
                    _spritesheetStep.Indexes, 0, 2);
            });
        }

        /// <summary>
        /// Calculate and return the bounding box of this entity (in world space).
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding box of the entity.</returns>
        protected override BoundingBox CalcBoundingBox(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            // decompose transformations
            Vector3 position; Quaternion rotation; Vector3 scale;
            worldTransformations.Decompose(out scale, out rotation, out position);

            // add position offset
            position += PositionOffset;

            // get bounding sphere
            return new BoundingBox(position - scale, position + scale);
        }

        /// <summary>
        /// Calculate and return the bounding sphere of this entity (in world space).
        /// </summary>
        /// <param name="parent">Parent node that's currently drawing this entity.</param>
        /// <param name="localTransformations">Local transformations from the direct parent node.</param>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        /// <returns>Bounding sphere of the entity.</returns>
        protected override BoundingSphere CalcBoundingSphere(Node parent, ref Matrix localTransformations, ref Matrix worldTransformations)
        {
            // decompose transformations
            Vector3 position; Quaternion rotation; Vector3 scale;
            worldTransformations.Decompose(out scale, out rotation, out position);

            // add position offset
            position += PositionOffset;

            // get bounding sphere
            return new BoundingSphere(position, System.Math.Max(scale.X, scale.Y));
        }
    }
}