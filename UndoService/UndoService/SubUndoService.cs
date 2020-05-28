// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;

namespace StateManagement 
{
    /// <summary>
    /// This is used to track changes to a particular section of the application. It is used in conjunction with UndoServiceAggregate.
    /// </summary>
    internal class SubUndoService : IUndoService
    {
        private readonly IUndoService _undoService;

        public SubUndoService(IUndoService undoService)
        {
            _undoService = undoService ?? throw new NullReferenceException();

            _undoService.StateRecorded += UndoService_StateRecorded;
            _undoService.StateSet += UndoService_StateSet;
        }

        public event StateRecordedEventHandler StateRecorded;

        public event StateSetEventHandler StateSet;

        /// <summary>
        /// This is used by the AggregateUndoService to keep track of where changes were made.
        /// </summary>
        internal int Index { get; set; }

        public bool CanUndo => _undoService.CanUndo;

        public bool CanRedo => _undoService.CanRedo;

        public void RecordState(object tag = null) => _undoService.RecordState( tag);

        public void ClearStacks() => _undoService.ClearStacks();

        public void ClearUndoStack() => _undoService.ClearUndoStack();

        public void Undo() => _undoService.Undo();

        public void Redo() => _undoService.Redo();

        private void UndoService_StateRecorded(object sender, EventArgs e)
        {
            StateRecorded?.Invoke(this, e);
        }

        private void UndoService_StateSet(object sender, StateSetEventArgs e)
        {
            StateSet?.Invoke(this, e);
        }
    }
}
