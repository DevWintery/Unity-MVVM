# Unity MVVM Framework (Inspired by WPF)

[🇰🇷 한국어 버전 보기](./README.ko.md)

*Written by ChatGPT*

Unity MVVM framework inspired by **WPF's INotifyPropertyChanged** and **ICommand**.  
This library allows you to implement the **MVVM pattern** in Unity for cleaner separation of concerns between UI (View) and logic (ViewModel).

---

## ✨ Features
- **BindableObject**
  - Implements `INotifyPropertyChanged`
  - Automatically syncs UI and data with `SetProperty`

- **RelayCommand**
  - WPF-style `ICommand` implementation
  - Supports `Execute` and `CanExecute`

- **ViewBehaviour**
  - Extends `MonoBehaviour`
  - Provides data binding and command binding
  - `Bind<T>()`: Connect ViewModel properties with UI updates  
  - `BindCommand()`: Bind `ICommand` with UnityEvents

---

## 🚀 Demo: Counter Example

### ViewModel
```csharp
public class CounterViewModel : BindableObject
{
    private int _count;
    public int Count { get => _count; set => SetProperty(ref _count, value); }

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }

    public CounterViewModel()
    {
        IncrementCommand = new RelayCommand(_ => Count++);
        DecrementCommand = new RelayCommand(_ => Count--, () => Count > 0);
    }
}