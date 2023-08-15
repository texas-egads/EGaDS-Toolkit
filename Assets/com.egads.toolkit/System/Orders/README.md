# Orders System User Guide

This Orders System is a set of interfaces and classes designed to manage and execute orders in a flexible and organized manner. Orders can be performed either in parallel (OrderList) or sequentially (OrderQueue). The system allows you to define custom orders by implementing the provided interfaces. The Orders System provides a simple yet powerful way to manage orders in Unity. It includes two main classes, `OrderList` and `OrderQueue`, which allow you to execute orders either simultaneously or sequentially.

## Table of Contents

1. [Overview](#overview)
2. [Interfaces](#interfaces)
   - [IOrder](#iorder)
   - [IOrderQueueElement](#iorderqueueelement)
   - [IEnumeratedOrder](#ienumeratedorder)
3. [Classes](#classes)
   - [OrdernBase](#ordernbase)
   - [OrderList](#orderlist)
   - [OrderQueue](#orderqueue)
4. [Custom Order Example](#custom-order-example)
5. [Usage Examples](#usage-examples)

## Interfaces

### IOrder

The `IOrder` interface represents a basic order that can be executed. It has a single method:

- `void Execute()`: Executes the order.

### IOrderQueueElement

The `IOrdernQueueElement` interface represents an order that lasts for a specific duration and can be added to a chronological queue. It has three methods:

- `void OnStart()`: Called when the order starts. Perform any necessary setup here.
- `void Update()`: Called each frame while the order is ongoing. Implement the main logic of the order here.
- `void OnExit()`: Called when the order is completed or interrupted. Clean up resources or perform final orders here.

### IEnumeratedOrder

The `IEnumeratedOrder` interface represents an order whose execution process can be divided into multiple steps using an IEnumerator. This allows the order to be executed in multiple frames and supports iteration. It has a single method:

- `IEnumerator Execute()`: Represents the order's execution process as an IEnumerator.

## Classes

### OrderBase

`OrderBase` is a base class that implements the `IOrderQueueElement` interface. It provides default implementations for the interface methods, allowing you to create custom orders more easily.

### OrderList

The `OrderList` class represents a list of orders that get executed simultaneously. Orders in the list will run in parallel, regardless of the execution time of individual orders.

Public Properties

- `bool hasContent`: Gets a value indicating whether the order list has any content (orders).

Public Methods

- `void Update()`: Should be called every frame by the parent object to update all the orders in the list.
- `void Add(IOrderQueueElement element)`: Adds a new element to the order list.
- `void Clear()`: Calls OnExit on all elements in the current list and clears all entries from the queue.

### OrderQueue

The `OrderQueue` class represents a list of orders that get executed one after another (in a queue). Orders in the queue will run sequentially.

Public Properties

- `bool hasContent`: Gets a value indicating whether the order queue has any content (orders).

Public Methods

- `void Update()`: Should be called every frame by the parent object to update the order queue.
- `void Add(IOrderQueueElement element)`: Adds a new element to the back of the order queue.
- `void AddPause(float duration)`: Adds a time duration where nothing happens (a pause) to the order queue.
- `void Clear()`: Calls OnExit on the current element and clears all entries from the order queue.

## Custom Order Example

Here's an example of creating a custom order using the provided interfaces:

```csharp
using UnityEngine;
using egads.system.orders

public class CustomOrder : IOrderQueueElement
{
    private bool _isCompleted;

    public bool hasEnded => _isCompleted;

    public void OnStart()
    {
        // Perform setup and initialization for the custom order.
        _isCompleted = false;
    }

    public void Update()
    {
        // Implement the main logic of the custom order.
        // For example, move a game object from one position to another.
        // Once the order is completed, set _isCompleted to true.
        // This will signal the OrderQueue to proceed to the next order.
    }

    public void OnExit()
    {
        // Perform cleanup or final orders when the order is completed or interrupted.
    }
}
```

## Usage Examples
### Using OrderList:
```csharp
using UnityEngine;
using egads.system.orders;

public class ExampleOrderListUsage : MonoBehaviour
{
    private OrderList orderList;

    private void Start()
    {
        orderList = new OrderList();

        // Add multiple orders to the list
        orderList.Add(new CustomOrder());
        orderList.Add(new AnotherCustomOrder());
        orderList.Add(new OrderWait(2f)); // Pause for 2 seconds

        // More orders can be added here...

        // Start the order list
        orderList.Update();
    }

    private void Update()
    {
        // Update the order list each frame
        if (orderList.hasContent)
        {
            orderList.Update();
        }
    }

    // Clear the order list when needed
    private void OnDestroy()
    {
        orderList.Clear();
    }
}
```
### Using OrderQueue:
```csharp
using UnityEngine;
using egads.system.orders;

public class ExampleOrderQueueUsage : MonoBehaviour
{
    private OrderQueue orderQueue;

    private void Start()
    {
        orderQueue = new OrderQueue();

        // Add orders to the queue
        orderQueue.Add(new CustomOrder());
        orderQueue.AddPause(1.5f); // Pause for 1.5 seconds
        orderQueue.Add(new AnotherCustomOrder());
        orderQueue.Add(new OrderWait(3f)); // Pause for 3 seconds

        // More orders can be added here...

        // Start the order queue
        orderQueue.Update();
    }

    private void Update()
    {
        // Update the order queue each frame
        if (orderQueue.hasContent)
        {
            orderQueue.Update();
        }
    }

    // Clear the order queue when needed
    private void OnDestroy()
    {
        orderQueue.Clear();
    }
}
```
Remember to replace `CustomOrder` and `AnotherCustomOrder` with your own custom orders
that implement the `IOrderQueueElement` interface. You can now create complex order
sequences in Unity using the Orders System to perform various tasks in your games or
applications!
