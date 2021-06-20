using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBot.Dialogs.Operations
{
    public class CreateTaskDialog: ComponentDialog
    {
        public CreateTaskDialog() : base(nameof(CreateTaskDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                TasksStepAsync,
                ActStepAsync,
                MoreTaskStepAsync,
                SumaryStepAsync

            };

            // WaterfallDialog có các bước dc khởi tạo trong biến waterfallSteps
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new CreateMoreTaskDialog());

            // lựa chọn Diaglog initial 
            InitialDialogId = nameof(WaterfallDialog);
        }

        // This will be Text Prompt asking the user to provide the task.
        private async Task<DialogTurnResult> TasksStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("How many kg do you weigh?") 
            }, cancellationToken);
        }

        // Capture the user response from TasksStepAsync and ask the user if he/she wants to add more tasks. This will be a Confirm Prompt that shows the user two buttons (Yes and No). The value returned by these buttons is of type boolean.
        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
            stepContext.Values["Task"] = (string)stepContext.Result;
            userDetails.TasksList.Add((string)stepContext.Values["Task"]);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Would you like to Add more tasks?")
            }, cancellationToken);
        }

        //Capture the boolean response. If Yes, begin another dialog CreateMoreTaskDialog.cs (Create a new class file with a constructor similar to CreateTaskDialog.) If No, move to the next step.
        private async Task<DialogTurnResult> MoreTaskStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Options;
            if ((bool)stepContext.Result)
            {
                return await stepContext.BeginDialogAsync(nameof(CreateMoreTaskDialog), userDetails, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(userDetails, cancellationToken);
            }
        }

        // Send all the tasks provided back to the user.
        private async Task<DialogTurnResult> SumaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userDetails = (User)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Here are the tasks you provided - "), cancellationToken);
            // thực hiện việc trả ra các task mà user nhập vào
            for (int i = 0; i < userDetails.TasksList.Count; i++)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(userDetails.TasksList[i]), cancellationToken);
            }

            return await stepContext.EndDialogAsync(userDetails, cancellationToken);
        }

    }
}

