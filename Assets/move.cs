using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct MoveCommand
{
    public Vector3 playerPos;       // 플레이어의 이전 위치
    public bool isPush;             // 박스를 밀었는가?
    public GameObject pushedBox;    // 밀린 박스 오브젝트
    public Vector3 boxPos;          // 박스의 이전 위치
}
public class move : MonoBehaviour
{
    public float moveSpeed = 5f;        // 이동 속도
    public LayerMask obstacleLayer;     // 벽(Wall)이나 박스(Box) 레이어 설정 필수

    private bool isMove = false;
    private MyStack<MoveCommand> historyStack = new MyStack<MoveCommand>();
    void Update()
    {
        if (isMove) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            TryUndo();
            return; // Undo를 했으면 이동 로직은 패스
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0)
        {
            TryMove(Vector3.right * h);
        }
        else if (v != 0)
        {
            TryMove(Vector3.up * v);
        }

    }
    
    private void TryMove(Vector3 direction)
    {
        Debug.DrawRay(transform.position, direction * 1f, Color.red, 1.0f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayer);
        if (hit.collider == null)
        {
            MoveCommand cmd = new MoveCommand();
            cmd.playerPos = transform.position;
            cmd.isPush = false;
            historyStack.Push(cmd);

            StartCoroutine(MoveCoroutine(transform, direction));

        }
        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Box"))
        {
            if (PushBox(hit.collider.gameObject, direction))
            {
                // 박스가 밀렸으므로, 플레이어 이동도 시작
                StartCoroutine(MoveCoroutine(transform, direction));
            }
        }
    }

    public bool PushBox(GameObject box,Vector3 direction)
    {
        Vector3 boxNextPos = box.transform.position + direction;

        // 박스가 갈 곳에 장애물이 있는지 확인
        if (Physics2D.OverlapCircle(boxNextPos, 0.1f, obstacleLayer))
        {
            return false; // 벽이나 다른 박스가 있어서 못 밈
        }
        MoveCommand cmd = new MoveCommand();
        cmd.playerPos = transform.position;
        cmd.isPush = true;                  
        cmd.pushedBox = box;                // 민 박스
        cmd.boxPos = box.transform.position;// 박스의 원래 위치
        historyStack.Push(cmd);

        StartCoroutine(MoveCoroutine(box.transform, direction));

        return true; // 성공
    }

    private void TryUndo()
    {
        if (historyStack.IsEmpty())
        {
            Debug.Log("더 이상 되돌릴 수 없습니다.");
            return;
        }

        MoveCommand lastCmd = historyStack.Pop();
        transform.position = lastCmd.playerPos;

        // 2. 박스를 밀었던 상황이라면, 박스도 원위치!
        if (lastCmd.isPush)
        {
            lastCmd.pushedBox.transform.position = lastCmd.boxPos;
        }

        // 혹시 이동 중에 Undo 했을 때를 대비해 상태 초기화
        StopAllCoroutines();
        isMove = false;
    }
    private IEnumerator MoveCoroutine(Transform target, Vector3 direction)
    {
        isMove = true;

        Vector3 startPos = target.position;
        Vector3 targetPos = startPos + direction;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            target.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        target.position = targetPos;
        isMove = false;
    }
   
}
