﻿// Copyright (c) Peter Dongan. All rights reserved. 
// Licensed under the MIT licence. https://opensource.org/licenses/MIT
// Project: https://github.com/peterdongan/UndoService

namespace StateManagement.DataStructures
{
    /// <summary>
    /// Interface to allow using Stacks or Dropout stacks interchangeably. (Dropout stacks are used where a capacity limit needs to be applied.)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IStack<T>
    {
        /// <summary>
        /// Occurs when the stack changes from having items to not having items and vice-versa
        /// </summary>
        event HasItemsChangedEventHandler HasItemsChanged;

        int Count { get; }

        void Push(T item);

        T Pop();

        void Clear();

        T Peek();
    }
}
