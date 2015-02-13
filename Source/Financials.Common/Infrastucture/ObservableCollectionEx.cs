﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Kernel;

namespace Financials.Common.Infrastucture
{
    public static class ObservableCollectionEx
    {
        /// <summary>
        /// Convert an observable collection into a dynamic stream of change sets
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// source
        /// or
        /// keySelector
        /// </exception>
        public static IObservable<IChangeSet<TObject, TKey>> AsObservable<TObject, TKey>(this ObservableCollection<TObject> source, Func<TObject, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            
            return Observable.Create<IChangeSet<TObject, TKey>>
                (
                    observer =>
                    {
                        //populate local cache, otherwise there is no way to deal with a reset
                        var resultCache = new SourceCache<TObject, TKey>(keySelector);

                        Func<ChangeSet<TObject, TKey>> initialChangeSet = () =>
                        {
                            var items = source.Select(t => new Change<TObject, TKey>(ChangeReason.Add, keySelector(t), t));
                            return new ChangeSet<TObject, TKey>(items);
                        };
                                 
                        var sourceUpdates =  Observable
                            .FromEventPattern<NotifyCollectionChangedEventArgs>(source, "CollectionChanged")
                            .Select
                            (
                                args =>
                                {
                                    var changes = args.EventArgs;

                                    switch (changes.Action)
                                    {
                                        case NotifyCollectionChangedAction.Add:
                                            return changes.NewItems.OfType<TObject>()
                                                .Select(t => new Change<TObject, TKey>(ChangeReason.Add, keySelector(t), t));

                                        case NotifyCollectionChangedAction.Remove:
                                            return changes.OldItems.OfType<TObject>()
                                                .Select(t =>new Change<TObject, TKey>(ChangeReason.Remove, keySelector(t), t));

                                        case NotifyCollectionChangedAction.Replace:
                                        {
                                            return changes.NewItems.OfType<TObject>()
                                                .Select((t, idx) =>
                                                {
                                                    var old = changes.OldItems[idx];
                                                    return new Change<TObject, TKey>(ChangeReason.Update, keySelector(t), t, (TObject)old);
                                                });
                                        }
                                        case NotifyCollectionChangedAction.Reset:
                                        {
                                            //Clear all from the cache and reload
                                            var removes = resultCache.KeyValues.Select(t => new Change<TObject, TKey>(ChangeReason.Remove,t.Key, t.Value)).ToArray();
                                            return removes.Concat(initialChangeSet());
                                        }
                                        default:
                                            return null;
                                    }
                                })
                            .Where(updates => updates != null)
                            .Select(updates => (IChangeSet<TObject, TKey>)new ChangeSet<TObject, TKey>(updates));

                        var cacheLoader = Observable.Return(initialChangeSet()).Concat(sourceUpdates).PopulateInto(resultCache);
                        var subscriber = resultCache.Connect().SubscribeSafe(observer);

                        return new CompositeDisposable(cacheLoader, subscriber, resultCache);
                    }

                );
        }
    }
}