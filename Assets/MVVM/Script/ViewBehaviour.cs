using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Wintery.MVVM
{
    /// <summary>
    /// MVVM ���Ͽ��� View�� �����ϱ����� MonoBehaviour Ȯ�� Ŭ����
    /// </summary>
    public class ViewBehaviour : MonoBehaviour
    {
        //���ε��� �Ӽ��� ������Ʈ �׼� ����
        protected Dictionary<string, List<Action<object>>> _bindings = new();

        //���� ���ε� �� ViewModel
        protected BindableObject _viewModel = null;

        /// <summary>
        /// ViewModel�� �����ϰ� ���ε��ϴ� �Լ�
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
                UpdateAllBindings(); //�ʱ� UI ���� ������Ʈ
            }
        }

        /// <summary>
        /// ViewModel�� PropertyChanged �̺�Ʈ �ڵ鷯 �Լ�
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
        /// ��� ���ε��� ���� ViewModel�� ������ ������Ʈ�ϴ� �Լ�
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
        /// ViewModel�� ������Ƽ�� ���ε��� �ϴ� �Լ�
        /// </summary>
        /// <param name="updateAction">���� ��������� �����ϴ� �ݹ��Լ�</param>
        protected void Bind<T>(string propertyName, Action<T> updateAction)
        {
            //���� ���ε��� ��Ͽ� ������Ƽ�� ���ٸ� ���ο� List�� ����
            if (_bindings.TryGetValue(propertyName, out var actions) == false)
            {
                actions = new List<Action<object>>();
                _bindings[propertyName] = actions;
            }

            // ���� �ݹ鿡 ���ο� ������Ʈ �Լ��� �߰�
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

            // 1) ������ ���� �������������� �������̸� �⺻������ ������Ʈ
            if (value == null && typeof(T).IsValueType)
            {
                updateAction(default);
            }
            // 2) ������ ���� Ÿ���� �����ϴٸ� ������Ʈ
            else if (value is T typedValue)
            {
                updateAction(typedValue);
            }
            // 3) Ÿ���� �ٸ����� �Ͻ��� ��ȯ�� ������ ��찡 �����Ƿ� Ÿ�Ժ�ȯ�� �õ�
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
        /// ViewModel�� Ŀ�ǵ忡 UnityEvent�� ���ε��ϴ� �Լ�
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

            // UnityEvent�� ��� ���� ������ �߰�
            unityEvent.AddListener(() => command.Execute(commandParameter));
        }

        /// <summary>
        /// ViewModel�� Ŀ�ǵ忡 �Ű������� �����ϴ� UnityEvent�� ���ε��ϴ� �Լ�
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

            // UnityEvent<T>�� ��� ���� ������ �߰�
            unityEvent.AddListener((param) =>
            {
                object commandParam = parameterProvider != null ? parameterProvider(param) : param;
                command.Execute(commandParam);
            });
        }

        /// <summary>
        /// GameObject�� �ı��� �� ȣ��Ǹ�, �̺�Ʈ ������ ����
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
