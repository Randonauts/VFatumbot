﻿using System;
using System.Collections.Generic;
using DSharpPlus.Entities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using VFatumbot.BotLogic;
using static VFatumbot.BotLogic.Enums;
using static VFatumbot.BotLogic.FatumFunctions;

namespace VFatumbot
{
    public static class CardFactory
    {
        public const string BIT_PNG = "https://bot.randonauts.com/bit.png"; // blank replacement for Google thumbnail images

        public static IMessageActivity CreateGetLocationFromGoogleMapsReply()
        {
            var attachments = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachments);

            var cardAction = new CardAction(ActionTypes.OpenUrl, "Open Google Maps App", value: "https://maps.google.com");

            var buttons = new List<CardAction> {
                cardAction
            };

            var heroCard = new HeroCard
            {
                Title = "Facebook's removing the Send Location button",
                Text = "Maps→Longpress & drop pin→Share to Messenger→Randonauts→Back to chat",
                Tap = cardAction,
                Buttons = buttons,
            };

            reply.Attachments.Add(heroCard.ToAttachment());

            return reply;
        }

        public static IMessageActivity[] CreateChainCardReply(ChannelPlatform platform, FinalAttractor[] generatedPoints)
        {
            var replies = new IMessageActivity[1];

            var attachments = new List<Attachment>();
            var attachmentReply = MessageFactory.Attachment(attachments);
            replies[0] = attachmentReply;

            attachmentReply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            double[][] incoords = new double[generatedPoints.Length][];
            for (int i = 0; i < incoords.Length; i++)
            {
                incoords[i] = new double[2];
                incoords[i][0] = generatedPoints[i].X.center.point.latitude;
                incoords[i][1] = generatedPoints[i].X.center.point.longitude;
            }

            //var images = new List<CardImage>();
            //images.Add(new CardImage(CreateGoogleMapsStaticThumbnail(incoords[0])));

            var cardAction = new CardAction(ActionTypes.OpenUrl, Loc.g("open"), value: CreateGoogleMapsRouteUrl(incoords));

            var buttons = new List<CardAction> {
                cardAction,
            };

            var heroCard = new HeroCard
            {
                Title = Loc.g("chain_map_open"),
                //Images = images,
                Buttons = buttons,
                Tap = cardAction
            };

            attachmentReply.Attachments.Add(heroCard.ToAttachment());

            return replies;
        }

        public static IMessageActivity[] CreateLocationCardsReply(ChannelPlatform platform, double[] incoords, bool showStreetAndEarthThumbnails = false, dynamic w3wResult = null, bool forRemoteViewing = false, bool paying = false, bool isIOS = false)
        {
            if (platform == ChannelPlatform.discord)
            {
                var embed = new DiscordEmbedBuilder();

                embed.Author = new DiscordEmbedBuilder.EmbedAuthor();
                embed.Author.Name = w3wResult?.words;
                embed.Color = DiscordColor.Azure;

                embed.Title = Loc.g("view_on_google_maps");
                embed.Url = CreateGoogleMapsUrl(incoords, false);

                embed.Description = $"{Loc.g("street_view")}:\n{CreateGoogleStreetViewUrl(incoords)}\n\n{Loc.g("google_earth")}:\n{CreateGoogleEarthUrl(incoords)}";
                embed.ImageUrl = (!paying ? BIT_PNG : CreateGoogleMapsStaticThumbnail(incoords));
                embed.ThumbnailUrl = (!paying ? BIT_PNG : CreateGoogleStreetViewThumbnailUrl(incoords));

                embed.Build();

                var entity = new Entity();
                entity.SetAs(embed);

                var replyActivity = new Activity(entities: new List<Entity> {{ entity }}, type: "DiscordEmbed");

                return new IMessageActivity[] { replyActivity };
            }

            var useNativeLocationWidget = platform == ChannelPlatform.telegram || platform == ChannelPlatform.line;

            var replies = new IMessageActivity[useNativeLocationWidget ? 2 : 1];

            if (useNativeLocationWidget)
            {
                var nativeLocationWidgetReply = MessageFactory.Text("");
                var entity = new Entity();
                var geo = new GeoCoordinates(latitude: incoords[0],
                                             longitude: incoords[1],
                                             elevation: 0);
                var place = new Place(name: w3wResult != null ? $"what3words {Loc.g("address")}" : Loc.g("location"),
                                       address: w3wResult != null ? w3wResult.words : "",
                                       geo: geo);
                entity.SetAs(place);
                nativeLocationWidgetReply.Entities = new List<Entity>() { entity };
                replies[1] = nativeLocationWidgetReply;
            }

            var attachments = new List<Attachment>();
            var attachmentReply = MessageFactory.Attachment(attachments);
            replies[0] = attachmentReply;

            attachmentReply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            attachmentReply.Attachments.Add(CreateGoogleMapCard(incoords, !useNativeLocationWidget || showStreetAndEarthThumbnails, showStreetAndEarthThumbnails, w3wResult, forRemoteViewing: forRemoteViewing, paying: paying, isIOS: isIOS));

            if (showStreetAndEarthThumbnails && paying)
            {
                attachmentReply.Attachments.Add(CreateGoogleStreetViewCard(incoords, paying));
                attachmentReply.Attachments.Add(CreateGoogleEarthCard(incoords, paying));
            }

            return replies;
        }

        public static Attachment CreateGoogleMapCard(double[] incoords, bool showMapsThumbnail, bool showStreetAndEarthThumbnails = false, dynamic w3wResult = null, bool forRemoteViewing = false, bool paying = false, bool isIOS = false)
        {
            var images = new List<CardImage>();
            if (showMapsThumbnail && paying)
            {
                images.Add(new CardImage((!paying ? BIT_PNG : CreateGoogleMapsStaticThumbnail(incoords, forRemoteViewing))));
            }

            var w3wAction = new CardAction(ActionTypes.OpenUrl, (forRemoteViewing ? $"{w3wResult.words} - {w3wResult?.nearestPlace}{Helpers.GetCountryFromW3W(w3wResult)}" : "what3words"), value: $"https://what3words.com/{w3wResult.words}");
            var cardAction = new CardAction(ActionTypes.OpenUrl, showStreetAndEarthThumbnails ? Loc.g("open_map") : Loc.g("map"), value: CreateGoogleMapsUrl(incoords, isIOS));

            var buttons = new List<CardAction> {
                w3wAction,
                cardAction,
            };

            if (!showStreetAndEarthThumbnails)
            {
                buttons.Add(new CardAction(ActionTypes.OpenUrl, Loc.g("street_view"), value: CreateGoogleStreetViewUrl(incoords)));
                buttons.Add(new CardAction(ActionTypes.OpenUrl, Loc.g("earth"), value: CreateGoogleEarthUrl(incoords)));
            }

            var heroCard = new HeroCard
            {
                Title = !showStreetAndEarthThumbnails ? $"{Loc.g("view_with_google")}:" : Loc.g("google_maps"),
                Images = images,
                Buttons = buttons,
                Tap = cardAction
            };

            return heroCard.ToAttachment();
        }

        public static Attachment CreateGoogleStreetViewCard(double[] incoords, bool paying = false)
        {
            var images = new List<CardImage> {
                new CardImage((!paying ? BIT_PNG : CreateGoogleStreetViewThumbnailUrl(incoords))),
            };

            var cardAction = new CardAction(ActionTypes.OpenUrl, Loc.g("open"), value: CreateGoogleStreetViewUrl(incoords));

            var buttons = new List<CardAction> {
                cardAction
            };

            var heroCard = new HeroCard
            {
                Title = Loc.g("google_street_view"),
                Images = images,
                Buttons = buttons,
                Tap = cardAction,
            };

            return heroCard.ToAttachment();
        }

        public static Attachment CreateGoogleEarthCard(double[] incoords, bool paying = false)
        {
            var images = new List<CardImage> {
                new CardImage((!paying ? BIT_PNG : CreateGoogleEarthThumbnailUrl(incoords))),
            };

            var cardAction = new CardAction(ActionTypes.OpenUrl, Loc.g("open"), value: CreateGoogleEarthUrl(incoords));

            var buttons = new List<CardAction> {
                cardAction
            };

            var heroCard = new HeroCard
            {
                Title = Loc.g("google_earth"),
                Images = images,
                Buttons = buttons,
                Tap = cardAction
            };

            return heroCard.ToAttachment();
        }

        public static string CreateGoogleMapsStaticThumbnail(double[] incoords, bool forRemoteViewing = false)
        {
            return "https://maps.googleapis.com/maps/api/staticmap?&markers=color:red%7Clabel:C%7C" + incoords[0] + "+" + incoords[1] + $"&zoom={(forRemoteViewing ? 4 : 15)}&size=" + Consts.THUMBNAIL_SIZE + "&maptype=roadmap&key=" + Consts.GOOGLE_MAPS_API_KEY;
        }

        public static string CreateGoogleMapsUrl(double[] incoords, bool isIOS)
        {
            if (isIOS)
            {
                // On 2020/07/25 we switched the the /maps/search/ type URL to stop the Android bot app defaulting to showing the
                // directions to the point in Google Maps but then that broke iOS. iOS <= v1.0.7 didn't have the logic for
                // google.com URLs with "/maps/search" in them to load them externally from the app so the "Maps" button started
                // loading everything an in internal inapp browser (uh icky I hate those).
                return "https://www.google.com/maps/place/" + incoords[0] + "+" + incoords[1] + "/@" + incoords[0] + "+" + incoords[1] + ",14z";
            }

            return "https://www.google.com/maps/search/?api=1&query=" + incoords[0] + "," + incoords[1] + "&zoom=14";
        }

        public static string CreateGoogleMapsRouteUrl(double[][] incoords)
        {
            var str = "https://www.google.com/maps/dir/";
            for (int i = 0; i < incoords.Length; i++)
            {
                str += incoords[i][0] + "+" + incoords[i][1] + "/";
            }
            return str;
        }

        public static string CreateGoogleStreetViewUrl(double[] incoords)
        {
            return "https://www.google.com/maps/@?api=1&map_action=pano&viewpoint=" + incoords[0] + "," + incoords[1] + "&fov=90&heading=235&pitch=10";
        }

        public static string CreateGoogleEarthUrl(double[] incoords)
        {
            return "https://earth.google.com/web/search/" + incoords[0] + "," + incoords[1];
        }

        public static string CreateGoogleStreetViewThumbnailUrl(double[] incoords)
        {
            return "https://maps.googleapis.com/maps/api/streetview?size=" + Consts.THUMBNAIL_SIZE + "&location=" + incoords[0] + "," + incoords[1] + "&fov=90&heading=235&pitch=10&key=" + Consts.GOOGLE_MAPS_API_KEY;
        }

        public static string CreateGoogleEarthThumbnailUrl(double[] incoords)
        {
            return "https://maps.googleapis.com/maps/api/staticmap?&markers=color:red%7Clabel:C%7C" + incoords[0] + "+" + incoords[1] + "&zoom=18&size=" + Consts.THUMBNAIL_SIZE + "&maptype=satellite&key=" + Consts.GOOGLE_MAPS_API_KEY;
        }

        public static IMessageActivity CreateAppStoreDownloadCard()
        {
            var buttons = new List<CardAction> {
                new CardAction(ActionTypes.OpenUrl, "Android", value: "https://play.google.com/store/apps/details?id=com.randonautica.app"),
                new CardAction(ActionTypes.OpenUrl, "iOS", value: "https://apps.apple.com/us/app/randonautica/id1493743521")
            };


            var heroCard = new HeroCard
            {
                Title = Loc.g("download_app"),
                Buttons = buttons,
            };


            var attachments = new List<Attachment>();
            attachments.Add(heroCard.ToAttachment());
            return MessageFactory.Attachment(attachments);
        }

        public static IMessageActivity GetWelcomeVideoCard()
        {
            var videoCard = new VideoCard
            {
                Image = new ThumbnailUrl
                {
                    Url = "https://youtube.com/watch?v=xEbbsG2U26k",
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://youtube.com/watch?v=xEbbsG2U26k",
                    },
                },
            };

            var attachments = new List<Attachment>();
            attachments.Add(videoCard.ToAttachment());
            return MessageFactory.Attachment(attachments);
        }

    }
}
