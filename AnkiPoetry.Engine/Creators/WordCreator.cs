﻿

/*

using System.Text;
using System.Text.RegularExpressions;

namespace AnkiPoetry.Engine;

public partial class WordCreator : BaseCreator<Card>
{
    protected override IEnumerable<Card> CardFromChunk(Chunk chunk, Parameters parameters)
    {
        for (var i = 1; i < chunk.Lines.Length; ++i)
        {
            var to = chunk.Lines[i];

            // if (i == 0)
            // {
            //     var number = CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, to.LineNumber);
            //     var beginning = CreateHeader(chunk, parameters);
            //     var ending = "";
            //     var nextLine = "";
            //     var card = CreateCard(number, beginning, ending, to, nextLine, parameters);

            //     yield return card;
            // }
            
            if (!to.NotMy && to.LineType != LineType.NextSong && to.LineType != LineType.NextPage)
            {
                var number = CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, to.LineNumber);

                // var beginning = CreateHeader(chunk, parameters) + JoinLines(chunk.Lines[..i], parameters);
                var beginning = CreateHeader(chunk, parameters);// + GetLineText(chunk.Lines[i].Text, chunk.Lines[i], parameters);

                // if (to.IsFirst)
                //     beginning += "<hr>";

                // var ending = to.IsLast ? "<hr>" : "";
                var ending = "";

                var nextLine = i < chunk.Lines.Length - 1
                    ? chunk.Lines[i + 1]
                    : null;

                var card = CreateCard(number, beginning, ending, to, nextLine, parameters);

                yield return card;
            }
        }
    }

    protected Card CreateCard(string number, string beginning, string ending, MyLine line, MyLine? lineNext, Parameters parameters)
    {
        var cloze_num = 1;
        var cloze = MakeCloze(line.Text, ref cloze_num);
        var formatted = GetLineText(line.Text, line, parameters);
        // var formatted = "";

        if (lineNext is not null && lineNext.Text != "" && !lineNext.NotMy)
        {
            var clozeNext = MakeClozeFirstWord(lineNext.Text, ref cloze_num);
            var formattedNext = GetLineText(clozeNext, lineNext, parameters);

            ending += formattedNext;

            //if (lineNext.IsLast)
                //ending += "<hr>";
        }
        
        // return new(number, beginning + line.Text + ending);
        return new(number, beginning + formatted + ending);
        // return new(number, beginning + ending);
    }

    private static string MakeCloze(string text, ref int cloze_num)
    {
        var matches = Regexes.RegexWord().Matches(text);
        var sb = new StringBuilder();
        var last_word_end = 0;

        foreach (var match in matches.Cast<Match>())
        {
            sb.Append(text[last_word_end..match.Index]);
            sb.Append($"{{{{c{cloze_num}::{match.Value}}}}}");

            cloze_num++; // cloze_num += 1, new cloze_num = prev. cloze_num + 1

            last_word_end = match.Index + match.Length;
        }

        sb.Append(text[last_word_end..]);

        return sb.ToString();
    }

    private static string MakeClozeFirstWord(string text, ref int cloze_num)
    {
        var matches = Regexes.RegexWord().Matches(text);
        var sb = new StringBuilder();

        if (matches.Count > 0)
        {
            var i = 0;
            var word = "";









            //CG0419 This changes the allowed characters limit when truncating words.. Gonna try to change to 5 and see what happens
            //CG0419 5 seems a good number!
            //CG0509 trying 6!
            //CG0607 trying 15!
            //CG0609 trying 20!
            //CG0614 going back to 6!
            //CG0614 going back to 15!
            
            while (i < matches.Count && word.Count(char.IsLetter) < 15)
            {
                var match = matches[i];
                word = text[0..(match.Index + match.Length)];
                i++;
            }

            if (i < matches.Count)
                word += "...";

            sb.Append($"{{{{c{cloze_num}::{word}:: w }}}}");

            cloze_num++;
        }

        return sb.ToString();
    }
}


***********************************************************************************

*/


//20250706 what follows is code from the line creator, just renamed to WordCreator


using System.Text;

namespace AnkiPoetry.Engine;

public class WordCreator : BaseCreator<Card>
{
    protected override IEnumerable<Card> CardFromChunk(Chunk chunk, Parameters parameters)
    {
        for (var i = 0; i < chunk.Lines.Length - 1; ++i)
        {
            var to = chunk.Lines[i + 1];

            if (!to.NotMy && to.LineType != LineType.NextPage)
            {
                var number = CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, to.LineNumber);


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

        return "<div>" + number + "... " + text + "</div>"; //CLAIRE

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
        return $"{{{{c1::{text} ... :: w }}}}";
    }
}
