namespace Paint
{
    using UnityEngine;

    public class PaintManager
    {
        private Material _drawMaterial;
        private Material _baseMaterial;
        private Material _paintMaterial;
        private Material _layerMaterial;
        private RenderTexture _texture;
        private float _strength;
        private float _size;
        private Color _color;
        private Camera _camera;

        private int _maxLayerArraySize = 0;
        private const int DEFAULT_MAX_LAYERS = 10;

        public PaintManager(Material drawMaterial, Material baseMaterial, Material paintMaterial,
            Material layerMaterial, RenderTexture texture, Camera camera, float strength, float size, Color color)
        {
            _drawMaterial = drawMaterial;
            _baseMaterial = baseMaterial;
            _paintMaterial = paintMaterial;
            _layerMaterial = layerMaterial;
            _texture = texture;
            _camera = camera;
            _strength = strength;
            _size = size;
            _color = color;

            // Initialize with default array size
            InitializeLayerArrays();
        }

        private void InitializeLayerArrays()
        {
            // Initialize the shader arrays with a default size
            var defaultColors = new Vector4[DEFAULT_MAX_LAYERS];
            for (int i = 0; i < DEFAULT_MAX_LAYERS; i++)
            {
                defaultColors[i] = Vector4.zero;
            }

            _layerMaterial.SetVectorArray("_LayerColors", defaultColors);
            _maxLayerArraySize = DEFAULT_MAX_LAYERS;
        }

        public void Paint()
        {
            if (!Input.GetMouseButton(0)) return;

            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                _drawMaterial.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                _drawMaterial.SetFloat("_Strength", _strength);
                _drawMaterial.SetFloat("_Size", _size);
                _drawMaterial.SetColor("_DrawColor", Color.white);

                RenderTexture temp =
                    RenderTexture.GetTemporary(_texture.width, _texture.height, 0, RenderTextureFormat.ARGB32);
                Graphics.Blit(_texture, temp);
                Graphics.Blit(temp, _texture, _drawMaterial);
                RenderTexture.ReleaseTemporary(temp);

            }
        }

        public void PaintLayers(LayerCollection layerCollection)
        {
    
            if (layerCollection == null || layerCollection.layers.Count <= 0) 
            {
                ClearLayers();
                return;
            }


            Texture mainTex = _baseMaterial.GetTexture("baseColorTexture");
            var layers = layerCollection.layers;
            var numOfLayers = layers.Count;
            var layersTexture2DArray = layerCollection.ConvertLayersToArray();

            int actualLayerCount = Mathf.Min(numOfLayers, _maxLayerArraySize);

            var layerColors = new Vector4[_maxLayerArraySize];

            for (int i = 0; i < actualLayerCount; i++)
            {
                Color c = layers[i].color;
                layerColors[i] = new Vector4(c.r, c.g, c.b, c.a);
            }

            for (int i = actualLayerCount; i < _maxLayerArraySize; i++)
            {
                layerColors[i] = Vector4.zero;
            }

            _layerMaterial.SetTexture("_MainTex", mainTex);
            _layerMaterial.SetTexture("_Textures", layersTexture2DArray);
            _layerMaterial.SetInteger("_Count", actualLayerCount);
            _layerMaterial.SetVectorArray("_LayerColors", layerColors);

            if (numOfLayers > _maxLayerArraySize)
            {
                Debug.LogWarning(
                    $"Layer count ({numOfLayers}) exceeds maximum supported layers ({_maxLayerArraySize}). Only first {_maxLayerArraySize} layers will be rendered.");
            }
        }
        
        public void ClearLayers()
        {
            _layerMaterial.SetInteger("_Count", 0);
            _layerMaterial.SetTexture("_Textures", null);
    
            var emptyColors = new Vector4[_maxLayerArraySize];
            for (int i = 0; i < _maxLayerArraySize; i++)
            {
                emptyColors[i] = Vector4.zero;
            }
            _layerMaterial.SetVectorArray("_LayerColors", emptyColors);
    
            Texture mainTex = _baseMaterial.GetTexture("baseColorTexture");
            _layerMaterial.SetTexture("_MainTex", mainTex);
        }


        public void SetColor(Color color)
        {
            _color = color;
        }

        public Color GetColor()
        {
            return _color;
        }

        public Material GetBaseMaterial()
        {
            return _baseMaterial;
        }

        public Camera GetCamera()
        {
            return _camera;
        }
    }
}