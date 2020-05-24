// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using StateManagement.DataStructures;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace StateManagement
{
    /// <summary>
    /// Validates Undo/Redo operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UndoServiceValidator<T>
    {
        private readonly IStack<T> _undoStack;
        private readonly Stack<T> _redoStack;

        public UndoServiceValidator(IStack<T> undoStack, Stack<T> redoStack)
        {
            _undoStack = undoStack;
            _redoStack = redoStack;
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

        public void ValidateUndo()
        {
            if (!CanUndo)
            {
                var resourceManager = new ResourceManager(typeof(StateManagement.Resources));
                throw new EmptyStackException(resourceManager.GetString("UndoWithoutCanUndo", CultureInfo.CurrentCulture));
            }
        }

        public void ValidateRedo()
        {
            if (!CanRedo)
            {
                var resourceManager = new ResourceManager(typeof(StateManagement.Resources));
                throw new EmptyStackException(resourceManager.GetString("RedoWithoutCanRedo", CultureInfo.CurrentCulture));
            }
        }
    }
}
