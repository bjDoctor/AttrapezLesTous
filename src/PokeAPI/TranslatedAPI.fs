//////
/// Module handling calls to the Translation API to get sentences tranlated like a master Jedi or the Bard of Avon
//////

module Core.YodaAPI

open Core.JsonProviders
open Core.Utilities

// Discriminated union to distinguish the traduction types
type Translation = 
    | Yoda 
    | Shakespeare

let private _funTranslationsBaseUrl = "https://api.funtranslations.com/translate"
let private _yodaBaseRequest =  _funTranslationsBaseUrl +/ "yoda.json?text="
let private _shakespeareBaseRequest = _funTranslationsBaseUrl +/ "shakespeare.json?text="

let private makeUrl input = function
    | Yoda _ -> _yodaBaseRequest + System.Net.WebUtility.UrlEncode(input)
    | Shakespeare _ -> _shakespeareBaseRequest + System.Net.WebUtility.UrlEncode(input)
    

let fetchTranslation input translation = 
    async {
        let! rawTranslation = TranslatedProvider.AsyncLoad(makeUrl input translation)
        
        return rawTranslation.Contents.Translated
    }
