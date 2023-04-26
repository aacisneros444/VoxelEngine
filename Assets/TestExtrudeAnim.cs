using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExtrudeAnim : MonoBehaviour {

    public bool start = false;
    public bool reverse;
    public bool reset = false;
    public bool captureStartPos = false;
    public bool startAnim;

    public float duration = 1f;
    public Vector3 startPos;
    public Vector3 endPos;

    private float startTime;

    private void Update() {

        if (captureStartPos) {
            startPos = transform.position;
            captureStartPos = false;
        }

        if (reset) {
            startAnim = false;
            transform.position = startPos;
            reset = false;
        }


        if (start) {
            startTime = Time.time;
            start = false;
            startAnim = true;
        }

        if (startAnim) {
            float timeSinceStart = (Time.time - startTime);
            float t = Mathf.Clamp01(timeSinceStart / duration);
            if (!reverse) {
                transform.position = Vector3.Lerp(startPos, endPos, t);
                if (Vector3.Distance(transform.position, endPos) < 0.005f) {
                    startAnim = false;
                }
            } else {
                transform.position = Vector3.Lerp(endPos, startPos, t);
                if (Vector3.Distance(transform.position, startPos) < 0.005f) {
                    startAnim = false;
                }
            }
        }
    }
}
