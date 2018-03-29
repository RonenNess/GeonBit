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
// Wrap and init the physics world.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace GeonBit.Core.Physics
{
    /// <summary>
    /// GeonBit.Core.Physics implement physics related stuff.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Data provided to physics collision callbacks.
    /// </summary>
    public struct CollisionData
    {
        /// <summary>
        /// Collision point.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Create the collision data.
        /// </summary>
        /// <param name="position">Collision point.</param>
        public CollisionData(Vector3 position)
        {
            Position = position;
        }
    }

    /// <summary>
    /// Raycast results object.
    /// </summary>
    public struct RaycastResults
    {
        /// <summary>
        /// All the data of a single raycast result.
        /// </summary>
        public struct SingleResult
        {
            /// <summary>
            /// Physical body we collided with.
            /// </summary>
            public ECS.Components.Physics.BasePhysicsComponent CollisionBody;

            /// <summary>
            /// Collision normal.
            /// </summary>
            public Vector3 CollisionNormal;

            /// <summary>
            /// Collision position in world space.
            /// </summary>
            public Vector3 CollisionPoint;

            /// <summary>
            /// Hit fraction.
            /// </summary>
            public float HitFraction;
        }

        /// <summary>
        /// Did the raycast hit anything?
        /// </summary>
        public bool HasHit;

        /// <summary>
        /// List of hit results.
        /// </summary>
        public SingleResult[] Collisions;

        /// <summary>
        /// Get first / main result.
        /// Careful not to call this without checking HasHit first.
        /// </summary>
        public SingleResult Collision
        {
            get
            {
                return Collisions[0];
            }
        }
    }

    /// <summary>
    /// The physical world.
    /// </summary>
    public class PhysicsWorld
    {
        // physical world components
        BulletSharp.CollisionConfiguration _config;
        BulletSharp.BroadphaseInterface _broadphase;
        BulletSharp.ConstraintSolver _solver;
        BulletSharp.Dispatcher _dispatcher;

        // physical world
        internal BulletSharp.DynamicsWorld _world;

        // current gravity vector
        BulletSharp.Math.Vector3 _gravity;

        // debug renderer
        PhysicsDebugDraw _debugDraw;

        /// <summary>
        /// Physics max sub steps per frame.
        /// </summary>
        public static int MaxSubStep = 14;

        /// <summary>
        /// Physics time factor.
        /// </summary>
        public static float TimeFactor = 1f;

        /// <summary>
        /// Physics fixed timestep interval.
        /// </summary>
        public static float FixedTimeStep = 1f / 32f;

        /// <summary>
        /// Init the physics world.
        /// </summary>
        public PhysicsWorld()
        {
            // init components
            _config = new BulletSharp.DefaultCollisionConfiguration();
            _dispatcher = new BulletSharp.CollisionDispatcher(_config);
            _broadphase = new BulletSharp.DbvtBroadphase();
            _solver = new BulletSharp.SequentialImpulseConstraintSolver();

            // create world instance
            _world = new BulletSharp.DiscreteDynamicsWorld(
                dispatcher: _dispatcher, 
                pairCache: _broadphase, 
                constraintSolver: _solver, 
                collisionConfiguration: _config);

            // for better performance
            _world.ForceUpdateAllAabbs = false;

            // create debug renderer
            _debugDraw = new PhysicsDebugDraw(Graphics.GraphicsManager.GraphicsDevice);

            // set default gravity
            SetGravity(Vector3.Down * 9.8f);
        }

        /// <summary>
        /// Init the physics world.
        /// </summary>
        ~PhysicsWorld()
        {
            Destroy();
        }

        /// <summary>
        /// Destroy the physical world.
        /// </summary>
        public void Destroy()
        {
            _world = null;
        }

        /// <summary>
        /// Class to store persistent collision data, so that bullet detach events will work.
        /// </summary>
        struct CollisionPersistData
        {
            public BasicPhysicalBody Body0;
            public BasicPhysicalBody Body1;
            public CollisionPersistData(BasicPhysicalBody body0, BasicPhysicalBody body1)
            {
                Body0 = body0;
                Body1 = body1;
            }
        }

        /// <summary>
        /// Initialize physical-engine related stuff and set callbacks to respond to contact start / ended / processed events.
        /// </summary>
        public static void Initialize()
        {
            // set collision start callback
            BulletSharp.ManifoldPoint.ContactAdded += (
                BulletSharp.ManifoldPoint cp,
                BulletSharp.CollisionObjectWrapper obj0,
                int partId0,
                int index0,
                BulletSharp.CollisionObjectWrapper obj1,
                int partId1,
                int index1) =>
            {
                // get physical bodies
                BasicPhysicalBody body0 = obj0.CollisionObject.UserObject as BasicPhysicalBody;
                BasicPhysicalBody body1 = obj1.CollisionObject.UserObject as BasicPhysicalBody;

                // if one of the bodies don't support collision skip
                if (body0 == null || body1 == null) { return; }

                // store both bodies for the collision end event
                cp.UserPersistentData = new CollisionPersistData(body0, body1);

                // send collision events
                CollisionData data = new CollisionData(ToMonoGame.Vector(cp.PositionWorldOnA));
                body0.CallCollisionStart(body1, ref data);
                body1.CallCollisionStart(body0, ref data);
            };

            // set while-collising callback
            BulletSharp.PersistentManifold.ContactProcessed += (
                BulletSharp.ManifoldPoint cp,
                BulletSharp.CollisionObject body0,
                BulletSharp.CollisionObject body1) =>
            {
                if (cp.UserPersistentData == null) { return; }
                CollisionPersistData data = (CollisionPersistData)cp.UserPersistentData;
                data.Body0.CallCollisionProcess(data.Body1);
                data.Body1.CallCollisionProcess(data.Body0);
            };

            // set collising-ended callback
            BulletSharp.PersistentManifold.ContactDestroyed += (object userPersistantData) =>
            {
                if (userPersistantData == null) { return; }
                CollisionPersistData data = (CollisionPersistData)userPersistantData;
                data.Body0.CallCollisionEnd(data.Body1);
                data.Body1.CallCollisionEnd(data.Body0);
            };
        }

        /// <summary>
        /// Called every frame to advance physics simulator.
        /// </summary>
        /// <param name="timeFactor">How much to advance this world step (or: time since last frame).</param>
        public void Update(float timeFactor)
        {
            if (TimeFactor == 0) { return; }
            _world.StepSimulation(timeFactor * TimeFactor, MaxSubStep, FixedTimeStep);
        }

        /// <summary>
        /// Set gravity vector.
        /// </summary>
        /// <param name="gravity"></param>
        public void SetGravity(Vector3 gravity)
        {
            _gravity = ToBullet.Vector(gravity);
            _world.SetGravity(ref _gravity);
        }

        /// <summary>
        /// Perform a raycast test and return colliding results.
        /// </summary>
        /// <param name="start">Start ray vector.</param>
        /// <param name="end">End ray vector.</param>
        /// <param name="returnNearest">If true, will only return the nearest object collided.</param>
        public RaycastResults Raycast(Vector3 start, Vector3 end, bool returnNearest = true)
        {
            // convert start and end vectors to bullet vectors
            BulletSharp.Math.Vector3 bStart = ToBullet.Vector(start);
            BulletSharp.Math.Vector3 bEnd = ToBullet.Vector(end);

            // create class to hold results
            BulletSharp.RayResultCallback resultsCallback = returnNearest ?
                new BulletSharp.ClosestRayResultCallback(ref bStart, ref bEnd) as BulletSharp.RayResultCallback :
                new BulletSharp.AllHitsRayResultCallback(bStart, bEnd);

            // perform ray cast
            return Raycast(bStart, bEnd, resultsCallback);
        }

        /// <summary>
        /// Perform a raycast test and return colliding results, while ignoring 'self' object.
        /// </summary>
        /// <param name="start">Start ray vector.</param>
        /// <param name="end">End ray vector.</param>
        /// <param name="self">Physical body to ignore.</param>
        public RaycastResults Raycast(Vector3 start, Vector3 end, ECS.Components.Physics.BasePhysicsComponent self)
        {
            // convert start and end vectors to bullet vectors
            BulletSharp.Math.Vector3 bStart = ToBullet.Vector(start);
            BulletSharp.Math.Vector3 bEnd = ToBullet.Vector(end);

            // create class to hold results
            BulletSharp.RayResultCallback resultsCallback = 
                new BulletSharp.KinematicClosestNotMeRayResultCallback(self._PhysicalBody._BulletEntity);

            // perform ray cast
            return Raycast(bStart, bEnd, resultsCallback);
        }

        /// <summary>
        /// Perform a raycast test and return colliding results.
        /// </summary>
        /// <param name="start">Start ray vector.</param>
        /// <param name="end">End ray vector.</param>
        /// <param name="resultsCallback">BulletSharp results callback.</param>
        internal RaycastResults Raycast(Vector3 start, Vector3 end, BulletSharp.RayResultCallback resultsCallback)
        {
            // convert start and end vectors to bullet vectors
            BulletSharp.Math.Vector3 bStart = ToBullet.Vector(start);
            BulletSharp.Math.Vector3 bEnd = ToBullet.Vector(end);

            // perform the ray test
            return Raycast(bStart, bEnd, resultsCallback);
        }

        /// <summary>
        /// Perform a raycast test and return colliding results, using native bullet objects.
        /// </summary>
        /// <param name="bStart">Start ray vector (bullet vector).</param>
        /// <param name="bEnd">End ray vector (bullet vector).</param>
        /// <param name="resultsCallback">BulletSharp results callback.</param>
        internal RaycastResults Raycast(BulletSharp.Math.Vector3 bStart, BulletSharp.Math.Vector3 bEnd, BulletSharp.RayResultCallback resultsCallback)
        {
            // perform the ray test
            _world.RayTestRef(ref bStart, ref bEnd, resultsCallback);

            // create results object to return
            RaycastResults results = new RaycastResults();

            // parse data based on type
            // closest result / closest but not me types:
            if (resultsCallback is BulletSharp.ClosestRayResultCallback)
            {
                // convert to closest results type
                BulletSharp.ClosestRayResultCallback closestReults = resultsCallback as BulletSharp.ClosestRayResultCallback;

                // set results data
                results.HasHit = closestReults.HasHit;
                if (results.HasHit)
                {
                    results.Collisions = new RaycastResults.SingleResult[1];
                    results.Collisions[0].HitFraction = closestReults.ClosestHitFraction;
                    results.Collisions[0].CollisionNormal = ToMonoGame.Vector(closestReults.HitNormalWorld);
                    results.Collisions[0].CollisionPoint = ToMonoGame.Vector(closestReults.HitPointWorld);
                    results.Collisions[0].CollisionBody = (closestReults.CollisionObject.UserObject as BasicPhysicalBody).EcsComponent;
                }
            }
            // all results type
            else if (resultsCallback is BulletSharp.AllHitsRayResultCallback)
            {
                // convert to all results type
                BulletSharp.AllHitsRayResultCallback allResults = resultsCallback as BulletSharp.AllHitsRayResultCallback;

                // set results data
                results.HasHit = allResults.HasHit;
                if (results.HasHit)
                {
                    results.Collisions = new RaycastResults.SingleResult[allResults.CollisionObjects.Count];
                    for (int i = 0; i < allResults.CollisionObjects.Count; ++i)
                    {
                        results.Collisions[i].HitFraction = allResults.HitFractions[i];
                        results.Collisions[i].CollisionNormal = ToMonoGame.Vector(allResults.HitNormalWorld[i]);
                        results.Collisions[i].CollisionPoint = ToMonoGame.Vector(allResults.HitPointWorld[i]);
                        results.Collisions[i].CollisionBody = (allResults.CollisionObjects[i].UserObject as BasicPhysicalBody).EcsComponent;
                    }
                }
            }

            // finally, return parsed results
            return results;
        }

        /// <summary>
        /// Add a physical body to the world
        /// </summary>
        /// <param name="body">Physics entity to add.</param>
        public void AddBody(BasicPhysicalBody body)
        {
            Utils.CountAndAlert.Count(Utils.CountAndAlert.PredefAlertTypes.AddedOrCreated);
            body.AddSelfToBulletWorld(_world);
            body._world = this;
        }

        /// <summary>
        /// Update single object's aabb.
        /// </summary>
        /// <param name="body">Body to update.</param>
        public void UpdateSingleAabb(BasicPhysicalBody body)
        {
            _world.UpdateSingleAabb(body._BulletEntity);
        }

        /// <summary>
        /// Remove a physical body from the world.
        /// </summary>
        /// <param name="body"></param>
        public void RemoveBody(BasicPhysicalBody body)
        {
            // this might happen after the world was destroyed, hence the _world != null test.
            if (_world != null) body.RemoveSelfFromBulletWorld(_world);
            body._world = null;
        }

        /// <summary>
        /// Debug-draw the physical world.
        /// </summary>
        public void DebugDraw()
        {
            _debugDraw.DrawDebugWorld(_world);
        }
    }
}
