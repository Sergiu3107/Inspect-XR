using Paint;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.UI;

namespace Paint
{
    public class PaintAnnotation : MonoBehaviour, IAnnotationLogic
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Shader _drawShader;
        [SerializeField] private FlexibleColorPicker _flexibleColorPicker;
        [SerializeField] private TextureHandler _textureHandler;
        
        private bool _isPainting;
        private PaintManager _paintManager;
        
        private RenderTexture _texture;
        private Material _drawMaterial;

        [Header("Painting Settings")] [SerializeField] [Range(0, 1)]
        private float _strength;

        [SerializeField] [Range(0, 500)] private float _size;
        [SerializeField] private Color _color = new Color(0, 1, 0, 0.4f);
        
        private Material _baseMaterial;
        private Material _layerMaterial;
        private Material _paintMaterial;
        void Start()
        {
            _drawMaterial = new Material(_drawShader);

            UploadService.Instance.onModelLoaded.AddListener(AssignMaterials);
        }

        void OnDestroy()
        {
            UploadService.Instance.onModelLoaded.RemoveListener(AssignMaterials);
        }

        void Update()
        {
            if (UIManager.Instance != null && _flexibleColorPicker != null)
            {
                _flexibleColorPicker.gameObject.SetActive(!UIManager.Instance.IsZoomOn());
            }

        }

        public void Add()
        {
            _color = _flexibleColorPicker.color;
            _paintManager?.SetColor(_color);
            
            if (Input.GetMouseButtonDown(0)) _isPainting = true;
            if (Input.GetMouseButtonUp(0)) _isPainting = false;

            if (_isPainting) _paintManager?.Paint();
        }

        private void AssignMaterials(GameObject gameObject)
        {
            Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
            _baseMaterial = materials[0];
            _layerMaterial = materials[1];
            _paintMaterial = materials[2];
            
            _textureHandler.CreateTexture();
            RenderTexture texture = _textureHandler.GetTexture();
            _paintMaterial.SetTexture("_SplatTex", texture);
            
            _paintManager = new PaintManager(_drawMaterial, _baseMaterial, _paintMaterial, _layerMaterial, texture, _camera, _strength, _size, _color);
            _textureHandler.SetPaintManager(_paintManager);
            UIManager.Instance.SetPaintManager(_paintManager);
        }
    }
    
    
}