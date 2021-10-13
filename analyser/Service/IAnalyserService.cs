using System.Collections.Generic;
using LanguageExt;
using analyser.Model;

namespace analyser.Service
{
    public interface IAnalyserService
    {
        Try<IEnumerable<string>> FindUsers(Input input);
    }
}
