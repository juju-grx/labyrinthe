using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] // permet de void la variable private dans l'éditeur Unity
    private float speed = 3f; // vitesse de déplacement

    [SerializeField]
    private float mouseSensitivityX = 3f; // sensibiliter de la souris horizontalement

    [SerializeField]
    private float mouseSensitivityY = 3f; // sensiniliter de la souris verticalement

    [SerializeField]
    private Camera Cam;

    [SerializeField]
    private Camera CamPerso;
    private PlayerMotor motor; // réference au script PlayerMotor

    private bool game= true;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(Cam.enabled == true)
            {
                Cam.enabled = false;
                CamPerso.enabled = true;
            } else
            {
                Cam.enabled = true;
                CamPerso.enabled = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (game)
            { game = false; }
            else { game = true; }
        }
        // Calcule la vélocité (vitesse) du movment de notre joueur
        float xMov = Input.GetAxisRaw("Horizontal"); // représente les mouvement gauche droite
        float zMov = Input.GetAxisRaw("Vertical"); // représente les mouvement avant arriere

        Vector3 moveHorisontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorisontal + moveVertical).normalized * speed; // correspond a la vitesse et direct de déplacement du personnage

        motor.Move(velocity);

        if (game)
        {
            //Calcule de la rotation du joueur en Vector3
            float yRot = Input.GetAxisRaw("Mouse X");
            Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivityX;
            motor.Rotate(rotation);

            //Calcule de la rotation de la camera en Vector3
            float xRot = Input.GetAxisRaw("Mouse Y");
            Vector3 cameraRotation = new Vector3(xRot, 0, 0) * mouseSensitivityY;
            motor.RotateCamera(cameraRotation);
        }
    }
}
 
