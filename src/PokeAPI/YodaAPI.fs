//////
/// Module handling calls to the Yoda API to get sentences tranlated like a master Jedi
//////

module APIs.YodaAPI

open APIs.JsonProviders

let private yodaBaseUrl = "https://api.funtranslations.com/translate/yoda.json?text="

let yodaTranslateAsync input = 
    async {
        let url = yodaBaseUrl + System.Net.WebUtility.UrlEncode(input)
        let! yodaTranslation = YodaProvider.AsyncLoad(url)
        
        return yodaTranslation.Contents.Translated
    }
