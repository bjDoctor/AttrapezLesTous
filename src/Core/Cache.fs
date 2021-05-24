//////
/// Module allowing managing the state of a cache and retieval of data asynchronously
/// (This caching mechanism is very basing, not to be used in prod)
//////

module Core.Cache

open System.Collections.Concurrent

/// Retrieve a value identified by key from a cache, if it exists,
/// or obtains data from an async fetch method and store its result
///     Note: this implememntation is NOT thread safe!!! 
///     When calling TryGetValue, the cache is not locked, see: http://www.fssnip.net/7Qr/2
///     Although we can live with it for now, the issue of storing the result of an async call in a thread safe way will need to be addressed
let getFromCacheOrFetch (cache: ConcurrentDictionary<'a, 'b>) fetch key = 
    let fetchAndUpdateCache() = 
        async {
            let! value = fetch key
            cache.TryAdd(key, value) |> ignore
            return value
        }

    async {
        match cache.TryGetValue(key) with
            | true, cachedDetails -> return cachedDetails 
            | _ -> return! fetchAndUpdateCache()
    }

