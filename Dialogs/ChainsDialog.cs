﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Logging;
using VFatumbot.BotLogic;

namespace VFatumbot
{
    public class ChainsDialog : ComponentDialog
    {
        protected readonly ILogger _logger;
        protected readonly IStatePropertyAccessor<UserProfileTemporary> _userProfileTemporaryAccessor;
        protected readonly MainDialog _mainDialog;

        public ChainsDialog(IStatePropertyAccessor<UserProfileTemporary> userProfileTemporaryAccessor, MainDialog mainDialog, ILogger<MainDialog> logger) : base(nameof(ChainsDialog))
        {
            _logger = logger;
            _userProfileTemporaryAccessor = userProfileTemporaryAccessor;
            _mainDialog = mainDialog;

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt))
            {
            });
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), DistanceValidatorAsync)
            {
            });
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChoosePointTypeStepAsync,
                ChooseCenterLocationTypeStepAsync,
                EnterDistanceStepAsync,
                StartChainingStepAsync
            })
            {
            });

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ChoosePointTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //_logger.LogInformation("ChainsDialog.ChoosePointTypeStepAsync");

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(Loc.g("choose_chain_type")),
                RetryPrompt = MessageFactory.Text(Loc.g("ch_invalid_point_type")),
                Choices = new List<Choice>()
                {
                    new Choice() {
                        Value = Loc.g("ch_attractors"),
                        Synonyms = new List<string>()
                                        {
                                            "attractors",
                                        }
                    },
                    new Choice() {
                        Value = Loc.g("ch_voids"),
                        Synonyms = new List<string>()
                                        {
                                            "voids",
                                            "Repellers",
                                            "repellers",
                                        }
                    },
                    new Choice() {
                        Value = Loc.g("ch_anomalies"),
                        Synonyms = new List<string>()
                                        {
                                            "anomalies",
                                        }
                    },
                    // TODO: implement one day
                    //new Choice() {
                    //    Value = "Quantums",
                    //    Synonyms = new List<string>()
                    //                    {
                    //                        "quantums",
                    //                    }
                    //},
                    //new Choice() {
                    //    Value = "Pseudos",
                    //    Synonyms = new List<string>()
                    //                    {
                    //                        "pseudos",
                    //                    }
                    //},
                    //new Choice() {
                    //    Value = "Mystery Points",
                    //    Synonyms = new List<string>()
                    //                    {
                    //                        "Mystery points",
                    //                        "mystery points",
                    //                    }
                    //},
                    //new Choice() {
                    //    Value = "< Back",
                    //    Synonyms = new List<string>()
                    //                    {
                    //                        "<",
                    //                        "Back",
                    //                        "back",
                    //                        "<back",
                    //                    }
                    //},
                }
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> ChooseCenterLocationTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //_logger.LogInformation($"ChainsDialog.ChooseCenterLocationTypeStepAsync[{((FoundChoice)stepContext.Result)?.Value}]");

            stepContext.Values["point_type"] = ((FoundChoice)stepContext.Result)?.Value;

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(Loc.g("ch_current_or_sequential")),
                RetryPrompt = MessageFactory.Text(Loc.g("invalid_answer")),
                Choices = new List<Choice>()
                {
                    new Choice() {
                        Value = Loc.g("ch_current"),
                        Synonyms = new List<string>()
                                        {
                                            "current",
                                        }
                    },
                    new Choice() {
                        Value = Loc.g("ch_sequential"),
                        Synonyms = new List<string>()
                                        {
                                            "sequential",
                                        }
                    },
                }
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> EnterDistanceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //_logger.LogInformation($"ChainsDialog.EnterDistanceStepAsync[{((FoundChoice)stepContext.Result)?.Value}]");

            stepContext.Values["center_location"] = ((FoundChoice)stepContext.Result)?.Value;

            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text(Loc.g("ch_num_points")),
                RetryPrompt = MessageFactory.Text(Loc.g("ch_invalid_number")),
            };

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), options, cancellationToken);
        }

        private async Task<bool> DistanceValidatorAsync(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
            float inputtedDistance;
            if (!float.TryParse(promptContext.Context.Activity.Text, out inputtedDistance))
            {
                await promptContext.Context.SendActivityAsync(MessageFactory.Text(Loc.g("ch_reprompt_num_points")), cancellationToken);
                return false;
            }

            if (inputtedDistance < Consts.CHAIN_DISTANCE_MIN)
            {
                await promptContext.Context.SendActivityAsync(MessageFactory.Text(Loc.g("ch_mte", Consts.CHAIN_DISTANCE_MIN)), cancellationToken);
                return false;
            }

            if (inputtedDistance > Consts.CHAIN_DISTANCE_MAX)
            {
                await promptContext.Context.SendActivityAsync(MessageFactory.Text(Loc.g("ch_lte", Consts.CHAIN_DISTANCE_MAX)), cancellationToken);
                return false;
            }

            return true;
        }

        private async Task<DialogTurnResult> StartChainingStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //_logger.LogInformation("ChainsDialog.StartChainingStepAsync");

            stepContext.Values["preferred_distance"] = stepContext.Result;

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text(
            //    $"type: {stepContext.Values["point_type"]}\n\n" +
            //    $"center generation: {stepContext.Values["center_location"]}\n\n" +
            //    $"preferred distance: {stepContext.Values["preferred_distance"]}\n\n"
            //    ), cancellationToken);


            var userProfileTemporary = await _userProfileTemporaryAccessor.GetAsync(stepContext.Context, () => new UserProfileTemporary());
            var actionHandler = new ActionHandler();

            var val = stepContext.Values["point_type"].ToString();

            if (Loc.g("ch_attractors").Equals(val))
            {
                await actionHandler.ChainActionAsync(stepContext.Context, userProfileTemporary, cancellationToken, _mainDialog,
                                        Enums.PointTypes.Attractor, (int)stepContext.Values["preferred_distance"], stepContext.Values["center_location"].ToString().ToLower().Equals("current"));
            }
            else if (Loc.g("ch_voids").Equals(val))
            {
                await actionHandler.ChainActionAsync(stepContext.Context, userProfileTemporary, cancellationToken, _mainDialog,
                                        Enums.PointTypes.Void, (int)stepContext.Values["preferred_distance"], stepContext.Values["center_location"].ToString().ToLower().Equals("current"));
            }
            else if (Loc.g("ch_anomalies").Equals(val))
            {
                await actionHandler.ChainActionAsync(stepContext.Context, userProfileTemporary, cancellationToken, _mainDialog,
                                        Enums.PointTypes.Anomaly, (int)stepContext.Values["preferred_distance"], stepContext.Values["center_location"].ToString().ToLower().Equals("current"));
            }

            //case "Quantums":
            //    await actionHandler.ChainActionAsync(stepContext.Context, userProfileTemporary, cancellationToken, _mainDialog,
            //                            Enums.PointTypes.Quantum, (int)stepContext.Values["preferred_distance"], stepContext.Values["center_location"].ToString().ToLower().Equals("current"));
            //    break;
            //case "Pseudos":
            //    await actionHandler.ChainActionAsync(stepContext.Context, userProfileTemporary, cancellationToken, _mainDialog,
            //                            Enums.PointTypes.Pseudo, (int)stepContext.Values["preferred_distance"], stepContext.Values["center_location"].ToString().ToLower().Equals("current"));
            //    break;

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
