using TMPro;
using UnityEngine;

public class HealthLabel : MonoBehaviour
{
    [SerializeField]
    ValueObject<float> healthObject;

    [SerializeField]
    IEvaluator healthEvaluator;

    [SerializeField]
    TextMeshProUGUI label;

    static readonly string str = "Health: ";

    private void OnEnable()
    { 
        healthObject.OnValueChanged += UpdateLabel;
    }

    private void OnDisable()
    {
        healthObject.OnValueChanged -= UpdateLabel;
    }

    void UpdateLabel(float health)
    {
        label.text = str + healthObject.Value;
    }
}
