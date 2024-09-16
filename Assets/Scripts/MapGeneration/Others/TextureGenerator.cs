using UnityEngine;

public static class TextureGenerator {


	public static Texture2D TextureFromColourMap(Color[] colourMap, int size) {
		Texture2D texture = new Texture2D (size, size, TextureFormat.RGB24, false, true);
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}

	public static Texture2D TextureFromNoiseMap(float[,] noiseMap) {
		int size = noiseMap.GetLength (0);
		Color[] colourMap = new Color[size * size];
		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				colourMap [y * size + x] = Color.Lerp (Color.black, Color.white, noiseMap[x, y]);
			}
		}
		return TextureFromColourMap (colourMap, size);
	}


    public static void WriteTexture(Texture2D texture, string pathToFile){
        // byte[] bytes = texture.EncodeToPNG();
		byte[] bytes = texture.EncodeToJPG();
        System.IO.File.WriteAllBytes(pathToFile, bytes);


    }
}
