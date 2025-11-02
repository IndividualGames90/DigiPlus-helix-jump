using System.Collections;
using UnityEngine;

public class FinishController : MonoBehaviour
{
    private bool _firstSegmentFinished;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (_firstSegmentFinished)
            {
                //Put score board here, then start the game again so player can fall to the score helixes.
            }
            else
            {
                StartCoroutine(FinishDelay());
            }

        }
    }

    private IEnumerator FinishDelay()
    {
        yield return new WaitForSeconds(3);
        GameController.Instance.GameOver(ScoreController.Instance.GetScore());

    }
}
