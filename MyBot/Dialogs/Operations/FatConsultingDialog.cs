using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBot.Dialogs.Operations
{
    public class FatConsultingDialog: ComponentDialog
    {
        public FatConsultingDialog() : base(nameof(FatConsultingDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
               AskIndex,
               AskCravings,
               DoEercise,
               TDEE,
               NutrientAbsorption,
               RulesLose,
               FoodsGain,
               Advice,
               AskDiseases,
               Finish

            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        

        private async Task<DialogTurnResult> AskIndex(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Bot want to ask you some question to know more about you body. Your BMI index is from 25 to 29.9, it is not good for you.Start losing weight to help your body get back into shape."), cancellationToken);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you often use sweets (soft drinks, cookies, ..)?")
            }, cancellationToken);

        }

        private async Task<DialogTurnResult> AskCravings(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you often have cravings?")
            }, cancellationToken);
        }

        private async  Task<DialogTurnResult> DoEercise(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("This is a sign that you have no control over how much food you eat. Divide your meals into several meals, which will help you reduce cravings better."), cancellationToken);
               
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Do you exercise a lot during the day?"), cancellationToken);
                List<string> operationList = new List<string> {
                "Sedentary (People who only eat, sleep, do office work)",
                "Light exercise (People who exercise 1-3 times a week)",
                "Moderately active (People who exercise daily, exercise 3-5 times a week)",
                "Heavy exercise (People who exercise regularly, play sports and exercise 6-7 times a week)",
                "Very heavy exercise (Unskilled workers, exercise twice a day)"
            };
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
                     Prompt = (Microsoft.Bot.Schema.Activity)MessageFactory.Attachment(new Attachment
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
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Do you exercise a lot during the day?"), cancellationToken);
                List<string> operationList = new List<string> {
                "Sedentary (People who only eat, sleep, do office work)",
                "Light exercise (People who exercise 1-3 times a week)",
                "Moderately active (People who exercise daily, exercise 3-5 times a week)",
                "Heavy exercise (People who exercise regularly, play sports and exercise 6-7 times a week)",
                "Very heavy exercise (Unskilled workers, exercise twice a day)"
                    };
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
                     Prompt = (Microsoft.Bot.Schema.Activity)MessageFactory.Attachment(new Attachment
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
        }

        private async  Task<DialogTurnResult> TDEE(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
            string operation = (string)stepContext.Values["Operation"];

            var userDetails = (User)stepContext.Options;

            double tdee = 0;
            double R = 0;
            double N = userDetails.Weight;
            double C = userDetails.Height * 100;
            double T = userDetails.Age;
            double brr = 0;
            if (operation == "Sedentary (People who only eat, sleep, do office work).")
            {
                R = 1.2;

            }
            else if (operation == "Light exercise (People who exercise 1-3 times a week).")
            {
                R = 1.375;

            }
            else if (operation == "Moderately active (People who exercise daily, exercise 3-5 times a week).")
            {
                R = 1.55;
            }
            else if (operation == "Heavy exercise (People who exercise regularly, play sports and exercise 6-7 times a week).")
            {
                R = 1.725;
            }
            else
            {
                R = 1.9;
            }

            if (userDetails.Sex == "Male")
            {
                brr = (9.99 * N) + (6.25 * C) - (4.92 * T) + 5;
                tdee = brr * R;
            }
            else
            {
                brr = (9.99 * N) + (6.25 * C) - (4.92 * T) - 161;
                tdee = brr * R;
            }
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"Your TDEE index is:{tdee} calo.If you want to lose weight, you need to eat less calories than this."), cancellationToken);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Can you tell to Bot, Do you have a number of digestive diseases that interfere with nutrient absorption: stomach pain, digestive disorders, ...?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> NutrientAbsorption(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"There are cases where the body is difficult to lose weight due to physical reasons or diseases, so even if you eat a lot and enough nutrients, it will not improve your body weight. You need to treat these diseases before you want to lose weight."), cancellationToken);
            }
            else
                await stepContext.Context.SendActivityAsync(
               MessageFactory.Text($"This is a great way to start losing weight without the first setbacks."), cancellationToken);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you know the rules to help you lose weight easily?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> RulesLose(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if ((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(
               MessageFactory.Text($"Those are useful knowledge, you should test them to get its effectiveness."), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"No problem, now the bot will give you some knowledge of the principles of weight lose!!!"), cancellationToken);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"1. Eat enough meals: the right diet plays a key role, more decisive than choosing the method of body movement. However, you should also not skip meals, but instead divide your meals, replacing starchy foods with green vegetables and fruits. "), cancellationToken);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"2. Limit junk food: If you record what you eat to calculate the amount of calories burned during the day, you will see how dangerous it is to eat junk food."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"3. Use 5 servings of fruits and vegetables per day: When you determine to lose weight fast, it means that green vegetables are a comrade on all fronts. Eating a lot of vegetables is very good for the safe weight loss process."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"4. Avoid fried and greasy foods: Absolutely avoid dishes such as: fried, roasted, skinned, fried, fried onions, onion fat, ... because they are the enemy of effective weight loss methods. If you can control yourself, you should only try 1 piece and only 1. "), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"5.Drink a lot of water. In addition to green vegetables, water is your reliable companion.Drink a lot of water, every day should drink from 3 - 4 liters of filtered water is not only good for your body but also good for your weight loss."), cancellationToken);   
            }
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you know the foods that help you lose weight effectively?")
            }, cancellationToken);

        }
        private async Task<DialogTurnResult> FoodsGain(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"Yup, Bot want to give you some more knowledge about effective weight gain foods such as:"), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"1. Lemon: Contains a lot of acid and helps the body absorb calcium easily, lemon is a panacea for people with beauty needs. Lemon helps to purify the body, burn excess fat quickly, detoxify the liver, effectively filter the blood. You can drink diluted lemon juice instead of filtered water daily. In addition, lemon juice mixed with honey and warm water is also very good for weight loss goals. "), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"2.Tofu: is a dish not to be missed in the weight loss menu. In tofu contains a lot of vegetable protein, so it helps you feel full for a long time, and at the same time, melts excess fat in your abdomen. You can eat tofu instead of the main dishes in the meal to speed up the process of slimming down your body! "), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"3.Oats: You can use oats as a menu for meals throughout the day. Many people, especially men, do not like the pale taste of oats, but that is also the reason to help you lose weight more effectively because you will not be afraid of losing control and overeating. "), cancellationToken);
            await stepContext.Context.SendActivityAsync(
            MessageFactory.Text($"4.Whole grains: have long been considered a food containing a lot of nutrients such as fiber and essential minerals for the body. Cereals not only help you feel full longer, but also aid digestion, regulate blood sugar and lower cholesterol in the body! "), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("5.Green tea: Due to its high content of antioxidants, vitamin E, green tea has the effect of burning fat, purifying the body, and preventing disease."), cancellationToken);
            

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Change the use of food to help improve your body fastest.")
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> Advice(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"An effective procedure you can follow to achieve weight loss:"), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"1.Determine the amount of calories that the body needs each day according to the TDEE index."), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"2. Adhere to a scientific nutrition regimen, monitor calorie intake."), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"3. Exercise at least 60 minutes, 4-5 days/week."), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"4.Do cardio exercises to lose fat 2-3 days/week."), cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"5.Persistently pursuing a weight loss regimen."), cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text($"To lose weight, keep calorie intake < calories burned! This can be achieved by adjusting a scientific diet, reducing calorie intake and adjusting the rest and exercise regimen.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AskDiseases(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("Do you suffer from any of the following diseases?"), cancellationToken);

            List<string> operationList = new List<string> { "Heart", "Diabetes", "Digestive Diseases", "Hight Blood Pressure", "No Disease" };
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
                 Prompt = (Microsoft.Bot.Schema.Activity)MessageFactory.Attachment(new Attachment
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
        private async Task<DialogTurnResult> Finish(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
            string operation = (string)stepContext.Values["Operation"];

            var userDetails = (User)stepContext.Options;
            userDetails.Diseases = operation;
            if (userDetails.Diseases == "Heart")
            {
                await stepContext.Context.SendActivityAsync("If you have heart disease: limit salt, increase fiber-rich foods, limit fat and do not use alcohol, beer, and tobacco,..");
            }
            else if (userDetails.Diseases == "Diabetes")
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("When you have diabetes, you should eat the following foods:"), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Green vegetables: Green vegetables are the first foods that need to be added to the list of what to eat with diabetes. It should be noted that instead of using fatty sauces or condiments to process vegetables, choose to eat raw or steamed, boiled, mixed vegetables for optimal blood sugar lowering effect."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Use good fats: Fats found in plants such as avocado, olive oil, in nuts such as sunflower seeds, pumpkin seeds ... are very useful for diabetics because they will reduce fat levels. in blood. They should be used as an alternative to animal fat sources."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Regularly eat fish: Fish is also a highly recommended food to add to the menu of diabetics."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Choose white meat: White meat is good for people with diabetes and people with cardiovascular disease. In addition to fish, people with diabetes should choose to eat white meat such as chicken. Do not eat a lot of red meat (such as pork, beef, etc.). Do not eat skin, organs."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Besides the diabetic menu should eat, you also need to stay away from specific foods: canned food, fried food, sugary foods, alcoholic drinks, stimulants "), cancellationToken);
            }
            else if (userDetails.Diseases == "Digestive Diseases")
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("When you have digestive diseases, you should: limit greasy foods, don't eat raw foods, limit foods high in sugar, limit the use of milk, don't drink alcohol, beer, stimulants."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Instead, you should use: Should eat white meat (chicken, fish, ..), Add fruit rich in vitamin C, should eat yogurt,.."), cancellationToken);
            }
            else if (userDetails.Diseases == "Hight Blood Pressure")
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("For patients with high blood pressure, the daily diet should ensure ingredients such as: protein, fat, carbohydrates, fiber, foods rich in calcium, magnesium, potassium, fish should be added. ….Each of these ingredients needs to ensure the right dosage. Do not use too much, use too little, it can make the body lack nutrition, especially limit the use of salt. "), cancellationToken);
            }
            return await stepContext.EndDialogAsync(userDetails, cancellationToken);
        }
    }

}
