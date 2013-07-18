﻿// Copyright 2012 Max Toro Q.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;

namespace MvcAccount {
   
   /// <summary>
   /// Class responsible for creating authentication cookies and setting them 
   /// as part of the outgoing HTTP response.
   /// </summary>
   public class FormsAuthenticationService {

      AccountConfiguration _Configuration;

      public AccountConfiguration Configuration {
         get {
            return _Configuration
               ?? (_Configuration = AccountConfiguration.Current());
         }
         set { _Configuration = value; }
      }

      /// <summary>
      /// Creates an authentication cookie for a given user name. This does not set
      /// the cookie as part of the outgoing response.
      /// </summary>
      /// <param name="userName">The name of the authenticated user.</param>
      /// <returns>
      /// An <see cref="HttpCookie"/> that contains encrypted forms-authentication ticket
      /// information.
      /// </returns>
      public HttpCookie GetAuthCookie(string userName) {
         return GetAuthCookie(userName, createPersistentCookie: false);
      }

      /// <summary>
      /// Creates an authentication cookie for a given user name. This does not set
      /// the cookie as part of the outgoing response.
      /// </summary>
      /// <param name="userName">The name of the authenticated user.</param>
      /// <param name="createPersistentCookie">
      /// true to create a durable cookie (one that is saved across browser sessions);
      /// otherwise, false.
      /// </param>
      /// <returns>
      /// An <see cref="HttpCookie"/> that contains encrypted forms-authentication ticket
      /// information.
      /// </returns>
      public HttpCookie GetAuthCookie(string userName, bool createPersistentCookie) {
         return GetAuthCookie(userName, createPersistentCookie, FormsAuthentication.FormsCookiePath);
      }

      /// <summary>
      /// Creates an authentication cookie for a given user name. This does not set
      /// the cookie as part of the outgoing response.
      /// </summary>
      /// <param name="userName">The name of the authenticated user.</param>
      /// <param name="createPersistentCookie">
      /// true to create a durable cookie (one that is saved across browser sessions);
      /// otherwise, false.
      /// </param>
      /// <param name="strCookiePath">The <see cref="HttpCookie.Path"/> of the authentication cookie.</param>
      /// <param name="customData">
      /// The user-specific data to be stored with the ticket.
      /// This data is to be provided by a custom override method.
      /// </param>
      /// <returns>
      /// An <see cref="HttpCookie"/> that contains encrypted forms-authentication ticket
      /// information.
      /// </returns>
      public virtual HttpCookie GetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath, string customData = "") {

         DateTime utcNow = DateTime.UtcNow;
         TimeSpan persistentTimeout = this.Configuration.PersistentCookieTimeout;
         
         TimeSpan timeout = (createPersistentCookie && persistentTimeout != default(TimeSpan)) ?
            persistentTimeout
            : FormsAuthentication.Timeout;

         DateTime expirationUtc = utcNow.Add(timeout);

         var ticket = new FormsAuthenticationTicket(
            version: 2,
            name: userName,
            issueDate: utcNow,
            expiration: expirationUtc,
            isPersistent: createPersistentCookie,
            userData: customData
         );

         string encryptedTicket = FormsAuthentication.Encrypt(ticket);

         var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) {
            HttpOnly = true,
            Path = strCookiePath,
            Secure = FormsAuthentication.RequireSSL
         };

         string domain = FormsAuthentication.CookieDomain;

         if (domain != null)
            cookie.Domain = domain;

         if (ticket.IsPersistent)
            cookie.Expires = ticket.Expiration;

         return cookie;
      }

      /// <summary>
      /// Creates an authentication ticket for the supplied user name and adds it to
      /// the cookies collection of the response.
      /// </summary>
      /// <param name="userName">The name of an authenticated user.</param>
      public void SetAuthCookie(string userName) {
         SetAuthCookie(GetAuthCookie(userName));
      }

      /// <summary>
      /// Creates an authentication ticket for the supplied user name and adds it to
      /// the cookies collection of the response.
      /// </summary>
      /// <param name="userName">The name of an authenticated user.</param>
      /// <param name="createPersistentCookie">
      /// true to create a durable cookie (one that is saved across browser sessions);
      /// otherwise, false.
      /// </param>
      public void SetAuthCookie(string userName, bool createPersistentCookie) {
         SetAuthCookie(GetAuthCookie(userName, createPersistentCookie));
      }

      /// <summary>
      /// Creates an authentication ticket for the supplied user name and adds it to
      /// the cookies collection of the response, using the supplied cookie path.
      /// </summary>
      /// <param name="userName">The name of an authenticated user.</param>
      /// <param name="createPersistentCookie">
      /// true to create a durable cookie (one that is saved across browser sessions);
      /// otherwise, false.
      /// </param>
      /// <param name="strCookiePath">The cookie path for the forms-authentication ticket.</param>
      public void SetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath) {
         SetAuthCookie(GetAuthCookie(userName, createPersistentCookie, strCookiePath));
      }

      /// <summary>
      /// Adds the <paramref name="cookie"/> to the cookies collection of the response.
      /// </summary>
      /// <param name="cookie">The authentication cookie.</param>
      public virtual void SetAuthCookie(HttpCookie cookie) {

         if (cookie == null) throw new ArgumentNullException("cookie");

         HttpContext.Current.Response.Cookies.Set(cookie);
      }

      /// <summary>
      /// Removes the forms-authentication ticket from the browser.
      /// </summary>
      public virtual void SignOut() {
         FormsAuthentication.SignOut();
      }
   }
}
