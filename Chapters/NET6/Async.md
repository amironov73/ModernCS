### Улучшения в Async

https://www.infoq.com/news/2021/04/Net6-Async/

Among the over 100 API changes in .NET 6 are several features designed to make working with asynchronous code easier and safer. Here are some of the highlights.

#### New WaitAsync Methods

While ideally all asynchronous functions offer the ability to cancel the operation, sometimes one isn’t provided. For those cases, three new WaitAsync methods were added to Task and Task<TResult>:

```c#
public Task WaitAsync(CancellationToken cancellationToken);
public Task WaitAsync(TimeSpan timeout);
public Task WaitAsync(TimeSpan timeout, CancellationToken cancellationToken);
```

As the signature implies, these methods take a timeout and/or CancellationToken. Either of these can be used to abort the waiting for an asynchronous operation to complete. Note this isn’t the same as aborting the operation itself; the operation may continue even though the calling code is no longer waiting for it.

As a bit of trivia, Microsoft now considers timeout values in the form of int milliseconds to be a mistake. Going forward, all duration-based timeouts should be expressed solely in terms of a TimeSpan.

#### Reusable CancellationTokenSource

When an externally triggered operation such as a web request starts, it often needs to create a CancellationTokenSource. This allows the request handler to abort if the requester cancels the request, the connection drops, etc. Most of the time the request won’t be canceled, meaning the CancellationTokenSource is never fired.

In order to improve performance, framework developers would like to [reuse the CancellationTokenSource for new operations](https://github.com/dotnet/runtime/issues/48492/). But currently they can’t because they have no way of knowing what may be holding onto an old CancellationToken linked to it. Strange errors may occur if an operation is cancelled because it happened to be sharing a recycled CancellationTokenSource with a completely different operation.

The new `CancellationTokenSource.TryReset` operation fixes this by disconnecting all of the old CancellationToken objects from the CancellationTokenSource. This way, any cancellation of a new operation cannot affect the previous operations. The old CancellationToken objects still exist, but they can’t receive the message from the recycled CancellationTokenSource.

It is called `TryReset` because it only works on a CancellationTokenSource that hasn’t been fired. Once a CancellationTokenSource is actually cancelled, it may not be recycled. Thus, the usage pattern will be:

```c#
if (!cts.TryReset())
  cts = new CancellationTokenSource();
```

#### Cancellation Events
One way to detect when a cancellation has been requested is to call CancellationToken.Register and give it a delegate to invoke. In rare cases, this delegate needs access to the original CancellationToken used.

A [new overload of CancellationToken.Register](https://github.com/dotnet/runtime/issues/40475) will offer this capability:

```c#
public CancellationTokenRegistration Register<T>(Action<T, CancellationToken> callback, T state);
public CancellationTokenRegistration UnsafeRegister<T>(Action<T, CancellationToken> callback, T state);
```

The documentation for UnsafeRegister is incomplete, but a comment in an old bug report says,

> `CancellationToken.Register` captures the current ExecutionContext and uses it to invoke the callback if/when it's invoked. That's generally desirable and is the right default, but in cases where we know for certain the callback doesn't care about EC (e.g. we're not invoking any 3rd-party code), we can use UnsafeRegister instead (newly added in 3.0), which skips capturing the ExecutionContext, as if Capture returned `null`.

#### Easier Execution Context Restoration

According to Ben Adams, “awaiting a Task returning method that is itself not async does not guarantee undoing AsyncLocal/ExecutionContext changes”. As a result, bugs can occur that are hard to diagnose. Examples of this include Propagated headers get mixed up without previous async middleware and Kestrel is causing AsyncLocal to bleed context data across requests on same HTTP/1.1 connection.

By combining `ExecutionContext.Capture` with the new `ExecutionContext.Restore` function, this problem can be avoided.
