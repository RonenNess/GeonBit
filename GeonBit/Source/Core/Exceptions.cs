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
// All GeonBit exception types.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion
using System;

namespace GeonBit.Exceptions
{
    /// <summary>
    /// Thrown when the user provided invalid value to a built-in function.
    /// </summary>
    public class InvalidValueException : Exception
    {
        /// <summary>
        /// Create the exception without message.
        /// </summary>
        public InvalidValueException()
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message.
        /// </summary>
        public InvalidValueException(string message)
            : base(message)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message and inner exception.
        /// </summary>
        public InvalidValueException(string message, Exception inner)
            : base(message, inner)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }
    }

    /// <summary>
    /// Thrown when user tries to reach out-of-range index.
    /// </summary>
    public class OutOfRangeException : Exception
    {
        /// <summary>
        /// Create the exception without message.
        /// </summary>
        public OutOfRangeException()
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message.
        /// </summary>
        public OutOfRangeException(string message)
            : base(message)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message and inner exception.
        /// </summary>
        public OutOfRangeException(string message, Exception inner)
            : base(message, inner)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }
    }

    /// <summary>
    /// Thrown for unsupported types / classes.
    /// For example, if the user tries to load an unsupported effect type.
    /// </summary>
    public class UnsupportedTypeException : Exception
    {
        /// <summary>
        /// Create the exception without message.
        /// </summary>
        public UnsupportedTypeException()
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message.
        /// </summary>
        public UnsupportedTypeException(string message)
            : base(message)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message and inner exception.
        /// </summary>
        public UnsupportedTypeException(string message, Exception inner)
            : base(message, inner)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }
    }

    /// <summary>
    /// Thrown for unsupported / invalid actions.
    /// For example, if the user tries to set bones for a material that doesn't support skinned animation.
    /// </summary>
    public class InvalidActionException : Exception
    {
        /// <summary>
        /// Create the exception without message.
        /// </summary>
        public InvalidActionException()
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message.
        /// </summary>
        public InvalidActionException(string message)
            : base(message)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message and inner exception.
        /// </summary>
        public InvalidActionException(string message, Exception inner)
            : base(message, inner)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }
    }

    /// <summary>
    /// Thrown for unexpected internal errors in the engine.
    /// </summary>
    public class InternalError : Exception
    {
        /// <summary>
        /// Create the exception without message.
        /// </summary>
        public InternalError()
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message.
        /// </summary>
        public InternalError(string message)
            : base(message)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }

        /// <summary>
        /// Create the exception with message and inner exception.
        /// </summary>
        public InternalError(string message, Exception inner)
            : base(message, inner)
        {
            Core.Utils.CountAndAlert.Count(Core.Utils.CountAndAlert.PredefAlertTypes.Exception);
        }
    }
}
