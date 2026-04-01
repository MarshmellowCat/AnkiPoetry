using System.Text;

namespace AnkiPoetry.Engine;



// *************************EVERYTHING BELOW IS THE IPA EDITED PAGE CREATOR****************


public class IPACreator : BaseCreator<Card>
{
    
    protected override IEnumerable<Card> CardFromChunk(Chunk chunk, Parameters parameters)


    {

        
        for (var i = 0; i < chunk.Lines.Length - 1; ++i)
        {
            var to = chunk.Lines[i + 1];

            if (!to.NotMy && to.LineType != LineType.NextPage)
            {
                //Claire: added "i" to designate card as IPA
                var number = CreateSortFieldText(chunk, parameters) + " - " + CreateNumber(chunk.MaxSongNumber, chunk.SectionNumber, chunk.SongNumber, to.LineNumber) + " - ipa";


                var beginning = CreateHeader(chunk, parameters) + JoinLines(chunk.Lines[..(i + 0)], parameters);
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
