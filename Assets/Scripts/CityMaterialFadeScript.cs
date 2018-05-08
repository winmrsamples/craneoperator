using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMaterialFadeScript : MonoBehaviour {

	public Material cityMaterial;
	public Material cityMaterialLoaded;
	
	void Start() {
		StartCoroutine(FadeScript(3));
	}

	IEnumerator FadeScript(float aTime)
	{

		for (float t = 0.0f; cityMaterial.color.a <= 1.0f; t += Time.deltaTime / aTime)
		{
			yield return new WaitForSeconds(0.01f);
			float alphaColour = cityMaterial.color.a;
			Color newColor = new Color(1f, 1f, 1f, alphaColour += .01f);
			cityMaterial.color = newColor;
		}
		cityMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		cityMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
		cityMaterial.SetInt("_ZWrite", 1);
		cityMaterial.DisableKeyword("_ALPHATEST_ON");
		cityMaterial.DisableKeyword("_ALPHABLEND_ON");
		cityMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
		cityMaterial.renderQueue = -1;
	}

}
