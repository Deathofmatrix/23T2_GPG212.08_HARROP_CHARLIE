using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChocolateFactory
{
    public class CraftingDatabase : MonoBehaviour
    {
        [SerializeField] private List<Recipe> recipes = new List<Recipe>();
        public static List<Recipe> StaticRecipes;
        private void Awake()
        {
            StaticRecipes = null;
            StaticRecipes = new List<Recipe>(recipes);
        }

        public static ItemSO CheckIfRecipieValid(PlacedObjectTypeSO.BuildingType buildingType, ItemSO itemInput1, ItemSO itemInput2)
        {
            ItemSO itemToCraft = null;

            switch (buildingType)
            {
                case PlacedObjectTypeSO.BuildingType.Undefined:
                    Debug.Log("No BuildingType Given");
                    break;
                case PlacedObjectTypeSO.BuildingType.Refiner:
                    Debug.Log("Recipe Chose Refiner");
                    itemToCraft = CheckRefinerRecipies(itemInput1);
                    break;
                case PlacedObjectTypeSO.BuildingType.Combiner:
                    Debug.Log("Recipe Chose Combiner");
                    itemToCraft = CheckCombinerRecipies(itemInput1, itemInput2);
                    break;
            }


            return itemToCraft;
        }

        public static ItemSO CheckRefinerRecipies(ItemSO itemInput)
        {
            ItemSO itemToCraft = null;
            foreach(Recipe recipe in StaticRecipes)
            {
                //Debug.Log(recipie.inputs[0]);
                //Debug.Log(recipie.BuildingToCreate);
                //Debug.Log(recipie.output);
                if (recipe.inputs[0] == itemInput && recipe.BuildingToCreate == PlacedObjectTypeSO.BuildingType.Refiner)
                {
                    Debug.Log("FoundMatch" + recipe.inputs[0] + " " + itemInput);
                    itemToCraft = recipe.output;
                }
            }
            return itemToCraft;
        }

        public static ItemSO CheckCombinerRecipies(ItemSO itemInput1, ItemSO itemInput2)
        {
            Debug.Log("started checking combinations");

            ItemSO itemToCraft = null;
            foreach (Recipe recipe in StaticRecipes)
            {
                if (recipe.BuildingToCreate == PlacedObjectTypeSO.BuildingType.Combiner)
                {
                    if (recipe.inputs[0] == itemInput1 || recipe.inputs[1] == itemInput1)
                    {
                        //Debug.Log("Input 1 = " + recipe.nameString);
                        if (recipe.inputs[0] == itemInput2 || recipe.inputs[1] == itemInput2)
                        {
                            Debug.Log(recipe.output.ToString());
                            //Debug.Log("FoundMatch" + recipie.inputs[0] + " " + itemInput);
                            itemToCraft = recipe.output;
                        }
                    }
                } 
            }

            return itemToCraft;
        }
    }

    [System.Serializable]
    public class Recipe
    {
        public string nameString;
        public PlacedObjectTypeSO.BuildingType BuildingToCreate;
        public ItemSO[] inputs;
        public ItemSO output;
    }
}
