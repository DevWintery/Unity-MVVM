using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/*
 * INotifyPropertyChanged �������̽��� ����
 * https://learn.microsoft.com/ko-kr/dotnet/api/system.componentmodel.inotifypropertychanged?view=net-9.0
 * ����
 */

namespace Wintery.MVVM
{
    /// <summary>
    /// ������Ƽ ���� �˸��� �����ϴ� �⺻ Ŭ����
    /// </summary>
    public class BindableObject : INotifyPropertyChanged
    {
        //������Ƽ ���� �� �߻��ϴ� �̺�Ʈ
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// ������Ƽ ���� �����ϰ�, ���� �� PropertyChanged �̺�Ʈ�� �߻���Ŵ.
        /// </summary>
        /// <param name="propertyName">������Ƽ�� �̸� CallerMemeberName���� �����Ǿ��ֱ⶧���� ���ڿ� �μ��� ������ �ʿ䰡 ����</param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            //���� �����ϸ� ������������
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            //�� ���� �� �̺�Ʈ ȣ��
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// PropertyChanged �̺�Ʈ�� �߻���Ű�� �Լ�
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// ���� ������Ƽ�� PropertyChanged �̺�Ʈ�� �߻���Ű�� �Լ�
        /// </summary>
        protected void OnPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (var name in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }

}
