using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Player;
        public Transform ShootPlatformTransform;
        public GameObject Target;
        public Material OneTargetMaterial;
        public Material ThreeTargetMaterial;
        public Bow Bow;
        public GameObject Canvas;
        public LineRenderer LineRenderer;
        public Transform IndexFingerTipTransform;
        public Transform IndexFingerRootTransform;
        public BarSight BarSight;

        private readonly State State = new State(EnumTargetType.One, EnumDistance.M18);

        // Start is called before the first frame update
        void Start()
        {
            Player.transform.position = ShootPlatformTransform.position;
            HideMenu();

            State.Distance = EnumDistance.M18;
            Target.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, 18);
            BarSight.SetDistance(EnumDistance.M18);
        }

        // Update is called once per frame
        void Update()
        {
            if (OVRInput.Get(OVRInput.Button.Start, OVRInput.Controller.LTouch))
            {
                if(!State.IsMenuOpen)
                    ShowMenu();
            }
            else
            {
                if (State.IsMenuOpen)
                    HideMenu();
            }
            
            if (State.IsMenuOpen)
            {
                LineRenderer.enabled = true;
                LineRenderer.SetPosition(0, IndexFingerTipTransform.position);
                LineRenderer.SetPosition(1, IndexFingerTipTransform.position + (IndexFingerTipTransform.position - IndexFingerRootTransform.position).normalized * 2);

                RaycastHit hit;
                if (Physics.Raycast(IndexFingerTipTransform.position, IndexFingerTipTransform.position - IndexFingerRootTransform.position, out hit))
                {
                    if (hit.collider.tag == "UI")
                    {
                        LineRenderer.SetPosition(1, hit.collider.transform.position);


                        if (OVRInput.GetDown(OVRInput.Button.Any, OVRInput.Controller.RTouch))
                        {
                            UIButton button = hit.collider.GetComponent<UIButton>();
                            if (button.ButtonType == UIButton.EnumButtonType.OneTarget)
                            {
                                State.TargetType = EnumTargetType.One;
                                Target.GetComponent<Renderer>().material = OneTargetMaterial;
                            }else if (button.ButtonType == UIButton.EnumButtonType.ThreeTarget)
                            {
                                State.TargetType = EnumTargetType.Three;
                                Target.GetComponent<Renderer>().material = ThreeTargetMaterial;
                            }
                            else if(button.ButtonType == UIButton.EnumButtonType.M18)
                            {
                                State.Distance = EnumDistance.M18;
                                Target.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, 18);
                                BarSight.SetDistance(EnumDistance.M18);
                            }
                            else if (button.ButtonType == UIButton.EnumButtonType.M30)
                            {
                                State.Distance = EnumDistance.M30;
                                Target.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, 30);
                                BarSight.SetDistance(EnumDistance.M30);
                            }
                            else if(button.ButtonType == UIButton.EnumButtonType.M50)
                            {
                                State.Distance = EnumDistance.M50;
                                Target.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, 50);
                                BarSight.SetDistance(EnumDistance.M50);
                            }
                            else if (button.ButtonType == UIButton.EnumButtonType.M70)
                            {
                                State.Distance = EnumDistance.M70;
                                Target.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, 70);
                                BarSight.SetDistance(EnumDistance.M70);
                            }
                            else if (button.ButtonType == UIButton.EnumButtonType.RemoveArrows)
                            {
                                Bow.DestroyArrows();
                            }
                        }
                    }
                }

            }
            else
            {
                LineRenderer.enabled = false;

                //Move player to near the target
                if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp, OVRInput.Controller.LTouch))
                {
                    Player.transform.position = Target.transform.position - new Vector3(0, 0, 2);
                    Bow.gameObject.SetActive(false);
                }
                else if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstickUp, OVRInput.Controller.RTouch) || OVRInput.GetUp(OVRInput.Button.PrimaryThumbstickUp, OVRInput.Controller.LTouch))
                {
                    Player.transform.position = ShootPlatformTransform.position;
                    Bow.gameObject.SetActive(true);
                }
            }
        }

        void ShowMenu()
        {
            State.IsMenuOpen = true;
            Bow.gameObject.SetActive(false);
            Canvas.SetActive(true);
        }
        void HideMenu()
        {
            State.IsMenuOpen = false;
            Bow.gameObject.SetActive(true);
            Canvas.SetActive(false);
        }
    }

    public enum EnumTargetType
    {
        One,
        Three
    }

    public enum EnumDistance
    {
        M18,
        M30,
        M50,
        M70
    }

    class State
    {
        public State(EnumTargetType targetType, EnumDistance distance)
        {
            IsMenuOpen = false;
            TargetType = targetType;
            Distance = distance;
        }

        public bool IsMenuOpen { get; set; }
        public EnumTargetType TargetType { get; set; }
        public EnumDistance Distance { get; set; }
    }
}
