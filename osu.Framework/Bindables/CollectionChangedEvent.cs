// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        /// <param name="action">The action that caused the event.</param>
        /// <param name="changedItems">The items affected by the change.</param>
        /// <param name="startingIndex">The index where the change occurred.</param>
        public CollectionChangedEvent(NotifyCollectionChangedAction action, List<T> changedItems, int startingIndex)
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:

                    if (startingIndex < -1)
                        throw new ArgumentException();

                    if (action == NotifyCollectionChangedAction.Add)
                    {
                        NewItems = changedItems.AsSlimReadOnly();
                        NewStartingIndex = startingIndex;
                    }
                    else
                    {
                        OldItems = changedItems.AsSlimReadOnly();
                        OldStartingIndex = startingIndex;
                    }

                    break;

                default:
                    throw new ArgumentException();
            }

            Action = action;
        }

        /// <summary>
        /// Construct a CollectionChangedEvent that describes a multi-item Replace event.
        /// </summary>
        /// <param name="action">Can only be a Replace action.</param>
        /// <param name="newItems">The new items replacing the original items.</param>
        /// <param name="oldItems">The original items that are replaced.</param>
        /// <param name="startingIndex">The starting index of the items being replaced.</param>
        public CollectionChangedEvent(NotifyCollectionChangedAction action, List<T> newItems, List<T> oldItems, int startingIndex)
        {
            if (action != NotifyCollectionChangedAction.Replace)
                throw new ArgumentException();

            Action = action;
            NewItems = newItems.AsSlimReadOnly();
            OldItems = oldItems.AsSlimReadOnly();
            NewStartingIndex = OldStartingIndex = startingIndex;
        }

        /// <summary>
        /// Construct a CollectionChangedEvent that describes a one-item Move event.
        /// </summary>
        /// <param name="action">Can only be a Move action.</param>
        /// <param name="changedItem">The item affected by the change.</param>
        /// <param name="index">The new index for the changed item.</param>
        /// <param name="oldIndex">The old index for the changed item.</param>
        public CollectionChangedEvent(NotifyCollectionChangedAction action, T changedItem, int index, int oldIndex)
        {
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException();
            }

            if (index < 0)
            {
                throw new ArgumentException();
            }

            Action = action;
            NewItems = OldItems = new List<T> { changedItem }.AsSlimReadOnly();
            NewStartingIndex = index;
            OldStartingIndex = oldIndex;
        }
    }
}
