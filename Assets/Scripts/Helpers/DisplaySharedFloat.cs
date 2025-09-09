using TMPro;
using UnityEngine;

public class DisplaySharedFloat : MonoBehaviour
{
    [SerializeField] private SharedFloat sharedFloat;
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";

    private TextMeshProUGUI label;

    private void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        sharedFloat.OnChange += UpdateLabel;
    }

    private void OnDisable()
    {
        sharedFloat.OnChange -= UpdateLabel;
    }

    private void Start()
    {
        UpdateLabel(sharedFloat.shared);
    }

    void UpdateLabel(float value)
    {
        label.text = $"{prefix}{value.ToString()}{suffix}";
    }
}
