# UndoService
Simple undo/redo service based on the momento pattern. It uses delegates to access state. It can track changes to different parts of the application individually, while using one unified interface for performing undo/redo. This reduces the memory imprint and facilitates modular design. See the unit tests for examples of usage. 


## Features
* Multiple undo/redo stacks can be used concurrently, reducing memory imprint.
* Undo/Redo can be performed interchangeably on the state of the application whole or on specific parts.
* No requirement to write custom classes or methods as it uses generic stacks and delegate methods.
* Optional cap on the size of Undo stacks.
* Recorded states can be tagged. Tag values are present in the arguments of the StateSet event, which is raised on Undo or Redo.


## Usage
The simplest approach is to use a single UndoService for application state. Alternatively you can use separate UndoServices for different sections in conjunction with an UndoServiceAggregate. This means that the whole of the application state does not need to be recorded on each change.

To create an UndoService, pass the delegate methods that are used to get and set the state. To use it, invoke RecordState() **after** making changes to the state. (Note that the initial state is recorded automatically when the UndoService is initialized.) Invoke Undo() and Redo() to undo and redo changes. Use CanUndo and CanRedo to enable/disable Undo/Redo commands.

To create an UndoServiceAggregate, pass a collection of UndoServices. To use it, invoke RecordState() in the child UndoServices to record changes. Generally undo and redo would be done via the UndoServiceAggregate. However, you can also do so in the child UndoServices directly to undo the last changes to specific elements.


## Public interfaces
* IStateTracker is used to record changes to state. Implemented by UndoService.
* IUndoRedo is used to execute Undo and Redo operations. Implemented by UndoService and UndoServiceAggregate.
* IUndoService is used to both record state and perform undo/redo. Implemented by UndoService.

```csharp
    /// <summary>
    /// Tracks changes to a part of the application for Undo/Redo. Used in conjunction with IUndoRedo
    /// </summary>
    public interface IStateTracker
    {
        /// <summary>
        /// Raised when Undo or Redo is executed.
        /// </summary>
        event StateSetEventHandler StateSet;

        /// <summary>
        /// Raised when RecordState() is executed.
        /// </summary>
        event StateRecordedEventHandler StateRecorded;

        /// <summary>
        /// Records the current state of the tracked objects and puts it on the undo stack
        /// </summary>
        /// <param name="tag">When the tracked object is reverted to this state, a StateSet event will be thrown with this as a property in its arguments. </param>
        void RecordState(object tag = null);
    }
```

```csharp
    /// <summary>
    /// Performs Undo/redo actions. Used in conjunction with object(s) that implement IStateTracker
    /// </summary>
    public interface IUndoRedo
    {
        bool CanUndo { get; }
        
        bool CanRedo { get; }

        /// <summary>
        /// Clear the Undo and Redo stacks.
        /// </summary>
        void ClearStacks();

        /// <summary>
        /// Clear the Undo stack (but not the redo stack).
        /// </summary>
        void ClearUndoStack();

        void Undo();

        void Redo();
    }
```


## Links
* [Home](https://peterdongan.github.io/UndoService/)
* [Repo](https://github.com/peterdongan/UndoService)
* [Nuget Package](https://www.nuget.org/packages/UndoService)


## Licence
Copyright 2020 Peter Dongan. Free to use under the [MIT licence](https://licenses.nuget.org/MIT).
