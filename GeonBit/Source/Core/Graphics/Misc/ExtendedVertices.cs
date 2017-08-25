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
// Define some custom vertices types.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;


namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Vertex type for normal mapping
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalTangentTexture : IVertexType
    {
        /// <summary>
        /// Vertex position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Vertex normal.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Texture coords.
        /// </summary>
        public Vector2 TextureCoordinate;

        /// <summary>
        /// Tangent.
        /// </summary>
        public Vector3 Tangent;

        /// <summary>
        /// Binormal.
        /// </summary>
        public Vector3 Binormal;

        /// <summary>
        /// Vertex declaration object.
        /// </summary>
        public static readonly VertexDeclaration VertexDeclaration;

        /// <summary>
        /// Vertex declaration.
        /// </summary>
        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        /// <summary>
        /// Static constructor to init vertex declaration.
        /// </summary>
        static VertexPositionNormalTangentTexture()
        {
            VertexElement[] elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                new VertexElement(44, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)};
            VertexDeclaration declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }


        /// <summary>
        /// Create the vertex.
        /// </summary>
        /// <param name="position">Vertex position.</param>
        /// <param name="normal">Vertex normal.</param>
        /// <param name="textureCoordinate">Texture coordinates.</param>
        /// <param name="tangent">Vertex tangent.</param>
        /// <param name="binormal">Vertex binormal.</param>
        public VertexPositionNormalTangentTexture(Vector3 position, Vector3 normal, Vector2 textureCoordinate, Vector3 tangent, Vector3 binormal)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoordinate = textureCoordinate;
            this.Tangent = tangent;
            this.Binormal = binormal;
        }

        /// <summary>
        /// Calculate tangent and binormal values automatically.
        /// </summary>
        public void CalcTangentBinormal()
        {
            // calc c1 and c2
            Vector3 c1 = Vector3.Cross(Normal, new Vector3(0.0f, 0.0f, 1.0f));
            Vector3 c2 = Vector3.Cross(Normal, new Vector3(0.0f, 1.0f, 0.0f));

            // check which is more fitting to be tangent
            Tangent = (c1.Length() > c2.Length()) ? c1 : c2;
            Tangent.Normalize();

            // calc binormal
            Binormal = Vector3.Normalize(Vector3.Cross(Normal, Tangent));
        }

        /// <summary>
        /// Get if equals another object.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>If objects are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != base.GetType())
            {
                return false;
            }
            return (this == ((VertexPositionNormalTangentTexture)obj));
        }

        /// <summary>
        /// Get the hash code of this vertex.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ TextureCoordinate.GetHashCode();
                hashCode = (hashCode * 397) ^ Tangent.GetHashCode();
                hashCode = (hashCode * 397) ^ Binormal.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Return string representation of this vertex.
        /// </summary>
        /// <returns>String representation of the vertex.</returns>
        public override string ToString()
        {
            return "{{Position:" + this.Position + " Normal:" + this.Normal + " TextureCoordinate:" + this.TextureCoordinate + " Tangent " + this.Tangent + "}}";
        }

        /// <summary>
        /// Return if two vertices are equal.
        /// </summary>
        /// <param name="left">Left side to compare.</param>
        /// <param name="right">Right side to compare.</param>
        /// <returns>If equal.</returns>
        public static bool operator ==(VertexPositionNormalTangentTexture left, VertexPositionNormalTangentTexture right)
        {
            return (((left.Position == right.Position) && (left.Normal == right.Normal)) && (left.TextureCoordinate == right.TextureCoordinate) && left.Binormal == right.Binormal && left.Tangent == right.Tangent);
        }

        /// <summary>
        /// Return if two vertices are not equal.
        /// </summary>
        /// <param name="left">Left side to compare.</param>
        /// <param name="right">Right side to compare.</param>
        /// <returns>If not equal.</returns>
        public static bool operator !=(VertexPositionNormalTangentTexture left, VertexPositionNormalTangentTexture right)
        {
            return !(left == right);
        }
    }
}