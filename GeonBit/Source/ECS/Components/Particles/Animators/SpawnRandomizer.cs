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
// A special component to random particles starting state.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.ECS.Components.Particles.Animators
{
    /// <summary>
    /// A component that create random starting properties, then destroy self.
    /// </summary>
    public class SpawnRandomizer : BaseComponent
    {
        // all jitters
        float? _minAlpha = null; float? _maxAlpha = null;
        float? _minScale = null; float? _maxScale = null;
        Vector3? _minScaleVector = null; Vector3? _maxScaleVector = null;
        Color? _minColor = null; Color? _maxColor = null;
        Vector3? _positionJitter = null; Vector3? _rotationJitter = null;
        Vector3? _impulseDirection = null; Vector3? _impulseDirectionJitter = null; float? _minImpulseStrength = null; float? _maxImpulseStrength = null;

        /// <summary>
        /// Clone this component.
        /// </summary>
        /// <returns>Cloned copy of this component.</returns>
        override public BaseComponent Clone()
        {
            // note: unlike in other clones that try to copy the entity perfectly, in this clone we create new with jitter
            // so we'll still have the random factor applied on the cloned entity.
            return CopyBasics(new SpawnRandomizer(
                _minAlpha, _maxAlpha, 
                _minScale, _maxScale, 
                _minScaleVector, _maxScaleVector,
                _minColor, _maxColor, 
                _positionJitter, _rotationJitter,
                _impulseDirection, _impulseDirectionJitter, _minImpulseStrength, _maxImpulseStrength));
        }

        // create random instance
        static System.Random Random = new System.Random();

        /// <summary>
        /// Create the spawn randomizer.
        /// </summary>
        /// <param name="minAlpha">Min alpha value (if set, maxAlpha must also be set).</param>
        /// <param name="maxAlpha">Max alpha value (if set, minAlpha must also be set).</param>
        /// <param name="minScale">Min scale value (if set, maxScale must also be set).</param>
        /// <param name="maxScale">Max scale value (if set, minScale must also be set).</param>
        /// <param name="minScaleVector">Min scale value as vector (if set, maxScaleVector must also be set).</param>
        /// <param name="maxScaleVector">Max scale value as vector (if set, minScaleVector must also be set).</param>
        /// <param name="minColor">Min color value (if set, maxColor must also be set).</param>
        /// <param name="maxColor">Max color value (if set, minColor must also be set).</param>
        /// <param name="positionJitter">Random position offset from starting position.</param>
        /// <param name="rotationJitter">Random rotation from starting rotation.</param>
        /// <param name="impulseDirection">Base direction vector to apply impulse (object must have physical body).</param>
        /// <param name="impulseDirectionJitter">Jitter vector to add on impulse direction vector.</param>
        /// <param name="minImpulseStrength">Min impulse force strength (must also set maxImpulseStrength).</param>
        /// <param name="maxImpulseStrength">Max impulse force strength (must also set minImpulseStrength).</param>
        public SpawnRandomizer(
            float? minAlpha = null, float? maxAlpha = null,
            float? minScale = null, float? maxScale = null,
            Vector3? minScaleVector = null, Vector3? maxScaleVector = null,
            Color? minColor = null, Color? maxColor = null,
            Vector3? positionJitter = null, Vector3? rotationJitter = null,
            Vector3? impulseDirection = null, Vector3? impulseDirectionJitter = null,
            float? minImpulseStrength = null, float? maxImpulseStrength = null)
        {
            _minAlpha = minAlpha;
            _maxAlpha = maxAlpha;
            _minScale = minScale;
            _maxScale = maxScale;
            _minScaleVector = minScaleVector;
            _maxScaleVector = maxScaleVector;
            _minColor = minColor;
            _maxColor = maxColor;
            _positionJitter = positionJitter;
            _rotationJitter = rotationJitter;
            _impulseDirection = impulseDirection;
            _impulseDirectionJitter = impulseDirectionJitter;
            _minImpulseStrength = minImpulseStrength;
            _maxImpulseStrength = maxImpulseStrength;
        }

        /// <summary>
        /// Called when GameObject spawns.
        /// </summary>
        protected override void OnSpawn()
        {
            // get model renderers
            Graphics.ModelRenderer[] targets = _GameObject.GetComponents<Graphics.ModelRenderer>().ToArray();

            // random alpha
            if (_minAlpha != null)
            {
                float alpha = _minAlpha.Value + ((float)Random.NextDouble() * (_maxAlpha.Value - _minAlpha.Value));
                foreach (var target in targets)
                {
                    target.MaterialOverride.Alpha = alpha;
                }
            }

            // random scale
            if (_minScale != null)
            {
                float scale = _minScale.Value + ((float)Random.NextDouble() * (_maxScale.Value - _minScale.Value));
                _GameObject.SceneNode.Scale *= scale;
            }

            // random scale by vector
            if (_minScaleVector != null)
            {
                Vector3 scale = AnimatorUtils.RandVector(Random, _minScaleVector.Value, _maxScaleVector.Value);
                _GameObject.SceneNode.Scale *= scale;
            }

            // random color
            if (_minColor != null)
            {
                Color color = AnimatorUtils.RandColor2(Random, _minColor.Value, _maxColor.Value);
                foreach (var target in targets)
                {
                    target.MaterialOverride.DiffuseColor = color;
                }
            }

            // random position
            if (_positionJitter != null)
            {
                Vector3 position = AnimatorUtils.RandVector(Random, _positionJitter.Value);
                _GameObject.SceneNode.Position += position;
            }

            // random rotation
            if (_rotationJitter != null)
            {
                Vector3 rotation = AnimatorUtils.RandVector(Random, _rotationJitter.Value);
                _GameObject.SceneNode.Rotation += rotation;
            }

            // TBD IMPULSE STUFF

            // finally, destroy self
            Destroy();
        }
    }
}
