using NUnit.Framework;
using StateManagement;

namespace UndoService.Test
{
    public class UndoServiceTests
    {
        private AggregateUndoService _aggregateService;
        private UndoService<string> _undoServiceForString;
        private UndoService<int> _undoServiceForInt;
        private string _statefulString;
        private int _statefulInt;

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

        [SetUp]
        public void Setup()
        {
            _undoServiceForInt = new UndoService<int>(GetIntState, SetIntState, 3);
            _undoServiceForString = new UndoService<string>(GetStringState, SetStringState, 5);
            IUndoService[] subservices = { _undoServiceForInt, _undoServiceForString };
            _aggregateService = new AggregateUndoService(subservices);
        }

        [Test]
        public void UndoRedoTest()
        {
            _statefulInt = 1;
            _undoServiceForInt.RecordState();
            _statefulInt = 2;
            _undoServiceForInt.RecordState();
            _undoServiceForInt.Undo();
            Assert.IsTrue(_statefulInt == 1);

            _undoServiceForInt.Redo();
            Assert.IsTrue(_statefulInt == 2);
        }

        [Test]
        public void CapacityTest()
        {
            _statefulInt = 1;
            _undoServiceForInt.RecordState();
            _statefulInt = 2;
            _undoServiceForInt.RecordState();
            _statefulInt = 3;
            _undoServiceForInt.RecordState();
            _statefulInt = 4;
            _undoServiceForInt.RecordState();

            _undoServiceForInt.Undo();
            _undoServiceForInt.Undo();
            _undoServiceForInt.Undo();
            Assert.IsTrue(_statefulInt == 1);
            Assert.IsFalse(_undoServiceForInt.CanUndo);

            _undoServiceForInt.Redo();
            _undoServiceForInt.Redo();
            _undoServiceForInt.Redo();
            Assert.IsTrue(_statefulInt == 4);
        }

        [Test]
        public void AggregateUndoServiceUndoRedoTest()
        {
            _statefulInt = 1;
            _undoServiceForInt.RecordState();
            _statefulString = "One";
            _undoServiceForString.RecordState();
            _statefulInt = 2;
            _undoServiceForInt.RecordState();
            _statefulInt = 3;
            _undoServiceForInt.RecordState();
            _statefulString = "Two";
            _undoServiceForString.RecordState();

            _aggregateService.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 3);

            _aggregateService.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 2);

            _aggregateService.Undo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 1);

            _aggregateService.Redo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 2);

            _aggregateService.Redo();
            Assert.IsTrue(_statefulString.Equals("One"));
            Assert.IsTrue(_statefulInt == 3);

            _aggregateService.Redo();
            Assert.IsTrue(_statefulString.Equals("Two"));
            Assert.IsTrue(_statefulInt == 3);
        }

        [Test]
        public void AggregateUndoServiceCapacityHandlingTest()
        {
            _statefulString = "One";
            _undoServiceForString.RecordState();
            _statefulString = "Two";
            _undoServiceForString.RecordState();
            _statefulInt = 1;
            _undoServiceForInt.RecordState();
            _statefulInt = 2;
            _undoServiceForInt.RecordState();
            _statefulInt = 3;
            _undoServiceForInt.RecordState();
            _statefulInt = 4;
            _undoServiceForInt.RecordState();
            _statefulString = "Three";
            _undoServiceForString.RecordState();

            _aggregateService.Undo();
            _aggregateService.Undo();
            _aggregateService.Undo();
            _aggregateService.Undo();
            Assert.IsFalse(_aggregateService.CanUndo);
            Assert.IsFalse(_undoServiceForInt.CanUndo);
            Assert.IsFalse(_undoServiceForString.CanUndo);
            Assert.IsTrue(_statefulInt == 1);
            Assert.IsTrue(_statefulString.Equals("Two"));

            _aggregateService.Redo();
            _aggregateService.Redo();
            _aggregateService.Redo();
            _aggregateService.Redo();
            Assert.IsTrue(_statefulInt == 4);
            Assert.IsTrue(_statefulString.Equals("Three"));
        }
    }
}