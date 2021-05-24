//////
/// Module publicly exposing async functions to be called by an external module (e.g. the PokemonController)
/// The async functions are wrapped into a Task, to enable calls from C#
/// Naming: We follow the Get<Something>Async convention, widely used in many C# code bases
//////

module Core.Resources

/// Callable from C#: converts an F# async into a C# task
let private startAsyncFunctionAsTask f =
    async {
        return! f()
    }
    |> Async.StartAsTask

/// Rertieves a Pokemon details, identified by name (or Id)
let GetPokemonAsync logger name = 
    let f() = PokeApi.fecthPokemon logger name
    startAsyncFunctionAsTask f

/// Rertieves a Pokemon details, identified by name (or Id), and translate its description into something funny
let GetTranslatedPokemonAsync logger name = 
    let f() = PokeApi.fetchTranslatedPokemon logger name
    startAsyncFunctionAsTask f