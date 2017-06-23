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
    /// Spritesheet define animation steps of a spritesheet texture.
    /// For example, if you have a spritesheet that describe 4 steps of a walking animation, this
    /// object define the steps of the spritesheet and its texture coords.
    /// </summary>
    public class SpriteSheet
    {
        // list with steps in spritesheet
        List<SpriteSheetStep> _steps = new List<SpriteSheetStep>();

        /// <summary>
        /// A single step inside a spritesheet.
        /// </summary>
        public class SpriteSheetStep
        {
            /// <summary>
            /// Vertices buffer.
            /// </summary>
            public VertexPositionNormalTexture[] Vertices { get; internal set; }

            /// <summary>
            /// Vertices indexes.
            /// </summary>
            public short[] Indexes { get; internal set; }
        }

        // dictionary of step names (to get sprite from spritesheet via string identifier)
        Dictionary<string, int> _stepNames = new Dictionary<string, int>();

        /// <summary>
        /// Create spritesheet without any steps (you need to add via AddStep).
        /// </summary>
        public SpriteSheet()
        {
        }

        /// <summary>
        /// Create spritesheet from constant steps count.
        /// </summary>
        public SpriteSheet(Point stepsCount)
        {
            BuildForConstStepSize(stepsCount);
        }

        /// <summary>
        /// Get spritesheet step by index.
        /// </summary>
        /// <param name="index">Step index to get.</param>
        /// <returns>Spritesheet step.</returns>
        public SpriteSheetStep GetStep(int index)
        {
            if (index >= _steps.Count) { throw new System.Exception("Spritesheet step out of range!"); }
            return _steps[index];
        }

        /// <summary>
        /// Get spritesheet step by string identifier (if set).
        /// </summary>
        /// <param name="identifier">Step index to get.</param>
        /// <returns>Spritesheet step.</returns>
        public SpriteSheetStep GetStep(string identifier)
        {
            return GetStep(_stepNames[identifier]);
        }

        /// <summary>
        /// Define a step in the spritesheet.
        /// </summary>
        /// <param name="position">Position in spritesheet texture, in percents (eg values range from 0 to 1).</param>
        /// <param name="size">Size in spritesheet texture, in percents (eg values range from 0 to 1).</param>
        /// <param name="identifier">Optional step string identifier.</param>
        public void AddStep(Vector2 position, Vector2 size, string identifier = null)
        {
            // create the step
            SpriteSheetStep step = BuildStep(position, size);
            _steps.Add(step);

            // if there's a string identifier, set it
            if (identifier != null)
            {
                _stepNames[identifier] = _steps.Count - 1;
            }
        }

        /// <summary>
        /// Build the entire spritesheet from a constant step size.
        /// Note: this only works for spritesheet with constant sprite size, eg all steps are at the same size.
        /// </summary>
        /// <param name="stepsCount">How many sprites there are on X and Y axis of the spritesheet.</param>
        public void BuildForConstStepSize(Point stepsCount)
        {
            // calc size of a single step
            Vector2 size = Vector2.One / new Vector2(stepsCount.X, stepsCount.Y);

            // create all steps
            for (int j = 0; j < stepsCount.Y; ++j)
            {
                for (int i = 0; i < stepsCount.X; ++i)
                {
                    Vector2 position = size * new Vector2(i, j);
                    AddStep(position, size);
                }
            }
        }

        /// <summary>
        /// Attach a string identifier to a spritesheet step.
        /// </summary>
        /// <param name="index">Step index.</param>
        /// <param name="identifier">Identifier to set.</param>
        public void AssignNameToStep(int index, string identifier)
        {
            _stepNames[identifier] = index;
        }

        /// <summary>
        /// Create a single spritesheet step.
        /// </summary>
        /// <param name="positionInSpritesheet">Position in spritesheet (0-1 coords).</param>
        /// <param name="sizeInSpriteSheet">Size in spritesheet (0-1 coords).</param>
        SpriteSheetStep BuildStep(Vector2 positionInSpritesheet, Vector2 sizeInSpriteSheet)
        {
            // create vertices
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[4];

            // set normal
            for (int i = 0; i < 4; ++i)
            {
                vertices[i].Normal = Vector3.Forward;
            }

            // set vertices position and UV
            float halfSize = 0.5f;
            vertices[0].Position = new Vector3(-halfSize, -halfSize, 0);    // bottom left
            vertices[1].Position = new Vector3(-halfSize, halfSize, 0);     // top left
            vertices[2].Position = new Vector3(halfSize, -halfSize, 0);     // bottom right
            vertices[3].Position = new Vector3(halfSize, halfSize, 0);      // top right

            // set vertices UVs
            vertices[0].TextureCoordinate = positionInSpritesheet + sizeInSpriteSheet;                              // bottom left
            vertices[1].TextureCoordinate = positionInSpritesheet + new Vector2(sizeInSpriteSheet.X, 0);            // top left
            vertices[2].TextureCoordinate = positionInSpritesheet + new Vector2(0, sizeInSpriteSheet.Y);            // bottom right
            vertices[3].TextureCoordinate = positionInSpritesheet;                                                  // top right

            // Set the index buffer for each vertex, using clockwise winding
            short[] indexes = new short[6];
            indexes[0] = 0;
            indexes[1] = 1;
            indexes[2] = 2;
            indexes[3] = 2;
            indexes[4] = 1;
            indexes[5] = 3;

            // create a new step and add to steps dictionary
            SpriteSheetStep step = new SpriteSheetStep();
            step.Vertices = vertices;
            step.Indexes = indexes;
            return step;
        }
    }

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
        SpriteSheet.SpriteSheetStep _spritesheetStep;

        // material to draw the sprite with
        Materials.MaterialAPI _material;

        /// <summary>
        /// Blending state of this entity.
        /// </summary>
        public BlendState BlendingState = BlendState.AlphaBlend;

        /// <summary>
        /// Optional custom render settings for this specific sprite instance.
        /// These settings will override some of the material's properties before rendering.
        /// Note: this method is much less efficient than using different materials.
        /// </summary>
        public MaterialOverrides MaterialOverride = new MaterialOverrides();

        // dictionary of shared materials. key is texture path, value is material to use for all sprites with this material.
        static Dictionary<string, Materials.MaterialAPI> _sharedMaterials = new Dictionary<string, Materials.MaterialAPI>();

        /// <summary>
        /// Get / set sprite material.
        /// </summary>
        public Materials.MaterialAPI Material
        {
            get; set;
        }

        /// <summary>
        /// Copy spritesheet step from other sprite.
        /// </summary>
        /// <param name="other">Other sprite to copy step from.</param>
        public void CopyStep(SpriteEntity other)
        {
            _spritesheetStep = other._spritesheetStep;
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
            _material = material;

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
        /// Change the spritesheet and current step of this sprite.
        /// </summary>
        /// <param name="newSpritesheet">New spritesheet data to use.</param>
        /// <param name="startingStep">Step to set from new spritesheet.</param>
        public void ChangeSpritesheet(SpriteSheet newSpritesheet, int startingStep = 0)
        {
            Spritesheet = newSpritesheet;
            _spritesheetStep = Spritesheet.GetStep(startingStep);
        }

        /// <summary>
        /// Set spritesheet step from string identifier.
        /// </summary>
        /// <param name="identifier">Step identifier (must be set in spriteshet).</param>
        public void SetStep(string identifier)
        {
            _spritesheetStep = Spritesheet.GetStep(identifier);
        }

        /// <summary>
        /// Set spritesheet step from index.
        /// </summary>
        /// <param name="index">Step index to set.</param>
        public void SetStep(int index)
        {
            _spritesheetStep = Spritesheet.GetStep(index);
        }

        /// <summary>
        /// Draw this model.
        /// </summary>
        /// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
        public override void DoEntityDraw(Matrix worldTransformations)
        {
            // decompose transformations
            Vector3 position; Quaternion rotation; Vector3 scale;
            worldTransformations.Decompose(out scale, out rotation, out position);

            // create a new world matrix for the billboard
            Matrix newWorld;

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

            // update per-entity override properties
            Materials.MaterialAPI material = MaterialOverride.Apply(_material);

            // set blend state
            GraphicsManager.GraphicsDevice.BlendState = BlendingState;

            // setup material
            material.Apply(newWorld);

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
    }
}