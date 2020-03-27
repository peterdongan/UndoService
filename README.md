# UndoService
Generic undo service using delegates to access state. [https://github.com/peterdongan/UndoService](https://github.com/peterdongan/UndoService)

## USAGE
### Constructor
Set the type that is used to record the state. 
Pass the delegate methods that are used to get and set the state. (If necessary use a wrapper to match the expected signature.) 
Optionally set a cap on the number of states stored. 
```csharp
UndoService(GetState<T> getState, SetState<T> setState, int? cap)	
```

If there is no cap then stacks are used to store the states. If there is a cap then a dropout stack is used for the undo stack.

### Properties
Check CanUndo and CanRedo properties before invoking Undo() or Redo() respectively.
```csharp
bool CanUndo
bool CanRedo
```

### Methods
Invoke RecordState() to add the current state to the undo history. 

```csharp
void RecordState() 
void Undo()
void Redo()
void Clear()
```

## LICENCE

MIT
