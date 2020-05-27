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
    /// Provides a unified Undo/Redo interface for multiple Undo SubUndoServices.
    /// Change tracking is done by the individual child SubUndoServices. Undo/Redo is done via this class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AggregateUndoService : IUndoRedo
    {
        private readonly List<SubUndoService> _subUndoServices;
        private readonly IntStackWithDelete _undoStack;
        private readonly IntStackWithDelete _redoStack;
        private readonly UndoServiceValidator<int> _undoServiceValidator;

        /// <summary>
        /// Used by the StateSet event handler on subservices to determine if the action was invoked from here.
        /// </summary>
        private bool _isInternallySettingState = false;

        public AggregateUndoService(SubUndoService[] subUndoServices)
        {
            _subUndoServices = subUndoServices.ToList() ?? throw new ArgumentNullException(nameof(subUndoServices));
            
            _undoStack = new IntStackWithDelete();
            _redoStack = new IntStackWithDelete();

            for (var i = 0; i < _subUndoServices.Count; i++)
            {
                _subUndoServices[i].StateRecorded += Subservice_StateRecorded;
                _subUndoServices[i].StateSet += Subservice_StateSet;
                _subUndoServices[i].Index = i;
            }

            _undoServiceValidator = new UndoServiceValidator<int>(_undoStack, _redoStack);
        }

        public event StateSetEventHandler StateSet;

        private void Subservice_StateSet(object sender, StateSetEventArgs e)
        {
            if(!_isInternallySettingState)
            {
                var subserviceIndex = ((SubUndoService)sender).Index;
                _undoStack.DeleteLast(subserviceIndex);
                _redoStack.Push(subserviceIndex);
            }
            StateSet?.Invoke(this, e);
        }

        public bool CanUndo => _undoServiceValidator.CanUndo;

        public bool CanRedo => _undoServiceValidator.CanRedo;

        public void AddSubUndoService(SubUndoService subService)
        {
            if (subService == null)
            {
                throw new ArgumentNullException(nameof(subService));
            }
            subService.StateRecorded += Subservice_StateRecorded;
            subService.Index = _subUndoServices.Count;
            _subUndoServices.Add(subService);
        }

        public void ClearStacks()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            foreach (var s in _subUndoServices)
            {
                s.ClearStacks();
            }
        }

        public void Undo()
        {
            _undoServiceValidator.ValidateUndo();

            var lastServiceIndex = _undoStack.Pop();

            _isInternallySettingState = true;
            _subUndoServices[lastServiceIndex].Undo();
            _isInternallySettingState = false;

            _redoStack.Push(lastServiceIndex);

            //Check if the next SubUndoService has become empty. If it has, then empty all undo stacks.
            if (_undoStack.Count > 0)
            {
                var nextService = _undoStack.Peek();
                if (!_subUndoServices[nextService].CanUndo)
                {
                    ClearUndoStack();
                }
            }
        }

        public void Redo()
        {
            _undoServiceValidator.ValidateRedo();

            var lastServiceIndex = _redoStack.Pop();
            _isInternallySettingState = true;
            _subUndoServices[lastServiceIndex].Redo();
            _isInternallySettingState = false;
            _undoStack.Push(lastServiceIndex);
        }

        private void Subservice_StateRecorded(object sender, EventArgs e)
        {
            var serviceId = ((SubUndoService)sender).Index;
            _undoStack.Push(serviceId);
        }

        public void ClearUndoStack()
        {
            _undoStack.Clear();
            foreach (var s in _subUndoServices)
            {
                s.ClearUndoStack();
            }
        }

    }
}
