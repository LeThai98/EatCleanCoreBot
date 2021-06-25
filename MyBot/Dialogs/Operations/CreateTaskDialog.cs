using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.Number;
using MyBot.Dialogs.Validations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace MyBot.Dialogs.Operations
{
    public class CreateTaskDialog: ComponentDialog
    {
        public CreateTaskDialog() : base(nameof(CreateTaskDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                AskNameAsync,
                AskSexAsync,
                AskAgeAsync,
                AskWeightAsync,
                ValidateWeightAsync,
               AskHeightAsync,
               BMIAsync,
                SumaryStepAsync

            };

            // WaterfallDialog có các bước dc khởi tạo trong biến waterfallSteps
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new CreateMoreTaskDialog());
            AddDialog(new ValidateWeight());
            AddDialog(new ThinConsultingDialog());
            AddDialog(new NormalConsultingDialog());
            AddDialog(new FatConsultingDialog());
            AddDialog(new QuiteFatConsultingDialog());

            // lựa chọn Diaglog initial 
            InitialDialogId = nameof(WaterfallDialog);
        }

        

        private async  Task<DialogTurnResult> AskNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Hello and welcome.What is your name?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AskSexAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
            userDetails.Name = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(
                 MessageFactory.Text($"Hey {userDetails.Name},Are you male or female?"), cancellationToken);

            List<string> operationList = new List<string> { "Male", "Female" };
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
                 Prompt = (Microsoft.Bot.Schema.Activity)MessageFactory.Attachment(new Microsoft.Bot.Schema.Attachment
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

        private async Task<DialogTurnResult> AskAgeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
            string operation = (string)stepContext.Values["Operation"];
            var userDetails = (User)stepContext.Options;
            userDetails.Sex = operation;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("How old are you?")
            }, cancellationToken);

        }

        // This will be Text Prompt asking the user to provide the task.
        private async Task<DialogTurnResult> AskWeightAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
            string age = (string)stepContext.Result;
            userDetails.Age = int.Parse(age);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("How many kg do you weigh?") 
            }, cancellationToken);
        }

        // Capture the user response from TasksStepAsync and ask the user if he/she wants to add more tasks. This will be a Confirm Prompt that shows the user two buttons (Yes and No). The value returned by these buttons is of type boolean.
        private async Task<DialogTurnResult> ValidateWeightAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // tìm đến class User 
            //var userDetails = (User)stepContext.Options;
            // lấy kq từ user
            //stepContext.Values["Task"] = (string)stepContext.Result;
            // add kq và List
            //userDetails.TasksList.Add((string)stepContext.Values["Task"]);


            var userDetails = (User)stepContext.Options;

            string input = (string)stepContext.Result;
            double weight = 0;
            string message = null;
            if( CheckWeight(input, out weight, out message))
            {
              
                
                userDetails.Weight = weight;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your weight is {weight}kg."), cancellationToken);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("How many mets do you height?")
                }, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(ValidateWeight), userDetails, cancellationToken);
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

        //Capture the boolean response. If Yes, begin another dialog CreateMoreTaskDialog.cs (Create a new class file with a constructor similar to CreateTaskDialog.) If No, move to the next step.
        private async Task<DialogTurnResult> AskHeightAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
           // await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{userDetails.Name}"), cancellationToken);
          

            string input = (string)stepContext.Result;
            double height = 0;
            string message = null;
            if (CheckHeight(input, out height, out message))
            {

                //stepContext.Values["Task"] = height;
                //userDetails.TasksList.Add((string)stepContext.Values["Task"]);
                userDetails.Height = height;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your height is {height}m."), cancellationToken);

                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Have you use food of Vegafood??")
                }, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(ValidateWeight), userDetails, cancellationToken);
            }

        }

        private bool CheckHeight(string input, out double height, out string message)
        {
            height = 0;
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
                        height = double.Parse(Va);
                        height = Math.Round(height,2);
                        //weight = (double)x;
                        //var a = weight;
                        if (height > 0)
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


        private async Task<DialogTurnResult> BMIAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your height is {userDetails.Height}m."), cancellationToken);
            double weight = userDetails.Weight;
            double height = userDetails.Height;
            double BMI = (double)weight / (height * height);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your BMI index is : {BMI}."), cancellationToken);
          
            if(BMI <18.5)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your BMI index < 18.5.You should review your diet, or eat more diligently."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You should choose a meal package containing a lot of kcal, the WEIGHT AGAIN meal plan is a suitable solution for you."), cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(ThinConsultingDialog), userDetails, cancellationToken);
            }
            else if( BMI>=18.5 && BMI <24.9 )
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your BMI index from 18.5 to 24.Please continue to maintain this BMI index."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"It's good for your healthy.We have a meal plan  FIGHTING to help you maintain this great index."), cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(NormalConsultingDialog), userDetails, cancellationToken);
            }
            else if(BMI >=25 && BMI <29.9)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your BMI index from 25 to 29.9.You are overweight, eat less and exercise more."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You should choose a low-calorie diet,  a STRONG meal plan to reduce your index fastest."), cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(FatConsultingDialog), userDetails, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your BMI index greater than 30. You are obese, your body is very overweight."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You should reduce your eating and work hard and exercise more.When you are excessively overweight, you need to follow a strict low-calorie diet. Choose a STRONG meal plan and exercise a lot.Fighting!!!"), cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(FatConsultingDialog), userDetails, cancellationToken);
            }

           
            if ((bool)stepContext.Result)
            {
                return await stepContext.BeginDialogAsync(nameof(CreateMoreTaskDialog), userDetails, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(userDetails, cancellationToken);
            }
            return null;

        }

        // Send all the tasks provided back to the user.
        private async Task<DialogTurnResult> SumaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for ask bot.The bot can give you nutrition advice at any time."), cancellationToken);
            // thực hiện việc trả ra các task mà user nhập vào
            //for (int i = 0; i < userDetails.TasksList.Count; i++)
            //{
            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(userDetails.TasksList[i]), cancellationToken);
            //}

            return await stepContext.EndDialogAsync(userDetails, cancellationToken);
        }

    }
}

