// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;

namespace StateManagement
{
    /// <summary>
    /// This is just a wrapper for UndoService that adds an index so AggregateUndoService can track which one it is.
    /// </summary>
    internal class SubUndoService : ISubUndoService
    {
        /// <summary>
        /// This is used by the AggregateUndoService to keep track of where changes were made.
        /// </summary>
        int ISubUndoService.Index { get; set; }

        private readonly IUndoService _undoService;

        public SubUndoService(IUndoService undoService)
        {
            _undoService = undoService;

            _undoService.StateRecorded += _undoService_StateRecorded;
        }

        private void _undoService_StateRecorded(object sender, EventArgs e)
        {
            StateRecorded?.Invoke(this, new EventArgs());
        }

        public event StateRecordedEventHandler StateRecorded;

        public event StateSetEventHandler StateSet
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

        public bool CanUndo => _undoService.CanUndo;

        public bool CanRedo => _undoService.CanRedo;

        public void ClearStacks() => _undoService.ClearStacks();

        public void ClearUndoStack() => _undoService.ClearUndoStack();

        public void Undo() => _undoService.Undo();

        public void Redo() => _undoService.Redo();

        public virtual void RecordState() => _undoService.RecordState();

    }
}
