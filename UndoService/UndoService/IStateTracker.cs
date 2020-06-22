// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;

namespace StateManagement
{
    /// <summary>
    /// Records state changes.
    /// </summary>
    public interface IStateTracker
    {
        /// <summary>
        /// Raised when Undo or Redo is executed.
        /// </summary>
        event StateSetEventHandler StateSet;

        /// <summary>
        /// Raised when RecordState() is executed.
        /// </summary>
        event StateRecordedEventHandler StateRecorded;

        /// <summary>
        /// Raised when a public method to clear one or both of the undo and redo stacks is invoked.
        /// </summary>
        event StackClearInvokedEventHandler ClearStackInvoked;

        /// <summary>
        /// Records the current state of the tracked objects and puts it on the undo stack
        /// </summary>
        /// <param name="tag">When the tracked object is reverted to this state, a StateSet event will be thrown with this as a property in its arguments. </param>
        void RecordState(object tag = null);

    }
}
