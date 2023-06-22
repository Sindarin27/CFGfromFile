// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

string textFile = "input.txt";
Dictionary<string, int> funcToId = new Dictionary<string, int>();
List<string> idToFunc = new List<string>();
int funcCount = 0;

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
using (StreamReader file = new StreamReader(textFile))
{
    while ((line = file.ReadLine()) != null)
    {
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
                callstack.Pop();
                break;
        }
    }

    file.Close();
}

// Write stats
Console.WriteLine($"Total: {totalCalls} function calls");
for (int i = 0; i < funcCount; i++)
{
    int called = callCounter[i];
    Console.WriteLine($"{idToFunc[i]}: called {called} times ({(float)called*100/totalCalls:F2}%)");
    for (int j = 0; j < funcCount; j++)
    {
        int calls = calledBy[i, j];
        if (calls == 0) continue;
        Console.WriteLine($"   by {idToFunc[j]}: {calls} times ({(float)calls*100/called:F2}%)");
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