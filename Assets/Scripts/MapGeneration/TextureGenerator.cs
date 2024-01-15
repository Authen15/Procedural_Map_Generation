using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TextureGenerator {


	public static Texture2D TextureFromColourMap(Color[] colourMap, int size) {
		Texture2D texture = new Texture2D (size, size);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}


	public static Texture2D TextureFromHeightMap(DataMap heightMap) {
		int size = heightMap.size;

		Color[] colourMap = new Color[size * size];
		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				colourMap [y * size + x] = Color.Lerp (Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue,heightMap.maxValue,heightMap.values [x, y]));
			}
		}

		return TextureFromColourMap (colourMap, size);
	}

    public static void WriteTexture(Texture2D texture, string pathToFile){
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(pathToFile, bytes);
    }
}
