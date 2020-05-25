# UndoService
Simple undo/redo service based on the momento pattern. It uses delegates to access state. It can track changes to different parts of the application individually, while using one unified interface for performing undo/redo. This reduces the memory imprint and facilitates modular design. See the unit tests for examples of usage. [https://github.com/peterdongan/UndoService](https://github.com/peterdongan/UndoService)

## Usage
To create an UndoService, set the type that is used to record the state. Pass the delegate methods that are used to get and set the state.

To use an UndoService, invoke RecordState() after changing the state. Invoke Undo() and Redo() to undo and redo changes. CanUndo and CanRedo should be checked respectively before invoking these methods. (Note that you should invoke RecordState() **after** making changes, as opposed to before you make them.

To use multiple UndoServices to track different parts of your application, use an AggregateUndoService. Pass an array of the different UndoServices to its constructor. In this case, RecordState() is still done by the UndoServices, but Undo() and Redo() must only be done via the AggregateUndoService.


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
This can be used with multiple UndoServices to manage Undo/Redo across separate segments of the application. In this case, the child UndoServices look after change tracking and Undo/Redo is done via the AggregateUndoService. Most of the members are similar to UndoService so they are not duplicated here.

### AggregateUndoService Constructor
Pass an array of the UndoServices that are used for different parts of the application.

```csharp
AggregateUndoService(IUndoService[] subUndoServices)
```

## LICENCE

MIT
