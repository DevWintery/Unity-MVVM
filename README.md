# Unity MVVM Framework (Inspired by WPF)

[English](./README.en.md)

Unity 프로젝트에서 **MVVM(Model-View-ViewModel) 패턴**을 손쉽게 적용할 수 있도록 제작한 프레임워크입니다.  
WPF의 `INotifyPropertyChanged`, `ICommand` 패턴을 참고하여, Unity 환경에서도 **데이터 바인딩과 명령 실행을 직관적으로 관리**할 수 있습니다.

---

## ✨ Features
- **BindableObject**
  - WPF의 `INotifyPropertyChanged` 구현
  - `SetProperty`로 UI와 데이터 자동 동기화

- **RelayCommand**
  - `ICommand` 기반 실행/검증 패턴 지원
  - Unity의 `Button.onClick` 등과 자연스럽게 연결 가능

- **ViewBehaviour**
  - Unity의 `MonoBehaviour`를 확장
  - ViewModel과 View를 바인딩할 수 있는 기본 클래스 제공
  - `Bind<T>()` : ViewModel 프로퍼티와 UI 컴포넌트 연동
  - `BindCommand()` : ICommand를 UnityEvent에 연결

---

## 📦 Installation
1. `Wintery.MVVM` 폴더를 Unity 프로젝트에 추가
2. `using Wintery.MVVM;` 선언 후 사용 가능

---

## 🚀 Demo
아래는 숫자 증가/감소를 MVVM으로 구현한 간단한 예제입니다.

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
