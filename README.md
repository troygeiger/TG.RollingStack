# TG.RollingStack

A small, rolling LIFO stack for .NET. When the stack reaches capacity, new pushes overwrite the oldest items without reallocating or reorganizing the underlying array.

## Features

- Fixed-capacity stack with rollover behavior
- `Push`, `Pop`, `TryPop`, `Peek`, `TryPeek`, indexer access
- Enumerates from newest to oldest
- Targets `netstandard2.0`

## Usage

```csharp
using TG.RollingStack;

var stack = new RollingStack<int>(3);
stack.Push(1);
stack.Push(2);
stack.Push(3);
stack.Push(4); // rolls over, discards 1

Console.WriteLine(stack.Count); // 3
Console.WriteLine(stack.Peek()); // 4

foreach (var item in stack)
{
    Console.WriteLine(item); // 4, 3, 2
}
```

## API Notes

- `Count` is the number of items currently stored.
- `Capacity` is fixed at construction time.
- `this[int index]` uses `0` for the newest item, `Count - 1` for the oldest.
- `Pop()` throws when empty; `TryPop(out T value)` returns `false` when empty.

## Build and Test

```bash
dotnet build
dotnet test
```
