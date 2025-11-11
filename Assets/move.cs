using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class move : MonoBehaviour
{
    public float moveSpeed = 5f;        // 이동 속도
    public LayerMask obstacleLayer;     // 벽(Wall)이나 박스(Box) 레이어 설정 필수

    private bool isMove = false;

    void Update()
    {
        if (isMove) return;

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            Vector3 moveDir = Vector3.right * Input.GetAxisRaw("Horizontal");
            TryMove(moveDir);
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            Vector3 moveDir = Vector3.up * Input.GetAxisRaw("Vertical");
            TryMove(moveDir);
        }
    }
    private void TryMove(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayer);
        if (hit.collider == null)
        {
            StartCoroutine(MoveCoroutine(direction));
        }
        else
        {
            Debug.Log("벽이나 장애물에 막힘!");
        }
    }

    private IEnumerator MoveCoroutine(Vector3 direction)
    {
        isMove = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        isMove = false;
    }
}
