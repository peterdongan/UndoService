// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using Newtonsoft.Json;
using NUnit.Framework;
using StateManagement;
using System;

namespace UndoService.Test
{
    public class MyClass
    {
        public int Id { get; set; }     
        public OtherClass MutableMember { get; set; }
    }

    public class OtherClass
    {
        public string Text { get; set; }
    }

    public class UndoServiceWithMutableObjectsExample
    {
        private MyClass _objectBeingTracked;
        private UndoService<string> _undoService;

        public void BrokenGetState1(out MyClass state)
        {
            state = _objectBeingTracked;    // BROKEN - Any changes to _objectBeingTracked will  be applied to the saved state as well.
        }
        public void BrokenGetState2(out MyClass state)
        {
            state = new MyClass { Id = _objectBeingTracked.Id, MutableMember = _objectBeingTracked.MutableMember };    //BROKEN - Any changes to _objectBeingTracked.MutableMember will be applied to the saved state as well.
        }

        public void WorkingGetState(out string state)
        {
            // Any method to perform a deep copy will work here. This one was chosen for brevity.

            state = JsonConvert.SerializeObject(_objectBeingTracked, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        public void BrokenSetState1(MyClass state)
        {
            _objectBeingTracked = state;    //BROKEN - After an undo is performed, changes to _objectBeingTracked will be applied to the saved state as well.
        }

        public void BrokenSetState2(MyClass state)
        {
            _objectBeingTracked.Id = state.Id;
            _objectBeingTracked.MutableMember = state.MutableMember;    //BROKEN - After an undo is performed, changes to _objectBeingTracked.MutableMember will be applied to the saved state as well.
        }

        private void WorkingSetState(string state)
        {
            _objectBeingTracked  = JsonConvert.DeserializeObject<MyClass>(state, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }

        [SetUp]
        public void Setup()
        {
            var otherObject = new OtherClass { Text = "Original" };
            _objectBeingTracked = new MyClass { Id = 1, MutableMember = otherObject };
            _undoService = new UndoService<string>(WorkingGetState,WorkingSetState);
        }

        private void GetState(out MyClass state)
        {
            state = _objectBeingTracked;
        }

        [Test]
        public void UndoRedoTest()
        {
            _objectBeingTracked.Id = 2;
            _undoService.RecordState();
            _objectBeingTracked.Id = 3;
            _undoService.RecordState();
            _undoService.Undo();
            _objectBeingTracked.MutableMember.Text = "Changed";
            _undoService.RecordState();
            _undoService.Undo();
            Assert.IsTrue(_objectBeingTracked.Id == 2 && _objectBeingTracked.MutableMember.Text.Equals("Original"));
        }
       
    }
}