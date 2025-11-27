using UnityEngine;
using UnityEngine.UI;
using Paint;
using TMPro;

public class ZoomUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject texturesCanvas;

    // Zoom state
    private bool _isZoomOn = false;
    private Material _baseZoomMat;
    private Material _normalZoomMat;
    private Material _occlusionZoomMat;
    private float _zoomScale = 5f;

    // Dependencies
    private PaintManager _paintManager;

    public void Initialize()
    {
    }

    public void SetPaintManager(PaintManager paintManager)
    {
        _paintManager = paintManager;
        InitializeZoomMaterials();
    }

    public void HandleZoomInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _isZoomOn = !_isZoomOn;
            texturesCanvas.SetActive(_isZoomOn);
        }

        if (_isZoomOn)
        {
            float scrollDelta = Input.mouseScrollDelta.y;

            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                _zoomScale = Mathf.Clamp(_zoomScale + scrollDelta, 1f, 50f);
            }

            UpdateZoom();
        }
    }

    public bool IsZoomOn()
    {
        return _isZoomOn;
    }

    private void InitializeZoomMaterials()
    {
        if (_paintManager == null)
        {
            Debug.LogWarning("Initializing zoom materials: No paint manager");
            return;
        }

        texturesCanvas.SetActive(true);

        Texture texture1 = _paintManager.GetBaseMaterial().GetTexture("baseColorTexture");
        Texture texture2 = _paintManager.GetBaseMaterial().GetTexture("normalTexture");
        Texture texture3 = _paintManager.GetBaseMaterial().GetTexture("occlusionTexture");

        RawImage baseImage = texturesCanvas.transform.GetChild(0).GetComponent<RawImage>();
        RawImage normalImage = texturesCanvas.transform.GetChild(1).GetComponent<RawImage>();
        RawImage occlusionImage = texturesCanvas.transform.GetChild(2).GetComponent<RawImage>();
        
        TMP_Text baseImageText = texturesCanvas.transform.GetChild(3).GetComponent<TMP_Text>();
        TMP_Text normalImageText = texturesCanvas.transform.GetChild(4).GetComponent<TMP_Text>();
        TMP_Text occlusionImageText = texturesCanvas.transform.GetChild(5).GetComponent<TMP_Text>();

        _baseZoomMat = new Material(Shader.Find("Unlit/ZoomShader"));
        _normalZoomMat = new Material(Shader.Find("Unlit/ZoomShader"));
        _occlusionZoomMat = new Material(Shader.Find("Unlit/ZoomShader"));

        _baseZoomMat.SetTexture("_MainTex", texture1);
        _normalZoomMat.SetTexture("_MainTex", texture2);
        _occlusionZoomMat.SetTexture("_MainTex", texture3);
        
        baseImageText.text = texture1?.name ?? "Base Color";
        normalImageText.text = texture2?.name ?? "Normal";
        occlusionImageText.text = texture3?.name ?? "Occlusion";

        baseImage.material = _baseZoomMat;
        normalImage.material = _normalZoomMat;
        occlusionImage.material = _occlusionZoomMat;

        texturesCanvas.SetActive(false);
    }

    private void UpdateZoom()
    {
        if (_paintManager == null || !_paintManager.GetCamera()) return;

        Camera cam = _paintManager.GetCamera();
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            Vector2 uv = hit.textureCoord;

            SetZoomMaterial(_baseZoomMat, uv, _zoomScale);
            SetZoomMaterial(_normalZoomMat, uv, _zoomScale);
            SetZoomMaterial(_occlusionZoomMat, uv, _zoomScale);
        }
    }

    private void SetZoomMaterial(Material mat, Vector2 uv, float scale)
    {
        if (mat == null) return;

        mat.SetVector("_Coordinates", uv);
        mat.SetFloat("_Scale", scale);
    }
}