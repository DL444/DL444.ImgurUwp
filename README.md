# ImgurUwp
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
  "mashapeKey": "000000000000000000000000000000000000000000000000000"
}
```
- `clientId`: The client ID received after you registered your app.
- `clientSecret`: The client secret key received after you registered your app.
- `mashapeKey`: If you want to use the Imgur API commercially, you have to register and pay through RapidAPI (previously Mashape). After registering there, you will receive a `X-RapidAPI-Key` string to be used in request header. Supply that key here. 
## Known issues
- It is said that Imgur might block some China users from completing OAuth authentication. Personal experiment also suggests so. If you are in China and are experiencing authentication issues, please try again from _outside_ of China.