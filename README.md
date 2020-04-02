# GraphUtilities

A .NET library for creating and using undirected graphs
Also provides pattern matching functionality (finding a pattern/subgraph in another graph)

## Installing / Getting started

Option 1:
Build the .dll yourself by opening the VisualStudio solution (GraphUtilities.sln) and building it (F7)

Option 2:
Copy needed files into your project

## Developing

GraphUtilitiesTest contains Unit Test for the library if you ever want to modify it and still check functionality

## Features

GraphUtilities provides a base **Graph** class as well as **Vertex/Edge** classes to extend

The class **Graph** represents an UNDIRECTED mathematical graph consisting of vertices and edges
  * Vertices and edges are saved as objects and can each contain arbitrary data
  * Allows having vertices that are not connected to anything
  * Does not yet support vertices having an edge to themselves (no loops!)
  * Provides functionality to find a pattern (subgraph) in a graph

The classes **ReplacementRule** and **ReplacementRuleBuilder** provide functionality to find and replace subgraphs
  * defined by a pattern graph that gets matched in the host graph and then replaced by a replacement graph
  * mapping of vertices from pattern to replacement graph
  * fluent interface for comfortable usage

GraphUtilitiesTest provides **light** unit testing

## Known Limitations/Bugs

* Still not really possible to remove edges easily, since both pattern and replacement graph need to be connected

## Licensing

"The code in this project is licensed under [MIT license](https://mit-license.org/)." 
