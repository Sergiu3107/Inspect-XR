using UnityEngine;
using System.IO;
using Unity.Netcode;
using UnityEditor;

namespace Paint
{

    public class TextureHandler : NetworkBehaviour, ITextureHandler
    {
        private RenderTexture _texture;
        private const int TextureSize = 1024;

        private PaintManager _paintManager;
        private LayerManager _layerManager;
        
        void Start()
        {
            
            _layerManager = LayerManager.Instance;
        }

        void OnDestroy()
        {
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner) return;

            var uiManager = FindObjectOfType<UIManager>();
            uiManager?.SetTextureHandler(this);

        }

        public void CreateTexture()
        {
            if (_texture == null)
            {
                _texture = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.ARGB32);
                _texture.Create();
            }
            else
            {
                ClearTexture();
            }
        }

        public void ClearTexture()
        {
            RenderTexture.active = _texture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
        }

        public (string path, Color color)? SaveTexture()
        {
            if (_texture == null) return null;

            Texture2D tex = new Texture2D(TextureSize, TextureSize, TextureFormat.ARGB32, false);
            RenderTexture.active = _texture;
            tex.ReadPixels(new Rect(0, 0, TextureSize, TextureSize), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            
            string path = Application.persistentDataPath + "/PaintedTexture_" + Random.Range(1000, 9999) + ".png";
            Color color = _paintManager.GetColor();
            
            File.WriteAllBytes(path, tex.EncodeToPNG());
            _layerManager.AddNewLayer(path, color);
            
            ClearTexture();
            Destroy(tex);
            
            return (path, color);
        }

        public void SetPaintManager(PaintManager paintManager)
        {
            _paintManager = paintManager;
        }

        public RenderTexture GetTexture() => _texture;
        
    }

}