// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using StateManagement.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StateManagement
{
    /// <summary>
    /// Generic Undo Service using delegates to access state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AggregateUndoService 
    {
        private List<IUndoService> _undoServices;
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

        public AggregateUndoService(IUndoService[] subUndoServices)
        {
            _undoServices = subUndoServices.ToList();
            _undoStack = new Stack<int>();
            _redoStack = new Stack<int>();

            for(var i =0; i< _undoServices.Count; i++)
            {
                _undoServices[i].StateRecorded += S_StateRecorded;
                _undoServices[i].Id = i;
            }
        }

        private void S_StateRecorded(object sender, StateRecordedEventArgs e)
        {
            var serviceId = ((IUndoService)sender).Id;
            _undoStack.Push(serviceId);
        }

        public void ClearStacks()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            foreach(var s in _undoServices)
            {
                s.ClearStacks();
            }
        }

        public void Undo()
        {
            if(!CanUndo)
            {
                throw new Exception("Nothing to undo. Check CanUndo is true before invoking Undo().");
            }
            var lastService = _undoStack.Pop();
            _undoServices[lastService].Undo();
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                throw new Exception("Nothing to redo. Check CanRedo is true before invoking Redo().");
            }

            var lastService = _redoStack.Pop();
            _undoServices[lastService].Redo();
        }

    }
}
