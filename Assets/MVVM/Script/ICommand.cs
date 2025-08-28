using System;

namespace Wintery.MVVM
{
    public interface ICommand
    {
        //명령이 현재 실행 가능한지 확인
        bool CanExecute();

        //명령을 실행함
        void Execute(object parameter = null);

        //명령의 실행 가능 상태가 변경될 때 발생하는 이벤트
        event EventHandler CanExecuteChanged;
    }
}
