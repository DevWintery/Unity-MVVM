using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/*
 * INotifyPropertyChanged 인터페이스에 대해
 * https://learn.microsoft.com/ko-kr/dotnet/api/system.componentmodel.inotifypropertychanged?view=net-9.0
 * 참고
 */

namespace Wintery.MVVM
{
    /// <summary>
    /// 프로퍼티 변경 알림을 제공하는 기본 클래스
    /// </summary>
    public class BindableObject : INotifyPropertyChanged
    {
        //프로퍼티 변경 시 발생하는 이벤트
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 프로퍼티 값을 설정하고, 변경 시 PropertyChanged 이벤트를 발생시킴.
        /// </summary>
        /// <param name="propertyName">프로퍼티의 이름 CallerMemeberName으로 설정되어있기때문에 문자열 인수를 제공할 필요가 없음</param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            //값이 동일하면 변경하지않음
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            //값 변경 및 이벤트 호출
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// PropertyChanged 이벤트를 발생시키는 함수
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 여러 프로퍼티의 PropertyChanged 이벤트를 발생시키는 함수
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
