using UnityEngine;

public sealed class ObjectiveTrigger : MonoBehaviour
{
    [SerializeField] private string destinationScene;
    private bool activated;

    public void Configure(string scene)
    {
        destinationScene = scene;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated || other.GetComponentInParent<PlayerController>() == null)
            return;

        activated = true;
        SceneTransitionManager.LoadScene(destinationScene);
    }
}
