/*
namespace AnkiPoetry.Engine;

public class LineCreator : BaseCreator<Card>
{
    protected override IEnumerable<Card> CardFromChunk(Chunk chunk, Parameters parameters)
    {
        for (var i = 0; i < chunk.Lines.Length - 1; ++i)
        {
            var to = chunk.Lines[i + 1];

            if (!to.NotMy && to.LineType != LineType.NextPage)
            {
                //Claire updating to CreateSortFieldText
                var number = CreateSortFieldText(chunk, parameters) + " - " + CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, to.LineNumber) + " - L";
                //var number = CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, to.LineNumber);

                //var beginning = CreateHeader(chunk, parameters) + JoinLines(chunk.Lines[..(i - 1)], parameters);
                var beginning = CreateHeader(chunk, parameters);

               // if (to.IsFirst)
                   // beginning += "<hr>";

                // var ending = to.IsLast ? "<hr>" : "";
                var ending = "";

                var card = CreateCard(number, beginning, ending, to, parameters);

                yield return card;
            }
        }
    }

    protected Card CreateCard(string number, string beginning, string ending, MyLine to, Parameters parameters)
    {
        var text = MakeCloze(to.Text);
        var cloze = AddLineNumber(to, text, parameters);
        return new(number, beginning + cloze + ending);
    }

    private static string MakeCloze(string text)
    {
        var matches = Regexes.RegexWord().Matches(text);
        var n = matches[0].Index + 1;
        return $"{{{{c1::{text} :: {text[0..n]} }}}}";
    }
}
*/ 

/* ABOVE JUST RETURNS A SINGLE LINE WITH NO CONTEXT */

/* BELOW IS THE ORIGINAL LINE CREATOR */

using System.Text;

namespace AnkiPoetry.Engine;

public class LineCreator : BaseCreator<Card>
{
    protected override IEnumerable<Card> CardFromChunk(Chunk chunk, Parameters parameters)
    {
        for (var i = 0; i < chunk.Lines.Length - 1; ++i)
        {
            var to = chunk.Lines[i + 1];

            if (!to.NotMy && to.LineType != LineType.NextPage)
            {
                
                var number = CreateSortFieldText(chunk, parameters) + " - " + CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, to.LineNumber) + " - w";
                //var number = CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, to.LineNumber);


                var beginning = CreateHeader(chunk, parameters) + FirstWordJoinLines(chunk.Lines[..(i + 1)], parameters);
                // var beginning = CreateHeader(chunk, parameters);

               // if (to.IsFirst)
                   // beginning += "<hr>";

                // var ending = to.IsLast ? "<hr>" : "";
                var ending = "";

                var card = CreateCard(number, beginning, ending, to, parameters);

                yield return card;
            }
        }
    }

    protected string FirstWordJoinLines(MyLine[] list, Parameters parameters)
    {
        var sb = new StringBuilder();
        var i = 0;

        foreach (var line in list)
        {
            var text = GetLineText(line.Text, line, parameters);
            if (i == 0)
            {
                text = FirstWordGetLineText(line.Text, line, parameters);
            }

            //if (line.IsFirst)
            //sb.Append("<hr>");

            sb.Append(text);

            //if (line.IsLast)
            // sb.Append("<hr>");
            i++;
        }

        return sb.ToString();
    }

    protected string FirstWordGetLineText(string text, MyLine line, Parameters parameters)
        => FirstWordAddLineNumber(line, text, parameters);
    
    protected string FirstWordAddLineNumber(MyLine line, string text, Parameters parameters)
    {
        var number = parameters.LineNumbers
            ? $"{(parameters.Continuous ? line.ContinuousNumber : line.LineNumber),3}. "
            : "";

        return "<div>" + number + text + "</div>"; //CLAIRE

        // Commented out to stop the line color br
        // return ColorLine(
        //     number + text,
        //     (line.LineNumber - 1), //to make first (zero) line violet not red
        //     parameters.Colors);
    }


    // below code affects real line
    protected Card CreateCard(string number, string beginning, string ending, MyLine to, Parameters parameters)
    {
        var text = MakeCloze(to.Text);
        var cloze = AddLineNumber(to, text, parameters);
        return new(number, beginning + cloze + ending);
    }

    private static string MakeCloze(string text)
    {
        var matches = Regexes.RegexWord().Matches(text);
        var n = matches[0].Index + 1;
        return $"{{{{c1::{text} :: {text[0..n]} }}}}";
    }
}
