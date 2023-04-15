using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Scroller : MonoBehaviour
{
    [SerializeField] private Renderer m_quadRenderer;

    [SerializeField] private float xScrollSpeed = .5f;
    [SerializeField] private float yScrollSpeed = .5f;

    #region  Unity Methods

    private void Start() =>
        m_quadRenderer = GetComponent<Renderer>();

    private void Update()
    {
        var textureOffset = new Vector2(xScrollSpeed * Time.time, yScrollSpeed * Time.time);
        m_quadRenderer.material.mainTextureOffset = textureOffset;
    }
    
    #endregion Unity Methods
}
