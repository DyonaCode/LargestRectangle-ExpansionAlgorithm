# LargestRectangles

Deterministic experiments for translating binary grids into non-overlapping rectangles.

Popular methods include `StackHistogram` and `Prefix`. This repo intends to propose a new solution to this 
problem, an 'Expansion' algorithm. 

This algorithm:
- Maintains a working grid (1D byte array) representing available cells
- Builds prefix arrays containing consecutive 1s count from right to left for each row
- For each potential top-left corner, expands rectangles downward using prefix data
- Greedily selects the largest area rectangle found
- Updates working grid and incrementally rebuilds affected prefix rows
- Continues until no valid rectangles remain

## Algorithmic Comparison

- `PrefixRectangleAlgorithm`: readable baseline that rebuilds row prefixes every iteration.
- `StackHistogramAlgorithm`: readable largest-rectangle finder using the histogram stack scan.
- `OptimizedStackHistogramAlgorithm`: hyper-optimized implementation of the stack-histogram baseline.
- `ExpansionRectangleAlgorithm`: readable version of the incremental expansion approach.
- `OptimizedPrefixAlgorithm`: hyper-optimized implementation of the prefix baseline.
- `OptimizedExpansionAlgorithm`: hyper-optimized implementation of the expansion approach.

## Run

- `dotnet test`
- `dotnet run --project Benchmark -c Release`
- `dotnet run --project LeastRectangles`

## Benchmarks

`ReferenceRectangleBenchmarks` in [`Benchmark/Program.cs`](Benchmark/Program.cs). 

- It compares readable implementations only for apple-to-apple comparisons.
- `OptimizedRectangleBenchmarks` compares the optimized prefix, stack, and expansion implementations directly.
- Optimized variants are benchmarked separately in `PrefixImplementationBenchmarks` and 
`ExpansionImplementationBenchmarks`.
- Stack variants are benchmarked separately in `StackImplementationBenchmarks`.
- All comparable algorithms use the same canonical tie-break: largest area, then top-most, 
then left-most, then shorter height.
- Benchmark setup validates exact output parity before timing begins.
- Random benchmark grids are generated from fixed seeds, and the structured `Rooms` 
scenario is deterministic.

### Benchmark Results 

Hardware: Apple M2 Pro Macbook - 16GB RAM. 

Initial findings seem to suggest that on smaller grid sizes the Expansion algorithm outperforms its competitors. 

| Method         | Size | Scenario | Mean          | Error        | StdDev       | Ratio | RatioSD | Gen0       | Gen1     | Gen2     | Allocated    | Alloc Ratio |
|--------------- |----- |--------- |--------------:|-------------:|-------------:|------:|--------:|-----------:|---------:|---------:|-------------:|------------:|
| Expansion      | 32   | Random35 |     242.76 us |     2.210 us |     1.959 us |  0.48 |    0.01 |     2.4414 |        - |        - |      20.3 KB |        0.03 |
| Prefix         | 32   | Random35 |     505.02 us |     5.114 us |     4.533 us |  1.00 |    0.01 |    96.6797 |   0.9766 |        - |     795.8 KB |        1.00 |
| StackHistogram | 32   | Random35 |   1,024.75 us |     9.658 us |     8.065 us |  2.03 |    0.02 |   332.0313 |   9.7656 |        - |   2714.76 KB |        3.41 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 32   | Random50 |     333.07 us |     2.132 us |     1.994 us |  0.50 |    0.00 |     2.4414 |        - |        - |      20.3 KB |        0.02 |
| Prefix         | 32   | Random50 |     667.15 us |     4.392 us |     3.894 us |  1.00 |    0.01 |   117.1875 |   0.9766 |        - |    957.36 KB |        1.00 |
| StackHistogram | 32   | Random50 |   1,320.58 us |     7.157 us |     6.345 us |  1.98 |    0.01 |   396.4844 |  11.7188 |        - |   3251.57 KB |        3.40 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 32   | Random70 |     391.60 us |     1.680 us |     1.572 us |  0.53 |    0.00 |     2.4414 |        - |        - |      20.3 KB |        0.02 |
| Prefix         | 32   | Random70 |     740.11 us |     2.275 us |     2.128 us |  1.00 |    0.00 |   112.3047 |   0.9766 |        - |    921.01 KB |        1.00 |
| StackHistogram | 32   | Random70 |   1,300.94 us |     2.840 us |     2.517 us |  1.76 |    0.01 |   371.0938 |   9.7656 |        - |   3039.09 KB |        3.30 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 32   | Rooms    |      90.29 us |     0.248 us |     0.232 us |  0.72 |    0.00 |     1.5869 |        - |        - |     13.23 KB |        0.11 |
| Prefix         | 32   | Rooms    |     125.20 us |     0.171 us |     0.160 us |  1.00 |    0.00 |    14.8926 |        - |        - |    122.28 KB |        1.00 |
| StackHistogram | 32   | Rooms    |     163.17 us |     0.258 us |     0.241 us |  1.30 |    0.00 |    54.1992 |   0.7324 |        - |    443.15 KB |        3.62 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 64   | Random35 |   4,111.23 us |    14.146 us |    12.540 us |  0.47 |    0.00 |     7.8125 |        - |        - |     80.34 KB |       0.006 |
| Prefix         | 64   | Random35 |   8,776.61 us |    87.473 us |    81.822 us |  1.00 |    0.01 |  1578.1250 |  46.8750 |        - |  13039.91 KB |       1.000 |
| StackHistogram | 64   | Random35 |  17,091.85 us |    19.660 us |    17.428 us |  1.95 |    0.02 |  4968.7500 | 187.5000 | 125.0000 |  40527.04 KB |       3.108 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 64   | Random50 |   5,066.09 us |     8.825 us |     8.255 us |  0.39 |    0.00 |     7.8125 |        - |        - |     80.34 KB |       0.006 |
| Prefix         | 64   | Random50 |  12,836.68 us |    80.017 us |    70.933 us |  1.00 |    0.01 |  1765.6250 |  62.5000 |        - |   14515.5 KB |       1.000 |
| StackHistogram | 64   | Random50 |  19,376.10 us |   145.801 us |   121.751 us |  1.51 |    0.01 |  5437.5000 | 187.5000 | 125.0000 |   44304.7 KB |       3.052 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 64   | Random70 |   6,895.00 us |    14.604 us |    12.195 us |  0.50 |    0.00 |     7.8125 |        - |        - |     80.34 KB |       0.006 |
| Prefix         | 64   | Random70 |  13,745.57 us |   140.307 us |   131.243 us |  1.00 |    0.01 |  1734.3750 |  62.5000 |        - |  14306.99 KB |       1.000 |
| StackHistogram | 64   | Random70 |  18,494.57 us |    95.575 us |    79.809 us |  1.35 |    0.01 |  5093.7500 | 187.5000 |  93.7500 |  41367.32 KB |       2.891 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 64   | Rooms    |     595.56 us |     1.523 us |     1.189 us |  0.83 |    0.01 |     5.8594 |        - |        - |     49.23 KB |        0.10 |
| StackHistogram | 64   | Rooms    |     596.81 us |     8.235 us |     7.703 us |  0.83 |    0.01 |   199.2188 |  10.7422 |        - |   1630.02 KB |        3.38 |
| Prefix         | 64   | Rooms    |     719.38 us |     5.311 us |     4.708 us |  1.00 |    0.01 |    58.5938 |        - |        - |    482.28 KB |        1.00 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 128  | Random35 |  96,346.54 us |   672.872 us |   596.483 us |  0.50 |    0.01 |          - |        - |        - |    320.39 KB |       0.002 |
| Prefix         | 128  | Random35 | 192,950.07 us | 3,358.115 us | 3,141.183 us |  1.00 |    0.02 | 25333.3333 | 333.3333 |        - | 209407.93 KB |       1.000 |
| StackHistogram | 128  | Random35 | 281,139.02 us | 5,295.146 us | 5,665.747 us |  1.46 |    0.04 | 74000.0000 | 500.0000 |        - | 604797.71 KB |       2.888 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 128  | Random50 | 133,561.58 us |   920.681 us | 1,023.334 us |  0.49 |    0.01 |          - |        - |        - |    320.39 KB |       0.001 |
| Prefix         | 128  | Random50 | 274,852.77 us | 2,613.278 us | 2,444.462 us |  1.00 |    0.01 | 28500.0000 | 500.0000 |        - | 235599.91 KB |       1.000 |
| StackHistogram | 128  | Random50 | 349,592.32 us |   756.450 us |   707.583 us |  1.27 |    0.01 | 80000.0000 |        - |        - | 659346.11 KB |       2.799 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| Expansion      | 128  | Random70 | 164,049.91 us | 2,936.844 us | 4,395.731 us |  0.53 |    0.01 |          - |        - |        - |    320.39 KB |       0.001 |
| Prefix         | 128  | Random70 | 308,959.24 us | 2,367.417 us | 2,214.484 us |  1.00 |    0.01 | 27000.0000 | 500.0000 |        - | 225225.58 KB |       1.000 |
| StackHistogram | 128  | Random70 | 368,986.18 us | 4,553.654 us | 4,259.491 us |  1.19 |    0.02 | 73000.0000 |        - |        - | 600473.66 KB |       2.666 |
|                |      |          |               |              |              |       |         |            |          |          |              |             |
| StackHistogram | 128  | Rooms    |   2,331.51 us |    33.907 us |    31.716 us |  0.49 |    0.01 |   750.0000 | 144.5313 |        - |   6151.77 KB |        3.20 |
| Expansion      | 128  | Rooms    |   3,981.27 us |    21.906 us |    20.491 us |  0.84 |    0.01 |    15.6250 |        - |        - |    193.23 KB |        0.10 |
| Prefix         | 128  | Rooms    |   4,719.00 us |    48.009 us |    44.908 us |  1.00 |    0.01 |   226.5625 |        - |        - |   1922.28 KB |        1.00 |


| Method                  | Size | Scenario | Mean          | Error        | StdDev       | Ratio | Gen0     | Allocated  | Alloc Ratio |
|------------------------ |----- |--------- |--------------:|-------------:|-------------:|------:|---------:|-----------:|------------:|
| ExpansionOptimized      | 32   | Random35 |     164.71 us |     2.678 us |     2.505 us |  0.41 |   0.7324 |    6.09 KB |        0.03 |
| PrefixOptimized         | 32   | Random35 |     404.36 us |     4.289 us |     3.802 us |  1.00 |  25.8789 |  213.78 KB |        1.00 |
| StackHistogramOptimized | 32   | Random35 |     462.36 us |     3.356 us |     2.975 us |  1.14 |   8.7891 |   75.06 KB |        0.35 |
|                         |      |          |               |              |              |       |          |            |             |
| ExpansionOptimized      | 32   | Random50 |     235.98 us |     4.367 us |     4.085 us |  0.42 |   0.7324 |    6.09 KB |        0.02 |
| PrefixOptimized         | 32   | Random50 |     557.71 us |     3.652 us |     3.050 us |  1.00 |  30.2734 |  254.72 KB |        1.00 |
| StackHistogramOptimized | 32   | Random50 |     613.87 us |     2.585 us |     2.159 us |  1.10 |   9.7656 |   87.25 KB |        0.34 |
|                         |      |          |               |              |              |       |          |            |             |
| ExpansionOptimized      | 32   | Random70 |     262.96 us |     0.767 us |     0.717 us |  0.44 |   0.4883 |    6.09 KB |        0.02 |
| PrefixOptimized         | 32   | Random70 |     592.30 us |     6.399 us |     5.673 us |  1.00 |  29.2969 |  245.51 KB |        1.00 |
| StackHistogramOptimized | 32   | Random70 |     607.23 us |     1.578 us |     1.399 us |  1.03 |   9.7656 |   84.51 KB |        0.34 |
|                         |      |          |               |              |              |       |          |            |             |
| ExpansionOptimized      | 32   | Rooms    |      63.13 us |     0.474 us |     0.420 us |  0.87 |   0.7324 |    6.09 KB |        0.16 |
| PrefixOptimized         | 32   | Rooms    |      72.75 us |     0.158 us |     0.132 us |  1.00 |   4.5166 |   37.84 KB |        1.00 |
| StackHistogramOptimized | 32   | Rooms    |      78.88 us |     0.449 us |     0.375 us |  1.08 |   2.0752 |   17.72 KB |        0.47 |
|                         |      |          |               |              |              |       |          |            |             |
| ExpansionOptimized      | 64   | Random35 |   2,993.98 us |    18.876 us |    17.657 us |  0.43 |        - |   24.09 KB |        0.37 |
| PrefixOptimized         | 64   | Random35 |   6,915.39 us |    31.947 us |    26.677 us |  1.00 |   7.8125 |   64.34 KB |        1.00 |
| StackHistogramOptimized | 64   | Random35 |   7,956.18 us |    38.551 us |    36.061 us |  1.15 |  62.5000 |  513.05 KB |        7.97 |
|                         |      |          |               |              |              |       |          |            |             |
| ExpansionOptimized      | 64   | Random50 |   4,316.59 us |    23.165 us |    21.668 us |  0.50 |        - |   24.09 KB |        0.37 |
| PrefixOptimized         | 64   | Random50 |   8,642.23 us |    34.842 us |    29.095 us |  1.00 |        - |   64.37 KB |        1.00 |
| StackHistogramOptimized | 64   | Random50 |   9,225.49 us |    29.865 us |    27.936 us |  1.07 |  62.5000 |  564.08 KB |        8.76 |
|                         |      |          |               |              |              |       |          |            |             |
| ExpansionOptimized      | 64   | Random70 |   4,390.38 us |     5.751 us |     5.098 us |  0.46 |        - |   24.09 KB |        0.37 |
| PrefixOptimized         | 64   | Random70 |   9,475.75 us |    24.564 us |    21.776 us |  1.00 |        - |   64.37 KB |        1.00 |
| StackHistogramOptimized | 64   | Random70 |  11,854.55 us |    48.460 us |    42.959 us |  1.25 |  62.5000 |  556.87 KB |        8.65 |
|                         |      |          |               |              |              |       |          |            |             |
| StackHistogramOptimized | 64   | Rooms    |     314.38 us |     0.523 us |     0.463 us |  0.90 |   5.8594 |   48.72 KB |        1.47 |
| PrefixOptimized         | 64   | Rooms    |     348.75 us |     1.125 us |     0.997 us |  1.00 |   3.9063 |   33.19 KB |        1.00 |
| ExpansionOptimized      | 64   | Rooms    |     426.53 us |     4.554 us |     4.259 us |  1.22 |   2.9297 |   24.09 KB |        0.73 |
|                         |      |          |               |              |              |       |          |            |             |
| ExpansionOptimized      | 128  | Random35 |  70,599.47 us |   515.376 us |   482.083 us |  0.42 |        - |   96.09 KB |        0.37 |
| PrefixOptimized         | 128  | Random35 | 168,060.07 us | 1,669.084 us | 1,561.263 us |  1.00 |        - |  261.69 KB |        1.00 |
| StackHistogramOptimized | 128  | Random35 | 196,814.76 us | 1,380.711 us | 1,223.964 us |  1.17 | 333.3333 | 3700.96 KB |       14.14 |
|                         |      |          |               |              |              |       |          |            |             |
| ExpansionOptimized      | 128  | Random50 | 103,793.07 us | 1,178.428 us | 1,044.646 us |  0.44 |        - |   96.09 KB |        0.37 |
| PrefixOptimized         | 128  | Random50 | 236,801.73 us | 1,824.147 us | 1,706.309 us |  1.00 |        - |  261.69 KB |        1.00 |
| StackHistogramOptimized | 128  | Random50 | 255,626.62 us | 2,005.127 us | 1,875.597 us |  1.08 | 500.0000 | 4132.33 KB |       15.79 |
|                         |      |          |               |              |              |       |          |            |             |
| ExpansionOptimized      | 128  | Random70 | 130,690.07 us |   890.340 us |   832.825 us |  0.50 |        - |   96.09 KB |        0.36 |
| StackHistogramOptimized | 128  | Random70 | 237,182.29 us | 1,856.412 us | 1,736.489 us |  0.91 | 333.3333 | 3961.47 KB |       14.98 |
| PrefixOptimized         | 128  | Random70 | 260,237.65 us | 2,170.638 us | 2,030.416 us |  1.00 |        - |  264.36 KB |        1.00 |
|                         |      |          |               |              |              |       |          |            |             |
| StackHistogramOptimized | 128  | Rooms    |   1,292.18 us |     5.276 us |     4.406 us |  0.74 |  17.5781 |  158.72 KB |        1.23 |
| PrefixOptimized         | 128  | Rooms    |   1,738.85 us |     8.411 us |     7.457 us |  1.00 |  15.6250 |  129.22 KB |        1.00 |
| ExpansionOptimized      | 128  | Rooms    |   3,005.77 us |     7.001 us |     6.549 us |  1.73 |   7.8125 |   96.09 KB |        0.74 |
