//// Copyright (c) Peter Dongan. All rights reserved.
//// Licensed under the MIT licence. https://opensource.org/licenses/MIT
//// Project: https://github.com/peterdongan/UndoService

//using NUnit.Framework;
//using StateManagement;

//namespace UndoService.Test
//{
//    public class UndoServiceTestsBase
//    {
//        protected AggregateUndoService AggregateService;
//        protected UndoService<int> IndividualUndoService;   
//        protected UndoService<int> UndoServiceForInt;
//        protected SubUndoService SubUndoServiceForString;
//        protected SubUndoService SubUndoServiceForInt;
//        protected string StatefulString;     //(In real use, more complex objects would be used to store state.)
//        protected int StatefulInt;

//        protected void GetStringState(out string state)
//        {
//            state = StatefulString;
//        }

//        protected void SetStringState(string value)
//        {
//            StatefulString = value;
//        }

//        protected void GetIntState(out int state)
//        {
//            state = StatefulInt;
//        }

//        protected void SetIntState(int value)
//        {
//            StatefulInt = value;
//        }

//        [SetUp]
//        public void Setup()
//        {
//            UndoServiceForInt = new UndoService<int>(GetIntState, SetIntState, 3);
//            IndividualUndoService = new UndoService<int>(GetIntState, SetIntState, 3);
//            SubUndoServiceForInt = SubUndoService.CreateSubUndoService<int>(GetIntState, SetIntState, 3);
//            SubUndoServiceForString = SubUndoService.CreateSubUndoService<string>(GetStringState, SetStringState, 3);
//            SubUndoService[] subservices = { SubUndoServiceForInt, SubUndoServiceForString };
//            AggregateService = new AggregateUndoService(subservices);
//        }

//        /// <summary>
//        /// Test undo/redo in a single UndoService
//        /// </summary>
//        [Test]
//        public void UndoRedoTest()
//        {
//            StatefulInt = 1;
//            UndoServiceForInt.RecordState();
//            StatefulInt = 2;
//            UndoServiceForInt.RecordState();
//            UndoServiceForInt.Undo();
//            Assert.IsTrue(StatefulInt == 1);

//            UndoServiceForInt.Redo();
//            Assert.IsTrue(StatefulInt == 2);
//        }

//        /// <summary>
//        /// Test that capacity limits are applied to a single UndoService
//        /// </summary>
//        [Test]
//        public void CapacityTest()
//        {
//            StatefulInt = 1;
//            UndoServiceForInt.RecordState();
//            StatefulInt = 2;
//            UndoServiceForInt.RecordState();
//            StatefulInt = 3;
//            UndoServiceForInt.RecordState();
//            StatefulInt = 4;
//            UndoServiceForInt.RecordState();

//            UndoServiceForInt.Undo();
//            UndoServiceForInt.Undo();
//            UndoServiceForInt.Undo();
//            Assert.IsTrue(StatefulInt == 1);
//            Assert.IsFalse(UndoServiceForInt.CanUndo);

//            UndoServiceForInt.Redo();
//            UndoServiceForInt.Redo();
//            UndoServiceForInt.Redo();
//            Assert.IsTrue(StatefulInt == 4);
//        }

//        /// <summary>
//        /// Test undo/redo in an AggregateUndoService
//        /// </summary>
//        [Test]
//        public void AggregateUndoServiceUndoRedoTest()
//        {
//            StatefulInt = 1;
//            SubUndoServiceForInt.RecordState();
//            StatefulString = "One";
//            SubUndoServiceForString.RecordState();
//            StatefulInt = 2;
//            SubUndoServiceForInt.RecordState();
//            StatefulInt = 3;
//            SubUndoServiceForInt.RecordState();
//            StatefulString = "Two";
//            SubUndoServiceForString.RecordState();

//            AggregateService.Undo();
//            Assert.IsTrue(StatefulString.Equals("One"));
//            Assert.IsTrue(StatefulInt == 3);

//            AggregateService.Undo();
//            Assert.IsTrue(StatefulString.Equals("One"));
//            Assert.IsTrue(StatefulInt == 2);

//            AggregateService.Undo();
//            Assert.IsTrue(StatefulString.Equals("One"));
//            Assert.IsTrue(StatefulInt == 1);

//            AggregateService.Redo();
//            Assert.IsTrue(StatefulString.Equals("One"));
//            Assert.IsTrue(StatefulInt == 2);

//            AggregateService.Redo();
//            Assert.IsTrue(StatefulString.Equals("One"));
//            Assert.IsTrue(StatefulInt == 3);

//            AggregateService.Redo();
//            Assert.IsTrue(StatefulString.Equals("Two"));
//            Assert.IsTrue(StatefulInt == 3);
//        }

//        /// <summary>
//        /// Test that AggregateUndoService detects when one of its component UndoServices has nothing left in its undo stack, and that it makes that point the effective end of its own undo stack.
//        /// </summary>
//        [Test]
//        public void AggregateUndoServiceCapacityHandlingTest()
//        {
//            StatefulString = "One";
//            SubUndoServiceForString.RecordState();
//            StatefulString = "Two";
//            SubUndoServiceForString.RecordState();
//            StatefulInt = 1;
//            SubUndoServiceForInt.RecordState();
//            StatefulInt = 2;
//            SubUndoServiceForInt.RecordState();
//            StatefulInt = 3;
//            SubUndoServiceForInt.RecordState();
//            StatefulInt = 4;
//            SubUndoServiceForInt.RecordState();
//            StatefulString = "Three";
//            SubUndoServiceForString.RecordState();

//            AggregateService.Undo();
//            AggregateService.Undo();
//            AggregateService.Undo();
//            AggregateService.Undo();
//            Assert.IsFalse(AggregateService.CanUndo);
//            Assert.IsTrue(StatefulInt == 1);
//            Assert.IsTrue(StatefulString.Equals("Two"));

//            AggregateService.Redo();
//            AggregateService.Redo();
//            AggregateService.Redo();
//            AggregateService.Redo();
//            Assert.IsTrue(StatefulInt == 4);
//            Assert.IsTrue(StatefulString.Equals("Three"));
//        }

//        /// <summary>
//        /// Test that individual UndoServices work without anything listening to the StateRecorded event.
//        /// </summary>
//        [Test]
//        public void NoEventHandlerTest()
//        {
//            StatefulInt = 1;
//            IndividualUndoService.RecordState();
//            StatefulInt = 2;
//            IndividualUndoService.RecordState();

//            IndividualUndoService.Undo();
//            Assert.IsTrue(StatefulInt == 1);

//            IndividualUndoService.Redo();
//            Assert.IsTrue(StatefulInt == 2);
//        }

//        /// <summary>
//        /// Test taht you can undo actions after redoing them in a single UndoService
//        /// </summary>
//        [Test]
//        public void RedoUndoSingleTest()
//        {
//            StatefulInt = 1;
//            UndoServiceForInt.RecordState();
//            StatefulInt = 2;
//            UndoServiceForInt.RecordState();
//            UndoServiceForInt.Undo();
//            UndoServiceForInt.Redo();

//            Assert.IsTrue(UndoServiceForInt.CanUndo);

//            UndoServiceForInt.Undo();
//            Assert.IsTrue(StatefulInt == 1);

//        }

//        /// <summary>
//        /// Test that you can undo actions after redoing them in an AggregateUndoService.
//        /// </summary>
//        [Test]
//        public void RedoUndoAggregateTest()
//        {
//            StatefulInt = 1;
//            SubUndoServiceForInt.RecordState();
//            StatefulString = "One";
//            SubUndoServiceForString.RecordState();
//            StatefulInt = 2;
//            SubUndoServiceForInt.RecordState();
//            StatefulInt = 3;
//            SubUndoServiceForInt.RecordState();
//            StatefulString = "Two";
//            SubUndoServiceForString.RecordState();

//            AggregateService.Undo();
//            AggregateService.Undo();
//            AggregateService.Undo();
//            AggregateService.Redo();
//            AggregateService.Redo();
//            AggregateService.Redo();

//            Assert.IsTrue(AggregateService.CanUndo);

//            AggregateService.Undo();
//            Assert.IsTrue(StatefulString.Equals("One"));
//            Assert.IsTrue(StatefulInt == 3);

//            AggregateService.Undo();
//            Assert.IsTrue(StatefulString.Equals("One"));
//            Assert.IsTrue(StatefulInt == 2);

//            AggregateService.Undo();
//            Assert.IsTrue(StatefulString.Equals("One"));
//            Assert.IsTrue(StatefulInt == 1);

//        }
//    }
//}