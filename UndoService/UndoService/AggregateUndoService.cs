// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System.Collections.Generic;

namespace StateManagement
{
    /// <summary>
    /// Generic Undo Service using delegates to access state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AggregateUndoService
    {
        private readonly List<IUndoService> _undoServices;
        private readonly Stack<int> _undoStack;
        private readonly Stack<int> _redoStack;


        public bool CanUndo
        {
            get
            {
                return _undoStack.Count > 0;
            }
        }

        public bool CanRedo
        {
            get
            {
                return _redoStack.Count > 0;
            }
        }

        public int Id { get; set; }

        public AggregateUndoService(List<IUndoService> subUndoServices)
        {
            _undoServices = subUndoServices;
            _undoStack = new Stack<int>();
            _redoStack = new Stack<int>();

            for (var i = 0; i < _undoServices.Count; i++)
            {
                _undoServices[i].StateRecorded += Subservice_StateRecorded;
                _undoServices[i].Id = i;
            }
        }

        public void ClearStacks()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            foreach (var s in _undoServices)
            {
                s.ClearStacks();
            }
        }

        public void Undo()
        {
            if (!CanUndo)
            {
                throw new EmptyStackException("Nothing to undo. Check CanUndo is true before invoking Undo().");
            }

            var lastService = _undoStack.Pop();
            _undoServices[lastService].Undo();
            _redoStack.Push(lastService);

            //Check if the next undoservice has become empty. If it has, then empty all undo stacks.
            if (_undoStack.Count > 0)
            {
                var nextService = _undoStack.Peek();
                if (!_undoServices[nextService].CanUndo)
                {
                    ClearUndoStacks();
                }
            }
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                throw new EmptyStackException("Nothing to redo. Check CanRedo is true before invoking Redo().");
            }

            var lastService = _redoStack.Pop();
            _undoServices[lastService].Redo();
        }

        private void Subservice_StateRecorded(object sender, StateRecordedEventArgs e)
        {
            var serviceId = ((IUndoService)sender).Id;
            _undoStack.Push(serviceId);
        }

        private void ClearUndoStacks()
        {
            _undoStack.Clear();
            foreach (var s in _undoServices)
            {
                s.ClearUndoStack();
            }
        }

    }
}
