// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    /// <summary>
    /// Tracks changes to a part of the application for Undo/Redo. Used in conjunction with IUndoRedo
    /// </summary>
    public interface IStateTracker
    {
        /// <summary>
        /// Occurs when Undo or Redo is performed.
        /// </summary>
        event StateSetEventHandler StateSet;

        event StateRecordedEventHandler StateRecorded;

        /// <summary>
        /// Records the current state of the tracked objects and puts it on the undo stack
        /// </summary>
        /// <param name="tag">When the tracked object is reverted to this state, a StateSet event will be thrown with this as a property in its arguments. </param>
        void RecordState(object tag = null);

    }
}
