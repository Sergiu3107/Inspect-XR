using Paint;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

enum AnnotationMode
{
    Point,
    Paint
}

public class AnnotationService : MonoBehaviour
{
    [SerializeField] private PaintAnnotation _paintAnnotation;
    [SerializeField] private PointAnnotation _pointAnnotation;

    // [SerializeField] private GameObject _paintUICanvas;
    [SerializeField] private GameObject _colorPickerCanvas;

    private AnnotationMode _mode = AnnotationMode.Paint;

    [SerializeField] private GameObject _player;
    private Movement _movement;
    private Looking _looking;

    private bool _isCursorVisible = false;

    void Start()
    {
        _movement = _player.GetComponent<Movement>();
        _looking = _player.GetComponent<Looking>();
    }

    void Update()
    {
        ChangeMode();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePointer();
        }

        if (IsMouseOverUI())
        {
            return;
        }

        switch (_mode)
        {
            case AnnotationMode.Point:
                ToggleCanvas(false);
                _pointAnnotation.Add();
                break;
            case AnnotationMode.Paint:
                ToggleCanvas(true);
                _paintAnnotation.Add();
                break;
        }
    }

    private void ChangeMode()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _mode = AnnotationMode.Point;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _mode = AnnotationMode.Paint;
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void TogglePointer()
    {
        _isCursorVisible = !_isCursorVisible;

        Cursor.visible = _isCursorVisible;
        Cursor.lockState = _isCursorVisible ? CursorLockMode.None : CursorLockMode.Locked;

        _movement.enabled = !_isCursorVisible;
        _looking.enabled = !_isCursorVisible;
    }

    private void ToggleCanvas(bool value)
    {
        _colorPickerCanvas.SetActive(value);
        UIManager.Instance.ToggleLayerUI(value);
    }
}