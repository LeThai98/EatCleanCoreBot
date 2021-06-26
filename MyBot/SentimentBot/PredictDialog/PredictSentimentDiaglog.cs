using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using MyBot.Models;
using MyBot.SentimentBot.SaveInfor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBot.SentimentBot
{

    public class PredictSentimentDiaglog : ComponentDialog
    {
        VegafoodBotContext context;
        public VegafoodBotContext Context { get { return context; } }

        public PredictSentimentDiaglog() : base(nameof(PredictSentimentDiaglog))
        {
            context = new VegafoodBotContext();
            var waterfallSteps = new WaterfallStep[]
            {
                AskNameAsync,
                AskEmailAsync,
                AskVegafoodAsync,
                AskServiceAsync,
               AsFoodAsync,
                SumaryStepAsync

            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> AskNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Hello and welcome.What is your name?")
            }, cancellationToken);
        }

        private async  Task<DialogTurnResult> AskEmailAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Sentiment.NameByUser = (string)stepContext.Result;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text($"Hello {Sentiment.NameByUser}.What is your email?")
            }, cancellationToken);

        }

        private async Task<DialogTurnResult> AskVegafoodAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Sentiment.Email = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Please let us know how you feel about Vegafood, so that we can serve you better."), cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text($"Hey {Sentiment.NameByUser},How do you feel about Vegafood ???")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AskServiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Sentiment.VegaComment = (string)stepContext.Result;

            var predict = new PredictSentiment(Sentiment.VegaComment);
            string result = predict.Predict();
            Sentiment.VegaPredict = result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your sentiment is: {result}"), cancellationToken);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text($"Ok, How do you feel about Vegafood's service ?")
            }, cancellationToken);

        }
        private async Task<DialogTurnResult> AsFoodAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Sentiment.ServiceComment = (string)stepContext.Result;

            var predict = new PredictSentiment(Sentiment.VegaComment);
            string result = predict.Predict();
            Sentiment.ServicePredict = result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your sentiment is: {result}"), cancellationToken);

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text($"Yup, How do you feel about food of Vegafood?")
            }, cancellationToken);
        }
        private async Task<DialogTurnResult> SumaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;

            Sentiment.FoodComment = (string)stepContext.Result;

            var predict = new PredictSentiment(Sentiment.VegaComment);
            string result = predict.Predict();
            Sentiment.FoodPredict = result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your sentiment is: {result}"), cancellationToken);

            CustomerSentiment cusen = new CustomerSentiment();
            cusen.NameByUser = Sentiment.NameByUser;
            cusen.Time = Sentiment.Time;
            cusen.VegaPredict = Sentiment.VegaPredict;
            cusen.VegaComment = Sentiment.VegaComment;
            cusen.FoodComment = Sentiment.FoodComment;
            cusen.FoodPredict = Sentiment.FoodPredict;
            cusen.ServiceComment = Sentiment.ServiceComment;
            cusen.ServicePredict = Sentiment.ServicePredict;
            cusen.Email = Sentiment.Email;
            //FindInfor(cusen, Sentiment.Email);  

            TakeSentiment(cusen);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text( $"This is Vegafood, We are very happy to receive your feedback.Thank you so much.!!!"), cancellationToken);

            return await stepContext.EndDialogAsync(userDetails, cancellationToken);

        }
        //method dùng để lưu thông tin sentiment of user into DB
         public async Task TakeSentiment(CustomerSentiment userSentiment)
        {
            var x = userSentiment.VegaComment;
            var ten = userSentiment.CustomerName;

            Users u = context.Users.Where(user => user.Email == userSentiment.Email).FirstOrDefault();
            if( u != null)
            {
                userSentiment.CustomerName = u.Name;
                userSentiment.Phone = u.PhoneNumber;
            }    
            context.CustomerSentiment.Add(userSentiment);
            await context.SaveChangesAsync();
        }


    }

    //public class PredictSentimentDiaglog : ActivityHandler
    //{
    //    VegafoodBotContext context;
    //    public VegafoodBotContext Context { get { return context; } }

    //    private readonly BotState _userState;

    //    private readonly BotState _conversationState;

    //    protected readonly int ExpireAfterSeconds;
    //    protected readonly IStatePropertyAccessor<DateTime> LastAccessedTimeProperty;
    //    public PredictSentimentDiaglog(UserState userState, ConversationState conversationState): base(nameof(DeleteTaskDialog))
    //    {
    //        _conversationState = conversationState;
    //        _userState = userState;
    //        context = new VegafoodBotContext();
    //    }
    //    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    //    {

    //        var welcomeText = "Hello and welcome! What's your name ?";
    //        await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
    //    }

    //    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    //    {
    //        // get state property
    //        var UserAccessor = _userState.CreateProperty<UserProfile>("UserFrofile");
    //        // gan cai state
    //        var Profile = await UserAccessor.GetAsync(turnContext, () => new UserProfile(), cancellationToken);

    //        var ConversationAccessor = _conversationState.CreateProperty<ConversationFlow>("ConversationFlow");
    //        var flow = await ConversationAccessor.GetAsync(turnContext, () => new ConversationFlow(), cancellationToken);
    //        await HandlerTurn(flow, Profile, turnContext, cancellationToken);
    //        await _userState.SaveChangesAsync(turnContext, false, cancellationToken);

    //        await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);

    //    }

    //    public async Task HandlerTurn(ConversationFlow flow, UserProfile profile, ITurnContext turnContext, CancellationToken cancellationToken)
    //    {

    //        var input = turnContext.Activity.Text?.Trim();
    //        if (input == "Nutrition Consulting")
    //            flow.UserChoose = ConversationFlow.Choose.Consulting;
    //        if (input == "Vegafood's Product")
    //            flow.UserChoose = ConversationFlow.Choose.Product;
    //        string message;
    //        var cal = ConversationFlow.BMI.Weight;

    //        switch (flow.LastQuestionAsked)
    //        {
    //            case ConversationFlow.Question.Name:
    //                if (ValidateName(input, out var name, out message))
    //                {
    //                    // property Name of profile state is name that uset typed 
    //                    profile.Name = name;

    //                    // use static class to save info, because it had not deleted when run class many times
    //                    Sentiment.NameByUser = name;

    //                    await turnContext.SendActivityAsync($"Hi {profile.Name}.", null, null, cancellationToken);

    //                    await turnContext.SendActivityAsync($"What is your email?");
    //                    flow.LastQuestionAsked = ConversationFlow.Question.Email;

    //                    break;

    //                }
    //                else
    //                {
    //                    // if validation return is false 
    //                    await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
    //                    break;
    //                }
    //                break;

    //            case ConversationFlow.Question.Email:

    //                if (ValidateEmail(input, out var email, out message))
    //                {
    //                    // use static class to save info, because it had not deleted when run class many times
    //                    Sentiment.Email = email;
    //                    await turnContext.SendActivityAsync($"Have you used Vegafood products before??");
    //                   // SendSuggestedActionsAsync(turnContext, cancellationToken);
    //                    flow.LastQuestionAsked = ConversationFlow.Question.Choose;
    //                    flow.UserChoose = ConversationFlow.Choose.Product;
    //                    break;
    //                }
    //                else
    //                {
    //                    // if validation return is false 
    //                    await turnContext.SendActivityAsync(message ?? "I'm sorry, I didn't understand that.", null, null, cancellationToken);
    //                    break;
    //                }
    //                break;

    //            case ConversationFlow.Question.Choose:
    //                if (flow.UserChoose == ConversationFlow.Choose.Product)
    //                {
    //                    if (flow.UserEmotion == ConversationFlow.Emotion.None)
    //                    {
    //                        await turnContext.SendActivityAsync($"How do you feel about Vegafood ???");
    //                        flow.UserEmotion = ConversationFlow.Emotion.Service;

    //                    }
    //                    else if (flow.UserEmotion == ConversationFlow.Emotion.Service)
    //                    {
    //                        var predict = new PredictSentiment(input);
    //                        string result = predict.Predict();
    //                        Sentiment.VegaPredict = result;
    //                        Sentiment.VegaComment = input;
    //                        await turnContext.SendActivityAsync($"Your sentiment is: {result}");
    //                        await turnContext.SendActivityAsync($"How do you feel about Vegafood's service ?");
    //                        flow.UserEmotion = ConversationFlow.Emotion.Product;
    //                    }
    //                    else if (flow.UserEmotion == ConversationFlow.Emotion.Product)
    //                    {
    //                        var predict = new PredictSentiment(input);
    //                        string result = predict.Predict();
    //                        Sentiment.ServicePredict = result;
    //                        Sentiment.ServiceComment = input;
    //                        await turnContext.SendActivityAsync($"Your sentiment about our        service is: {result}");
    //                        await turnContext.SendActivityAsync($"How do you feel about food      of Vegafood ?");
    //                        flow.UserEmotion = ConversationFlow.Emotion.End;

    //                    }
    //                    else if (flow.UserEmotion == ConversationFlow.Emotion.End)
    //                    {
    //                        var x = Sentiment.End;
    //                        if (Sentiment.End)
    //                        {
    //                            var predict = new PredictSentiment(input);
    //                            string result = predict.Predict();
    //                            Sentiment.FoodPredict = result;
    //                            Sentiment.FoodComment = input;
    //                            await turnContext.SendActivityAsync($"Your sentiment about our food is: {result}");
    //                            await turnContext.SendActivityAsync($"Thanks for your respond about Vegafood !!!.");
    //                            if (Sentiment.Check)
    //                            {
    //                                CustomerSentiment cusen = new CustomerSentiment();
    //                                cusen.CustomerName = Sentiment.NameByUser;
    //                                cusen.Time = Sentiment.Time;
    //                                cusen.VegaPredict = Sentiment.VegaPredict;
    //                                cusen.VegaComment = Sentiment.VegaComment;
    //                                cusen.FoodComment = Sentiment.FoodComment;
    //                                cusen.FoodPredict = Sentiment.FoodPredict;
    //                                cusen.ServiceComment = Sentiment.ServiceComment;
    //                                cusen.ServicePredict = Sentiment.ServicePredict;
    //                                cusen.Email = Sentiment.Email;
    //                                //FindInfor(cusen, Sentiment.Email);  

    //                                TakeSentiment(cusen);
    //                                Sentiment.Check = false;

    //                            }
    //                            Sentiment.End = false;
    //                        }
    //                        else
    //                        {
    //                            await turnContext.SendActivityAsync($"This is Vegafood, We             are very happy to receive your feedback.");
    //                        }
    //                        break;

    //                    }
    //                    else
    //                        await turnContext.SendActivityAsync($"Thank you very much.");

    //                }
    //                break;

    //        }
    //    }

    //    private bool ValidateName(string input, out string name, out string message)
    //    {
    //        name = null;
    //        message = null;

    //        if (string.IsNullOrWhiteSpace(input))
    //        {
    //            message = "Please enter a name that contains at least one character.";
    //        }
    //        else
    //        {
    //            name = input.Trim();
    //        }

    //        return message is null;
    //    }

    //    private bool ValidateEmail(string input, out string email, out string message)
    //    {
    //        email = null;
    //        message = null;

    //        if (string.IsNullOrWhiteSpace(input))
    //        {
    //            message = "Please enter a name that contains at least one character.";
    //        }
    //        else
    //        {
    //            email = input.Trim();
    //        }

    //        return message is null;
    //    }

    //    private async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    //    {
    //        var reply = MessageFactory.Text("What can i help you ?");

    //        // Type = ActionTypes.ImBack,Image = "https://via.placeholder.com/20/FF0000?text=R", ImageAltText = "R" 
    //        reply.SuggestedActions = new SuggestedActions()
    //        {
    //            Actions = new List<CardAction>()
    //                {

    //                    new CardAction() { Title = "Nutrition Consulting",Type = ActionTypes.ImBack, Value = "Nutrition Consulting", },
    //                    new CardAction() { Title = "Vegafood's Product", Type = ActionTypes.ImBack, Value = "Vegafood's Product", Image = "https://via.placeholder.com/20/FFFF00?text=Y", ImageAltText = "Y" },
    //                    //new CardAction() { Title = "Blue", Type = ActionTypes.ImBack, Value = "Blue", Image = "https://via.placeholder.com/20/0000FF?text=B", ImageAltText = "B"   },
    //                },
    //        };
    //        await turnContext.SendActivityAsync(reply, cancellationToken);

    //    }

    //    // method dùng để lưu thông tin sentiment of user into DB
    //    public async Task TakeSentiment(CustomerSentiment userSentiment)
    //    {
    //        var x = userSentiment.VegaComment;
    //        var ten = userSentiment.CustomerName;
    //        context.CustomerSentiment.Add(userSentiment);
    //        await context.SaveChangesAsync();
    //    }


    //}
}
