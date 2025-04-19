using System.Text;

//Hello world. Claire here. I'm learning how VSC talks to Github.

namespace AnkiPoetry.Engine;

public abstract class BaseCreator<T>
{
    protected abstract IEnumerable<T> CardFromChunk(Chunk chunk, Parameters parameters);

    public T[] Run(Chunk[] chunks, Parameters parameters)
    {
        if (parameters.Colors <= 0)
            parameters.Colors = 0;
//changed colors parameters to be able to be zero; made it stop automatically changing 0 to 1
// ugh when colors is set to 0 the GUI returns an error; need to investigate
        return chunks.SelectMany(a => CardFromChunk(a, parameters)).ToArray();
    }

    protected string JoinLines(MyLine[] list, Parameters parameters)
    {
        var sb = new StringBuilder();

        foreach (var line in list)
        {
            var text = GetLineText(line.Text, line, parameters);

            if (line.IsFirst)
                sb.Append("<hr>");

            sb.Append(text);

            if (line.IsLast)
                sb.Append("<hr>");
        }

        return sb.ToString();
    }

    protected virtual string CreateNumber(int maxSongNumber, int sectionNumber, int songNumber, int lineNumber)
        => $"{sectionNumber:00}.{songNumber:00}.{lineNumber:000}";

    protected static string CreateHeader(Chunk chunk, Parameters parameters)
    {

        //made starsHtmL always have two breaks even if StarsMode isn't on - just for safety
        var starsHtml = "<br><br>";

        if (parameters.StarMode != StarMode.None)
        {
            var total = 5;

            var num = parameters.StarMode switch
            {
                StarMode.PerChunk => chunk.ChunkNumber,
                StarMode.PerSection => chunk.SongNumber - 1, //TODO: make song number from zero
                StarMode.PerSong => chunk.ScreenNumber,
                _ => throw new NotImplementedException(),
            };

            var stars = num % total;
            var color = (num / total) % parameters.Colors;

            var starsText = String.Join("", Enumerable.Range(0, total).Select(a => a == stars ? "★" : "☆"));

            starsHtml = $" <span class=\"line{color}\">{starsText}</span>";
        }
        
        // made the starsHtml always just be two breaks; no more stars yay!
        starsHtml = "<br><br>";
        
        return $"<div class=\"header\">{chunk.Header}{starsHtml}</div>";
    }

    //C# operator % returns negative numbers for negative x, so we need to adjust result
    static int Mod(int x, int m)
    {
        var r = x % m;
        return r < 0 ? r + m : r;
    }

    static string ColorLine(string text, int n, int colors)
        => $"<div class=\"line{Mod(n, colors)}\">" + text + "</div>";

    protected string GetLineText(string text, MyLine line, Parameters parameters)
        => AddLineNumber(line, text, parameters);

    protected string AddLineNumber(MyLine line, string text, Parameters parameters)
    {
        var number = parameters.LineNumbers
            ? $"{(parameters.Continuous ? line.ContinuousNumber : line.LineNumber),3}. "
            : "";

        return ColorLine(
            number + text,
            (line.LineNumber - 1), //to make first (zero) line violet not red
            parameters.Colors);
    }
}
