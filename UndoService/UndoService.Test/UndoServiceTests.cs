using NUnit.Framework;
using StateManagement;
using System.Collections.Generic;

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
            _undoServiceForInt = new UndoService<int>(GetIntState, SetIntState, 5);
            _undoServiceForString = new UndoService<string>(GetStringState, SetStringState, 8);
            var subServices = new List<IUndoService> { _undoServiceForInt, _undoServiceForString };
            _aggregateService = new AggregateUndoService(subServices);
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
    }
}