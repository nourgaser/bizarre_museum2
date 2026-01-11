using UnityEngine;

[CreateAssetMenu(fileName = "ItemDefinition", menuName = "BizarreMuseum/Item Definition", order = 0)]
public class ItemDefinition : ScriptableObject
{
    public string slug = "gravity-anomaly";
    public string displayName = "Gravity Anomaly";
    public Color hintColor = new Color(0f, 1f, 1f, 0.35f);
    public GameObject innerPrefab;
}
