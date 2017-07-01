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
// Provide simplied API to read and write game data files.
// Much of the code here is credited to Daniel Schroeder: 
// http://blog.danskingdom.com/saving-and-loading-a-c-objects-data-to-an-xml-json-or-binary-file/
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace GeonBit.Managers
{
    /// <summary>
    /// Provide easier access to game files.
    /// </summary>
    public class GameFiles : IManager
    {
        // the singleton instance.
        static GameFiles _instance;

        /// <summary>
        /// Root directory to store game files.
        /// </summary>
        public string GameFilesPath = Path.Combine("GameData", "files");

        /// <summary>
        /// Get application utils instance.
        /// </summary>
        public static GameFiles Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameFiles();
                }
                return _instance;
            }
        }

        /// <summary>
        /// To make it a true singleton.
        /// </summary>
        private GameFiles() { }

        /// <summary>
        /// Different file formats we can use.
        /// </summary>
        public enum FileFormats
        {
            /// <summary>
            /// Read / write objects as binary data.
            /// </summary>
            Binary,

            /// <summary>
            /// Read / write objects as XML.
            /// </summary>
            Xml,
        }

        /// <summary>
        /// Convert path to be under the the game files path (based on GameFilesPath).
        /// </summary>
        /// <param name="path">Path to set.</param>
        /// <param name="createPath">If true, will also create the folders required for path.</param>
        /// <returns>The given path under Game files folder.</returns>
        public string ToGameFilesPath(string path, bool createPath = false)
        {
            // get the full path of the file
            string ret = Path.Combine(GameFilesPath, path);

            // create path if needed
            if (createPath)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ret));
            }

            // return the full path
            return ret;
        }

        /// <summary>
        /// Delete an existing file.
        /// </summary>
        /// <param name="filePath">File to delete.</param>
        public void DeleteFile(string filePath)
        {
            // convert to full path
            filePath = ToGameFilesPath(filePath, false);

            // delete file
            File.Delete(filePath);
        }

        /// <summary>
        /// Writes the given object instance to a file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="format">Which format to use with this file.</param>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public void WriteToFile<T>(FileFormats format, string filePath, T objectToWrite, bool append = false)
        {
            switch (format)
            {
                // write as binary
                case FileFormats.Binary:
                    WriteToBinaryFile<T>(filePath, objectToWrite, append);
                    break;

                // write as xml
                case FileFormats.Xml:
                    if (append)
                    {
                        throw new Exceptions.InvalidActionException("Cannot use 'append' option when writing an XML file.");
                    }
                    WriteToXmlFile<T>(filePath, objectToWrite);
                    break;

                // should never happen.
                default:
                    throw new Exceptions.UnsupportedTypeException("Unknown file format!");
            }
        }

        /// <summary>
        /// Reads an object instance from a file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="format">Which format to use with this file.</param>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public T ReadFromFile<T>(FileFormats format, string filePath)
        {
            switch (format)
            {
                // write as binary
                case FileFormats.Binary:
                    return ReadFromBinaryFile<T>(filePath);

                // write as xml
                case FileFormats.Xml:
                    return ReadFromXmlFile<T>(filePath);

                // should never happen.
                default:
                    throw new Exceptions.UnsupportedTypeException("Unknown file format!");
            }
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            filePath = ToGameFilesPath(filePath, true);
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public T ReadFromBinaryFile<T>(string filePath)
        {
            filePath = ToGameFilesPath(filePath);
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        public void WriteToXmlFile<T>(string filePath, T objectToWrite)
        {
            filePath = ToGameFilesPath(filePath, true);
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, false);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public T ReadFromXmlFile<T>(string filePath)
        {
            filePath = ToGameFilesPath(filePath);
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Update manager.
        /// </summary>
        /// <param name="time">GameTime, as provided by MonoGame.</param>
        public void Update(GameTime time)
        {
        }

        /// <summary>
        /// Called every frame during the Draw() process.
        /// </summary>
        public void Draw(GameTime time)
        {
        }

        /// <summary>
        /// Init game files manager.
        /// </summary>
        public void Initialize()
        {
            // create the game files dir
            System.IO.Directory.CreateDirectory(GameFilesPath);
        }

        /// <summary>
        /// Called every constant X seconds during the Update() phase.
        /// </summary>
        /// <param name="interval">Time since last FixedUpdate().</param>
        public void FixedUpdate(float interval)
        {
        }
    }
}
