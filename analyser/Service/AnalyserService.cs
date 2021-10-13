using System;
using System.Collections.Generic;
using analyser.Model;
using LanguageExt;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace analyser.Service
{
    public class AnalyserService : IAnalyserService
    { 
        Try<IEnumerable<string>> IAnalyserService.FindUsers(Input input) => () =>
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = baseDir.Substring(0, baseDir.IndexOf("bin/")) + "data";
            return Directory.GetFiles(path)
                .Filter(file => file.EndsWith(".json"))
                .Map(filePath =>{
                    var id = Path.GetFileName(filePath).Replace(".json","");
                    var dates = GetMealDates(filePath);
                
                    return input.Mode switch
                    {
                        Mode.Active =>  IsActive(input, dates) ? id : "",
                        Mode.SuperActive => IsSuperActive(input, dates) ? id : "",
                        Mode.Bored => IsBored(input, dates) ? id : "",
                        _=>""
                    };
               
                })
                .Filter(el=>!String.IsNullOrEmpty(el))
                .ToList();
            
        };
        int Compare(DateTime d1, DateTime d2) => DateTime.Compare(d1, d2);

        bool IsActive(Input input, IEnumerable<DateTime> dts) => dts.Filter(dt => Compare(dt, input.StartState) >= 0 && Compare(dt, input.EndDate) <= 0).Count()>=5;

        bool IsSuperActive(Input input, IEnumerable<DateTime> dts) => dts.Filter(dt => Compare(dt, input.StartState) >= 0 && Compare(dt, input.EndDate) <= 0).Count() > 10;

        bool IsBored(Input input, IEnumerable<DateTime> dts){
            var state = dts.Fold((0, 0), (acc, dt) =>
            {
                if(Compare(dt, input.StartState) < 0)
                {
                    acc.Item1++;
                }
                else if (Compare(dt, input.StartState) >= 0 && Compare(dt, input.EndDate) <= 0)
                {
                    acc.Item2++;
                }
                return acc;
            });
            return state.Item1 >= 5 && state.Item2 < 5;
        }
       
        IEnumerable<DateTime> GetMealDates(string filePath)
        {
            using (JsonDocument document = JsonDocument.Parse(File.ReadAllText(filePath)))
            {
                var details = document.RootElement.GetProperty("calendar").GetProperty("daysWithDetails");
                return details.EnumerateObject()
                    .Map(prop => details.GetProperty(prop.Name))
                    .Map(el => el.GetProperty("day"))
                    .Map(el => el.GetProperty("date"))
                    .Map(el => el.GetDateTime()).ToList();
            }
        }
        
    }
}
