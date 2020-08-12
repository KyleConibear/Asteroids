using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private Image playerHealthImage = null;

    /// <summary>
    /// Update the player health bar image by reducing its localScale on the 'x' axis.
    /// </summary>
    /// <param name="percentRemaining">The percentage of player health remaining.</param>
    public void UpdatePlayerHealth(float percentRemaining)
    {
        this.playerHealthImage.rectTransform.localScale = new Vector3(percentRemaining, 1, 1);
    }
}
