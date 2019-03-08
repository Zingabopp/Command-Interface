using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace Command_Interface
{
    public class OBSStatusText : MonoBehaviour
    {
        //public TextMeshProUGUI tmpGui;
        public Canvas textCanvas;
        public GameObject textGO;
        public float fontSize = 20f;
        public Vector3 pos
        {
            get { return textGO.transform.position; }
            set { textGO.transform.position = value; }
        }
        public Vector3 eul
        {
            get { return textGO.transform.eulerAngles; }
        }

        public Vector3 loc
        {
            get { return textGO.transform.localScale; }
        }
        public static int timeIndex = 0;
        public static OBSStatusText _instance;

        private void Start()
        {
            Console.WriteLine("---------Started TMProTest!!!----------------------------");
            //CreateNotifications();
            if (_instance != null)
                Destroy(_instance);
            _instance = this;
            //StartCoroutine(ChangePos());
        }

        public void ModifyPos(float x, float y, float z, float rot = 0)
        {
            Console.WriteLine("-------------------");
            Console.WriteLine($"    Position: {VectorToString(pos)}");
            Console.WriteLine($"Euler Angles: {VectorToString(eul)}");
            Console.WriteLine($"  localScale: {VectorToString(loc)}");
            Console.WriteLine("-------------------");
            pos = new Vector3(pos.x + x, pos.y + y, pos.z + z);
            Vector3 targetPos = new Vector3(0, 1.7f, 0);
            var rotAngle = Quaternion.LookRotation(targetPos - pos);
            Console.WriteLine($"Rotation Angle: {rotAngle.ToString()}");
            textGO.transform.rotation = rotAngle;
            Console.WriteLine($"New Position: {VectorToString(pos)}");
            Console.WriteLine($"Euler Angles: {VectorToString(eul)}");
            Console.WriteLine("-------------------");
        }
        private IEnumerator<WaitForSeconds> ChangePos()
        {
            while (true)
            {
                yield return new WaitForSeconds(3);
                Console.WriteLine("-------------------");
                Console.WriteLine($"    Position: {VectorToString(pos)}");
                Console.WriteLine($"Euler Angles: {VectorToString(eul)}");
                Console.WriteLine($"  localScale: {VectorToString(loc)}");
                Console.WriteLine("-------------------");
                switch (timeIndex)
                {
                    case 0:
                        pos = new Vector3(pos.x + .5f, pos.y, pos.z);
                        timeIndex++;
                        break;
                    case 1:
                        pos = new Vector3(pos.x, pos.y + .5f, pos.z);
                        timeIndex++;
                        break;
                    case 2:
                        pos = new Vector3(pos.x, pos.y, pos.z + .5f);
                        timeIndex = 0;
                        break;
                    default:
                        break;
                }
                Console.WriteLine($"New Position: {VectorToString(pos)}");
                Console.WriteLine("-------------------");
            }
        }

        private static string VectorToString(Vector3 v)
        {
            return $"({v.x},{v.y},{v.z})";
        }

        public Canvas CreateNotifications()
        {
            textGO = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(textGO);
            textGO.transform.position = new Vector3(0f, 0f, 2.5f);
            textGO.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            textGO.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            textCanvas = textGO.AddComponent<Canvas>();
            textCanvas.renderMode = RenderMode.WorldSpace;
            (textCanvas.transform as RectTransform).sizeDelta = new Vector2(200f, 50f);
            TextMeshProUGUI txtOne =  CreateNotificationText("Test Text 1");
            txtOne.rectTransform.SetParent(textCanvas.transform, false);

            return textCanvas;
        }

        public TextMeshProUGUI CreateNotificationText(string text, float yOffset = 0)
        {
            
            TextMeshProUGUI textMeshProUGUI = new GameObject("OBS_StatusText").AddComponent<TextMeshProUGUI>();
            RectTransform rectTransform = textMeshProUGUI.transform as RectTransform;
            rectTransform.anchoredPosition = new Vector2(0f, -20f);
            rectTransform.sizeDelta = new Vector2(400f, 20f);
            rectTransform.Translate(new Vector3(0, yOffset, 0));
            textMeshProUGUI.text = text;
            textMeshProUGUI.fontSize = fontSize;
            textMeshProUGUI.alignment = TextAlignmentOptions.Left;
            textMeshProUGUI.ForceMeshUpdate();
            return textMeshProUGUI;
        }
    }
}
