// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using System;
using System.Collections.Generic;

namespace StateManagement
{
    /// <summary>
    /// Provides a unified Undo/Redo interface for multiple UndoServices.
    /// Change tracking is still done by the individual child UndoServices. Undo/Redo is done via this class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AggregateUndoService
    {
        private readonly ISubUndoService[] _undoServices;
        private readonly Stack<int> _undoStack;
        private readonly Stack<int> _redoStack;

        public AggregateUndoService(IUndoService[] undoServices)
        {
            _undoServices = new ISubUndoService[undoServices.Length];
            _undoStack = new Stack<int>();
            _redoStack = new Stack<int>();

            for (var i = 0; i < _undoServices.Length; i++)
            {
                _undoServices[i] = (ISubUndoService)undoServices[i];
                _undoServices[i].StateRecorded += Subservice_StateRecorded;
                _undoServices[i].Id = i;
            }
        }

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
            _undoStack.Push(lastService);
        }

        private void Subservice_StateRecorded(object sender, EventArgs e)
        {
            var serviceId = ((ISubUndoService)sender).Id;
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
