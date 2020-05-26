// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    /// <summary>
    /// Tracks changes to a part of the application for Undo/Redo
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
        /// <param name="tag">An optional tag. StateSetEventArgs will include its value. Can be used to identify the section of the state being changed by Undo/Redo and focus the UI on it.</param>
        void RecordState(object tag = null);

    }
}
