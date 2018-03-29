using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float vertical = Input.GetAxis("Vertical");

        controller.SimpleMove(vertical * transform.forward * 250f * Time.deltaTime);

        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDist = 0.0f;

        if (playerPlane.Raycast(ray, out hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            targetRotation.x = 0f;
            targetRotation.z = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 30f * Time.deltaTime);
        }

        Vector4 pos = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0f);
        Shader.SetGlobalVector("GlobalMask_Position", pos);

    }
}