/* Copyright © 2014 Apex Software. All rights reserved. */
namespace Apex.Services
{
    /// <summary>
    /// Interface for message buses
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Subscribes the specified subscriber.
        /// </summary>
        /// <typeparam name="T">The type of message being subscribed to</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        void Subscribe<T>(IHandleMessage<T> subscriber);

        /// <summary>
        /// Unsubscribes the specified subscriber.
        /// </summary>
        /// <typeparam name="T">The type of message being unsubscribed from</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        void Unsubscribe<T>(IHandleMessage<T> subscriber);

        /// <summary>
        /// Posts the specified message.
        /// </summary>
        /// <typeparam name="T">The type of message</typeparam>
        /// <param name="message">The message.</param>
        void Post<T>(T message);
    }
}
