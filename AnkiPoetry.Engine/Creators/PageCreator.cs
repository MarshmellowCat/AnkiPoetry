using System.Text;

namespace AnkiPoetry.Engine;

public partial class PageCreator : BaseCreator<Card>
{
    protected override IEnumerable<Card> CardFromChunk(Chunk chunk, Parameters parameters)
    {
        var from = chunk.Lines.First();

        var header = CreateHeader(chunk, parameters);
        var number = CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, from.LineNumber);

        yield return CreateOddEven(chunk, parameters, header, number);
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
       $"{(line.IsFirst ? "<hr>" : "")}{text}{(line.IsLast ? "<hr>" : "")}";

    private static string MakeCloze(int cloze_num, string text)
        => $"{{{{c{cloze_num}::{text}}}}}";

    private static string MakeClozeLeaveFirstLetter(int cloze_num, string text)
    {
        var matches = Regexes.RegexWord().Matches(text);
        var n = matches[0].Index + 1;
        return $"{{{{c{cloze_num}::{text[n..]}::{text[0..n]}}}}}";


        // 1. A [A]

        //ABCDEFJ
    }
}/*  ORIGINAL MakeClozeLeaveFirstLetter

private static string MakeClozeLeaveFirstLetter(int cloze_num, string text)
    {
        var matches = Regexes.RegexWord().Matches(text);
        var n = matches[0].Index + 1;
        return text[0..n] + $"{{{{c{cloze_num}::{text[n..]}}}}}";


        */