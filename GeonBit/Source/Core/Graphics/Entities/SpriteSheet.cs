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

    /// <summary>
    /// Spritesheet define animation steps of a spritesheet texture.
    /// For example, if you have a spritesheet that describe 4 steps of a walking animation, this
    /// object define the steps of the spritesheet and its texture coords.
    /// </summary>
    public class SpriteSheet
    {
        // list with steps in spritesheet
        List<SpriteSheetStep> _steps = new List<SpriteSheetStep>();

        // dictionary of step names (to get sprite from spritesheet via string identifier)
        Dictionary<string, int> _stepsIdentifierToIndex = new Dictionary<string, int>();

        /// <summary>
        /// Create spritesheet without any steps (you need to add via AddStep).
        /// </summary>
        public SpriteSheet()
        {
            BuildForConstStepSize(new Point(1, 1));
        }

        /// <summary>
        /// Create spritesheet from constant steps count.
        /// </summary>
        public SpriteSheet(Point? stepsCount)
        {
            if (stepsCount != null)
                BuildForConstStepSize(stepsCount.Value);
        }

        /// <summary>
        /// Get spritesheet step by index.
        /// </summary>
        /// <param name="index">Step index to get.</param>
        /// <returns>Spritesheet step.</returns>
        public SpriteSheetStep GetStep(int index)
        {
            if (index >= _steps.Count) { throw new Exceptions.OutOfRangeException("Spritesheet step out of range!"); }
            return _steps[index];
        }

        /// <summary>
        /// Get spritesheet step by string identifier (if set).
        /// </summary>
        /// <param name="identifier">Step index to get.</param>
        /// <returns>Spritesheet step.</returns>
        public SpriteSheetStep GetStep(string identifier)
        {
            return GetStep(_stepsIdentifierToIndex[identifier]);
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
                _stepsIdentifierToIndex[identifier] = _steps.Count - 1;
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
            _stepsIdentifierToIndex[identifier] = index;
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
    /// Define an animation clip from animation steps.
    /// </summary>
    public class SpriteAnimationClip
    {
        /// <summary>
        /// Animation starting step index.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Animation ending step index.
        /// </summary>
        public int EndIndex { get; private set; }

        /// <summary>
        /// Animation playing speed.
        /// This number basically means how many animation steps we pass per second.
        /// </summary>
        public float Speed { get; private set; }

        /// <summary>
        /// Does this animation clip plays in loop.
        /// </summary>
        public bool IsLooping { get; private set; }

        // optional delay to add per-step
        Dictionary<int, float> _perStepDelays = new Dictionary<int, float>();

        /// <summary>
        /// Create animation clip.
        /// </summary>
        /// <param name="startStep">Starting step index.</param>
        /// <param name="endStep">Ending step index.</param>
        /// <param name="speed">Animation speed.</param>
        /// <param name="loop">If true, will play animation in loop.</param>
        public SpriteAnimationClip(int startStep, int endStep, float speed = 1f, bool loop = true)
        {
            // sanity check
            if (startStep > endStep)
            {
                throw new Exceptions.InvalidValueException("Animation clip ending step index cannot be smaller than starting step index!");
            }

            // store properties
            StartIndex = startStep;
            EndIndex = endStep;
            Speed = speed;
            IsLooping = loop;
        }

        /// <summary>
        /// Get how long, in seconds, we need to wait per step (based on general speed and per-step delays).
        /// </summary>
        /// <param name="stepRelativeIndex">Step index to get delay for (step index is relative to starting index).</param>
        /// <returns>How long, in seconds, we need to wait on this animation step.</returns>
        public float DelayForStep(int stepRelativeIndex)
        {
            float ret = 1f / Speed;
            float extra;
            if (_perStepDelays.TryGetValue(stepRelativeIndex, out extra))
            {
                return ret + extra;
            }
            return ret;
        }

        /// <summary>
        /// Add special per-step delay.
        /// </summary>
        /// <param name="stepRelativeIndex">On which step to add this delay (step index is relative to starting index).</param>
        /// <param name="delay">How long, in seconds, to add extra delay. Can be negative to create faster steps.</param>
        public void AddStepDelay(int stepRelativeIndex, float delay)
        {
            _perStepDelays[stepRelativeIndex] = delay;
        }
    }

    /// <summary>
    /// A callback function for when animation clip ends.
    /// </summary>
    public delegate void OnAnimationClipEnds();

    /// <summary>
    /// A sprite animation clip that's currently playing.
    /// </summary>
    public class SpriteAnimationClipPlay
    {
        /// <summary>
        /// Animation clip.
        /// </summary>
        public SpriteAnimationClip Clip { get; private set; }

        /// <summary>
        /// Speed factor for this specific animation play.
        /// </summary>
        public float SpeedFactor = 1f;

        /// <summary>
        /// Get current step index.
        /// </summary>
        public int CurrentStep { get; private set; }

        /// <summary>
        /// Callback to call when animation clip ends.
        /// Note: if looping animation this will be called every time a cycle ends. If not looping, will be called once.
        /// </summary>
        public OnAnimationClipEnds OnAnimationEnd;

        // Time until next animation step.
        float _timeForNextStep = 0f;

        // turns true if animation is not looping and we reached the end.
        bool _isDone = false;

        /// <summary>
        /// Create a new animation clip play instance.
        /// </summary>
        /// <param name="clip">Which animation clip to play.</param>
        /// <param name="speed">Animation playing speed.</param>
        /// <param name="startingStep">Optional starting step index.</param>
        public SpriteAnimationClipPlay(SpriteAnimationClip clip, float speed = 1f, int? startingStep = null)
        {
            Clip = clip;
            SpeedFactor = speed;
            CurrentStep = clip.StartIndex + startingStep ?? 0;
            _timeForNextStep = clip.DelayForStep(CurrentStep - Clip.StartIndex);
        }

        /// <summary>
        /// Get / set the current animation step, relative to starting step.
        /// </summary>
        public int RelativeStep
        {
            set
            {
                CurrentStep = Clip.StartIndex + value;
            }
            get
            {
                return CurrentStep - Clip.StartIndex;
            }
        }

        /// <summary>
        /// Advance animation and return current animation step index.
        /// </summary>
        /// <param name="timeFactor">Time factor for animation playing speed.</param>
        /// <returns>Current animation step index.</returns>
        public int Update(float timeFactor)
        {
            // if finished, return last index and stop here
            if (_isDone)
            {
                return CurrentStep;
            }

            // advance current step
            _timeForNextStep -= timeFactor * SpeedFactor;

            // did we finish current step?
            if (_timeForNextStep <= 0f)
            {
                // advance current animation step
                CurrentStep++;

                // did we finish animation clip?
                if (CurrentStep > Clip.EndIndex)
                {
                    // if its looping, just start over
                    if (Clip.IsLooping)
                    {
                        CurrentStep = Clip.StartIndex;
                    }
                    // if not looping, mark as done and return to last step to get "stuck" on
                    else
                    {
                        _isDone = true;
                        CurrentStep = Clip.EndIndex;
                    }

                    // invoke finish animation callback
                    OnAnimationEnd?.Invoke();
                }

                // get time until next step
                _timeForNextStep = Clip.DelayForStep(CurrentStep - Clip.StartIndex);
            }

            // return current animation step
            return CurrentStep;
        }
    }
}