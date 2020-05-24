// -----------------------------------------------------------------
// <copyright file="AStar.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Common.Utilities.Pathfinding
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface to setup and run the AStar algorithm.
    /// </summary>
    public class AStar
    {
        /// <summary>
        /// The open list.
        /// </summary>
        private readonly SortedList<int, INode> nextToVisit;

        /// <summary>
        /// The closed list.
        /// </summary>
        private readonly SortedList<int, INode> visited;

        /// <summary>
        /// The reference to the node factory in use.
        /// </summary>
        private readonly INodeFactory nodeFactory;

        /// <summary>
        /// The maximum number of steps to perform in the search before giving up.
        /// </summary>
        private readonly int maxSteps;

        /// <summary>
        /// The goal node.
        /// </summary>
        private readonly INode goal;

        /// <summary>
        /// The current amount of steps that the algorithm has performed.
        /// </summary>
        private int currentStepCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AStar"/> class.
        /// </summary>
        /// <param name="nodeFactory">A reference to the node factory in use.</param>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        /// <param name="maxSearchSteps">Optional. The maximum number of Step operations to perform on the search.</param>
        public AStar(INodeFactory nodeFactory, INode start, INode goal, int maxSearchSteps = 100)
        {
            nodeFactory.ThrowIfNull(nameof(nodeFactory));
            start.ThrowIfNull(nameof(start));
            goal.ThrowIfNull(nameof(goal));

            this.nextToVisit = new SortedList<int, INode>(new DuplicateIntegerComparer());
            this.visited = new SortedList<int, INode>(new DuplicateIntegerComparer());

            this.nodeFactory = nodeFactory;

            this.goal = goal;
            this.maxSteps = maxSearchSteps;

            this.currentStepCount = 0;

            this.CurrentNode = start;
            this.CurrentNode.ShouldBeVisited = true;

            this.nextToVisit.Add(this.CurrentNode);
        }

        /// <summary>
        /// Gets the current state of the open list.
        /// </summary>
        public IEnumerable<INode> OpenList => this.nextToVisit.Values;

        /// <summary>
        /// Gets the current state of the closed list.
        /// </summary>
        public IEnumerable<INode> ClosedList => this.visited.Values;

        /// <summary>
        /// Gets the current node that the AStar algorithm is at.
        /// </summary>
        public INode CurrentNode { get; private set; }

        /// <summary>
        /// Steps the AStar algorithm forward until it either fails or finds the goal node.
        /// </summary>
        /// <returns>Returns the state the algorithm finished in, Failed or GoalFound.</returns>
        public SearchState Run()
        {
            var state = SearchState.Searching;

            // Continue searching until either failure or the goal node has been found.
            while (state == SearchState.Searching)
            {
                state = this.Step();
            }

            return state;
        }

        /// <summary>
        /// Moves the AStar algorithm forward one step.
        /// </summary>
        /// <returns>Returns the state the alorithm is in after the step, either Failed, GoalFound or still Searching.</returns>
        public SearchState Step()
        {
            if (this.maxSteps > 0 && this.currentStepCount >= this.maxSteps)
            {
                return SearchState.Failed;
            }

            this.currentStepCount++;

            while (true)
            {
                // There are no more nodes to search, return failure.
                if (this.nextToVisit.IsEmpty())
                {
                    return SearchState.Failed;
                }

                // Pick the next best node in the graph by lowest total cost.
                this.CurrentNode = this.nextToVisit.Pop();

                // This node has already been visited, skip.
                if (this.CurrentNode.HasBeenVisited)
                {
                    continue;
                }

                // An unsearched node has been found, search it.
                break;
            }

            // First of all, flag this node to not be visited again.
            this.visited.Add(this.CurrentNode);
            this.CurrentNode.ShouldBeVisited = false;
            this.CurrentNode.HasBeenVisited = true;

            // Check if this node is the goal.
            if (this.CurrentNode.IsGoal(this.goal))
            {
                // Found the goal, stop searching.
                return SearchState.GoalFound;
            }

            // Node was not the goal. Add all the adjacent nodes to the next-to-visit list.
            // But before adding them, we need to estimate and set their movement and estimated costs.
            foreach (var adjacent in this.CurrentNode.FindAdjacent(this.nodeFactory))
            {
                // If the node has already been visited, ignore it.
                if (adjacent.HasBeenVisited)
                {
                    continue;
                }

                // If the node is waiting to be visited, check if we get a better cost by setting this as it's parent instead.
                if (adjacent.ShouldBeVisited)
                {
                    var oldCost = adjacent.MovementCost;

                    adjacent.SetMovementCost(parent: this.CurrentNode);

                    if (oldCost != adjacent.MovementCost)
                    {
                        // it's better with this parent.
                        adjacent.Parent = this.CurrentNode;
                    }

                    continue;
                }

                adjacent.Parent = this.CurrentNode;
                adjacent.SetMovementCost(this.CurrentNode);
                adjacent.SetEstimatedCost(this.goal);
                this.nextToVisit.Add(adjacent);
                adjacent.ShouldBeVisited = true;
            }

            // This step did not find the goal so return status of still searching.
            return SearchState.Searching;
        }

        /// <summary>
        /// Gets the path of the last solution of the AStar algorithm.
        /// Will return a partial path if the algorithm has not finished yet.
        /// </summary>
        /// <returns>Returns null if the algorithm has never been run.</returns>
        public IEnumerable<INode> GetLastPath()
        {
            if (this.CurrentNode != null)
            {
                var path = new Stack<INode>();

                var current = this.CurrentNode;

                while (current != null)
                {
                    path.Push(current);

                    current = current.Parent;
                }

                return path.ToArray();
            }

            return null;
        }
    }
}
