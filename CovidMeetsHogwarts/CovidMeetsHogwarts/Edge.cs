using System;
using System.IO.Compression;
using System.Runtime.CompilerServices;


namespace CovidMeetsHogwarts
{
    public class Edge
    {
        // Attributes
        private (Node source, Node destination) endpoints; // source and destination
        // can be interchanged because we're working with undirected graphs
        
        // Methods
        // - constructor
        public Edge(Node source, Node destination)
        {
            this.endpoints = (source,destination);
        }
        
        // - getters
        public (Node source, Node destination) GetEndpoints()
        {
            return endpoints;
        }

        // - == and != operators overload
        public static bool operator== (Edge edge1, Edge edge2)
        {
            if (object.ReferenceEquals(null,edge1) && object.ReferenceEquals(null,edge2))
                return false;
            try
            {
                if (edge1.endpoints.source == edge2.endpoints.source)
                {
                    if (edge1.endpoints.destination == edge2.endpoints.destination)
                    {
                        return true;
                    }
                    return false;
                }

                if (edge1.endpoints.source == edge2.endpoints.destination)
                {
                    if (edge1.endpoints.destination == edge2.endpoints.source)
                    {
                        return true;
                    }
                    return false;
                }

                return false;
            }
            catch (NullReferenceException e)
            {
                return false;
            }
        }

        public static bool operator!= (Edge edge1, Edge edge2)
        {
            return !(edge1 == edge2);
        }

        /// <summary>
        /// represent edge by its end points in DOT language
        /// </summary>
        /// <returns>string describing this edge in DOT language followed by a newline character</returns>
        public override string ToString()
        {
            return "\t" + this.endpoints.source + " -- " + this.endpoints.destination + "\n";
            
        }
    }
}