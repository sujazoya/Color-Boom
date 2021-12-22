using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GuiLevelItem : MonoBehaviour {

    [HideInInspector]
    public LevelIdentifier level;

    public Image Background;
    public GameObject OkIcon;
    public GameObject LockIcon;
    public Text Label;
    public GameObject Selected;

    public Color lockedColor;
    public Color unlockColor;
    public Color completedColor;
    [SerializeField] AudioSource buttonSound;
    GuiLevelGrid levelGrid;
    private void OnEnable()
    {
        levelGrid = FindObjectOfType<GuiLevelGrid>();
        transform.GetComponent<Button>().onClick.AddListener(DoOnClick);
    }

    void DoOnClick()
    {
        levelGrid.currentLevelItem = this;
        levelGrid.guiLevelItemOnClick();
        buttonSound.Play();
    }

}
