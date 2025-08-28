using System;

namespace Wintery.MVVM
{
    public interface ICommand
    {
        //����� ���� ���� �������� Ȯ��
        bool CanExecute();

        //����� ������
        void Execute(object parameter = null);

        //����� ���� ���� ���°� ����� �� �߻��ϴ� �̺�Ʈ
        event EventHandler CanExecuteChanged;
    }
}
