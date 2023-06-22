# CFGFromFile

Generate a Control Flow Graph from a simple-to-write text document. Gives direct statistics as well as Mermaid graph code.

## Input

Input should consist of two types of lines: `start [name]`, written when the function `[name]` is called, and `return`, written when the current function returns.