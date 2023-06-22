// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

string textFile = "input.txt";
Dictionary<string, int> funcToId = new Dictionary<string, int>();
List<string> idToFunc = new List<string>();
int funcCount = 0;
int linesHandledNames = 0;

int GetIdOrAdd(string name)
{
    if (funcToId.TryGetValue(name, out int id)) return id;
    id = funcToId.Count;
    funcToId.Add(name, id);
    idToFunc.Add(name);
    funcCount++;
    return id;
}

string? line;
// Read the file once to get all the function names
using (StreamReader file = new StreamReader(textFile))
{
    while ((line = file.ReadLine()) != null)
    {
        if (++linesHandledNames % 1000000 == 0) Console.WriteLine($"Names checked: {linesHandledNames/1000000}M lines");
        string[] parts = line.Split();
        switch (parts[0])
        {
            case "start":
                GetIdOrAdd(parts[1]);
                break;
        }
    }

    file.Close();
}

// Read the file again now that we know stuff
int totalCalls = 0;
int[] callCounter = new int[funcCount];
int[,] calledBy = new int[funcCount, funcCount];
Stack<int> callstack = new Stack<int>();
callstack.Push(-1);
int linesTracked = 0;
using (StreamReader file = new StreamReader(textFile))
{
    while ((line = file.ReadLine()) != null)
    {
        if (++linesTracked % 1000000 == 0) Console.WriteLine($"Names checked: {linesTracked/1000000}M/{linesHandledNames/1000000}M lines");
        string[] parts = line.Split();
        switch (parts[0])
        {
            case "start":
                int id = funcToId[parts[1]];
                callCounter[id]++;
                int caller = callstack.Peek();
                if (caller != -1) calledBy[id, caller]++;
                callstack.Push(id);
                totalCalls++;
                break;
            case "return":
                int expected = callstack.Pop();
                int returner = funcToId[parts[1]];
                if (expected != returner) Console.Error.WriteLine($"Expected return from {idToFunc[expected]}, instead got {parts[1]}!");
                break;
        }
    }

    file.Close();
}

if (callstack.Count != 1) Console.Error.WriteLine($"Callstack not empty: {callstack.Count - 1} items still on callstack!");

Console.WriteLine("Finished reading! Output:");

// Write stats
Console.WriteLine($"Total: {totalCalls} function calls");
for (int i = 0; i < funcCount; i++)
{
    int called = callCounter[i];
    Console.WriteLine($"{idToFunc[i]}: called {called} times ({(float)called*100/totalCalls:F4}%)");
    for (int j = 0; j < funcCount; j++)
    {
        int calls = calledBy[i, j];
        if (calls == 0) continue;
        Console.WriteLine($"   by {idToFunc[j]}: {calls} times ({(float)calls*100/called:F3}%)");
    }
}

// Generate Mermaid graph
Console.WriteLine("----- MERMAID START -----");
Console.WriteLine("flowchart TD");
for (int i = 0; i < funcCount; i++)
{
    int called = callCounter[i];
    for (int j = 0; j < funcCount; j++)
    {
        int calls = calledBy[i, j];
        if (calls == 0) continue;
        Console.WriteLine($"   {idToFunc[j]} -->|{(float)calls*100/called:F2}%| {idToFunc[i]}");
    }
}

Console.WriteLine("------ MERMAID END ------");