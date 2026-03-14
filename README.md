# LeastRectangles

Deterministic experiments for decomposing binary grids into non-overlapping rectangles.

The repository is set up around one public claim: if you want to compare the expansion approach against stack-histogram and prefix scans fairly, all compared implementations must solve the same problem, use the same greedy selection rule, and be benchmarked on the same fixed inputs.

## What is comparable

- `PrefixRectangleAlgorithm`: readable baseline that rebuilds row prefixes every iteration.
- `StackHistogramAlgorithm`: readable largest-rectangle finder using the classic histogram stack scan.
- `ExpansionRectangleAlgorithm`: readable version of the incremental expansion approach.
- `OptimizedPrefixAlgorithm`: micro-optimized implementation of the prefix baseline.
- `OptimizedExpansionAlgorithm`: micro-optimized implementation of the expansion approach.

`OptimizedBranchingAlgorithm` and `RandomizedRectangleAlgorithm` are kept as experiments, but they are not part of the deterministic apples-to-apples benchmark story.

## Fair benchmark rules

- The benchmark of record is `ReferenceRectangleBenchmarks` in [`Benchmark/Program.cs`](Benchmark/Program.cs). It compares readable implementations only.
- Optimized variants are benchmarked separately in `PrefixImplementationBenchmarks` and `ExpansionImplementationBenchmarks`.
- All comparable algorithms use the same canonical tie-break: largest area, then top-most, then left-most, then shorter height.
- Benchmark setup validates exact output parity before timing begins.
- Random benchmark grids are generated from fixed seeds, and the structured `Rooms` scenario is deterministic.

## Run it

- `dotnet test`
- `dotnet run --project Benchmark -c Release`
- `dotnet run --project LeastRectangles`

## Publishable story

If you publish benchmark results from this repo, cite the reference suite first. That is the readable, reviewer-friendly comparison. The optimized suites are useful follow-up evidence that the expansion idea still wins when you engineer it harder, but they should not replace the readable baseline comparison.
