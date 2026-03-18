using UnityEditor;

namespace SGL.Boot.Editor
{
    /// <summary>
    /// Automatically sets Texture Type to Sprite for all textures
    /// imported into the com.sgl.boot/Sprites/ folder.
    /// </summary>
    public class BootSpritePostprocessor : AssetPostprocessor
    {
        private const string SpritesFolder = "Packages/com.sgl.boot/Sprites/";

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(SpritesFolder)) return;

            TextureImporter importer = (TextureImporter)assetImporter;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType    = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100f;
                importer.mipmapEnabled  = false;
            }
        }
    }
}