using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerCollider : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PV;
    public GameObject player;

    private bool canCollide = true;
    public float score = -1e6f;
    public float penalty = 1.0f;
    public TMPro.TMP_Text scoreText;

    private Text message = null;

    private void Start()
    {
        if(PV.IsMine)
        {
            message = GameObject.FindGameObjectWithTag("Message").GetComponent<Text>();
            message.text = "WASD and the mouse";
            score = Random.Range(-100, 100);
        }
    }

    private void Update()
    {
        scoreText.text = score < -1e5 ? "..." : score.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        if (PV.IsMine && other.gameObject.tag == "Player" && canCollide)
        {
            var otherPlayer = other.GetComponentInChildren<PlayerCollider>();
            if(otherPlayer != null)
            {
                float otherScore = otherPlayer.score;
                if (score < otherScore)
                {
                    canCollide = false;
                    Debug.Log("Collided with a player");
                    StartCoroutine(Death());
                }
            }
        }
    }

    /// <summary>
    /// Kind of a macabre name, but just sends the player to a random location.
    /// Good location for art/effects from the students.
    /// Set canCollide to true at the end so other collisions can occur
    /// </summary>
    /// <returns></returns>
    private IEnumerator Death()
    {
        if(message != null)
        {
            message.text = "You got hit!";
        }
        player.GetComponent<PlayerMovement>().paused = true;
        score -= penalty;

        float deathTime = 1.0f;//seconds
        float start = Time.time;
        Vector3 startPos = player.transform.position;
        Vector3 endPos = Vector3.zero;
        while (Time.time < start + deathTime)
        {
            yield return null;
            player.transform.position = Vector3.Lerp(startPos, endPos, (Time.time - start) / deathTime);
        }
        player.transform.position = endPos;
        canCollide = true;
        player.GetComponent<PlayerMovement>().paused = false;

        if (message != null)
        {
            message.text = "";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(score);
        }
        else
        {
            // Network player, receive data
            this.score = (float)stream.ReceiveNext();
        }
    }
}
