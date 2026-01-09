
using HomeFlow.Extensions;
using HomeFlow.Features.MealPlanning.GroceryItems;
using HomeFlow.Interfaces;

namespace HomeFlow.Features.MealPlanning.Recipes;

public class RecipeGroceryItem : IOrderable
{
    public Guid Id { get; set; }

    public int Quantity { get; set; }

    public MeasurementFraction MeasurementFraction { get; set; } = MeasurementFraction.None;

    public MeasurementType MeasurementType { get; set; } = MeasurementType.None;

    public string AdditionalDetail { get; set; } = string.Empty;

    public int Order { get; set; }

    public GroceryItem GroceryItem { get; set; } = new GroceryItem();

    public string Text
    {
        get => ToString();
    }

    public void ExtractDetailsFromText( string text )
    {
        // Remove any extra notes in parentheses from the text
        var extraNotes = StringExtensions.ExtractTextInParanthesis( ref text );

        var stringParts = text.Replace( ',', ' ' ).Split( ' ' ).ToList();

        // start by extracting a unit count
        for ( int i = 0; i < stringParts.Count(); i++ )
        {
            if ( int.TryParse( stringParts[i], out int quantity ) )
            {
                Quantity = quantity;
                stringParts.RemoveAt( i );
                break;
            }
        }

        // next, try to extract a measurement fraction
        for ( int i = 0; i < stringParts.Count(); i++ )
        {
            MeasurementFractionLookup.TryGetValue( stringParts[i], out MeasurementFraction fraction );
            if ( fraction != MeasurementFraction.None )
            {
                MeasurementFraction = fraction;

                stringParts.RemoveAt( i );
                break;
            }
        }

        // next, extract a measurement type
        for ( int i = 0; i < stringParts.Count(); i++ )
        {
            var input = stringParts[i].ToLower().Replace( '.', ' ' ).Trim();

            MeasurementTypeLookup.TryGetValue( input, out MeasurementType type );
            if ( type != MeasurementType.None )
            {
                MeasurementType = type;

                stringParts.RemoveAt( i );
                break;
            }
        }

        // next, parse special phrases such as "finely chopped"
        AdditionalDetail = string.Empty;
        for ( int i = 0; i < stringParts.Count - 1; i++ )
        {
            string word1 = stringParts[i].ToLower();
            string word2 = stringParts[i + 1].ToLower();
            bool match = false;

            if ( DetailAdverbs.Contains( word1 ) && DetailVerbs.Contains( word2 ) )
            {
                AdditionalDetail = $"{word1} {word2}".ToTitleCase();
                stringParts.RemoveAt( i + 1 );
                stringParts.RemoveAt( i );
                break;
            }

            if ( DetailVerbs.Contains( word1 ) )
            {
                AdditionalDetail = word1.ToTitleCase();
                stringParts.RemoveAt( i );
                match = true;
            }

            if ( DetailVerbs.Contains( word2 ) )
            {
                AdditionalDetail = string.Concat( AdditionalDetail, match ? " " : "", word2.ToTitleCase() );

                if ( match )
                {
                    stringParts.RemoveAt( i );
                }
                else
                {
                    stringParts.RemoveAt( i + 1 );
                }

                match = true;
            }

            if ( match )
            {
                break;
            }
        }

        string sizingInfo = string.Empty;
        foreach ( var extraNote in extraNotes )
        {
            // check if any of the words below match a measurement type
            var words = extraNote.Split( ' ' );

            foreach ( var word in words )
            {
                if ( MeasurementTypeLookup.ContainsKey( word ) )
                {
                    sizingInfo = extraNote;
                    break;
                }
            }
        }

        // take the remaining string parts and join them to form the grocery item name
        string groceryItemName = string.Join( " ", stringParts );

        // add any extra notes to the ingredient name
        if ( !string.IsNullOrEmpty( sizingInfo ) )
        {
            groceryItemName += $" ({sizingInfo})";
        }

        groceryItemName = groceryItemName.ToTitleCase();

        GroceryItem.Name = groceryItemName;
    }

    public override string ToString()
    {
        string output = string.Empty;
        if ( Quantity > 0 )
        {
            output += $"{Quantity} ";
        }

        switch ( MeasurementFraction )
        {
            case MeasurementFraction.Half:
                output += "1/2 ";
                break;
            case MeasurementFraction.Quarter:
                output += "1/4 ";
                break;
            case MeasurementFraction.Third:
                output += "1/3 ";
                break;
            case MeasurementFraction.ThreeQuarters:
                output += "3/4 ";
                break;
            case MeasurementFraction.TwoThirds:
                output += "2/3 ";
                break;
            default:
                output += " ";
                break;
        }

        if ( MeasurementType != MeasurementType.None )
        {
            output += $"{MeasurementType} ";
        }

        output += $"{GroceryItem.Name}";

        if ( AdditionalDetail != string.Empty )
        {
            output += $", {AdditionalDetail}";
        }

        return output;
    }

    private static readonly Dictionary<string, MeasurementFraction> MeasurementFractionLookup = new()
    {
        ["1/2"] = MeasurementFraction.Half,
        ["½"] = MeasurementFraction.Half,
        ["1/3"] = MeasurementFraction.Third,
        ["⅓"] = MeasurementFraction.Third,
        ["1/4"] = MeasurementFraction.Quarter,
        ["¼"] = MeasurementFraction.Quarter,
        ["3/4"] = MeasurementFraction.ThreeQuarters,
        ["¾"] = MeasurementFraction.ThreeQuarters,
        ["2/3"] = MeasurementFraction.TwoThirds,
        ["⅔"] = MeasurementFraction.TwoThirds
    };

    private static readonly Dictionary<string, MeasurementType> MeasurementTypeLookup = new( StringComparer.OrdinalIgnoreCase )
    {
        // Teaspoons
        ["teaspoon"] = MeasurementType.Teaspoons,
        ["tsp"] = MeasurementType.Teaspoons,
        ["tsps"] = MeasurementType.Teaspoons,
        ["teaspoons"] = MeasurementType.Teaspoons,

        // Tablespoons
        ["tablespoon"] = MeasurementType.Tablespoons,
        ["tbsp"] = MeasurementType.Tablespoons,
        ["tbs"] = MeasurementType.Tablespoons,
        ["tbl"] = MeasurementType.Tablespoons,
        ["tablespoons"] = MeasurementType.Tablespoons,

        // Cups
        ["cup"] = MeasurementType.Cups,
        ["cups"] = MeasurementType.Cups,
        ["c"] = MeasurementType.Cups,

        // Pints
        ["pint"] = MeasurementType.Pints,
        ["pints"] = MeasurementType.Pints,
        ["pt"] = MeasurementType.Pints,

        // Quarts
        ["quart"] = MeasurementType.Quarts,
        ["quarts"] = MeasurementType.Quarts,
        ["qt"] = MeasurementType.Quarts,

        // Ounces
        ["ounce"] = MeasurementType.Ounces,
        ["ounces"] = MeasurementType.Ounces,
        ["oz"] = MeasurementType.Ounces,

        // Pounds
        ["pound"] = MeasurementType.Pounds,
        ["pounds"] = MeasurementType.Pounds,
        ["lb"] = MeasurementType.Pounds,
        ["lbs"] = MeasurementType.Pounds,

        // Grams
        ["gram"] = MeasurementType.Grams,
        ["grams"] = MeasurementType.Grams,
        ["g"] = MeasurementType.Grams,

        // Milliliters
        ["milliliter"] = MeasurementType.Milliliters,
        ["milliliters"] = MeasurementType.Milliliters,
        ["ml"] = MeasurementType.Milliliters,

        // Dash
        ["dash"] = MeasurementType.Dash,
        ["dashes"] = MeasurementType.Dash,

        // Whole
        ["whole"] = MeasurementType.Whole,
        ["each"] = MeasurementType.Whole,
        ["entire"] = MeasurementType.Whole,

        // Sizes
        ["small"] = MeasurementType.Small,
        ["medium"] = MeasurementType.Medium,
        ["large"] = MeasurementType.Large,

        // Containers
        ["bottle"] = MeasurementType.Bottle,
        ["bottles"] = MeasurementType.Bottle,

        // Can
        ["can"] = MeasurementType.Can,
        ["cans"] = MeasurementType.Can,

        // Package
        ["package"] = MeasurementType.Package,
        ["packages"] = MeasurementType.Package,
        ["pkg"] = MeasurementType.Package,
        ["pkgs"] = MeasurementType.Package,

        // Stalks
        ["stalk"] = MeasurementType.Stalks,
        ["stalks"] = MeasurementType.Stalks
    };

    private static readonly string[] DetailVerbs = new[]
{
    "chopped", "diced", "minced", "shredded", "sliced", "peeled", "cubed", "crushed", "ground", "mashed", "beaten", "grated", "zested", "cooked", "roasted", "baked", "blanched"
};

    private static readonly string[] DetailAdverbs = new[]
    {
    "finely", "roughly", "lightly", "coarsely", "thinly", "evenly", "loosely", "freshly"
};
}
