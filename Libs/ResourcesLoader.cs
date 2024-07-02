using RedLoader;
using System.Reflection;
using UnityEngine;

namespace ZombieMode.Libs;

public class ResourcesLoader
{
    public static bool TryGetEmbeddedResourceBytes(string name, out byte[] bytes)
    {
        bytes = null;

        var executingAssembly = Assembly.GetExecutingAssembly();

        var desiredManifestResources = executingAssembly.GetManifestResourceNames().FirstOrDefault(resourceName =>
        {
            var assemblyName = executingAssembly.GetName().Name;
            return !string.IsNullOrEmpty(assemblyName) && resourceName.StartsWith(assemblyName) && resourceName.Contains(name);
        });

        if (string.IsNullOrEmpty(desiredManifestResources))
            return false;

        using (var ms = new MemoryStream())
        {
            executingAssembly.GetManifestResourceStream(desiredManifestResources).CopyTo(ms);
            bytes = ms.ToArray();
            return true;
        }
    }

    public static Texture2D ByteToTex(byte[] imgBytes)
    {
        Texture2D tex = new(2, 2);
        tex.LoadImage(imgBytes);
        return tex;
    }

    public static Texture2D ResourceToTex(string name)
    {
        if (TryGetEmbeddedResourceBytes(name, out byte[] bytes))
        {
            return ByteToTex(bytes);
        }
        else RLog.Error($"Couldn't convert {name} resource to texture");
        return null;
    }

    public static Sprite ToSprite(Texture2D texture)
    {
        var rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
        var pivot = new Vector2(0.5f, 0.5f);
        var border = Vector4.zero;
        var sprite = Sprite.CreateSprite_Injected(texture, ref rect, ref pivot, 100, 0, SpriteMeshType.Tight, ref border, false, null);
        sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        return sprite;
    }

    public static Font LoadEmbeddedFont(string resourceName)
    {
        if (TryGetEmbeddedResourceBytes(resourceName, out byte[] bytes))
        {
            byte[] fontData = bytes;
            string tempFilePath = Path.Combine(Application.persistentDataPath, $"{Guid.NewGuid()}.ttf");
            File.WriteAllBytes(tempFilePath, fontData);

            Font customFont = new Font(tempFilePath);

            File.Delete(tempFilePath);

            return customFont;
        }
        return null;
    }
}
