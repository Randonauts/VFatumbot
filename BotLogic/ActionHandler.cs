﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static VFatumbot.BotLogic.Enums;
using static VFatumbot.BotLogic.FatumFunctions;
using static VFatumbot.QuantumRandomNumberGeneratorWrapper;

namespace VFatumbot.BotLogic
{
    public class ActionHandler
    {
        [DllImport("libAttract", CallingConvention = CallingConvention.Cdecl)]
        public extern static int getVersionMajor();
        [DllImport("libAttract", CallingConvention = CallingConvention.Cdecl)]
        public extern static int getVersionMinor();
        [DllImport("libAttract", CallingConvention = CallingConvention.Cdecl)]
        public extern static int getVersionPatch();

#if RELEASE_PROD
        static string API_SERVER = $"https://api.randonauts.com";
#else
        static string API_SERVER = $"https://api.randonauts.com";
        //static string API_SERVER = $"http://127.0.0.1:3000";
#endif

        public void DispatchWorkerThread(DoWorkEventHandler handler)
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += handler;
            backgroundWorker.RunWorkerAsync();
        }

        public async Task ParseSlashCommands(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog)
        {
            var command = turnContext.Activity.Text.ToLower();
            if (command.StartsWith("/ongshat", StringComparison.InvariantCulture))
            {
                await turnContext.SendActivityAsync("You're not authorized to do that!");
                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            //else if (command.StartsWith("/stevedick", StringComparison.InvariantCulture))
            //{
            //    var dstr = "";
            //    dstr += "...............…………………………._¸„„„„_  \n";
            //    dstr += "…………………….…………...„--~*'¯…….'\\  \n";
            //    dstr += "………….…………………… („-~~--„¸_….,/ì'Ì  \n";
            //    dstr += "…….…………………….¸„-^\"¯ : : : : :¸-¯\"¯/'  \n";
            //    dstr += "……………………¸„„-^\"¯ : : : : : : : '\\¸„„,-\"  \n";
            //    dstr += "**¯¯¯'^^~-„„„----~^*'\"¯ : : : : : : : : : :¸-\"  \n";
            //    dstr += ".:.:.:.:.„-^\" : : : : : : : : : : : : : : : : :„-\"  \n";
            //    dstr += ":.:.:.:.:.:.:.:.:.:.: : : : : : : : : : ¸„-^¯  \n";
            //    dstr += ".::.:.:.:.:.:.:.:. : : : : : : : ¸„„-^¯  \n";
            //    dstr += ":.' : : '\\ : : : : : : : ;¸„„-~\"  \n";
            //    dstr += ":.:.:: :\"-„\"\"***/*'ì¸'¯  \n";
            //    dstr += ":.': : : : :\"-„ : : :\"\\  \n";
            //    dstr += ".:.:.: : : : :\" : : : : \\,  \n";
            //    dstr += ":.: : : : : : : : : : : : 'Ì  \n";
            //    dstr += ": : : : : : :, : : : : : :/  \n";
            //    dstr += "\"-„_::::_„-*__„„~\"  \n";

            //    await turnContext.SendActivityAsync(dstr);
            //}
            else if (command.StartsWith("/steve", StringComparison.InvariantCulture))
            {
                var imallkinds = MessageFactory.Text(Loc.g("all_kinds_steve"));
                
                var images = new List<CardImage> {
                    new CardImage("http://thispersondoesnotexist.com/image"),
                };
                var cardAction = new CardAction(ActionTypes.OpenUrl, Loc.g("let_steve_help"), value: "https://docs.google.com/forms/d/e/1FAIpQLSekcgPLv7MUd5nfPn9JVLpZHH0I5MNP_e7ekw7_mEmJsMJkzw/viewform");
                var buttons = new List<CardAction> {
                    cardAction
                };
                var heroCard = new HeroCard
                {
                    Title = $"↑↑ {Loc.g("this_is_steve")} ↑↑",
                    Images = images,
                    Buttons = buttons,
                    Tap = cardAction,
                };

                imallkinds.Attachments = new List<Attachment>();
                imallkinds.Attachments.Add(heroCard.ToAttachment());

                await turnContext.SendActivityAsync(imallkinds);
            }
            //else if (command.StartsWith("/newsteve22", StringComparison.InvariantCulture))
            //{
            //    var videoCard = new VideoCard
            //    {
            //        Title = "The New Steve 22",
            //        Media = new List<MediaUrl>
            //        {
            //            new MediaUrl()
            //            {
            //                Url = "https://bot.randonauts.com/therealsteve.mp4",
            //            },
            //        },
            //        Buttons = new List<CardAction>
            //        {
            //            new CardAction()
            //            {
            //                Title = "Let Steve guide you",
            //                Type = ActionTypes.OpenUrl,
            //                Value = "https://docs.google.com/forms/d/e/1FAIpQLSekcgPLv7MUd5nfPn9JVLpZHH0I5MNP_e7ekw7_mEmJsMJkzw/viewform",
            //            },
            //        },
            //    };

            //    var imallkinds = MessageFactory.Text("I'm the new all kinds of Steve 22!");
            //    imallkinds.Attachments = new List<Attachment>();
            //    imallkinds.Attachments.Add(videoCard.ToAttachment());

            //    await turnContext.SendActivityAsync(imallkinds);

            //}
            else if (command.StartsWith("/getattractor", StringComparison.InvariantCulture))
            {
                await AttractorActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
            }
            else if (command.StartsWith("/getvoid", StringComparison.InvariantCulture))
            {
                await VoidActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
            }
            else if (command.StartsWith("/getintents", StringComparison.InvariantCulture))
            {
                await IntentSuggestionActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
            }
            else if (command.StartsWith("/getpair", StringComparison.InvariantCulture))
            {
                await PairActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
            }
            else if (command.StartsWith("/getpseudo", StringComparison.InvariantCulture))
            {
                await PseudoActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
            }
            else if (command.StartsWith("/getquantum", StringComparison.InvariantCulture))
            {
                await QuantumActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
            }
            else if (command.StartsWith("/stargate", StringComparison.InvariantCulture)) 
            {
                // for remote viewing experiments
                await QuantumActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog, forRemoteViewing: true);
            }
            else if (command.StartsWith("/getqtime", StringComparison.InvariantCulture))
            {
                await QuantumActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog, true);
            }
            else if (command.StartsWith("/getida", StringComparison.InvariantCulture) ||
                     command.StartsWith("/getanomaly", StringComparison.InvariantCulture))
            {
                await AnomalyActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
            }
            else if (command.StartsWith("/scanida", StringComparison.InvariantCulture) ||
                     command.StartsWith("/scananomaly", StringComparison.InvariantCulture))
            {
                await AnomalyActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog, true);
            }
            else if (command.StartsWith("/scanattractor", StringComparison.InvariantCulture))
            {
                await AttractorActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog, true);
            }
            else if (command.StartsWith("/scanvoid", StringComparison.InvariantCulture))
            {
                await VoidActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog, true);
            }
            else if (command.StartsWith("/scanpair", StringComparison.InvariantCulture))
            {
                await PairActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog, true);
            }
            else if (command.StartsWith("/getpoint", StringComparison.InvariantCulture) ||
                     command.StartsWith("/mysterypoint", StringComparison.InvariantCulture))
            {
                await MysteryPointActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
            }
            //else if (command.StartsWith("/myrandotrips", StringComparison.InvariantCulture))
            //{
            //    await RandotripsActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog, "my");
            //}
            //else if (command.StartsWith("/randotrips", StringComparison.InvariantCulture))
            //{
            //    var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            //    if (command.Contains(" "))
            //    {
            //        string arg = command.Replace(command.Substring(0, command.IndexOf(" ", StringComparison.InvariantCulture)), "");
            //        arg = arg.Trim();
            //        if (!"all".Equals(arg))
            //        {
            //            DateTime parsedDateTime;
            //            if (DateTime.TryParse(arg, out parsedDateTime))
            //                date = parsedDateTime.ToString("yyyy-MM-dd");
            //        }
            //        else
            //        {
            //            date = arg; // all
            //        }
            //    }

            //    await RandotripsActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog, date);
            //}
            else if (command.StartsWith("/setradius", StringComparison.InvariantCulture))
            {
                if (command.Contains(" "))
                {
                    bool noLimit = false; // archon mode
                    if (command.Contains(" limitless")
                        || userProfileTemporary.UserId.Equals("16716e1a5797c6285a0d6d84c3520ec20f962b1e54be46f631b62ea3bc70488d")) // for Patrick who helped raise the importance of ToS and lives on a ridge the Jungle somewhere and anything >1km is inaccessible
                    {
                        noLimit = true;
                        command = command.Replace(" limitless", "");
                    }
                    string newRadiusStr = command.Replace(command.Substring(0, command.IndexOf(" ", StringComparison.InvariantCulture)), "");
                    int oldRadius = userProfileTemporary.Radius, inputtedRadius;
                    if (Int32.TryParse(newRadiusStr, out inputtedRadius))
                    {
                        if (!noLimit)
                        {
                            if (inputtedRadius < Consts.RADIUS_MIN)
                            {
                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("radius_gte", Consts.RADIUS_MIN)), cancellationToken);
                                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
                                return;
                            }
                            if (userProfileTemporary.Has20kmRadius && inputtedRadius > 20000)
                            {
                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("radius_lte", 20000)), cancellationToken);
                                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
                                return;
                            }
                            else if (!userProfileTemporary.Has20kmRadius && inputtedRadius > Consts.RADIUS_MAX)
                            {
                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("radius_lte", Consts.RADIUS_MAX)), cancellationToken);
                                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
                                return;
                            }
                        }

                        userProfileTemporary.Radius = inputtedRadius;
                        //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Set Radius", new Dictionary<string, object>() { { "Radius", inputtedRadius }, { "Command", "/setradius" } });
                        await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("changed_radius", oldRadius, userProfileTemporary.Radius)), cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("invalid_radius")), cancellationToken);
                    }
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("current_radius", userProfileTemporary.Radius)), cancellationToken);
                }

                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            else if (command.StartsWith("/setlocation", StringComparison.InvariantCulture))
            {
                if (command.Contains(" "))
                {
                    double lat = 0, lon = 0;
                    if (Helpers.InterceptLocation(turnContext, out lat, out lon)) // Intercept any locations the user sends us, no matter where in the conversation they are
                    {
                        bool validCoords = true;
                        if (lat == Consts.INVALID_COORD && lon == Consts.INVALID_COORD && userProfileTemporary.HasLocationSearch)
                        {
                            // Do a geocode query lookup against the address the user sent
                            var result = await Helpers.GeocodeAddressAsync(turnContext.Activity.Text.ToLower().Replace("/setlocation", ""));
                            if (result != null)
                            {
                                lat = result.Item1;
                                lon = result.Item2;
                            }
                            else
                            {
                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("cant_find_place")), cancellationToken);
                                validCoords = false;
                            }
                        }
                        else if (lat == Consts.INVALID_COORD && lon == Consts.INVALID_COORD && !userProfileTemporary.HasLocationSearch)
                        {
                            validCoords = false;
                        }


                        if (validCoords)
                        {
                            // Update user's location
                            userProfileTemporary.Latitude = lat;
                            userProfileTemporary.Longitude = lon;

                            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("current_location", lat.ToString("#0.000000", System.Globalization.CultureInfo.InvariantCulture), lon.ToString("#0.000000", System.Globalization.CultureInfo.InvariantCulture))), cancellationToken);

                            var incoords = new double[] { lat, lon };
                            var w3wResult = await Helpers.GetWhat3WordsAddressAsync(incoords);
                            await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);

                            await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);

                            return;
                        }
                    }
                }

                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            else if (command.StartsWith("/mylocation", StringComparison.InvariantCulture))
            {
                await LocationActionAsync(turnContext, userProfileTemporary, cancellationToken);
                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            else if (command.StartsWith("/togglewater", StringComparison.InvariantCulture))
            {
                userProfileTemporary.IsIncludeWaterPoints = !userProfileTemporary.IsIncludeWaterPoints;
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("water_points_will_be", userProfileTemporary.IsIncludeWaterPoints ? Loc.g("included") : Loc.g("skipped"))), cancellationToken);
                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            else if (command.StartsWith("/toggleclassic", StringComparison.InvariantCulture))
            {
                userProfileTemporary.IsUseClassicMode = !userProfileTemporary.IsUseClassicMode;
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("classic_mode", userProfileTemporary.IsUseClassicMode ? Loc.g("enabled") : Loc.g("disabled"))), cancellationToken);
                CallbackOptions callbackOptions = new CallbackOptions();
                callbackOptions.UpdateSettings = true;
                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken, callbackOptions);
            }
            else if (command.StartsWith("/help", StringComparison.InvariantCulture) ||
                     command.StartsWith("/morehelp", StringComparison.InvariantCulture))
            {
                //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Help", new Dictionary<string, object>() { { "Command", "/help" } });
                await Helpers.HelpAsync(turnContext, userProfileTemporary, mainDialog, cancellationToken);
            }
            else if (command.Equals("/stats"))
            {
                // Quick hack to see KPIs

                // TODO: get more like DAU etc

                // How many reports today?
                await Task.Run(() =>
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = Consts.DB_SERVER;
                    builder.UserID = Consts.DB_USER;
                    builder.Password = Consts.DB_PASSWORD;
                    builder.InitialCatalog = Consts.DB_NAME;

                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        connection.Open();

                        StringBuilder ssb = new StringBuilder();
                        ssb.Append("SELECT COUNT(*) FROM ");
#if RELEASE_PROD
                        ssb.Append("reports");
#else
                        ssb.Append("reports_dev");
#endif
                        ssb.Append(";");
                        Console.WriteLine("SQL:" + ssb.ToString());

                        using (SqlCommand sqlCommand = new SqlCommand(ssb.ToString(), connection))
                        {
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                string commandResult = "";
                                while (reader.Read())
                                {
                                    commandResult += $"{reader.GetInt32(0)}\n";
                                }
                                turnContext.SendActivityAsync(MessageFactory.Text(commandResult));
                            }
                        }
                    }
                });

                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            else if (command.Equals("/resetflags"))
            {
                userProfileTemporary.IsScanning = false;

                await turnContext.SendActivityAsync(MessageFactory.Text($"Flags reset"), cancellationToken);

                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            else if (command.Equals("/resetlocation"))
            {
                userProfileTemporary.ResetLocation();

                await turnContext.SendActivityAsync(MessageFactory.Text($"Current location reset"), cancellationToken);

                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            else if (command.Equals("/jsondump"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(JsonConvert.SerializeObject(turnContext.Activity)), cancellationToken);

                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            else if (command.Equals("/pushid"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Your Push ID is {userProfileTemporary.PushUserId}"), cancellationToken);
                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
            }
            else if (command.Equals("/closemenu"))
            {
                // First release was leaving dialog prompt menus sticking on everyone's keyboard in the Randonaut Telegram lobby.
                // This is a hack to close it next update.
                var reply = turnContext.Activity.CreateReply("Closing menu");
                var replyMarkup = new
                {
                    reply_markup = new
                    {
                        remove_keyboard = true,
                    }
                };
                var channelData = new
                {
                    method = "sendMessage",
                    parameters = replyMarkup,
                };
                reply.ChannelData = JObject.FromObject(channelData);
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            else if (command.Equals("/test"))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(
                    $"Randonautica {Consts.APP_VERSION} is alive.{Helpers.GetNewLine(turnContext)}" +
                    $"Entangled waypoint coordinate calculations being done with the hyper quantum flux capacitor libAttract v{getVersionMajor()}.{getVersionMinor()}.{getVersionPatch()}.{Helpers.GetNewLine(turnContext)}" +
                    "Checking QRNG source too..."),
                    cancellationToken);

                await turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                   async (context, token) =>
                   {
                       QuantumRandomNumberGeneratorWrapper rnd = new QuantumRandomNumberGeneratorWrapper(turnContext, mainDialog, cancellationToken);
                       try
                       {
                           rnd.NextHex(10);
                           await turnContext.SendActivityAsync(MessageFactory.Text($"QRNG source is alive too!"), cancellationToken);

                           await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);

                       }
                       catch (Exception e)
                       {
                           if (!e.GetType().Equals(typeof(CanIgnoreException)))
                           {
                               await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken);
                           }
                       }
                   },
                   cancellationToken);
            }
        }

        public async Task AttractorActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog, bool doScan = false, int idacou = 1, string entropyQueryString = null)
        {
            if (turnContext.Activity.Text.Contains("["))
            {
                string[] buf = turnContext.Activity.Text.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                Int32.TryParse(buf[1], out idacou);
                if (idacou < 1 || idacou > 10)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("invalid_num_points")), cancellationToken);
                    return;
                }
            }

            if (doScan)
            {
                userProfileTemporary.IsScanning = true;
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("scan_time_take")), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("finding_location")), cancellationToken);
            }

            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        string mesg = "";
                        int numWaterPointsSkipped = 0;

                    redo:
                        
                        var idatup = await GetIDAFromAzureFunction(userProfileTemporary.Location, userProfileTemporary.Radius, doScan ? 1 : 0, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token, entropyQueryString: entropyQueryString));
                        var ida = idatup.Item1;
                        var shaGid = idatup.Item2;
                        //if (string.IsNullOrEmpty(entropyQueryString) || !entropyQueryString.Contains("pool=true")) await turnContext.SendActivityAsync(MessageFactory.Text($"[Visualize your entropy in an image!]({API_SERVER}/visualizeEntropy?gid={shaGid})"), cancellationToken);
                        ida = SortIDA(ida, "attractor", idacou);
                        if (ida.Length > 0)
                        {
                            var shortCodesArray = new string[ida.Count()];
                            var messagesArray = new string[ida.Count()];
                            var pointTypesArray = new PointTypes[ida.Count()];
                            var numWaterPointsSkippedArray = new int[ida.Count()];
                            var what3WordsArray = new string[ida.Count()];
                            var nearestPlacesArray = new string[ida.Count()];

                            for (int i = 0; i < ida.Count(); i++)
                            {
                                var incoords = new double[] { ida[i].X.center.point.latitude, ida[i].X.center.point.longitude };

                                // If water points are set to be skipped, and there's only 1 point in the result array, try again else just exclude those from the results
                                if (!userProfileTemporary.IsIncludeWaterPoints)
                                {
                                    var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(incoords);
                                    if (isWaterPoint)
                                    {
                                        numWaterPointsSkipped++;

                                        if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                        {
                                            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("nothing_but_water_points")), cancellationToken);
                                            await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, new CallbackOptions() { ResetFlag = doScan });
                                            return;
                                        }

                                        if (ida.Length == 1)
                                        {
                                            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped_so_far", numWaterPointsSkipped)), cancellationToken);
                                            goto redo;
                                        }

                                        continue;
                                    }
                                }

                                shortCodesArray[i] = Helpers.Crc32Hash($"{turnContext.Activity.From.Id}{turnContext.Activity.Timestamp.ToString()}");
                                mesg = Tolog(turnContext, "attractor", ida[i], shortCodesArray[i]);
                                await turnContext.SendActivityAsync(MessageFactory.Text((idacou > 1 ? ("#" + (i + 1) + " ") : "") + Helpers.DirectLineNewLineFix(turnContext, mesg)), cancellationToken);

                                dynamic w3wResult = await Helpers.GetWhat3WordsAddressAsync(incoords);

                                if (numWaterPointsSkipped > 0)
                                {
                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped", numWaterPointsSkipped)), cancellationToken);
                                }

                                messagesArray[i] = mesg;
                                pointTypesArray[i] = PointTypes.Attractor;
                                numWaterPointsSkippedArray[i] = numWaterPointsSkipped;
                                what3WordsArray[i] = ""+w3wResult?.words;
                                nearestPlacesArray[i] = "" + w3wResult?.nearestPlace + Helpers.GetCountryFromW3W(w3wResult);

                                await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);
                                await Helpers.SendPushNotification(userProfileTemporary, Loc.g("point_generated"), mesg);
                                //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Point Generated", new Dictionary<string, object>() { { "Type", "Attractor" }, {"IsScan", doScan } , { "Radius", userProfileTemporary.Radius }, {"RNG", userProfileTemporary.LastRNGType }, { "IDA Count", idacou } });
                            }

                            CallbackOptions callbackOptions = new CallbackOptions()
                            {
                                StartTripReportDialog = true,
                                ShortCodes = shortCodesArray,
                                Messages = messagesArray,
                                PointTypes = pointTypesArray,
                                GeneratedPoints = ida,
                                ShaGids = new string[] { shaGid },
                                NumWaterPointsSkipped = numWaterPointsSkippedArray,
                                What3Words = what3WordsArray,
                                NearestPlaces = nearestPlacesArray,
                                ResetFlag = doScan,
                                DeductOwlTokens = 1
                            };
                            await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, callbackOptions);
                        }
                        else
                        {
                            if (!userProfileTemporary.IsUseClassicMode && !doScan)
                            {
                                mesg = Loc.g("no_points_detected_quantum");
                                await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);
                                await QuantumActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
                            }
                            else
                            {
                                mesg = Loc.g("no_points_detected");
                                await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);
                                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, new CallbackOptions() { ResetFlag = doScan });
                            }
                        }
                    }, cancellationToken);
            });
        }

        public async Task VoidActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog, bool doScan = false, int idacou = 1, string entropyQueryString = null)
        {
            if (turnContext.Activity.Text.Contains("["))
            {
                string[] buf = turnContext.Activity.Text.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                Int32.TryParse(buf[1], out idacou);
                if (idacou < 1 || idacou > 10)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("invalid_num_points")), cancellationToken);
                    return;
                }
            }

            if (doScan)
            {
                userProfileTemporary.IsScanning = true;
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("scan_time_take")), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("finding_location")), cancellationToken);
            }

            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        string mesg = "";
                        int numWaterPointsSkipped = 0;

                    redo:
                        var idatup = await GetIDAFromAzureFunction(userProfileTemporary.Location, userProfileTemporary.Radius, doScan ? 1 : 0, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token, entropyQueryString: entropyQueryString));
                        var ida = idatup.Item1;
                        var shaGid = idatup.Item2;
                        //if (string.IsNullOrEmpty(entropyQueryString) || !entropyQueryString.Contains("pool=true")) await turnContext.SendActivityAsync(MessageFactory.Text($"[Visualize your entropy in an image!]({API_SERVER}/visualizeEntropy?gid={shaGid})"), cancellationToken);
                        ida = SortIDA(ida, "void", idacou);
                        if (ida.Length > 0)
                        {
                            var shortCodesArray = new string[ida.Count()];
                            var messagesArray = new string[ida.Count()];
                            var pointTypesArray = new PointTypes[ida.Count()];
                            var numWaterPointsSkippedArray = new int[ida.Count()];
                            var what3WordsArray = new string[ida.Count()];
                            var nearestPlacesArray = new string[ida.Count()];

                            for (int i = 0; i < ida.Count(); i++)
                            {
                                var incoords = new double[] { ida[i].X.center.point.latitude, ida[i].X.center.point.longitude };

                                // If water points are set to be skipped, and there's only 1 point in the result array, try again else just exclude those from the results
                                if (!userProfileTemporary.IsIncludeWaterPoints)
                                {
                                    var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(incoords);
                                    if (isWaterPoint)
                                    {
                                        numWaterPointsSkipped++;

                                        if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                        {
                                            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("nothing_but_water_points")), cancellationToken);
                                            await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, new CallbackOptions() { ResetFlag = doScan });
                                            return;
                                        }

                                        if (ida.Length == 1)
                                        {
                                            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped_so_far", numWaterPointsSkipped)), cancellationToken);
                                            goto redo;
                                        }

                                        continue;
                                    }
                                }

                                shortCodesArray[i] = Helpers.Crc32Hash($"{turnContext.Activity.From.Id}{turnContext.Activity.Timestamp.ToString()}");
                                mesg = Tolog(turnContext, "void", ida[i], shortCodesArray[i]);
                                await turnContext.SendActivityAsync(MessageFactory.Text((idacou > 1 ? ("#" + (i + 1) + " ") : "") + Helpers.DirectLineNewLineFix(turnContext, mesg)), cancellationToken);

                                dynamic w3wResult = await Helpers.GetWhat3WordsAddressAsync(incoords);

                                if (numWaterPointsSkipped > 0)
                                {
                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped", numWaterPointsSkipped)), cancellationToken);
                                }

                                messagesArray[i] = mesg;
                                pointTypesArray[i] = PointTypes.Void;
                                numWaterPointsSkippedArray[i] = numWaterPointsSkipped;
                                what3WordsArray[i] = "" + w3wResult?.words;
                                nearestPlacesArray[i] = "" + w3wResult?.nearestPlace + Helpers.GetCountryFromW3W(w3wResult);

                                await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);
                                await Helpers.SendPushNotification(userProfileTemporary, Loc.g("point_generated"), mesg);
                                //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Point Generated", new Dictionary<string, object>() { { "Type", "Void" }, {"IsScan", doScan } , { "Radius", userProfileTemporary.Radius }, {"RNG", userProfileTemporary.LastRNGType }, { "IDA Count", idacou } });
                            }

                            CallbackOptions callbackOptions = new CallbackOptions()
                            {
                                StartTripReportDialog = true,
                                ShortCodes = shortCodesArray,
                                Messages = messagesArray,
                                PointTypes = pointTypesArray,
                                GeneratedPoints = ida,
                                ShaGids = new string[] { shaGid },
                                NumWaterPointsSkipped = numWaterPointsSkippedArray,
                                What3Words = what3WordsArray,
                                NearestPlaces = nearestPlacesArray,
                                ResetFlag = doScan,
                                DeductOwlTokens = 1
                            };
                            await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, callbackOptions);
                        }
                        else
                        {
                            if (!userProfileTemporary.IsUseClassicMode && !doScan)
                            {
                                mesg = Loc.g("no_points_detected_quantum");
                                await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);
                                await QuantumActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
                            }
                            else
                            {
                                mesg = Loc.g("no_points_detected");
                                await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);
                                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, new CallbackOptions() { ResetFlag = doScan });
                            }
                        }
                    }, cancellationToken);
            });
        }

        public async Task LocationActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken)
        {
            var incoords = new double[] { userProfileTemporary.Latitude, userProfileTemporary.Longitude };

            dynamic w3wResult = await Helpers.GetWhat3WordsAddressAsync(incoords);

            await turnContext.SendActivityAsync(MessageFactory.Text($"{Loc.g("current_radius", userProfileTemporary.Radius)}\n\n" +
                                                                    $"{Loc.g("current_location", userProfileTemporary.Latitude.ToString("#0.000000", System.Globalization.CultureInfo.InvariantCulture), userProfileTemporary.Longitude.ToString("#0.000000", System.Globalization.CultureInfo.InvariantCulture))}\n\n" +
                                                                    (w3wResult != null ? $"what3words {Loc.g("address")}: {w3wResult?.words}" : "")
                                                                    ), cancellationToken);

            await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);
        }

        public async Task AnomalyActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog, bool doScan = false, int idacou = 1, string entropyQueryString = null)
        {
            if (turnContext.Activity.Text.Contains("["))
            {
                string[] buf = turnContext.Activity.Text.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                Int32.TryParse(buf[1], out idacou);
                if (idacou < 1 || idacou > 10)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("invalid_num_points")), cancellationToken);
                    return;
                }
            }

            if (doScan)
            {
                userProfileTemporary.IsScanning = true;
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("scan_time_take")), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("finding_location")), cancellationToken);
            }

            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        string mesg = "";
                        int numWaterPointsSkipped = 0;

                    redo:
                        var idatup = await GetIDAFromAzureFunction(userProfileTemporary.Location, userProfileTemporary.Radius, doScan ? 1 : 0, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token, entropyQueryString: entropyQueryString));
                        var ida = idatup.Item1;
                        var shaGid = idatup.Item2;
                        //if (string.IsNullOrEmpty(entropyQueryString) || !entropyQueryString.Contains("pool=true")) await turnContext.SendActivityAsync(MessageFactory.Text($"[Visualize your entropy in an image!]({API_SERVER}/visualizeEntropy?gid={shaGid})"), cancellationToken);
                        ida = SortIDA(ida, "any", idacou);
                        if (ida.Length > 0)
                        {
                            var shortCodesArray = new string[ida.Count()];
                            var messagesArray = new string[ida.Count()];
                            var pointTypesArray = new PointTypes[ida.Count()];
                            var numWaterPointsSkippedArray = new int[ida.Count()];
                            var what3WordsArray = new string[ida.Count()];
                            var nearestPlacesArray = new string[ida.Count()];

                            for (int i = 0; i < ida.Count(); i++)
                            {
                                var incoords = new double[] { ida[i].X.center.point.latitude, ida[i].X.center.point.longitude };

                                // If water points are set to be skipped, and there's only 1 point in the result array, try again else just exclude those from the results
                                if (!userProfileTemporary.IsIncludeWaterPoints)
                                {
                                    var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(incoords);
                                    if (isWaterPoint)
                                    {
                                        numWaterPointsSkipped++;

                                        if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                        {
                                            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("nothing_but_water_points")), cancellationToken);
                                            await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, new CallbackOptions() { ResetFlag = doScan });
                                            return;
                                        }

                                        if (ida.Length == 1)
                                        {
                                            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped_so_far", numWaterPointsSkipped)), cancellationToken);
                                            goto redo;
                                        }

                                        continue;
                                    }
                                }

                                shortCodesArray[i] = Helpers.Crc32Hash($"{turnContext.Activity.From.Id}{turnContext.Activity.Timestamp.ToString()}");
                                mesg = Tolog(turnContext, "ida", ida[i], shortCodesArray[i]);
                                await turnContext.SendActivityAsync(MessageFactory.Text((idacou > 1 ? ("#" + (i + 1) + " ") : "") + Helpers.DirectLineNewLineFix(turnContext, mesg)), cancellationToken);

                                dynamic w3wResult = await Helpers.GetWhat3WordsAddressAsync(incoords);

                                if (numWaterPointsSkipped > 0)
                                {
                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped", numWaterPointsSkipped)), cancellationToken);
                                }

                                messagesArray[i] = mesg;
                                pointTypesArray[i] = PointTypes.Anomaly;
                                numWaterPointsSkippedArray[i] = numWaterPointsSkipped;
                                what3WordsArray[i] = "" + w3wResult?.words;
                                nearestPlacesArray[i] = "" + w3wResult?.nearestPlace + Helpers.GetCountryFromW3W(w3wResult);

                                await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);
                                await Helpers.SendPushNotification(userProfileTemporary, Loc.g("point_generated"), mesg);
                                //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Point Generated", new Dictionary<string, object>() { { "Type", "Anomaly" }, { "IsScan", doScan }, { "Radius", userProfileTemporary.Radius }, { "RNG", userProfileTemporary.LastRNGType } });
                            }

                            CallbackOptions callbackOptions = new CallbackOptions()
                            {
                                StartTripReportDialog = true,
                                ShortCodes = shortCodesArray,
                                Messages = messagesArray,
                                PointTypes = pointTypesArray,
                                GeneratedPoints = ida,
                                ShaGids = new string[] { shaGid },
                                NumWaterPointsSkipped = numWaterPointsSkippedArray,
                                What3Words = what3WordsArray,
                                NearestPlaces = nearestPlacesArray,
                                ResetFlag = doScan,
                                DeductOwlTokens = 1
                            };
                            await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, callbackOptions);
                        }
                        else
                        {
                            if (!userProfileTemporary.IsUseClassicMode && !doScan)
                            {
                                mesg = Loc.g("no_points_detected_quantum");
                                await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);
                                await QuantumActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
                            }
                            else
                            {
                                mesg = Loc.g("no_points_detected");
                                await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);
                                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, new CallbackOptions() { ResetFlag = doScan });
                            }
                        }
                    }, cancellationToken);
            });
        }

        public async Task IntentSuggestionActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("get_intent_suggestions")), cancellationToken);

            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        string[] intentSuggestions = await Helpers.GetIntentSuggestionsAsync(new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                        string suggestionsStr = "  \n";
                        foreach (var word in intentSuggestions)
                        {
                            suggestionsStr += $"[{word}](https://www.google.com/search?q=define%20{word.Replace(" ", "%20")}) [({Loc.g("spotify_search")})](https://open.spotify.com/search/{word.Replace(" ", " %20")})  \n";
                        }

                        await turnContext.SendActivityAsync(MessageFactory.Text($"{Loc.g("intent_suggestions")}: " + suggestionsStr), cancellationToken);
                        await Helpers.SendPushNotification(userProfileTemporary, $"{Loc.g("intent_suggestions")}", suggestionsStr);

                        CallbackOptions callbackOptions = new CallbackOptions()
                        {
                            UpdateIntentSuggestions = true,
                            IntentSuggestions = intentSuggestions,
                            TimeIntentSuggestionsSet = turnContext.Activity.Timestamp.ToString()
                        };
                        await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, callbackOptions);
                    }, cancellationToken);
            });
        }

        public async Task QuantumActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog, bool suggestTime = false, bool forRemoteViewing = false)
        {
            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        int numWaterPointsSkipped = 0;

                    redo:
                        double[] incoords = GetQuantumRandom(userProfileTemporary.Latitude, userProfileTemporary.Longitude, forRemoteViewing ? 500000 /* go global */ : userProfileTemporary.Radius, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));

                        // Skip water points?
                        if (!userProfileTemporary.IsIncludeWaterPoints || forRemoteViewing)
                        {
                            var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(incoords);
                            if (isWaterPoint)
                            {
                                numWaterPointsSkipped++;

                                if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                {
                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("nothing_but_water_points")), cancellationToken);
                                    return;
                                }

                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped_so_far", numWaterPointsSkipped)), cancellationToken);
                                goto redo;
                            }
                        }

                        string shortCode = Helpers.Crc32Hash($"{turnContext.Activity.From.Id}{turnContext.Activity.Timestamp.ToString()}");
                        string mesg = "";
                        if (forRemoteViewing)
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text($"Remote viewing coordinates are {incoords[0].ToString("#0.000")},{incoords[1].ToString("#0.000")}. Look away from screen now."), cancellationToken);
                        }
                        else
                        {
                            mesg = Tolog(turnContext, "random", (float)incoords[0], (float)incoords[1], suggestTime ? "qtime" : "quantum", shortCode, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                        }
                        await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);

                        dynamic w3wResult = await Helpers.GetWhat3WordsAddressAsync(incoords);

                        if (forRemoteViewing)
                        {
                            await Task.Delay(15000);
                        }

                        await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult, paying: userProfileTemporary.HasMapsPack, forRemoteViewing: forRemoteViewing, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);
                        if (!string.IsNullOrEmpty(mesg))
                        {
                            await Helpers.SendPushNotification(userProfileTemporary, Loc.g("point_generated"), mesg);
                            //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Point Generated", new Dictionary<string, object>() { { "Type", "Quantum" + (suggestTime ? " Time" : "") }, { "Radius", userProfileTemporary.Radius }, { "RNG", userProfileTemporary.LastRNGType } });
                        }

                        CallbackOptions callbackOptions = new CallbackOptions()
                        {
                            StartTripReportDialog = !forRemoteViewing,
                            ShortCodes = new string[] { shortCode },
                            Messages = new string[] { mesg },
                            PointTypes = new PointTypes[] { suggestTime ? PointTypes.QuantumTime : PointTypes.Quantum },
                            GeneratedPoints = new FinalAttractor[] {
                                // FinalAttractor is just a good wrapper to carry point/type info across the boundary to the TripReportDialog
                                new FinalAttractor()
                                {
                                    X = new FinalAttr()
                                    {
                                        center = new Coordinate()
                                        {
                                            point = new LatLng(incoords[0], incoords[1]),
                                        },
                                    }
                                }
                            },
                            NumWaterPointsSkipped = new int[] { numWaterPointsSkipped },
                            What3Words = new string[] { w3wResult?.words },
                            NearestPlaces = new string[] { w3wResult?.nearestPlace + Helpers.GetCountryFromW3W(w3wResult) },
                            DeductOwlTokens = 1
                        };
                        await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken, callbackOptions);
                    }, cancellationToken);
            });
        }

        public async Task PseudoActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog)
        {
            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        int numWaterPointsSkipped = 0;

                    redo:
                        double[] incoords = GetPseudoRandom(userProfileTemporary.Latitude, userProfileTemporary.Longitude, userProfileTemporary.Radius);

                        // Skip water points?
                        if (!userProfileTemporary.IsIncludeWaterPoints)
                        {
                            var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(incoords);
                            if (isWaterPoint)
                            {
                                numWaterPointsSkipped++;

                                if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                {
                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("nothing_but_water_points")), cancellationToken);
                                    return;
                                }

                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped_so_far", numWaterPointsSkipped)), cancellationToken);
                                goto redo;
                            }
                        }

                        string shortCode = Helpers.Crc32Hash($"{turnContext.Activity.From.Id}{turnContext.Activity.Timestamp.ToString()}");
                        string mesg = Tolog(turnContext, "random", (float)incoords[0], (float)incoords[1], "pseudo", shortCode, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                        await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);

                        dynamic w3wResult = await Helpers.GetWhat3WordsAddressAsync(incoords);

                        await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);
                        await Helpers.SendPushNotification(userProfileTemporary, Loc.g("point_generated"), mesg);
                        //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Point Generated", new Dictionary<string, object>() { { "Type", "Pseudo" }, { "Radius", userProfileTemporary.Radius }, { "RNG", userProfileTemporary.LastRNGType } });

                        CallbackOptions callbackOptions = new CallbackOptions()
                        {
                            StartTripReportDialog = true,
                            ShortCodes = new string[] { shortCode },
                            Messages = new string[] { mesg },
                            PointTypes = new PointTypes[] { PointTypes.Pseudo },
                            GeneratedPoints = new FinalAttractor[] {
                                // FinalAttractor is just a good wrapper to carry point/type info across the boundary to the TripReportDialog
                                new FinalAttractor()
                                {
                                    X = new FinalAttr()
                                    {
                                        center = new Coordinate()
                                        {
                                            point = new LatLng(incoords[0], incoords[1]),
                                        },
                                    }
                                }
                            },
                            NumWaterPointsSkipped = new int[] { numWaterPointsSkipped },
                            What3Words = new string[] { w3wResult?.words },
                            NearestPlaces = new string[] { w3wResult?.nearestPlace + Helpers.GetCountryFromW3W(w3wResult) },
                            DeductOwlTokens = 0 // don't deduct for pseudo points
                        };
                        await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(turnContext, mainDialog, cancellationToken, callbackOptions);
                    }, cancellationToken);
            });
        }

        public async Task PairActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog, bool doScan = false, int idacou = 1, string entropyQueryString = null)
        {
            if (turnContext.Activity.Text.Contains("["))
            {
                string[] buf = turnContext.Activity.Text.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                Int32.TryParse(buf[1], out idacou);
                if (idacou < 1 || idacou > 10)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("invalid_num_points")), cancellationToken);
                    return;
                }
            }

            if (doScan)
            {
                userProfileTemporary.IsScanning = true;
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("scan_time_take")), cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("finding_location")), cancellationToken);
            }

            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        int numAttWaterPointsSkipped = 0;
                        int numVoiWaterPointsSkipped = 0;
                        string mesg = "";

                        var idatup = await GetIDAFromAzureFunction(userProfileTemporary.Location, userProfileTemporary.Radius, doScan ? 1 : 0, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token, entropyQueryString: entropyQueryString));
                        var ida = idatup.Item1;
                        var shaGid = idatup.Item2;
                        //if (string.IsNullOrEmpty(entropyQueryString) || !entropyQueryString.Contains("pool=true")) await turnContext.SendActivityAsync(MessageFactory.Text($"[Visualize your entropy in an image!]({API_SERVER}/visualizeEntropy?gid={shaGid})"), cancellationToken);
                        FinalAttractor[] att = SortIDA(ida, "attractor", idacou);
                        FinalAttractor[] voi = SortIDA(ida, "void", idacou);
                        if (att.Count() > voi.Count())
                        {
                            idacou = voi.Count();
                        }
                        else
                        {
                            idacou = att.Count();
                        }
                        if (idacou > 0)
                        {
                            var attShortCodesArray = new string[ida.Count()];
                            var attMessagesArray = new string[ida.Count()];
                            var attPointTypesArray = new PointTypes[ida.Count()];
                            var attNumWaterPointsSkippedArray = new int[ida.Count()];
                            var attWhat3WordsArray = new string[ida.Count()];
                            var attNearestPlacesArray = new string[ida.Count()];

                            var voiShortCodesArray = new string[ida.Count()];
                            var voiMessagesArray = new string[ida.Count()];
                            var voiPointTypesArray = new PointTypes[ida.Count()];
                            var voiNumWaterPointsSkippedArray = new int[ida.Count()];
                            var voiWhat3WordsArray = new string[ida.Count()];
                            var voiNearestPlacesArray = new string[ida.Count()];

                            for (int i = 0; i < idacou; i++)
                            {
                                // TODO: The "(idacou > 1 ? ("#"+(i+1)+" ") : "")" logic below for pairs needs to be split between attractor/void

                                var incoords = new double[] { att[i].X.center.point.latitude, att[i].X.center.point.longitude };
                                if (!userProfileTemporary.IsIncludeWaterPoints && await Helpers.IsWaterCoordinatesAsync(incoords))
                                {
                                    numAttWaterPointsSkipped++;
                                }
                                else
                                {
                                    attShortCodesArray[i] = Helpers.Crc32Hash($"{turnContext.Activity.From.Id}{turnContext.Activity.Timestamp.ToString()}");

                                    mesg = Tolog(turnContext, "attractor", att[i], attShortCodesArray[i]);
                                    await turnContext.SendActivityAsync(MessageFactory.Text((idacou > 1 ? ("#" + (i + 1) + " ") : "") + Helpers.DirectLineNewLineFix(turnContext, mesg)), cancellationToken);
                                    dynamic w3wResult1 = await Helpers.GetWhat3WordsAddressAsync(incoords);
                                    await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult1, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);

                                    attMessagesArray[i] = mesg;
                                    attPointTypesArray[i] = PointTypes.PairAttractor;
                                    attNumWaterPointsSkippedArray[i] = numAttWaterPointsSkipped;
                                    attWhat3WordsArray[i] = "" + w3wResult1.words;
                                    attNearestPlacesArray[i] = "" + w3wResult1.nearestPlace + Helpers.GetCountryFromW3W(w3wResult1);
                                }

                                incoords = new double[] { voi[i].X.center.point.latitude, voi[i].X.center.point.longitude };
                                if (!userProfileTemporary.IsIncludeWaterPoints && await Helpers.IsWaterCoordinatesAsync(incoords))
                                {
                                    numVoiWaterPointsSkipped++;
                                }
                                else
                                {
                                    voiShortCodesArray[i] = Helpers.Crc32Hash($"{turnContext.Activity.From.Id}{turnContext.Activity.Timestamp.ToString()}");

                                    mesg = Tolog(turnContext, "void", voi[i], voiShortCodesArray[i]);
                                    await turnContext.SendActivityAsync(MessageFactory.Text((idacou > 1 ? ("#" + (i + 1) + " ") : "") + Helpers.DirectLineNewLineFix(turnContext, mesg)), cancellationToken);
                                    dynamic w3wResult2 = await Helpers.GetWhat3WordsAddressAsync(incoords);
                                    await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult2, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);

                                    voiMessagesArray[i] = mesg;
                                    voiPointTypesArray[i] = PointTypes.PairVoid;
                                    voiNumWaterPointsSkippedArray[i] = numAttWaterPointsSkipped;
                                    voiWhat3WordsArray[i] = "" + w3wResult2.words;
                                    voiNearestPlacesArray[i] = "" + w3wResult2.nearestPlace + Helpers.GetCountryFromW3W(w3wResult2);
                                }
                            }

                            if (numAttWaterPointsSkipped > 1)
                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_attractors_skipped", numAttWaterPointsSkipped)), cancellationToken);
                            if (numVoiWaterPointsSkipped > 1)
                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_voids_skipped", numVoiWaterPointsSkipped)), cancellationToken);

                            await Helpers.SendPushNotification(userProfileTemporary, Loc.g("pair_points_detected"), attMessagesArray[0]); // just send one notification
                            //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Point Generated", new Dictionary<string, object>() { { "Type", "Pair" }, { "Radius", userProfileTemporary.Radius }, { "RNG", userProfileTemporary.LastRNGType } });

                            CallbackOptions callbackOptions = new CallbackOptions()
                            {
                                StartTripReportDialog = true,
                                ShortCodes = attShortCodesArray.Concat(voiShortCodesArray).ToArray(),
                                Messages = attMessagesArray.Concat(voiMessagesArray).ToArray(),
                                PointTypes = attPointTypesArray.Concat(voiPointTypesArray).ToArray(),
                                GeneratedPoints = att.Concat(voi).ToArray(),
                                ShaGids = new string[] { shaGid },
                                NumWaterPointsSkipped = attNumWaterPointsSkippedArray.Concat(voiNumWaterPointsSkippedArray).ToArray(),
                                What3Words = attWhat3WordsArray.Concat(voiWhat3WordsArray).ToArray(),
                                NearestPlaces = attNearestPlacesArray.Concat(voiNearestPlacesArray).ToArray(),
                                ResetFlag = doScan,
                                DeductOwlTokens = 1
                            };
                            await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, callbackOptions);
                        }
                        else
                        {
                            if (!userProfileTemporary.IsUseClassicMode && !doScan)
                            {
                                mesg = Loc.g("no_points_detected_quantum");
                                await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);
                                await QuantumActionAsync(turnContext, userProfileTemporary, cancellationToken, mainDialog);
                            }
                            else
                            {
                                mesg = Loc.g("no_points_detected");
                                await turnContext.SendActivityAsync(MessageFactory.Text(mesg), cancellationToken);
                                await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, new CallbackOptions() { ResetFlag = doScan });
                            }
                        }
                    }, cancellationToken);
            });
        }

        public async Task ChainActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog, Enums.PointTypes pointType, int maxDistance, bool isCentered = false)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("generating_chain")), cancellationToken);

            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        string mesg = "";
                        int numWaterPointsSkipped = 0;

                        string idaType = "";
                        if (pointType == PointTypes.Attractor) idaType = "attractor";
                        else if (pointType == PointTypes.Void) idaType = "void";
                        else if (pointType == PointTypes.Anomaly) idaType = "any";

                        int numPoints = (int)maxDistance; // TODO: revise numPoints to a fancy distance calculation algo
                        FinalAttractor[] generatedPoints = new FinalAttractor[numPoints];
                        var shortCodesArray = new string[numPoints];
                        var messagesArray = new string[numPoints];
                        var pointTypesArray = new PointTypes[numPoints];
                        var shaGids = new string[numPoints];
                        var numWaterPointsSkippedArray = new int[numPoints];
                        var what3WordsArray = new string[numPoints];
                        var nearestPlacesArray = new string[numPoints];

                        for (int j = 0; j < numPoints; j++)
                        {
                            LatLng centerLocation;
                            if (isCentered
                                || j == 0 // first point
                                )
                            {
                                // each point generated from the current user's location
                                centerLocation = userProfileTemporary.Location;
                            }
                            else
                            {
                                // sequentially reset current location to each previously generated point
                                centerLocation = new LatLng(generatedPoints[j - 1].X.center.point.latitude, generatedPoints[j - 1].X.center.point.longitude);
                            }

                        redo:
                            var idatup = await GetIDAFromAzureFunction(centerLocation, userProfileTemporary.Radius, 0, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                            var idas = idatup.Item1;
                            var shaGid = idatup.Item2;
                            //await turnContext.SendActivityAsync(MessageFactory.Text($"[Visualize your entropy in an image!]({API_SERVER}/visualizeEntropy?gid={shaGid})"), cancellationToken);
                            idas = SortIDA(idas, idaType, 1);

                            if (idas.Count() == 0)
                            {
                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("chain_nothing_found", j+1)), cancellationToken);
                                goto redo;
                            }

                            generatedPoints[j] = idas[0];
                            var incoords = new double[] { generatedPoints[j].X.center.point.latitude, generatedPoints[j].X.center.point.longitude };

                            // If water points are set to be skipped, and there's only 1 point in the result array, try again else just exclude those from the results
                            if (!userProfileTemporary.IsIncludeWaterPoints)
                            {
                                var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(incoords);
                                if (isWaterPoint)
                                {
                                    numWaterPointsSkipped++;

                                    if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                    {
                                        await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("chain_nothing_but_water_points", j+1)), cancellationToken);
                                        await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, new CallbackOptions() { ResetFlag = false });
                                        return;
                                    }

                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("chain_num_water_points_skipped_so_far", j+1, numWaterPointsSkipped)), cancellationToken);
                                    goto redo;
                                }
                            }

                            shortCodesArray[j] = Helpers.Crc32Hash($"{turnContext.Activity.From.Id}{turnContext.Activity.Timestamp.ToString()}");
                            mesg = Tolog(turnContext, idaType, idas[0], shortCodesArray[j]);
                            await turnContext.SendActivityAsync(MessageFactory.Text($"#{j+1} {Helpers.DirectLineNewLineFix(turnContext, mesg)}"), cancellationToken);

                            dynamic w3wResult = await Helpers.GetWhat3WordsAddressAsync(incoords);

                            if (numWaterPointsSkipped > 0)
                            {
                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("chain_num_water_points_skipped", j+1, numWaterPointsSkipped)), cancellationToken);
                            }

                            messagesArray[j] = mesg;
                            pointTypesArray[j] = Enum.Parse<PointTypes>("Chain" + pointType.ToString());
                            shaGids[j] = shaGid;
                            numWaterPointsSkippedArray[j] = numWaterPointsSkipped;
                            what3WordsArray[j] = "" + w3wResult?.words;
                            nearestPlacesArray[j] = "" + w3wResult?.nearestPlace + Helpers.GetCountryFromW3W(w3wResult);

                            await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);
                        }

                        var origin = new FinalAttractor[] {
                                // FinalAttractor is just a wrapper for the origin
                                new FinalAttractor()
                                {
                                    X = new FinalAttr()
                                    {
                                        center = new Coordinate()
                                        {
                                            point = userProfileTemporary.Location,
                                        },
                                    }
                                }
                            };
                        var chain = origin.Concat(generatedPoints).ToArray();
                        await turnContext.SendActivitiesAsync(CardFactory.CreateChainCardReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), chain), cancellationToken);
                        await Helpers.SendPushNotification(userProfileTemporary, Loc.g("chain_generated"), mesg);
                        //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Chain Generated", new Dictionary<string, object>() { { "Type", pointType }, { "Radius", userProfileTemporary.Radius }, { "RNG", userProfileTemporary.LastRNGType }, { "Ordering", isCentered ? "Centered" : "Sequential" }, { "MaxDistance", maxDistance } });

                        CallbackOptions callbackOptions = new CallbackOptions()
                        {
                            StartTripReportDialog = true,
                            ShortCodes = shortCodesArray,
                            Messages = messagesArray,
                            PointTypes = pointTypesArray,
                            GeneratedPoints = generatedPoints,
                            ShaGids = shaGids,
                            NumWaterPointsSkipped = numWaterPointsSkippedArray,
                            What3Words = what3WordsArray,
                            NearestPlaces = nearestPlacesArray,
                            ResetFlag = false
                        };
                        await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, callbackOptions);
                    }, cancellationToken);
            });
        }

        public async Task MysteryPointActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("finding_location")), cancellationToken);

            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        string mesg = "";
                        double[] incoords = new double[2];
                        int numWaterPointsSkipped = 0;

                    redoIDA:
                        var idatup = await GetIDAFromAzureFunction(userProfileTemporary.Location, userProfileTemporary.Radius, 0, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                        var ida = idatup.Item1;
                        var shaGid = idatup.Item2;
                        //await turnContext.SendActivityAsync(MessageFactory.Text($"[Visualize your entropy in an image!]({API_SERVER}/visualizeEntropy?gid={shaGid})"), cancellationToken);
                        FinalAttractor[] att = SortIDA(ida, "attractor", 1);
                        if (att.Length > 0 && !userProfileTemporary.IsIncludeWaterPoints)
                        {
                            var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(new double[] { att[0].X.center.point.latitude, att[0].X.center.point.longitude });
                            if (isWaterPoint)
                            {
                                numWaterPointsSkipped++;

                                if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                {
                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("nothing_but_water_points")), cancellationToken);
                                    await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken);
                                    return;
                                }

                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped_so_far", numWaterPointsSkipped)), cancellationToken);
                                goto redoIDA;
                            }
                        }

                        FinalAttractor[] voi = SortIDA(ida, "void", 1);
                        if (voi.Length > 0 && !userProfileTemporary.IsIncludeWaterPoints)
                        {
                            var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(new double[] { voi[0].X.center.point.latitude, voi[0].X.center.point.longitude });
                            if (isWaterPoint)
                            {
                                numWaterPointsSkipped++;

                                if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                {
                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("nothing_but_water_points")), cancellationToken);
                                    await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken);
                                    return;
                                }

                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped_so_far", numWaterPointsSkipped)), cancellationToken);
                                goto redoIDA;
                            }
                        }

                        ida = SortIDA(ida, "any", 1);

                    redoPseudo:
                        double[] pcoords = GetPseudoRandom(userProfileTemporary.Location.latitude, userProfileTemporary.Location.longitude, userProfileTemporary.Radius);
                        if (!userProfileTemporary.IsIncludeWaterPoints)
                        {
                            var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(pcoords);
                            if (isWaterPoint)
                            {
                                numWaterPointsSkipped++;

                                if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                {
                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("nothing_but_water_points")), cancellationToken);
                                    await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken);
                                    return;
                                }

                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped_so_far", numWaterPointsSkipped)), cancellationToken);
                                goto redoPseudo;
                            }
                        }

                    redoQuantum:
                        double[] qcoords = GetQuantumRandom(userProfileTemporary.Location.latitude, userProfileTemporary.Location.longitude, userProfileTemporary.Radius, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                        if (!userProfileTemporary.IsIncludeWaterPoints)
                        {
                            var isWaterPoint = await Helpers.IsWaterCoordinatesAsync(qcoords);
                            if (isWaterPoint)
                            {
                                numWaterPointsSkipped++;

                                if (numWaterPointsSkipped > Consts.WATER_POINTS_SEARCH_MAX)
                                {
                                    await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("nothing_but_water_points")), cancellationToken);
                                    await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken);
                                    return;
                                }

                                await turnContext.SendActivityAsync(MessageFactory.Text(Loc.g("num_water_points_skipped_so_far", numWaterPointsSkipped)), cancellationToken);
                                goto redoQuantum;
                            }
                        }

                        Random rn = new Random();
                        int mtd = rn.Next(100);
                        var shortCode = Helpers.Crc32Hash($"{turnContext.Activity.From.Id}{turnContext.Activity.Timestamp.ToString()}");

                        if (mtd < 20)
                        {
                            incoords[0] = pcoords[0];
                            incoords[1] = pcoords[1];
                            mesg = Tolog(turnContext, "blind", (float)incoords[0], (float)incoords[1], "pseudo", shortCode, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                        }
                        if ((mtd < 40) && (mtd > 20))
                        {
                            if (att.Count() > 0)
                            {
                                incoords[0] = att[0].X.center.point.latitude;
                                incoords[1] = att[0].X.center.point.longitude;
                                mesg = Tolog(turnContext, "blind", att[0], shortCode);
                            }
                            else
                            {
                                incoords[0] = pcoords[0];
                                incoords[1] = pcoords[1];
                                mesg = Tolog(turnContext, "blind", (float)incoords[0], (float)incoords[1], "pseudo", shortCode, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                            }
                        }
                        if ((mtd < 60) && (mtd > 40))
                        {
                            incoords[0] = qcoords[0];
                            incoords[1] = qcoords[1];
                            mesg = Tolog(turnContext, "blind", (float)incoords[0], (float)incoords[1], "quantum", shortCode, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                        }
                        if ((mtd < 80) && (mtd > 60))
                        {
                            if (voi.Count() > 0)
                            {
                                incoords[0] = voi[0].X.center.point.latitude;
                                incoords[1] = voi[0].X.center.point.longitude;
                                mesg = Tolog(turnContext, "blind", voi[0], shortCode);
                            }
                            else
                            {
                                mtd = 90;
                            }
                        }
                        if ((mtd < 100) && (mtd > 80))
                        {
                            if (ida.Count() > 0)
                            {
                                incoords[0] = ida[0].X.center.point.latitude;
                                incoords[1] = ida[0].X.center.point.longitude;
                                mesg = Tolog(turnContext, "blind", ida[0], shortCode);
                            }
                            else
                            {
                                incoords[0] = qcoords[0];
                                incoords[1] = qcoords[1];
                                mesg = Tolog(turnContext, "blind", (float)incoords[0], (float)incoords[1], "quantum", shortCode, new QuantumRandomNumberGeneratorWrapper(context, mainDialog, token));
                            }
                        }

                        await turnContext.SendActivityAsync(MessageFactory.Text(Helpers.DirectLineNewLineFix(turnContext, mesg)), cancellationToken);

                        dynamic w3wResult = await Helpers.GetWhat3WordsAddressAsync(incoords);

                        await turnContext.SendActivitiesAsync(CardFactory.CreateLocationCardsReply(Enum.Parse<ChannelPlatform>(turnContext.Activity.ChannelId), incoords, userProfileTemporary.IsDisplayGoogleThumbnails, w3wResult: w3wResult, paying: userProfileTemporary.HasMapsPack, isIOS: userProfileTemporary.BotSrc == WebSrc.ios), cancellationToken);
                        await Helpers.SendPushNotification(userProfileTemporary, Loc.g("mystery_point_generated"), mesg);
                        //AmplitudeService.Amplitude.InstanceFor(userProfileTemporary.UserId, userProfileTemporary.UserProperties).Track("Point Generated", new Dictionary<string, object>() { { "Type", "Mystery" }, { "Radius", userProfileTemporary.Radius }, { "RNG", userProfileTemporary.LastRNGType } });

                        CallbackOptions callbackOptions = new CallbackOptions()
                        {
                            StartTripReportDialog = true,
                            ShortCodes = new string[] { shortCode },
                            Messages = new string[] { mesg },
                            PointTypes = new PointTypes[] { PointTypes.MysteryPoint },
                            GeneratedPoints = ida,
                            ShaGids = new string[] { shaGid },
                            NumWaterPointsSkipped = new int[] { numWaterPointsSkipped },
                            What3Words = new string[] { w3wResult?.words },
                            NearestPlaces = new string[] { w3wResult?.nearestPlace + Helpers.GetCountryFromW3W(w3wResult) },
                            DeductOwlTokens = 1
                        };
                        await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken, callbackOptions);
                    }, cancellationToken);
            });
        }

        public async Task RandotripsActionAsync(ITurnContext turnContext, UserProfileTemporary userProfileTemporary, CancellationToken cancellationToken, MainDialog mainDialog, string date)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text("Grabbing the randotrips... hold on."), cancellationToken);

            DispatchWorkerThread((object sender, DoWorkEventArgs e) =>
            {
                turnContext.Adapter.ContinueConversationAsync(Consts.APP_ID, turnContext.Activity.GetConversationReference(),
                    async (context, token) =>
                    {
                        string whereClause = "", filename = "";
                        var ext = ".kml";
                        var isMyRandotrips = false;
                        if ("my".Equals(date))
                        {
                            whereClause = $"AND user_id = '{userProfileTemporary.UserId}'";
                            filename = userProfileTemporary.UserId + ext;
                            isMyRandotrips = true;
                        }
                        else if ("all".Equals(date))
                        {
                            whereClause = ""; // no extra filtering. hushhush.
                            filename = "all" + ext;
                        }
                        else
                        {
                            whereClause = $"AND DATETIME LIKE '%{date}%'";
                            filename = $"randotrips_{date}{ext}";
                        }

                        var numPoints = RandotripKMLGenerator.Generate(whereClause, filename, isMyRandotrips);
                        if (numPoints < 0)
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text("There was an error. Uh-oh."), cancellationToken);
                        }
                        else if (numPoints == 0)
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text("There are no trips."), cancellationToken);
                        }
                        else
                        {
#if RELEASE_PROD
                            var baseUrl = "https://bot.randonauts.com/flythrus/";
#else
                            var baseUrl = "https://devbot.randonauts.com/flythrus/";
#endif
                            await turnContext.SendActivityAsync(MessageFactory.Text(Helpers.DirectLineNewLineFix(turnContext,
                                $"If you're downloading from a web browser on a computer, goto https://earth.google.com/web/ in Chrome → click ☰ → Projects → New Project → Import KML file from computer → load it → select the most recent one and tap PLAY▶︎\n\n\n\n" +
                                "On phones; open/copy/share the file with the Google Earth app → tap ☰ → Projects, select the most recent one and tap PLAY▶︎")
                                ), cancellationToken);
                            await turnContext.SendActivityAsync(MessageFactory.Text($"Found {numPoints} trip{(numPoints > 1 ? "s" : "")}. Download this file: {baseUrl}{filename}"), cancellationToken);
                        }
                        await ((AdapterWithErrorHandler)turnContext.Adapter).RepromptMainDialog(context, mainDialog, cancellationToken);
                    }, cancellationToken);
            });
        }
    }
}
