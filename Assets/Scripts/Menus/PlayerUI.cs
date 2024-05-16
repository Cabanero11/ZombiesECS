using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI nivelTexto;
}

public class PlayerUIBaker : Baker<PlayerUI>
{
    public override void Bake(PlayerUI authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponentObject(entity, authoring);
    }
}
