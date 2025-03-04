using UnityEngine;

public class ShaderEffectController : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] bool eightDirectionOutline;
    [SerializeField] float emissionMultiplier = 1;
    [SerializeField] int outlineWidth;

    MaterialPropertyBlock propertyBlock;

    public void Initialize()
    {
        propertyBlock = new MaterialPropertyBlock();

        sr.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat("_OutlineSize", outlineWidth);
        propertyBlock.SetFloat("_Use8Directions", eightDirectionOutline ? 1f : 0);
        propertyBlock.SetFloat("_EmissionMultiplier", emissionMultiplier);

        sr.SetPropertyBlock(propertyBlock);
    }

    public void SetHitEffect(float hitEffectBlend)
    {
        propertyBlock.SetFloat("_HitEffectBlend", hitEffectBlend);
        sr.SetPropertyBlock(propertyBlock);
    }

    public void SetColor(Color color)
    {
        propertyBlock.SetColor("_BaseColor", color);
        sr.SetPropertyBlock(propertyBlock);
    }

    public void SetOutlineColor(Color outlineColor)
    {
        propertyBlock.SetColor("_OutlineColor", outlineColor);
        sr.SetPropertyBlock(propertyBlock);
    }

    public void SetInvincible(float value)
    {
        propertyBlock.SetFloat("_Invincibility", value);
        sr.SetPropertyBlock(propertyBlock);
    }
}
