// Copyright (c) Peter Dongan. All rights reserved.
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

using NUnit.Framework;
using StateManagement;
using StateManagement.DataStructures;
using System;

namespace UndoService.Test
{
    public class DropoutStackTests
    {
        private DropoutStack<int> _testStack;
        private int _hasItemsChangedFiredCount = 0;

        [SetUp]
        public void Setup()
        {
            _testStack = new DropoutStack<int>(20);
            _testStack.HasItemsChanged += _testStack_HasItemsChanged;
            _hasItemsChangedFiredCount = 0;
        }

        private void _testStack_HasItemsChanged(object sender, EventArgs e)
        {
            _hasItemsChangedFiredCount++;
        }

        [Test]
        public void TestPush()
        {
            _testStack.Push(1);
            Assert.IsTrue(_hasItemsChangedFiredCount == 1);
            _hasItemsChangedFiredCount = 0;
            _testStack.Push(2);
            Assert.IsFalse(_hasItemsChangedFiredCount == 1);
        }

        [Test]
        public void TestPop()
        {
            _testStack.Push(1);
            _testStack.Push(2);
            _hasItemsChangedFiredCount = 0;
            _testStack.Pop();
            Assert.IsTrue(_hasItemsChangedFiredCount == 0);
            _testStack.Pop();
            Assert.IsTrue(_hasItemsChangedFiredCount == 1);
        }

        [Test]
        public void TestClear()
        {
            _testStack.Clear();
            Assert.IsTrue(_hasItemsChangedFiredCount == 0);
            _testStack.Push(1);
            _hasItemsChangedFiredCount = 0;
            _testStack.Clear();
            Assert.IsTrue(_hasItemsChangedFiredCount == 1);
        }
    }
}