using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;

namespace CovidMeetsHogwarts
{
    public class Graph
    {
        // Attributes
        private string name;
        private List<Node> nodes; // spots
        private List<Edge> edges; // paths
        
        // Methods
        // - constructor
        public Graph(string name)
        {
            this.edges = new List<Edge>();
            this.nodes = new List<Node>();
            this.name = name;
        }

        // - getters
        public string GetName()
        {
            return name;
        }

        public List<Node> GetNodes()
        {
            return nodes;
        }

        public List<Edge> GetEdges()
        {
            return edges;
        }

        /// <summary>
        /// try to create and add node to this graph.
        /// if a node with the same label already exists, then return existing node.
        /// return created node otherwise.
        /// </summary>
        /// <param name="label">label of the node to add</param>
        /// <returns>created/existing node</returns>
        Node AddNode(string label)
        {
            //TODO try catch
            Node find =this.nodes.Find(x => x.GetLabel() == label);
            if (find == null)
            {
                find = new Node(label);
                nodes.Add(find);
            }

            return find;
        }
        
        /// <summary>
        /// try to create and add edge to this graph.
        /// if this edge already exists, then return false because no edges were added.
        /// return true otherwise.
        /// </summary>
        /// <param name="source">source node of the edge to add</param>
        /// <param name="destination">destination node of the edge to add</param>
        /// <returns>a boolean of whether an edge was added or not</returns>
        bool AddEdge(Node source, Node destination)
        {
            if (this.edges.Exists(e => e== new Edge(source,destination)))
            {
                return false;
            }
            edges.Add(new Edge(source, destination));
            return true;
        }

        /// <summary>
        /// extract the graph's name of the first line in a DOT file. (first line of a graph declaration)
        /// 
        /// example of a first line: "graph Epita {".
        /// In this example, the string "Epita" should be returned.
        /// </summary>
        /// <param name="firstLine">first line of a DOT file</param>
        /// <returns>the name of the graph</returns>
        public static string ExtractNameFromLine(string firstLine)
        {
            return firstLine.Split(" ")[1];
       
        }
        
        /// <summary>
        /// extract nodes and edge from a given edge line in DOT file and add them
        /// to given 'graph'.
        /// The format of edgeLine's string is the same as the ToString() method
        /// in Edge.cs without the newline character.
        /// 
        /// example of edgeLine: "    VA302 -- VA303;".
        /// In this example, the nodes of respective labels "VA302" and "VA303" as well
        /// as the edge linking those two should be added to the graph.
        /// </summary>
        /// <param name="edgeLine">string in DOT language describing an edge</param>
        /// <param name="graph">graph to update</param>
        /// <exception cref="Exception">an exception should be raised if the edge already exists</exception>
        public static void UpdateGraphFromLine(string edgeLine, Graph graph)
        {
            string[] partsline = edgeLine.Split("--");

            Node a = graph.AddNode(partsline[0].Trim(' '));
            Node b = graph.AddNode(partsline[1].Trim(' ').Trim(';'));
            
            //neighboor

            if (graph.AddEdge(a, b))
            {
                a.SetNeighbors(b);
                b.SetNeighbors(a);
            }
            else
            {
                throw new Exception("arête avec les mêmes extrémités");
            }


        }
        
        /// <summary>
        /// generate graph from file written in simple DOT language.
        /// </summary>
        /// <param name="filepath">path of file in DOT language</param>
        /// <returns>created graph</returns>
        public static Graph FromFile(string filepath)
        {
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filepath);  
            Graph graph = new Graph(ExtractNameFromLine(file.ReadLine()));

            while((line = file.ReadLine()) != "}")  
            {  
                UpdateGraphFromLine(line,graph);
            }
            return graph;
        }
    }
}