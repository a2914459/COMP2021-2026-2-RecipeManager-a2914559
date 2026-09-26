using System.Collections.Generic;
using System.Net.Mail;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }

    [Fact]
    public void AddRecipe_ReturnsTrue()
    {
        var manager = CreateManager();
        var newRecipe = new Recipe {Id = 100, Title = "Test Recipe"};

        bool result = manager.AddRecipe(newRecipe);

        Assert.True(result);
        Assert.Equal(3, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_ReturnsFalse()
    {
        var manager = CreateManager();
        var duplicateRecipe = new Recipe {Id = 10, Title = "duplicate"};

        bool result = manager.AddRecipe(duplicateRecipe);

        Assert.False(result);
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void FindRecipe_MissingId()
    {
        var manager = CreateManager();
        Recipe? found = manager.FindRecipe(999);
        Assert.Null(found);
    }

    [Fact]
    public void RemoveRecipe_ReturnsTrue()
    {
        var manager = CreateManager();

        bool result = manager.RemoveRecipe(10);

        Assert.True(result);
        Assert.Null(manager.FindRecipe(10));
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact]
    public void RemoveRecipe_ReturnsFalse()
    {
        var manager = CreateManager();
        bool result = manager.RemoveRecipe(999);
        Assert.False(result);
    }

    [Fact]
    public void AddIngredientsToShoppingList_AddsIngredientsInOrder()
    {
        var manager = CreateManager();
        int addedCount = manager.AddIngredientsToShoppingList(10);
        Assert.Equal (1, addedCount);
        Assert.Equal(new[] {"1 apple"}, manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredientsToShoppingList_MissingRecipe()
    {
        var manager = CreateManager();

        int addedCount = manager.AddIngredientsToShoppingList(999);

        Assert.Equal(0, addedCount);

        Assert.Empty(manager.GetShoppingList());
    }


    [Fact]
    public void ClearShoppingList_RemovesAll()
    {
        var manager = CreateManager();

        manager.AddIngredientsToShoppingList(10);

        manager.ClearShoppingList();

        Assert.Empty(manager.GetShoppingList());

        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_AddsRecipesInOrder()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);

        Assert.Equal(new[] {10, 20}, manager.GetCookingPlan());
    }

    [Fact]
    public void AddRecipeToCookingPlan_MissingRecipe()
    {
        var manager = CreateManager();

        bool result = manager.AddRecipeToCookingPlan(999);

        Assert.False(result);
        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_ReturnFalse()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        bool result = manager.AddRecipeToCookingPlan(10);

        Assert.False(result);
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_ReturnTrue()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);

        bool result = manager.RemoveRecipeFromCookingPlan(10);

        Assert.True(result);
        Assert.Equal(0, manager.CookingPlanCount);
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_ReturnFalse()
    {
        var manager = CreateManager();

        bool result = manager.RemoveRecipeFromCookingPlan(10);

        Assert.False(result);
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void PeekLastRemovedRecipe_ReturnsNull()
    {
        var manager = CreateManager();
        int? result = manager.PeekLastRemovedRecipe();
        Assert.Null(result);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_ReturnsFalse()
    {
        var manager = CreateManager();
        bool result = manager.RestoreLastRemovedRecipe();
        Assert.False(result);
    }

    [Fact]
    public void RemovedRecipies_RestoreInLIFO()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);

        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);

        Assert.Equal(20, manager.PeekLastRemovedRecipe());

        bool restored = manager.RestoreLastRemovedRecipe();

        Assert.True(restored);
        Assert.Equal(new[] {20}, manager.GetCookingPlan());
        Assert.Equal(10, manager.PeekLastRemovedRecipe());
    }


}
