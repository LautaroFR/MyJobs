using Autohand;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public List<GameObject> puzzlePieces = new List<GameObject>();

    //[SerializeField]
    //private PhysicsGadgetLever turnableKey;

    private Animation chestAnimation;

    private bool _chestUnlocked;

    void Start()
    {
        //turnableKey.OnMax.AddListener(KeyTurned);
        chestAnimation = GetComponent<Animation>();
    }

    private void KeyTurned()
    {
        if (!_chestUnlocked)
        {
            UnlockChest();
        }

    }

    public void UnlockChest()
    {
        chestAnimation.Play();
        _chestUnlocked = true;
        foreach (var piece in puzzlePieces)
        {
            piece.SetActive(true);
        }
    }
}
