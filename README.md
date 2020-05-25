IUndoService[] subservices = { _undoServiceForInt, _undoServiceForString };

I want to use a public interface for SubService that only exposes the RecordState() method.
I also want to have an internal interface that exposes all the IUndoService methods ann also an Index.
It has to be a typed class so as I understand it I need to only use the interfaces if using them in a collection or a non-typed class or a method in a non-typed class.

A public factory method:

Instead of passing the services to the constructor, use an add() method to add them.
That would actually work ok even if they added them after the service had been running.#


# UndoService
Simple undo/redo service based on the momento pattern. It uses delegates to access state. It can track changes to different parts of the application individually, while using one unified interface for performing undo/redo. This reduces the memory imprint and facilitates modular design. See the unit tests for examples of usage. [https://github.com/peterdongan/UndoService](https://github.com/peterdongan/UndoService)

## Usage
Use UndoService to track the state on a single stack.

To create an UndoService, set the type that is used to record the state. Pass the delegate methods that are used to get and set the state.

To use an UndoService, invoke RecordState() **after** making changes to the state. (Note that the initial state is recorded automatically when the UndoService is initialized.) Invoke Undo() and Redo() to undo and redo changes. Use CanUndo and CanRedo to enable/disable Undo/Redo commands in the UI.

Use AggregateUndoService if you want to use different stacks to track the changes to different parts of your application. This uses an array of SubUndoServices. Each SubUndoService is used to record the state of a different part of the application.

To create an AggregateUndoService, first create the SubUndoServices. To create a SubUndoService, use the static factory method CreateSubUndoService(...) in the SubUndoService class. The arguments are the same as for the UndoService constructor. 

To use the AggregateUndoService, invoke RecordState() in the SubUndoServices to record changes. Invoke Undo/Redo etc on the AggregateService itself.


## UndoService Class
This can be used by itself to record state and perform undo/redo actions, or it can be used as a subservice of AggregateUndoService to manage changes to a particular segment. In the latter case, Undo() and Redo() should only be invoked via the AggregateUndoService. View the unit tests for example of both forms of use.

### UndoService Constructor
Set the type that is used to record the state. 
Pass the delegate methods that are used to get and set the state. (If necessary use a wrapper to match the expected signature.) 
Optionally set a cap on the number of states stored. 
```csharp
UndoService(GetState<T> getState, SetState<T> setState, int? cap)	
```

If there is no cap then stacks are used for the undo and the redo stack. If there is a cap then a dropout stack is used for the undo stack.

### UndoService Properties
Check CanUndo and CanRedo properties before invoking Undo() or Redo() respectively.
```csharp
bool CanUndo
bool CanRedo
```

### UndoService Methods
Invoke RecordState() to add the current state to the undo history. 

```csharp
void RecordState() 
void Undo()
void Redo()
void ClearStacks()
void ClearUndoStacks()
```

## AggregateUndoService Class
This can be used with multiple UndoServices to manage Undo/Redo across separate segments of the application. In this case, the child SubUndoServices look after change tracking and Undo/Redo is done via the AggregateUndoService. Most of the members are similar to UndoService so they are not duplicated here.

### AggregateUndoService Constructor
Pass an array of the SubUndoServices that are used for different parts of the application.

```csharp
AggregateUndoService(IUndoService[] subUndoServices)
```

## LICENCE

MIT
