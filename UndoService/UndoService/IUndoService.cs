// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    public interface IUndoService
    {
        event StateRecordedEventHandler StateRecorded;

        /// <summary>
        /// Occurs when Undo or Redo is performed.
        /// </summary>
        event StateSetEventHandler StateSet;

        bool CanUndo { get; }

        bool CanRedo { get; }

        /// <summary>
        /// Records the current state of the tracked objects. Invoking Undo() will revert to the previous (not current) recorded state. 
        /// </summary>
        void RecordState();

        /// <summary>
        /// Clear the Undo and Redo stacks.
        /// </summary>
        void ClearStacks();

        void ClearUndoStack();

        void Undo();

        void Redo();
    }
}
