# Stylish F#

## Table of contents

- Chapter 2: Designing Functions Using Types
- Chapter 3: Missing Data
- Chapter 4: Working Effectively with Collection Functions
- Chapter 5: Immutability and Mutation
- Chapter 6: Pattern Matching
- Chapter 7: Record Types
- Chapter 8: Classes
- Chapter 9: Programming with Functions
- Chapter 10: Aysnchronous and Parallel Programming
- Chapter 11: Railway Oriented Programming
- Chapter 12: Performance
- Chapter 13: Layout and Naming

## Testing

I added a test project `stylishFsharp.Tests`.

Why? 

- I don't feel comfortable using a REPL
- code should be testable from the get-go (IMHO)

I changed testing frameworks during the course of learning F#.
Since I'm coming from C#, I started with [`FsUnit`](https://github.com/fsprojects/FsUnit). 
`FsUnit` is similar to [`FluentAssertions`](https://fluentassertions.com/) in C# and has a nice API.

After reading [Review: F# unit testing frameworks and libraries](https://devonburriss.me/review-fsharp-test-libs/) 
I decided to give [`Unquote`](https://github.com/SwensenSoftware/unquote) a try.

`Unquote`'s basic test syntax is:

```fsharp
test <@ (1 + 1) = (1 + 2) @>
// -> test fails
```

Shorthand syntax:

```fsharp
(1 + 1) =! (1 + 2)
// -> test fails
```

Note: `=!` not `!=`!! Read more like `#!` "she-bang": "equal-bang"...

