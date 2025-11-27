using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Paint
{
    [System.Serializable]
    public class LayerCollection
    {
        public List<LayerData> layers = new List<LayerData>();
        
        public Texture2DArray ConvertLayersToArray()
        {
            if (layers.Count == 0)
            {
                return null;
            }
            
            Texture2D firstTexture = LoadTexture(layers[0].texture);
            if (firstTexture == null)
            {
                Debug.LogWarning("No texture found!");
                return null;
            }

            int width = firstTexture.width;
            int height = firstTexture.height;
            Texture2DArray textureArray = new Texture2DArray(width, height, layers.Count, TextureFormat.ARGB32, false);

            for (int i = 0; i < layers.Count; i++)
            {
                Texture2D layerTexture = LoadTexture(layers[i].texture);
                if (layerTexture == null)
                {
                    Debug.LogWarning($"Failed to load texture at path: {layers[i].texture}");
                }
                else
                {
                    Debug.Log($"Loaded texture for layer {i}: {layers[i].texture}");
                }
                
                textureArray.SetPixels(layerTexture.GetPixels(), i);
            }
            
            textureArray.Apply();
            
            return textureArray;
        }
        
        private Texture2D LoadTexture(string path)
        {
            if (!File.Exists(path)) return null;
            byte[] imageData = File.ReadAllBytes(path);
            
            Texture2D texture = new Texture2D(1024, 1024, TextureFormat.ARGB32, false, false);
            texture.LoadImage(imageData, false);
            
            return texture;
        }
    }
    
    
}