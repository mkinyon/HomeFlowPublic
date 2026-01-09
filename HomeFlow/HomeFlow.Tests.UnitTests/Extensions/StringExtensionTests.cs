using HomeFlow.Extensions;

namespace HomeFlow.Tests.UnitTests.Extensions;

public class StringExtensionTests
{
    #region ToTitleCase

    [Fact]
    public void ToTitleCase_EmptyString_ReturnsEmptyString()
    {
        string input = string.Empty;
        string result = input.ToTitleCase();
        Assert.Equal( string.Empty, result );
    }

    [Fact]
    public void ToTitleCase_NullString_ReturnsEmptyString()
    {
        string? input = null;
        string result = input!.ToTitleCase();
        Assert.Equal( string.Empty, result );
    }

    [Fact]
    public void ToTitleCase_SingleWord_ReturnsCapitalizedWord()
    {
        string input = "hello";
        string result = input.ToTitleCase();
        Assert.Equal( "Hello", result );
    }

    [Fact]
    public void ToTitleCase_MultipleWords_ReturnsCapitalizedWords()
    {
        string input = "hello world";
        string result = input.ToTitleCase();
        Assert.Equal( "Hello World", result );
    }

    [Fact]
    public void ToTitleCase_LeadingAndTrailingSpaces_ReturnsTrimmedCapitalizedWords()
    {
        string input = "  hello world  ";
        string result = input.ToTitleCase();
        Assert.Equal( "Hello World", result );
    }

    #endregion

    #region ExtractTextInParanthesis

    [Fact]
    public void ExtractTextInParanthesis_EmptyString_ReturnsEmptyList()
    {
        string input = string.Empty;
        var result = StringExtensions.ExtractTextInParanthesis( ref input );
        Assert.Empty( result );
        Assert.Equal( string.Empty, input );
    }

    [Fact]
    public void ExtractTextInParanthesis_NoParentheses_ReturnsEmptyList()
    {
        string input = "Hello World";
        var result = StringExtensions.ExtractTextInParanthesis( ref input );
        Assert.Empty( result );
        Assert.Equal( "Hello World", input );
    }

    [Fact]
    public void ExtractTextInParanthesis_SingleParentheses_ReturnsExtractedText()
    {
        string input = "Hello (World)";
        var result = StringExtensions.ExtractTextInParanthesis( ref input );
        Assert.Single( result );
        Assert.Equal( "World", result[0] );
        Assert.Equal( "Hello", input );
    }

    [Fact]
    public void ExtractTextInParanthesis_MultipleParentheses_ReturnsAllExtractedTexts()
    {
        string input = "Hello (World) and (Universe)";
        var result = StringExtensions.ExtractTextInParanthesis( ref input );
        Assert.Equal( 2, result.Count );
        Assert.Equal( "World", result[0] );
        Assert.Equal( "Universe", result[1] );
        Assert.Equal( "Hello and", input );
    }

    #endregion

    #region TruncateWords

    [Fact]
    public void TruncateWords_EmptyString_ReturnsEmptyString()
    {
        string input = string.Empty;
        string result = input.TruncateWords( 5 );
        Assert.Equal( string.Empty, result );
    }

    [Fact]
    public void TruncateWords_SingleWordLessThanMax_ReturnsOriginalString()
    {
        string input = "Hello";
        string result = input.TruncateWords( 5 );
        Assert.Equal( "Hello", result );
    }

    [Fact]
    public void TruncateWords_SingleWordEqualToMax_ReturnsOriginalString()
    {
        string input = "Hello";
        string result = input.TruncateWords( 1 );
        Assert.Equal( "Hello", result );
    }

    [Fact]
    public void TruncateWords_MultipleWordsLessThanMax_ReturnsOriginalString()
    {
        string input = "Hello World";
        string result = input.TruncateWords( 5 );
        Assert.Equal( "Hello World", result );
    }

    [Fact]
    public void TruncateWords_MultipleWordsGreaterThanMax_ReturnsTruncatedString()
    {
        string input = "Hello World";
        string result = input.TruncateWords( 1, "..." );
        Assert.Equal( "Hello...", result );
    }

    #endregion

    #region SplitPascalCase

    [Fact]
    public void SplitPascalCase_EmptyString_ReturnsEmptyString()
    {
        string input = string.Empty;
        string result = input.SplitPascalCase();
        Assert.Equal( string.Empty, result );
    }

    [Fact]
    public void SplitPascalCase_SingleWord_ReturnsSameWord()
    {
        string input = "Hello";
        string result = input.SplitPascalCase();
        Assert.Equal( "Hello", result );
    }

    [Fact]
    public void SplitPascalCase_TwoWords_ReturnsSeparatedWords()
    {
        string input = "HelloWorld";
        string result = input.SplitPascalCase();
        Assert.Equal( "Hello World", result );
    }

    [Fact]
    public void SplitPascalCase_MultipleWords_ReturnsSeparatedWords()
    {
        string input = "HelloWorldFromHomeFlow";
        string result = input.SplitPascalCase();
        Assert.Equal( "Hello World From Home Flow", result );
    }

    #endregion
}
