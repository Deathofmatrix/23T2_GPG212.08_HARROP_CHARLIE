using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ChocolateFactory
{
    public class BuildingGhost : MonoBehaviour
    {
        private Transform visual;
        private PlacedObjectTypeSO placedObjectTypeSO;

        private Color origionalColour;

        private void Start()
        {
            RefreshVisual();

            GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
        }

        private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
        {
            RefreshVisual();
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = (GridBuildingSystem.Instance.GetMouseWorldSnappedPosition());
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

            transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
            //Debug.Log(GridBuildingSystem.Instance.GetPlacedObjectRotation());
        }

        private void RefreshVisual()
        {
            if (visual != null)
            {
                Destroy(visual.gameObject);
                visual = null;
            }

            PlacedObjectTypeSO placedObjectTypeSO = GridBuildingSystem.Instance.GetPlacedObjectTypeSO();

            if (placedObjectTypeSO != null)
            {
                visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
                visual.parent = transform;
                visual.localPosition = new Vector3(0,0,10);
                visual.localEulerAngles = Vector3.zero;

                visual.GetComponent<SortingGroup>().enabled = true;

                Color newColor;

                SpriteRenderer[] childrenSprite = GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer spriteRenderer in childrenSprite)
                {
                    newColor = spriteRenderer.color;
                    newColor.a = 0.5f;
                    spriteRenderer.color = newColor;
                }
                
                SetLayerRecursive(visual.gameObject, 11);
            }
        }

        private void SetLayerRecursive(GameObject targetGameObject, int layer)
        {
            targetGameObject.layer = layer;
            foreach (Transform child in targetGameObject.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }
    }
}
