// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using osu.Framework.Extensions.ListExtensions;
using osu.Framework.Lists;

namespace osu.Framework.Bindables
{
    public class CollectionChangedEvent<T>
    {
        private static readonly SlimReadOnlyListWrapper<T> empty_list = new List<T>().AsSlimReadOnly();

        public readonly NotifyCollectionChangedAction Action;
        public readonly SlimReadOnlyListWrapper<T> NewItems = empty_list;
        public readonly SlimReadOnlyListWrapper<T> OldItems = empty_list;
        public readonly int NewStartingIndex = -1;
        public readonly int OldStartingIndex = -1;

        /// <summary>
        /// Construct a CollectionChangedEvent that describes a multi-item change (or a reset).
        /// </summary>
        /// <param name="action">The action that caused the event - can be <see cref="NotifyCollectionChangedAction.Add"/> or <see cref="NotifyCollectionChangedAction.Remove"/></param>
        /// <param name="changedItems">The items affected by the change.</param>
        /// <param name="startingIndex">The index where the change occurred.</param>
        public CollectionChangedEvent(NotifyCollectionChangedAction action, List<T> changedItems, int startingIndex)
        {
            Debug.Assert(startingIndex >= -1);

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    NewItems = changedItems.AsSlimReadOnly();
                    NewStartingIndex = startingIndex;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    OldItems = changedItems.AsSlimReadOnly();
                    OldStartingIndex = startingIndex;
                    break;

                default:
                    throw new ArgumentException();
            }

            Action = action;
        }

        /// <summary>
        /// Construct a CollectionChangedEvent that describes a multi-item Replace event.
        /// </summary>
        /// <param name="newItems">The new items replacing the original items.</param>
        /// <param name="oldItems">The original items that are replaced.</param>
        /// <param name="startingIndex">The starting index of the items being replaced.</param>
        public CollectionChangedEvent(List<T> newItems, List<T> oldItems, int startingIndex)
        {
            Action = NotifyCollectionChangedAction.Replace;
            NewItems = newItems.AsSlimReadOnly();
            OldItems = oldItems.AsSlimReadOnly();
            NewStartingIndex = OldStartingIndex = startingIndex;
        }

        /// <summary>
        /// Construct a CollectionChangedEvent that describes a one-item Move event.
        /// </summary>
        /// <param name="movedItem">The item affected by the change.</param>
        /// <param name="index">The new index for the changed item.</param>
        /// <param name="oldIndex">The old index for the changed item.</param>
        public CollectionChangedEvent(T movedItem, int index, int oldIndex)
        {
            if (index < 0)
            {
                throw new ArgumentException();
            }

            Action = NotifyCollectionChangedAction.Move;
            NewItems = OldItems = new List<T> { movedItem }.AsSlimReadOnly();
            NewStartingIndex = index;
            OldStartingIndex = oldIndex;
        }
    }
}
