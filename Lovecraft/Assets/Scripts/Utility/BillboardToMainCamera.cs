using UnityEngine;

public class BillboardToMainCamera : MonoBehaviour
{
    public Vector3 ClampToAxis = Vector3.one;

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(Camera.main.transform,ClampToAxis);
        //Quaternion rot = transform.rotation;
        //rot.eulerAngles = new Vector3(rot.eulerAngles.x * ClampToAxis.x, rot.eulerAngles.y - 180f, rot.eulerAngles.z * ClampToAxis.z);
        //transform.rotation = rot;
        transform.forward = Camera.main.transform.forward;
    }
}
