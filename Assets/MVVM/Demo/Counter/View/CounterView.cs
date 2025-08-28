using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wintery.MVVM;

public class CounterView : ViewBehaviour
{
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Button incrementButton;
    [SerializeField] private Button decrementButton;

    private void Awake()
    {
        var vm = new CounterViewModel();
        SetViewModel(vm);

        Bind<int>(nameof(vm.Count), value => countText.text = $"Count : {value}");

        BindCommand(nameof(vm.IncrementCommand), incrementButton.onClick);
        BindCommand(nameof(vm.DecrementCommand), decrementButton.onClick);
    }
}
