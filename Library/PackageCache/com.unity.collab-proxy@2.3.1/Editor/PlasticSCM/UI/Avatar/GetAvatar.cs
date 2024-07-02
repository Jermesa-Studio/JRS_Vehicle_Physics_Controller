using System;

using UnityEngine;

using PlasticGui;
using CodiceApp.Gravatar;

namespace Unity.PlasticSCM.Editor.UI.Avatar
{
    internal static class GetAvatar
    {
        internal static Texture2D ForEmail(
            string email,
            Action avatarLoadedAction)
        {
            if (string.IsNullOrEmpty(email))
                return Images.GetEmptyGravatar();

            if (AvatarImages.HasGravatar(email))
                return AvatarImages.GetAvatar(email);

            Texture2D defaultImage =
                Images.GetEmptyGravatar();

            AvatarImages.AddGravatar(email, defaultImage);

            LoadAvatar.ForEmail(
                email, avatarLoadedAction,
                AfterDownloadSucceed);

            return defaultImage;
        }

        static void AfterDownloadSucceed(
            string email,
            GravatarImagesProvider.Result result,
            Action avatarLoadedAction)
        {
            if (result.ResultCode == GravatarImagesProvider.Result.OperationResult.OK)
            {
                AvatarImages.UpdateGravatar(email, result.RawGravatar);

                avatarLoadedAction();
            }
        }
    }
}