using System.Collections;
using System.Linq;

namespace TG.RollingStack.Tests;

public class RollingTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestOverFill()
    {
        var stack = new RollingStack<int>(5);
        for (int i = 0; i < 10; i++)
        {
            stack.Push(i);
        }

        Assert.That(stack.Count, Is.EqualTo(stack.Capacity));
        Assert.That(stack[0], Is.EqualTo(9));
        Assert.That(stack[1], Is.EqualTo(8));
        Assert.That(stack[2], Is.EqualTo(7));
        Assert.That(stack[3], Is.EqualTo(6));
        Assert.That(stack[4], Is.EqualTo(5));
    }

    [Test]
    public void TestPop()
    {
        var stack = new RollingStack<int>(5);
        for (int i = 0; i < 5; i++)
        {
            stack.Push(i);
        }

        Assert.That(stack.Pop(), Is.EqualTo(4));
        Assert.That(stack.Pop(), Is.EqualTo(3));
        Assert.That(stack.Count, Is.EqualTo(3));
        Assert.That(stack[0], Is.EqualTo(2));
        Assert.That(stack[1], Is.EqualTo(1));
        Assert.That(stack[2], Is.EqualTo(0));
    }

    [Test]
    public void TestTryPop()
    {
        var stack = new RollingStack<int>(5);
        int lastValue = 0;
        for (int i = 0; i < 5; i++)
        {
            stack.Push(i);
            lastValue = i;
        }

        while (stack.TryPop(out var item))
        {
            Assert.That(item, Is.EqualTo(lastValue));
            lastValue--;
        }
    }

    [Test]
    public void TestRolloverThenPushPop()
    {
        var stack = new RollingStack<int>(5);
        for (int i = 0; i < 10; i++)
        {
            stack.Push(i);
        }

        Assert.That(stack.Count, Is.EqualTo(stack.Capacity));
        Assert.That(stack.Pop(), Is.EqualTo(9));
        stack.Push(10);
        Assert.That(stack.Pop(), Is.EqualTo(10));
    }

    [Test]
    public void TestIndexerOutOfRangeThrows()
    {
        var stack = new RollingStack<int>(3);
        stack.Push(1);

        Assert.That(() => _ = stack[-1], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => _ = stack[1], Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void TestPeekAndTryPeek()
    {
        var stack = new RollingStack<int>(3);

        Assert.That(() => stack.Peek(), Throws.TypeOf<InvalidOperationException>());
        Assert.That(stack.TryPeek(out var emptyValue), Is.False);
        Assert.That(emptyValue, Is.EqualTo(default(int)));

        stack.Push(42);
        Assert.That(stack.Peek(), Is.EqualTo(42));
        Assert.That(stack.TryPeek(out var value), Is.True);
        Assert.That(value, Is.EqualTo(42));
        Assert.That(stack.Count, Is.EqualTo(1));
    }

    [Test]
    public void TestPopOnEmptyThrows()
    {
        var stack = new RollingStack<int>(2);

        Assert.That(() => stack.Pop(), Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void TestEnumerationOrder()
    {
        var stack = new RollingStack<int>(5);
        for (int i = 0; i < 5; i++)
        {
            stack.Push(i);
        }

        var items = stack.ToArray();
        Assert.That(items, Is.EqualTo(new[] { 4, 3, 2, 1, 0 }));
    }

    [Test]
    public void TestEnumerationAfterRollover()
    {
        var stack = new RollingStack<int>(5);
        for (int i = 0; i < 8; i++)
        {
            stack.Push(i);
        }

        var items = stack.ToArray();
        Assert.That(items, Is.EqualTo(new[] { 7, 6, 5, 4, 3 }));
    }

    [Test]
    public void TestReverseExtensionUsesEnumerationOrder()
    {
        var stack = new RollingStack<int>(4);
        for (int i = 0; i < 4; i++)
        {
            stack.Push(i);
        }

        var reversed = stack.Reverse().ToArray();
        Assert.That(reversed, Is.EqualTo(new[] { 0, 1, 2, 3 }));
    }

    [Test]
    public void TestEnumeratorReset()
    {
        var stack = new RollingStack<int>(3);
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        using var enumerator = stack.GetEnumerator();
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(3));

        enumerator.Reset();
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(3));
    }

    [Test]
    public void TestCapacityOneRollover()
    {
        var stack = new RollingStack<int>(1);
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        Assert.That(stack.Count, Is.EqualTo(1));
        Assert.That(stack[0], Is.EqualTo(3));
        Assert.That(stack.ToArray(), Is.EqualTo(new[] { 3 }));
        Assert.That(stack.Pop(), Is.EqualTo(3));
        Assert.That(stack.TryPop(out _), Is.False);
    }

    [Test]
    public void TestCapacityTwoRollover()
    {
        var stack = new RollingStack<int>(2);
        stack.Push(0);
        stack.Push(1);
        stack.Push(2);

        Assert.That(stack.Count, Is.EqualTo(2));
        Assert.That(stack[0], Is.EqualTo(2));
        Assert.That(stack[1], Is.EqualTo(1));
        Assert.That(stack.ToArray(), Is.EqualTo(new[] { 2, 1 }));
    }

    [Test]
    public void TestInterleavedPushPopAcrossRollover()
    {
        var stack = new RollingStack<int>(5);
        for (int i = 0; i < 7; i++)
        {
            stack.Push(i);
        }

        Assert.That(stack.Pop(), Is.EqualTo(6));
        Assert.That(stack.Pop(), Is.EqualTo(5));
        Assert.That(stack.Pop(), Is.EqualTo(4));

        stack.Push(7);
        stack.Push(8);
        stack.Push(9);

        Assert.That(stack.Count, Is.EqualTo(5));
        Assert.That(stack.ToArray(), Is.EqualTo(new[] { 9, 8, 7, 3, 2 }));
        Assert.That(stack[0], Is.EqualTo(9));
        Assert.That(stack[4], Is.EqualTo(2));
    }

    [Test]
    public void TestTryPeekAndTryPopWithReferenceType()
    {
        var stack = new RollingStack<string?>(2);
        stack.Push(null);

        Assert.That(stack.TryPeek(out var peekValue), Is.True);
        Assert.That(peekValue, Is.Null);

        Assert.That(stack.TryPop(out var popValue), Is.True);
        Assert.That(popValue, Is.Null);

        Assert.That(stack.TryPop(out _), Is.False);
    }

    [Test]
    public void TestNonGenericEnumeratorCurrentAndEnd()
    {
        var stack = new RollingStack<int>(2);

        var emptyEnumerator = ((IEnumerable)stack).GetEnumerator();
        Assert.That(emptyEnumerator.Current, Is.EqualTo(default(int)));
        Assert.That(emptyEnumerator.MoveNext(), Is.False);

        stack.Push(7);
        var enumerator = ((IEnumerable)stack).GetEnumerator();
        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(enumerator.Current, Is.EqualTo(7));
        Assert.That(enumerator.MoveNext(), Is.False);
    }
}