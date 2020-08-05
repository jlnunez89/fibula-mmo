# The Creature's walk plan design:

Every creature is initialized with a single `WalkPlan` instance and maintains this reference throughout it's existence. This `WalkPlan` is the class that encapsulates the walking plan that a `Creature` has, and it comes into play when an `AutoWalkOrchestrationOperation` is executed with the creature as the requestor.

Instead of instructing the game to move a creature when a move request -arrow keys are pressed- is handled (walking on-demand), we buffer the directions that a creature intends to move to in a set. This set of directions to walk in are called *waypoints*. The walk plan is initialized with empty *waypoints*, which are implemented using a linked list.

The `AutoWalkOrchestrationOperation` operation is responsible for the decisions that the creature makes regarding walks, while the `WalkPlan` is active, which includes:
- Deciding whether the current walk plan is still active, and if so;
- Scheduling new movements as `MovementOperations` (with the creature as the requestor) between each waypoint reached.
- Deciding whether the current walk plan's waypoints need to be recalculated.
- Completing the walk plan, going into an inactive state.

> A walk plan's *waypoints* are usually ~~re~~calculated using the game's chosen path-finding algorithm, abstracted in the `IPathFinder` interface.

The `AutoWalkOrchestrationOperation` repeats itself until the walk plan is no longer active, or it is cancelled. This operation ends naturally when:
- The goal is reached and the plan does not support recalculation.
- The walk plan is interrupted (i.e. a waypoint could not be reached) and the plan does not support recalculation.

> The `AutoWalkOrchestrationOperation` is cancellable. Actions such as *stop attacking*, *stop following*, or *stop everything*, cancel the creature's current auto walk orchestration operation.

## Transitioning between interaction states:
A creature can only ever be in one of 3 different *interaction states*:

```
          .------> Attacking <-.
         /          /       \   \         
    Standing <---.-'         |   |
         \        \          |   |
          '---> Following <-'    |
                       `--------'                       
```

This implies that a creature can't be `Attacking` and `Following` at the same time and, thus, the next logical question is: ***"What about chasing after an attacked creature?"***. There is a distinction between the `Following` state and *chasing* after a creature while in an `Attacking` state: `Following` refers only to the (friendly) mechanic of following another creature without attacking it, while `Attacking` may be carried out while *chasing*, *keeping distance*, or *staying* wherever you direct to.

Transitioning between interaction states has the following effects over a creature's `AutoWalkOrchestrationOperation`:

|From|To|Effect
|--|--|--|
|Standing|Attacking|Any current `AutoWalkOrchestrationOperation` is cancelled, and a new operation is scheduled.|
|Standing|Following|Any current `AutoWalkOrchestrationOperation` is cancelled, and a new operation is scheduled.|
|Attacking|Standing|Any current `AutoWalkOrchestrationOperation` is cancelled.|
|Following|Standing|Any current `AutoWalkOrchestrationOperation` is cancelled.|
|Attacking|Following|-|
|Following|Attacking|-|

## Walk plan states and strategies:

Reiterating, the creature's `AutoWalkOrchestrationOperation` manages the entire walk plan, stopping when the goal is reached, or when it gives up, or until it is cancelled.

During it's excution, a `WalkPlan` transitions between 4 different `WalkPlanStates`:

```
         .------> AtGoal
        /          /    \         
    OnTrack <---.-'      '------,----> Aborted
         \       \             /
          '---> NeedsToRecalculate
```

|State|Active?|Description|
|--|--|--|
|OnTrack|Yes|The walk plan is considered to be on track, having waypoints still pending to be visited.|
|NeedsToRecalculate|Yes|The walk plan's waypoints need to be recalculated.|
|AtGoal|Yes|The walk plan reached the goal set for the time being.|
|Aborted|No|The walk plan was aborted.|

A `WalkPlan` has implicit stategies, based on the goal toward which the plan marks the way for, with one being completely `Static` and the other labeled as `Dynamic`. 

As the name would suggest:
 - A `static goal`, always remains static, like a specific `Location` in the map.
 - A `dynamic goal`, can constantly change/move, but also remain static, like a creature's location.

These implicit strategies are broken down into 5 different explicit strategies:

|Strategy|Type|Behavior| 
|--|--|--|
|DoNotRecalculate|Static|When a creature following a walk plan with this strategy is suddenly interrupted, it will give up and abort without recalculating. This is useful for static goals where we don't care if the action is interrupted. One example would be walking towards an item after the directive to use it.|
|RecalculateOnInterruption|Static|When a creature following a walk plan with this strategy is suddenly interrupted, it will recalculate the plan from it's current position. This is useful for static goals where we only care to retry if an interruption happens.|
|ConservativeRecalculation|Dynamic|The creature follows the walk plan and there is a low chance of recalculating at each waypoint. If the waypoint is not accessible or the creature is otherwise interrupted, it will recalculate the plan from it's current position. This is useful for slow moving goals or low-level AI simulation.|
|AggressiveRecalculation|Dynamic|The creature follows the walk plan and there is a medium chance of recalculating at each waypoint. If the waypoint is not accessible or the creature is otherwise interrupted, it will recalculate the plan from it's current position. This is useful for moderate moving goals or medium-level AI simulation.|
|ExtremeRecalculation|Dynamic|The creature follows the walk plan and there is a high chance of recalculating at each waypoint. If the waypoint is not accessible or the creature is otherwise interrupted, it will recalculate the plan from it's current position. This is useful for fast moving goals or high-level AI simulation.|
