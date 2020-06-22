// Copyright (c) Peter Dongan. All rights reserved. 
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using StateManagement.DataStructures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace StateManagement
{
    /// <summary>
    /// Provides a unified Undo/Redo interface for multiple Undo SubUndoServices.
    /// Change tracking is done by the individual child UndoServices. 
    /// </summary>
    public class UndoServiceAggregate : IUndoRedo
    {
        private readonly List<SubUndoService> _subUndoServices;
        private readonly IntStackWithDelete _undoStack;
        private readonly IntStackWithDelete _redoStack;
        private readonly UndoServiceValidator<int> _undoServiceValidator;
        
        /// <summary>
        /// Semaphgore counting the number of clear stack methods invoked.
        /// </summary>
        private int _clearStackInvocationsCount;

        /// <summary>
        /// Used by the StateSet event handler on subservices to determine if the action was invoked from here.
        /// </summary>
        private bool _isInternallySettingState = false;

        /// <summary>
        /// Create an aggregate of UndoServices.
        /// </summary>
        /// <param name="subUndoServices"></param>
        public UndoServiceAggregate(IUndoService[] subUndoServices)
        {
            _undoStack = new IntStackWithDelete();
            _redoStack = new IntStackWithDelete();
            _subUndoServices = new List<SubUndoService>();
            _clearStackInvocationsCount = 0;

            if(subUndoServices == null)
            {
                throw new ArgumentNullException(nameof(subUndoServices));
            }

            for (var i = 0; i < subUndoServices.Length; i++)
            {
                AddUndoService(subUndoServices[i]);
            }

            _undoServiceValidator = new UndoServiceValidator<int>(_undoStack, _redoStack);
        }

        /// <summary>
        /// Raised when Undo or Redo is performed.
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public bool IsStateChanged
        {
            get
            {
                return _subUndoServices.Any(x => x.IsStateChanged);
            }
        }

        /// <summary>
        /// </summary>
        public bool CanUndo => _undoServiceValidator.CanUndo;

        /// <summary>
        /// </summary>
        public bool CanRedo => _undoServiceValidator.CanRedo;

        /// <summary>
        /// Include a new SubUndoService in the aggregated Undo/Redo stack.
        /// </summary>
        /// <param name="subService"></param>
        public void AddUndoService(IUndoService subService)
        {
            if (subService == null)
            {
                throw new ArgumentNullException(nameof(subService));
            }

            var resourceManager = new ResourceManager(typeof(StateManagement.Resources));

            if(subService.CanRedo || subService.CanRedo)
            {
                throw new Exception(resourceManager.GetString("AddingPopulatedSubserviceExceptionMessage", CultureInfo.CurrentCulture));
            }
            var nextSubService = new SubUndoService(subService);
            nextSubService.StateRecorded += Subservice_StateRecorded;
            nextSubService.StateSet += Subservice_StateSet;
            nextSubService.ClearStackInvoked += NextSubService_ClearStackInvoked;
            nextSubService.Index = _subUndoServices.Count;
            _subUndoServices.Add(nextSubService);
        }

        private void NextSubService_ClearStackInvoked(object sender, EventArgs e)
        {
            if(_clearStackInvocationsCount <1)
            {
                var err = new InvalidOperationException("A clear stack method was invoked directly on an UndoService that is part of an UndoServiceAggregate. Invoke the clear stack methods on the UndoServiceAggregate instead.");
                throw err;
            }
            _clearStackInvocationsCount--;
        }


        /// <summary>
        /// Clear the Undo and Redo stacks for this object and all its subservices.
        /// </summary>
        public void ClearStacks()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            foreach (var s in _subUndoServices)
            {
                _clearStackInvocationsCount++;
                s.ClearStacks();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Undo()
        {
            _undoServiceValidator.ValidateUndo();

            var lastServiceIndex = _undoStack.Pop();

            _isInternallySettingState = true;
            try
            {
                _subUndoServices[lastServiceIndex].Undo();
            }
            catch
            {
                throw new Exception("Undo failed in subservice when aggregate expected it to succeed.");
            }
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

        /// <summary>
        /// 
        /// </summary>
        public void Redo()
        {
            _undoServiceValidator.ValidateRedo();

            var lastServiceIndex = _redoStack.Pop();
            _isInternallySettingState = true;
            try
            {
                _subUndoServices[lastServiceIndex].Redo();
            }
            catch
            {
                throw new Exception("Redo failed in subservice when aggregate expected it to succeed.");
            }
            _isInternallySettingState = false;
            _undoStack.Push(lastServiceIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearRedoStack()
        {
            _redoStack.Clear();
            foreach (var s in _subUndoServices)
            {
                _clearStackInvocationsCount++;
                s.ClearRedoStack();
            }
        }

        /// <summary>
        /// </summary>
        public void ClearUndoStack()
        {
            _undoStack.Clear();
            foreach (var s in _subUndoServices)
            {
                _clearStackInvocationsCount++;
                s.ClearUndoStack();
            }
        }

        public void ClearIsChangedFlag()
        {
            foreach(var s in _subUndoServices)
            {
                s.ClearIsChangedFlag();
            }
        }

        private void Subservice_StateRecorded(object sender, EventArgs e)
        {
            var serviceId = ((SubUndoService)sender).Index;
            _undoStack.Push(serviceId);
            if (_redoStack.Count > 0)
            {
                ClearRedoStack();
            }
        }

    }
}
