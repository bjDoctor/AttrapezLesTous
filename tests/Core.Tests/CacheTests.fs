namespace CacheTests

open System
open Xunit
open FsUnit.Xunit
open FsCheck.Xunit

open Core.Cache
open System.Collections.Concurrent

open FsCheck

module getFromCacheOrFetch = 
    [<Property>]
    let ``Fetches data if not in cache, and returns an async result`` (key: NonEmptyString, value: string) =
        //Arrange
        let emptyCache = ConcurrentDictionary<string, string>()
        let mutable fetchHasBeenCalled = false
        let fetch() = 
            async {
                fetchHasBeenCalled <- true
                return value
            }

        //Act
        let result = getFromCacheOrFetch emptyCache fetch key.Get

        //Assert
        (result |> Async.RunSynchronously) |> should equal value
        fetchHasBeenCalled |> should be True

    [<Property>]
    let ``Gets data from cache if found, and returns an async result`` (key: NonEmptyString, cachedalue: string, fetchedValue: string) =
        //Arrange
        let cache = ConcurrentDictionary<string, string>()
        cache.TryAdd(key.Get, cachedalue) |> ignore

        let mutable fetchHasBeenCalled = false
        let fetch() = 
            async {
                fetchHasBeenCalled <- true
                return fetchedValue
            }

        //Act
        let result = getFromCacheOrFetch cache fetch key.Get

        //Assert
        (result |> Async.RunSynchronously) |> should equal cachedalue
        fetchHasBeenCalled |> should be False

