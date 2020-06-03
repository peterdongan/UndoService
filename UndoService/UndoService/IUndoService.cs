// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement
{
    /// <summary>
    /// Full UndoService interface (inherits IStateTracker, IChangeCounter and IUndoRedo).
    /// </summary>
    public interface IUndoService : IStateTracker, IUndoRedo
    {
        /// <summary>
        /// Raised when Undo or Redo is executed.
        /// </summary>
        event StateSetEventHandler StateSet;

        /// <summary>
        /// Raised when RecordState() is executed.
        /// </summary>
        event StateRecordedEventHandler StateRecorded;

        bool IsStateChanged { get; }

        /// <summary>
        /// 
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// 
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Clear the Undo and Redo stacks.
        /// </summary>
        void ClearStacks();

        void ClearIsChangedFlag();

        /// <summary>
        /// Clear the Undo stack (but not the redo stack).
        /// </summary>
        void ClearUndoStack();

        /// <summary>
        /// 
        /// </summary>
        void Undo();

        /// <summary>
        /// 
        /// </summary>
        void Redo();



        /// <summary>
        /// Records the current state of the tracked objects and puts it on the undo stack
        /// </summary>
        /// <param name="tag">When the tracked object is reverted to this state, a StateSet event will be thrown with this as a property in its arguments. </param>
        void RecordState(object tag = null);
    }
}
