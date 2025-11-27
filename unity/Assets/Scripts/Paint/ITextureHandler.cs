using UnityEngine;

namespace Paint
{
    public interface ITextureHandler
    {
        void CreateTexture();
        void ClearTexture();
        (string path, Color color)? SaveTexture();
    }

}