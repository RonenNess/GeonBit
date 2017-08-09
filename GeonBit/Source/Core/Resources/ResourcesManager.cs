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
// Manage loadable game resources.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using GeonBit.Core.Utils;

namespace GeonBit.Core
{
    /// <summary>
    /// Manage loadable resources (textures / sound / models / etc..).
    /// This class replaces MonoGame's Content Manager. 
    /// </summary>
    public class ResourcesManager
    {
        // singleton instance
        private static ResourcesManager _instance;

        /// <summary>
        /// Get class instance.
        /// </summary>
        static public ResourcesManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ResourcesManager();
                }
                return _instance;
            }
        }

        // content manager
        ContentManager _content;

        /// <summary>
        /// List with models we already loaded and processed.
        /// </summary>
        Dictionary<string, Model> _processedModels = new Dictionary<string, Model>();

        /// <summary>
        /// Private constructor to make it a singleton.
        /// </summary>
        private ResourcesManager()
        {
        }

        /// <summary>
        /// Initialize the resources manager.
        /// </summary>
        /// <param name="content">Content manager.</param>
        public void Initialize(ContentManager content)
        {
            _content = content;
        }

        /// <summary>
        /// Get a sound effect.
        /// </summary>
        /// <param name="path">Asset path / name.</param>
        /// <returns>Sound effect instance.</returns>
        public SoundEffect GetSound(string path)
        {
            return _content.Load<SoundEffect>(path);
        }

        /// <summary>
        /// Get a song.
        /// </summary>
        /// <param name="path">Asset path / name.</param>
        /// <returns>Song instance.</returns>
        public Song GetSong(string path)
        {
            return _content.Load<Song>(path);
        }

        /// <summary>
        /// Get a 2d texture.
        /// </summary>
        /// <param name="path">Asset path / name.</param>
        /// <returns>Texture instance.</returns>
        public Texture2D GetTexture(string path)
        {
            return _content.Load<Texture2D>(path);
        }

        /// <summary>
        /// Get strings array.
        /// </summary>
        /// <param name="path">Asset path / name.</param>
        /// <returns>Strings array.</returns>
        public string[] GetStringArray(string path)
        {
            return _content.Load<string[]>(path);
        }

        /// <summary>
        /// Get an effect (shader).
        /// </summary>
        /// <param name="path">Asset path / name.</param>
        /// <returns>Effect instance.</returns>
        public Effect GetEffect(string path)
        {
            return _content.Load<Effect>(path);
        }

        /// <summary>
        /// Get any other custom type.
        /// </summary>
        /// <typeparam name="T">Type to load.</typeparam>
        /// <param name="path">Path to get resource from.</param>
        /// <returns>Loaded instance.</returns>
        public T GetCustomType<T>(string path)
        {
            return _content.Load<T>(path);
        }

        /// <summary>
        /// Get a 3d model.
        /// </summary>
        /// <param name="path">Asset path / name.</param>
        /// <returns>Model instance.</returns>
        public Model GetModel(string path)
        {
            // model to return
            Model ret;
            
            // try to get from cache of processed models (means we already processed this one)
            if (_processedModels.TryGetValue(path, out ret))
            {
                return ret;
            }
            
            // if we got here it means its a model we didn't load and process yet. load it.
            ret = _content.Load<Model>(path);

            // create GeonBit material per effect and set it as the tag property
            foreach (var mesh in ret.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Tag = Graphics.Materials.DefaultMaterialsFactory.GetDefaultMaterial(effect);
                }
            }

            // add to processed models dict
            _processedModels[path] = ret;

            // return the model
            return ret;
        }
    }

    /// <summary>
    /// Create some extensions to built-in objects.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Get all materials of a mesh.
        /// </summary>
        static public Graphics.Materials.MaterialAPI[] GetMaterials(this ModelMesh mesh)
        {
            ResizableArray<Graphics.Materials.MaterialAPI> ret = new ResizableArray<Graphics.Materials.MaterialAPI>();
            foreach (Effect effect in mesh.Effects)
            {
                ret.Add(effect.Tag as Graphics.Materials.MaterialAPI);
            }
            ret.Trim();
            return ret.InternalArray;
        }

        /// <summary>
        /// Get a material from a mesh effect.
        /// Note: this will only work on an effect that are loaded as part of a model.
        /// </summary>
        static public Graphics.Materials.MaterialAPI GetMaterial(this Effect effect)
        {
            return effect.Tag as Graphics.Materials.MaterialAPI;
        }

        /// <summary>
        /// Get a material from a mesh part.
        /// </summary>
        static public Graphics.Materials.MaterialAPI GetMaterial(this ModelMeshPart meshpart)
        {
            return meshpart.Effect.GetMaterial();
        }
    }
}
