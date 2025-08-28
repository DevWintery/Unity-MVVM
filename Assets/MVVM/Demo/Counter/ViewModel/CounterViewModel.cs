using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wintery.MVVM;

public class CounterViewModel : BindableObject
{
    private int _count;
    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }

    public CounterViewModel()
    {
        IncrementCommand = new RelayCommand(_ => Count++);
        DecrementCommand = new RelayCommand(_ => Count--, () => Count > 0);
    }
}
