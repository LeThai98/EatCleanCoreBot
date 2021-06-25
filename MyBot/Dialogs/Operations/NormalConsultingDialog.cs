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
    public class NormalConsultingDialog: ComponentDialog
    {
        public NormalConsultingDialog() : base(nameof(NormalConsultingDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
               AskEercise,
               DoEercise,
               TDEE,
               NutrientAbsorption,
               RulesKeep,
               FoodsKeep,
               Advice,
               AskDiseases,
               Finish
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> AskEercise(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you do any exercise regime: gym, jogging, swimming, ..?")
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
                MessageFactory.Text($"Your TDEE index is:{tdee} calo.This is a great indicator that many people want .If you want to keep your good index, you need to eat equal calories this."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
            MessageFactory.Text($"When you maintain your good index, you alway have good body."), cancellationToken);
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
                MessageFactory.Text($"Gastrointestinal diseases are a major reason why you can't make the right diet for you, treat these diseases first and then follow the desired diet."), cancellationToken);
            }
            else
                await stepContext.Context.SendActivityAsync(
               MessageFactory.Text($"This is a great way to start working on the diet and exercise that's right for you without the first hitch."), cancellationToken);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you know the rules to help you gain weight easily?")
            }, cancellationToken);
        }


        private async Task<DialogTurnResult> RulesKeep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("1. Using fats from plants: fats are divided into 2 types: animal fats and vegetable fats. Therefore, when eating, you should only eliminate foods belonging to the animal fat group, and still add vegetable fat foods to the menu every day. Some foods containing plant fats that you can add to your belly fat reduction diet such as: soybean oil, almonds, walnuts, tuna, nuts ... "), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("2. Limit salt intake :To get the best results from your belly fat reduction diet menu, you should pay attention to eating less salt every day. Limit salt intake because this is the reason why the body's ability to retain water is higher. Therefore, pay attention to control the amount of salt you consume in your body every day, especially for those who have a habit of eating salty foods. "), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("3. Always eat a lot of green vegetables :Green vegetables and fruits are considered healthy foods for the body, because green vegetables contain a lot of fiber and antioxidants. Some green vegetables that you should focus on in meals such as: cauliflower, spinach, vegetables, tomatoes... "), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("4. Do not drink soft drinks: According to nutrition studies, you should not drink soft drinks, because in soft drinks there are artificial substances that disrupt the body's ability to regulate calories, thereby making you easy gain more weight. Therefore, if you are not having a very slim waist, you should pay attention not to drink soft drinks."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("5. Always remember to drink a lot of water: You should not drink soft drinks, but always drink a lot of filtered water when following a diet to reduce belly fat. Because drinking water is especially important, water will help promote fat burning hydration and metabolism in the body, thereby helping to reduce fat significantly."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("6. Sleep is really important: The last rule when doing a belly fat loss diet process that you need to remember is -Take care of your sleep.According to many studies on nutrition for People who lack sleep often gain weight more easily than those with scientific sleep. So, pay attention to sleep enough 8 hours a day to ensure health and shape."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("6. Always remember to drink a lot of water: You should not drink soft drinks, but always drink a lot of filtered water when following a diet to reduce belly fat. Because drinking water is especially important, water will help promote fat burning hydration and metabolism in the body, thereby helping to reduce fat significantly."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("7:Eat multiple meals a day: small snacks will help you absorb nutrients better"), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("8:Exercise: Exercise will help you develop and maintain physical health and overall health. Regular exercise and sports is a very important factor in preventing diseases such as cardiovascular disease, coronary heart disease, type 2 diabetes, obesity, stronger musculoskeletal system and joints. , enhances the activity of the immune system."), cancellationToken);

            }
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you know the foods that help you keep weight effectively?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> FoodsKeep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(
            MessageFactory.Text($"Yup, Bot want to give you some more knowledge about effective weight gain foods such as:"), cancellationToken);

            await stepContext.Context.SendActivityAsync(
            MessageFactory.Text($"1.Carbohydrate supplier group: rice, potato, corn, noodles, vermicelli, pho. Should eat whole grains to provide more fiber to prevent constipation because constipation will affect the skin. Amount to eat a day: about 300g of rice, if you eat other types, you have to reduce rice."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"2. Protein supply group: should eat: lean meat, lean fish, shrimp, crab, fish, eggs, milk... Every day should only eat about 150g of meat (fish, shrimp); eggs 1 week 3-4 eggs; milk should drink 400-500ml/day, choose low-sugar light milk, low-fat powdered milk."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"3.Group providing fat: oil or fat should only eat about 20g per day."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
            MessageFactory.Text($"4.Group providing vitamins and minerals: should eat a lot of green vegetables and fresh fruits, every day should eat about 300-400g of green vegetables and 400-500g of ripe fruit, choose a less sweet ripe fruit: grapefruit, orange, pear, apple, bar dragon fruit, cucumber."), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("5.Drinking water: should drink mineral water or fresh fruit juice without sugar, every day should drink from 2 to 2.5 liters of water."), cancellationToken);
            await stepContext.Context.SendActivityAsync(
             MessageFactory.Text($"In addition, foods to limit or avoid:Processed food, fast food: Bâté, sausage, sausage, carbonated soft drinks; Fatty food: fatty meat, butter, cheese,.."), cancellationToken);
          

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Change the use of food to help keep your good body.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> Advice( WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Here's the bot's advice for you: You should realize that you have a great body, investing in your health is a wise thing to do. Take care of yourself to always have a better life.")
            }, cancellationToken);

        }


        
        private async  Task<DialogTurnResult> AskDiseases(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
