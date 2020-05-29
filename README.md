# UndoService
Simple undo/redo service based on the momento pattern. It uses delegates to access state. It can track changes to different parts of the application individually, while using one unified interface for performing undo/redo. This reduces the memory imprint and facilitates modular design. See the unit tests for examples of usage. [https://github.com/peterdongan/UndoService](https://github.com/peterdongan/UndoService)

## Features
* Multiple undo/redo stacks can be used concurrently, reducing memory imprint.
* Undo/Redo can be performed interchangeably on the state of the application whole or on specific parts.
* Generic stacks are used to store state and delegate methods are used to access it. This means you can use existing classes to store state and existing methods to access it. Therefore very little plumbing is required.
* You can optionally cap the size of the Undo stack or stacks.
* Recorded states can be assigned a tag. This tag will be present in the arguments of an event that is raised when the state is restored via undo or redo. This can be used to identify what was changed and set the focus on that in a UI.

## Usage
The simplest approach is to use a single UndoService for application state. Alternatively you can use separate UndoServices for different sections in conjunction with an AggregatedUndoService. This means that the whole of the application state does not need to be recorded on each change.

To create an UndoService, pass the delegate methods that are used to get and set the state. To create an UndoServiceAggregate, pass a collection of UndoServices.

To use an UndoService, invoke RecordState() **after** making changes to the state. (Note that the initial state is recorded automatically when the UndoService is initialized.) Invoke Undo() and Redo() to undo and redo changes. Use CanUndo and CanRedo to enable/disable Undo/Redo commands in the UI.

To use the UndoServiceAggregate, invoke RecordState() in the child UndoServices to record changes. Generally undo and redo would be done via the UndoServiceAggregate. However, you can also do so in the child UndoServices directly to undo the last changes to specific elements.

## Public interfaces
IStateTracker is used to record changes to state.
IUndoRedo is used to execute Undo and Redo operations.
IUndoService is used to do both.

## LICENCE

MIT

