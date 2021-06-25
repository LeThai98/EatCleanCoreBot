using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBot.Dialogs.Operations
{
    public class ThinConsultingDialog: ComponentDialog
    {
        public ThinConsultingDialog() : base(nameof(ThinConsultingDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                AskIndex,
               AskEat,
               DoEercise,
               TDEE,
               NutrientAbsorption,
               RulesGain,
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
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("How often do you care about your body measurements?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AskEat(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Good job,paying attention to body indexes is a factor that helps you maintain a healthy body."), cancellationToken);
            }
            else
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Take care of your body mass index, this will help you maintain your current good BMI."), cancellationToken);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("According to you, do you eat less during the day?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> DoEercise(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

        private async Task<DialogTurnResult> TDEE(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
            string operation = (string)stepContext.Values["Operation"];

            var userDetails = (User)stepContext.Options;
            
            double tdee = 0;
            double R = 0;
            double N = userDetails.Weight;
            double C = userDetails.Height*100;
            double T = userDetails.Age;
            double brr = 0;
            if(operation == "Sedentary (People who only eat, sleep, do office work).")
            {
                R = 1.2;
                
            }
            else if( operation == "Light exercise (People who exercise 1-3 times a week).")
            {
                R = 1.375;
               
            }
            else if( operation == "Moderately active (People who exercise daily, exercise 3-5 times a week).")
            {
                R = 1.55;
            }
            else if(operation == "Heavy exercise (People who exercise regularly, play sports and exercise 6-7 times a week).")
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
                MessageFactory.Text($"Your TDEE index is:{tdee} calo.If you want to gain weight, you need to eat more calories than this."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
            MessageFactory.Text($"When you maintain a higher calorie intake than this number, you will quickly gain weight. "), cancellationToken);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Can you tell to Bot, Do you have a number of digestive diseases that interfere with nutrient absorption: stomach pain, digestive disorders, ...?")
            }, cancellationToken);

            

        }
        private async Task<DialogTurnResult> NutrientAbsorption(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"There are cases where the body is difficult to gain weight due to physical reasons or diseases, so even if you eat a lot and enough nutrients, it will not improve your body weight. You need to treat these diseases before you want to lose weight."), cancellationToken);
            } 
            else
                await stepContext.Context.SendActivityAsync(
               MessageFactory.Text($"This is a great way to start gaining weight without the first setbacks."), cancellationToken);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you know the rules to help you gain weight easily?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> RulesGain(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(
               MessageFactory.Text($"Those are useful knowledge, you should test them to get its effectiveness."), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(
              MessageFactory.Text($"No problem, now the bot will give you some knowledge of the principles of weight gain!!!"), cancellationToken);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("1.Calorie consumption is lower than calorie intake: Thin people need to eat foods with higher calories than their tdee number to gain weight easily. "), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("2.Eat several meals a day: Snacks will help the body receive nutrients continuously and fully."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("3.Combine exercise: Every day we should spend about 30 minutes to exercise to help the body absorb the nutrients in food most effectively."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("4.Get enough sleep: Getting 7-8 hours of sleep a day is the best thing to help the body grow."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("5.Add nutritious foods: choose clean foods such as meat, fish, eggs, milk, butter, sweet potatoes ... and limit foods with too many calories including milk tea, instant noodles..."), cancellationToken);

            }
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you know the foods that help you gain weight effectively?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> FoodsGain(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"Yup, Bot want to give you some more knowledge about effective weight gain foods such as:"), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"1.Rice: Contains little fat, provides plenty of carbohydrates. 1 bowl of rice contains about 4.5g of protein and 206 calories. Therefore, rice helps to gain weight very well."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"2.Chicken breast: In 100g of chicken meat contains 165 calories. Chicken contains a lot of calories and protein, does not accumulate fat in the body, helping skinny people gain weight effectively. At the same time, chicken also has a high content of tryptophan - which helps to increase the amount of serotonin in the brain, effectively reducing stress."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"3.Eggs: An average egg contains 90 calories. This is a food that helps build muscle and gain weight effectively. In addition, eggs are rich in selenium and vitamin D. Selenium helps improve the body's metabolism and vitamin D improves the immune system and keeps bones and teeth strong."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
            MessageFactory.Text($"4.Beef: In 100g of lean beef, there are 332 calories, making it easier for skinny people to gain weight. In particular, beef also has a natural amount of creatinine, which enhances protein synthesis for cells to improve body weight quickly."), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("5.Milk: 1 cup of milk can contain 122 calories. Milk is high in protein, fat, good calcium, vitamins and minerals. At the same time, milk is also high in casein and whey proteins - proteins that are good for the body. Drinking milk helps to improve physique, gain weight quickly "), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"6.Pork: In 100g of pork contains up to 548 calories, so this is a good food for those who want to gain weight fast. However, pork has a lot of cholesterol, so it is best to choose lean meat to gain healthy weight and have the desired physique."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"7.Other foods: Honey (1 scoop contains 64 calories), avocado (1 cup contains 240 calories), peanut butter (100g contains 589 calories), cheese (100g has 353 calories), chocolate (100g has 589 calories). 600 calories), potatoes and starch (oats, corn, squash, beans, ...), dried fruits are all good food groups for thin people who want to gain weight fast and healthy."), cancellationToken);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Change the use of food to help improve your body fastest.")
            }, cancellationToken);

        }

        private async Task<DialogTurnResult> Advice(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Here's the bot's advice for you: Weight gain is a process. Therefore, thin people who want to gain weight safely to improve their physique should combine eating scientifically, maintaining a healthy lifestyle and performing appropriate exercises. ")
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

        private async Task<DialogTurnResult> Finish(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
            string operation = (string)stepContext.Values["Operation"];

            var userDetails = (User)stepContext.Options;
            userDetails.Diseases = operation;
            if(userDetails.Diseases == "Heart")
            {
                await stepContext.Context.SendActivityAsync("If you have heart disease: limit salt, increase fiber-rich foods, limit fat and do not use alcohol, beer, and tobacco,..");
            }
            else if(userDetails.Diseases == "Diabetes")
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("When you have diabetes, you should eat the following foods:"), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Green vegetables: Green vegetables are the first foods that need to be added to the list of what to eat with diabetes. It should be noted that instead of using fatty sauces or condiments to process vegetables, choose to eat raw or steamed, boiled, mixed vegetables for optimal blood sugar lowering effect."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Use good fats: Fats found in plants such as avocado, olive oil, in nuts such as sunflower seeds, pumpkin seeds ... are very useful for diabetics because they will reduce fat levels. in blood. They should be used as an alternative to animal fat sources."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Regularly eat fish: Fish is also a highly recommended food to add to the menu of diabetics."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Choose white meat: White meat is good for people with diabetes and people with cardiovascular disease. In addition to fish, people with diabetes should choose to eat white meat such as chicken. Do not eat a lot of red meat (such as pork, beef, etc.). Do not eat skin, organs."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Besides the diabetic menu should eat, you also need to stay away from specific foods: canned food, fried food, sugary foods, alcoholic drinks, stimulants "), cancellationToken);
            }
            else if(userDetails.Diseases == "Digestive Diseases")
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("When you have digestive diseases, you should: limit greasy foods, don't eat raw foods, limit foods high in sugar, limit the use of milk, don't drink alcohol, beer, stimulants."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Instead, you should use: Should eat white meat (chicken, fish, ..), Add fruit rich in vitamin C, should eat yogurt,.."), cancellationToken);
            }
            else if(userDetails.Diseases == "Hight Blood Pressure")
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("For patients with high blood pressure, the daily diet should ensure ingredients such as: protein, fat, carbohydrates, fiber, foods rich in calcium, magnesium, potassium, fish should be added. ….Each of these ingredients needs to ensure the right dosage. Do not use too much, use too little, it can make the body lack nutrition, especially limit the use of salt. "), cancellationToken);
            }
            return await stepContext.EndDialogAsync(userDetails, cancellationToken);
        }
    }
}
