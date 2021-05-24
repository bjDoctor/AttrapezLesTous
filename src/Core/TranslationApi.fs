namespace Core

open Core.JsonProviders
open Core.Utilities
open Core.Domain

//////
/// Internal module handling calls to the Translation API to get sentences tranlated like a master Jedi or the Bard of Avon
/// Since it consistes of retrieving data through the internet, this module is not tested at a unit level
//////

module internal TranslationApi = 
    // Constants
    let private _funTranslationsBaseUrl = "https://api.funtranslations.com/translate"
    let private _yodaBaseRequest =  _funTranslationsBaseUrl +/ "yoda.json?text="
    let private _shakespeareBaseRequest = _funTranslationsBaseUrl +/ "shakespeare.json?text="
    
    /// Non-pure helper function, depends on private constants
    let private makeUrl input = function
        | Yoda _ -> _yodaBaseRequest + System.Net.WebUtility.UrlEncode(input)
        | Shakespeare _ -> _shakespeareBaseRequest + System.Net.WebUtility.UrlEncode(input)
        
    
    /// Uses the funtranslations API to asynchronously translates an input into a given dialect
    let fetchTranslation input translation = 
        async {
            let! rawTranslation = TranslationProvider.AsyncLoad(makeUrl input translation)
            
            return rawTranslation.Contents.Translated
        }

