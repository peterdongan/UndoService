// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;

namespace StateManagement
{
    /// <summary>
    /// This is just a wrapper for UndoService for when it's used in AggregateService. It adds an index so the parent AggregateUndoService can track which one it is.
    /// </summary>
    internal class SubUndoService 
    {
        /// <summary>
        /// This is used by the AggregateUndoService to keep track of where changes were made.
        /// </summary>
        internal int Index { get; set; }

        private readonly IUndoService _undoService;

        internal SubUndoService(IUndoService undoService)
        {
            _undoService = undoService;

            _undoService.StateRecorded += UndoService_StateRecorded;
        }

        private void UndoService_StateRecorded(object sender, EventArgs e)
        {
            StateRecorded?.Invoke(this, new EventArgs());
        }

        internal event StateRecordedEventHandler StateRecorded;

        internal event StateSetEventHandler StateSet
        {
            add
            {
                _undoService.StateSet += value;
            }
            remove
            {
                _undoService.StateSet -= value;
            }
        }

        internal bool CanUndo => _undoService.CanUndo;

        internal bool CanRedo => _undoService.CanRedo;

        internal void ClearStacks() => _undoService.ClearStacks();

        internal void ClearUndoStack() => _undoService.ClearUndoStack();

        internal void Undo() => _undoService.Undo();

        internal void Redo() => _undoService.Redo();

        internal virtual void RecordState() => _undoService.RecordState();

    }
}
