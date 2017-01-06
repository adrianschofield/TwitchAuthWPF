using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net; //required for HttpListenerRequest
using System.IO; //required for Streaming requests and responses
using System.Web; //required for HttpUtility - don't forget to add a Reference
using Newtonsoft.Json; 

namespace TwitchAuthWPF
{
    class Program
    {
        public MainWindow main;
        //Configuration from your Twitch Application replace these with the values from
        //Twitch, Settings, Connections, Developer Applicatioon (scroll down)

        private const string twitchClientId = "t6ma91fizbc7hudx6n5z60u9vb9h42";
        private const string twitchClientSecret = "uuktleakynfo887oxt8a8h6vzfnc1s";
        private const string twitchRedirectUri = "http://localhost:8080/twitch/callback";
        private const string twichScope = "channel_read";

        public void DoWork()
        {
            
            //Start the webserver to listen for the Twitch auth callback
            //The second parameter is called a prefix, this needs to match your Twitch Application Redirect URI
            //Make sure the URL ends in a /
            try
            {
                //main.textBox.Text = "Working...";
                WebServer ws = new WebServer(this.SendResponse, twitchRedirectUri + "/");
                ws.Run();
            }
            catch(Exception ex)
            {
                this.main.textBoxContent = string.Format("Ex: {0}", ex.Message);
                throw ex;
            }

        }

        public string SendResponse(HttpListenerRequest request)
        {
            //We got the response, because this is authorization flow we need to do some more work with the code returned
            //The function below does the final POST to Twitch to grab the oAuth data
            //We expect one query parameter in the response which should be ?code= so we can easiy extract this from the request

            this.TwitchAuthorizationApi(request.QueryString.GetValues("code")[0]);

            //We need to put something "pretty" in our WebBrowser window to show the user they were Authorized
            //This is in a string.Format as I was passing test values back to the page

            return string.Format("<HTML><BODY>Thanks for allowing TwitchAuthWPF to authenticate<br></BODY></HTML>");
        }


        public void TwitchAuthorizationApi(string code)
        {
            HttpWebRequest myWebRequest = null;
            ASCIIEncoding encoding = new ASCIIEncoding();
            Dictionary<string, string> postDataDictionary = new Dictionary<string, string>();

            //We need to prepare the POST data ahead of time, Add each entry required by the Twitch Authorization Code Flow
            //Then spin through URLEncoding the keys and values and joining them into one string using & and =

            postDataDictionary.Add("client_id", twitchClientId);
            postDataDictionary.Add("client_secret", twitchClientSecret);
            postDataDictionary.Add("grant_type", "authorization_code");
            postDataDictionary.Add("redirect_uri", twitchRedirectUri);
            postDataDictionary.Add("state", "123456");
            postDataDictionary.Add("code", code);

            string postData = "";

            foreach (KeyValuePair<string, string> kvp in postDataDictionary)
            {
                postData += HttpUtility.UrlEncode(kvp.Key) + "=" + HttpUtility.UrlEncode(kvp.Value) + "&";
            }
            
            //We need the POST data as a byte array, using ASCII encoding to keep things simple

            byte[] byte1 = encoding.GetBytes(postData);

            //OK set up our request for the final step in the Authorization Code Flow
            //This is the destination URI as described in https://dev.twitch.tv/docs/v5/guides/authentication/

            myWebRequest = WebRequest.CreateHttp("https://api.twitch.tv/kraken/oauth2/token");

            //This request is a POST with the required content type

            myWebRequest.Method = "POST";
            myWebRequest.ContentType = "application/x-www-form-urlencoded";

            //Set the request length based on our byte array

            myWebRequest.ContentLength = byte1.Length;

            //Things can go wrong here so let's do some sensible exception handling, this sample is
            //short lived but best practice is to manage the POST and response

            //POST

            Stream postStream = null;

            try
            {
                //Set up the request and write the data this should complete the POST

                postStream = myWebRequest.GetRequestStream();
                postStream.Write(byte1, 0, byte1.Length);
            }
            catch (Exception ex)
            {
                //We should log any exception here but I am just going to supress them for this sample
                this.main.textBoxContent = string.Format("Ex: {0}", ex.Message);
                throw ex;
            }
            finally
            {
                postStream.Close();
            }

            //response to POST

            Stream responseStream = null;
            StreamReader responseStreamReader = null;
            WebResponse response = null;
            string jsonResponse = null;

            try
            {
                //Wait for the response from the POST above and get a stream with the data

                response = myWebRequest.GetResponse();
                responseStream = response.GetResponseStream();

                //Read the response, if everything worked we'll have our JSON encoded oauth token
                responseStreamReader = new StreamReader(responseStream);
                jsonResponse = responseStreamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                //We should log any exception here but I am just going to supress them for this sample
                this.main.textBoxContent = string.Format("Ex: {0}", ex.Message);
                throw ex;
            }
            finally
            {
                responseStreamReader.Close();
                responseStream.Close();
                response.Close();
            }

            //We got the jsonResponse from Twitch let's Deserialize it,
            //I'm using Newtonsoft - Install-Package Newtonsoft.Json -Version 9.0.1
            //Class for deserializing is defined below

            TwitchAuthResponse myAuthResponse = null;

            try
            {
                myAuthResponse = JsonConvert.DeserializeObject<TwitchAuthResponse>(jsonResponse);
            }
            catch(Exception ex)
            {
                this.main.textBoxContent = string.Format("Ex: {0}", ex.Message);
                throw ex;
            }
            
            //Update the MainWindow TextBox with the access_token
            //You never need to display the access_token in a real world situation, just grab it and use
            //it in your authenticated Twitch API requests

            this.main.textBoxContent = string.Format("Token: {0}",myAuthResponse.access_token);

            return;
        }
    }


    //A class to represent the reponse from the Twitch auth call just to make it easy
    //to deserialize the oAuth access token

    public class TwitchAuthResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public List<string> scope { get; set; }
    }
}
