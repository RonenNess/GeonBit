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
// Set of predefined collision groups and masks you can use.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

namespace GeonBit.Core.Physics
{
    /// <summary>
    /// Contain a set of pre-defined collision groups.
    /// This is just for your convinience, you do not have to use this.
    /// </summary>
    public static class CollisionGroups
    {
        /// <summary>
        /// Used for collision with terrain / floor / walls.
        /// </summary>
        public static readonly short Terrain = 1 << 0;

        /// <summary>
        /// Used for collision with static objects.
        /// </summary>
        public static readonly short StaticObjects = 1 << 1;

        /// <summary>
        /// Used for collision with dynamic objects.
        /// </summary>
        public static readonly short DynamicObjects = 1 << 2;

        /// <summary>
        /// Used for collision with powerups.
        /// </summary>
        public static readonly short PowerUps = 1 << 3;

        /// <summary>
        /// Used for collision with Doodads (grass etc.).
        /// </summary>
        public static readonly short Doodads = 1 << 4;

        /// <summary>
        /// Used for collision with the player.
        /// </summary>
        public static readonly short Player = 1 << 5;

        /// <summary>
        /// Used for collision with the player.
        /// </summary>
        public static readonly short PlayerProjectiles = 1 << 6;

        /// <summary>
        /// Used for collision with invisible blocks and force-field like bodies.
        /// </summary>
        public static readonly short ForceField = 1 << 7;

        /// <summary>
        /// Used for collision with any pickable items.
        /// </summary>
        public static readonly short Pickables = 1 << 8;

        /// <summary>
        /// Used for collision with enemies.
        /// </summary>
        public static readonly short Enemies = 1 << 9;

        /// <summary>
        /// Used for collision with enemy projectiles.
        /// </summary>
        public static readonly short EnemyProjectiles = 1 << 10;

        /// <summary>
        /// Used for collision with friendly targets.
        /// </summary>
        public static readonly short Friends = 1 << 11;

        /// <summary>
        /// Used for collision with friends projectiles.
        /// </summary>
        public static readonly short FriendProjectiles = 1 << 12;

        /// <summary>
        /// Used for collision with neutral targets.
        /// </summary>
        public static readonly short Neutrals = 1 << 13;

        /// <summary>
        /// Used for collision with trigger and switches.
        /// </summary>
        public static readonly short Triggers = 1 << 14;

        /// <summary>
        /// Used for collision with any projectiles.
        /// </summary>
        public static readonly short AllProjectiles = OR(EnemyProjectiles, FriendProjectiles, PlayerProjectiles);

        /// <summary>
        /// Used for collision with any characters.
        /// </summary>
        public static readonly short AllCharacters = OR(Enemies, Friends, Player, Neutrals);

        /// <summary>
        /// Combine collision flags using OR operator.
        /// </summary>
        public static short OR(params short[] args)
        {
            short ret = 0;
            foreach (var i in args)
            {
                ret |= i;
            }
            return ret;
        }

        /// <summary>
        /// Combine collision flags using AND operator.
        /// </summary>
        public static short AND(params short[] args)
        {
            short ret = 0;
            foreach (var i in args)
            {
                ret &= i;
            }
            return ret;
        }
    }

    /// <summary>
    /// Contain a set of pre-defined collision masks.
    /// This is just for your convinience, you do not have to use this.
    /// </summary>
    public static class CollisionMasks
    {
        /// <summary>
        /// Default collision mask for all type of characters - characters, player, and enemies.
        /// </summary>
        public static readonly short Characters = 
            CollisionGroups.AllCharacters;

        /// <summary>
        /// Default collision mask for valid targets.
        /// For example, you can use this for projectiles that can hit stuff.
        /// </summary>
        public static readonly short Targets = CollisionGroups.OR(
            CollisionGroups.AllCharacters,
            CollisionGroups.PowerUps,
            CollisionGroups.Pickables,
            CollisionGroups.DynamicObjects,
            CollisionGroups.StaticObjects,
            CollisionGroups.Triggers,
            CollisionGroups.Terrain);

        /// <summary>
        /// Default collision mask for valid dynamic targets.
        /// For example, you can use this for projectiles that can hit stuff.
        /// </summary>
        public static readonly short DynamicTargets = CollisionGroups.OR(
            CollisionGroups.AllCharacters,
            CollisionGroups.PowerUps,
            CollisionGroups.Pickables,
            CollisionGroups.DynamicObjects);

        /// <summary>
        /// All non-character dynamic and static targets.
        /// </summary>
        public static readonly short NonCharacterTargets = CollisionGroups.OR(
            CollisionGroups.PowerUps,
            CollisionGroups.Pickables,
            CollisionGroups.DynamicObjects,
            CollisionGroups.StaticObjects,
            CollisionGroups.Triggers,
            CollisionGroups.Terrain);

        /// <summary>
        /// Default collision mask for targets associated with the player and his allies.
        /// </summary>
        public static readonly short FriendlyTargets = CollisionGroups.OR(
            CollisionGroups.Player,
            CollisionGroups.Friends,
            CollisionGroups.Neutrals);

        /// <summary>
        /// Default collision mask for stuff you can pick up.
        /// </summary>
        public static readonly short Pickups = CollisionGroups.OR(
            CollisionGroups.PowerUps,
            CollisionGroups.Pickables);

        /// <summary>
        /// Default collision mask for all static objects.
        /// </summary>
        public static readonly short Statics = CollisionGroups.OR(
            CollisionGroups.StaticObjects,
            CollisionGroups.ForceField,
            CollisionGroups.Terrain);

        /// <summary>
        /// Default collision mask for everything that should block movement.
        /// </summary>
        public static readonly short Blocking = CollisionGroups.OR(
            CollisionGroups.AllCharacters,
            CollisionGroups.DynamicObjects,
            CollisionGroups.StaticObjects,
            CollisionGroups.ForceField,
            CollisionGroups.Terrain);

    }
}
