using UnityEngine;
using UnityEditor;

public class UIAssetProcessor : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        if (assetPath.Contains("UI"))
        {
            TextureImporter e = (assetImporter as TextureImporter);

            e.textureType = TextureImporterType.Sprite;

            e.mipmapEnabled = false;
            e.alphaIsTransparency = true;
            e.wrapMode = TextureWrapMode.Repeat;

            AssetDatabase.Refresh();
        }
        
    }


   
}
