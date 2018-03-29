using UnityEngine;

public class MaskCamera : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
		RenderTexture texture = new RenderTexture((int)(Screen.width * 0.5f), (int)(Screen.height * 0.5f), 0, RenderTextureFormat.R8);
        texture.name = "ViewMaskTex";
        texture.Create();
        cam.targetTexture = texture;

        Shader.SetGlobalTexture("_ViewMask", texture);
    }
}
