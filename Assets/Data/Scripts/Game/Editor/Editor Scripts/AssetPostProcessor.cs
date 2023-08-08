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

            //e.mipmapEnabled = false;
            e.alphaIsTransparency = true;
            e.wrapMode = TextureWrapMode.Clamp;

            AssetDatabase.Refresh();
        }

        if (assetPath.Contains("Cursor UI"))
        {
            TextureImporter e  = (assetImporter as TextureImporter);
            
            e.textureType           = TextureImporterType.Cursor;

            e.alphaIsTransparency   = true;
            e.wrapMode              = TextureWrapMode.Clamp;
            e.filterMode            = FilterMode.Point;
            e.isReadable            = true; 
            
            AssetDatabase.Refresh();
        }


    }
    


   
}

