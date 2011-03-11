using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace FSquareCSharp
{
    public partial class FSquare : System.Web.UI.Page
    {
        public string _consumerkey;
        public string _consumersecret;
        public oAuth4Square oAuth;
        public string fsquareauthenticateurl;
        public string fname;
        public string lname;
        public string jsonresponse;
        protected void Page_Load(object sender, EventArgs e)
        {
            oAuth = new oAuth4Square();
            oAuth.ConsumerKey = "BOQ05D3OZZR01KCUKX10VS2LQ1Y0XFTYF20LQLQW4LBZQ2EG";
            oAuth.ConsumerSecret = "WZM1N5QMKNZQB3XOJ3RF4XXUJBFMCLNFMVHS0ZGAH1EVSNTH";
            oAuth.CallBackUrl = "http://ravmemcacheme.com:15120/FSquare.aspx";
            if (Request.QueryString["code"] == null)
            {
                fsquareauthenticateurl = oAuth.oAuthRequestToken + "?" + "client_id=" + oAuth.ConsumerKey + "&response_type=code&redirect_uri=" + oAuth.CallBackUrl;
            }
            else
            {
                string retjson;
                string tokenvalue;
                FSquareToken fstoken = new FSquareToken();
                try
                {
                    retjson = oAuth.oAuthRequest(Request.QueryString["code"]);
                    fstoken = GetFSquareTokenDetails(retjson);
                    //the authenticated token we get back from foursquare
                    tokenvalue = fstoken.AccessToken;

                    //now that we have the token, lets start making requests to the API
                    string apiurl = "https://api.foursquare.com/v2/users/self";
                    retjson = oAuth.oAuthRequest(apiurl, tokenvalue);
                    jsonresponse = retjson;
                }
                catch (Exception oe)
                {
                    Response.Write("Begin -- " + oe.Message + "--" + oe.StackTrace);
                }
            }

        }
        private FSquareToken GetFSquareTokenDetails(string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(FSquareToken));
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                FSquareToken list = (FSquareToken)serializer.ReadObject(stream);
                return list;
            }
        }
    }
    [DataContract()]
    public class FSquareToken
    {
        private string f_accesstoken;
        [DataMember(Name = "access_token")]
        public string AccessToken
        {
            get { return this.f_accesstoken; }

            set { this.f_accesstoken = value; }
        }
    }
}