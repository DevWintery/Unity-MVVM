using System;
using Object = System.Object;

namespace Wintery.MVVM
{
    public class RelayCommand : ICommand
    {
        //�ش� Ŀ�ǵ带 ������ ������ ��� Predicate
        private readonly Func<bool> _canExecute;

        //������ �ݹ��Լ�
        private readonly Action<object> _execute;
        

        public event EventHandler CanExecuteChanged;

        // �� RelayCommand �ν��Ͻ��� ����
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
        /// ����� ���� ���� ���°� ����Ǿ����� �˸�
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
