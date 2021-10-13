
using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static System.Console;
using analyser.Model;
using analyser.Service;

namespace analyser
{
   
    class Program
    {
        static void Main(string[] args)
        { 
            ValidateArgs(args)
                .Bind(input=>GetService(input))
                .Bind(t => t.service.FindUsers(t.input))
                .Match(
                    Succ:s => WriteLine(String.Join(',', s)),
                    Fail:f => WriteLine(f.Message)
                );
        }

        static Try<Input> ValidateArgs(string[] args) =>()=>{
            if (args.Length < 3) return failwith<Input>("Not sufficient args");
            var modeStr = args[0].ToLower();
            Mode mode = modeStr == "active" ? Mode.Active : modeStr == "superactive" ? Mode.SuperActive : Mode.Bored;
            return new Input(mode, DateTime.Parse(args[1]), DateTime.Parse(args[2]));
        };
        
        static Try<(Input input, IAnalyserService service)> GetService(Input input)=> () => (input, new AnalyserService());
        
    }
}
