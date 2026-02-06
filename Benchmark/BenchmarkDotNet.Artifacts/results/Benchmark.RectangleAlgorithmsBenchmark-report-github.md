```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.3 (24G419) [Darwin 24.6.0]
Apple M2 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 8.0.415
  [Host]     : .NET 8.0.21 (8.0.21, 8.0.2125.47513), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 8.0.21 (8.0.21, 8.0.2125.47513), Arm64 RyuJIT armv8.0-a


```
| Method                       | GridSize | Mean         | Error     | StdDev    | Gen0    | Gen1   | Allocated |
|----------------------------- |--------- |-------------:|----------:|----------:|--------:|-------:|----------:|
| **PrefixRectangleAlgorithmTest** | **4**        |     **474.8 ns** |   **1.47 ns** |   **1.15 ns** |  **0.1192** |      **-** |    **1000 B** |
| OptimizedExpansionTest       | 4        |     197.0 ns |   0.51 ns |   0.47 ns |  0.0238 |      - |     200 B |
| HistogramStackTest           | 4        |     854.4 ns |   1.34 ns |   1.19 ns |  0.3471 |      - |    2904 B |
| OptimizedBranchingTest       | 4        |     228.9 ns |   1.42 ns |   1.33 ns |  0.0219 |      - |     184 B |
| **PrefixRectangleAlgorithmTest** | **7**        |   **2,297.8 ns** |  **11.55 ns** |  **10.24 ns** |  **0.4654** |      **-** |    **3912 B** |
| OptimizedExpansionTest       | 7        |   1,032.4 ns |   1.57 ns |   1.47 ns |  0.0534 |      - |     448 B |
| HistogramStackTest           | 7        |   4,671.6 ns |   9.37 ns |   8.77 ns |  1.5259 |      - |   12776 B |
| OptimizedBranchingTest       | 7        |     896.0 ns |   1.95 ns |   1.82 ns |  0.0477 |      - |     400 B |
| **PrefixRectangleAlgorithmTest** | **20**       | **105,065.5 ns** | **438.36 ns** | **366.05 ns** | **19.4092** |      **-** |  **163288 B** |
| OptimizedExpansionTest       | 20       |  35,399.0 ns | 132.47 ns | 123.91 ns |  0.3052 |      - |    2888 B |
| HistogramStackTest           | 20       | 206,950.7 ns | 423.36 ns | 396.02 ns | 77.1484 | 0.9766 |  647200 B |
| OptimizedBranchingTest       | 20       |  33,094.5 ns | 113.66 ns | 106.32 ns |  0.2441 |      - |    2488 B |
