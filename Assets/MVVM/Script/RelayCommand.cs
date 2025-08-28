using System;
using Object = System.Object;

namespace Wintery.MVVM
{
    public class RelayCommand : ICommand
    {
        //해당 커맨드를 실행할 조건을 담는 Predicate
        private readonly Func<bool> _canExecute;

        //실행할 콜백함수
        private readonly Action<object> _execute;
        

        public event EventHandler CanExecuteChanged;

        // 새 RelayCommand 인스턴스를 생성
        public RelayCommand(Action<Object> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentException(nameof(execute));
            _canExecute = canExecute;
        }
        public bool CanExecute() => _canExecute == null || _canExecute();

        public void Execute(object parameter = null)
        {
            if (CanExecute())
            {
                _execute(parameter);
            }
        }

        /// <summary>
        /// 명령의 실행 가능 상태가 변경되었음을 알림
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
