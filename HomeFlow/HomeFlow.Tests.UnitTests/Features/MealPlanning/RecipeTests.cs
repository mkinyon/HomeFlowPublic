
using HomeFlow.Features.MealPlanning.Recipes;

namespace HomeFlow.Tests.UnitTests.Features.MealPlanning;

public class RecipeTests
{
    public static IEnumerable<object[]> GroceryItemParseTestCases =>
        new List<object[]>
        {
        new object[] { new GroceryItemParseTestCase("1 Tbsp. Chili Powder", 1, MeasurementFraction.None, MeasurementType.Tablespoons, string.Empty, "Chili Powder") },
        new object[] { new GroceryItemParseTestCase("1 bottle (12 Ounce Bottle) Good Beer", 1, MeasurementFraction.None, MeasurementType.Bottle, string.Empty, "Good Beer (12 Ounce Bottle)") },
        new object[] { new GroceryItemParseTestCase("½ cup red wine", 0, MeasurementFraction.Half, MeasurementType.Cups, string.Empty, "Red Wine") },
        new object[] { new GroceryItemParseTestCase("2 pounds sirloin tips, cubed", 2, MeasurementFraction.None, MeasurementType.Pounds, "Cubed", "Sirloin Tips") },
        new object[] { new GroceryItemParseTestCase("1 (1.25 ounce) package beef with onion soup mix", 1, MeasurementFraction.None, MeasurementType.Package, string.Empty, "Beef With Onion Soup Mix (1.25 Ounce)") },
        new object[] { new GroceryItemParseTestCase("2 (10.5 ounce) cans condensed cream of mushroom soup (such as Campbell’s)", 2, MeasurementFraction.None, MeasurementType.Can, string.Empty, "Condensed Cream Of Mushroom Soup (10.5 Ounce)") },
        new object[] { new GroceryItemParseTestCase("2 stalks celery, finely chopped", 2, MeasurementFraction.None, MeasurementType.Stalks, "Finely Chopped", "Celery") },

        // Sample: https://www.allrecipes.com/dill-pickle-chicken-salad-recipe-11711924
        new object[] { new GroceryItemParseTestCase("3 cups shredded cooked chicken", 3, MeasurementFraction.None, MeasurementType.Cups, "Shredded Cooked", "Chicken") },
        new object[] { new GroceryItemParseTestCase("2 stalks celery, finely chopped", 2, MeasurementFraction.None, MeasurementType.Stalks, "Finely Chopped", "Celery") },
        // TODO: figure out this scenario new object[] { new GroceryItemParseTestCase("1/2 cup chopped dill pickles, plus 1 tablespoon pickle brine from jar", 1, MeasurementFraction.Half, MeasurementType.Cups, "Chopped", "Dill Pickles") },
        new object[] { new GroceryItemParseTestCase("1/2 cup mayonnaise", 0, MeasurementFraction.Half, MeasurementType.Cups, string.Empty, "Mayonnaise") },
        new object[] { new GroceryItemParseTestCase("1/2 teaspoon white sugar", 0, MeasurementFraction.Half, MeasurementType.Teaspoons, string.Empty, "White Sugar") },
        new object[] { new GroceryItemParseTestCase("1/4 teaspoon onion powder", 0, MeasurementFraction.Quarter, MeasurementType.Teaspoons, string.Empty, "Onion Powder") },
        new object[] { new GroceryItemParseTestCase("1/4 teaspoon salt", 0, MeasurementFraction.Quarter, MeasurementType.Teaspoons, string.Empty, "Salt") },
        new object[] { new GroceryItemParseTestCase("1/4 teaspoon freshly ground black pepper", 0, MeasurementFraction.Quarter, MeasurementType.Teaspoons, "Freshly Ground", "Black Pepper") },
    };

    [Theory( DisplayName = "Should parse ingredient text correctly" )]
    [MemberData( nameof( GroceryItemParseTestCases ), DisableDiscoveryEnumeration = false )]
    public void ExtractGroceryItemDetails_ShouldParseCorrectly( GroceryItemParseTestCase testCase )
    {
        var recipeGroceryItem = new RecipeGroceryItem();

        recipeGroceryItem.ExtractDetailsFromText( testCase.Input );

        Assert.Equal( testCase.ExpectedQuantity, recipeGroceryItem.Quantity );
        Assert.Equal( testCase.ExpectedFraction, recipeGroceryItem.MeasurementFraction );
        Assert.Equal( testCase.ExpectedMeasurement, recipeGroceryItem.MeasurementType );
        Assert.Equal( testCase.ExpectedName, recipeGroceryItem.GroceryItem.Name );
        Assert.Equal( testCase.AdditionalDetail, recipeGroceryItem.AdditionalDetail );
    }

    public record GroceryItemParseTestCase(
        string Input,
        int ExpectedQuantity,
        MeasurementFraction ExpectedFraction,
        MeasurementType ExpectedMeasurement,
        string AdditionalDetail,
        string ExpectedName
    )
    {
        public override string ToString() => $"\"{Input}\" → {ExpectedQuantity} {ExpectedFraction} {ExpectedMeasurement}";
    }
}
