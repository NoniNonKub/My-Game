using UnityEngine;

public class MoveButton : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    public void OnClickMove()
    {
        Vector3 target = TapManager.Instance.LastWorldPosition;

        PlayerMover mover = player.GetComponent<PlayerMover>();
        if (mover != null)
        {
            mover.SetTarget(target);
        }

        // ✅ ใช้ Singleton แทน Find
        TapUI.Instance?.HideUI();
    }
}
