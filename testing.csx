using System.Collections;

// var guid = Guid.NewGuid();
// Console.WriteLine(guid.ToString());

// Console.WriteLine();
// Console.WriteLine("GetEnvironmentVariables: ");
// foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
//     Console.WriteLine("  {0} = {1}", de.Key, de.Value);


string target = "this,is,the,target";
string comma = "test,is,test,target".ToLower().Trim();
bool isFour = comma.Split(',').Length == 4;

var attempt = comma.Split(',');
var targetArr = target.Split(',');
var count = attempt.Zip(targetArr, (ta, att) => ta == att).Aggregate(0, (result, value) => value ? result + 1 : result);
Console.WriteLine("matches: " + count);
var output = attempt.Zip(targetArr, (ta, att) => ta == att ? ta : "*****");
output.ToList().ForEach(x => Console.WriteLine(x));
