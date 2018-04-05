﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Invaders
{
    class CircleQueue
    {
        private int count;
        public int Count { get { return count; } private set { } }

        private int front; // pop 되는 위치
        public int Front { get { return front; } }

        private int rear; // push 되는 위치
        public int Rear { get { return rear; } }

        private int size;
        public int Size { get { return size; } }
        private Shot[] arr;

        public CircleQueue(int size)
        {
            this.size = size;
            front = 0;
            rear = front;
            arr = new Shot[size + 1];
        }

        public bool Insert(Shot shot)
        {
            arr[rear++] = shot;
            Console.WriteLine("Insert");
            return true;
        }

        public void Reset() { rear = front; }

        public Shot GetData(int num)
        {
            Shot shot = arr[num];
            return shot;
        }

        public bool Push(Point location)
        {
            if (IsFull())
            {
                Console.WriteLine("full 입니다.");
                front %= size;
                rear %= size;
                return false;
            }

            arr[rear++].ActivateShot(location);
            rear %= size;
            count++;
            Console.WriteLine("Push");
            return true;
        }

        public bool Pop()
        {
            if (IsEmpty())
            {
                Console.WriteLine("empty");
                front %= size;
                rear %= size;
                return false;
            }

            front++; count--;
            front %= size;
            Console.WriteLine("Pop");
            return true;
        }
         
        public bool IsFull() // 큐가 꽉 찼는지 확인
        {
            if (front < rear) return (rear - front) == size - 1;
            return rear + 1 == front;
        }

        public bool IsEmpty() // 큐가 비었는지 확인
        {
            if (front == rear) return true;
            return false;
        }

        public bool IsNull(int i) // 해당 위치의 데이터가 존재하는지 확인
        {
            i %= size;
            if (front < rear) return !(front <= i && i < rear);
            else if (rear < front) return !(front <= i || i < rear);
            return true;
        }
    }
}
