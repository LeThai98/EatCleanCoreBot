// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.13.2

using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using MyBot.Dialogs.Operations;
using MyBot.SentimentBot;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly TodoLUISRecognizer _luisRecognizer;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(TodoLUISRecognizer luisRecognizer, ILogger<MainDialog> logger)  : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            Logger = logger;

            // tiến hành khởi tạo các Dialog theo khai báo
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new CreateTaskDialog());
            AddDialog(new ProductFeedBackDialog());
            AddDialog(new DeleteTaskDialog());
            AddDialog(new PredictSentimentDiaglog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(
                 MessageFactory.Text("What operation you would like to perform?"), cancellationToken);

            List<string> operationList = new List<string> { "Nutrition Consulting", "Product Feedback", };
            // Create card
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                // Use LINQ to turn the choices into submit actions
                Actions = operationList.Select(choice => new AdaptiveSubmitAction
                {
                    Title = choice,
                    Data = choice,  // This will be a string
                }).ToList<AdaptiveAction>(),
            };

            //Prompt
            return await stepContext.PromptAsync(
             nameof(ChoicePrompt),
             new PromptOptions
             {
                 Prompt = (Activity)MessageFactory.Attachment(new Attachment
                 {
                     ContentType = AdaptiveCard.ContentType,
                     // Convert the AdaptiveCard to a JObject
                     Content = JObject.FromObject(card),
                 }),
                 Choices = ChoiceFactory.ToChoices(operationList),
                 // Don't render the choices outside the card
                 Style = ListStyle.None,
             },
             cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
            string operation = (string)stepContext.Values["Operation"];
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("You have selected - " + operation), cancellationToken);
            

            if ("Nutrition Consulting".Equals(operation))
            {
                // khởi tạo 1 đối tượng user và chạy class CreateTaskDialog
                return await stepContext.BeginDialogAsync(nameof(CreateTaskDialog), new User(), cancellationToken);
               // return await stepContext.ReplaceDialogAsync(nameof(ChoicePrompt), cancellationToken);
               //while( true)
               // {
               //     return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
               //     {
               //         Prompt = MessageFactory.Text("Hồng Hạnh.")
               //     }, cancellationToken);

               //     string x = (string)stepContext.Result;
               //     if (x == "ok")
               //         continue;

               // }    
            }
            else if ("Product Feedback".Equals(operation))
            {
                return await stepContext.BeginDialogAsync(nameof(PredictSentimentDiaglog), new User(), cancellationToken);
            }
           
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("User Input not matched."), cancellationToken);
                return await stepContext.NextAsync(null, cancellationToken);
            }
            

            // return await stepContext.NextAsync(null, cancellationToken);
        }
    

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           
            // Restart the main dialog with a different message the second time around
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
