using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    #region Game Settings


    #endregion

    #region Debug Settings

    [Header("Debug Settings")]
    [Tooltip("Enable or disable debug mode for additional logging.")]
    public bool debugMode = false;

    #endregion
}