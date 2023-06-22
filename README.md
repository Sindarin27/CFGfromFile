# CFGFromFile

Generate a Control Flow Graph from a simple-to-write text document. Gives direct statistics as well as Mermaid graph code.

## Input

Input should consist of two types of lines: `start [name]`, written when the function `[name]` is called, and `return [name]`, written when the function `[name]` returns. Lines not starting with `start` or `return` will be ignored, as well as any additional information after the name of the function.

### Example input
```
start funcA
start funcB
return
start funcB
return
start funcC
start funcB
return
start funcD
return
return
start funcB
return
return
```
### Example output
```
Total: 7 function calls
funcA: called 1 times (14.29%)
funcB: called 4 times (57.14%)
   by funcA: 3 times (75.00%)
   by funcC: 1 times (25.00%)
funcC: called 1 times (14.29%)
   by funcA: 1 times (100.00%)
funcD: called 1 times (14.29%)
   by funcC: 1 times (100.00%)
----- MERMAID START -----
flowchart TD
   funcA -->|75.00%| funcB
   funcC -->|25.00%| funcB
   funcA -->|100.00%| funcC
   funcC -->|100.00%| funcD
------ MERMAID END ------
```