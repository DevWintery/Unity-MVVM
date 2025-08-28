using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Wintery.MVVM
{
    /// <summary>
    /// MVVM 패턴에서 View를 구현하기위한 MonoBehaviour 확장 클래스
    /// </summary>
    public class ViewBehaviour : MonoBehaviour
    {
        //바인딩된 속성과 업데이트 액션 매핑
        protected Dictionary<string, List<Action<object>>> _bindings = new();

        //현재 바인딩 된 ViewModel
        protected BindableObject _viewModel = null;

        /// <summary>
        /// ViewModel을 설정하고 바인딩하는 함수
        /// </summary>
        protected void SetViewModel(BindableObject viewModel)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            _viewModel = viewModel;

            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += OnViewModelPropertyChanged;
                UpdateAllBindings(); //초기 UI 상태 업데이트
            }
        }

        /// <summary>
        /// ViewModel의 PropertyChanged 이벤트 핸들러 함수
        /// </summary>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_bindings.TryGetValue(e.PropertyName, out var actions))
            {
                PropertyInfo property = _viewModel.GetType().GetProperty(e.PropertyName);
                if (property != null)
                {
                    object value = property.GetValue(_viewModel);
                    foreach (var action in actions)
                    {
                        action(value);
                    }
                }
            }
        }


        /// <summary>
        /// 모든 바인딩을 현재 ViewModel의 값으로 업데이트하는 함수
        /// </summary>
        private void UpdateAllBindings()
        {
            if (_viewModel == null)
            {
                return;
            }

            foreach (var binding in _bindings)
            {
                PropertyInfo property = _viewModel.GetType().GetProperty(binding.Key);
                if (property != null)
                {
                    object value = property.GetValue(_viewModel);
                    foreach (var action in binding.Value)
                    {
                        action(value);
                    }
                }
            }
        }

        /// <summary>
        /// ViewModel의 프로퍼티에 바인딩을 하는 함수
        /// </summary>
        /// <param name="updateAction">값이 변경됐을때 실행하는 콜백함수</param>
        protected void Bind<T>(string propertyName, Action<T> updateAction)
        {
            //현재 바인딩된 목록에 프로퍼티가 없다면 새로운 List를 생성
            if (_bindings.TryGetValue(propertyName, out var actions) == false)
            {
                actions = new List<Action<object>>();
                _bindings[propertyName] = actions;
            }

            // 기존 콜백에 새로운 업데이트 함수를 추가
            actions.Add((value) =>
            {
                if (value == null && typeof(T).IsValueType)
                {
                    updateAction(default);
                    return;
                }

                try
                {
                    if (value is T typedValue)
                    {
                        updateAction(typedValue);
                    }

                    else
                    {
                        updateAction((T)Convert.ChangeType(value, typeof(T)));
                    }
                }

                catch (Exception ex)
                {
                    Debug.LogError($"Failed to convert value {value} to type {typeof(T).Name} for property {propertyName}: {ex.Message}");
                }
            });

            if (_viewModel == null)
            {
                return;
            }

            PropertyInfo property = _viewModel.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return;
            }

            object value = property.GetValue(_viewModel);

            // 1) 변경할 값이 존재하지않으나 값형식이면 기본값으로 업데이트
            if (value == null && typeof(T).IsValueType)
            {
                updateAction(default);
            }
            // 2) 변경할 값이 타입이 완전하다면 업데이트
            else if (value is T typedValue)
            {
                updateAction(typedValue);
            }
            // 3) 타입이 다르더라도 암시적 변환이 가능할 경우가 있으므로 타입변환을 시도
            else if (value != null)
            {
                try
                {
                    updateAction((T)Convert.ChangeType(value, typeof(T)));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Initial binding failed: Cannot convert {value.GetType().Name} to {typeof(T).Name} for property {propertyName}: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Property {propertyName} not found in ViewModel {_viewModel.GetType().Name}");
            }
        }

        /// <summary>
        /// ViewModel의 커맨드에 UnityEvent를 바인딩하는 함수
        /// </summary>
        protected void BindCommand(string commandName, UnityEvent unityEvent, object commandParameter = null)
        {
            if (_viewModel == null)
            {
                Debug.LogWarning($"Cannot bind command {commandName}: ViewModel is null");
                return;
            }

            PropertyInfo commandProperty = _viewModel.GetType().GetProperty(commandName);
            if (commandProperty == null)
            {
                Debug.LogWarning($"Command property {commandName} not found in ViewModel {_viewModel.GetType().Name}");
                return;
            }

            if (!typeof(ICommand).IsAssignableFrom(commandProperty.PropertyType))
            {
                Debug.LogError($"Property {commandName} is not an ICommand in ViewModel {_viewModel.GetType().Name}");
                return;
            }

            ICommand command = (ICommand)commandProperty.GetValue(_viewModel);
            if (command == null)
            {
                Debug.LogWarning($"Command {commandName} is null in ViewModel {_viewModel.GetType().Name}");
                return;
            }

            // UnityEvent에 명령 실행 리스너 추가
            unityEvent.AddListener(() => command.Execute(commandParameter));
        }

        /// <summary>
        /// ViewModel의 커맨드에 매개변수를 전달하는 UnityEvent를 바인딩하는 함수
        /// </summary>
        protected void BindCommand<T>(string commandName, UnityEvent<T> unityEvent, Func<T, object> parameterProvider = null)
        {
            if (_viewModel == null)
            {
                Debug.LogWarning($"Cannot bind command {commandName}: ViewModel is null");
                return;
            }

            PropertyInfo commandProperty = _viewModel.GetType().GetProperty(commandName);
            if (commandProperty == null)
            {
                Debug.LogWarning($"Command property {commandName} not found in ViewModel {_viewModel.GetType().Name}");
                return;
            }

            if (!typeof(ICommand).IsAssignableFrom(commandProperty.PropertyType))
            {
                Debug.LogError($"Property {commandName} is not an ICommand in ViewModel {_viewModel.GetType().Name}");
                return;
            }

            ICommand command = (ICommand)commandProperty.GetValue(_viewModel);
            if (command == null)
            {
                Debug.LogWarning($"Command {commandName} is null in ViewModel {_viewModel.GetType().Name}");
                return;
            }

            // UnityEvent<T>에 명령 실행 리스너 추가
            unityEvent.AddListener((param) =>
            {
                object commandParam = parameterProvider != null ? parameterProvider(param) : param;
                command.Execute(commandParam);
            });
        }

        /// <summary>
        /// GameObject가 파괴될 때 호출되며, 이벤트 구독을 해제
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_viewModel == null)
            {
                return;
            }

            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }
}
