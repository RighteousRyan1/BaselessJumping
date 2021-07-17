using System;

namespace BaselessJumping.Internals.Core.Exceptions
{
    /// <summary>
    /// This exception is thrown when a Keybind is failed to be bound.
    /// </summary>
    public class KeybindFailException : Exception
    {
        public KeybindFailException(string message = default) : base(message)
        {
            if (message == default)
                message = "The Key you were trying to assign was not found!";
        }
    }
}