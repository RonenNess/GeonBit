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
// Implements basic resizeable array.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System;


namespace GeonBit.Core.Utils
{
    /// <summary>
    /// An array you can add elements to, but still access the internal array object.
    /// </summary>
    /// <typeparam name="T">Type to store in array.</typeparam>
    class ResizableArray<T>
    {
        /// <summary>
        /// Items array.
        /// </summary>
        T[] m_array;

        /// <summary>
        /// Items count.
        /// </summary>
        int m_count;

        /// <summary>
        /// Create the resizable array with default starting size.
        /// </summary>
        /// <param name="initialCapacity">Optional initial starting size.</param>
        public ResizableArray(int? initialCapacity = null)
        {
            m_array = new T[initialCapacity ?? 4];
        }

        /// <summary>
        /// Get the internal array.
        /// </summary>
        public T[] InternalArray { get { return m_array; } }

        /// <summary>
        /// Get array real size.
        /// </summary>
        public int Count { get { return m_count; } }

        /// <summary>
        /// Clear the array.
        /// </summary>
        public void Clear()
        {
            m_array = new T[4];
            m_count = 0;
        }

        /// <summary>
        /// Remove the extra buffer from array and resize it to actual size.
        /// </summary>
        public void Trim()
        {
            Array.Resize(ref m_array, m_count);
        }

        /// <summary>
        /// Add element to array.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Add(T element)
        {
            // check if need to enlarge array
            if (m_count == m_array.Length)
            {
                Array.Resize(ref m_array, m_array.Length * 2);
            }

            // add to array and increase count
            m_array[m_count++] = element;
        }

        /// <summary>
        /// Add range of values to array.
        /// </summary>
        /// <param name="values"></param>
        public void AddRange(T[] values)
        {
            foreach (var val in values)
            {
                Add(val);
            }
        }
    }
}
