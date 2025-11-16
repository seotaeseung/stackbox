using System;

public class MyStack<T>
{
    private class Node
    {
        public T Data;
        public Node Next;

        public Node(T data)
        {
            this.Data = data;
            this.Next = null;
        }
    }

    private Node top;
    private int cnt;
    public MyStack()
    {
        top = null;
        cnt = 0;
    }
    public void Push(T item)
    {
        Node newnode = new Node(item);
        newnode.Next = top;
        top = newnode;
        cnt++;
    }
    public T Pop()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Stack is empty!");
        }
        T data = top.Data;
        top = top.Next;
        cnt--;
        return data;

    }
    public bool IsEmpty()
    {
        return top == null;
    }

    // 개수 확인
    public int Count()
    {
        return cnt;
    }
}
