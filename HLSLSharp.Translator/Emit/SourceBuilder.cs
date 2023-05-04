using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HLSLSharp.Translator.Emit;
internal class SourceBuilder
{
    private StringBuilder CurrentLine = new StringBuilder(60);

    private bool CurrentLineEmpty = true;

    private List<string> Lines = new List<string>(15);

    public void Write(string value)
    {
        CurrentLine.Append(value);

        CurrentLineEmpty = false;
    }

    public void WriteLine(string value) 
    {
        CurrentLine.Append(value);

        CurrentLineEmpty = false;

        Lines.Add(CurrentLine.ToString());

        CurrentLine.Clear();

        CurrentLineEmpty = true;
    }

    public void Concat(IEnumerable<string> lines)
    {
        Lines.AddRange(lines);
    }

    public IEnumerable<string> GetLines()
    {
        if (CurrentLineEmpty)
        {
            return Lines;
        }
        else
        {
            return Lines.Append(CurrentLine.ToString());
        }
    }

    public override string ToString()
    {
        string combined = string.Join("\n", GetLines());

        if (Lines.Count > 0 && CurrentLineEmpty)
        {
            combined += "\n";
        }

        return combined;
    }
}
