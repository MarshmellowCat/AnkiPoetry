

 
using System.Text;

namespace AnkiPoetry.Engine;

public partial class PageCreator : BaseCreator<Card>
{
    protected override IEnumerable<Card> CardFromChunk(Chunk chunk, Parameters parameters)
    {
        var from = chunk.Lines.First();

        var header = CreateHeader(chunk, parameters);
        var number = CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, from.LineNumber);

        // yield return CreateOddEven(chunk, parameters, header, number);
        yield return CreateFirstLetter(chunk, parameters, header, number);
        yield return CreateWhole(chunk, parameters, header, number);
    }

    private Card Create(Chunk chunk, Parameters parameters, string header, string number, string suffix, Func<int, MyLine, string> func)
    {
        var sb = new StringBuilder();
        sb.Append(header);

        foreach (var (index, line) in chunk.Lines.Index())
        {
            var inner_text = line.Text;

            if (!line.NotMy && !String.IsNullOrEmpty(line.Text))
                inner_text = func(index, line);

            var text = AddLineNumber(line, inner_text, parameters);
            sb.Append(AddHr(line, text));
        }

        return new(number + suffix, sb.ToString());
    }


//3
 private Card CreateWhole(Chunk chunk, Parameters parameters, string header, string number)
        => Create(chunk, parameters, header, number, "_whole",
            (index, line) => index > 0 ? MakeCloze(1, line.Text) : line.Text);

    
//2
    private Card CreateFirstLetter(Chunk chunk, Parameters parameters, string header, string number)
        => Create(chunk, parameters, header, number, "_initial",
            (index, line) => index > 0 ? MakeClozeLeaveFirstLetter(1, line.Text) : line.Text);

//1

private Card CreateOddEven(Chunk chunk, Parameters parameters, string header, string number)
        => Create(chunk, parameters, header, number, "_odd_even",
            (index, line) => MakeCloze((index % 2) == 0 ? 1 : 2, line.Text));



    private static string AddHr(MyLine line, string text) =>
       $"{(line.IsFirst ? "" : "")}{text}{(line.IsLast ? "" : "")}";

    private static string MakeCloze(int cloze_num, string text)
        => $"{{{{c{cloze_num}::{text}}}}}";

    private static string MakeClozeLeaveFirstLetter(int cloze_num, string text)
    {
        var matches = Regexes.RegexWord().Matches(text);
        var n = matches[0].Index + 1;
        return $"{{{{c{cloze_num}::{text} :: {text[0..n]} }}}}";


        // 1. A [A]

        //ABCDEFJ
    }

}





/*  ORIGINAL MakeClozeLeaveFirstLetter

private static string MakeClozeLeaveFirstLetter(int cloze_num, string text)
    {
        var matches = Regexes.RegexWord().Matches(text);
        var n = matches[0].Index + 1;
        return text[0..n] + $"{{{{c{cloze_num}::{text[n..]}}}}}";


        */





/*
*************************EVERYTHING ABOVE IS THE ORIGINAL PAGE CREATOR****************
*/







/*
*************************EVERYTHING BELOW IS THE IPA EDITED PAGE CREATOR****************
*/


/*

namespace AnkiPoetry.Engine;

public class PageCreator : BaseCreator<Card>
{
    

     
    protected override IEnumerable<Card> CardFromChunk(Chunk chunk, Parameters parameters)


    {

        
        for (var i = 0; i < chunk.Lines.Length - 1; ++i)
        {
            var to = chunk.Lines[i + 1];

            if (!to.NotMy && to.LineType != LineType.NextPage)
            {
                var number = CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, to.LineNumber);

                var beginning = CreateHeader(chunk, parameters) + JoinLines(chunk.Lines[..(i + 1)], parameters);
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
        // return $"{{text}}";
        return "<span class=\"text\">" + text + "</span>";
       //return text;
    }
}

         
*/