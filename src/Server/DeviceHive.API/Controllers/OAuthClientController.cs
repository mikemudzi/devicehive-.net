﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DeviceHive.API.Filters;
using DeviceHive.Core.Mapping;
using DeviceHive.Data.Model;
using Newtonsoft.Json.Linq;

namespace DeviceHive.API.Controllers
{
    /// <resource cref="OAuthClient" />
    public class OAuthClientController : BaseController
    {
        /// <name>list</name>
        /// <summary>
        /// Gets list of OAuth clients.
        /// </summary>
        /// <query cref="OAuthClientFilter" />
        /// <returns cref="OAuthClient">If successful, this method returns array of <see cref="OAuthClient"/> resources in the response body.</returns>
        public JArray Get()
        {
            var filter = MapObjectFromQuery<OAuthClientFilter>();
            return new JArray(DataContext.OAuthClient.GetAll(filter).Select(c => Mapper.Map(c)));
        }

        /// <name>get</name>
        /// <summary>
        /// Gets information about OAuth client.
        /// </summary>
        /// <param name="id">OAuth client identifier.</param>
        /// <returns cref="OAuthClient">If successful, this method returns an <see cref="OAuthClient"/> resource in the response body.</returns>
        public JObject Get(int id)
        {
            var oauthClient = DataContext.OAuthClient.Get(id);
            if (oauthClient == null)
                ThrowHttpResponse(HttpStatusCode.NotFound, "OAuth client not found!");

            return Mapper.Map(oauthClient);
        }

        /// <name>insert</name>
        /// <summary>
        /// Creates new OAuth client.
        /// </summary>
        /// <param name="json" cref="OAuthClient">In the request body, supply a <see cref="OAuthClient"/> resource.</param>
        /// <returns cref="OAuthClient" mode="OneWayOnly">If successful, this method returns a <see cref="OAuthClient"/> resource in the response body.</returns>
        [AuthorizeAdmin]
        [HttpCreatedResponse]
        public JObject Post(JObject json)
        {
            var oauthClient = Mapper.Map(json);
            oauthClient.GenerateSecret();
            Validate(oauthClient);

            if (DataContext.OAuthClient.Get(oauthClient.OAuthID) != null)
                ThrowHttpResponse(HttpStatusCode.Forbidden, "OAuth client with such OAuthID already exists!");

            DataContext.OAuthClient.Save(oauthClient);
            return Mapper.Map(oauthClient, oneWayOnly: true);
        }

        /// <name>update</name>
        /// <summary>
        /// Updates an existing OAuth client.
        /// </summary>
        /// <param name="id">OAuth client identifier.</param>
        /// <param name="json" cref="OAuthClient">In the request body, supply a <see cref="OAuthClient"/> resource.</param>
        /// <request>
        ///     <parameter name="name" required="false" />
        ///     <parameter name="domain" required="false" />
        ///     <parameter name="subnet" required="false" />
        ///     <parameter name="redirectUri" required="false" />
        ///     <parameter name="oauthId" required="false" />
        /// </request>
        [AuthorizeAdmin]
        [HttpNoContentResponse]
        public void Put(int id, JObject json)
        {
            var oauthClient = DataContext.OAuthClient.Get(id);
            if (oauthClient == null)
                ThrowHttpResponse(HttpStatusCode.NotFound, "OAuth client not found!");

            Mapper.Apply(oauthClient, json);
            Validate(oauthClient);

            var existing = DataContext.OAuthClient.Get(oauthClient.OAuthID);
            if (existing != null && existing.ID != oauthClient.ID)
                ThrowHttpResponse(HttpStatusCode.Forbidden, "OAuth client with such OAuthID already exists!");

            DataContext.OAuthClient.Save(oauthClient);
        }

        /// <name>delete</name>
        /// <summary>
        /// Deletes an existing OAuth client.
        /// </summary>
        /// <param name="id">OAuth client identifier.</param>
        [AuthorizeAdmin]
        [HttpNoContentResponse]
        public void Delete(int id)
        {
            DataContext.OAuthClient.Delete(id);
        }

        private IJsonMapper<OAuthClient> Mapper
        {
            get { return GetMapper<OAuthClient>(); }
        }
    }
}