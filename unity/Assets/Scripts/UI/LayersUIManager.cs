using System.Collections.Generic;
using Paint;
using UnityEngine;
using UnityEngine.UIElements;

public class LayersUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject layersUI;

    private UIDocument _layersUIDocument;
    private ListView _layersListView;
    private VisualElement _layersContainerList;
    private VisualElement _layersContainerInfo;
    private VisualElement _layersContainer;
    private TextField _layerInfoTextField;
    private Button _updateLayersButton;
    private Button _showLayersButton;
    private Button _clearLayerButton;
    private Button _layerSaveButton;
    private Button _layerDeleteButton;

    private AnnotationApi _annotationApi;
    private UIManager _uiManager;
    private PaintManager _paintManager;
    private ITextureHandler _textureHandler;

    private LayerData _currentSelectedLayer;

    public void Initialize(AnnotationApi annotationApi, UIManager uiManager)
    {
        _annotationApi = annotationApi;
        _uiManager = uiManager;
        SetupUI();
    }

    private void SetupUI()
    {
        _layersUIDocument = layersUI.GetComponent<UIDocument>();

        // Setup UI elements
        _layersContainer = _layersUIDocument.rootVisualElement.Q("container");
        _layersContainerList = _layersUIDocument.rootVisualElement.Q("container-list");
        _layersContainerInfo = _layersUIDocument.rootVisualElement.Q("container-info");
        _layersListView = _layersUIDocument.rootVisualElement.Q("annotation-list") as ListView;
        _updateLayersButton = _layersUIDocument.rootVisualElement.Q("update-layers") as Button;
        _layerInfoTextField = _layersUIDocument.rootVisualElement.Q("layer-info-text-field") as TextField;
        
        _showLayersButton = _layersUIDocument.rootVisualElement.Q("show-layers-button") as Button;
        _showLayersButton!.clicked += OnShowLayersClick;
        
        _clearLayerButton = _layersUIDocument.rootVisualElement.Q("layer-clear-button") as Button;
        _clearLayerButton!.clicked += OnClearLayerClick;
        
        _layerSaveButton = _layersUIDocument.rootVisualElement.Q("layer-save-button") as Button;
        _layerSaveButton!.clicked += OnLayerSaveClick;
        
        _layerDeleteButton = _layersUIDocument.rootVisualElement.Q("layer-delete-button") as Button;
        _layerDeleteButton!.clicked += OnLayerDeleteClick;
    }

    public void SetPaintManager(PaintManager paintManager)
    {
        _paintManager = paintManager;
    }

    public void SetTextureHandler(ITextureHandler textureHandler)
    {
        _textureHandler = textureHandler;
    }

    public void ToggleLayerUI(bool toggle)
    {
        _layersContainerList.visible = toggle;
        _layersContainerInfo.visible = toggle;
        _showLayersButton.visible = toggle;
        _clearLayerButton.visible = toggle;
    }

    private void OnClearLayerClick()
    {
        _textureHandler?.ClearTexture();
        _currentSelectedLayer = null;
        _layerInfoTextField.value = "";
        _layerInfoTextField.label = "New Annotation Layer";
    }

    private void OnLayerSaveClick()
    {
        string dataText = _layerInfoTextField.value;

        if (string.IsNullOrEmpty(dataText))
        {
            Debug.LogWarning("Data text is null or empty, cancelling layer save.");
            _textureHandler?.ClearTexture();
            return;
        }

        if (_currentSelectedLayer != null)
        {
            UpdateExistingLayer(dataText);
        }
        else
        {
            CreateNewLayer(dataText);
        }
    }

    private void UpdateExistingLayer(string dataText)
    {
        Color newColor = _paintManager.GetColor();
        Debug.Log("new color " + newColor);
        
        PaintAnnotationRequestDto request = new PaintAnnotationRequestDto
        {
            createdById = _currentSelectedLayer.createdById,
            modelId = _currentSelectedLayer.modelId,
            sessionId = _currentSelectedLayer.sessionId,
            data = dataText,
            imageLayer = _currentSelectedLayer.texture,
            color = ColorUtility.ToHtmlStringRGBA(newColor)
        };

        StartCoroutine(_annotationApi.UpdatePaintAnnotation(_currentSelectedLayer.id, request,
            onSuccess: (response) => { _uiManager.RefreshLayersForAllClientsServerRpc(); },
            onError: (error) => { Debug.LogError("Failed to update Paint Annotation: " + error); }));
    }

    private void CreateNewLayer(string dataText)
    {
        var textureHandlerConcrete = _textureHandler as TextureHandler;
        var savedData = textureHandlerConcrete?.SaveTexture();

        if (savedData == null)
        {
            Debug.LogError("Failed to save texture.");
            return;
        }

        PaintAnnotationRequestDto request = new PaintAnnotationRequestDto
        {
            createdById = LocalUserStorage.LoadUser().id,
            modelId = _uiManager.GetSelectedModelId(),
            sessionId = _uiManager.GetSelectedSessionId(),
            data = dataText,
            imageLayer = savedData.Value.path,
            color = ColorUtility.ToHtmlStringRGBA(savedData.Value.color)
        };

        StartCoroutine(_annotationApi.AddPaintAnnotation(request,
            onSuccess: (response) => { _uiManager.RefreshLayersForAllClientsServerRpc(); },
            onError: (error) => { Debug.LogError("Failed to add Paint Annotation: " + error); }));
    }

    private void OnLayerDeleteClick()
    {
        if (_currentSelectedLayer == null) return;

        StartCoroutine(_annotationApi.DeleteAnnotation(_currentSelectedLayer.id,
            onSuccess: () =>
            {
                var imagePath = _currentSelectedLayer.texture;
                if (!System.IO.File.Exists(imagePath)) return;

                System.IO.File.Delete(imagePath);
            
                _currentSelectedLayer = null;
                _layerInfoTextField.value = "";
                _layerInfoTextField.label = "New Annotation Layer";
            
                _uiManager.RefreshLayersForAllClientsServerRpc();
            },
            onError: (error) => { Debug.LogError("Failed to delete Paint Annotation: " + error); }));
    }

    private void OnShowLayersClick()
    {
        bool show = !_layersContainer.visible;

        _layersContainer.visible = show;
        _layersContainer.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        _layersContainer.pickingMode = show ? PickingMode.Position : PickingMode.Ignore;

        _showLayersButton.text = show ? "Hide Layers" : "Show Layers";
    }

    public void ListLayers(LayerCollection collection)
    {
        VisualTreeAsset entryVT = Resources.Load<VisualTreeAsset>("LayerEntryVT");

        _layersListView.makeItem = () =>
        {
            var item = entryVT.CloneTree();
            return item;
        };

        _layersListView.bindItem = (item, index) =>
        {
            if (index < 0 || index >= collection.layers.Count)
            {
                Debug.LogWarning($"Invalid index {index} for layers list of size {collection.layers.Count}");
                return;
            }

            var layer = collection.layers[index];
            item.userData = layer;

            var label = item.Q<Label>("name");
            label.text = $"Annotation Layer {layer.id} ";

            var slider = item.Q<Slider>("opacity");
            slider.value = layer.color.a;
            slider.userData = layer.color.a;
            slider.RegisterValueChangedCallback(evt => { slider.userData = evt.newValue; });

            var toggle = item.Q<Toggle>("focus");
            toggle.value = true;

            item.RegisterCallback<ClickEvent>(evt =>
            {
                var clickedLayer = item.userData as LayerData;
                if (clickedLayer == null) return;

                _currentSelectedLayer = clickedLayer;
                if (_layerInfoTextField != null)
                {
                    _layerInfoTextField.label = "Annotation Layer " + _currentSelectedLayer.id;
                    _layerInfoTextField.value = _currentSelectedLayer.data ?? "-- no infos --";
                }
            });
        };

        _updateLayersButton.RegisterCallback<ClickEvent>(_ =>
        {
            UpdateLayersOpacity(collection);
        });

        _layersListView.itemsSource = collection.layers;
        _layersListView.Rebuild();
        _paintManager?.PaintLayers(collection);
    }

    private void UpdateLayersOpacity(LayerCollection collection)
    {
        for (int i = 0; i < _layersListView.itemsSource.Count; i++)
        {
            if (i >= collection.layers.Count) continue;

            var layer = collection.layers[i];
            var item = _layersListView.GetRootElementForIndex(i);
            if (item == null) continue;

            var slider = item.Q<Slider>("opacity");
            var toggle = item.Q<Toggle>("focus");

            if (toggle != null && !toggle.value)
            {
                layer.color.a = 0f;
            }
            else if (slider != null && slider.userData is float newAlpha)
            {
                layer.color.a = newAlpha;
            }
        }

        _paintManager?.PaintLayers(collection);
    }

    public void FetchLayerAnnotations(long sessionId)
    {
        StartCoroutine(_annotationApi.GetPaintAnnotationsBySession(sessionId,
            onSuccess: (annotations) =>
            {
                LayerCollection collection = new LayerCollection
                {
                    layers = new List<LayerData>()
                };
        
                foreach (var paintAnnotation in annotations.annotations)
                {
                    Color color;
                    string colorString = "#" + paintAnnotation.color;
                    Debug.Log(colorString);
                    if (!ColorUtility.TryParseHtmlString(colorString, out color))
                        color = Color.white;

                    collection.layers.Add(new LayerData
                    {
                        id = paintAnnotation.id,
                        data = paintAnnotation.data,
                        texture = paintAnnotation.imageLayer,
                        color = color
                    });
                }
        
                if (collection.layers.Count <= 0)
                {
                    _paintManager?.PaintLayers(collection);
                    _currentSelectedLayer = null;
                    _layerInfoTextField.value = "";
                    _layerInfoTextField.label = "New Annotation Layer";
                
                    ToggleVisualElement(_layersContainerList, false);
                    ToggleVisualElement(_showLayersButton, false);
                    return;
                }
        
                ToggleVisualElement(_layersContainerList, true);
                ToggleVisualElement(_showLayersButton, true);
        
                ListLayers(collection);
            },
            onError: error => { Debug.LogError("Failed to fetch paint annotations: " + error); }));
    }

    private void ToggleVisualElement(VisualElement visualElement, bool toggle)
    {
        visualElement.visible = toggle;
        visualElement.style.display = toggle ? DisplayStyle.Flex : DisplayStyle.None;
        visualElement.pickingMode = toggle ? PickingMode.Position : PickingMode.Ignore;
    }
}