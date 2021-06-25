using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.Number;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBot.Dialogs.Validations
{
    public class ValidateWeight: ComponentDialog
    {
        public ValidateWeight() : base(nameof(ValidateWeight))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                AskWeightAsync,
                ConfirmStepAsync,
               
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private async Task<DialogTurnResult> AskWeightAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("How many kg do you weight? Please enter an weight greater than 0.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;

            string input = (string)stepContext.Result;
            double weight = 0;
            string message = null;

            if (!CheckWeight(input, out weight, out message))
            {
                return await stepContext.ReplaceDialogAsync(InitialDialogId, userDetails, cancellationToken);
            }
            else
            {
                stepContext.Values["Task"] = weight;
                userDetails.Weight = weight;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your weight is {weight}kg."), cancellationToken);
                return await stepContext.EndDialogAsync(userDetails, cancellationToken);
            }
        }
        private bool CheckWeight(string input, out double weight, out string message)
        {
            weight = 0;
            message = null;

            // Try to recognize the input as a number. This works for responses such as "twelve" as well as "12".
            try
            {
                // Attempt to convert the Recognizer result to an integer. This works for "a dozen", "twelve", "12", and so on.
                // The recognizer returns a list of potential recognition results, if any.

                var results = NumberRecognizer.RecognizeNumber(input, Culture.English);

                foreach (var result in results)
                {
                    // The result resolution is a dictionary, where the "value" entry contains the processed string.
                    if (result.Resolution.TryGetValue("value", out var value))
                    {
                        string Va = value.ToString();
                        weight = double.Parse(Va);
                        //weight = (double)x;
                        //var a = weight;
                        if (weight > 0)
                        {
                            return true;
                        }
                    }
                }

                message = "Please enter an weight greater than 0. ";
            }
            catch
            {
                message = "I'm sorry, I could not interpret that as an age. Please enter an age between 18 and 120.";
            }

            return message is null;
        }


    }
}
