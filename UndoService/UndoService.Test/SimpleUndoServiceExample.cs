// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using NUnit.Framework;
using StateManagement;

namespace UndoService.Test
{
    public class SimpleUndoServiceExample
    {
        /// <summary>
        /// We will demonstrate change tracking on this string. (You can track more complex objects, as long as you have methods to get and set their state.)
        /// </summary>
        private string _statefulString;     // (You can track changes of more complex objects.)

        /// <summary>
        /// This is the method to get the state of the tracked object, which will be passed as a delegate to the UndoService.
        /// If you have existing methods to access state, you can probably just put them in a wrapper to match the delegate signature.
        /// </summary>
        private void GetStringState(out string state)
        {
            state = _statefulString;
        }

        /// <summary>
        /// Method to set the state, conforming to the delegate signature.
        /// </summary>
        private void SetStringState(string value)
        {
            _statefulString = value;
        }

        [Test]
        public void UndoRedoTest()
        {
            var undoServiceForString = new UndoService<string>(GetStringState, SetStringState, null);

            _statefulString = "One";
            undoServiceForString.RecordState();
            _statefulString = "Two";
            undoServiceForString.RecordState();

            undoServiceForString.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));

            undoServiceForString.Redo();
            Assert.IsTrue(_statefulString.Equals("Two"));
        }
    }
}