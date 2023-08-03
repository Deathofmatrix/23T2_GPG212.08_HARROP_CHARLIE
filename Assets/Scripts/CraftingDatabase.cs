using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace ChocolateFactory
{
    public class CraftingDatabase : MonoBehaviour
    {
        [SerializeField] private List<Recipe> recipes = new List<Recipe>();
        public static List<Recipe> StaticRecipes;

        [SerializeField] private ItemSO incorrectCombinationOutput;
        public static ItemSO incorrectCombinationOutputStatic;
        private void Awake()
        {
            StaticRecipes = null;
            StaticRecipes = new List<Recipe>(recipes); // AG: I think this will simply replace the List; you might not need the line above that says "StaticRecipes = null;"
            incorrectCombinationOutputStatic = incorrectCombinationOutput;
        }

        public static ItemSO CheckIfRecipieValid(PlacedObjectTypeSO.BuildingType buildingType, ItemSO itemInput1, ItemSO itemInput2)
        {
            ItemSO itemToCraft = incorrectCombinationOutputStatic;

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
            ItemSO itemToCraft = incorrectCombinationOutputStatic;
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

            ItemSO itemToCraft = incorrectCombinationOutputStatic;

            if (itemInput1 == itemInput2) return itemToCraft;

            foreach (Recipe recipe in StaticRecipes)
            {
                if (recipe.BuildingToCreate != PlacedObjectTypeSO.BuildingType.Combiner) continue;
                //Debug.Log("Building To create is Correct on " + recipe.nameString);
                if (!AreInputsIdentical(recipe.inputs[0], itemInput1) && !AreInputsIdentical(recipe.inputs[1], itemInput1)) continue;
                //Debug.Log("One Item is Correct on " + recipe.nameString);
                if (!AreInputsIdentical(recipe.inputs[0], itemInput2) && !AreInputsIdentical(recipe.inputs[1], itemInput2)) continue;
                //Debug.Log("Two items are Correct on " + recipe.nameString);


                Debug.Log(recipe.output.ToString());
                //Debug.Log("FoundMatch" + recipie.inputs[0] + " " + itemInput);
                itemToCraft = recipe.output;
            }

            return itemToCraft;
        }

        public static bool AreInputsIdentical(ItemSO item1, ItemSO item2)
        {
            if (item1 == item2)
            {
                return true;
            }
            else
            {
                return false;
            }
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
