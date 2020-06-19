// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using NUnit.Framework;
using StateManagement

namespace UndoService.Test
{
    public class SimpleUndoServiceAggregateExample
    {
        /// <summary>
        /// This will have an UndoService tracking its changes
        /// </summary>
        private string _statefulString;     

        /// <summary>
        /// This will have an UndoService tracking its changes
        /// </summary>
        private int _statefulInt;

        /// <summary>
        /// Test undo/redo in an AggregateUndoService
        /// </summary>
        [Test]
        public void AggregateUndoServiceUndoRedoTest()
        {
            // UndoServiceAggregate is created using an IUndoService array:
            var undoServiceForInt = new UndoService<int>(GetIntState, SetIntState, null);
            var undoServiceForString = new UndoService<string>(GetStringState, SetStringState, null);
            IUndoService[] subservices = { undoServiceForInt, undoServiceForString };
            var serviceAggregate = new UndoServiceAggregate(subservices);


            // Changes are recorded by the individual UndoServices
            _statefulInt = 1;
            undoServiceForInt.RecordState();
            _statefulString = "One";
            undoServiceForString.RecordState();
            _statefulInt = 2;
            undoServiceForInt.RecordState();
            _statefulInt = 3;
            undoServiceForInt.RecordState();
            _statefulString = "Two";
            undoServiceForString.RecordState();


           /*
            * The UndoServiceAggregate provides a unified interface for performing undo/redo on the different tracked objects.
            * (You can also perform Undo/Redo on the individual services, which will undo the last change on the corresponding object.)
            */
            serviceAggregate.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 3);

            serviceAggregate.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 2);

            serviceAggregate.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 1);

            serviceAggregate.Redo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 2);

            serviceAggregate.Redo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 3);

            serviceAggregate.Redo();
            Assert.IsTrue(_statefulString.Equals("Two"));
            Assert.IsTrue(_statefulInt == 3);
        }


        // These are the methods to get/access state, used by the UndoServices above.

        private void GetStringState(out string state)
        {
            state = _statefulString;
        }

        private void SetStringState(string value)
        {
            _statefulString = value;
        }

        private void GetIntState(out int state)
        {
            state = _statefulInt;
        }

        private void SetIntState(int value)
        {
            _statefulInt = value;
        }
    }
}