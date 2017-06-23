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
// Define the physical body shape constructors (classes used to define the
// physical body shapes).
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using GeonBit.Core.Physics.CollisionShapes;

namespace GeonBit.ECS.Components.Physics
{
    /// <summary>
    /// Different physical body shapes we can have.
    /// </summary>
    public enum PhysicalBodyShapeTypes
    {
        /// <summary>
        /// An endless plane.
        /// </summary>
        EndlessPlane,

        /// <summary>
        /// Sphere shape.
        /// </summary>
        Sphere,

        /// <summary>
        /// Box shape.
        /// </summary>
        Box,

        /// <summary>
        /// 2D Box shape.
        /// </summary>
        Box2D,

        /// <summary>
        /// Capsule shape.
        /// </summary>
        Capsule,

        /// <summary>
        /// Cone shape.
        /// </summary>
        Cone,

        /// <summary>
        /// Convex hull shape.
        /// </summary>
        ConvexHull,

        /// <summary>
        /// Cylinder shape.
        /// </summary>
        Cylinder,

        /// <summary>
        /// Triangle shape.
        /// </summary>
        Triangle,
    }

    /// <summary>
    /// The interface for a physical body shape info (object with data about how to create
    /// the shape for this physical body).
    /// </summary>
    public interface IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        PhysicalBodyShapeTypes ShapeType { get; }

        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        ICollisionShape CreateShape();
    }

    /// <summary>
    /// Data struct to init an endless plane physical body.
    /// </summary>
    public class EndlessPlaneInfo : IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        public PhysicalBodyShapeTypes ShapeType { get { return PhysicalBodyShapeTypes.EndlessPlane; } }

        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        public ICollisionShape CreateShape()
        {
            return new CollisionEndlessPlane(Normal);
        }

        /// <summary>
        /// Plane normal.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Create the info to create endless plane body.
        /// </summary>
        /// <param name="normal">Plane normal (default to Vector3.Up).</param>
        public EndlessPlaneInfo(Vector3? normal = null)
        {
            Normal = normal ?? Vector3.Up;
        }
    }

    /// <summary>
    /// Data struct to init a sphere physical body.
    /// </summary>
    public class SphereInfo : IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        public PhysicalBodyShapeTypes ShapeType { get { return PhysicalBodyShapeTypes.Sphere; } }

        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        public ICollisionShape CreateShape()
        {
            return new CollisionSphere(Radius);
        }

        /// <summary>
        /// Sphere radius
        /// </summary>
        public float Radius;

        /// <summary>
        /// Create the info to create a sphere body.
        /// </summary>
        /// <param name="radius">Sphere radius.</param>
        public SphereInfo(float radius = 1f)
        {
            Radius = radius;
        }
    }

    /// <summary>
    /// Data struct to init a box physical body.
    /// </summary>
    public class BoxInfo : IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        public PhysicalBodyShapeTypes ShapeType { get { return PhysicalBodyShapeTypes.Box; } }

        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        public ICollisionShape CreateShape()
        {
            return new CollisionBox(BaseSize.X, BaseSize.Y, BaseSize.Z);
        }

        /// <summary>
        /// Box basic size.
        /// </summary>
        public Vector3 BaseSize;

        /// <summary>
        /// Create the info to create a box body.
        /// </summary>
        /// <param name="baseSize">Box base size (default to Vector3.One).</param>
        public BoxInfo(Vector3? baseSize = null)
        {
            BaseSize = baseSize ?? Vector3.One;
        }
    }

    /// <summary>
    /// Data struct to init a box-2d physical body.
    /// </summary>
    public class Box2dInfo : IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        public PhysicalBodyShapeTypes ShapeType { get { return PhysicalBodyShapeTypes.Box2D; } }
        
        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        public ICollisionShape CreateShape()
        {
            return new CollisionBox2D(BaseSize.X, BaseSize.Y, BaseSize.Z);
        }

        /// <summary>
        /// Box basic size.
        /// </summary>
        public Vector3 BaseSize;

        /// <summary>
        /// Create the info to create a box2d body.
        /// </summary>
        /// <param name="baseSize">Box base size (default to Vector3.One).</param>
        public Box2dInfo(Vector3? baseSize = null)
        {
            BaseSize = baseSize ?? Vector3.One;
        }
    }

    /// <summary>
    /// Data struct to init a capsule physical body.
    /// </summary>
    public class CapsuleInfo : IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        public PhysicalBodyShapeTypes ShapeType { get { return PhysicalBodyShapeTypes.Capsule; } }

        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        public ICollisionShape CreateShape()
        {
            return new CollisionCapsule(Radius, Height, Axis);
        }

        /// <summary>
        /// Capsule radius.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Capsule height.
        /// </summary>
        public float Height;

        /// <summary>
        /// Capsule align axis.
        /// </summary>
        public CapsuleDirectionAxis Axis;

        /// <summary>
        /// Create the info to create a capsule body.
        /// </summary>
        /// <param name="radius">Capsule radius.</param>
        /// <param name="height">Capsule height.</param>
        /// <param name="axis">Capsule direction axis (Y = standing up).</param>
        public CapsuleInfo(float radius = 1f, float height = 1f, CapsuleDirectionAxis axis = CapsuleDirectionAxis.Y)
        {
            Radius = radius;
            Height = height;
            Axis = axis;
        }
    }

    /// <summary>
    /// Data struct to init a cone physical body.
    /// </summary>
    public class ConeInfo : IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        public PhysicalBodyShapeTypes ShapeType { get { return PhysicalBodyShapeTypes.Cone; } }

        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        public ICollisionShape CreateShape()
        {
            return new CollisionCone(Radius, Height, Axis);
        }

        /// <summary>
        /// Capsule radius.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Capsule height.
        /// </summary>
        public float Height;

        /// <summary>
        /// Cone align axis.
        /// </summary>
        public ConeDirectionAxis Axis;

        /// <summary>
        /// Create the info to create a cone body.
        /// </summary>
        /// <param name="radius">Cone radius.</param>
        /// <param name="height">Cone height.</param>
        /// <param name="axis">Cone direction axis (Y = pointing up).</param>
        public ConeInfo(float radius = 1f, float height = 1f, ConeDirectionAxis axis = ConeDirectionAxis.Y)
        {
            Radius = radius;
            Height = height;
            Axis = axis;
        }
    }

    /// <summary>
    /// Data struct to init a convex hull physical body.
    /// </summary>
    public class ConvexHullInfo : IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        public PhysicalBodyShapeTypes ShapeType { get { return PhysicalBodyShapeTypes.ConvexHull; } }

        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        public ICollisionShape CreateShape()
        {
            return new CollisionConvexHull(Points);
        }

        /// <summary>
        /// Hull shape points.
        /// </summary>
        public Vector3[] Points;

        /// <summary>
        /// Create the info to create a convex-hull body.
        /// </summary>
        /// <param name="points">Points to build the convex hull from.</param>
        public ConvexHullInfo(Vector3[] points)
        {
            Points = points;
        }
    }

    /// <summary>
    /// Data struct to init a cylinder physical body.
    /// </summary>
    public class CylinderInfo : IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        public PhysicalBodyShapeTypes ShapeType { get { return PhysicalBodyShapeTypes.Cylinder; } }

        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        public ICollisionShape CreateShape()
        {
            return new CollisionCylinder(HalfExtent, Axis);
        }

        /// <summary>
        /// Half extent of the cylinder on X, Y and Z axis.
        /// </summary>
        public Vector3 HalfExtent;
        
        /// <summary>
        /// Cylinder align axis.
        /// </summary>
        public CylinderDirectionAxis Axis;

        /// <summary>
        /// Create the info to create a cylinder body.
        /// </summary>
        /// <param name="halfExtent">Cylinder half extent.</param>
        /// <param name="axis">Cylinder direction axis (Y = standing up).</param>
        public CylinderInfo(Vector3 halfExtent, CylinderDirectionAxis axis = CylinderDirectionAxis.Y)
        {
            HalfExtent = halfExtent;
            Axis = axis;
        }

        /// <summary>
        /// Create the info to create a cylinder body.
        /// </summary>
        /// <param name="radius">Cylinder radius.</param>
        /// <param name="height">Cylinder height.</param>
        /// <param name="axis">Cylinder direction axis (Y = standing up).</param>
        public CylinderInfo(float radius, float height, CylinderDirectionAxis axis = CylinderDirectionAxis.Y) : 
            this(new Vector3(radius, height, radius), axis)
        {
        }
    }

    /// <summary>
    /// Data struct to init a triangle physical body.
    /// </summary>
    public class TriangleInfo : IBodyShapeInfo
    {
        /// <summary>
        /// Get shape type enum.
        /// </summary>
        public PhysicalBodyShapeTypes ShapeType { get { return PhysicalBodyShapeTypes.Triangle; } }

        /// <summary>
        /// Create and return a collision shape of this type.
        /// </summary>
        /// <returns>Collision shape instance.</returns>
        public ICollisionShape CreateShape()
        {
            return new CollisionTriangle(P1, P2, P3);
        }

        /// <summary>
        /// Triangle point 1.
        /// </summary>
        public Vector3 P1;

        /// <summary>
        /// Triangle point 2.
        /// </summary>
        public Vector3 P2;

        /// <summary>
        /// Triangle point 3.
        /// </summary>
        public Vector3 P3;

        /// <summary>
        /// Create the info to create a triangle body.
        /// </summary>
        /// <param name="p1">Triangle point 1.</param>
        /// <param name="p2">Triangle point 2.</param>
        /// <param name="p3">Triangle point 3.</param>
        public TriangleInfo(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }
    }
}
