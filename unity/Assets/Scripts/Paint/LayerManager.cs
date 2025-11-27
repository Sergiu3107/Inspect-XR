using UnityEditor;
using UnityEngine.Events;

namespace Paint
{
    using UnityEngine;
    using System.IO;
    using System.Collections.Generic;

    public class LayerManager
    {
        public UnityEvent<LayerCollection> OnLayerUpdated = new UnityEvent<LayerCollection>();
        
        private string jsonPath;
        private LayerCollection _layerCollection;
        
        private static LayerManager _instance;

        public static LayerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerManager();
                }

                return _instance;
            }
        }

        public LayerManager()
        {
            jsonPath = Application.persistentDataPath + "/LayersData.json";
        }

        public LayerCollection LoadExistingLayers()
        {
            if (File.Exists(jsonPath))
            {
                string existingJson = File.ReadAllText(jsonPath);
                return JsonUtility.FromJson<LayerCollection>(existingJson);
            }

            return new LayerCollection();
        }

        public void AddNewLayer(string texturePath, Color color)
        {
            _layerCollection = LoadExistingLayers();
            
            LayerData newLayer = new LayerData
            {
                texture = texturePath,
                color = color,
            };
                
            _layerCollection.layers.Add(newLayer);
            
            OnLayerUpdated?.Invoke(_layerCollection);
        }
    }
}