using System.Collections;

var guid = Guid.NewGuid();
Console.WriteLine(guid.ToString());

Console.WriteLine();
Console.WriteLine("GetEnvironmentVariables: ");
foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
    Console.WriteLine("  {0} = {1}", de.Key, de.Value);

