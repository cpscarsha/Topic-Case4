using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularBuffer<T>{
    T[] buffer;
    int buffer_size;

    public CircularBuffer(int buffer_size){
        this.buffer_size = buffer_size;
        buffer = new T[buffer_size];
    }

    public void Add(T item, int index) => buffer[index % buffer_size] = item;
    public T Get(int index) => buffer[index % buffer_size];
    public void Clear() => buffer = new T[buffer_size];
}
