#TwitchAuthWPF

This is a sample project to show how to get a Twitch oAuth access token using WPF and C#

The sample using Newtonsoft JSON to handle JSON datat and so you may need to install it. Install using NuGet like this:
Install-Package Newtonsoft.Json -Version 9.0.1

The tool is based on a simple WPF project created using Visual Studio 20165 Community
The code for the server is courtesy of David:
https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server

The code does the following:

1) Spin up your web server listening to the url you configured as your redirect url in your Twitch application settings
2) Send the user to the Auth URL using your WebBrowser control
3) The user will have to enter their credentials, and maybe Two Factor auth details
4) On a successful attempt your web server should then get a callback
5) The callback for Authorization Code Flow contains a code
6) Make the final POST to Twitch and get the oAuth token back in the Json response.

One thing I noticed is that sometimes the WebBrowser control will show an error (see CookieError.png) you can just ignore this and choose Yes to continue running scripts.