using HutchAgent.Models;

namespace HutchAgent.Services;

public interface IQueueReader
{
  /// <summary>
  /// Attempt to pop a message from the top of the Queue. 
  /// </summary>
  /// <param name="queueName">The queue to pop a message from.</param>
  /// <returns>A message from the queue or <c>null</c> if empty.</returns>
  JobQueueMessage? Pop(string queueName);
}
