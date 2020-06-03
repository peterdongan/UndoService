// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    /// <summary>
    /// Counts the number of state changes made to an object
    /// </summary>
    public interface IChangeCounter
    {
        /// <summary>
        /// Number of changes made
        /// </summary>
        int ChangeCount { get; }

        /// <summary>
        /// Reset ChangeCount
        /// </summary>
        void ResetChangeCount();
    }
}
