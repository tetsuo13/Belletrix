﻿#region license
// http://www.gnu.org/licenses/gpl-3.0.html GPL v3 or later
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Globalization;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Piwik - Open source web analytics
/// For more information, see http://piwik.org
/// </summary>
namespace Piwik.Tracker
{
    using System.IO;
    using System.Web.Routing;

    /// <summary>
    /// Piwik - Open source web analytics
    ///
    /// Client to record visits, page views, Goals, Ecommerce activity (product views, add to carts, Ecommerce orders) in a Piwik server.
    /// This is a PHP Version of the piwik.js standard Tracking API.
    /// For more information, see http://piwik.org/docs/tracking-api/
    /// </summary>
    public class PiwikTracker
    {
        /// <summary>
        /// API Version
        /// </summary>
        private const int VERSION = 1;

        /// <summary>
        /// Visitor ID length
        /// </summary>
        private const int LENGTH_VISITOR_ID = 16;

        /// <summary>
        /// Piwik base URL, for example http://example.org/piwik/
        /// Must be set before using the class by calling PiwikTracker.URL = 'http://yourwebsite.org/piwik/'
        /// </summary>
        public static string URL;

        private string DEBUG_APPEND_URL;
        private bool cookieSupport;
        private string userAgent;
        private DateTimeOffset localTime;
        private bool hasCookies;
        private string plugins;
        private Dictionary<string, string[]> visitorCustomVar;
        private Dictionary<string, string[]> pageCustomVar;
        private string customData;
        private DateTimeOffset forcedDatetime;
        private string token_auth;
        private AttributionInfo attributionInfo;
        private DateTimeOffset ecommerceLastOrderTimestamp;
        private Dictionary<string, object[]> ecommerceItems;
        private Cookie requestCookie;
        private int idSite;
        private string urlReferrer;
        private string pageUrl;
        private string ip;
        private string acceptLanguage;
        private string visitorId;
        private string forcedVisitorId;
        private int width;
        private int height;
        private int requestTimeout;
        private bool doBulkRequests;
        private List<string> storedTrackingActions;

        public enum ActionType {download, link};

        /// <summary>
        /// Builds a PiwikTracker object, used to track visits, pages and Goal conversions
        /// for a specific website, by using the Piwik Tracking API.
        /// 
        /// If the tracker is used within a web page or web controller, the following information are pre-initialised : 
        /// URL Referrer, current page URL, remote IP, Accept-Language HTTP header and User-Agent HTTP header.
        /// </summary>       
        /// <param name="idSite">Id site to be tracked</param>
        /// <param name="apiUrl">"http://example.org/piwik/" or "http://piwik.example.org/". If set, will overwrite PiwikTracker.URL</param>
        public PiwikTracker(int idSite, string apiUrl = null)
        {
            this.cookieSupport = false;
            this.userAgent = null;
            this.localTime = DateTimeOffset.MinValue;
            this.hasCookies = false;
            this.plugins = null;
            this.visitorCustomVar = new Dictionary<string, string[]>();
            this.pageCustomVar =  new Dictionary<string, string[]>();
            this.customData = null;
            this.forcedDatetime = DateTimeOffset.MinValue;
            this.token_auth = null;
            this.attributionInfo = null;
            this.ecommerceLastOrderTimestamp = DateTimeOffset.MinValue;
            this.ecommerceItems =  new Dictionary<string, object[]>();
            this.requestCookie = null;
            this.idSite = idSite;
            
            var currentContext = HttpContext.Current;
            if (currentContext != null)
            {
                if (currentContext.Request.UrlReferrer != null)
                {
                    this.urlReferrer = currentContext.Request.UrlReferrer.AbsoluteUri;
                }
                
                this.ip = currentContext.Request.UserHostAddress;

                if (currentContext.Request.UserLanguages != null && currentContext.Request.UserLanguages.Any())
                    this.acceptLanguage = currentContext.Request.UserLanguages.First();

                this.userAgent = currentContext.Request.UserAgent;
            }
            this.pageUrl = getCurrentUrl();
            if (!String.IsNullOrEmpty(apiUrl))
            {
                URL = apiUrl;
            }
            this.setNewVisitorId();

		    // Allow debug while blocking the request
    	    this.requestTimeout = 600;
    	    this.doBulkRequests = false;
    	    this.storedTrackingActions = new List<string>();
        }


        /// <summary>
        /// Sets the current URL being tracked
        /// </summary>       
        /// <param name="url">Raw URL (not URL encoded)</param>
        public void setUrl(string url)
        {
            pageUrl = url;
        }


        /// <summary>
        /// Sets the URL referrer used to track Referrers details for new visits.
        /// </summary>       
        /// <param name="url">Raw URL (not URL encoded)</param>
        public void setUrlReferrer(string url)
        {
            urlReferrer = url;
        }


        /// <summary>
        /// Sets the attribution information to the visit, so that subsequent Goal conversions are 
        /// properly attributed to the right Referrer URL, timestamp, Campaign Name & Keyword.
        /// </summary>       
        /// <param name="attributionInfo">Attribution info for the visit</param>
        public void setAttributionInfo(AttributionInfo attributionInfo)
        {
            this.attributionInfo = attributionInfo;
        }


        /// <summary>
        /// Sets Visit Custom Variable.
        /// See http://piwik.org/docs/custom-variables/
        /// </summary>       
        /// <param name="id">Custom variable slot ID from 1-5</param>
        /// <param name="name">Custom variable name</param>
        /// <param name="value">Custom variable value</param>
        /// <param name="scope">Custom variable scope. Possible values: CustomVar.Scopes</param>
        public void setCustomVariable(int id, string name, string value, CustomVar.Scopes scope = CustomVar.Scopes.visit)
        {
            string stringId = Convert.ToString(id);
            string[] customVar = {name, value};

            switch (scope)
            {
                case CustomVar.Scopes.page:
                    pageCustomVar.Add(stringId, customVar);
                    break;

                case CustomVar.Scopes.visit:
                    visitorCustomVar.Add(stringId, customVar);
                    break;

                default:
                    throw new Exception("Unimplemented scope");
            }
        }


        /// <summary>
        /// Returns the currently assigned Custom Variable stored in a first party cookie.
        /// 
        /// This function will only work if the user is initiating the current request, and his cookies
        /// can be read from an active HttpContext.
        /// </summary>       
        /// <param name="id">Custom Variable integer index to fetch from cookie. Should be a value from 1 to 5</param>
        /// <param name="scope">Custom variable scope. Possible values: visit, page</param> 
        /// <returns>The requested custom variable</returns>
        public CustomVar getCustomVariable(int id, CustomVar.Scopes scope = CustomVar.Scopes.visit)
        {
            var stringId = Convert.ToString(id);

            if (scope.Equals(CustomVar.Scopes.page))
            {
                return pageCustomVar.ContainsKey(stringId) ? new CustomVar(pageCustomVar[stringId][0], pageCustomVar[stringId][1]) : null;
            }
            else if (!scope.Equals(CustomVar.Scopes.visit))
            {
                throw new Exception("Invalid 'scope' parameter value");
            }
            if (visitorCustomVar.ContainsKey(stringId))
            {
                return new CustomVar(visitorCustomVar[stringId][0], visitorCustomVar[stringId][1]);
            }
            var customVariablesCookie = "cvar." + idSite + ".";
            var cookie = getCookieMatchingName(customVariablesCookie);
            if (cookie == null)
            {
                return null;
            }
            var cookieDecoded = new JavaScriptSerializer().Deserialize<Dictionary<string, string[]>>(HttpUtility.UrlDecode(cookie.Value));
            if(!cookieDecoded.ContainsKey(stringId)    		
    		    || cookieDecoded[stringId].Count() != 2)
    	    {
    		    return null;
    	    }
            return new CustomVar(cookieDecoded[stringId][0], cookieDecoded[stringId][1]);
        }

        
         /// <summary>
        /// Sets the current visitor ID to a random new one.
        /// </summary>       
        public void setNewVisitorId()
        {
            var encodedGuidBytes = new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.Default.GetBytes(Guid.NewGuid().ToString()));
            visitorId = BitConverter.ToString(encodedGuidBytes).Replace("-", "").Substring(0, LENGTH_VISITOR_ID);
        }
    

        /// <summary>
        /// Sets the Browser language. Used to guess visitor countries when GeoIP is not enabled
        /// </summary>       
        /// <param name="acceptLanguage">For example "fr-fr"</param>    
        public void setBrowserLanguage( string acceptLanguage )
        {
            this.acceptLanguage = acceptLanguage;
        }


        /// <summary>
        /// Sets the user agent, used to detect OS and browser.
        /// If this function is not called, the User Agent will default to the current user agent
        /// if there is an active HttpContext
        /// </summary>       
        /// <param name="userAgent">HTTP User Agent</param>  
        public void setUserAgent(string userAgent)
        {
            this.userAgent = userAgent;
        }
        
        /// <summary>
        /// Enables the bulk request feature. When used, each tracking action is stored until the
        /// doBulkTrack method is called. This method will send all tracking data at once.
        /// </summary>     
	    public void enableBulkTracking()
	    {
		    this.doBulkRequests = true;
	    }

        /// <summary>
        /// Tracks a page view
        /// </summary>       
        /// <param name="documentTitle">Page title as it will appear in the Actions > Page titles report</param> 
        /// <returns>HTTP Response from the server or null if using bulk requests.</returns>
        public HttpWebResponse doTrackPageView(string documentTitle = null)
        {
            string url = getUrlTrackPageView(documentTitle);
            return sendRequest(url);
        }


        /// <summary>
        /// Records a Goal conversion
        /// </summary>       
        /// <param name="idGoal">Id Goal to record a conversion</param> 
        /// <param name="revenue">Revenue for this conversion</param> 
        /// <returns>HTTP Response from the server or null if using bulk requests.</returns>
        public HttpWebResponse doTrackGoal(int idGoal, float revenue = float.MinValue)
        {
    	    string url = getUrlTrackGoal(idGoal, revenue);
    	    return sendRequest(url);
        }


        /// <summary>
        /// Tracks a download or outlink
        /// </summary>       
        /// <param name="actionUrl">URL of the download or outlink</param> 
        /// <param name="actionType">Type of the action: 'download' or 'link'</param> 
        /// <returns>HTTP Response from the server or null if using bulk requests.</returns>
        public HttpWebResponse doTrackAction(string actionUrl, ActionType actionType)
        {
            // Referrer could be udpated to be the current URL temporarily (to mimic JS behavior)
    	    string url = getUrlTrackAction(actionUrl, actionType);
    	    return sendRequest(url); 
        }


        /// <summary>
        /// Adds an item in the Ecommerce order.
        /// 
        /// This should be called before doTrackEcommerceOrder(), or before doTrackEcommerceCartUpdate().
        /// This function can be called for all individual products in the cart (or order).
        /// SKU parameter is mandatory. Other parameters are optional (set to false if value not known).
        /// Ecommerce items added via this function are automatically cleared when doTrackEcommerceOrder() or getUrlTrackEcommerceOrder() is called.
        /// </summary>       
        /// <param name="sku">SKU, Product identifier </param> 
        /// <param name="name">Product name</param> 
        /// <param name="categories">Array of product categories (up to 5 categories can be specified for a given product)</param> 
        /// <param name="price"> Individual product price (supports integer and decimal prices)</param> 
        /// <param name="quantity">Product quantity. If not specified, will default to 1 in the Reports</param> 
        public void addEcommerceItem(string sku, string name = null, List<string> categories = null, double price = 0, UInt64 quantity = 1)
        {
    	    if(String.IsNullOrEmpty(sku))
    	    {
    		    throw new Exception("You must specify a SKU for the Ecommerce item");
    	    }

            object[] eCommerceItem = { sku, name, categories, formatMonetaryValue(price), quantity };

            ecommerceItems.Remove(sku);
            ecommerceItems.Add(sku, eCommerceItem);
        }


        /// <summary>
	    /// Tracks a Cart Update (add item, remove item, update item).
	    /// 
	    /// On every Cart update, you must call addEcommerceItem() for each item (product) in the cart, 
	    /// including the items that haven't been updated since the last cart update.
	    /// Items which were in the previous cart and are not sent in later Cart updates will be deleted from the cart (in the database).
        /// </summary>       
        /// <param name="grandTotal">Cart grandTotal (typically the sum of all items' prices)</param> 
        public HttpWebResponse doTrackEcommerceCartUpdate(double grandTotal)
        {
    	    string url = getUrlTrackEcommerceCartUpdate(grandTotal);
    	    return sendRequest(url); 
        }

        /// <summary>
        /// Sends all stored tracking actions at once. Only has an effect if bulk tracking is enabled.
        /// 
        /// To enable bulk tracking, call enableBulkTracking().
        /// </summary>       
        /// <returns>Response</returns>
        public HttpWebResponse doBulkTrack()
        {
            if (string.IsNullOrWhiteSpace(this.token_auth))
    	    {
    		    throw new Exception("Token auth is required for bulk tracking.");
    	    }
    	
    	    if (!this.storedTrackingActions.Any())
    	    {
    		    return null;
    	    }
    	
    	    var data = new Dictionary<string, Object>();
            data["requests"] = this.storedTrackingActions;
            data["token_auth"] = this.token_auth;
    	
    	    var postData = new JavaScriptSerializer().Serialize(data);
    	    var response = this.sendRequest(this.getBaseUrl(), "POST", postData, true);
    	
    	    this.storedTrackingActions = new List<string>();
    	
    	    return response;
        }

        /// <summary>
	    /// Tracks an Ecommerce order.
	    /// 
	    /// If the Ecommerce order contains items (products), you must call first the addEcommerceItem() for each item in the order.
	    /// All revenues (grandTotal, subTotal, tax, shipping, discount) will be individually summed and reported in Piwik reports.
	    /// Only the parameters $orderId and $grandTotal are required. 
        /// </summary>       
        /// <param name="orderId">Unique Order ID. This will be used to count this order only once in the event the order page is reloaded several times. orderId must be unique for each transaction, even on different days, or the transaction will not be recorded by Piwik.</param> 
        /// <param name="grandTotal">Grand Total revenue of the transaction (including tax, shipping, etc.)</param> 
        /// <param name="subTotal">Sub total amount, typically the sum of items prices for all items in this order (before Tax and Shipping costs are applied)</param> 
        /// <param name="tax">Tax amount for this order</param> 
        /// <param name="shipping">Shipping amount for this order</param> 
        /// <param name="discount">Discounted amount in this order</param> 
        /// <returns>HTTP Response from the server or null if using bulk requests.</returns>
        public HttpWebResponse doTrackEcommerceOrder(string orderId, double grandTotal, double subTotal = Double.MinValue, double tax = Double.MinValue, double shipping = Double.MinValue, double discount = Double.MinValue)
        {
    	    string url = getUrlTrackEcommerceOrder(orderId, grandTotal, subTotal, tax, shipping, discount);
    	    return sendRequest(url); 
        }


        /// <summary>
        /// Sets the current page view as an item (product) page view, or an Ecommerce Category page view.
        /// 
        /// This must be called before doTrackPageView() on this product/category page. 
        /// It will set 3 custom variables of scope "page" with the SKU, Name and Category for this page view.
        /// Note: Custom Variables of scope "page" slots 3, 4 and 5 will be used.
        ///  
        /// On a category page, you may set the parameter $category only and set the other parameters to false.
        /// 
        /// Tracking Product/Category page views will allow Piwik to report on Product & Categories 
        /// conversion rates (Conversion rate = Ecommerce orders containing this product or category / Visits to the product or category)
        ///
        /// </summary>       
        /// <param name="sku">Product SKU being viewed</param> 
        /// <param name="name">Product Name being viewed</param> 
        /// <param name="categories">Category being viewed. On a Product page, this is the product's category. You can also specify an array of up to 5 categories for a given page view.</param> 
        /// <param name="price">Specify the price at which the item was displayed</param> 
        public void setEcommerceView(string sku = null, string name = null, List<string> categories = null, double price = double.MinValue)
        {
            var serializedCategories = "";
            if (categories != null)
            {
                serializedCategories = new JavaScriptSerializer().Serialize(categories);
            }
            setCustomVariable(5, "_pkc", serializedCategories, CustomVar.Scopes.page);

            if (!price.Equals(Double.MinValue))
            {
                setCustomVariable(2, "_pkp", formatMonetaryValue(price), CustomVar.Scopes.page);
            }

            // On a category page, do not record "Product name not defined" 
            if (String.IsNullOrEmpty(sku) && String.IsNullOrEmpty(name))
            {
                return;
            }
            if (!String.IsNullOrEmpty(sku))
            {
                setCustomVariable(3, "_pks", sku, CustomVar.Scopes.page);
            }
            if (String.IsNullOrEmpty(name))
            {
                name = "";
            }
            setCustomVariable(4, "_pkn", name, CustomVar.Scopes.page);
        }


        /// <summary>
        /// Returns URL used to track Ecommerce Cart updates
        /// Calling this function will reinitializes the property ecommerceItems to empty array 
        /// so items will have to be added again via addEcommerceItem()  
        /// </summary>        
        private string getUrlTrackEcommerceCartUpdate(double grandTotal)
        {
        	return getUrlTrackEcommerce(grandTotal);
        }


        /// <summary>
        /// Returns URL used to track Ecommerce Orders
        /// Calling this function will reinitializes the property ecommerceItems to empty array 
        /// so items will have to be added again via addEcommerceItem()  
        /// </summary>  
        public string getUrlTrackEcommerceOrder(string orderId, double grandTotal, double subTotal = Double.MinValue, double tax = Double.MinValue, double shipping = Double.MinValue, double discount = Double.MinValue)
        {
    	    if(String.IsNullOrEmpty(orderId))
    	    {
    		    throw new Exception("You must specifiy an orderId for the Ecommerce order");
    	    }

    	    string url = getUrlTrackEcommerce(grandTotal, subTotal, tax, shipping, discount);
    	    url += "&ec_id=" + urlEncode(orderId);

    	    ecommerceLastOrderTimestamp = forcedDatetime.Equals(DateTimeOffset.MinValue) ? DateTimeOffset.Now : forcedDatetime;

    	    return url;
        }


        /// <summary>
        /// Returns URL used to track Ecommerce orders
        /// Calling this function will reinitializes the property ecommerceItems to empty array 
        /// so items will have to be added again via addEcommerceItem()  
        /// </summary>  
        protected string getUrlTrackEcommerce(double grandTotal, double subTotal = Double.MinValue, double tax = Double.MinValue, double shipping = Double.MinValue, double discount = Double.MinValue)
        {
    	
    	    string url = getRequest( idSite ) + "&idgoal=0&revenue="  + formatMonetaryValue(grandTotal);

            if (!subTotal.Equals(Double.MinValue))
    	    {
    		    url += "&ec_st=" + formatMonetaryValue(subTotal);
    	    }

            if (!tax.Equals(Double.MinValue))
    	    {
    		    url += "&ec_tx=" + formatMonetaryValue(tax);
    	    }

            if (!shipping.Equals(Double.MinValue))
    	    {
    		    url += "&ec_sh="  + formatMonetaryValue(shipping);
    	    }

            if (!discount.Equals(Double.MinValue))
    	    {
    		    url += "&ec_dt=" + formatMonetaryValue(discount);
    	    }

    	    if(ecommerceItems.Count > 0)
    	    {
    		    url += "&ec_items=" + urlEncode(new JavaScriptSerializer().Serialize(ecommerceItems.Values));                
    	    }

            ecommerceItems = new Dictionary<string, object[]>();

    	    return url;
        }


        /// <see>doTrackPageView</see>
        /// <param name="documentTitle">Page view name as it will appear in Piwik reports</param> 
        /// <returns>HTTP Response from the server</returns>
        public string getUrlTrackPageView(string documentTitle = null)
        {
            string url = getRequest(idSite);

            if (!String.IsNullOrEmpty(documentTitle))
            {
                url += "&action_name=" + urlEncode(documentTitle);
            }

            return url;
        }


        /// <param name="idGoal">Id Goal to record a conversion</param> 
        /// <param name="revenue">Revenue for this conversion</param> 
        /// <returns>URL to piwik.php with all parameters set to track the goal conversion</returns>
        public string getUrlTrackGoal(int idGoal, float revenue)
        {
    	    string url = getRequest( idSite );

            url += "&idgoal=" + idGoal;

    	    if(!revenue.Equals(float.MinValue)) {
                url += "&revenue=" + formatMonetaryValue(revenue);
    	    }

    	    return url;
        }


        /// <param name="actionUrl">URL of the download or outlink</param> 
        /// <param name="actionType">Type of the action: 'download' or 'link'</param> 
        /// <returns>URL to piwik.php with all parameters set to track an action</returns>
        public string getUrlTrackAction(string actionUrl, ActionType actionType)
        {
    	    var url = getRequest( idSite );
		    url += "&" + actionType + "=" + urlEncode(actionUrl);		
    	    return url;
        }


        /// <summary>
        /// Overrides server date and time for the tracking requests. 
        /// By default Piwik will track requests for the "current datetime" but this function allows you 
        /// to track visits in the past. All times are in UTC.
        /// 
        /// Allowed only for Super User, must be used along with setTokenAuth()
        /// </summary>
        /// <param name="dateTime">Date to set</param>
        public void setForceVisitDateTime(DateTimeOffset dateTime)
        {
            this.forcedDatetime = dateTime;
        }


        /// <summary>
        /// Overrides IP address
        /// 
        /// Allowed only for Super User, must be used along with setTokenAuth()
        /// </summary>
        /// <param name="ip">IP string, eg. 130.54.2.1</param>  
        public void setIp(string ip)
        {
            this.ip = ip;
        }     


        /// <summary>
        /// Forces the requests to be recorded for the specified Visitor ID
        /// rather than using the heuristics based on IP and other attributes.
        /// 
        /// This is typically used with the Javascript getVisitorId() function.
        /// 
        /// Allowed only for Super User, must be used along with setTokenAuth()
        /// </summary>       
        /// <param name="visitorId">16 hexadecimal characters visitor ID, eg. "33c31e01394bdc63"</param>          
        public void setVisitorId(string visitorId)
        {

        	if(visitorId.Length != LENGTH_VISITOR_ID)
    	    {
        		throw new Exception("setVisitorId() expects a " + LENGTH_VISITOR_ID + " characters ID");
        	}

    	    this.forcedVisitorId = visitorId;
        }


        /// <summary>
        /// If the user initiating the request has the Piwik first party cookie, 
        /// this function will try and return the ID parsed from this first party cookie.
        /// 
        /// If you call this function from a server, where the call is triggered by a cron or script
        /// not initiated by the actual visitor being tracked, then it will return 
        /// the random Visitor ID that was assigned to this visit object.
        /// 
        /// This can be used if you wish to record more visits, actions or goals for this visitor ID later on.
        /// </summary>       
        /// <returns>16 hex chars visitor ID string</returns>
        public string getVisitorId()
        {
    	    if(!String.IsNullOrEmpty(forcedVisitorId))
    	    {
    		    return forcedVisitorId;
    	    }
    	
    	    HttpCookie idCookie = getCookieMatchingName("id." + idSite + ".");

    	    if(idCookie != null)
    	    {
                string cookieVal = idCookie.Value;
    		    string parsedVisitorId = cookieVal.Substring(0, cookieVal.IndexOf("."));
    		    if(parsedVisitorId.Length == LENGTH_VISITOR_ID)
    		    {
    			    return parsedVisitorId;
    		    }
    	    }

    	    return visitorId;
        }


        /// <summary>
        /// Returns the currently assigned Attribution Information stored in a first party cookie.
        /// 
        /// This function will only work if the user is initiating the current request, and his cookies
        /// can be read from an active HttpContext.
        /// </summary>       
        /// <returns>Referer information for Goal conversion attribution</returns>        
        public AttributionInfo getAttributionInfo()
        {
            HttpCookie refCookie = getCookieMatchingName("ref." + idSite + ".");

            if(refCookie == null) {
                return null;
            }

            string[] cookieDecoded = new JavaScriptSerializer().Deserialize<string[]>(HttpUtility.UrlDecode(refCookie.Value));

            if(cookieDecoded == null) {
                return null;
            }

            int arraySize = cookieDecoded.Length;

            if(arraySize == 0) {
                return null;
            }

            AttributionInfo attributionInfo = new AttributionInfo();

            if(!String.IsNullOrEmpty(cookieDecoded[0])) {
                attributionInfo.campaignName = cookieDecoded[0];
            }

            if(arraySize > 1 && !String.IsNullOrEmpty(cookieDecoded[1])) {
                attributionInfo.campaignKeyword = cookieDecoded[1];
            }

            if(arraySize > 2 && !String.IsNullOrEmpty(cookieDecoded[2])) {
                attributionInfo.referrerTimestamp = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToInt32(cookieDecoded[2]));
            }

            if(arraySize > 3 && !String.IsNullOrEmpty(cookieDecoded[3])) {
                attributionInfo.referrerUrl = cookieDecoded[3];
            }

            return attributionInfo;            
        }


        /// <summary>
        /// Some Tracking API functionnality requires express authentication, using either the 
        /// Super User token_auth, or a user with 'admin' access to the website.
        /// 
        /// The following features require access:
        /// - force the visitor IP
        /// - force the date & time of the tracking requests rather than track for the current datetime
        /// - force Piwik to track the requests to a specific VisitorId rather than use the standard visitor matching heuristic
        /// 
        /// </summary>
        /// <param name="token_auth">32 chars token_auth string</param>
	    public void setTokenAuth(string token_auth)
	    {
    		this.token_auth = token_auth;
    	}


        /// <summary>
        /// Sets local visitor time
        /// </summary>
        /// <param name="localTime">Time to set</param>
        public void setLocalTime(DateTimeOffset localTime)
        {
            this.localTime = localTime;
        }


        /// <summary>
        /// Sets user resolution width and height.
        /// </summary>       
        public void setResolution(int width, int height)
        {
    	    this.width = width;
    	    this.height = height;
        }


        /// <summary>
        /// Sets if the browser supports cookies 
        /// This is reported in "List of plugins" report in Piwik.
        /// </summary>  
        public void setBrowserHasCookies( bool hasCookies )
        {
            this.hasCookies = hasCookies;
        }


        /// <summary>
        /// Will append a custom string at the end of the Tracking request. 
        /// </summary> 
        public void setDebugStringAppend( string debugString )
        {
            this.DEBUG_APPEND_URL = debugString;
        }


        /// <summary>
        /// Sets visitor browser supported plugins
        /// </summary>       
        public void setPlugins(BrowserPlugins browserPlugins)
        {
    	    plugins =
                "&fla=" + (browserPlugins.flash ? "1" : "0") +
                "&java=" + (browserPlugins.java ? "1" : "0") +
                "&dir=" + (browserPlugins.director ? "1" : "0") + 
    		    "&qt=" + (browserPlugins.quickTime ? "1" : "0") + 
    		    "&realp=" + (browserPlugins.realPlayer ? "1" : "0") + 
    		    "&pdf=" + (browserPlugins.pdf ? "1" : "0") + 
    		    "&wma=" + (browserPlugins.windowsMedia ? "1" : "0") + 
    		    "&gears=" + (browserPlugins.gears ? "1" : "0") + 
    		    "&ag=" + (browserPlugins.silverlight ? "1" : "0"); 
        }


        /// <summary>
        /// By default, PiwikTracker will read third party cookies 
        /// from the response and sets them in the next request.
        /// This can be disabled by calling this function.
        /// </summary>      
        public void disableCookieSupport()
        {
        	cookieSupport = false;
        }

        /// <summary>
        /// Returns the maximum number of seconds the tracker will spend waiting for a response
        /// from Piwik. Defaults to 600 seconds.
        /// </summary>   
        public int getRequestTimeout()
        {
    	    return this.requestTimeout;
        }
	
        /// <summary>
        /// Sets the maximum number of seconds that the tracker will spend waiting for a response
        /// from Piwik.
        /// </summary>
        /// <param name="timeout"></param>
        public void setRequestTimeout( int timeout )
        {
    	    if (timeout < 0)
    	    {
    		    throw new Exception("Invalid value supplied for request timeout: $timeout");
    	    }
    	
    	    this.requestTimeout = timeout;
        }

        private HttpWebResponse sendRequest(string url, string method = "GET", string data = null, bool force = false)
        {
    	    // if doing a bulk request, store the url
    	    if (this.doBulkRequests && !force)
    	    {
    		    this.storedTrackingActions.Add(
                    url
                    + (!String.IsNullOrEmpty(userAgent) ? "&ua=" + urlEncode(userAgent) : "")
                    + (!String.IsNullOrEmpty(acceptLanguage) ? "&lang=" + urlEncode(acceptLanguage) : "")
                );

    		    return null;
    	    }

		    if(!cookieSupport)
		    {
			    requestCookie = null;
		    }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.UserAgent = this.userAgent;            

            request.Headers.Add("Accept-Language", acceptLanguage);
            
            if(requestCookie != null)
            {
                request.Headers.Add("Cookie", requestCookie.Name + "=" + requestCookie.Value);
            }

            request.Timeout = this.requestTimeout;

            if (!string.IsNullOrEmpty(data))
            {
                request.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
            }

            var response = (HttpWebResponse) request.GetResponse();
            var cookies = response.Cookies;

            // The cookie in the response will be set in the next request
            if (cookies != null)
            {
                // in case several cookies returned, we keep only the latest one (ie. XDEBUG puts its cookie first in the list)
                for (var i = 0; i < cookies.Count; i++)
                {                    
                    // XDEBUG is a PHP Debugger
                    if (!cookies[i].Name.Contains("XDEBUG"))
                    {
                        requestCookie = cookies[i];
                    }
                }   
            }

		    return response;
        }

        /// <summary>
        /// Returns the base URL for the piwik server.
        /// </summary>
        protected string getBaseUrl()
        {
            if (String.IsNullOrEmpty(URL))
            {
                throw new Exception("You must first set the Piwik Tracker URL by calling PiwikTracker.URL = \"http://your-website.org/piwik/\";");
            }
            if (!URL.Contains("/piwik.php")
                && !URL.Contains("/proxy-piwik.php"))
            {
                URL += "/piwik.php";
            }
            return URL;
        }

        private string getRequest( int idSite )
        {   	
            var url = this.getBaseUrl() +
                "?idsite=" + idSite +
		        "&rec=1" +
		        "&apiv=" + VERSION + 
	            "&r=" + new Random().Next(0, 1000000).ToString("000000") +

                // Only allowed for Super User, token_auth required,
		        (!String.IsNullOrEmpty(ip) ? "&cip=" + ip : "") +
    	        (!String.IsNullOrEmpty(forcedVisitorId) ? "&cid=" + forcedVisitorId : "&_id=" + visitorId) +
                (!forcedDatetime.Equals(DateTimeOffset.MinValue) ? "&cdt=" + formatDateValue(forcedDatetime) : "") +
                (!String.IsNullOrEmpty(token_auth) && !this.doBulkRequests ? "&token_auth=" + urlEncode(token_auth) : "") +
	        
		        // These parameters are set by the JS, but optional when using API
	            (!String.IsNullOrEmpty(plugins) ? plugins : "") +
                (!localTime.Equals(DateTimeOffset.MinValue) ? "&h=" + localTime.Hour + "&m=" + localTime.Minute + "&s=" + localTime.Second : "") +
	            ((width != 0 && height != 0) ? "&res=" + width + "x" + height : "") +
	            (hasCookies ? "&cookie=1" : "") +
                (!ecommerceLastOrderTimestamp.Equals(DateTimeOffset.MinValue) ? "&_ects=" + formatTimestamp(ecommerceLastOrderTimestamp) : "") +
	        
	            // Various important attributes
	            (!String.IsNullOrEmpty(customData) ? "&data=" + customData : "") +
                (visitorCustomVar.Count() > 0 ? "&_cvar=" + urlEncode(new JavaScriptSerializer().Serialize(visitorCustomVar)) : "") +
                (pageCustomVar.Count() > 0 ? "&cvar=" + urlEncode(new JavaScriptSerializer().Serialize(pageCustomVar)) : "") +
	        
	            // URL parameters
                (!String.IsNullOrEmpty(pageUrl) ? "&url=" + urlEncode(pageUrl) : "") +
                (!String.IsNullOrEmpty(urlReferrer) ? "&urlref=" + urlEncode(urlReferrer) : "") +
	        
	            // Attribution information, so that Goal conversions are attributed to the right referrer or campaign
	            // Campaign name
                ((attributionInfo != null && !String.IsNullOrEmpty(attributionInfo.campaignName)) ? "&_rcn=" + urlEncode(attributionInfo.campaignName) : "") +
    	        // Campaign keyword
                ((attributionInfo != null && !String.IsNullOrEmpty(attributionInfo.campaignKeyword)) ? "&_rck=" + urlEncode(attributionInfo.campaignKeyword) : "") +
    	        // Timestamp at which the referrer was set
                ((attributionInfo != null && !attributionInfo.referrerTimestamp.Equals(DateTimeOffset.MinValue)) ? "&_refts=" + formatTimestamp(attributionInfo.referrerTimestamp) : "") +
    	        // Referrer URL
                ((attributionInfo != null && !String.IsNullOrEmpty(attributionInfo.referrerUrl)) ? "&_ref=" + urlEncode(attributionInfo.referrerUrl) : "") +

    	        // DEBUG 
	            DEBUG_APPEND_URL;

            // Reset page level custom variables after this page view
            pageCustomVar = new Dictionary<string ,string[]>();
    	
            return url;
        }

        private HttpCookie getCookieMatchingName(string name)
        {
            HttpContext currentContext = HttpContext.Current;

            if (currentContext == null)
            {
                throw new Exception("Can not read cookies without an active HttpContext");
            }

            HttpCookieCollection cookies = currentContext.Request.Cookies;

            for (int i = 0; i < cookies.Count; i++)
            {
                if (cookies[i].Name.Contains(name))
                {
                    return cookies[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the current full URL (scheme, host, path and query string.
        /// </summary>   
        static protected string getCurrentUrl()
        {
            return HttpContext.Current != null ? HttpContext.Current.Request.Url.AbsoluteUri : "http://unknown";
	    }

        private string formatDateValue(DateTimeOffset date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private string formatTimestamp(DateTimeOffset date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            double seconds = Convert.ToInt32(diff.TotalSeconds);
            return seconds.ToString();
        }

        private string formatMonetaryValue(double value)
        {
            return value.ToString("0.##", new CultureInfo("en-US")); 
        }        

        private string urlEncode(string value)
        {
            return HttpUtility.UrlEncode(value);
        }

    } 
}
