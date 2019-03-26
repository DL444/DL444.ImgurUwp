# ImgUn
__Imgur app on Universal Windows Platform.__

## How to build
1. The solution was built with Visual Studio 2017, targeting .NET Standard 2.0 and Windows SDK version 17763. Make sure you have the appropriate tools installed, then clone the repository.
2. To make API calls, you need to register with Imgur and obtain a set of API key. For more information on this step please refer to [their documentation](https://apidocs.imgur.com/).
3. Create a text file with name `ApiKey.json` in the `Assets` folder of the app. This file will supply the keys you obtained in the previous step to the app. See below for information on the content of this file.
4. After creating the previous file, set the Build Action property of this file to `Content` in the Properties window.
5. Now you should be able to build and run the app.
## Content of `ApiKey.json`
Example:
```json
{
  "clientId": "000000000000000",
  "clientSecret": "0000000000000000000000000000000000000000",
  "mashapeKey": "000000000000000000000000000000000000000000000000000",
  "host": "api.imgur.com"
}
```
- `clientId`: The client ID received after you registered your app.
- `clientSecret`: The client secret key received after you registered your app.
- `mashapeKey`: If you want to use the Imgur API commercially, you have to register and pay through RapidAPI (previously Mashape). After registering there, you will receive a `X-RapidAPI-Key` string to be used in request header. Supply that key here. 
- `host`: The API host you want to use in the app. If you use the non-commercial API from Imgur, the host should be `api.imgur.com`. If you use the commercial API, it should be `imgur-apiv3.p.rapidapi.com`.
## Development roadmap
__Private Beta 0.1.0__  
- [x] Infrastructure: Model, API  
- [x] Gallery View  
- [x] Gallery Item View  
- [x] Commenting  
- [x] Upload  
- [x] Share Target  
- [x] Tags  
- [x] My Posts  
- [ ] My Private Uploads (in work)  
- [x] User Account  
- [ ] Settings  
- [ ] Adaptive Design  
- [ ] Live Tiles  
- [ ] Error Handling  
- [ ] Localization  

__Public Beta 0.2.0__  
- [ ] Anonymous Mode  
- [ ] Chat + Notification  
- [ ] Inking + Editor  
- [ ] Memegen  
- [ ] Themes  
- [ ] Content Translation  
- [ ] Etag Optimization  
- [ ] Accessibility  
- [ ] Keyboard shortcut  
## Known issues
- It is said that Imgur might block some China users from completing OAuth authentication, and Imgur is inaccessible for some China users. If you are in China and are experiencing issues, please try again from _outside_ of China.
