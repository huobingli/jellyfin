﻿using MediaBrowser.Common.Net.Handlers;
using MediaBrowser.Model.DTO;
using MediaBrowser.Model.Entities;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading.Tasks;

namespace MediaBrowser.Api.HttpHandlers
{
    /// <summary>
    /// Provides a handler to set a user's rating for an item
    /// </summary>
    [Export(typeof(BaseHandler))]
    public class UserItemRatingHandler : BaseSerializationHandler<DTOUserItemData>
    {
        public override bool HandlesRequest(HttpListenerRequest request)
        {
            return ApiService.IsApiUrlMatch("UserItemRating", request);
        }

        protected override Task<DTOUserItemData> GetObjectToSerialize()
        {
            // Get the item
            BaseItem item = ApiService.GetItemById(QueryString["id"]);

            // Get the user
            User user = ApiService.GetUserById(QueryString["userid"], true);

            // Get the user data for this item
            UserItemData data = item.GetUserData(user);

            if (data == null)
            {
                data = new UserItemData();
                item.AddUserData(user, data);
            }

            // If clearing the rating, set it to null
            if (QueryString["clear"] == "1")
            {
                data.Rating = null;
            }

            // If the user's rating mode is set to like/dislike
            else if (user.ItemRatingMode == ItemRatingMode.LikeOrDislike)
            {
                data.Likes = QueryString["likes"] == "1";
            }

            // If the user's rating mode is set to numeric
            else if (user.ItemRatingMode == ItemRatingMode.Numeric)
            {
                data.Rating = float.Parse(QueryString["value"]);
            }

            return Task.FromResult<DTOUserItemData>(ApiService.GetDTOUserItemData(data));
        }
    }
}