using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // TODO Part A: add your private collection fields here.
    Dictionary<int, Recipe> catalogue = new Dictionary<int, Recipe>();
    List<string> shoppingList = new List<string>();
    LinkedList<int> cookingPlan = new LinkedList<int>();
    Stack<int> removedRecipes = new Stack<int>();
    Queue<string> pendingInstructions = new Queue<string>();


    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        // TODO Part A: validate recipes and build Dictionary<int, Recipe>.

        if (recipes == null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }
        foreach (Recipe recipe in recipes)
        {
            if (recipe == null)
            {
                throw new ArgumentException("Recipe is empty.");
            }
            if (recipe.Id <= 0)
            {
                throw new ArgumentException("Recipe ID should be over 1");
            }
            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException("Recipe Title is empty.");
            }
            bool alreadyExist = catalogue.ContainsKey(recipe.Id);
            if (alreadyExist)
            {
                throw new ArgumentException("Recipe with the same ID already exist.");
            }
            catalogue.Add(recipe.Id, recipe);
        }
    }

    public int RecipeCount => catalogue.Count;
    public int ShoppingItemCount => shoppingList.Count;
    public int CookingPlanCount => cookingPlan.Count;
    public int PendingInstructionCount => pendingInstructions.Count;
    public int RemovedRecipeCount => removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }
        if (recipe.Id <= 0)
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            return false;
        }
        bool alreadyExist = catalogue.ContainsKey(recipe.Id);
        if (alreadyExist)
        {
            return false;
        }
        catalogue.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        Recipe? found = null;
        catalogue.TryGetValue(recipeId, out found);
        return found;
    }

    public bool RemoveRecipe(int recipeId)
    {
        bool inCookingPlan = cookingPlan.Contains(recipeId);
        if (inCookingPlan)
        {
            return false;
        }
        bool removed = catalogue.Remove(recipeId);
        return removed;
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if (recipe == null)
        {
            return 0;
        }

        int count = 0;

        foreach (string ingredient in recipe.Ingredients)
        {
            shoppingList.Add(ingredient);
            count += 1;
        }

        return count;
    }

    public IReadOnlyList<string> GetShoppingList() => new List<string>(shoppingList);
    public void ClearShoppingList() => shoppingList.Clear();

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if (recipe == null)
        {
            return false;
        }
        bool alreadyInPlan = cookingPlan.Contains(recipeId);
        if (alreadyInPlan)
        {
            return false;
        }
        cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        bool removed = cookingPlan.Remove(recipeId);
        if (removed == false)
        {
            return false;
        }

        removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (removedRecipes.Count == 0)
        {
            return false;
        }

        int recipeId = removedRecipes.Pop();

        Recipe? recipe = FindRecipe(recipeId);

        if (recipe == null)
        {
            return false;
        }

        bool alreadyInPlan = cookingPlan.Contains(recipeId);
        if (alreadyInPlan)
        {
            return false;
        }

        cookingPlan.AddLast(recipeId);
        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        if (removedRecipes.Count == 0)
        {
            return null;
        }

        return removedRecipes.Peek();
    }

    public IReadOnlyList<int> GetCookingPlan() =>
         new List<int>(cookingPlan);
    public bool StartCooking(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);

        if (recipe == null)
        {
            return false;
        }

        if (recipe.Instructions.Count == 0)
        {
            return false;
        }

        pendingInstructions.Clear();

        foreach(string instruction in recipe.Instructions)
        {
            pendingInstructions.Enqueue(instruction);
        }

        return true;
    }

    public string? PeekNextInstruction()
    {
        if (pendingInstructions.Count == 0)
        {
            return null;
        }

        return pendingInstructions.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if (pendingInstructions.Count == 0)
        {
            return null;
        }

        return pendingInstructions.Dequeue();
    }

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
