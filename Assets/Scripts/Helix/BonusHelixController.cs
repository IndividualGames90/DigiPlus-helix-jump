using UnityEngine;
using TMPro;

public class BonusHelixController : HelixController
{
    [Header("Bonus Settings")]
    [SerializeField] private int _multiplierValue = 2;
    [SerializeField] private Material[] _bonusMaterials;
    [SerializeField] private TextMeshPro[] _multiplierTexts;

    private void Start()
    {
        if (_bonusMaterials != null && _bonusMaterials.Length > 0)
        {
            var selectedMaterial = _bonusMaterials[Random.Range(0, _bonusMaterials.Length)];

            foreach (var helix in _allHelix)
            {
                var renderer = helix.GetComponent<MeshRenderer>();
                if (renderer != null)
                    renderer.material = selectedMaterial;
            }
        }


        // Update all text labels to show multiplier value
        if (_multiplierTexts != null && _multiplierTexts.Length > 0)
        {
            foreach (var text in _multiplierTexts)
            {
                if (text)
                    text.text = "x" + _multiplierValue;
            }
        }
    }

    public void BonusHelixHit()
    {
        ScoreController.Instance.AddBonusHelixBreak(_multiplierValue);
        GameController.Instance.GameOver(ScoreController.Instance.GetScore(), false);
    }
}
