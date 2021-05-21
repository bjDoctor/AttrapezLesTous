//////
/// Module handling calls to the Translation API to get sentences tranlated like a master Jedi or the Bard of Avon
//////

module APIs.YodaAPI

open APIs.JsonProviders

// Discriminated union to distinguish the traduction types
type Translation = 
    | Yoda 
    | Shakespeare

let private yodaBaseUrl = "https://api.funtranslations.com/translate/yoda.json?text="
let private shakespeareBaseUrl = "https://api.funtranslations.com/translate/shakespeare.json?text="

let private makeUrl input = function
    | Yoda _ -> yodaBaseUrl + System.Net.WebUtility.UrlEncode(input)
    | Shakespeare _ -> shakespeareBaseUrl + System.Net.WebUtility.UrlEncode(input)
    

let fetchTranslation input translation = 
    async {
        let url = makeUrl input translation
        let! translation = TranslatedProvider.AsyncLoad(url)
        
        return translation.Contents.Translated
    }
