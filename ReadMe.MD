#TwitchAuthWPF

This is a sample project to show how to get a Twitch oAuth access token using WPF and C#

The sample uses Newtonsoft JSON to handle JSON data and so you may need to install it. Install using NuGet like this:
Install-Package Newtonsoft.Json -Version 9.0.1

The tool is based on a simple WPF project created using Visual Studio 2015 Community

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

Please Note, I have included a client secret in the code, this secret is no longer valid and so the sample will not work as it stands. To fix this you need to create a new Twitch Developer Application and update the values at the top of program.cs. You'll also need to amend the URL in MainWindow.xaml.cs

Licenses

This software is release under the MIT License, please included all Licences (there are two) if you do anything with this piece of software.

MITLicense.txt applies to everything in Server.cs
MITLicense_a.txt applies to the rest of the code in this solution