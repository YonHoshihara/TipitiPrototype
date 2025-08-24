using UnityEngine;

/// Builds a grayscale visibility mask (R8) from a Sprite's alpha.
/// White (1) = visible; Black (0) = transparent.
public static class SpriteVisibilityMask
{
    // Builds a Texture2D (R8) with size (w,h) in sprite UV space [0..1],
    // sampling the source sprite's alpha from its textureRect.
    public static Texture2D Build(Sprite sprite, int w, int h, float alphaThreshold = 0.01f)
    {
        if (sprite == null || sprite.texture == null)
        {
            Debug.LogError("SpriteVisibilityMask.Build: invalid sprite/texture.");
            return null;
        }
        var tex = sprite.texture;
        if (!tex.isReadable)
        {
            Debug.LogError($"SpriteVisibilityMask.Build: Texture '{tex.name}' is not readable. Enable Read/Write in Import Settings.");
            return null;
        }

        // We will sample inside the sprite.textureRect (the packed/trimmed rect in the atlas/texture)
        Rect tr = sprite.textureRect; // in pixels
        float invTexW = 1f / tex.width;
        float invTexH = 1f / tex.height;

        // Create output (R8-like) texture
        var mask = new Texture2D(w, h, TextureFormat.R8, false, true);
        mask.filterMode = FilterMode.Bilinear;
        mask.wrapMode = TextureWrapMode.Clamp;

        // For each pixel in mask (u,v in 0..1), sample source alpha from textureRect
        var buffer = mask.GetRawTextureData<byte>();
        int idx = 0;
        for (int y = 0; y < h; y++)
        {
            float v = (y + 0.5f) / h; // center sampling
            for (int x = 0; x < w; x++)
            {
                float u = (x + 0.5f) / w;

                // Map uv [0..1] to sprite.textureRect pixels
                float px = tr.x + u * tr.width;
                float py = tr.y + v * tr.height;

                // Sample alpha
                Color c = tex.GetPixelBilinear(px * invTexW, py * invTexH);
                float a = c.a;

                // Binarize or keep soft? Aqui usamos "soft" com limiar baixo.
                byte vis = (byte)(Mathf.Clamp01(a >= alphaThreshold ? a : 0f) * 255f);
                buffer[idx++] = vis;
            }
        }
        mask.Apply(false, false);
        return mask;
    }
}
