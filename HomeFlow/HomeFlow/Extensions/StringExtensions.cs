
using System.Text.RegularExpressions;

namespace HomeFlow.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase( this string str )
    {
        if ( string.IsNullOrEmpty( str ) )
        {
            return string.Empty;
        }

        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase( str.ToLower() ).Trim();
    }

    public static List<string> ExtractTextInParanthesis( ref string input )
    {
        var matches = Regex.Matches( input, @"\(([^)]+)\)" );

        var extraNotes = matches
            .Cast<Match>()
            .Select( m => m.Groups[1].Value.Trim() )
            .ToList();

        // Remove all parenthetical groups from the string
        input = Regex.Replace( input, @"\s*\([^)]+\)", "" ).Trim();

        return extraNotes;
    }

    public static string TruncateWords( this string input, int maxWords, string ellipsis = "..." )
    {
        if ( string.IsNullOrWhiteSpace( input ) )
            return input;

        var words = input.Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

        if ( words.Length <= maxWords )
            return input;

        return string.Join( " ", words.Take( maxWords ) ) + ellipsis;
    }

    public static string SplitPascalCase( this string input )
    {
        return Regex.Replace( input, "(\\B[A-Z])", " $1" );
    }
}
