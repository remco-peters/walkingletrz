using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WordListCallbacks : MonoBehaviour, IPunObservable
{
    private void Awake()
    {
        PhotonNetwork.SendRate = 1;
        PhotonNetwork.SerializationRate = 1;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
