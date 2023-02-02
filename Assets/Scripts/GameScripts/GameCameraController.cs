using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform PlayerControllerTransform;
    public Vector3 DistanceToPlayer;
    [SerializeField] float CameraMoveSpeed;
    public bool CanMove = false;
    private void OnEnable()
    {
        EventManager.StartListening(GameConstants.GameEvents.COMPLETED_FOLLOWING_PATH, MoveCamera);
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameConstants.GameEvents.COMPLETED_FOLLOWING_PATH, MoveCamera);
    }

    void MoveCamera(EventParam param)
    {
        StartCoroutine(SetCanMoveCRT());
        //transform.position = Vector3.Lerp(transform.position, PlayerControllerTransform.transform.position - DistanceToPlayer, CameraMoveSpeed * Time.deltaTime);
        //transform.position = Vector3.Lerp(transform.position, new Vector3(0,transform.position.y,PlayerControllerTransform.position.z), CameraMoveSpeed * Time.deltaTime);
    }

    IEnumerator SetCanMoveCRT()
    {
        CanMove = true;
        yield return new WaitForSeconds(1f);
        CanMove = false;
    }

    void Start()
    {
        DistanceToPlayer = transform.position - PlayerControllerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(PlayerControllerTransform.position.x + DistanceToPlayer.x, transform.position.y, PlayerControllerTransform.position.z+DistanceToPlayer.z), CameraMoveSpeed * Time.deltaTime);
            if (transform.position.z == PlayerControllerTransform.position.z + DistanceToPlayer.z)
            {
                CanMove = false;
            }
        }
    }
}
