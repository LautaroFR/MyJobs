using UnityEngine.UI;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using TMPro;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using TooltipAttribute = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using BehaviorDesigner.Runtime;

[TaskCategory("ISVR.UI")]
public class CheckUIValueChangeAction : Action
{
    public SharedGameObject targetGameObject;

    [SerializeField]
    private UiType uiType;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset uiElement;
#endif

    private bool _hasValueChanged;
    private bool _canCompleteTask = true;

    private GameObject _currentGameObject;

    public override void OnStart()
    {
        _hasValueChanged = false;

        _currentGameObject = targetGameObject.Value;

        if (_currentGameObject == null)
        {
            return;
        }

        switch (uiType)
        {
            case UiType.Toggle:
                _currentGameObject.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChange);
                break;
            case UiType.Dropdown:
                _currentGameObject.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(OnDropdownValueChange);
                break;
            case UiType.Scrollbar:
                _currentGameObject.GetComponent<Scrollbar>().onValueChanged.AddListener(OnScrollbarValueChange);
                break;
        }
    }

    private void OnToggleValueChange(bool newValue)
    {
        _hasValueChanged = true;
    }

    private void OnDropdownValueChange(int newIndex)
    {
        _hasValueChanged = true;
    }

    private void OnScrollbarValueChange(float newValue)
    {
        _hasValueChanged = true;
    }

    public override TaskStatus OnUpdate()
    {
        if (_currentGameObject == null)
        {
            Debug.Log($"[{GetType()}] Not assigned or incorrect type");
            return TaskStatus.Failure;
        }

        switch (uiType)
        {
            case UiType.Toggle:
                if (_currentGameObject.GetComponent<Toggle>() == null)
                    _canCompleteTask = false;
                break;
            case UiType.Dropdown:
                if (_currentGameObject.GetComponent<TMP_Dropdown>() == null)
                    _canCompleteTask = false;
                break;
            case UiType.Scrollbar:
                if (_currentGameObject.GetComponent<Scrollbar>() == null)
                    _canCompleteTask = false;
                break;
            default:
                break;
        }

        if (!_canCompleteTask)
        {
            Debug.Log($"[{GetType()}]  Not assigned or incorrect type");
            return TaskStatus.Failure;
        }

        if (_hasValueChanged)
        {
            RemoveListener();
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }

    private void RemoveListener()
    {
        switch (uiType)
        {
            case UiType.Toggle:
                _currentGameObject.GetComponent<Toggle>().onValueChanged.RemoveListener(OnToggleValueChange);
                break;
            case UiType.Dropdown:
                _currentGameObject.GetComponent<TMP_Dropdown>().onValueChanged.RemoveListener(OnDropdownValueChange);
                break;
            case UiType.Scrollbar:
                _currentGameObject.GetComponent<Scrollbar>().onValueChanged.RemoveListener(OnScrollbarValueChange);
                break;
        }
    }
}

public enum UiType
{
    Toggle,
    Dropdown,
    Scrollbar
}